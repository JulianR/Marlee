using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Marlee.Jsv.Deserialization
{
  internal class DeserializerTypeContext
  {
    public Type Type { get; set; }
    public Expression BlockInsideLoop { get; set; }
    public ParameterExpression InstanceVar { get; set; }
    public ParameterExpression EndVar { get; set; }
    public ParameterExpression StartParam { get; set; }
    public ParameterExpression StringParam { get; set; }
    public ParameterExpression IteratorVar { get; set; }
    public ParameterExpression StringLengthVar { get; set; }
    public ParameterExpression CharVar { get; set; }
    public LabelTarget BreakLabel { get; set; }
    public LabelTarget ContinueLabel { get; set; }
    public LabelTarget ReturnLabel { get; set; }
    public ParameterExpression WhiteSpaceCounter { get; set; }
    public ParameterExpression SubStringVar { get; set; }
  }
}
