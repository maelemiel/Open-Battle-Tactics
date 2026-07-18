namespace System.Xml.XPath
{
	internal class PrecedingSiblingIterator : SimpleIterator
	{
		private bool finished;

		private bool started;

		private XPathNavigator startPosition;

		public override bool ReverseAxis
		{
			get
			{
				return true;
			}
		}

		public PrecedingSiblingIterator(BaseIterator iter)
			: base(iter)
		{
			startPosition = iter.Current.Clone();
		}

		private PrecedingSiblingIterator(PrecedingSiblingIterator other)
			: base(other, true)
		{
			startPosition = other.startPosition;
			started = other.started;
			finished = other.finished;
		}

		public override XPathNodeIterator Clone()
		{
			return new PrecedingSiblingIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (finished)
			{
				return false;
			}
			if (!started)
			{
				started = true;
				XPathNodeType nodeType = _nav.NodeType;
				if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
				{
					finished = true;
					return false;
				}
				_nav.MoveToFirst();
				if (!_nav.IsSamePosition(startPosition))
				{
					return true;
				}
			}
			else if (!_nav.MoveToNext())
			{
				finished = true;
				return false;
			}
			if (_nav.ComparePosition(startPosition) != XmlNodeOrder.Before)
			{
				finished = true;
				return false;
			}
			return true;
		}
	}
}
