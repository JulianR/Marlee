﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Common.Deserialization
{
  internal delegate T DeserializeHandler<T>(ref int start, string data);
}
