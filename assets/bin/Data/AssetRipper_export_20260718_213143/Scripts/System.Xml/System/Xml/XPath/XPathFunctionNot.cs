namespace System.Xml.XPath
{
	internal class XPathFunctionNot : XPathBooleanFunction
	{
		private Expression arg0;

		internal override bool Peer
		{
			get
			{
				return arg0.Peer;
			}
		}

		public XPathFunctionNot(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("not takes one arg");
			}
			arg0 = args.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			return !arg0.EvaluateBoolean(iter);
		}

		public override string ToString()
		{
			return "not(" + arg0.ToString() + ")";
		}
	}
}
