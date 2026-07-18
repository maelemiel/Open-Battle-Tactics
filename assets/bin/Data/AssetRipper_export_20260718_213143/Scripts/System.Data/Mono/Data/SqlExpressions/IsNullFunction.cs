using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class IsNullFunction : UnaryExpression
	{
		private IExpression defaultExpr;

		public IsNullFunction(IExpression e, IExpression defaultExpr)
			: base(e)
		{
			this.defaultExpr = defaultExpr;
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
			IsNullFunction isNullFunction = (IsNullFunction)obj;
			if (!isNullFunction.defaultExpr.Equals(defaultExpr))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return defaultExpr.GetHashCode() ^ base.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			object obj = expr.Eval(row);
			if (obj == null || obj == DBNull.Value)
			{
				return defaultExpr.Eval(row);
			}
			return obj;
		}
	}
}
