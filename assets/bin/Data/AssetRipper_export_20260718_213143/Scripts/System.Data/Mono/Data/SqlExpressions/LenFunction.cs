using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class LenFunction : StringFunction
	{
		public LenFunction(IExpression e)
			: base(e)
		{
		}

		public override object Eval(DataRow row)
		{
			string text = (string)base.Eval(row);
			if (text == null)
			{
				return 0;
			}
			return text.Length;
		}
	}
}
