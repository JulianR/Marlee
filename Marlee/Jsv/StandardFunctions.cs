using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Jsv
{
  public static class StandardFunctions
  {
    public static bool IgnoreChar(char c)
    {
      return c < 33;
    }

    private static ICollection<string> ExtractStringCollection(ref int i, string str, ICollection<string> collection)
    {
      int start = -1;

      string subStr;

      for (var j = i; j < str.Length; j++)
      {
        char c = str[j];

        if (c == '[') continue;

        if (char.IsWhiteSpace(c)) continue;

        if (c == ',')
        {
          subStr = str.Substring(start, j - start);
          collection.Add(subStr);
          start = j + 1;
        }
        else if (c == ']')
        {
          subStr = str.Substring(start, j - start);
          collection.Add(subStr);

          i = j;

          return collection;
        }

        if (start < 0) start = j;
      }

      throw new InvalidOperationException();
    }

    private static ICollection<int> ExtractInt32Collection(ref int i, string str, ICollection<int> collection)
    {
      int start = -1;

      string subStr;
      int val;

      for (var j = i; j < str.Length; j++)
      {
        char c = str[j];

        if (c == '[') continue;

        if (char.IsWhiteSpace(c)) continue;

        if (c == ',')
        {
          subStr = str.Substring(start, j - start);
          int.TryParse(subStr, out val);

          collection.Add(val);
          start = j + 1;
        }
        else if (c == ']')
        {
          subStr = str.Substring(start, j - start);
          int.TryParse(subStr, out val);
          collection.Add(val);
          i = j;
          return collection;
        }

        if (start < 0) start = j;
      }

      throw new InvalidOperationException();
    }

    public static int ExtractInt32(ref int i, string str)
    {
      var end = -1;

      for (var j = i; j < str.Length; j++)
      {
        char c = str[j];

        if (c == ',' || c == '}')
        {
          end = j;
          break;
        }
      }

      if (end < 0) throw new InvalidOperationException();

      var subStr = str.Substring(i, end - i);

      int val;

      int.TryParse(subStr, out val);

      i = end - 1;

      return val;

    }

    public static string ExtractString(ref int i, string str)
    {
      int end = -1;
      char c;
      int start;
      bool replace = false;

      while (IgnoreChar(c = str[i]))
      {
        i++;
      }

      if (c == '"')
      {
        start = i + 1;

        for (var j = i + 1; j < str.Length; j++)
        {
          c = str[j];

          if (c == '"')
          {
            if (str[j + 1] != '"') // end of string
            {
              end = j;
              i = end + 1;
              break;
            }
            else
            {
              replace = true; // Replace double double quotes with single double quotes
              j++;
            }
          }

        }

        if (end < 0) throw new InvalidOperationException("String not properly terminated");

        str = str.Substring(start, end - start);

        if (replace)
        {
          str = str.Replace("\"\"", "\""); // Jsv double quotes in a string are escaped with double double quotes
        }

        return str;
      }
      else
      {
        start = i;
        for (var j = i + 1; j < str.Length; j++)
        {
          c = str[j];

          if (c == ',' || c == '}' || c < 14) // < 14 == CR or LF
          {
            end = j;
            i = end - 1;
            break;
          }
        }

        if (end < 0) throw new InvalidOperationException("String not properly terminated");

        str = str.Substring(start, end - start);

        return str;
      }


    }

    public static void Skip(ref int start, string str)
    {
      char c;
      char startChar = str[start];
      int startCharCount = 1;
      int endCharCount = 0;
      char endChar;
      bool enteredString = false;

      if (startChar == '[')
      {
        endChar = ']';
      }
      else if (startChar == '{')
      {
        endChar = '}';
      }
      else if (startChar == '"')
      {
        endChar = ',';
        startChar = '\0';
        enteredString = true;
      }
      else
      {
        endChar = ',';
        startChar = '\0';
      }

      for (var i = start + 1; i < str.Length; i++)
      {
        c = str[i];

        if (c == '"' && !enteredString)
        {
          enteredString = true;
        }
        else if (c == '"')
        {
          enteredString = false;
        }

        if (c == startChar)
        {
          startCharCount++;
          continue;
        }

        if (!enteredString)
        {
          if (endChar == ',')
          {
            if (c == ',' || c == '}' || c == ']')
            {
              endCharCount++;

              if (startCharCount == endCharCount)
              {
                start = i;
                return;
              }
            }
          }
          else if (c == endChar)
          {
            endCharCount++;

            if (startCharCount == endCharCount)
            {
              start = i;
              return;
            }
          }
        }
      }

      throw new InvalidOperationException();
    }
  }
}
