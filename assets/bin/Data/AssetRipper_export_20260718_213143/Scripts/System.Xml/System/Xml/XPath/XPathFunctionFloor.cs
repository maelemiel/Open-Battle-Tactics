namespace System.Xml.XPath
{
	internal class XPathFunctionFloor : XPathNumericFunction
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
				return (!HasStaticValue) ? 0.0 : Math.Floor(arg0.StaticValueAsNumber);
			}
		}

		internal override bool Peer
		{
			get
			{
				return arg0.Peer;
			}
		}

		public XPathFunctionFloor(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("floor takes one arg");
			}
			arg0 = args.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			return Math.Floor(arg0.EvaluateNumber(iter));
		}

		public override string ToString()
		{
			return "floor(" + arg0.ToString() + ")";
		}
	}
}
