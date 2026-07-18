namespace System.Xml.XPath
{
	internal abstract class XPathNumericFunction : XPathFunction
	{
		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.Number;
			}
		}

		public override object StaticValue
		{
			get
			{
				return StaticValueAsNumber;
			}
		}

		internal XPathNumericFunction(FunctionArguments args)
			: base(args)
		{
		}
	}
}
