using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Common.Parsers
{
  internal static class ParseLookups
  {
    internal static readonly long[] PowLookup = new[]
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

    internal static readonly double[] DoubleExponentLookup = GetDoubleExponents();

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
