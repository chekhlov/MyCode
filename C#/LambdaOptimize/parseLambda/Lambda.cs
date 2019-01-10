using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace parseLambda
{
	public class Lambda
	{
		public Expression LambdaExpression { get; set; }

		public Lambda()
		{
		}

		public Lambda(Expression Lambda)
		{
			LambdaExpression = Lambda;
		}

/*
		// Математические функции
     Add,
		AddChecked,
    Divide,
    Multiply,
    MultiplyChecked,
    Subtract,
    SubtractChecked,

		// Условия
		Equal,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    NotEqual,

		// Логические операции
    And,
    AndAlso,
		Or,
    OrElse,
		
			// Условие
    Conditional,

		// Лямбда выражение	
    Lambda,

		// Унарные операции
    Increment,
    Decrement,
			

     Coalesce,

		[__DynamicallyInvokable] ArrayLength,
    [__DynamicallyInvokable] ArrayIndex,
    [__DynamicallyInvokable] Call,
    [__DynamicallyInvokable] Constant,
    [__DynamicallyInvokable] Convert,
    [__DynamicallyInvokable] ConvertChecked,

			[__DynamicallyInvokable] ExclusiveOr,
    [__DynamicallyInvokable] Invoke,
    [__DynamicallyInvokable] LeftShift,
    [__DynamicallyInvokable] ListInit,
    [__DynamicallyInvokable] MemberAccess,
    [__DynamicallyInvokable] MemberInit,
    [__DynamicallyInvokable] Modulo,
    [__DynamicallyInvokable] Negate,
    [__DynamicallyInvokable] UnaryPlus,
    [__DynamicallyInvokable] NegateChecked,
    [__DynamicallyInvokable] New,
    [__DynamicallyInvokable] NewArrayInit,
    [__DynamicallyInvokable] NewArrayBounds,
    [__DynamicallyInvokable] Not,
    [__DynamicallyInvokable] Parameter,
    [__DynamicallyInvokable] Power,
    [__DynamicallyInvokable] Quote,
    [__DynamicallyInvokable] RightShift,
    [__DynamicallyInvokable] TypeAs,
    [__DynamicallyInvokable] TypeIs,
    [__DynamicallyInvokable] Assign,
    [__DynamicallyInvokable] Block,
    [__DynamicallyInvokable] DebugInfo,
    [__DynamicallyInvokable] Dynamic,
    [__DynamicallyInvokable] Default,
    [__DynamicallyInvokable] Extension,
    [__DynamicallyInvokable] Goto,
    [__DynamicallyInvokable] Index,
    [__DynamicallyInvokable] Label,
    [__DynamicallyInvokable] RuntimeVariables,
    [__DynamicallyInvokable] Loop,
    [__DynamicallyInvokable] Switch,
    [__DynamicallyInvokable] Throw,
    [__DynamicallyInvokable] Try,
    [__DynamicallyInvokable] Unbox,
    [__DynamicallyInvokable] AddAssign,
    [__DynamicallyInvokable] AndAssign,
    [__DynamicallyInvokable] DivideAssign,
    [__DynamicallyInvokable] ExclusiveOrAssign,
    [__DynamicallyInvokable] LeftShiftAssign,
    [__DynamicallyInvokable] ModuloAssign,
    [__DynamicallyInvokable] MultiplyAssign,
    [__DynamicallyInvokable] OrAssign,
    [__DynamicallyInvokable] PowerAssign,
    [__DynamicallyInvokable] RightShiftAssign,
    [__DynamicallyInvokable] SubtractAssign,
    [__DynamicallyInvokable] AddAssignChecked,
    [__DynamicallyInvokable] MultiplyAssignChecked,
    [__DynamicallyInvokable] SubtractAssignChecked,
    [__DynamicallyInvokable] PreIncrementAssign,
    [__DynamicallyInvokable] PreDecrementAssign,
    [__DynamicallyInvokable] PostIncrementAssign,
    [__DynamicallyInvokable] PostDecrementAssign,
    [__DynamicallyInvokable] TypeEqual,
    [__DynamicallyInvokable] OnesComplement,
    [__DynamicallyInvokable] IsTrue,
    [__DynamicallyInvokable] IsFalse,
 */

		private class NodeReplace
		{
			public string name;
			public ParameterExpression param = null;
			public MethodCallExpression call = null;
		}

		private List<NodeReplace> optimizeCall;


		private Expression ParseExpression(Expression node, bool isRoot = false)
		{
			BinaryExpression be;
			Expression newNode = node;
			switch (node.NodeType) {
				case ExpressionType.Lambda:
						var le = (LambdaExpression) node;
					  int prevCntParam = optimizeCall.Count;
					  var body = ParseExpression(le.Body);
						int cntParam = optimizeCall.Count;
						var param = le.Parameters.ToList();
						for (int i = prevCntParam; i < cntParam; i++) {
							param.Add(optimizeCall[i].param);	
						}
					  newNode = Expression.Lambda(body, param);
						break;
				case ExpressionType.Add:
					be = (BinaryExpression) node;
						newNode = Expression.Add(ParseExpression(be.Left), ParseExpression(be.Right));
  					break;
				case ExpressionType.Subtract: 
					be = (BinaryExpression) node;
					newNode = Expression.Subtract(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.Multiply: 
					be = (BinaryExpression) node;
					newNode = Expression.Multiply(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.Divide:
					be = (BinaryExpression) node;
					newNode = Expression.Divide(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.AddChecked:
					be = (BinaryExpression) node;
					newNode = Expression.AddChecked(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.SubtractChecked:
					be = (BinaryExpression) node;
					newNode = Expression.SubtractChecked(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.MultiplyChecked: 
					be = (BinaryExpression) node;
					newNode = Expression.MultiplyChecked(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.Conditional:
					var ce = (ConditionalExpression) node;
					newNode = Expression.Condition(ParseExpression(ce.Test), ParseExpression(ce.IfTrue), ParseExpression(ce.IfFalse));
					break;
				case ExpressionType.Equal:
					be = (BinaryExpression) node;
					newNode = Expression.Equal(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.GreaterThan:
					be = (BinaryExpression) node;
					newNode = Expression.GreaterThan(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.GreaterThanOrEqual:
					be = (BinaryExpression) node;
					newNode = Expression.GreaterThanOrEqual(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.LessThan:
					be = (BinaryExpression) node;
					newNode = Expression.LessThan(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.NotEqual:
					be = (BinaryExpression) node;
					newNode = Expression.NotEqual(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.Or:
					be = (BinaryExpression) node;
					newNode = Expression.Or(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.OrElse:
					be = (BinaryExpression) node;
					newNode = Expression.OrElse(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.And:
					be = (BinaryExpression) node;
					newNode = Expression.And(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.AndAlso:
					be = (BinaryExpression) node;
					newNode = Expression.OrElse(ParseExpression(be.Left), ParseExpression(be.Right));
					break;
				case ExpressionType.OrAssign:
					be = (BinaryExpression) node;
					newNode = Expression.OrAssign(ParseExpression(be.Left), ParseExpression(be.Right));
					break;

				case ExpressionType.Coalesce:
					be = (BinaryExpression) node;
					newNode = Expression.Coalesce(ParseExpression(be.Left), ParseExpression(be.Right), (LambdaExpression) ParseExpression(be.Conversion));
					break;

				case ExpressionType.Increment:
				case ExpressionType.Decrement:
				case ExpressionType.Parameter:
				case ExpressionType.Constant:
					newNode = node;
					break;
				case ExpressionType.Call:
					var ca = (MethodCallExpression) node;
					var caName = ca.ToString();
					var el = optimizeCall.FirstOrDefault(s => s.name == caName);
					if (el != null) {
						newNode = el.param;
					}
					else {
						newNode = Expression.Parameter(ca.Type, $"p{optimizeCall.Count}");
						optimizeCall.Add(new NodeReplace() {
							name = caName, 
							call = ca,
							param = (ParameterExpression) newNode
						});
					}

					break;
				default: newNode = node;
					break;
			}

			return newNode;

		}


		public object CalculateLambda(Expression optimizedLambda, object[] lambdaParams)
		{
			LambdaExpression lambdaExpr =(LambdaExpression) optimizedLambda;
			var result = lambdaExpr.Compile().DynamicInvoke(lambdaParams);
			return result;
		}

		public Expression OptimizeLambda(Expression Lambda)
		{
			optimizeCall = new List<NodeReplace>();
			var optimizedLambda = ParseExpression(Lambda);
			return optimizedLambda;
		} 
		private object CalculateFunction(NodeReplace node, Expression Lambda, object[] lambdaParams)
		{

			var param = ((LambdaExpression) Lambda).Parameters;

			LambdaExpression lambda = Expression.Lambda(node.call, param);
			var res = lambda.Compile().DynamicInvoke(lambdaParams.ToArray());

			return res;
		}


		public object OptimizeCalculation(Expression Lambda, object[] lambdaParams)
		{
			var optimizedlambda = OptimizeLambda(Lambda);

			// вычисляем параметры вида p0..px
			var param = new List<object>(lambdaParams);
			foreach (var el in optimizeCall) {
				var res = CalculateFunction(el, Lambda, lambdaParams);
				param.Add(res);
			}

			return CalculateLambda(optimizedlambda, param.ToArray());
		}


	}
}
