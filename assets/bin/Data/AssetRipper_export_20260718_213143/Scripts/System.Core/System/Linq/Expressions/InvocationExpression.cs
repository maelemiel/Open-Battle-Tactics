using System.Collections.ObjectModel;

namespace System.Linq.Expressions
{
	public sealed class InvocationExpression : Expression
	{
		private Expression expression;

		private ReadOnlyCollection<Expression> arguments;

		public Expression Expression
		{
			get
			{
				return expression;
			}
		}

		public ReadOnlyCollection<Expression> Arguments
		{
			get
			{
				return arguments;
			}
		}

		internal InvocationExpression(Expression expression, Type type, ReadOnlyCollection<Expression> arguments)
			: base(ExpressionType.Invoke, type)
		{
			this.expression = expression;
			this.arguments = arguments;
		}

		internal override void Emit(EmitContext ec)
		{
			ec.EmitCall(expression, arguments, expression.Type.GetInvokeMethod());
		}
	}
}
