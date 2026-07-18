using System.Collections;

namespace System.Xml.XPath
{
	internal class AncestorIterator : SimpleIterator
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
				return navigators.Count;
			}
		}

		public AncestorIterator(BaseIterator iter)
			: base(iter)
		{
			startPosition = iter.Current.Clone();
		}

		private AncestorIterator(AncestorIterator other)
			: base(other, true)
		{
			startPosition = other.startPosition;
			if (other.navigators != null)
			{
				navigators = other.navigators;
			}
			currentPosition = other.currentPosition;
		}

		public override XPathNodeIterator Clone()
		{
			return new AncestorIterator(this);
		}

		private void CollectResults()
		{
			navigators = new ArrayList();
			XPathNavigator xPathNavigator = startPosition.Clone();
			while (xPathNavigator.NodeType != XPathNodeType.Root && xPathNavigator.MoveToParent())
			{
				navigators.Add(xPathNavigator.Clone());
			}
			currentPosition = navigators.Count;
		}

		public override bool MoveNextCore()
		{
			if (navigators == null)
			{
				CollectResults();
			}
			if (currentPosition == 0)
			{
				return false;
			}
			_nav.MoveTo((XPathNavigator)navigators[--currentPosition]);
			return true;
		}
	}
}
