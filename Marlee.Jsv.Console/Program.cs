using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Marlee.Jsv.Console
{
  class Program
  {
    [Serializable]
    class TestClass
    {
      public int Test { get; set; }
    }

    static void Main(string[] args)
    {
      SerializeTest();
      SerializeTest();
      //QuoteTest();
      //QuoteTest();
      //GenerateIntLookup(100);
      //var str = "{Test:1E6}";

      //var result = TypeSerializer.DeserializeFromString<TestClass>(str);


    }

    private static void GenerateIntLookup(int count)
    {
      var sb = new StringBuilder();

      for (var i = 0; i < count; i++)
      {
        sb.AppendLine(string.Format(@"case ""{0}"": return {0};", i));
      }

      sb.ToString();

    }

    public static string Result;

    private static void SerializeTest()
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
        PhoneNumbers = new List<PhoneNumber>
        {
          new PhoneNumber
          {
            Type = PhoneNumberTypes.Mobile,
            Number = "124234234"
          }
        }
      };

      //UnsafeSerializationTest.SerializeCustomerBytes(customer);

      var sw = Stopwatch.StartNew();

      for (var i = 0; i < 1000000; i++)
      {
        var sb = new StringWriter(new StringBuilder(256));

        SerializeCustomer(sb, customer);

        Result = sb.ToString();

        var bytes = Encoding.UTF8.GetBytes(Result);

        new MemoryStream().Write(bytes, 0, bytes.Length);
      }

      sw.Stop();

      //var x = new JsvConverter().DeserializeFromString<Customer>(Result);

      //System.Console.WriteLine(x);

      System.Console.WriteLine("Safe: " + sw.Elapsed);

      sw.Restart();

      for (var i = 0; i < 1000000; i++)
      {
        using (var stream = new MemoryStream(256))
        using (var writer = new StreamWriter(stream, Encoding.UTF8, 256))
        {

          SerializeCustomer(writer, customer);

        }
      }

      sw.Stop();

      //var x = new JsvConverter().DeserializeFromString<Customer>(Result);

      //System.Console.WriteLine(x);

      System.Console.WriteLine("Stream: " + sw.Elapsed);

      //sw.Restart();

      //for (var i = 0; i < 1000000; i++)
      //{
      //  Result = TypeSerializer.SerializeToString(customer);
      //}

      //sw.Stop();

      //System.Console.WriteLine(sw.Elapsed);

      //sw.Restart();

      //for (var i = 0; i < 1000000; i++)
      //{
      //  TypeSerializer.SerializeToStream(customer, new MemoryStream());
      //}

      //sw.Stop();

      //System.Console.WriteLine(sw.Elapsed);

      sw.Restart();

      for (var i = 0; i < 1000000; i++)
      {
        UnsafeSerializationTest.SerializeCustomerBytes(customer, new MemoryStream());
      }

      sw.Stop();

      System.Console.WriteLine("Unsafe: " + sw.Elapsed);

    }

    public static bool Foo;

    private static void QuoteTest()
    {

      //var r = new []
      //{
      //  new RegexCompilationInfo("(\\[|\\]|\\{|\\}|\\,|\\\")", RegexOptions.Compiled,"CompiledPATTERN", "Chapter_Code", true)
      //};

      //var aName = new System.Reflection.AssemblyName();
      //aName.Name = "Test";
      //Regex.CompileToAssembly(r, aName);

      var quotes = new Regex("(\\[|\\]|\\{|\\}|\\,|\\\")", RegexOptions.Compiled);


      var sw = Stopwatch.StartNew();

      for (var i = 0; i < 1000000; i++)
      {
        Foo = quotes.IsMatch("absdfsdffwfsdfdf");
        Foo = quotes.IsMatch("absdfsdf,fwfsdfdf");
        Foo = quotes.IsMatch(",absdfsdf[fwfsdfdf");
        Foo = quotes.IsMatch("absdfsdf]fwfsdfdf");
        Foo = quotes.IsMatch("absdfsdffwf\"sdfdf");
        Foo = quotes.IsMatch("absdfsdffwfsdfdf}");

      }

      sw.Stop();

      System.Console.WriteLine(sw.Elapsed);

      Foo = false;

      sw.Restart();


      for (var i = 0; i < 1000000; i++)
      {
        Foo = Escape("absdfsdffwfsdfdf");
        Foo = Escape("absdfsdf,fwfsdfdf");
        Foo = Escape(",absdfsdf[fwfsdfdf");
        Foo = Escape("absdfsdf]fwfsdfdf");
        Foo = Escape("absdfsdffwf\"sdfdf");
        Foo = Escape("absdfsdffwfsdfdf}");
      }

      sw.Stop();

      System.Console.WriteLine(sw.Elapsed);

    }

    private static bool Escape(string s)
    {
      for (var i = 0; i < s.Length; i++)
      {
        var c = s[i];

        if (c == ',' || c == '"' || c == '{' || c == '}' || c == '[' || c == ']')
        {
          return true;
        }
      }
      return false;
    }

    private enum EscapeOptions
    {
      None,
      Quote,
      Other
    }

    private static EscapeOptions GetEscapeOption(string s)
    {
      for (var i = 0; i < s.Length; i++)
      {
        var c = s[i];

        if (c == ',' || c == '{' || c == '}' || c == '[' || c == ']')
        {
          return EscapeOptions.Other;
        }
        else if (c == '"')
        {
          return EscapeOptions.Quote;
        }
      }
      return EscapeOptions.None;
    }

    private static void WriteString(TextWriter sb, string s)
    {
      sb.Write('"');
      if (s.IndexOf('\"') >= 0)
      {
        sb.Write(s.Replace("\"", "\"\""));
      }
      else
      {
        sb.Write(s);
      }
      sb.Write('"');
    }

    private static void WriteStringOptionalEscape(StringBuilder sb, string s)
    {
      var option = GetEscapeOption(s);

      switch (option)
      {
        case EscapeOptions.None:
          sb.Append(s);
          break;
        case EscapeOptions.Quote:
          sb.Append('"');
          sb.Append(s.Replace("\"", "\"\""));
          sb.Append('"');
          break;
        case EscapeOptions.Other:
          sb.Append('"');
          sb.Append(s);
          sb.Append('"');
          break;
      }
    }



    private static void SerializeCustomer(TextWriter sb, Customer c)
    {
      sb.Write('{');
      if (c.Name != null)
      {
        sb.Write("Name:");
        WriteString(sb, c.Name);
        sb.Write(',');
      }
      sb.Write("ID:");
      sb.Write(c.ID);
      sb.Write(',');

      if (c.Roles != null)
      {
        sb.Write("Roles:[");
        for (var i = 0; i < c.Roles.Count; i++)
        {
          WriteString(sb, c.Roles[i]);
        }
        sb.Write(']');
        sb.Write(',');
      }

      if (c.Address != null)
      {
        SerializeAddress(sb, c.Address);
        sb.Write(',');
      }

      if (c.PhoneNumbers != null)
      {
        sb.Write("PhoneNumbers:[");
        for (var i = 0; i < c.PhoneNumbers.Count; i++)
        {
          SerializePhoneNumber(sb, c.PhoneNumbers[i]);
        }
        sb.Write(']');
        sb.Write(',');
      }

      if (c.FirstName != null)
      {
        sb.Write("FirstName:");
        WriteString(sb, c.FirstName);
        sb.Write(',');
      }

      if (c.LastName != null)
      {
        sb.Write("LastName:");
        WriteString(sb, c.LastName);
        sb.Write(',');
      }

      sb.Write("Age:");
      sb.Write(c.Age);
      sb.Write('}');
    }

    private static void SerializeAddress(TextWriter sb, Address a)
    {
      sb.Write('{');
      if (a.Street != null)
      {
        sb.Write("Street:");
        WriteString(sb, a.Street);
        sb.Write(',');
      }

      sb.Write("ID:");
      sb.Write(a.ID);
      sb.Write('}');
    }

    private static void SerializePhoneNumber(TextWriter sb, PhoneNumber p)
    {
      sb.Write('{');
      if (p.Number != null)
      {
        sb.Write("Number:");
        WriteString(sb, p.Number);
        sb.Write(',');
      }

      sb.Write("Type:");

      switch (p.Type)
      {
        case PhoneNumberTypes.Land:
          sb.Write("Land");
          break;
        case PhoneNumberTypes.Mobile:
          sb.Write("Mobile");
          break;
      }
      sb.Write('}');
    }

    [Serializable]
    public class Customer
    {
      public string Name { get; set; }
      public int ID { get; set; }
      public List<string> Roles { get; set; }
      public Address Address { get; set; }
      //public Customer Parent { get; set; }
      public IList<PhoneNumber> PhoneNumbers { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public int Age { get; set; }
      //public DateTime DateOfBirth { get; set; }
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
  }
}
