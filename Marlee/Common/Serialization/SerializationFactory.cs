using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Tree;

namespace Marlee.Common.Serialization
{
  internal class SerializationFactory
  {
    private IConverter _converter;

    internal SerializationFactory()
    {
      
    }

    public SerializationFactory(IConverter converter)
    {
      this._converter = converter;
    }

    public SerializationHandler<T> CreateSerializer<T>(Type t)
    {
      //Serializer serializer;

      //var builder = new TreeBuilder(_converter);

      //var generator = new SerializeCodeGenerator(builder);

      //var knownTypeDel = builder.TryGetKnownTypeDelegate(t);

      //if (knownTypeDel == null)
      //{
      //  var root = builder.CreateTree(t);

      //  var del = generator.Generate(root);

      //  serializer = new Serializer { Method = del };
      //}
      //else
      //{
      //  serializer = new Serializer { Method = knownTypeDel };
      //}

      return null;
      //return serializer;
    }
  }
}
