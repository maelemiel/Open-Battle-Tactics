namespace System.Xml.XPath
{
	internal class ExprNE : EqualityExpr
	{
		protected override string Operator
		{
			get
			{
				return "!=";
			}
		}

		public ExprNE(Expression left, Expression right)
			: base(left, right, false)
		{
		}
	}
}
