using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Deserialization.Tree;
using System.Reflection;
using System.Runtime.Serialization;
using Marlee.Jsv.Deserialization;

namespace Marlee.Common.Deserialization
{
  internal class DeserializerFactory
  {
    public DeserializeHandler CreateDeserializer(Type t)
    {
      var root = ProcessType(t);
      return (DeserializeHandler)new CodeGenerator().Generate(root);
    }

    private RootNode ProcessType(Type t)
    {
      var node = new RootNode
      {
        Type = t
      };

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

    private static readonly Dictionary<Type, Func<PropertyOrFieldInfo, Node>> _deserializers = new Dictionary<Type, Func<PropertyOrFieldInfo, Node>>();

    static DeserializerFactory()
    {
      _deserializers.Add(typeof(int), p =>
        new IntNode
        {
          Member = p
        }
      );

      _deserializers.Add(typeof(string), p =>
        new StringNode
        {
          Member = p
        }
      );
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

      }

      Node node;

      if (nodeFunc != null)
      {
        node = nodeFunc(member);
      }
      else
      {
        var customNode = new UnknownTypeNode
        {
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

    private void ProcessMember(PropertyOrFieldInfo member)
    {
      var node = GetDeserializationNode(member);


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
