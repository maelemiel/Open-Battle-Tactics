namespace System.Xml.XPath
{
	internal class ExprNEG : Expression
	{
		private Expression _expr;

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.Number;
			}
		}

		internal override bool Peer
		{
			get
			{
				return _expr.Peer;
			}
		}

		public override bool HasStaticValue
		{
			get
			{
				return _expr.HasStaticValue;
			}
		}

		public override double StaticValueAsNumber
		{
			get
			{
				return (!_expr.HasStaticValue) ? 0.0 : (-1.0 * _expr.StaticValueAsNumber);
			}
		}

		internal override bool IsPositional
		{
			get
			{
				return _expr.IsPositional;
			}
		}

		public ExprNEG(Expression expr)
		{
			_expr = expr;
		}

		public override string ToString()
		{
			return "- " + _expr.ToString();
		}

		public override Expression Optimize()
		{
			_expr = _expr.Optimize();
			return HasStaticValue ? ((Expression)new ExprNumber(StaticValueAsNumber)) : ((Expression)this);
		}

		public override object Evaluate(BaseIterator iter)
		{
			return 0.0 - _expr.EvaluateNumber(iter);
		}

		public override double EvaluateNumber(BaseIterator iter)
		{
			return 0.0 - _expr.EvaluateNumber(iter);
		}
	}
}
