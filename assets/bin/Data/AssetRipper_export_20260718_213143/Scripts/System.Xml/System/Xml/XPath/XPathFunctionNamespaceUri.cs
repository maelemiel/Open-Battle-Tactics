namespace System.Xml.XPath
{
	internal class XPathFunctionNamespaceUri : XPathFunction
	{
		private Expression arg0;

		internal override bool Peer
		{
			get
			{
				return arg0 == null || arg0.Peer;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.String;
			}
		}

		public XPathFunctionNamespaceUri(FunctionArguments args)
			: base(args)
		{
			if (args != null)
			{
				arg0 = args.Arg;
				if (args.Tail != null)
				{
					throw new XPathException("namespace-uri takes 1 or zero args");
				}
			}
		}

		public override object Evaluate(BaseIterator iter)
		{
			if (arg0 == null)
			{
				return iter.Current.NamespaceURI;
			}
			BaseIterator baseIterator = arg0.EvaluateNodeSet(iter);
			if (baseIterator == null || !baseIterator.MoveNext())
			{
				return string.Empty;
			}
			return baseIterator.Current.NamespaceURI;
		}

		public override string ToString()
		{
			return "namespace-uri(" + arg0.ToString() + ")";
		}
	}
}
