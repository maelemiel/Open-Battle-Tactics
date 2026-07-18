using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.Xsl
{
	internal class XsltGenerateId : XPathFunction
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
				return arg0.Peer;
			}
		}

		public XsltGenerateId(FunctionArguments args)
			: base(args)
		{
			if (args != null)
			{
				if (args.Tail != null)
				{
					throw new XPathException("generate-id takes 1 or no args");
				}
				arg0 = args.Arg;
			}
		}

		public override object Evaluate(BaseIterator iter)
		{
			XPathNavigator xPathNavigator;
			if (arg0 != null)
			{
				XPathNodeIterator xPathNodeIterator = arg0.EvaluateNodeSet(iter);
				if (!xPathNodeIterator.MoveNext())
				{
					return string.Empty;
				}
				xPathNavigator = xPathNodeIterator.Current.Clone();
			}
			else
			{
				xPathNavigator = iter.Current.Clone();
			}
			StringBuilder stringBuilder = new StringBuilder("Mono");
			stringBuilder.Append(XmlConvert.EncodeLocalName(xPathNavigator.BaseURI));
			stringBuilder.Replace('_', 'm');
			stringBuilder.Append(xPathNavigator.NodeType);
			stringBuilder.Append('m');
			do
			{
				stringBuilder.Append(IndexInParent(xPathNavigator));
				stringBuilder.Append('m');
			}
			while (xPathNavigator.MoveToParent());
			return stringBuilder.ToString();
		}

		private int IndexInParent(XPathNavigator nav)
		{
			int num = 0;
			while (nav.MoveToPrevious())
			{
				num++;
			}
			return num;
		}
	}
}
