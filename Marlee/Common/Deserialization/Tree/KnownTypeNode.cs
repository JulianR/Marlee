using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Common.Deserialization.Tree
{
  internal class KnownTypeNode : MemberNode
  {
    public Type Type { get; set; }
    public IList<Node> Children { get; set; }
  }
}
