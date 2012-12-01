using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Marlee.Common.Tree
{
  internal class KnownTypeNode : MemberNode
  {
    public Type Type { get; set; }
    public MethodInfo Method { get; set; }
  }
}
