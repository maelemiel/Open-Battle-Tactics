namespace System.Xml.XPath
{
	internal class ExprEQ : EqualityExpr
	{
		protected override string Operator
		{
			get
			{
				return "=";
			}
		}

		public ExprEQ(Expression left, Expression right)
			: base(left, right, true)
		{
		}
	}
}
