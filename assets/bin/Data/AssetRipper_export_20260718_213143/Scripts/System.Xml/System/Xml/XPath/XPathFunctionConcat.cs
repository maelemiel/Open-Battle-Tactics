using System.Collections;
using System.Globalization;
using System.Text;

namespace System.Xml.XPath
{
	internal class XPathFunctionConcat : XPathFunction
	{
		private ArrayList rgs;

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
				for (int i = 0; i < rgs.Count; i++)
				{
					if (!((Expression)rgs[i]).Peer)
					{
						return false;
					}
				}
				return true;
			}
		}

		public XPathFunctionConcat(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail == null)
			{
				throw new XPathException("concat takes 2 or more args");
			}
			args.ToArrayList(rgs = new ArrayList());
		}

		public override object Evaluate(BaseIterator iter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = rgs.Count;
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(((Expression)rgs[i]).EvaluateString(iter));
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("concat(");
			for (int i = 0; i < rgs.Count - 1; i++)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", rgs[i].ToString());
				stringBuilder.Append(',');
			}
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", rgs[rgs.Count - 1].ToString());
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}
	}
}
