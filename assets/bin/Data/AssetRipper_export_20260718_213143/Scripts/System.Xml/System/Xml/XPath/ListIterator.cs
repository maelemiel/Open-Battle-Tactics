using System.Collections;

namespace System.Xml.XPath
{
	internal class ListIterator : BaseIterator
	{
		private IList _list;

		public override XPathNavigator Current
		{
			get
			{
				if (_list.Count == 0 || CurrentPosition == 0)
				{
					return null;
				}
				return (XPathNavigator)_list[CurrentPosition - 1];
			}
		}

		public override int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public ListIterator(BaseIterator iter, IList list)
			: base(iter.NamespaceManager)
		{
			_list = list;
		}

		public ListIterator(IList list, IXmlNamespaceResolver nsm)
			: base(nsm)
		{
			_list = list;
		}

		private ListIterator(ListIterator other)
			: base(other)
		{
			_list = other._list;
		}

		public override XPathNodeIterator Clone()
		{
			return new ListIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (CurrentPosition >= _list.Count)
			{
				return false;
			}
			return true;
		}
	}
}
