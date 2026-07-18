using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal abstract class UnaryExpression : BaseExpression
	{
		protected IExpression expr;

		public UnaryExpression(IExpression e)
		{
			expr = e;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is UnaryExpression))
			{
				return false;
			}
			UnaryExpression unaryExpression = (UnaryExpression)obj;
			if (!unaryExpression.expr.Equals(expr))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ expr.GetHashCode();
		}

		public override bool DependsOn(DataColumn other)
		{
			return expr.DependsOn(other);
		}

		public override bool EvalBoolean(DataRow row)
		{
			return (bool)Eval(row);
		}
	}
}
