namespace System.Xml.XPath
{
	internal abstract class ExprBinary : Expression
	{
		protected Expression _left;

		protected Expression _right;

		public override bool HasStaticValue
		{
			get
			{
				return _left.HasStaticValue && _right.HasStaticValue;
			}
		}

		protected abstract string Operator { get; }

		internal override XPathNodeType EvaluatedNodeType
		{
			get
			{
				if (_left.EvaluatedNodeType == _right.EvaluatedNodeType)
				{
					return _left.EvaluatedNodeType;
				}
				return XPathNodeType.All;
			}
		}

		internal override bool IsPositional
		{
			get
			{
				return _left.IsPositional || _right.IsPositional;
			}
		}

		internal override bool Peer
		{
			get
			{
				return _left.Peer && _right.Peer;
			}
		}

		public ExprBinary(Expression left, Expression right)
		{
			_left = left;
			_right = right;
		}

		public override Expression Optimize()
		{
			_left = _left.Optimize();
			_right = _right.Optimize();
			return this;
		}

		public override string ToString()
		{
			return _left.ToString() + ' ' + Operator + ' ' + _right.ToString();
		}
	}
}
