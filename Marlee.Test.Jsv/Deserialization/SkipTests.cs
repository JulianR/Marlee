using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marlee.Jsv;

namespace Marlee.Test.Jsv.Deserialization
{
  [TestClass]
  public class SkipTests
  {
    [Serializable]
    public class TestClass
    {
      public string Foo { get; set; }
    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped()
    {
      var str = "{Foo:test,Bar:x}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);
    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_1()
    {
      var str = "{Bar:x, Foo:test}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_2()
    {
      var str = "{Bar:\"x\", Foo:\"test\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_3()
    {
      var str = "{Bar:\"x,\", Foo:\"test\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_4()
    {
      var str = "{Bar:\"x[]\"\",\", Foo:\"test\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_5()
    {
      var str = "{Bar:{ Test: 1235, Foobar: \"tester\"}, Foo:\"test\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_6()
    {
      var str = "{Bar:{ Test: 1235, Foobar: [\"tester\", \"foo\"]}, Foo:\"test\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_7()
    {
      var str = "{Bar:{ Test: { }, Foobar: [\"tester\", \"foo\"]}, Foo:\"test\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_8()
    {
      var str = "{Bar:\"\", Foo:\"test\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_9()
    {
      var str = "{Bar:null, Foo:\"test\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_10()
    {
      var str = "{Bar:  test1, Foo:test}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }

    [TestMethod]
    public void UnknownPropertiesAreSkipped_11()
    {
      var str = "{Bar:  \t\"test1\", Foo:test}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Foo);

    }
  }
}
