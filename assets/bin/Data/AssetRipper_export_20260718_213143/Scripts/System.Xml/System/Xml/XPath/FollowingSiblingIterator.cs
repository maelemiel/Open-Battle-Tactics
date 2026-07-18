namespace System.Xml.XPath
{
	internal class FollowingSiblingIterator : SimpleIterator
	{
		public FollowingSiblingIterator(BaseIterator iter)
			: base(iter)
		{
		}

		private FollowingSiblingIterator(FollowingSiblingIterator other)
			: base(other, true)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new FollowingSiblingIterator(this);
		}

		public override bool MoveNextCore()
		{
			XPathNodeType nodeType = _nav.NodeType;
			if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
			{
				return false;
			}
			if (_nav.MoveToNext())
			{
				return true;
			}
			return false;
		}
	}
}
