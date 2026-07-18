namespace System.Xml.XPath
{
	internal class ExprLiteral : Expression
	{
		protected string _value;

		public string Value
		{
			get
			{
				return _value;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.String;
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

		public override string StaticValueAsString
		{
			get
			{
				return _value;
			}
		}

		public ExprLiteral(string value)
		{
			_value = value;
		}

		public override string ToString()
		{
			return "'" + _value + "'";
		}

		public override object Evaluate(BaseIterator iter)
		{
			return _value;
		}

		public override string EvaluateString(BaseIterator iter)
		{
			return _value;
		}
	}
}
