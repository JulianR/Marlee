using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Deserialization;

namespace Marlee.Jsv
{
  public class JsvConverter
  {
    private Dictionary<Type, DeserializeHandler> _delegateCache = new Dictionary<Type, DeserializeHandler>();

    private DeserializerFactory _factory = new DeserializerFactory();

    public T DeserializeFromString<T>(string data)
    {
      DeserializeHandler del;

      if (!_delegateCache.TryGetValue(typeof(T), out del))
      {
        del = _factory.CreateDeserializer(typeof(T));
        _delegateCache.Add(typeof(T), del);
      }

      int start = 0;
      return (T)del(ref start, data);
    }
  }
}
