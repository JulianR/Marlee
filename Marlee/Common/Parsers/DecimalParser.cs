using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Internal;

namespace Marlee.Common.Parsers
{
  internal static class DecimalParser
  {

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
      int length = i;

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
            length = i - length;
            goto ProcessFraction;
          }
        }
        temp = 10 * temp + (c - 48);
      }
      length = i - length;

    ProcessFraction:

      decimal fraction = (decimal)temp;

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

      return true;
    }

  }
}
