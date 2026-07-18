using System.Collections;

namespace System.Xml.XPath
{
	internal class AncestorOrSelfIterator : SimpleIterator
	{
		private int currentPosition;

		private ArrayList navigators;

		private XPathNavigator startPosition;

		public override bool ReverseAxis
		{
			get
			{
				return true;
			}
		}

		public override int Count
		{
			get
			{
				if (navigators == null)
				{
					CollectResults();
				}
				return navigators.Count + 1;
			}
		}

		public AncestorOrSelfIterator(BaseIterator iter)
			: base(iter)
		{
			startPosition = iter.Current.Clone();
		}

		private AncestorOrSelfIterator(AncestorOrSelfIterator other)
			: base(other, true)
		{
			startPosition = other.startPosition;
			if (other.navigators != null)
			{
				navigators = (ArrayList)other.navigators.Clone();
			}
			currentPosition = other.currentPosition;
		}

		public override XPathNodeIterator Clone()
		{
			return new AncestorOrSelfIterator(this);
		}

		private void CollectResults()
		{
			navigators = new ArrayList();
			XPathNavigator xPathNavigator = startPosition.Clone();
			if (xPathNavigator.MoveToParent())
			{
				while (xPathNavigator.NodeType != XPathNodeType.Root)
				{
					navigators.Add(xPathNavigator.Clone());
					xPathNavigator.MoveToParent();
				}
				currentPosition = navigators.Count;
			}
		}

		public override bool MoveNextCore()
		{
			if (navigators == null)
			{
				CollectResults();
				if (startPosition.NodeType != XPathNodeType.Root)
				{
					_nav.MoveToRoot();
					return true;
				}
			}
			if (currentPosition == -1)
			{
				return false;
			}
			if (currentPosition-- == 0)
			{
				_nav.MoveTo(startPosition);
				return true;
			}
			_nav.MoveTo((XPathNavigator)navigators[currentPosition]);
			return true;
		}
	}
}
