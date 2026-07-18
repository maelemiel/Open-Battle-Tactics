namespace System.Xml.XPath
{
	internal class SelfIterator : SimpleIterator
	{
		public override XPathNavigator Current
		{
			get
			{
				return (CurrentPosition != 0) ? _nav : null;
			}
		}

		public SelfIterator(BaseIterator iter)
			: base(iter)
		{
		}

		public SelfIterator(XPathNavigator nav, IXmlNamespaceResolver nsm)
			: base(nav, nsm)
		{
		}

		protected SelfIterator(SelfIterator other, bool clone)
			: base(other, true)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new SelfIterator(this, true);
		}

		public override bool MoveNextCore()
		{
			if (CurrentPosition == 0)
			{
				return true;
			}
			return false;
		}
	}
}
