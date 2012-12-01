using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Marlee.Common.Serialization
{
  internal delegate void SerializationHandler<T>(TextWriter writer, T obj);
}
