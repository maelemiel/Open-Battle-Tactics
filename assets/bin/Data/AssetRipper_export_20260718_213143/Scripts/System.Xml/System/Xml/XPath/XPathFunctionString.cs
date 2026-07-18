namespace System.Xml.XPath
{
	internal class XPathFunctionString : XPathFunction
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

		public XPathFunctionString(FunctionArguments args)
			: base(args)
		{
			if (args != null)
			{
				arg0 = args.Arg;
				if (args.Tail != null)
				{
					throw new XPathException("string takes 1 or zero args");
				}
			}
		}

		public override object Evaluate(BaseIterator iter)
		{
			if (arg0 == null)
			{
				return iter.Current.Value;
			}
			return arg0.EvaluateString(iter);
		}

		public override string ToString()
		{
			return "string(" + arg0.ToString() + ")";
		}
	}
}
