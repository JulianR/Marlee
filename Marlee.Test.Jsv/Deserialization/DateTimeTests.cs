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
  public class DateTimeTests
  {
    public class DateTimeProperty
    {
      public DateTime Date { get; set; }
    }
    [TestMethod]
    public void Test_1()
    {
      var instance = new DateTimeProperty
      {
        Date = new DateTime(2013, 10, 1, 14, 45, 28, DateTimeKind.Local)
      };

      var serialized = TypeSerializer.SerializeToString(instance);

      var deserializer = new JsvConverter();

      var result = deserializer.DeserializeFromString<DateTimeProperty>(serialized);
    }
  }
}
