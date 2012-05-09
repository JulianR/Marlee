using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marlee.Jsv;
using ServiceStack.Text;

namespace Marlee.Test.Jsv.Deserialization
{
  [TestClass]
  public class StringCollectionTests
  {
    [Serializable]
    public class TestClass
    {
      public IEnumerable<string> Strings { get; set; }
      public string Foo { get; set; }
    }

    [TestMethod]
    public void StringCollectionIsDeserialized()
    {
      var str = "{Strings:[test,test1,test2],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }


    [TestMethod]
    public void StringCollectionIsDeserialized_1()
    {
      var str = "{Strings:[\"test\",\"test1\",\"test2\"],Foo:\"test3\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }


    [TestMethod]
    public void StringCollectionIsDeserialized_2()
    {
      var str = "{Strings:[\"test\",test1,\"test2\"],Foo:\"test3\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_3()
    {
      var str = "{Strings:[\"test\",test1,test2],Foo:\"test3\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_4()
    {
      var str = "{Strings:[test,\"test1\",test2],Foo:\"test3\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_5()
    {
      var str = "{Strings:[test,\"te\"\"s\"\"t1\",test2],Foo:\"test3\"}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "te\"s\"t1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_6()
    {
      var str = "{Strings:[test,   test1,test2],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_7()
    {
      var str = "{Strings:[     test,test1,test2],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_8()
    {
      var str = "{Strings:[test,test1,test2      ],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      //result = TypeSerializer.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2      " }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_9()
    {
      var str = @"
{
  Strings:
  [
    test,
    test1,
    test2
  ],
  Foo:test3
}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_10()
    {
      var str = @"
{
  Strings:
  [
    ""test"",
    ""test1"",
    ""test2""
  ],
  Foo:test3
}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_11()
    {
      var str = @"
{
  Strings:
  [
    ""test"",
    test1,
    ""test2""
  ],
  Foo:test3
}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_12()
    {
      var str = @"
{
  Strings:
  [
    ""test"",
    ""te""""st""""1"",
    ""test2""
  ],
  Foo:test3
}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "te\"st\"1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [Serializable]
    public class TestClassList
    {
      public List<string> Strings { get; set; }
      public string Foo { get; set; }
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_List()
    {
      var str = "{Strings:[test,test1,test2],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClassList>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [Serializable]
    public class TestClassIList
    {
      public List<string> Strings { get; set; }
      public string Foo { get; set; }
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_IList()
    {
      var str = "{Strings:[test,test1,test2],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClassIList>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [Serializable]
    public class TestClassICollection
    {
      public ICollection<string> Strings { get; set; }
      public string Foo { get; set; }
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_ICollection()
    {
      var str = "{Strings:[test,test1,test2],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClassICollection>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }

    [Serializable]
    public class TestClassArray
    {
      public string[] Strings { get; set; }
      public string Foo { get; set; }
    }

    [TestMethod]
    public void StringCollectionIsDeserialized_Array()
    {
      var str = "{Strings:[test,test1,test2],Foo:test3}";

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClassArray>(str);

      Assert.IsTrue(result.Strings.SequenceEqual(new[] { "test", "test1", "test2" }));
      Assert.AreEqual("test3", result.Foo);
    }
  }
}
