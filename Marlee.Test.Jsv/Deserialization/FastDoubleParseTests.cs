using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marlee.Common.Helpers;
using Marlee.Common.Parsers;

namespace Marlee.Test.Jsv.Deserialization
{
  /// <summary>
  /// Summary description for FastDoubleParseTests
  /// </summary>
  [TestClass]
  public class FastDoubleParseTests
  {
    [TestMethod]
    public void DoubleParseTest()
    {
      var s = "1";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.0, d);
    }

    [TestMethod]
    public void DoubleParseTest_1()
    {
      var s = "1.0";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.0, d);
    }

    [TestMethod]
    public void DoubleParseTest_2()
    {
      var s = "1.5";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.5, d);
    }

    [TestMethod]
    public void DoubleParseTest_3()
    {
      var s = "1.500";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.5, d);
    }

    [TestMethod]
    public void DoubleParseTest_4()
    {
      var s = "1.505";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.505, d);
    }

    [TestMethod]
    public void DoubleParseTest_5()
    {
      var s = "1000.505";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1000.505, d);
    }

    [TestMethod]
    public void DoubleParseTest_6()
    {
      var s = "-1.505";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(-1.505, d);
    }

    [TestMethod]
    public void DoubleParseTest_7()
    {
      var s = "    -1.505";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(-1.505, d);
    }

    [TestMethod]
    public void DoubleParseTest_8()
    {
      var s = "    -1.505    ";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(-1.505, d);
    }

    [TestMethod]
    public void DoubleParseTest_9()
    {
      var s = "3.023";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(3.023, d);
    }

    [TestMethod]
    public void DoubleParseTest_10()
    {
      var s = "3.023E10";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(3.023E10, d);
    }

    [TestMethod]
    public void DoubleParseTest_11()
    {
      var s = "1.023E308";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.IsTrue(d > 0);
    }

    [TestMethod]
    public void DoubleParseTest_12()
    {
      var s = "3.023E-10";

      double d;

      Assert.IsTrue(DoubleParser.TryParseDoubleFastStream(s, 0, s.Length, out d));
      Assert.IsTrue(Math.Abs(d - 3.023E-10) < 1.0E-10);
    }
  }
}
