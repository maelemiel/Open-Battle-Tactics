using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class IifFunction : UnaryExpression
	{
		private IExpression trueExpr;

		private IExpression falseExpr;

		public IifFunction(IExpression e, IExpression trueExpr, IExpression falseExpr)
			: base(e)
		{
			this.trueExpr = trueExpr;
			this.falseExpr = falseExpr;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is IifFunction))
			{
				return false;
			}
			IifFunction iifFunction = (IifFunction)obj;
			if (!iifFunction.falseExpr.Equals(falseExpr))
			{
				return false;
			}
			if (!iifFunction.trueExpr.Equals(trueExpr))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode ^= falseExpr.GetHashCode();
			return hashCode ^ trueExpr.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			object obj = expr.Eval(row);
			if (obj == DBNull.Value)
			{
				return obj;
			}
			return (!Convert.ToBoolean(obj)) ? falseExpr.Eval(row) : trueExpr.Eval(row);
		}
	}
}
