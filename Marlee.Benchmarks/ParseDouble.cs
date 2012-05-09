using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Marlee.Common.Helpers;

namespace Marlee.Benchmarks
{
  public static class ParseDouble
  {
    public static void Benchmark()
    {
      var sw = Stopwatch.StartNew();

      double j;

      for (var i = 0; i < 1000000; i++)
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

      for (var i = 0; i < 1000000; i++)
      {
        Parsers.TryParseDoubleFastStream("1", 0, 1, out j);
        Parsers.TryParseDoubleFastStream("2.0", 0, 3, out j);
        Parsers.TryParseDoubleFastStream("3.5", 0, 3, out j);
        Parsers.TryParseDoubleFastStream("-4.5", 0, 4, out j);
        Parsers.TryParseDoubleFastStream("50.06", 0, 5, out j);
        Parsers.TryParseDoubleFastStream("1000.65", 0, 7, out j);
        Parsers.TryParseDoubleFastStream("-10000.8600", 0, 11, out j);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);
    }

  }
}
