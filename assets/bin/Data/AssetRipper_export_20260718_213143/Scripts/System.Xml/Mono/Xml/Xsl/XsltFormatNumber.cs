using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class XsltFormatNumber : XPathFunction
	{
		private Expression arg0;

		private Expression arg1;

		private Expression arg2;

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
				return arg0.Peer && arg1.Peer && (arg2 == null || arg2.Peer);
			}
		}

		public XsltFormatNumber(FunctionArguments args, IStaticXsltContext ctx)
			: base(args)
		{
			if (args == null || args.Tail == null || (args.Tail.Tail != null && args.Tail.Tail.Tail != null))
			{
				throw new XPathException("format-number takes 2 or 3 args");
			}
			arg0 = args.Arg;
			arg1 = args.Tail.Arg;
			if (args.Tail.Tail != null)
			{
				arg2 = args.Tail.Tail.Arg;
				this.ctx = ctx;
			}
		}

		public override object Evaluate(BaseIterator iter)
		{
			double number = arg0.EvaluateNumber(iter);
			string pattern = arg1.EvaluateString(iter);
			XmlQualifiedName name = XmlQualifiedName.Empty;
			if (arg2 != null)
			{
				name = XslNameUtil.FromString(arg2.EvaluateString(iter), ctx);
			}
			try
			{
				return ((XsltCompiledContext)iter.NamespaceManager).Processor.CompiledStyle.LookupDecimalFormat(name).FormatNumber(number, pattern);
			}
			catch (ArgumentException ex)
			{
				throw new XsltException(ex.Message, ex, iter.Current);
			}
		}
	}
}
