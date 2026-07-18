namespace System.Xml.XPath
{
	internal class XPathFunctionNumber : XPathNumericFunction
	{
		private Expression arg0;

		public override bool HasStaticValue
		{
			get
			{
				return arg0 != null && arg0.HasStaticValue;
			}
		}

		public override double StaticValueAsNumber
		{
			get
			{
				return (arg0 == null) ? 0.0 : arg0.StaticValueAsNumber;
			}
		}

		internal override bool Peer
		{
			get
			{
				return arg0 == null || arg0.Peer;
			}
		}

		public XPathFunctionNumber(FunctionArguments args)
			: base(args)
		{
			if (args != null)
			{
				arg0 = args.Arg;
				if (args.Tail != null)
				{
					throw new XPathException("number takes 1 or zero args");
				}
			}
		}

		public override Expression Optimize()
		{
			if (arg0 == null)
			{
				return this;
			}
			arg0 = arg0.Optimize();
			return arg0.HasStaticValue ? ((Expression)new ExprNumber(StaticValueAsNumber)) : ((Expression)this);
		}

		public override object Evaluate(BaseIterator iter)
		{
			if (arg0 == null)
			{
				return XPathFunctions.ToNumber(iter.Current.Value);
			}
			return arg0.EvaluateNumber(iter);
		}

		public override string ToString()
		{
			return "number(" + arg0.ToString() + ")";
		}
	}
}
