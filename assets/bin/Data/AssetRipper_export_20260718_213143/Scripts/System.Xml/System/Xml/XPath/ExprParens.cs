namespace System.Xml.XPath
{
	internal class ExprParens : Expression
	{
		protected Expression _expr;

		public override bool HasStaticValue
		{
			get
			{
				return _expr.HasStaticValue;
			}
		}

		public override object StaticValue
		{
			get
			{
				return _expr.StaticValue;
			}
		}

		public override string StaticValueAsString
		{
			get
			{
				return _expr.StaticValueAsString;
			}
		}

		public override double StaticValueAsNumber
		{
			get
			{
				return _expr.StaticValueAsNumber;
			}
		}

		public override bool StaticValueAsBoolean
		{
			get
			{
				return _expr.StaticValueAsBoolean;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return _expr.ReturnType;
			}
		}

		internal override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return _expr.EvaluatedNodeType;
			}
		}

		internal override bool IsPositional
		{
			get
			{
				return _expr.IsPositional;
			}
		}

		internal override bool Peer
		{
			get
			{
				return _expr.Peer;
			}
		}

		public ExprParens(Expression expr)
		{
			_expr = expr;
		}

		public override Expression Optimize()
		{
			_expr.Optimize();
			return this;
		}

		public override string ToString()
		{
			return "(" + _expr.ToString() + ")";
		}

		public override object Evaluate(BaseIterator iter)
		{
			object obj = _expr.Evaluate(iter);
			XPathNodeIterator xPathNodeIterator = obj as XPathNodeIterator;
			BaseIterator baseIterator = xPathNodeIterator as BaseIterator;
			if (baseIterator == null && xPathNodeIterator != null)
			{
				baseIterator = new WrapperIterator(xPathNodeIterator, iter.NamespaceManager);
			}
			if (baseIterator != null)
			{
				return new ParensIterator(baseIterator);
			}
			return obj;
		}
	}
}
