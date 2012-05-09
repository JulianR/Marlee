using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ServiceStack.Text;
using Marlee.Jsv;

namespace Marlee.Benchmarks
{
  public static class TypeSerializerTests
  {
    [Serializable]
    public class TestClass
    {
      public string Foo { get; set; }

      public IList<int> Ints { get; set; }

      public IList<string> Strings { get; set; }

      public int Bar { get; set; }
    }

    public static void BenchmarkSimple()
    {

      var instance = new TestClass
      {
        Bar = 100,
        Foo = "test",
        Strings = new[] { "test", "test1", "test2" },
        Ints = new[] { 1, 2, 300, 5000, 3535, 22346, 11, 1543 }
      };

      var str = TypeSerializer.SerializeToString(instance);

      var result = TypeSerializer.DeserializeFromString<TestClass>(str);

      var jsv = new JsvConverter();
      result = null;
      result = jsv.DeserializeFromString<TestClass>(str);

      var sw = Stopwatch.StartNew();

      for (var i = 0; i < 1000000; i++)
      {
        result = TypeSerializer.DeserializeFromString<TestClass>(str);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);

      sw.Restart();

      for (var i = 0; i < 1000000; i++)
      {
        result = jsv.DeserializeFromString<TestClass>(str);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);

    }
  }
}
