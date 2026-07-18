using System.Collections;

namespace System.Xml.XPath
{
	internal class XPathIteratorComparer : IComparer
	{
		public static XPathIteratorComparer Instance = new XPathIteratorComparer();

		private XPathIteratorComparer()
		{
		}

		public int Compare(object o1, object o2)
		{
			XPathNodeIterator xPathNodeIterator = o1 as XPathNodeIterator;
			XPathNodeIterator xPathNodeIterator2 = o2 as XPathNodeIterator;
			if (xPathNodeIterator == null)
			{
				return -1;
			}
			if (xPathNodeIterator2 == null)
			{
				return 1;
			}
			switch (xPathNodeIterator.Current.ComparePosition(xPathNodeIterator2.Current))
			{
			case XmlNodeOrder.Same:
				return 0;
			case XmlNodeOrder.After:
				return -1;
			default:
				return 1;
			}
		}
	}
}
