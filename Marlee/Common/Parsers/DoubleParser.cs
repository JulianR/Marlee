using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Internal;

namespace Marlee.Common.Parsers
{
  internal static class DoubleParser
  {

    public static double Parse(string s, int start, out int endChar)
    {
      var result = 0.0;
      char c = s[start];
      int sign = 0;
      int len = s.Length;
      if (c == '-')
      {
        sign = -1;
        ++start;
      }
      else if (c > '9' || c < '0')
      {
        if (StandardFunctions.IgnoreChar(c))
        {
          do
          {
            ++start;
          }
          while (start < len && StandardFunctions.IgnoreChar(c = s[start]));

          if (start >= len)
          {
            endChar = -1;
            return 0;
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
          endChar = -1;
          return 0;
        }
      }
      else
      {
        ++start;
        result = 10 * result + (c - '0');
        sign = 1;
      }

      int i = start;
      int length = i;
      long temp = 0;
      double exponent = 0;

      for (; i < len; ++i)
      {
        c = s[i];
        if (c > '9' || c < '0')
        {
          if (c == '.')
          {
            ++i;
            goto DecimalPoint;
          }
          else if (c == 'e' || c == 'E')
          {
            length = i - length;
            goto ProcessExponent;
          }
          else if (c == ',' || c == '}' || c == ']')
          {
            endChar = i;
            result *= sign;
            return result;
          }
          else
          {
            endChar = -1;
            return 0;
          }
        }

        result = 10 * result + (c - 48);
      }

      endChar = len;
      return result * sign;

    DecimalPoint:

      length = i;
      
      for (; i < len; ++i)
      {
        c = s[i];
        if (c > '9' || c < '0')
        {
          if (!StandardFunctions.IgnoreChar(c))
          {
            if (c == 'e' || c == 'E')
            {
              length = i - length;
              goto ProcessExponent;
            }
            else if (c == ',' || c == '}' || c == ']')
            {
              length = i - length;
              goto ProcessFraction;
            }

            endChar = -1;
            return 0;
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

      endChar = i;
      return result;

    ProcessExponent:

      int expSign = 1;
      int exp = 0;

      for (++i; i < len; ++i)
      {
        c = s[i];
        if (c > 57 || c < 48)
        {
          if (c == '-')
          {
            expSign = -1;
            continue;
          }
          else
          {
            exponent = ParseLookups.DoubleExponentLookup[exp] * expSign;
            goto ProcessFraction;
          }
        }

        exp = 10 * exp + (c - 48);
      }

      exponent = ParseLookups.DoubleExponentLookup[exp] * expSign;

      goto ProcessFraction;
    }
  }
}
