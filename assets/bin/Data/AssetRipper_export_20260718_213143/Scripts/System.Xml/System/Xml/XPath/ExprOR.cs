namespace System.Xml.XPath
{
	internal class ExprOR : ExprBoolean
	{
		protected override string Operator
		{
			get
			{
				return "or";
			}
		}

		public override bool StaticValueAsBoolean
		{
			get
			{
				return HasStaticValue && (_left.StaticValueAsBoolean || _right.StaticValueAsBoolean);
			}
		}

		public ExprOR(Expression left, Expression right)
			: base(left, right)
		{
		}

		public override bool EvaluateBoolean(BaseIterator iter)
		{
			if (_left.EvaluateBoolean(iter))
			{
				return true;
			}
			return _right.EvaluateBoolean(iter);
		}
	}
}
