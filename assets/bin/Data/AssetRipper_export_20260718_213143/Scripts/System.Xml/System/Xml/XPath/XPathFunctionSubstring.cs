namespace System.Xml.XPath
{
	internal class XPathFunctionSubstring : XPathFunction
	{
		private Expression arg0;

		private Expression arg1;

		private Expression arg2;

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
				return arg0.Peer && arg1.Peer && (arg2 == null || arg2.Peer);
			}
		}

		public XPathFunctionSubstring(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail == null || (args.Tail.Tail != null && args.Tail.Tail.Tail != null))
			{
				throw new XPathException("substring takes 2 or 3 args");
			}
			arg0 = args.Arg;
			arg1 = args.Tail.Arg;
			if (args.Tail.Tail != null)
			{
				arg2 = args.Tail.Tail.Arg;
			}
		}

		public override object Evaluate(BaseIterator iter)
		{
			string text = arg0.EvaluateString(iter);
			double num = Math.Round(arg1.EvaluateNumber(iter)) - 1.0;
			if (double.IsNaN(num) || double.IsNegativeInfinity(num) || num >= (double)text.Length)
			{
				return string.Empty;
			}
			if (arg2 == null)
			{
				if (num < 0.0)
				{
					num = 0.0;
				}
				return text.Substring((int)num);
			}
			double num2 = Math.Round(arg2.EvaluateNumber(iter));
			if (double.IsNaN(num2))
			{
				return string.Empty;
			}
			if (num < 0.0 || num2 < 0.0)
			{
				num2 = num + num2;
				if (num2 <= 0.0)
				{
					return string.Empty;
				}
				num = 0.0;
			}
			double num3 = (double)text.Length - num;
			if (num2 > num3)
			{
				num2 = num3;
			}
			return text.Substring((int)num, (int)num2);
		}

		public override string ToString()
		{
			return "substring(" + arg0.ToString() + "," + arg1.ToString() + "," + arg2.ToString() + ")";
		}
	}
}
