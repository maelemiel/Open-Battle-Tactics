namespace System.Xml.XPath
{
	internal class SimpleSlashIterator : BaseIterator
	{
		private NodeSet _expr;

		private BaseIterator _left;

		private BaseIterator _right;

		private XPathNavigator _current;

		public override XPathNavigator Current
		{
			get
			{
				return _current;
			}
		}

		public SimpleSlashIterator(BaseIterator left, NodeSet expr)
			: base(left.NamespaceManager)
		{
			_left = left;
			_expr = expr;
		}

		private SimpleSlashIterator(SimpleSlashIterator other)
			: base(other)
		{
			_expr = other._expr;
			_left = (BaseIterator)other._left.Clone();
			if (other._right != null)
			{
				_right = (BaseIterator)other._right.Clone();
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new SimpleSlashIterator(this);
		}

		public override bool MoveNextCore()
		{
			while (_right == null || !_right.MoveNext())
			{
				if (!_left.MoveNext())
				{
					return false;
				}
				_right = _expr.EvaluateNodeSet(_left);
			}
			if (_current == null)
			{
				_current = _right.Current.Clone();
			}
			else if (!_current.MoveTo(_right.Current))
			{
				_current = _right.Current.Clone();
			}
			return true;
		}
	}
}
