using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class Negation : UnaryExpression
	{
		public Negation(IExpression e)
			: base(e)
		{
		}

		public override object Eval(DataRow row)
		{
			object obj = expr.Eval(row);
			if (obj == DBNull.Value)
			{
				return obj;
			}
			return !(bool)obj;
		}

		public override bool EvalBoolean(DataRow row)
		{
			return !expr.EvalBoolean(row);
		}
	}
}
