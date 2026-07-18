namespace System.Xml.XPath
{
	internal class FollowingIterator : SimpleIterator
	{
		private bool _finished;

		public FollowingIterator(BaseIterator iter)
			: base(iter)
		{
		}

		private FollowingIterator(FollowingIterator other)
			: base(other, true)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new FollowingIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (_finished)
			{
				return false;
			}
			bool flag = true;
			if (CurrentPosition == 0)
			{
				flag = false;
				XPathNodeType nodeType = _nav.NodeType;
				if (nodeType == XPathNodeType.Attribute || nodeType == XPathNodeType.Namespace)
				{
					_nav.MoveToParent();
					flag = true;
				}
				else
				{
					if (_nav.MoveToNext())
					{
						return true;
					}
					while (_nav.MoveToParent())
					{
						if (_nav.MoveToNext())
						{
							return true;
						}
					}
				}
			}
			if (flag)
			{
				if (_nav.MoveToFirstChild())
				{
					return true;
				}
				do
				{
					if (_nav.MoveToNext())
					{
						return true;
					}
				}
				while (_nav.MoveToParent());
			}
			_finished = true;
			return false;
		}
	}
}
