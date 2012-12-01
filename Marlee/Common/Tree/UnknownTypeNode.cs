using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Common.Tree
{
  internal class UnknownTypeNode : MemberNode
  {
    public Type Type { get; set; }
    public IList<Node> Children { get; set; }
  }
}
