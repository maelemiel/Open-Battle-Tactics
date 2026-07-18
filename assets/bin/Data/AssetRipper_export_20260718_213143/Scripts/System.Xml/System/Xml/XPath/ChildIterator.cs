namespace System.Xml.XPath
{
	internal class ChildIterator : BaseIterator
	{
		private XPathNavigator _nav;

		public override XPathNavigator Current
		{
			get
			{
				if (CurrentPosition == 0)
				{
					return null;
				}
				return _nav;
			}
		}

		public ChildIterator(BaseIterator iter)
			: base(iter.NamespaceManager)
		{
			_nav = ((iter.CurrentPosition != 0) ? iter.Current : iter.PeekNext());
			if (_nav != null && _nav.HasChildren)
			{
				_nav = _nav.Clone();
			}
			else
			{
				_nav = null;
			}
		}

		private ChildIterator(ChildIterator other)
			: base(other)
		{
			_nav = ((other._nav != null) ? other._nav.Clone() : null);
		}

		public override XPathNodeIterator Clone()
		{
			return new ChildIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (_nav == null)
			{
				return false;
			}
			return (CurrentPosition != 0) ? _nav.MoveToNext() : _nav.MoveToFirstChild();
		}
	}
}
