using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Internal;

namespace Marlee.Common.Parsers
{
  public static class Int32Parser
  {
    public static bool TryParseInt32Fast(string s, out int result)
    {
      return TryParseInt32FastStream(s, 0, s.Length, out result);
    }

    public static bool TryParseInt32FastStream(string s, int begin, int end, out int result)
    {
      result = 0;
      char c = s[begin];
      int sign = 0;
      int start = begin;

      if (c == '-')
      {
        sign = -1;
        start = begin + 1;
      }
      else if (c > 57 || c < 48)
      {
        if (StandardFunctions.IgnoreChar(c))
        {
          do
          {
            ++start;
          }
          while (start < end && StandardFunctions.IgnoreChar(c = s[start]));

          if (start >= end)
          {
            return false;
          }

          if (c == '-')
          {
            sign = -1;
            ++start;
          }
          else
          {
            sign = 1;
          }
        }
        else
        {
          result = 0;
          return false;
        }
      }
      else
      {
        start = begin + 1;
        result = 10 * result + (c - 48);
        sign = 1;
      }

      for (int i = start; i < end; ++i)
      {
        c = s[i];
        if (c > 57 || c < 48)
        {
          if (StandardFunctions.IgnoreChar(c) || c == '.')
          {
            result *= sign;
            return true;
          }
          else
          {
            result = 0;
            return false;
          }
        }

        result = 10 * result + (c - 48);
      }

      result *= sign;
      return true;
    }
  }
}
