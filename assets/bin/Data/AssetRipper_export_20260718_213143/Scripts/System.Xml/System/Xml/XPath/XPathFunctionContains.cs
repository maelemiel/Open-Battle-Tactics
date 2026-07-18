namespace System.Xml.XPath
{
	internal class XPathFunctionContains : XPathFunction
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

		public XPathFunctionContains(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail == null || args.Tail.Tail != null)
			{
				throw new XPathException("contains takes 2 args");
			}
			arg0 = args.Arg;
			arg1 = args.Tail.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			return arg0.EvaluateString(iter).IndexOf(arg1.EvaluateString(iter)) != -1;
		}

		public override string ToString()
		{
			return "contains(" + arg0.ToString() + "," + arg1.ToString() + ")";
		}
	}
}
