namespace System.Xml.XPath
{
	internal abstract class ExprBoolean : ExprBinary
	{
		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.Boolean;
			}
		}

		public ExprBoolean(Expression left, Expression right)
			: base(left, right)
		{
		}

		public override Expression Optimize()
		{
			base.Optimize();
			if (!HasStaticValue)
			{
				return this;
			}
			if (StaticValueAsBoolean)
			{
				return new XPathFunctionTrue(null);
			}
			return new XPathFunctionFalse(null);
		}

		public override object Evaluate(BaseIterator iter)
		{
			return EvaluateBoolean(iter);
		}

		public override double EvaluateNumber(BaseIterator iter)
		{
			return EvaluateBoolean(iter) ? 1 : 0;
		}

		public override string EvaluateString(BaseIterator iter)
		{
			return (!EvaluateBoolean(iter)) ? "false" : "true";
		}
	}
}
