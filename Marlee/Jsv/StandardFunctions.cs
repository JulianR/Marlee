using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Helpers;
using Marlee.Common.Parsers;

namespace Marlee.Internal
{
  public static class StandardFunctions
  {
    public static bool IgnoreChar(char c)
    {
      return c < 33;
    }

    public static T ExtractStringCollection<T>(ref int i, string str, T collection) where T : ICollection<string>
    {
      int start = -1;

      string subStr;

      bool replace = false;

      var end = -1;

      int j;

      for (j = i; j < str.Length; ++j)
      {
        char c = str[j];

        if (IgnoreChar(c)) continue;

        switch (c)
        {
          case '[': 
          case ',': continue;
          case ']':
            i = j + 1;
            return collection;
          case '"':
          
            start = ++j;

            for (; j < str.Length; ++j)
            {
              c = str[j];

              if (c == '"')
              {
                if (str[j + 1] != '"') // end of string
                {
                  end = j;
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

            subStr = str.Substring(start, end - start);

            if (replace)
            {
              subStr = subStr.Replace("\"\"", "\""); // Jsv double quotes in a string are escaped with double double quotes
            }

            collection.Add(subStr);
            break;
          default:
            start = j;

            for (; j < str.Length; ++j)
            {
              c = str[j];

              if (c == ',' || c < 14) // < 14 == CR or LF
              {
                end = j;

                subStr = str.Substring(start, end - start);

                collection.Add(subStr);

                break;
              }
              else if(c == ']')
              {
                end = j;

                subStr = str.Substring(start, end - start);

                i = j + 1;

                collection.Add(subStr);

                return collection;
              }
            }

            if (end < 0) throw new InvalidOperationException("String not properly terminated");

            break;
        }
      }

      throw new InvalidOperationException();
    }

    public static T ExtractInt32Collection<T>(ref int i, string str, T collection) where T : ICollection<int>
    {
      int val;

      for (var j = i; j < str.Length; )
      {
        char c = str[j];

        switch (c)
        {
          case '[':
          case ',':
            ++j;
            continue;
          case ']':
            i = j + 1;
            return collection;
        }

        if (IgnoreChar(c))
        {
          ++j;
          continue;
        }

        val = Int32Parser.Parse(str, j, out j);

        if (j < 0) throw new InvalidOperationException();

        collection.Add(val);
      }
    
      throw new InvalidOperationException();
    }

    public static int ExtractInt32(ref int i, string str)
    {
      var val = Int32Parser.Parse(str, i, out i);

      if (i < 0) throw new InvalidOperationException("Invalid Int32");

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
              i = end + 2;
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
            i = end;
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
              start = i + 1;
              return;
            }
          }
        }
      }

      throw new InvalidOperationException();
    }

    public static double ExtractDouble(ref int i, string str)
    {
      var val = DoubleParser.Parse(str, i, out i);

      if (i < 0) throw new InvalidOperationException();

      return val;
    }

    public static decimal ExtractDecimal(ref int i, string str)
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

      decimal val;

      DecimalParser.TryParseDecimalFastStream(str, i, end, out val);

      i = end - 1;

      return val;
    }

    private static ICollection<T> ExtractGenericCollection<T>(ref int start, string str, ExtractCollectionItem<T> function, ICollection<T> collection) where T : new()
    {
      char c;

      for (var i = start; i < str.Length; )
      {
        c = str[i];

        if (c == '[')
        {
          ++i;
          continue;
        }

        if (StandardFunctions.IgnoreChar(c))
        {
          ++i;
          continue;
        }

        if (c == ']')
        {
          start = i + 1;
          return collection;
        }

        var nr = function(ref i, str);

        collection.Add(nr);
      }

      return null;
    } 
  }
}
