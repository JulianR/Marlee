using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marlee.Common.Parsers;

namespace Marlee.Test.Jsv.Deserialization
{
  [TestClass]
  public class DoubleParseTests
  {
    [TestMethod]
    public void Test_1()
    {
      var s = "1";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(1, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_2()
    {
      var s = "1.5";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(1.5, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_3()
    {
      var s = "1.500";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(1.5, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_4()
    {
      var s = "21.500";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(21.5, val);
      Assert.AreEqual(s.Length, end);
    }


    [TestMethod]
    public void Test_5()
    {
      var s = "-1";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(-1, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_6()
    {
      var s = "-1.5";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(-1.5, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_7()
    {
      var s = "-1.500";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(-1.5, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_8()
    {
      var s = "-21.500";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(-21.5, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_9()
    {
      var s = "21E10";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(21E10, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_10()
    {
      var s = "21.5E10";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(21.5E10, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_11()
    {
      var s = "21.505E10";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(21.505E10, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_12()
    {
      var s = "\t21.505E10";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(21.505E10, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_13()
    {
      var s = " -21.505E10";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(-21.505E10, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_14()
    {
      var s = " -21.505E-2";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(-21.505E-2, val);
      Assert.AreEqual(s.Length, end);
    }

    [TestMethod]
    public void Test_15()
    {
      var s = " -21.505E-2\t\t";
      int end;
      var val = DoubleParser.Parse(s, 0, out end);
      Assert.AreEqual(-21.505E-2, val);
      Assert.AreEqual(s.Length-2, end);
    }
  }
}
