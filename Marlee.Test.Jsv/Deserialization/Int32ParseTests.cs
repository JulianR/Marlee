using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marlee.Common.Parsers;

namespace Marlee.Test.Jsv.Deserialization
{
  [TestClass]
  public class Int32ParseTests
  {
    [TestMethod]
    public void Test_1()
    {
      var s = "1";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1, val);
    }

    [TestMethod]
    public void Test_2()
    {
      var s = "1,";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1, val);
    }

    [TestMethod]
    public void Test_3()
    {
      var s = "1.";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1, val);
    }

    [TestMethod]
    public void Test_4()
    {
      var s = "1.50";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1, val);
    }

    [TestMethod]
    public void Test_5()
    {
      var s = "1 ,";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1, val);
    }

    [TestMethod]
    public void Test_6()
    {
      var s = "1 .";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1, val);
    }

    [TestMethod]
    public void Test_7()
    {
      var s = "1}.";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1, val);
    }

    [TestMethod]
    public void Test_8()
    {
      var s = "123";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(123, val);
    }

    [TestMethod]
    public void Test_9()
    {
      var s = "1001";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1001, val);
    }

    [TestMethod]
    public void Test_10()
    {
      var s = "     1001";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1001, val);
    }

    [TestMethod]
    public void Test_11()
    {
      var s = "\t1001";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1001, val);
    }


    [TestMethod]
    public void Test_12()
    {
      var s = "\t1001.990";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1001, val);
    }

    [TestMethod]
    public void Test_13()
    {
      var s = "\t1001.}990";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1001, val);
    }

    [TestMethod]
    public void Test_14()
    {
      var s = "}1001";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(0, val);
    }


    [TestMethod]
    public void Test_15()
    {
      var s = "\t}1001.990";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(0, val);
    }


    [TestMethod]
    public void Test_16()
    {
      var s = "-100";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(-100, val);
    }


    [TestMethod]
    public void Test_17()
    {
      var s = "    -100";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(-100, val);
    }

    [TestMethod]
    public void Test_18()
    {
      var s = "\t-100";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(-100, val);
    }

    [TestMethod]
    public void Test_19()
    {
      var s = "10-0";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(10, val);
    }

    [TestMethod]
    public void Test_20()
    {
      var s = "-10,0";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(-10, val);
    }

    [TestMethod]
    public void Test_21()
    {
      var s = @"

1000
,";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(1000, val);
    }

    [TestMethod]
    public void Test_22()
    {
      var s = @"

-1000
,";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(-1000, val);
    }

    [TestMethod]
    public void Test_23()
    {
      var s = @"-

1000
,";
      int end;
      var val = Int32Parser.Parse(s, 0, out end);
      Assert.AreEqual(0, val);
    }
  }
}
