using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Internal;

namespace Marlee.Common.Parsers
{
  public static class Int64Parser
  {
    /// <summary>
    /// Parses a long from a JSON/JSV string starting at the given position within the string.
    /// It does so without having to create a substring.
    /// </summary>
    /// <returns></returns>
    public static long Parse(string s, int start, out int endChar)
    {
      long result = 0;
      char c = s[start];
      int sign = 1;
      var len = s.Length;

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
          while (start < len && StandardFunctions.IgnoreChar(c = s[start]));

          // Reached end of string
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
      }

      for (int i = start; i < len; ++i)
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
            for (var j = start + 1; j < len; ++j)
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
            endChar = len;
            return result * sign;
          }
        }
        // If it's a normal number
        result = 10 * result + (c - 48);
      }

      endChar = len;
      return result * sign;
    }
  }
}
