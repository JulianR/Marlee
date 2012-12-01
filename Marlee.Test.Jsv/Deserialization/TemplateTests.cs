using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Marlee.Jsv;
using ServiceStack.Text;
using Marlee.Internal;

namespace Marlee.Test.Jsv.Deserialization
{
  [TestClass]
  public class TemplateTests
  {


    private static Customer Create()
    {
      var customer = new Customer
      {
        Name = "Julian Rooze",
        ID = 1,
        //DateOfBirth = DateTime.Now,
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
        PhoneNumbers = new List<PhoneNumber>
        {
          new PhoneNumber
          {
            Type = PhoneNumberTypes.Mobile,
            Number = "124234234"
          }
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

      if (original.PhoneNumbers != null)
      {
        for (var i = 0; i < original.PhoneNumbers.Count; i++)
        {
          var originalNr = original.PhoneNumbers[i];
          var deserializedNr = deserialized.PhoneNumbers[i];

          Assert.AreEqual(originalNr.Number, deserializedNr.Number);
          Assert.AreEqual(originalNr.Type, deserializedNr.Type);
        }
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

      int i = 0;

      var deserialized = ExtractCustomer(ref i, serialized);

      DoAsserts(customer, deserialized);
    }

    [Serializable]
    public class Customer
    {
      public string Name { get; set; }
      public int ID { get; set; }
      public List<string> Roles { get; set; }
      public Address Address { get; set; }
      public Customer Parent { get; set; }
      public IList<PhoneNumber> PhoneNumbers { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public int Age { get; set; }
      public DateTime DateOfBirth { get; set; }
    }

    [Serializable]
    public enum PhoneNumberTypes
    {
      Land = 0,
      Mobile = 1
    }

    [Serializable]
    public class PhoneNumber
    {
      public PhoneNumberTypes Type { get; set; }
      public string Number { get; set; }
    }

    [Serializable]
    public class Address
    {
      public string Street { get; set; }
      public int ID { get; set; }
    }

    private static Customer ExtractCustomer(ref int start, string str)
    {
      var newCustomer = new Customer();

      int end;

      for (var i = start; i < str.Length; i++)
      {
        var c = str[i];

        if (StandardFunctions.IgnoreChar(c))
        {
          continue;
        }

        switch (c)
        {
          case ',':
          case '{':
            continue;
          case '}':
            start = i;
            return newCustomer;
        }

        end = -1;
        int sub = 0;

        for (var j = i; j < str.Length; j++)
        {
          c = str[j];

          if (StandardFunctions.IgnoreChar(c))
          {
            sub++;
          }
          else if (c == ':')
          {
            end = j;
            break;
          }
        }

        if (end < 0) throw new InvalidOperationException();

        var subStr = str.Substring(i, end - i - sub);

        i = end + 1;

        switch (subStr)
        {
          case "Name":
            newCustomer.Name = StandardFunctions.ExtractString(ref i, str);
            break;
          case "FirstName":
            newCustomer.FirstName = StandardFunctions.ExtractString(ref i, str);
            break;
          case "LastName":
            newCustomer.LastName = StandardFunctions.ExtractString(ref i, str);
            break;
          case "Age":
            newCustomer.Age = StandardFunctions.ExtractInt32(ref i, str);
            break;
          case "ID":
            newCustomer.ID = StandardFunctions.ExtractInt32(ref i, str);
            break;
          case "Address":
            var address = ExtractAddress(ref i, str);
            newCustomer.Address = address;
            break;
          case "Roles":
            newCustomer.Roles = StandardFunctions.ExtractStringCollection<List<string>>(ref i, str, new List<string>());
            break;
          case "Parent":
            newCustomer.Parent = ExtractCustomer(ref i, str);
            break;
          case "PhoneNumbers":
            var numbers = ExtractPhoneNumbersCollection<List<PhoneNumber>>(ref i, str, new List<PhoneNumber>());
            newCustomer.PhoneNumbers = numbers;
            break;
          default:
            StandardFunctions.Skip(ref i, str);
            break;
        }
      }

      return newCustomer;
    }

    private static T ExtractPhoneNumbersCollection<T>(ref int start, string str, T instance) where T : ICollection<PhoneNumber>, new()
    {
      char c;

      for (var i = start; i < str.Length; i++)
      {
        c = str[i];

        if (c == '[') continue;

        if (StandardFunctions.IgnoreChar(c)) continue;

        if (c == ']')
        {
          start = i;
          return instance;
        }

        var nr = ExtractPhoneNumber(ref i, str);

        instance.Add(nr);
      }

      return default(T);
    }

    private static PhoneNumberTypes ExtractPhoneNumberTypes(ref int start, string str)
    {
      int end = -1;
      int sub = 0;

      for (var i = start; i < str.Length; i++)
      {
        var c = str[i];

        if (c == ',')
        {
          end = i;
          break;
        }

        if (c == '}' || c == ']')
        {
          end = i;
          sub = 1;
          break;
        }
      }

      if (end < 0) throw new InvalidOperationException();

      var subStr = str.Substring(start, end - start);

      switch (subStr)
      {
        case "Mobile":
          start = end - sub;
          return PhoneNumberTypes.Mobile;
        case "Land":
          start = end - sub;
          return PhoneNumberTypes.Land;
        default:
          return default(PhoneNumberTypes);
      }
    }

    private static PhoneNumber ExtractPhoneNumber(ref int start, string str)
    {
      var number = new PhoneNumber();

      int end;

      for (var i = start; i < str.Length; i++)
      {
        var c = str[i];

        if (StandardFunctions.IgnoreChar(c)) continue;

        switch (c)
        {
          case ',':
          case '{':
            continue;
          case '}':
            start = i;
            return number;
        }

        end = -1;
        int sub = 0;
        for (var j = i; j < str.Length; j++)
        {
          c = str[j];

          if (StandardFunctions.IgnoreChar(c))
          {
            sub++;
          }
          else if (c == ':')
          {
            end = j;
            break;
          }
        }

        if (end < 0) throw new InvalidOperationException();

        var subStr = str.Substring(i, end - i - sub);

        i = end + 1;

        switch (subStr)
        {
          case "Number":
            number.Number = StandardFunctions.ExtractString(ref i, str);
            break;
          case "Type":
            number.Type = ExtractPhoneNumberTypes(ref i, str);
            break;
          default:
            StandardFunctions.Skip(ref i, str);
            break;
        }

      }

      throw new InvalidOperationException();
    }

    private static Address ExtractAddress(ref int start, string str)
    {
      var address = new Address();

      int end;

      for (var i = start; i < str.Length; i++)
      {
        var c = str[i];

        if (StandardFunctions.IgnoreChar(c)) continue;

        switch (c)
        {
          case ',':
          case '{':
            continue;
          case '}':
            start = i;
            return address;
        }

        end = -1;
        int sub = 0;

        for (var j = i; j < str.Length; j++)
        {
          c = str[j];

          if (StandardFunctions.IgnoreChar(c))
          {
            sub++;
          }
          else if (c == ':')
          {
            end = j;
            break;
          }
        }

        if (end < 0) throw new InvalidOperationException();

        var subStr = str.Substring(i, end - i - sub);

        i = end + 1;

        switch (subStr)
        {
          case "Street":
            address.Street = StandardFunctions.ExtractString(ref i, str);
            break;
          case "ID":
            address.ID = StandardFunctions.ExtractInt32(ref i, str);
            break;
          default:
            StandardFunctions.Skip(ref i, str);
            break;
        }

      }
      throw new InvalidOperationException();
    }

  }
}
