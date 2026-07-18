namespace System.Xml.XPath
{
	internal class BooleanConstant : Expression
	{
		private bool _value;

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.Boolean;
			}
		}

		internal override bool Peer
		{
			get
			{
				return true;
			}
		}

		public override bool HasStaticValue
		{
			get
			{
				return true;
			}
		}

		public override bool StaticValueAsBoolean
		{
			get
			{
				return _value;
			}
		}

		public BooleanConstant(bool value)
		{
			_value = value;
		}

		public override string ToString()
		{
			return (!_value) ? "false()" : "true()";
		}

		public override object Evaluate(BaseIterator iter)
		{
			return _value;
		}

		public override bool EvaluateBoolean(BaseIterator iter)
		{
			return _value;
		}
	}
}
