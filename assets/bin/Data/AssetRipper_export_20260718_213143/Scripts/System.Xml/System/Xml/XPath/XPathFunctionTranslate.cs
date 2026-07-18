using System.Text;

namespace System.Xml.XPath
{
	internal class XPathFunctionTranslate : XPathFunction
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
				return arg0.Peer && arg1.Peer && arg2.Peer;
			}
		}

		public XPathFunctionTranslate(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail == null || args.Tail.Tail == null || args.Tail.Tail.Tail != null)
			{
				throw new XPathException("translate takes 3 args");
			}
			arg0 = args.Arg;
			arg1 = args.Tail.Arg;
			arg2 = args.Tail.Tail.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			string text = arg0.EvaluateString(iter);
			string text2 = arg1.EvaluateString(iter);
			string text3 = arg2.EvaluateString(iter);
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			int i = 0;
			int length = text.Length;
			int length2 = text3.Length;
			for (; i < length; i++)
			{
				int num = text2.IndexOf(text[i]);
				if (num != -1)
				{
					if (num < length2)
					{
						stringBuilder.Append(text3[num]);
					}
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return "string-length(" + arg0.ToString() + "," + arg1.ToString() + "," + arg2.ToString() + ")";
		}
	}
}
