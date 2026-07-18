using System.Collections;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class MSXslNodeSet : XPathFunction
	{
		private Expression arg0;

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}

		internal override bool Peer
		{
			get
			{
				return arg0.Peer;
			}
		}

		public MSXslNodeSet(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("element-available takes 1 arg");
			}
			arg0 = args.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			XsltCompiledContext nsm = iter.NamespaceManager as XsltCompiledContext;
			XPathNavigator xPathNavigator = ((iter.Current == null) ? null : iter.Current.Clone());
			XPathNavigator xPathNavigator2 = arg0.EvaluateAs(iter, XPathResultType.Navigator) as XPathNavigator;
			if (xPathNavigator2 == null)
			{
				if (xPathNavigator != null)
				{
					return new XsltException("Cannot convert the XPath argument to a result tree fragment.", null, xPathNavigator);
				}
				return new XsltException("Cannot convert the XPath argument to a result tree fragment.", null);
			}
			ArrayList arrayList = new ArrayList();
			arrayList.Add(xPathNavigator2);
			return new ListIterator(arrayList, nsm);
		}
	}
}
