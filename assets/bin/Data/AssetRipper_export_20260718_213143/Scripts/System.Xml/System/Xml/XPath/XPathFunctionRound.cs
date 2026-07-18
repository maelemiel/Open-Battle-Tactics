namespace System.Xml.XPath
{
	internal class XPathFunctionRound : XPathNumericFunction
	{
		private Expression arg0;

		public override bool HasStaticValue
		{
			get
			{
				return arg0.HasStaticValue;
			}
		}

		public override double StaticValueAsNumber
		{
			get
			{
				return (!HasStaticValue) ? 0.0 : Round(arg0.StaticValueAsNumber);
			}
		}

		internal override bool Peer
		{
			get
			{
				return arg0.Peer;
			}
		}

		public XPathFunctionRound(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("round takes one arg");
			}
			arg0 = args.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			return Round(arg0.EvaluateNumber(iter));
		}

		private double Round(double arg)
		{
			if (arg < -0.5 || arg > 0.0)
			{
				return Math.Floor(arg + 0.5);
			}
			return Math.Round(arg);
		}

		public override string ToString()
		{
			return "round(" + arg0.ToString() + ")";
		}
	}
}
