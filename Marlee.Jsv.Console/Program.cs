using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;

namespace Marlee.Jsv.Console
{
  class Program
  {
    [Serializable]
    class TestClass
    {
      public int Test { get;set;}
    }

    static void Main(string[] args)
    {
      GenerateIntLookup(100);
      var str = "{Test:1E6}";

      var result = TypeSerializer.DeserializeFromString<TestClass>(str);


    }

    private static void GenerateIntLookup(int count)
    {
      var sb = new StringBuilder();

      for (var i = 0; i < count; i++)
      {
        sb.AppendLine(string.Format(@"case ""{0}"": return {0};", i));
      }

      sb.ToString();

    }
  }
}
