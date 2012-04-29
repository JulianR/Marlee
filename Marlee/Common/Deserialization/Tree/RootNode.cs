using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Common.Deserialization.Tree
{
  internal class RootNode : Node
  {
    public Type Type { get; set; }

    public List<Node> Children { get; set; }
  }
}
