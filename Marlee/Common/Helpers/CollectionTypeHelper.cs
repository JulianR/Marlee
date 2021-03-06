﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Marlee.Common.Helpers
{
  internal static class CollectionTypeHelper
  {

    public static bool IsEnumerable(Type t)
    {
      return typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(string);
    }

    public static Type GetTypeInsideEnumerable(Type type)
    {
      var getEnumeratorMethod = type.GetMethod("GetEnumerator", Type.EmptyTypes);

      if (getEnumeratorMethod == null)
      {
        getEnumeratorMethod = (from i in type.GetInterfaces()
                               from m in i.GetMethods()
                               where m.Name == "GetEnumerator"
                               orderby m.ReturnType.IsGenericType descending
                               select m).FirstOrDefault();

      }

      if (getEnumeratorMethod == null) return null;

      if (getEnumeratorMethod.ReturnType.IsGenericType)
      {
        var args = getEnumeratorMethod.ReturnType.GetGenericArguments();

        if (typeof(IDictionary).IsAssignableFrom(type) && args.Length == 2)
        {
          return typeof(KeyValuePair<,>).MakeGenericType(args[0], args[1]);
        }

        return args.First();
      }
      else if (type.IsArray)
      {
        return type.GetElementType();
      }

      return typeof(object);

    }
  }
}
