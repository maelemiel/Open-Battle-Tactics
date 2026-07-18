using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class DTMXPathDocument2 : IXPathNavigable
	{
		private readonly XPathNavigator root;

		internal readonly XmlNameTable NameTable;

		internal readonly DTMXPathLinkedNode2[] Nodes;

		internal readonly DTMXPathAttributeNode2[] Attributes;

		internal readonly DTMXPathNamespaceNode2[] Namespaces;

		internal readonly string[] AtomicStringPool;

		internal readonly string[] NonAtomicStringPool;

		internal readonly Hashtable IdTable;

		public DTMXPathDocument2(XmlNameTable nameTable, DTMXPathLinkedNode2[] nodes, DTMXPathAttributeNode2[] attributes, DTMXPathNamespaceNode2[] namespaces, string[] atomicStringPool, string[] nonAtomicStringPool, Hashtable idTable)
		{
			Nodes = nodes;
			Attributes = attributes;
			Namespaces = namespaces;
			AtomicStringPool = atomicStringPool;
			NonAtomicStringPool = nonAtomicStringPool;
			IdTable = idTable;
			NameTable = nameTable;
			root = new DTMXPathNavigator2(this);
		}

		public XPathNavigator CreateNavigator()
		{
			return root.Clone();
		}
	}
}
