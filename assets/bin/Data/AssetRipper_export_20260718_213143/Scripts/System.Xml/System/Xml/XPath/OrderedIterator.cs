using System.Collections;

namespace System.Xml.XPath
{
	internal class OrderedIterator : BaseIterator
	{
		private BaseIterator iter;

		private ArrayList list;

		private int index = -1;

		public override XPathNavigator Current
		{
			get
			{
				return (iter != null) ? iter.Current : ((index >= 0) ? ((XPathNavigator)list[index]) : null);
			}
		}

		public OrderedIterator(BaseIterator iter)
			: base(iter.NamespaceManager)
		{
			list = new ArrayList();
			while (iter.MoveNext())
			{
				list.Add(iter.Current);
			}
			list.Sort(XPathNavigatorComparer.Instance);
		}

		private OrderedIterator(OrderedIterator other, bool dummy)
			: base(other)
		{
			if (other.iter != null)
			{
				iter = (BaseIterator)other.iter.Clone();
			}
			list = other.list;
			index = other.index;
		}

		public override XPathNodeIterator Clone()
		{
			return new OrderedIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (iter != null)
			{
				return iter.MoveNext();
			}
			if (index++ < list.Count)
			{
				return true;
			}
			index--;
			return false;
		}
	}
}
