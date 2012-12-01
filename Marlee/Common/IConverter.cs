using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Common
{
  internal interface IConverter
  {
    Guid Version { get; }

    //Func<DeserializeCodeGenerator>
  }
}
