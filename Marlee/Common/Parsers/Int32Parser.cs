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

    /// <summary>
    /// Parses an int from a JSON/JSV string starting at the given position within the string.
    /// It does so without having to create a substring.
    /// </summary>
    /// <returns></returns>
    public static int Parse(string s, int start, out int endChar)
    {
      int result = 0;
      char c = s[start];
      int sign = 0;

      if (c == '-') // If the first char is the sign char
      {
        sign = -1;
        ++start; // Proceed to the next
      }
      else if (c > '9' || c < '0') // If it's not a number
      {
        // If it's a char we can ignore (whitespace)
        // then loop until we find something that we can't ignore.
        if (StandardFunctions.IgnoreChar(c)) 
        {
          do
          {
            ++start;
          }
          while (start < s.Length && StandardFunctions.IgnoreChar(c = s[start]));

          // Reached end of string
          if (start >= s.Length)
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
        else // It's not a number or whitespace, so give up.
        {
          endChar = -1;
          return 0;
        }
      }
      else // If it's a number then process the number
      {
        ++start;
        result = 10 * result + (c - 48);
        sign = 1;
      }

      for (int i = start; i < s.Length; ++i)
      {
        c = s[i];
        if (c > '9' || c < '0')
        {
          // The end of the number. The order here is significant,
          // a comma is likely to come first after an int (at the end of a property or an array element)
          // so check that first, otherwise it's likely to be a brace or square bracket, in that order.
          if (c == ',' || c == '}' || c == ']') 
          {
            endChar = i;
            result *= sign;
            return result;
          } 
          else
          {
            // The end of the number, but we need to find the first comma, } or ] before we can exit.
            for (var j = start + 1; j < s.Length; ++j)
            {
              c = s[j];

              if (c == ',' || c == '}' || c == ']')
              {
                endChar = j;
                result *= sign;
                return result;
              }
            }

            // Made it until the very end of the string without finding a comma, } or ], apparently
            endChar = s.Length - 1;
            return result * sign;
          }
        }
        // If it's a normal number
        result = 10 * result + (c - 48);
      }

      endChar = s.Length - 1;
      return result * sign;
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
