namespace System.Xml.XPath
{
	internal class UnionIterator : BaseIterator
	{
		private BaseIterator _left;

		private BaseIterator _right;

		private bool keepLeft;

		private bool keepRight;

		private XPathNavigator _current;

		public override XPathNavigator Current
		{
			get
			{
				return (CurrentPosition <= 0) ? null : _current;
			}
		}

		public UnionIterator(BaseIterator iter, BaseIterator left, BaseIterator right)
			: base(iter.NamespaceManager)
		{
			_left = left;
			_right = right;
		}

		private UnionIterator(UnionIterator other)
			: base(other)
		{
			_left = (BaseIterator)other._left.Clone();
			_right = (BaseIterator)other._right.Clone();
			keepLeft = other.keepLeft;
			keepRight = other.keepRight;
			if (other._current != null)
			{
				_current = other._current.Clone();
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new UnionIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (!keepLeft)
			{
				keepLeft = _left.MoveNext();
			}
			if (!keepRight)
			{
				keepRight = _right.MoveNext();
			}
			if (!keepLeft && !keepRight)
			{
				return false;
			}
			if (!keepRight)
			{
				keepLeft = false;
				SetCurrent(_left);
				return true;
			}
			if (!keepLeft)
			{
				keepRight = false;
				SetCurrent(_right);
				return true;
			}
			switch (_left.Current.ComparePosition(_right.Current))
			{
			case XmlNodeOrder.Same:
				keepLeft = (keepRight = false);
				SetCurrent(_right);
				return true;
			case XmlNodeOrder.Before:
			case XmlNodeOrder.Unknown:
				keepLeft = false;
				SetCurrent(_left);
				return true;
			case XmlNodeOrder.After:
				keepRight = false;
				SetCurrent(_right);
				return true;
			default:
				throw new InvalidOperationException("Should not happen.");
			}
		}

		private void SetCurrent(XPathNodeIterator iter)
		{
			if (_current == null)
			{
				_current = iter.Current.Clone();
			}
			else if (!_current.MoveTo(iter.Current))
			{
				_current = iter.Current.Clone();
			}
		}
	}
}
