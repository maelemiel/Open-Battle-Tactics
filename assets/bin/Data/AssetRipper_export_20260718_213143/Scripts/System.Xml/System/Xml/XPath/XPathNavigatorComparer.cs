using System.Collections;

namespace System.Xml.XPath
{
	internal class XPathNavigatorComparer : IComparer, IEqualityComparer
	{
		public static XPathNavigatorComparer Instance = new XPathNavigatorComparer();

		private XPathNavigatorComparer()
		{
		}

		bool IEqualityComparer.Equals(object o1, object o2)
		{
			XPathNavigator xPathNavigator = o1 as XPathNavigator;
			XPathNavigator xPathNavigator2 = o2 as XPathNavigator;
			return xPathNavigator != null && xPathNavigator2 != null && xPathNavigator.IsSamePosition(xPathNavigator2);
		}

		int IEqualityComparer.GetHashCode(object obj)
		{
			return obj.GetHashCode();
		}

		public int Compare(object o1, object o2)
		{
			XPathNavigator xPathNavigator = o1 as XPathNavigator;
			XPathNavigator xPathNavigator2 = o2 as XPathNavigator;
			if (xPathNavigator == null)
			{
				return -1;
			}
			if (xPathNavigator2 == null)
			{
				return 1;
			}
			switch (xPathNavigator.ComparePosition(xPathNavigator2))
			{
			case XmlNodeOrder.Same:
				return 0;
			case XmlNodeOrder.After:
				return 1;
			default:
				return -1;
			}
		}
	}
}
