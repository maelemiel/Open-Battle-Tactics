namespace System.Xml.XPath
{
	internal class XPathFunctionName : XPathFunction
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

		public XPathFunctionName(FunctionArguments args)
			: base(args)
		{
			if (args != null)
			{
				arg0 = args.Arg;
				if (args.Tail != null)
				{
					throw new XPathException("name takes 1 or zero args");
				}
			}
		}

		public override object Evaluate(BaseIterator iter)
		{
			if (arg0 == null)
			{
				return iter.Current.Name;
			}
			BaseIterator baseIterator = arg0.EvaluateNodeSet(iter);
			if (baseIterator == null || !baseIterator.MoveNext())
			{
				return string.Empty;
			}
			return baseIterator.Current.Name;
		}

		public override string ToString()
		{
			return "name(" + ((arg0 == null) ? string.Empty : arg0.ToString()) + ")";
		}
	}
}
