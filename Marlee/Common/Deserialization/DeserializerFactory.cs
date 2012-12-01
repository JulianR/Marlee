using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Tree;
using System.Reflection;
using System.Runtime.Serialization;
using Marlee.Jsv.Deserialization;
using System.Collections.Concurrent;

namespace Marlee.Common.Deserialization
{
  internal class DeserializerFactory
  {
    private IConverter _converter;

    internal DeserializerFactory()
    {
      
    }

    public DeserializerFactory(IConverter converter)
    {
      this._converter = converter;
    }

    public Deserializer CreateDeserializer(Type t)
    {
      Deserializer deserializer;

      var builder = new TreeBuilder(_converter);

      var generator = new DeserializeCodeGenerator(builder);

      var knownTypeDel = builder.TryGetKnownTypeDelegate(t);

      if (knownTypeDel == null)
      {
        var root = builder.CreateTree(t);

        var del = generator.Generate(root);

        deserializer = new Deserializer { Method = del };
      }
      else
      {
        deserializer = new Deserializer { Method = knownTypeDel };
      }

      return deserializer;
    }

    
  }
}
