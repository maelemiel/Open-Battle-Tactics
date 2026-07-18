namespace System.Xml.XPath
{
	internal class ExprGT : RelationalExpr
	{
		protected override string Operator
		{
			get
			{
				return ">";
			}
		}

		public ExprGT(Expression left, Expression right)
			: base(left, right)
		{
		}

		public override bool Compare(double arg1, double arg2)
		{
			return arg1 > arg2;
		}
	}
}
