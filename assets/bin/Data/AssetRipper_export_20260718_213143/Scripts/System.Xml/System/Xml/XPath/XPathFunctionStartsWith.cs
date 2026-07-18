namespace System.Xml.XPath
{
	internal class XPathFunctionStartsWith : XPathFunction
	{
		private Expression arg0;

		private Expression arg1;

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
				return arg0.Peer && arg1.Peer;
			}
		}

		public XPathFunctionStartsWith(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail == null || args.Tail.Tail != null)
			{
				throw new XPathException("starts-with takes 2 args");
			}
			arg0 = args.Arg;
			arg1 = args.Tail.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			return arg0.EvaluateString(iter).StartsWith(arg1.EvaluateString(iter));
		}

		public override string ToString()
		{
			return "starts-with(" + arg0.ToString() + "," + arg1.ToString() + ")";
		}
	}
}
