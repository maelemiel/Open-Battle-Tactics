namespace System.Xml.XPath
{
	internal class XPathFunctionSum : XPathNumericFunction
	{
		private Expression arg0;

		internal override bool Peer
		{
			get
			{
				return arg0.Peer;
			}
		}

		public XPathFunctionSum(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("sum takes one arg");
			}
			arg0 = args.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			XPathNodeIterator xPathNodeIterator = arg0.EvaluateNodeSet(iter);
			double num = 0.0;
			while (xPathNodeIterator.MoveNext())
			{
				num += XPathFunctions.ToNumber(xPathNodeIterator.Current.Value);
			}
			return num;
		}

		public override string ToString()
		{
			return "sum(" + arg0.ToString() + ")";
		}
	}
}
