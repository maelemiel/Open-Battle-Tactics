namespace System.Xml.XPath
{
	internal abstract class ExprNumeric : ExprBinary
	{
		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.Number;
			}
		}

		public ExprNumeric(Expression left, Expression right)
			: base(left, right)
		{
		}

		public override Expression Optimize()
		{
			base.Optimize();
			return HasStaticValue ? ((Expression)new ExprNumber(StaticValueAsNumber)) : ((Expression)this);
		}

		public override object Evaluate(BaseIterator iter)
		{
			return EvaluateNumber(iter);
		}
	}
}
