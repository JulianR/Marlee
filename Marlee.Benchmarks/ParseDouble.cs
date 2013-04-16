using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Marlee.Common.Helpers;
using Marlee.Common.Parsers;

namespace Marlee.Benchmarks
{
  public static class ParseDouble
  {
    public static void Benchmark()
    {
      var sw = Stopwatch.StartNew();

      double j;

      for (var i = 0; i < 10000000; i++)
      {
        double.TryParse("1", out j);
        double.TryParse("2.0", out j);
        double.TryParse("3.5", out j);
        double.TryParse("-4.5", out j);
        double.TryParse("50.06", out j);
        double.TryParse("1000.65", out j);
        double.TryParse("-10000.8600", out j);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);

      sw.Restart();

      int end = 0;
      for (var i = 0; i < 10000000; i++)
      {
        DoubleParser.Parse("1", 0, out end);
        DoubleParser.Parse("2.0", 0, out end);
        DoubleParser.Parse("3.5", 0, out end);
        DoubleParser.Parse("-4.5", 0, out end);
        DoubleParser.Parse("50.06", 0, out end);
        DoubleParser.Parse("1000.65", 0, out end);
        DoubleParser.Parse("-10000.8600", 0, out end);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);
      Console.WriteLine("On average {0}ns per double", Math.Floor((sw.ElapsedMilliseconds * 1000000) / 70000000.0));
    }

    private static readonly long[] _powLookup = new[]
    {
      1, // 10^0
      10, // 10^1
      100, // 10^2
      1000, // 10^3
      10000, // 10^4
      100000, // 10^5
      1000000, // 10^6
      10000000, // 10^7
      100000000, // 10^8
      1000000000, // 10^9,
      10000000000, // 10^10,
      100000000000, // 10^11,
      1000000000000, // 10^12,
      10000000000000, // 10^13,
      100000000000000, // 10^14,
      1000000000000000, // 10^15,
      10000000000000000, // 10^16,
      100000000000000000, // 10^17,
    };

    private static readonly double[] _doubleExpLookup = GetDoubleExponents();

    private static double[] GetDoubleExponents()
    {
      var max = 309;

      var exps = new double[max];

      for (var i = 0; i < max; i++)
      {
        exps[i] = Math.Pow(10, i);
      }

      return exps;
    }

    // http://stackoverflow.com/a/101613/61632
    private static long IntPow(int n, int exp)
    {
      long result = 1;

      while (exp > 0)
      {
        if ((exp & 1) == 1)
        {
          result *= n;
        }
        exp >>= 1;
        n *= n;
      }

      return result;
    }
  }
}
