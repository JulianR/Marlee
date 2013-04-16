using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using Marlee.Jsv;
using System.Diagnostics;

namespace Marlee.Benchmarks
{

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
    public IList<int> OrganizationUnitIDs { get; set; }
  }

  [Serializable]
  public class Address
  {
    public string Street { get; set; }
    public int ID { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
  }


  public static class ComplexBenchmarks
  {

    private static Customer Create()
    {
      var customer = new Customer
      {
        Name = "Julian Rooze",
        ID = 1,
        FirstName = "Julian",
        LastName = "Rooze",
        Age = 24,
        OrganizationUnitIDs = new []
        {
          10,124,3431,1
        },
        Roles = new List<string>
          {
            "test",
            "test"
          },
        Address = new Address
        {
          ID = 1,
          Street = "Street",
          //Longitude = 5.21788000,
          //Latitude = 52.36897000
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

    public static void Benchmark()
    {
      var instance = Create();

      var str = TypeSerializer.SerializeToString(instance);

      var result = TypeSerializer.DeserializeFromString<Customer>(str);

      var jsv = new JsvConverter();
      result = null;
      result = jsv.DeserializeFromString<Customer>(str);

      Console.WriteLine(result);

      var sw = Stopwatch.StartNew();

      for (var i = 0; i < 1000000; i++)
      {
        result = TypeSerializer.DeserializeFromString<Customer>(str);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);

      sw.Restart();

      for (var i = 0; i < 1000000; i++)
      {
        result = jsv.DeserializeFromString<Customer>(str);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);
    }
  }
}
