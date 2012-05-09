using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marlee.Jsv;

namespace Marlee.Test.Jsv.Deserialization
{
  [TestClass]
  public class IntCollectionTests
  {
    [Serializable]
    public class TestClass
    {
      public IEnumerable<int> Ints { get; set; }
      public string Foo { get; set; }
    }

    [TestMethod]
    public void IntCollectionIsDeserialized()
    {
      var str = "{Ints:[1,2,3],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Ints.SequenceEqual(new[] { 1, 2, 3 }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void IntCollectionIsDeserialized_1()
    {
      var str = "{Ints:[1, 2, 3],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Ints.SequenceEqual(new[] { 1, 2, 3 }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void IntCollectionIsDeserialized_2()
    {
      var str = "{Ints:[1, 2, 3,],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Ints.SequenceEqual(new[] { 1, 2, 3 }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void IntCollectionIsDeserialized_3()
    {
      var str = "{Ints:[1, 2, 3,  ],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Ints.SequenceEqual(new[] { 1, 2, 3 }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void IntCollectionIsDeserialized_4()
    {
      var str = "{Ints:[1, 2aa, 3,  ],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Ints.SequenceEqual(new[] { 1, 3 }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void IntCollectionIsDeserialized_5()
    {
      var str = "{Ints:[1, 2, 3, aaa ],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Ints.SequenceEqual(new[] { 1, 2, 3 }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void IntCollectionIsDeserialized_6()
    {
      var str = "{Ints:[1, 2, -3],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Ints.SequenceEqual(new[] { 1, 2, -3 }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void IntCollectionIsDeserialized_7()
    {
      var str = "{Ints:[1,-2,-3],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Ints.SequenceEqual(new[] { 1, -2, -3 }));
      Assert.AreEqual("test3", result.Foo);
    }
  }
}
