namespace System.Xml.XPath
{
	internal class XPathFunctionCount : XPathFunction
	{
		private Expression arg0;

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
				return arg0.Peer;
			}
		}

		public XPathFunctionCount(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("count takes 1 arg");
			}
			arg0 = args.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			return (double)arg0.EvaluateNodeSet(iter).Count;
		}

		public override bool EvaluateBoolean(BaseIterator iter)
		{
			if (arg0.GetReturnType(iter) == XPathResultType.NodeSet)
			{
				return arg0.EvaluateBoolean(iter);
			}
			return arg0.EvaluateNodeSet(iter).MoveNext();
		}

		public override string ToString()
		{
			return "count(" + arg0.ToString() + ")";
		}
	}
}
