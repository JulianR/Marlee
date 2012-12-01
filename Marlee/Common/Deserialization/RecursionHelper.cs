using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Tree;
using Marlee.Common.Deserialization;
using Marlee.Internal;

namespace Marlee.Internal
{
  public class RecursionHelper
  {
    public static Delegate GetDeserializer<T>(IKnownTypeProvider builder)
    {
      var handler = new DeserializeHandler<T>((ref int i, string str) =>
      {
        var del = builder.TryGetKnownTypeDelegate(typeof(T));

        if (del != null)
        {
          return ((DeserializeHandler<T>)del)(ref i, str);
        }

        return default(T);
      });

      return handler;
    }
  }
}
