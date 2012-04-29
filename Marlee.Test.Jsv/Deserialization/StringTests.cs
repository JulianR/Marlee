using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marlee.Jsv;
using ServiceStack.Text;

namespace Marlee.Test.Jsv.Deserialization
{
  [TestClass]
  public class StringTests
  {
    [Serializable]
    public class TestClass1
    {
      public string Foo { get; set; }
      public int Test { get; set; }
      public string Foobar { get; set; }
    }


    [TestMethod]
    public void DeserializeSimpleStringWorks()
    {
      var jsv = new JsvConverter();
      var str = "{Foo:Test,Test:1,Foobar:ssdfsdfsdfsdf}";
      var result = jsv.DeserializeFromString<TestClass1>(str);

      Assert.AreEqual("Test", result.Foo);
      Assert.AreEqual(1, result.Test);
      Assert.AreEqual("ssdfsdfsdfsdf", result.Foobar);
    }

    [TestMethod]
    public void PropertyOrderIsNotImportant()
    {
      var jsv = new JsvConverter();
      var str = "{Test:1,Foo:Test,Foobar:ssdfsdfsdfsdf}";
      var result = jsv.DeserializeFromString<TestClass1>(str);

      Assert.AreEqual("Test", result.Foo);
      Assert.AreEqual(1, result.Test);
      Assert.AreEqual("ssdfsdfsdfsdf", result.Foobar);
    }

    [TestMethod]
    public void DeserializeSimpleStringWorksWithMultiLine()
    {
      var jsv = new JsvConverter();
      var str = @"
{
  Foo:Test,
  Test:1,
  Foobar:ssdfsdfsdfsdf
}";
      var result = jsv.DeserializeFromString<TestClass1>(str);

      Assert.AreEqual("Test", result.Foo);
      Assert.AreEqual(1, result.Test);
      Assert.AreEqual("ssdfsdfsdfsdf", result.Foobar);
    }

    [TestMethod]
    public void DeserializeSimpleStringWorksWithMultiLineAndTrailingComma()
    {
      var jsv = new JsvConverter();
      var str = @"
{
  Foo:Test,
  Test:1,
  Foobar:ssdfsdfsdfsdf,
}";
      var result = jsv.DeserializeFromString<TestClass1>(str);

      Assert.AreEqual("Test", result.Foo);
      Assert.AreEqual(1, result.Test);
      Assert.AreEqual("ssdfsdfsdfsdf", result.Foobar);
    }

    [TestMethod]
    public void DeserializeSimpleStringWorksWithMultiLineAndMultipleTrailingComma()
    {
      var jsv = new JsvConverter();
      var str = @"
{
  Foo:Test,
  Test:1,
  Foobar:ssdfsdfsdfsdf,,,,
}";
      var result = jsv.DeserializeFromString<TestClass1>(str);

      Assert.AreEqual("Test", result.Foo);
      Assert.AreEqual(1, result.Test);
      Assert.AreEqual("ssdfsdfsdfsdf", result.Foobar);
    }

    [TestMethod]
    public void DeserializeSimpleStringWorksWithArbitraryWhiteSpace()
    {
      var jsv = new JsvConverter();
      var str = @"
{
  Foo:    Test,
  Test  : 1,
  Foobar      :ssdfsdfsdfsdf,,,,
}";
      var result = jsv.DeserializeFromString<TestClass1>(str);

      Assert.AreEqual("Test", result.Foo);
      Assert.AreEqual(1, result.Test);
      Assert.AreEqual("ssdfsdfsdfsdf", result.Foobar);
    }

    [TestMethod]
    public void DeserializeSimpleStringWorksWithLiteralNewLines()
    {
      var jsv = new JsvConverter();
      var str = "{Foo:Test,Test:1,Foobar:ssdfsdfsdfsdf\n\n\n\n}";
      var result = jsv.DeserializeFromString<TestClass1>(str);

      Assert.AreEqual("Test", result.Foo);
      Assert.AreEqual(1, result.Test);
      Assert.AreEqual("ssdfsdfsdfsdf", result.Foobar);
    }

    [TestMethod]
    public void DeserializeSimpleStringWorksWithQuotedValues()
    {
      var jsv = new JsvConverter();
      var str = @"{Foo:Test,Test:1,Foobar:""ssdfsdfsdfsdf""}";
      var result = jsv.DeserializeFromString<TestClass1>(str);

      Assert.AreEqual("Test", result.Foo);
      Assert.AreEqual(1, result.Test);
      Assert.AreEqual("ssdfsdfsdfsdf", result.Foobar);
    }

    [TestMethod]
    public void DeserializeSimpleStringWorksWithQuotedValues_1()
    {
      var obj = new TestClass1
      {
        Foo = "Test",
        Test = 1,
        Foobar = @"ssdfs""df""sdfsdf"
      };

      var str = TypeSerializer.SerializeToString(obj);

      var jsv = new JsvConverter();
      //var str = @"{Foo:Test,Test:1,Foobar:""ssdfs""""df""""sdfsdf""}";
      var result = jsv.DeserializeFromString<TestClass1>(str);

      Assert.AreEqual("Test", result.Foo);
      Assert.AreEqual(1, result.Test);
      Assert.AreEqual(@"ssdfs""df""sdfsdf", result.Foobar);
    }

    [Serializable]
    public class TestClass3
    {
      public string Foo { get; set; }
    }

    [TestMethod]
    public void SinglePropertyWorks()
    {
      var str = "{Foo:test}";

      var jsv = new JsvConverter();
      var result = jsv.DeserializeFromString<TestClass3>(str);

      Assert.AreEqual("test", result.Foo);
    }

  }
}
