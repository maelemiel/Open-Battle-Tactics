namespace System.Xml.XPath
{
	internal class ParentIterator : SimpleIterator
	{
		private bool canMove;

		public ParentIterator(BaseIterator iter)
			: base(iter)
		{
			canMove = _nav.MoveToParent();
		}

		private ParentIterator(ParentIterator other, bool dummy)
			: base(other, true)
		{
			canMove = other.canMove;
		}

		public ParentIterator(XPathNavigator nav, IXmlNamespaceResolver nsm)
			: base(nav, nsm)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new ParentIterator(this, true);
		}

		public override bool MoveNextCore()
		{
			if (!canMove)
			{
				return false;
			}
			canMove = false;
			return true;
		}
	}
}
