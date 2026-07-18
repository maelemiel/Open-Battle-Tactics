using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class TrimFunction : StringFunction
	{
		public TrimFunction(IExpression e)
			: base(e)
		{
		}

		public override object Eval(DataRow row)
		{
			string text = (string)base.Eval(row);
			if (text == null)
			{
				return null;
			}
			return text.Trim();
		}
	}
}
