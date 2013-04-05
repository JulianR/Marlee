using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;
using Marlee.Common.Deserialization;
using Marlee.Internal;

namespace Marlee.Common.Tree
{
  internal class TreeBuilder : IKnownTypeProvider
  {
    private Stack<Type> _recursionStack = new Stack<Type>();

    public RootNode CreateTree(Type t)
    {
      var node = new RootNode
      {
        Type = t
      };

      _recursionStack.Push(t);

      var members = GetMembers(t);

      node.Children = ProcessMembers(members);

      return node;
    }

    private List<Node> ProcessMembers(IList<PropertyOrFieldInfo> members)
    {
      var nodes = new List<Node>();
      foreach (var member in members)
      {
        var node = GetDeserializationNode(member);
        nodes.Add(node);
      }

      return nodes;
    }

    private static readonly ConcurrentDictionary<Type, Func<PropertyOrFieldInfo, Node>> _deserializers = new ConcurrentDictionary<Type, Func<PropertyOrFieldInfo, Node>>();
    private static readonly ConcurrentDictionary<Type, MethodInfo> _knownTypes = new ConcurrentDictionary<Type, MethodInfo>();
    private IConverter _converter;

    static TreeBuilder()
    {
      _deserializers[typeof(int)] = p => new IntegerNode { Member = p };
      _deserializers[typeof(long)] = p => new IntegerNode { Member = p };
      _deserializers[typeof(short)] = p => new IntegerNode { Member = p };
      _deserializers[typeof(byte)] = p => new IntegerNode { Member = p };
      _deserializers[typeof(uint)] = p => new IntegerNode { Member = p };
      _deserializers[typeof(ulong)] = p => new IntegerNode { Member = p };
      _deserializers[typeof(ushort)] = p => new IntegerNode { Member = p };
      _deserializers[typeof(sbyte)] = p => new IntegerNode { Member = p };
      _deserializers[typeof(double)] = p => new DecimalNode { Member = p };
      _deserializers[typeof(float)] = p => new DecimalNode { Member = p };
      _deserializers[typeof(decimal)] = p => new DecimalNode { Member = p };
      _deserializers[typeof(string)] = p => new StringNode { Member = p };
      _deserializers[typeof(ICollection<string>)] = p => new StringCollectionNode { Member = p };
      _deserializers[typeof(ICollection<int>)] = p => new IntCollectionNode { Member = p };
    }

    public TreeBuilder(IConverter _converter)
    {
      this._converter = _converter;
    }

    public Delegate TryGetKnownTypeDelegate(Type t)
    {
      MethodInfo knownTypeDeserializer;

      if (_knownTypes.TryGetValue(t, out knownTypeDeserializer))
      {
        var del = Delegate.CreateDelegate(typeof(DeserializeHandler<>).MakeGenericType(t), knownTypeDeserializer);
        return del;
      }

      return null;
    }

    internal void AddKnownType(Type t, Delegate del)
    {
      var method = del.Method;
      _deserializers[t] = (p) => new KnownTypeNode { Member = p, Type = t, Method = method };
      _knownTypes[t] = method;
      RecursionHelper.SetDeserializer(t, del);
    }

    private Node GetDeserializationNode(PropertyOrFieldInfo member)
    {
      Func<PropertyOrFieldInfo, Node> nodeFunc;

      if (!_deserializers.TryGetValue(member.PropertyOrFieldType, out nodeFunc))
      {
        var key = _deserializers.Keys.FirstOrDefault(k => k.IsAssignableFrom(member.PropertyOrFieldType));

        if (key != null)
        {
          nodeFunc = _deserializers[key];
        }
        else
        {
          key = _deserializers.Keys.FirstOrDefault(k => member.PropertyOrFieldType.IsAssignableFrom(k));

          if (key != null)
          {
            nodeFunc = _deserializers[key];
          }

        }

      }

      Node node;

      if (nodeFunc != null)
      {
        node = nodeFunc(member);
      }
      else if (_recursionStack.Contains(member.PropertyOrFieldType))
      {
        node = new RecursionNode
        {
          Type = member.PropertyOrFieldType,
          Member = member
        };
      }
      else
      {
        var customNode = new UnknownTypeNode
        {
          Type = member.PropertyOrFieldType,
          Member = member,
          Children = new List<Node>()
        };

        var members = GetMembers(member.PropertyOrFieldType);

        foreach (var childMember in members)
        {
          var childNode = GetDeserializationNode(childMember);
          customNode.Children.Add(childNode);
        }

        node = customNode;

      }

      return node;
    }

    private IList<PropertyOrFieldInfo> GetMembers(Type t)
    {
      var typeIsSerializable = t.GetCustomAttributes(typeof(SerializableAttribute), false).Any();

      var members = (from p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     where p.CanWrite && !p.GetIndexParameters().Any()
                     && typeIsSerializable || p.GetCustomAttributes(typeof(DataMemberAttribute), false).Any()
                     select (PropertyOrFieldInfo)p)
                    .Union(from f in t.GetFields()
                           where !f.IsStatic
                           && typeIsSerializable || f.GetCustomAttributes(typeof(DataMemberAttribute), false).Any()
                           select (PropertyOrFieldInfo)f).ToList();

      return members;

    }
  }
}
