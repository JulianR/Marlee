using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marlee.Jsv;
using Marlee.Common.Helpers;
using Marlee.Common.Parsers;

namespace Marlee.Test.Jsv.Deserialization
{
  /// <summary>
  /// Summary description for FastIntParseTests
  /// </summary>
  [TestClass]
  public class FastIntParseTests
  {
    [TestMethod]
    public void IntParseTest()
    {
      var s = "1";

      int i = 0;

      Assert.IsTrue(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(1, i);
    }

    [TestMethod]
    public void IntParseTest_1()
    {
      var s = "1000";

      int i = 0;

      Assert.IsTrue(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(1000, i);
    }

    [TestMethod]
    public void IntParseTest_2()
    {
      var s = "-1000";

      int i = 0;

      Assert.IsTrue(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(-1000, i);
    }

    [TestMethod]
    public void IntParseTest_3()
    {
      var s = "aaa";

      int i = 0;

      Assert.IsFalse(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(0, i);
    }

    [TestMethod]
    public void IntParseTest_4()
    {
      var s = "1.2";

      int i = 0;

      Assert.IsTrue(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(1, i);
    }

    [TestMethod]
    public void IntParseTest_5()
    {
      var s = "-1";

      int i = 0;

      Assert.IsTrue(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(-1, i);
    }

    [TestMethod]
    public void IntParseTest_6()
    {
      var s = " -1";

      int i = 0;

      Assert.IsTrue(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(-1, i);
    }

    [TestMethod]
    public void IntParseTest_7()
    {
      var s = " -1 ";

      int i = 0;

      Assert.IsTrue(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(-1, i);
    }


    [TestMethod]
    public void IntParseTest_8()
    {
      var s = "1.45";

      int i = 0;

      Assert.IsTrue(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(1, i);
    }

    [TestMethod]
    public void IntParseTest_9()
    {
      var s = "-1.95";

      int i = 0;

      Assert.IsTrue(Int32Parser.TryParseInt32Fast(s, out i));

      Assert.AreEqual(-1, i);
    }
  }
}
