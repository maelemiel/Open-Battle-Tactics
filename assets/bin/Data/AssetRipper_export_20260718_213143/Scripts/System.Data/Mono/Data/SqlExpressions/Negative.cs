using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class Negative : UnaryExpression
	{
		public Negative(IExpression e)
			: base(e)
		{
		}

		public override object Eval(DataRow row)
		{
			return Numeric.Negative((IConvertible)expr.Eval(row));
		}
	}
}
