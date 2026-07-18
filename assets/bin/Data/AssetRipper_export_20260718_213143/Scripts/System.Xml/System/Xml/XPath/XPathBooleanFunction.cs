namespace System.Xml.XPath
{
	internal abstract class XPathBooleanFunction : XPathFunction
	{
		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.Boolean;
			}
		}

		public override object StaticValue
		{
			get
			{
				return StaticValueAsBoolean;
			}
		}

		public XPathBooleanFunction(FunctionArguments args)
			: base(args)
		{
		}
	}
}
