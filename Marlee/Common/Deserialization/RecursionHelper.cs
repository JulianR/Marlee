using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Tree;
using Marlee.Common.Deserialization;
using Marlee.Internal;
using System.Collections.Concurrent;

namespace Marlee.Internal
{
  public class RecursionHelper
  {
    private static ConcurrentDictionary<Type, Delegate> _recursionLookup = new ConcurrentDictionary<Type, Delegate>();

    public static DeserializeHandler<T> GetDeserializer<T>()
    {
      return (DeserializeHandler<T>)_recursionLookup[typeof(T)];
    }

    public static void SetDeserializer(Type t, Delegate del)
    {
      _recursionLookup[t] = del;
    }
  }
}
