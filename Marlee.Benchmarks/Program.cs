using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Marlee.Jsv;
using Marlee.Common.Helpers;
using Marlee.Common.Parsers;

namespace Marlee.Benchmarks
{
  class Program
  {
    static void Main(string[] args)
    {
      ParseDouble.Benchmark();
      Console.Clear();
      ParseDouble.Benchmark();
      //SwitchTest.Benchmark();
      //Console.Clear();
      //SwitchTest.Benchmark();
      //TypeSerializerTests.BenchmarkSimple();
      //Console.Clear();
      //TypeSerializerTests.BenchmarkSimple();
      //TestParse();
      //Console.Clear();
      //TestParse();
      //TestGeneric();
      //Console.Clear();
      //TestGeneric();
    }

    public static void TestGeneric()
    {
      Stopwatch sw = Stopwatch.StartNew();

      var coll = new List<int>(10);

      for (var i = 0; i < 10000000; i++)
      {
        FooGeneric(coll).Clear();
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);

      sw.Restart();

      for (var i = 0; i < 10000000; i++)
      {
        Foo(coll).Clear();
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);

    }

    public static T FooGeneric<T>(T collection) where T : ICollection<int>
    {
      for (var i = 0; i < 2; i++)
      {
        collection.Add(i);
      }

      return collection;
    }

    public static ICollection<int> Foo(ICollection<int> collection)
    {
      for (var i = 0; i < 2; i++)
      {
        collection.Add(i);
      }

      return collection;
    }

    public static void TestParse()
    {
      Stopwatch sw = Stopwatch.StartNew();
      int j;
      for (var i = 0; i < 1000000; i++)
      {
        TryParseNormal("1", out j);
        TryParseNormal("2", out j);
        TryParseNormal("3", out j);
        TryParseNormal("4", out j);
        TryParseNormal("50", out j);
        TryParseNormal("1000", out j);
        TryParseNormal("10000", out j);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);

      sw.Restart();

      for (var i = 0; i < 10000000; i++)
      {
        TryParseASCII("1", out j);
        TryParseASCII("2", out j);
        TryParseASCII("3", out j);
        TryParseASCII("4", out j);
        TryParseASCII("50", out j);
        TryParseASCII("1000", out j);
        TryParseASCII("10000", out j);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);

      sw.Restart();

      for (var i = 0; i < 10000000; i++)
      {
        Int32Parser.TryParseInt32Fast("1", out j);
        Int32Parser.TryParseInt32Fast("2", out j);
        Int32Parser.TryParseInt32Fast("3", out j);
        Int32Parser.TryParseInt32Fast("4", out j);
        Int32Parser.TryParseInt32Fast("50", out j);
        Int32Parser.TryParseInt32Fast("1000", out j);
        Int32Parser.TryParseInt32Fast("10000", out j);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);
      Console.WriteLine("On average {0}ns per int", Math.Floor((sw.ElapsedMilliseconds * 1000000) / 70000000.0));

    }

    public static bool TryParseNormal(string s, out int i)
    {
      return int.TryParse(s, out i); 
    }

    public static bool TryParseASCII(string s, out int result)
    {
      result = 0;

      char c = s[0];

      int sign = c == '-' ? -1 : 1;

      result = 10 * result + (c - 48);

      for (int i = 1; i < s.Length; i++)
      {
        c = s[i];
        result = 10 * result + (c - 48);
      }

      result *= sign;

      return true;
    }

    public static bool TryParseASCII_1(string s, out int result)
    {
      result = 0;

      char c = s[0];

      int sign = c == '-' ? -1 : 1;

      for (int i = 0; i < s.Length; i++)
      {
        c = s[i];
        result = 10 * result + (c - 48);
      }

      result *= sign;

      return true;
    }
  }
}
