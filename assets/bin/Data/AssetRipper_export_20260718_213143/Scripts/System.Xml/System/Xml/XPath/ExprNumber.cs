namespace System.Xml.XPath
{
	internal class ExprNumber : Expression
	{
		protected double _value;

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

		public override double StaticValueAsNumber
		{
			get
			{
				return XPathFunctions.ToNumber(_value);
			}
		}

		internal override bool IsPositional
		{
			get
			{
				return false;
			}
		}

		public ExprNumber(double value)
		{
			_value = value;
		}

		public override string ToString()
		{
			return _value.ToString();
		}

		public override object Evaluate(BaseIterator iter)
		{
			return _value;
		}

		public override double EvaluateNumber(BaseIterator iter)
		{
			return _value;
		}
	}
}
