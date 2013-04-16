using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Internal
{
  public delegate T DeserializeHandler<T>(ref int start, string data);
  public delegate T ExtractCollectionItem<T>(ref int i, string s);
}
