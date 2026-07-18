namespace System.Xml.XPath
{
	internal abstract class SimpleIterator : BaseIterator
	{
		protected readonly XPathNavigator _nav;

		protected XPathNavigator _current;

		private bool skipfirst;

		public override XPathNavigator Current
		{
			get
			{
				if (CurrentPosition == 0)
				{
					return null;
				}
				_current = _nav;
				return _current;
			}
		}

		public SimpleIterator(BaseIterator iter)
			: base(iter.NamespaceManager)
		{
			if (iter.CurrentPosition == 0)
			{
				skipfirst = true;
				iter.MoveNext();
			}
			if (iter.CurrentPosition > 0)
			{
				_nav = iter.Current.Clone();
			}
		}

		protected SimpleIterator(SimpleIterator other, bool clone)
			: base(other)
		{
			if (other._nav != null)
			{
				_nav = ((!clone) ? other._nav : other._nav.Clone());
			}
			skipfirst = other.skipfirst;
		}

		public SimpleIterator(XPathNavigator nav, IXmlNamespaceResolver nsm)
			: base(nsm)
		{
			_nav = nav.Clone();
		}

		public override bool MoveNext()
		{
			if (skipfirst)
			{
				if (_nav == null)
				{
					return false;
				}
				skipfirst = false;
				SetPosition(1);
				return true;
			}
			return base.MoveNext();
		}
	}
}
