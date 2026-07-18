using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class Literal : BaseExpression
	{
		private object val;

		public Literal(object val)
		{
			this.val = val;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is Literal))
			{
				return false;
			}
			Literal literal = (Literal)obj;
			if (literal.val != null)
			{
				if (!literal.val.Equals(val))
				{
					return false;
				}
			}
			else if (val != null)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return val.GetHashCode() ^ base.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			return val;
		}

		public override bool DependsOn(DataColumn other)
		{
			return false;
		}
	}
}
