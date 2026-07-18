using System.Collections.ObjectModel;
using System.Text;

namespace System.Linq.Expressions
{
	internal class ExpressionPrinter : ExpressionVisitor
	{
		private const string ListSeparator = ", ";

		private StringBuilder builder;

		private ExpressionPrinter(StringBuilder builder)
		{
			this.builder = builder;
		}

		private ExpressionPrinter()
			: this(new StringBuilder())
		{
		}

		public static string ToString(Expression expression)
		{
			ExpressionPrinter expressionPrinter = new ExpressionPrinter();
			expressionPrinter.Visit(expression);
			return expressionPrinter.builder.ToString();
		}

		public static string ToString(ElementInit init)
		{
			ExpressionPrinter expressionPrinter = new ExpressionPrinter();
			expressionPrinter.VisitElementInitializer(init);
			return expressionPrinter.builder.ToString();
		}

		public static string ToString(MemberBinding binding)
		{
			ExpressionPrinter expressionPrinter = new ExpressionPrinter();
			expressionPrinter.VisitBinding(binding);
			return expressionPrinter.builder.ToString();
		}

		private void Print(string str)
		{
			builder.Append(str);
		}

		private void Print(object obj)
		{
			builder.Append(obj);
		}

		private void Print(string str, params object[] objs)
		{
			builder.AppendFormat(str, objs);
		}

		protected override void VisitElementInitializer(ElementInit initializer)
		{
			Print(initializer.AddMethod);
			Print("(");
			VisitExpressionList(initializer.Arguments);
			Print(")");
		}

		protected override void VisitUnary(UnaryExpression unary)
		{
			switch (unary.NodeType)
			{
			case ExpressionType.ArrayLength:
			case ExpressionType.Convert:
			case ExpressionType.ConvertChecked:
			case ExpressionType.Not:
				Print("{0}(", unary.NodeType);
				Visit(unary.Operand);
				Print(")");
				break;
			case ExpressionType.Negate:
				Print("-");
				Visit(unary.Operand);
				break;
			case ExpressionType.Quote:
				Visit(unary.Operand);
				break;
			case ExpressionType.TypeAs:
				Print("(");
				Visit(unary.Operand);
				Print(" As {0})", unary.Type.Name);
				break;
			case ExpressionType.UnaryPlus:
				Print("+");
				Visit(unary.Operand);
				break;
			default:
				throw new NotImplementedException();
			}
		}

		private static string OperatorToString(BinaryExpression binary)
		{
			switch (binary.NodeType)
			{
			case ExpressionType.Add:
			case ExpressionType.AddChecked:
				return "+";
			case ExpressionType.AndAlso:
				return "&&";
			case ExpressionType.Coalesce:
				return "??";
			case ExpressionType.Divide:
				return "/";
			case ExpressionType.Equal:
				return "=";
			case ExpressionType.ExclusiveOr:
				return "^";
			case ExpressionType.GreaterThan:
				return ">";
			case ExpressionType.GreaterThanOrEqual:
				return ">=";
			case ExpressionType.LeftShift:
				return "<<";
			case ExpressionType.LessThan:
				return "<";
			case ExpressionType.LessThanOrEqual:
				return "<=";
			case ExpressionType.Modulo:
				return "%";
			case ExpressionType.Multiply:
			case ExpressionType.MultiplyChecked:
				return "*";
			case ExpressionType.NotEqual:
				return "!=";
			case ExpressionType.OrElse:
				return "||";
			case ExpressionType.Power:
				return "^";
			case ExpressionType.RightShift:
				return ">>";
			case ExpressionType.Subtract:
			case ExpressionType.SubtractChecked:
				return "-";
			case ExpressionType.And:
				return (!IsBoolean(binary)) ? "&" : "And";
			case ExpressionType.Or:
				return (!IsBoolean(binary)) ? "|" : "Or";
			default:
				return null;
			}
		}

		private static bool IsBoolean(Expression expression)
		{
			return expression.Type == typeof(bool) || expression.Type == typeof(bool?);
		}

		private void PrintArrayIndex(BinaryExpression index)
		{
			Visit(index.Left);
			Print("[");
			Visit(index.Right);
			Print("]");
		}

		protected override void VisitBinary(BinaryExpression binary)
		{
			ExpressionType nodeType = binary.NodeType;
			if (nodeType == ExpressionType.ArrayIndex)
			{
				PrintArrayIndex(binary);
				return;
			}
			Print("(");
			Visit(binary.Left);
			Print(" {0} ", OperatorToString(binary));
			Visit(binary.Right);
			Print(")");
		}

		protected override void VisitTypeIs(TypeBinaryExpression type)
		{
			ExpressionType nodeType = type.NodeType;
			if (nodeType == ExpressionType.TypeIs)
			{
				Print("(");
				Visit(type.Expression);
				Print(" Is {0})", type.TypeOperand.Name);
				return;
			}
			throw new NotImplementedException();
		}

		protected override void VisitConstant(ConstantExpression constant)
		{
			object value = constant.Value;
			if (value == null)
			{
				Print("null");
			}
			else if (value is string)
			{
				Print("\"");
				Print(value);
				Print("\"");
			}
			else if (!HasStringRepresentation(value))
			{
				Print("value(");
				Print(value);
				Print(")");
			}
			else
			{
				Print(value);
			}
		}

		private static bool HasStringRepresentation(object obj)
		{
			return obj.ToString() != obj.GetType().ToString();
		}

		protected override void VisitConditional(ConditionalExpression conditional)
		{
			Print("IIF(");
			Visit(conditional.Test);
			Print(", ");
			Visit(conditional.IfTrue);
			Print(", ");
			Visit(conditional.IfFalse);
			Print(")");
		}

		protected override void VisitParameter(ParameterExpression parameter)
		{
			Print(parameter.Name ?? "<param>");
		}

		protected override void VisitMemberAccess(MemberExpression access)
		{
			if (access.Expression == null)
			{
				Print(access.Member.DeclaringType.Name);
			}
			else
			{
				Visit(access.Expression);
			}
			Print(".{0}", access.Member.Name);
		}

		protected override void VisitMethodCall(MethodCallExpression call)
		{
			if (call.Object != null)
			{
				Visit(call.Object);
				Print(".");
			}
			Print(call.Method.Name);
			Print("(");
			VisitExpressionList(call.Arguments);
			Print(")");
		}

		protected override void VisitMemberAssignment(MemberAssignment assignment)
		{
			Print("{0} = ", assignment.Member.Name);
			Visit(assignment.Expression);
		}

		protected override void VisitMemberMemberBinding(MemberMemberBinding binding)
		{
			Print(binding.Member.Name);
			Print(" = {");
			VisitList(binding.Bindings, VisitBinding);
			Print("}");
		}

		protected override void VisitMemberListBinding(MemberListBinding binding)
		{
			Print(binding.Member.Name);
			Print(" = {");
			VisitList(binding.Initializers, VisitElementInitializer);
			Print("}");
		}

		protected override void VisitList<T>(ReadOnlyCollection<T> list, Action<T> visitor)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (i > 0)
				{
					Print(", ");
				}
				visitor(list[i]);
			}
		}

		protected override void VisitLambda(LambdaExpression lambda)
		{
			if (lambda.Parameters.Count != 1)
			{
				Print("(");
				VisitList(lambda.Parameters, Visit);
				Print(")");
			}
			else
			{
				Visit(lambda.Parameters[0]);
			}
			Print(" => ");
			Visit(lambda.Body);
		}

		protected override void VisitNew(NewExpression nex)
		{
			Print("new {0}(", nex.Type.Name);
			if (nex.Members != null && nex.Members.Count > 0)
			{
				for (int i = 0; i < nex.Members.Count; i++)
				{
					if (i > 0)
					{
						Print(", ");
					}
					Print("{0} = ", nex.Members[i].Name);
					Visit(nex.Arguments[i]);
				}
			}
			else
			{
				VisitExpressionList(nex.Arguments);
			}
			Print(")");
		}

		protected override void VisitMemberInit(MemberInitExpression init)
		{
			Visit(init.NewExpression);
			Print(" {");
			VisitList(init.Bindings, VisitBinding);
			Print("}");
		}

		protected override void VisitListInit(ListInitExpression init)
		{
			Visit(init.NewExpression);
			Print(" {");
			VisitList(init.Initializers, VisitElementInitializer);
			Print("}");
		}

		protected override void VisitNewArray(NewArrayExpression newArray)
		{
			Print("new ");
			switch (newArray.NodeType)
			{
			case ExpressionType.NewArrayBounds:
				Print(newArray.Type);
				Print("(");
				VisitExpressionList(newArray.Expressions);
				Print(")");
				break;
			case ExpressionType.NewArrayInit:
				Print("[] {");
				VisitExpressionList(newArray.Expressions);
				Print("}");
				break;
			default:
				throw new NotSupportedException();
			}
		}

		protected override void VisitInvocation(InvocationExpression invocation)
		{
			Print("Invoke(");
			Visit(invocation.Expression);
			if (invocation.Arguments.Count != 0)
			{
				Print(", ");
				VisitExpressionList(invocation.Arguments);
			}
			Print(")");
		}
	}
}
