using System.Xml.XPath;

namespace System.Xml.Xsl
{
	public abstract class XsltContext : XmlNamespaceManager
	{
		public abstract bool Whitespace { get; }

		protected XsltContext()
			: base(new NameTable())
		{
		}

		protected XsltContext(NameTable table)
			: base(table)
		{
		}

		public abstract bool PreserveWhitespace(XPathNavigator nav);

		public abstract int CompareDocument(string baseUri, string nextbaseUri);

		public abstract IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes);

		public abstract IXsltContextVariable ResolveVariable(string prefix, string name);

		internal virtual IXsltContextVariable ResolveVariable(XmlQualifiedName name)
		{
			return ResolveVariable(LookupPrefix(name.Namespace), name.Name);
		}

		internal virtual IXsltContextFunction ResolveFunction(XmlQualifiedName name, XPathResultType[] argTypes)
		{
			return ResolveFunction(name.Name, name.Namespace, argTypes);
		}
	}
}
