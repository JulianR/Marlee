using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Internal;

namespace Marlee.Common.Parsers
{
  internal static class DoubleParser
  {
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
      int length = i;
      double exponent = 0;

      for (; i < end; ++i)
      {
        c = s[i];
        if (c > 57 || c < 48)
        {
          if (!StandardFunctions.IgnoreChar(c))
          {
            if (c == 'e' || c == 'E')
            {
              length = i - length;
              goto ProcessExponent;
            }

            result = 0;
            return false;
          }
          else
          {
            length = i - length;
            goto ProcessFraction;
          }
        }
        temp = 10 * temp + (c - 48);
      }
      length = i - length;

    ProcessFraction:

      double fraction = (double)temp;

      if (length < ParseLookups.PowLookup.Length)
      {
        fraction = fraction / ParseLookups.PowLookup[length];
      }
      else
      {
        fraction = fraction / ParseLookups.PowLookup[ParseLookups.PowLookup.Length - 1];
      }

      result += fraction;

      result *= sign;

      if (exponent > 0)
      {
        result *= exponent;
      }
      else if (exponent < 0)
      {
        result /= -exponent;
      }

      return true;

    ProcessExponent:

      int expSign = 1;
      int exp = 0;

      for (++i; i < end; ++i)
      {
        c = s[i];
        if (c > 57 || c < 48)
        {
          if (c == '-')
          {
            expSign = -1;
            continue;
          }
        }

        exp = 10 * exp + (c - 48);
      }

      exponent = ParseLookups.DoubleExponentLookup[exp] * expSign;

      goto ProcessFraction;
    }

  }
}
