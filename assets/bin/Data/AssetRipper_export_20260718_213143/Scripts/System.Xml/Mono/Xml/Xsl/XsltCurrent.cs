using System.Xml.XPath;

namespace Mono.Xml.Xsl
{
	internal class XsltCurrent : XPathFunction
	{
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
				return false;
			}
		}

		public XsltCurrent(FunctionArguments args)
			: base(args)
		{
			if (args != null)
			{
				throw new XPathException("current takes 0 args");
			}
		}

		public override object Evaluate(BaseIterator iter)
		{
			XsltCompiledContext xsltCompiledContext = (XsltCompiledContext)iter.NamespaceManager;
			return new SelfIterator(xsltCompiledContext.Processor.CurrentNode, xsltCompiledContext);
		}

		public override string ToString()
		{
			return "current()";
		}
	}
}
