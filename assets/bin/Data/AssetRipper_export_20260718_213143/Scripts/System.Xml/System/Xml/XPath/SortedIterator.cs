using System.Collections;

namespace System.Xml.XPath
{
	internal class SortedIterator : BaseIterator
	{
		private ArrayList list;

		public override XPathNavigator Current
		{
			get
			{
				return (CurrentPosition != 0) ? ((XPathNavigator)list[CurrentPosition - 1]) : null;
			}
		}

		public override int Count
		{
			get
			{
				return list.Count;
			}
		}

		public SortedIterator(BaseIterator iter)
			: base(iter.NamespaceManager)
		{
			list = new ArrayList();
			while (iter.MoveNext())
			{
				list.Add(iter.Current.Clone());
			}
			if (list.Count == 0)
			{
				return;
			}
			XPathNavigator xPathNavigator = (XPathNavigator)list[0];
			list.Sort(XPathNavigatorComparer.Instance);
			for (int i = 1; i < list.Count; i++)
			{
				XPathNavigator xPathNavigator2 = (XPathNavigator)list[i];
				if (xPathNavigator.IsSamePosition(xPathNavigator2))
				{
					list.RemoveAt(i);
					i--;
				}
				else
				{
					xPathNavigator = xPathNavigator2;
				}
			}
		}

		public SortedIterator(SortedIterator other)
			: base(other)
		{
			list = other.list;
			SetPosition(other.CurrentPosition);
		}

		public override XPathNodeIterator Clone()
		{
			return new SortedIterator(this);
		}

		public override bool MoveNextCore()
		{
			return CurrentPosition < list.Count;
		}
	}
}
