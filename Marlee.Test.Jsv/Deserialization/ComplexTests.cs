using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.Text;
using Marlee.Jsv;

namespace Marlee.Test.Jsv.Deserialization
{
  [TestClass]
  public class ComplexTests
  {
    [Serializable]
    public class TestClass
    {
      public TestClass1 Bar { get; set; }
    }

    [Serializable]
    public class TestClass1
    {
      public string Foo { get; set; }
    }

    [TestMethod]
    public void ComplexTest()
    {
      var instance = new TestClass
      {
        Bar = new TestClass1
        {
          Foo = "test"
        }
      };

      var str = TypeSerializer.SerializeToString(instance);

      var jsv = new JsvConverter();

      var result = jsv.DeserializeFromString<TestClass>(str);

      Assert.AreEqual("test", result.Bar.Foo);

      var foo = new TestClass1
      {
        Foo = "test1"
      };

      var result1 = jsv.DeserializeFromString<TestClass1>(TypeSerializer.SerializeToString(foo));

      Assert.AreEqual("test1", result1.Foo);
    }

    [Serializable]
    public class Customer
    {
      public string Name { get; set; }
      public int ID { get; set; }
      public List<string> Roles { get; set; }
      public Address Address { get; set; }
      public Customer Parent { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public int Age { get; set; }
    }

    [Serializable]
    public class Address
    {
      public string Street { get; set; }
      public int ID { get; set; }
    }


      private static Customer Create()
      {
        var customer = new Customer
        {
          Name = "Julian Rooze",
          ID = 1,
          FirstName = "Julian",
          LastName = "Rooze",
          Age = 24,
          Roles = new List<string>
          {
            "test",
            "test"
          },
          Address = new Address
          {
            ID = 1,
            Street = "Street"
          },
          Parent = new Customer
          {
            Name = "Parent",
            ID = 1,
            Roles = new List<string>
            {
              "test"
            },
            Address = new Address
            {
              ID = 1,
              Street = "Street 1"
            }
          }
        };

        return customer;
      }

    private void DoAsserts(Customer original, Customer deserialized)
    {
      Assert.AreEqual(original.Name, deserialized.Name);
      Assert.AreEqual(original.ID, deserialized.ID);
      Assert.AreEqual(original.FirstName, deserialized.FirstName);
      Assert.AreEqual(original.LastName, deserialized.LastName);
      Assert.AreEqual(original.Age, deserialized.Age);
      Assert.IsTrue(original.Roles.SequenceEqual(deserialized.Roles));

      if (original.Address != null)
      {
        Assert.AreEqual(original.Address.Street, deserialized.Address.Street);
        Assert.AreEqual(original.Address.ID, deserialized.Address.ID);
      }

      Assert.AreEqual(original.Parent == null, deserialized.Parent == null);

      if (original.Parent != null)
      {
        DoAsserts(original.Parent, deserialized.Parent);
      }

    }

    [TestMethod]
    public void VerifyTemplate()
    {
      var customer = Create();

      var serialized = TypeSerializer.SerializeToString(customer);

      var jsv = new JsvConverter();

      var deserialized = jsv.DeserializeFromString<Customer>(serialized);

      DoAsserts(customer, deserialized);
    }


  }
}
