using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal abstract class BinaryExpression : BaseExpression
	{
		protected IExpression expr1;

		protected IExpression expr2;

		protected BinaryExpression(IExpression e1, IExpression e2)
		{
			expr1 = e1;
			expr2 = e2;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is BinaryExpression))
			{
				return false;
			}
			BinaryExpression binaryExpression = (BinaryExpression)obj;
			if (!binaryExpression.expr1.Equals(expr1) || !binaryExpression.expr2.Equals(expr2))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode ^= expr1.GetHashCode();
			return hashCode ^ expr2.GetHashCode();
		}

		public override bool DependsOn(DataColumn other)
		{
			return expr1.DependsOn(other) || expr2.DependsOn(other);
		}

		public override void ResetExpression()
		{
			expr1.ResetExpression();
			expr2.ResetExpression();
		}
	}
}
