namespace System.Xml.XPath
{
	internal class DescendantOrSelfIterator : SimpleIterator
	{
		private int _depth;

		private bool _finished;

		public DescendantOrSelfIterator(BaseIterator iter)
			: base(iter)
		{
		}

		private DescendantOrSelfIterator(DescendantOrSelfIterator other)
			: base(other, true)
		{
			_depth = other._depth;
		}

		public override XPathNodeIterator Clone()
		{
			return new DescendantOrSelfIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (_finished)
			{
				return false;
			}
			if (CurrentPosition == 0)
			{
				return true;
			}
			if (_nav.MoveToFirstChild())
			{
				_depth++;
				return true;
			}
			while (_depth != 0)
			{
				if (_nav.MoveToNext())
				{
					return true;
				}
				if (!_nav.MoveToParent())
				{
					throw new XPathException("Current node is removed while it should not be, or there are some bugs in the XPathNavigator implementation class: " + _nav.GetType());
				}
				_depth--;
			}
			_finished = true;
			return false;
		}
	}
}
