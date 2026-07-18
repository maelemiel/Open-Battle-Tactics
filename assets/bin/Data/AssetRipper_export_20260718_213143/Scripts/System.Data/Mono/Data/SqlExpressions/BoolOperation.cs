using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class BoolOperation : BinaryOpExpression
	{
		public BoolOperation(Operation op, IExpression e1, IExpression e2)
			: base(op, e1, e2)
		{
		}

		public override object Eval(DataRow row)
		{
			return EvalBoolean(row);
		}

		public override bool EvalBoolean(DataRow row)
		{
			if (op == Operation.OR)
			{
				return expr1.EvalBoolean(row) || expr2.EvalBoolean(row);
			}
			if (op == Operation.AND)
			{
				return expr1.EvalBoolean(row) && expr2.EvalBoolean(row);
			}
			return false;
		}
	}
}
