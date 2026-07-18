namespace System.Xml.XPath
{
	internal class XPathFunctionLocalName : XPathFunction
	{
		private Expression arg0;

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
				return arg0 == null || arg0.Peer;
			}
		}

		public XPathFunctionLocalName(FunctionArguments args)
			: base(args)
		{
			if (args != null)
			{
				arg0 = args.Arg;
				if (args.Tail != null)
				{
					throw new XPathException("local-name takes 1 or zero args");
				}
			}
		}

		public override object Evaluate(BaseIterator iter)
		{
			if (arg0 == null)
			{
				return iter.Current.LocalName;
			}
			BaseIterator baseIterator = arg0.EvaluateNodeSet(iter);
			if (baseIterator == null || !baseIterator.MoveNext())
			{
				return string.Empty;
			}
			return baseIterator.Current.LocalName;
		}

		public override string ToString()
		{
			return "local-name(" + arg0.ToString() + ")";
		}
	}
}
