using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class XPathEditableDocument : IXPathNavigable
	{
		private XmlNode node;

		public XmlNode Node
		{
			get
			{
				return node;
			}
		}

		public XPathEditableDocument(XmlNode node)
		{
			this.node = node;
		}

		public XPathNavigator CreateNavigator()
		{
			return new XmlDocumentEditableNavigator(this);
		}
	}
}
