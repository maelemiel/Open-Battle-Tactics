namespace System.Xml.XPath
{
	internal class XPathFunctionSubstringBefore : XPathFunction
	{
		private Expression arg0;

		private Expression arg1;

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
				return arg0.Peer && arg1.Peer;
			}
		}

		public XPathFunctionSubstringBefore(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail == null || args.Tail.Tail != null)
			{
				throw new XPathException("substring-before takes 2 args");
			}
			arg0 = args.Arg;
			arg1 = args.Tail.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			string text = arg0.EvaluateString(iter);
			string value = arg1.EvaluateString(iter);
			int num = text.IndexOf(value);
			if (num <= 0)
			{
				return string.Empty;
			}
			return text.Substring(0, num);
		}

		public override string ToString()
		{
			return "substring-before(" + arg0.ToString() + "," + arg1.ToString() + ")";
		}
	}
}
