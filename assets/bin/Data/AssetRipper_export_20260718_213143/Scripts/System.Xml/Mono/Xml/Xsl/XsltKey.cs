using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class XsltKey : XPathFunction
	{
		private Expression arg0;

		private Expression arg1;

		private IStaticXsltContext staticContext;

		public Expression KeyName
		{
			get
			{
				return arg0;
			}
		}

		public Expression Field
		{
			get
			{
				return arg1;
			}
		}

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
				return arg0.Peer && arg1.Peer;
			}
		}

		public XsltKey(FunctionArguments args, IStaticXsltContext ctx)
			: base(args)
		{
			staticContext = ctx;
			if (args == null || args.Tail == null)
			{
				throw new XPathException("key takes 2 args");
			}
			arg0 = args.Arg;
			arg1 = args.Tail.Arg;
		}

		public bool PatternMatches(XPathNavigator nav, XsltContext nsmgr)
		{
			XsltCompiledContext xsltCompiledContext = nsmgr as XsltCompiledContext;
			return xsltCompiledContext.MatchesKey(nav, staticContext, arg0.StaticValueAsString, arg1.StaticValueAsString);
		}

		public override object Evaluate(BaseIterator iter)
		{
			XsltCompiledContext xsltCompiledContext = iter.NamespaceManager as XsltCompiledContext;
			return xsltCompiledContext.EvaluateKey(staticContext, iter, arg0, arg1);
		}
	}
}
