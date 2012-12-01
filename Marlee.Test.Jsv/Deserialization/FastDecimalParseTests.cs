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
  /// Summary description for FastDecimalParseTests
  /// </summary>
  [TestClass]
  public class FastDecimalParseTests
  {
    [TestMethod]
    public void DecimalParseTest()
    {
      var s = "1";

      Decimal d;

      Assert.IsTrue(DecimalParser.TryParseDecimalFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.0m, d);
    }

    [TestMethod]
    public void DecimalParseTest_1()
    {
      var s = "1.0";

      Decimal d;

      Assert.IsTrue(DecimalParser.TryParseDecimalFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.0m, d);
    }

    [TestMethod]
    public void DecimalParseTest_2()
    {
      var s = "1.5";

      Decimal d;

      Assert.IsTrue(DecimalParser.TryParseDecimalFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.5m, d);
    }

    [TestMethod]
    public void DecimalParseTest_3()
    {
      var s = "1.500";

      Decimal d;

      Assert.IsTrue(DecimalParser.TryParseDecimalFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.5m, d);
    }

    [TestMethod]
    public void DecimalParseTest_4()
    {
      var s = "1.505";

      Decimal d;

      Assert.IsTrue(DecimalParser.TryParseDecimalFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1.505m, d);
    }

    [TestMethod]
    public void DecimalParseTest_5()
    {
      var s = "1000.505";

      Decimal d;

      Assert.IsTrue(DecimalParser.TryParseDecimalFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(1000.505m, d);
    }

    [TestMethod]
    public void DecimalParseTest_6()
    {
      var s = "-1.505";

      Decimal d;

      Assert.IsTrue(DecimalParser.TryParseDecimalFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(-1.505m, d);
    }

    [TestMethod]
    public void DecimalParseTest_7()
    {
      var s = "    -1.505";

      Decimal d;

      Assert.IsTrue(DecimalParser.TryParseDecimalFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(-1.505m, d);
    }


    [TestMethod]
    public void DecimalParseTest_8()
    {
      var s = "    -1.505    ";

      Decimal d;

      Assert.IsTrue(DecimalParser.TryParseDecimalFastStream(s, 0, s.Length, out d));
      Assert.AreEqual(-1.505m, d);
    }
  }
}
