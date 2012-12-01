using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Tree;

namespace Marlee.Internal
{
  public static class MemberHashHelper
  {
    public static Dictionary<int, string> CanUseHashLookup(IEnumerable<string> members)
    {
      //return null;
      var result = new Dictionary<int, string>();

      foreach (var member in members)
      {
        if (member == null) return null;

        if (member.Length == 1) return null;

        var hash = GetPropertyHashValue(member);

        if (result.ContainsKey(hash)) return null;

        result.Add(hash, member);
      }

      return result;
    }

    public static int GetPropertyHashValue(string s)
    {
      return (s[0] * 31) + (s[s.Length - 1] * 31);
    }

    public static int Hash(string s, int start, int end)
    {
      return (s[start] * 31) + (s[end - 1] * 31);
    }

  }
}
