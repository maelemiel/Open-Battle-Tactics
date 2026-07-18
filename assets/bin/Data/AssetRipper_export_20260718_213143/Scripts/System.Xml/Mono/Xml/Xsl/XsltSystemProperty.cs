using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class XsltSystemProperty : XPathFunction
	{
		private Expression arg0;

		private IStaticXsltContext ctx;

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

		public XsltSystemProperty(FunctionArguments args, IStaticXsltContext ctx)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("system-property takes 1 arg");
			}
			arg0 = args.Arg;
			this.ctx = ctx;
		}

		public override object Evaluate(BaseIterator iter)
		{
			XmlQualifiedName xmlQualifiedName = XslNameUtil.FromString(arg0.EvaluateString(iter), ctx);
			if (xmlQualifiedName.Namespace == "http://www.w3.org/1999/XSL/Transform")
			{
				switch (xmlQualifiedName.Name)
				{
				case "version":
					return "1.0";
				case "vendor":
					return "Mono";
				case "vendor-url":
					return "http://www.go-mono.com/";
				}
			}
			return string.Empty;
		}
	}
}
