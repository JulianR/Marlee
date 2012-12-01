using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Common.Tree
{
  internal abstract class MemberNode : Node
  {
    public PropertyOrFieldInfo Member { get; set; }
  }
}
