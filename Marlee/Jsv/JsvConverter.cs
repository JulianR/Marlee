using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Deserialization;
using Marlee.Common;
using Marlee.Internal;
using Marlee.Jsv.Deserialization;

namespace Marlee.Jsv
{
  public class JsvConverter : IConverter
  {
    private Dictionary<Type, Deserializer> _deserializerCache = new Dictionary<Type, Deserializer>();

    private DeserializerFactory _factory;

    public Guid Version { get; private set; }

    public JsvConverter()
    {
      Version = Guid.NewGuid();

      _factory = new DeserializerFactory(this);
    }

    public T DeserializeFromString<T>(string data)
    {
      var func = DeserializerStore<T>.Function;
      if (func == null)
      {
        var deserializer = _factory.CreateDeserializer(typeof(T));
        func = DeserializerStore<T>.Function = (DeserializeHandler<T>)deserializer.Method;
      }

      int start = 0;
      return func(ref start, data);

      //Deserializer deserializer;

      //if (!_deserializerCache.TryGetValue(typeof(T), out deserializer))
      //{
      //  deserializer = _factory.CreateDeserializer(typeof(T));

      //  _deserializerCache.Add(typeof(T), deserializer);
      //}

      //int start = 0;

      //return ((DeserializeHandler<T>)deserializer.Method)(ref start, data);
    }
  }
}
