using System.Collections;

namespace System.Xml.XPath
{
	internal class XPathSorters : IComparer
	{
		private readonly ArrayList _rgSorters = new ArrayList();

		int IComparer.Compare(object o1, object o2)
		{
			XPathSortElement xPathSortElement = (XPathSortElement)o1;
			XPathSortElement xPathSortElement2 = (XPathSortElement)o2;
			for (int i = 0; i < _rgSorters.Count; i++)
			{
				XPathSorter xPathSorter = (XPathSorter)_rgSorters[i];
				int num = xPathSorter.Compare(xPathSortElement.Values[i], xPathSortElement2.Values[i]);
				if (num != 0)
				{
					return num;
				}
			}
			switch (xPathSortElement.Navigator.ComparePosition(xPathSortElement2.Navigator))
			{
			case XmlNodeOrder.Same:
				return 0;
			case XmlNodeOrder.After:
				return 1;
			default:
				return -1;
			}
		}

		public void Add(object expr, IComparer cmp)
		{
			_rgSorters.Add(new XPathSorter(expr, cmp));
		}

		public void Add(object expr, XmlSortOrder orderSort, XmlCaseOrder orderCase, string lang, XmlDataType dataType)
		{
			_rgSorters.Add(new XPathSorter(expr, orderSort, orderCase, lang, dataType));
		}

		public void CopyFrom(XPathSorter[] sorters)
		{
			_rgSorters.Clear();
			_rgSorters.AddRange(sorters);
		}

		public BaseIterator Sort(BaseIterator iter)
		{
			ArrayList rgElts = ToSortElementList(iter);
			return Sort(rgElts, iter.NamespaceManager);
		}

		private ArrayList ToSortElementList(BaseIterator iter)
		{
			ArrayList arrayList = new ArrayList();
			int count = _rgSorters.Count;
			while (iter.MoveNext())
			{
				XPathSortElement xPathSortElement = new XPathSortElement();
				xPathSortElement.Navigator = iter.Current.Clone();
				xPathSortElement.Values = new object[count];
				for (int i = 0; i < _rgSorters.Count; i++)
				{
					XPathSorter xPathSorter = (XPathSorter)_rgSorters[i];
					xPathSortElement.Values[i] = xPathSorter.Evaluate(iter);
				}
				arrayList.Add(xPathSortElement);
			}
			return arrayList;
		}

		public BaseIterator Sort(ArrayList rgElts, IXmlNamespaceResolver nsm)
		{
			rgElts.Sort(this);
			XPathNavigator[] array = new XPathNavigator[rgElts.Count];
			for (int i = 0; i < rgElts.Count; i++)
			{
				XPathSortElement xPathSortElement = (XPathSortElement)rgElts[i];
				array[i] = xPathSortElement.Navigator;
			}
			return new ListIterator(array, nsm);
		}
	}
}
