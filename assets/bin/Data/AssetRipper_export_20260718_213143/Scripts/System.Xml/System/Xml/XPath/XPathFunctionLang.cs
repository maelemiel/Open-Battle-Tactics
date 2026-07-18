using System.Globalization;

namespace System.Xml.XPath
{
	internal class XPathFunctionLang : XPathFunction
	{
		private Expression arg0;

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.Boolean;
			}
		}

		internal override bool Peer
		{
			get
			{
				return arg0.Peer;
			}
		}

		public XPathFunctionLang(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("lang takes one arg");
			}
			arg0 = args.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			return EvaluateBoolean(iter);
		}

		public override bool EvaluateBoolean(BaseIterator iter)
		{
			string text = arg0.EvaluateString(iter).ToLower(CultureInfo.InvariantCulture);
			string text2 = iter.Current.XmlLang.ToLower(CultureInfo.InvariantCulture);
			return text == text2 || text == text2.Split('-')[0];
		}

		public override string ToString()
		{
			return "lang(" + arg0.ToString() + ")";
		}
	}
}
