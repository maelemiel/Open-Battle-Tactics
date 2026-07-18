using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class SubstringFunction : StringFunction
	{
		private IExpression start;

		private IExpression len;

		public SubstringFunction(IExpression e, IExpression start, IExpression len)
			: base(e)
		{
			this.start = start;
			this.len = len;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is SubstringFunction))
			{
				return false;
			}
			SubstringFunction substringFunction = (SubstringFunction)obj;
			if (substringFunction.start != start)
			{
				return false;
			}
			if (substringFunction.len != len)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode ^= start.GetHashCode();
			return hashCode ^ len.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			string text = (string)base.Eval(row);
			object obj = start.Eval(row);
			int num = Convert.ToInt32(start.Eval(row));
			int val = Convert.ToInt32(len.Eval(row));
			if (text == null)
			{
				return null;
			}
			if (num > text.Length)
			{
				return string.Empty;
			}
			return text.Substring(num - 1, System.Math.Min(val, text.Length - (num - 1)));
		}
	}
}
