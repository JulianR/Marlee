using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Common.Deserialization
{
  public delegate object DeserializeHandler(ref int start, string data);
}
