using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class XsltFunctionAvailable : XPathFunction
	{
		private Expression arg0;

		private IStaticXsltContext ctx;

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

		public XsltFunctionAvailable(FunctionArguments args, IStaticXsltContext ctx)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("element-available takes 1 arg");
			}
			arg0 = args.Arg;
			this.ctx = ctx;
		}

		public override object Evaluate(BaseIterator iter)
		{
			string text = arg0.EvaluateString(iter);
			int num = text.IndexOf(':');
			if (num > 0)
			{
				return (iter.NamespaceManager as XsltCompiledContext).ResolveFunction(XslNameUtil.FromString(text, ctx), null) != null;
			}
			int num2;
			switch (text)
			{
			default:
				num2 = ((text == "system-property") ? 1 : 0);
				break;
			case "boolean":
			case "ceiling":
			case "concat":
			case "contains":
			case "count":
			case "false":
			case "floor":
			case "id":
			case "lang":
			case "last":
			case "local-name":
			case "name":
			case "namespace-uri":
			case "normalize-space":
			case "not":
			case "number":
			case "position":
			case "round":
			case "starts-with":
			case "string":
			case "string-length":
			case "substring":
			case "substring-after":
			case "substring-before":
			case "sum":
			case "translate":
			case "true":
			case "document":
			case "format-number":
			case "function-available":
			case "generate-id":
			case "key":
			case "current":
			case "unparsed-entity-uri":
			case "element-available":
				num2 = 1;
				break;
			}
			return (byte)num2 != 0;
		}
	}
}
