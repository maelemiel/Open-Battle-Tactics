namespace System.Xml.XPath
{
	internal class ExprAND : ExprBoolean
	{
		protected override string Operator
		{
			get
			{
				return "and";
			}
		}

		public override bool StaticValueAsBoolean
		{
			get
			{
				return HasStaticValue && _left.StaticValueAsBoolean && _right.StaticValueAsBoolean;
			}
		}

		public ExprAND(Expression left, Expression right)
			: base(left, right)
		{
		}

		public override bool EvaluateBoolean(BaseIterator iter)
		{
			if (!_left.EvaluateBoolean(iter))
			{
				return false;
			}
			return _right.EvaluateBoolean(iter);
		}
	}
}
