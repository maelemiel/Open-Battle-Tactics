using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal abstract class StringFunction : UnaryExpression
	{
		protected StringFunction(IExpression e)
			: base(e)
		{
		}

		public override object Eval(DataRow row)
		{
			object obj = expr.Eval(row);
			if (obj == null)
			{
				return null;
			}
			if (!(obj is string))
			{
				string text = GetType().ToString();
				int num = text.LastIndexOf('.') + 1;
				text = text.Substring(num, text.Length - num - "Function".Length);
				throw new EvaluateException(string.Format("'{0}' can be applied only to strings.", text));
			}
			return obj;
		}
	}
}
