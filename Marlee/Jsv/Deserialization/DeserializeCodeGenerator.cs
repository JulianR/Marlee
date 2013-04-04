using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marlee.Common.Tree;
using System.Linq.Expressions;
using Marlee.Common.Deserialization;
using System.Reflection;
using System.Reflection.Emit;
using Marlee.Common.Helpers;
using System.Collections;
using Marlee.Internal;

namespace Marlee.Jsv.Deserialization
{
  internal class DeserializeCodeGenerator
  {

    private static readonly MethodInfo _extractStringCollection = typeof(StandardFunctions).GetMethod("ExtractStringCollection");
    private static readonly MethodInfo _extractIntCollection = typeof(StandardFunctions).GetMethod("ExtractInt32Collection");
    private static readonly MethodInfo _extractString = typeof(StandardFunctions).GetMethod("ExtractString");
    private static readonly MethodInfo _extractInt32 = typeof(StandardFunctions).GetMethod("ExtractInt32");
    private static readonly MethodInfo _extractInt64 = typeof(StandardFunctions).GetMethod("ExtractInt64");
    private static readonly PropertyInfo _stringIndexMethod = typeof(string).GetProperties().FirstOrDefault(p => p.GetIndexParameters().Length == 1);
    private static readonly MethodInfo _ignoreFunc = typeof(StandardFunctions).GetMethod("IgnoreChar");
    private static readonly MethodInfo _subStringMethod = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });
    private static readonly MethodInfo _toArray = typeof(Enumerable).GetMethod("ToArray");
    private static readonly MethodInfo _hashProperty = typeof(MemberHashHelper).GetMethod("Hash");


    private static ModuleBuilder _moduleBuilder;
    private static byte[] _syncRoot = new byte[0];

    private TreeBuilder _treeBuilder;

    public DeserializeCodeGenerator(TreeBuilder builder)
    {
      _treeBuilder = builder;
    }

    public Delegate Generate(RootNode root)
    {
      var ctx = new DeserializerTypeContext
      {
        Type = root.Type
      };

      InitializeVars(ctx);

      var @switch = GetSwitchStatement(ctx, root.Children);

      ctx.BlockInsideLoop = GetInnerLoopTemplate(ctx, @switch);

      var lambda = GetTemplate(ctx);

      var del = CompileExpression(root.Type, lambda);

      _treeBuilder.AddKnownType(root.Type, del.Method);

      return del;
    }


    private class DeserializerTypeContext
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
      public Dictionary<int, string> HashValues { get; set; }
      public ParameterExpression PropertyHashValue { get; set; }
    }

    private void InitializeVars(DeserializerTypeContext ctx)
    {
      ctx.InstanceVar = Expression.Variable(ctx.Type, "new" + ctx.Type.Name);

      ctx.EndVar = Expression.Variable(typeof(int), "end");

      ctx.StartParam = Expression.Parameter(typeof(int).MakeByRefType(), "start");

      ctx.IteratorVar = Expression.Variable(typeof(int), "i");

      ctx.StringParam = Expression.Parameter(typeof(string), "str");

      ctx.StringLengthVar = Expression.Variable(typeof(int), "len");

      ctx.CharVar = Expression.Variable(typeof(char), "c");

      ctx.BreakLabel = Expression.Label("break");
      ctx.ContinueLabel = Expression.Label("continue");
      ctx.ReturnLabel = Expression.Label("return");

      ctx.WhiteSpaceCounter = Expression.Variable(typeof(int), "whitespace");

      ctx.SubStringVar = Expression.Variable(typeof(string), "subStr");
    }

    private LambdaExpression GetTemplate(DeserializerTypeContext ctx)
    {
      var newInstance = Expression.New(ctx.Type);

      var assignInstance = Expression.Assign(ctx.InstanceVar, newInstance);

      var terminationCondition = Expression.LessThan(ctx.IteratorVar, ctx.StringLengthVar);

      var initializeIterationVar = Expression.Assign(ctx.IteratorVar, ctx.StartParam);

      var assignLen = Expression.Assign(ctx.StringLengthVar, Expression.Property(ctx.StringParam, "Length"));

      var @for = Expression.Loop(
                        Expression.IfThenElse(
                        terminationCondition,
                            ctx.BlockInsideLoop
                        , Expression.Break(ctx.BreakLabel)), ctx.BreakLabel);

      var bodyExpressions = new List<Expression>();

      bodyExpressions.Add(assignInstance);
      bodyExpressions.Add(initializeIterationVar);
      bodyExpressions.Add(assignLen);
      bodyExpressions.Add(@for);
      bodyExpressions.Add(Expression.Label(ctx.ReturnLabel));
      bodyExpressions.Add(ctx.InstanceVar);

      var body = Expression.Block(new[] // the variables
      { 
        ctx.InstanceVar, 
        ctx.EndVar, 
        ctx.IteratorVar, 
        ctx.StringLengthVar, 
        ctx.CharVar, 
        ctx.WhiteSpaceCounter,
        ctx.PropertyHashValue ?? ctx.SubStringVar 
      },
        // the body
      bodyExpressions);

      var lambdaType = typeof(DeserializeHandler<>).MakeGenericType(ctx.Type);

      var lambda = Expression.Lambda(lambdaType, body, ctx.StartParam, ctx.StringParam);

      return lambda;

    }

    private Expression GetInnerLoopTemplate(DeserializerTypeContext ctx, Expression propertySwitch)
    {
      Expression<Func<string, char>> x = (s) => s[1];

      var accessStringIndex = Expression.MakeIndex(ctx.StringParam, _stringIndexMethod, new[] { ctx.IteratorVar });

      var assignToChar = Expression.Assign(ctx.CharVar, accessStringIndex);

      var invokeIgnoreFunc = Expression.Call(null, _ignoreFunc, ctx.CharVar);

      var ifIgnore = Expression.IfThen(invokeIgnoreFunc, Expression.Continue(ctx.ContinueLabel));

      var increment = Expression.PostIncrementAssign(ctx.IteratorVar);

      var bodyExpressions = new List<Expression>();

      var ifCommaOrBrace = Expression.SwitchCase(Expression.Continue(ctx.ContinueLabel), Expression.Constant(','), Expression.Constant('{'));

      var ifClosingBrace = Expression.SwitchCase(Expression.Block(Expression.Assign(ctx.StartParam, ctx.IteratorVar), Expression.Return(ctx.ReturnLabel)), Expression.Constant('}'));

      var @switch = Expression.Switch(ctx.CharVar, ifCommaOrBrace, ifClosingBrace);

      var resetEndVar = Expression.Assign(ctx.EndVar, Expression.Constant(-1));
      var resetWhiteSpaceVar = Expression.Assign(ctx.WhiteSpaceCounter, Expression.Constant(0));

      var throwIfEndIsNegative = Expression.IfThen(Expression.LessThan(ctx.EndVar, Expression.Constant(0)),
        Expression.Throw(Expression.New(typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) })
        , Expression.Constant("Expected a property, but encountered none."))));

      Expression assignToSwitchValue;

      if (ctx.HashValues == null)
      {
        var callSubString = Expression.Call(ctx.StringParam, _subStringMethod, ctx.IteratorVar,
          Expression.Subtract(Expression.Subtract(ctx.EndVar, ctx.IteratorVar), ctx.WhiteSpaceCounter));

        assignToSwitchValue = Expression.Assign(ctx.SubStringVar, callSubString);
      }
      else
      {
        var callHash = Expression.Call(null, _hashProperty, ctx.StringParam, ctx.IteratorVar,
          Expression.Subtract(ctx.EndVar, ctx.WhiteSpaceCounter));

        assignToSwitchValue = Expression.Assign(ctx.PropertyHashValue, callHash);
      }

      var assignI = Expression.Assign(ctx.IteratorVar, Expression.Add(ctx.EndVar, Expression.Constant(1)));

      bodyExpressions.Add(assignToChar);
      bodyExpressions.Add(ifIgnore);
      bodyExpressions.Add(@switch);
      bodyExpressions.Add(resetEndVar);
      bodyExpressions.Add(resetWhiteSpaceVar);

      bodyExpressions.Add(GetPropertyFinderLoop(ctx));

      bodyExpressions.Add(throwIfEndIsNegative);
      bodyExpressions.Add(assignToSwitchValue);

      bodyExpressions.Add(assignI);
      bodyExpressions.Add(propertySwitch);

      bodyExpressions.Add(Expression.Label(ctx.ContinueLabel));
      bodyExpressions.Add(increment);

      return Expression.Block(bodyExpressions);
    }

    private Expression GetPropertyFinderLoop(DeserializerTypeContext ctx)
    {

      var outerLoopExpressions = new List<Expression>();

      var jParam = Expression.Variable(typeof(int), "j");

      var terminationCondition = Expression.LessThan(jParam, ctx.StringLengthVar);

      var @break = Expression.Label();

      var assignItoJ = Expression.Assign(jParam, ctx.IteratorVar);

      var increment = Expression.PostIncrementAssign(jParam);

      var accessStringIndex = Expression.MakeIndex(ctx.StringParam, _stringIndexMethod, new[] { jParam });

      var assignToChar = Expression.Assign(ctx.CharVar, accessStringIndex);

      var invokeIgnoreFunc = Expression.Call(null, _ignoreFunc, ctx.CharVar);

      var assignJtoEnd = Expression.Assign(ctx.EndVar, jParam);

      var exitLoopBlock = Expression.Block(assignJtoEnd, Expression.Break(@break));

      var ifWhiteSpace = Expression.IfThenElse(invokeIgnoreFunc,
        Expression.PostIncrementAssign(ctx.WhiteSpaceCounter),

        Expression.IfThen(Expression.Equal(ctx.CharVar, Expression.Constant(':')),

        exitLoopBlock));

      var innerLoopExpressions = new List<Expression>();

      innerLoopExpressions.Add(assignToChar);
      innerLoopExpressions.Add(ifWhiteSpace);
      innerLoopExpressions.Add(increment);

      var innerLoopBlock = Expression.Block(innerLoopExpressions);

      var @for = Expression.Loop(
                        Expression.IfThenElse(
                        terminationCondition,
                            innerLoopBlock
                        , Expression.Break(@break)), @break);

      outerLoopExpressions.Add(assignItoJ);
      outerLoopExpressions.Add(@for);

      var outerLoopBlock = Expression.Block(new[] { jParam }, outerLoopExpressions);

      return outerLoopBlock;
    }

    private Expression GetDefaultCase(DeserializerTypeContext ctx)
    {
      var skipMethod = typeof(StandardFunctions).GetMethod("Skip");

      var callSkip = Expression.Call(null, skipMethod, ctx.IteratorVar, ctx.StringParam);

      return callSkip;
    }

    private Expression GetSwitchStatement(DeserializerTypeContext ctx, IList<Node> members)
    {
      var switchCases = new List<SwitchCase>();

      ctx.HashValues = MemberHashHelper.CanUseHashLookup(members.Cast<MemberNode>().Select(m => m.Member.Name));

      foreach (var c in members)
      {
        var caseStatement = ProcessNode(ctx, c as dynamic) as SwitchCase;

        if (caseStatement != null)
        {
          switchCases.Add(caseStatement);
        }
      }

      if (!switchCases.Any())
      {
        return Expression.Empty();
      }

      Expression switchValue;

      if (ctx.HashValues == null)
      {
        switchValue = ctx.SubStringVar;
      }
      else
      {
        ctx.PropertyHashValue = Expression.Variable(typeof(int), "hashValue");

        switchValue = ctx.PropertyHashValue;
      }

      var @switch = Expression.Switch(switchValue, GetDefaultCase(ctx), switchCases.ToArray());
      return @switch;
    }

    private SwitchCase ProcessNode(DeserializerTypeContext ctx, KnownTypeNode node)
    {
      return GetKnownTypeSwitchCase(ctx, node.Method, node);
    }

    private SwitchCase ProcessNode(DeserializerTypeContext ctx, RecursionNode node)
    {
      var method = typeof(RecursionHelper).GetMethod("GetDeserializer").MakeGenericMethod(node.Type);
      
      var del = method.Invoke(null, new[] {_treeBuilder}) as Delegate;

      if (del == null) return null;

      return GetKnownTypeSwitchCase(ctx, del.Method, node, del);
    }

    private SwitchCase GetKnownTypeSwitchCase(DeserializerTypeContext ctx, MethodInfo method, MemberNode node, Delegate del = null)
    {
      var bodyExpressions = new List<Expression>();

      var callExtractString = Expression.Call(null, method, ctx.IteratorVar, ctx.StringParam);

      var accessMember = Expression.MakeMemberAccess(ctx.InstanceVar, node.Member);

      var assignMember = Expression.Assign(accessMember, callExtractString);

      bodyExpressions.Add(assignMember);
      bodyExpressions.Add(Expression.Empty());

      var body = Expression.Block(bodyExpressions);

      var @case = Expression.SwitchCase(body, GetSwitchConstant(ctx, node));

      return @case;
    }

    private SwitchCase ProcessNode(DeserializerTypeContext parentCtx, UnknownTypeNode node)
    {
      var ctx = new DeserializerTypeContext
      {
        Type = node.Member.PropertyOrFieldType
      };

      InitializeVars(ctx);

      var @switch = GetSwitchStatement(ctx, node.Children);

      ctx.BlockInsideLoop = GetInnerLoopTemplate(ctx, @switch);

      var lambda = GetTemplate(ctx);

      var del = CompileExpression(node.Type, lambda);

      _treeBuilder.AddKnownType(node.Type, del.Method);

      return GetKnownTypeSwitchCase(parentCtx, del.Method, node);
    }

    private SwitchCase ProcessNode(DeserializerTypeContext ctx, StringNode node)
    {
      var bodyExpressions = new List<Expression>();

      var callExtractString = Expression.Call(null, _extractString, ctx.IteratorVar, ctx.StringParam);

      var accessMember = Expression.MakeMemberAccess(ctx.InstanceVar, node.Member);

      var assignMember = Expression.Assign(accessMember, callExtractString);

      //var assignToSubStr = Expression.Assign(ctx.SubStringVar, callExtractString);

      bodyExpressions.Add(assignMember);
      bodyExpressions.Add(Expression.Empty());

      var body = Expression.Block(bodyExpressions);

      var @case = Expression.SwitchCase(body, GetSwitchConstant(ctx, node));

      return @case;
    }


    private SwitchCase ProcessNode(DeserializerTypeContext ctx, IntegerNode node)
    {
      var bodyExpressions = new List<Expression>();

      var callExtractInt = Expression.Call(null, _extractInt32, ctx.IteratorVar, ctx.StringParam);

      var accessMember = Expression.MakeMemberAccess(ctx.InstanceVar, node.Member);

      var assignMember = Expression.Assign(accessMember, callExtractInt);

      //var assignToSubStr = Expression.Assign(ctx.SubStringVar, callExtractString);

      bodyExpressions.Add(assignMember);
      bodyExpressions.Add(Expression.Empty());

      var body = Expression.Block(bodyExpressions);

      var @case = Expression.SwitchCase(body, GetSwitchConstant(ctx, node));

      return @case;
    }

    private Expression GetSwitchConstant(DeserializerTypeContext ctx, MemberNode node)
    {
      if (ctx.HashValues == null) return Expression.Constant(node.Member.Name);

      var hash = ctx.HashValues.FirstOrDefault(kv => kv.Value == node.Member.Name);

      if (hash.Value == null) throw new InvalidOperationException("Unknown hash for member " + node.Member);

      return Expression.Constant(hash.Key);
    }

    private Type GetNewCollectionType(Type memberType)
    {
      var typeInsideEnumerable = CollectionTypeHelper.GetTypeInsideEnumerable(memberType);

      return typeof(List<>).MakeGenericType(typeInsideEnumerable);
    }

    private SwitchCase ProcessCollectionNode(DeserializerTypeContext ctx, MemberNode node, MethodInfo method)
    {
      var bodyExpressions = new List<Expression>();

      var collectionType = GetNewCollectionType(node.Member.PropertyOrFieldType);

      method = method.MakeGenericMethod(collectionType);

      var createNewCollection = Expression.New(collectionType);

      var callExtractItems = Expression.Call(null, method, ctx.IteratorVar, ctx.StringParam, createNewCollection);

      var accessMember = Expression.MakeMemberAccess(ctx.InstanceVar, node.Member);

      if (node.Member.PropertyOrFieldType.IsArray)
      {
        var callToArray = Expression.Call(null, _toArray.MakeGenericMethod(node.Member.PropertyOrFieldType.GetElementType()), callExtractItems);
        callExtractItems = callToArray;
      }

      var assignMember = Expression.Assign(accessMember, callExtractItems);

      bodyExpressions.Add(assignMember);
      bodyExpressions.Add(Expression.Empty());

      var body = Expression.Block(bodyExpressions);

      var @case = Expression.SwitchCase(body, GetSwitchConstant(ctx, node));

      return @case;
    }

    private SwitchCase ProcessNode(DeserializerTypeContext ctx, StringCollectionNode node)
    {
      return ProcessCollectionNode(ctx, node, _extractStringCollection);
    }

    private SwitchCase ProcessNode(DeserializerTypeContext ctx, IntCollectionNode node)
    {
      return ProcessCollectionNode(ctx, node, _extractIntCollection);
    }


    private TypeBuilder DefineMappingType(string name)
    {
      lock (_syncRoot)
      {
        if (_moduleBuilder == null)
        {
          var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Jsv_" + Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.RunAndSave);

          _moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
        }
      }
      var typeBuilder = _moduleBuilder.DefineType(name, TypeAttributes.Public);
      return typeBuilder;
    }

    private Delegate CompileExpression(Type t, LambdaExpression expression)
    {
      var typeBuilder = DefineMappingType(string.Format("Deserialize{0}_{1}", t.Name, Guid.NewGuid().ToString("N")));

      var methodBuilder = typeBuilder.DefineMethod(
        "Deserialize",
        MethodAttributes.Public | MethodAttributes.Static,
        t,
        expression.Parameters.Select(p => p.Type).ToArray());

      expression.CompileToMethod(methodBuilder);

      var resultingType = typeBuilder.CreateType();

      var function = Delegate.CreateDelegate(expression.Type, resultingType.GetMethod("Deserialize"));

      return function;
    }

  }
}
