using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Marlee.Benchmarks
{
  public class SwitchTest
  {
    class Customer
    {
      public int ID { get; set; }
      public string Name { get; set; }
      public string City { get; set; }
    }

    public static int Foo { get; set; }

    public static void Benchmark()
    {
      string id = "ID";
      string name = "Name";
      string city = "City";
      var x = Hash(id[0], id[1]);
      var y = Hash(name[0], name[3]);
      var z = Hash(city[0], city[3]);

      var sw = Stopwatch.StartNew();

      for (var i = 0; i < 1000000; i++)
      {
        SwitchString("{ID:", 1, 3);
        SwitchString("{Name:", 1, 5);
        SwitchString("{City:", 1, 5);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);

      sw.Restart();

      for (var i = 0; i < 1000000; i++)
      {
        Switch("{ID:", 1, 3);
        Switch("{Name:", 1, 5);
        Switch("{City:", 1, 5);
      }

      sw.Stop();

      Console.WriteLine(sw.Elapsed);
    }

    public static int Hash(char a, char b)
    {
      return (a * 31) + (b * 31);
    }

    public static void SwitchString(string s, int start, int end)
    {
      var str = s.Substring(start, end - start);

      switch (str)
      {
        case "ID":
          Foo++;
          break;
        case "Name":
          Foo++;
          break;
        case "City":
          Foo--;
          break;
      }
    }

    public static void Switch(string s, int start, int end)
    {
      var str = Hash(s[start], s[end-1]);

      switch (str)
      {
        case 4371:
          Foo++;
          break;
        case 5549:
          Foo++;
          break;
        case 5828:
          Foo--;
          break;
      }
    }
  }
}
