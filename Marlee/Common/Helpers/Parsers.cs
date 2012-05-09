using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Jsv;

namespace Marlee.Common.Helpers
{
  internal static class Parsers
  {

    public static bool TryParseInt32Fast(string s, out int result)
    {
      result = 0;
      char c = s[0];
      int sign = 0;
      int start = 0;

      if (c == '-')
      {
        sign = -1;
        start = 1;
      }
      else if (c > 57 || c < 48)
      {
        if (StandardFunctions.IgnoreChar(c))
        {
          do
          {
            start++;
          }
          while (start < s.Length && StandardFunctions.IgnoreChar(c = s[start]));

          if (start >= s.Length)
          {
            return false;
          }

          if (c == '-')
          {
            sign = -1;
            start++;
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
        start = 1;
        result = 10 * result + (c - 48);
        sign = 1;
      }

      for (int i = start; i < s.Length; ++i)
      {
        c = s[i];
        if (c > 57 || c < 48)
        {
          if (StandardFunctions.IgnoreChar(c))
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
          if (StandardFunctions.IgnoreChar(c))
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

    public static bool TryParseDoubleFastStream(string s, int begin, int end, out double result)
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

      int i = start;

      for (; i < end; ++i)
      {
        c = s[i];
        if (c > 57 || c < 48)
        {
          if (c == '.')
          {
            ++i;
            goto DecimalPoint;
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

    DecimalPoint:

      long temp = 0;
      int length = 0;

      for (; i < end; ++i)
      {
        c = s[i];
        if (c > 57 || c < 48)
        {
          if (!StandardFunctions.IgnoreChar(c))
          {
            result = 0;
            return false;
          }
          else
          {
            goto ProcessFraction;
          }
        }
        ++length;
        temp = 10 * temp + (c - 48);
      }

    ProcessFraction:

      double fraction = (double)temp;

      if (length < _powLookup.Length)
      {
        fraction = fraction / _powLookup[length];
      }
      else
      {
        fraction = fraction / _powLookup[_powLookup.Length - 1];
      }

      result += fraction;

      result *= sign;

      return true;
    }

    public static bool TryParseDecimalFastStream(string s, int begin, int end, out decimal result)
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

      int i = start;

      for (; i < end; ++i)
      {
        c = s[i];
        if (c > 57 || c < 48)
        {
          if (c == '.')
          {
            ++i;
            goto DecimalPoint;
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

    DecimalPoint:

      long temp = 0;
      int length = 0;

      for (; i < end; ++i)
      {
        c = s[i];
        if (c > 57 || c < 48)
        {
          if (!StandardFunctions.IgnoreChar(c))
          {
            result = 0;
            return false;
          }
          else
          {
            goto ProcessFraction;
          }
        }
        ++length;
        temp = 10 * temp + (c - 48);
      }

    ProcessFraction:

      decimal fraction = (decimal)temp;

      if (length < _powLookup.Length)
      {
        fraction = fraction / _powLookup[length];
      }
      else
      {
        result = 0;
        return false;
      }

      result += fraction;

      result *= sign;

      return true;
    }

    private static readonly long[] _powLookup = new[]
    {
      1, // 10^0
      10, // 10^1
      100, // 10^2
      1000, // 10^3
      10000, // 10^4
      100000, // 10^5
      1000000, // 10^6
      10000000, // 10^7
      100000000, // 10^8
      1000000000, // 10^9,
      10000000000, // 10^10,
      100000000000, // 10^11,
      1000000000000, // 10^12,
      10000000000000, // 10^13,
      100000000000000, // 10^14,
      1000000000000000, // 10^15,
      10000000000000000, // 10^16,
      100000000000000000, // 10^17,
    };
  }
}
