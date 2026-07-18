using System.Text;

namespace System.Xml.XPath
{
	internal class XPathFunctionNormalizeSpace : XPathFunction
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

		public XPathFunctionNormalizeSpace(FunctionArguments args)
			: base(args)
		{
			if (args != null)
			{
				arg0 = args.Arg;
				if (args.Tail != null)
				{
					throw new XPathException("normalize-space takes 1 or zero args");
				}
			}
		}

		public override object Evaluate(BaseIterator iter)
		{
			string text = ((arg0 == null) ? iter.Current.Value : arg0.EvaluateString(iter));
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in text)
			{
				if (c == ' ' || c == '\t' || c == '\r' || c == '\n')
				{
					flag = true;
					continue;
				}
				if (flag)
				{
					flag = false;
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(' ');
					}
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return "normalize-space(" + ((arg0 == null) ? string.Empty : arg0.ToString()) + ")";
		}
	}
}
