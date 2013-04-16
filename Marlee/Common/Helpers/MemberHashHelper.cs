using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Tree;
using System.Reflection;

namespace Marlee.Internal
{
  public class HashingMethod
  {
    public Dictionary<int, string> Values { get; set; }
    public Func<string, int, int, int> Method { get; set; }
  }

  public static class MemberHashHelper
  {
    public static HashingMethod CanUseHashLookup(IEnumerable<string> members)
    {
      //return null;

      HashingMethod method;

      if (CanUseMethod(HashOne, members, out method))
      {
        return method;
      }
      else if (CanUseMethod(HashTwo, members, out method))
      {
        return method;
      }

      return null;
    }

    private static bool CanUseMethod(Func<string, int, int, int> func, IEnumerable<string> members, out HashingMethod method)
    {
      method = null;

      var result = new Dictionary<int, string>();

      foreach (var member in members)
      {
        if (member == null) return false;

        if (member.Length == 1) return false;

        var hash = func(member, 0, member.Length);

        if (result.ContainsKey(hash)) return false;

        result.Add(hash, member);
      }

      method = new HashingMethod
      {
        Values = result,
        Method = func
      };

      return true;
    }

    public static int HashOne(string s, int start, int end)
    {
      return (s[start] * 31) + (s[end - 1] * 31);
    }

    public static int HashTwo(string s, int start, int end)
    {
      return (s[start + 1] * 31) + (s[end - 2] * 31);
    }
  }
}
