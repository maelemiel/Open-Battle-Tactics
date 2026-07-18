using System.Collections;
using System.Xml.Xsl;
using Mono.Xml.XPath;

namespace System.Xml.XPath
{
	public abstract class XPathExpression
	{
		public abstract string Expression { get; }

		public abstract XPathResultType ReturnType { get; }

		internal XPathExpression()
		{
		}

		public abstract void AddSort(object expr, IComparer comparer);

		public abstract void AddSort(object expr, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType);

		public abstract XPathExpression Clone();

		public abstract void SetContext(XmlNamespaceManager nsManager);

		public static XPathExpression Compile(string xpath)
		{
			return Compile(xpath, null, null);
		}

		public static XPathExpression Compile(string xpath, IXmlNamespaceResolver nsmgr)
		{
			return Compile(xpath, nsmgr, null);
		}

		internal static XPathExpression Compile(string xpath, IXmlNamespaceResolver nsmgr, IStaticXsltContext ctx)
		{
			XPathParser xPathParser = new XPathParser(ctx);
			CompiledExpression compiledExpression = new CompiledExpression(xpath, xPathParser.Compile(xpath));
			compiledExpression.SetContext(nsmgr);
			return compiledExpression;
		}

		public abstract void SetContext(IXmlNamespaceResolver nsResolver);
	}
}
