using System.Collections;

namespace System.Xml.XPath
{
	internal class SlashIterator : BaseIterator
	{
		private BaseIterator _iterLeft;

		private BaseIterator _iterRight;

		private NodeSet _expr;

		private SortedList _iterList;

		private bool _finished;

		private BaseIterator _nextIterRight;

		public override XPathNavigator Current
		{
			get
			{
				return (CurrentPosition != 0) ? _iterRight.Current : null;
			}
		}

		public SlashIterator(BaseIterator iter, NodeSet expr)
			: base(iter.NamespaceManager)
		{
			_iterLeft = iter;
			_expr = expr;
		}

		private SlashIterator(SlashIterator other)
			: base(other)
		{
			_iterLeft = (BaseIterator)other._iterLeft.Clone();
			if (other._iterRight != null)
			{
				_iterRight = (BaseIterator)other._iterRight.Clone();
			}
			_expr = other._expr;
			if (other._iterList != null)
			{
				_iterList = (SortedList)other._iterList.Clone();
			}
			_finished = other._finished;
			if (other._nextIterRight != null)
			{
				_nextIterRight = (BaseIterator)other._nextIterRight.Clone();
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new SlashIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (_finished)
			{
				return false;
			}
			if (_iterRight == null)
			{
				if (!_iterLeft.MoveNext())
				{
					return false;
				}
				_iterRight = _expr.EvaluateNodeSet(_iterLeft);
				_iterList = new SortedList(XPathIteratorComparer.Instance);
			}
			while (!_iterRight.MoveNext())
			{
				if (_iterList.Count > 0)
				{
					int index = _iterList.Count - 1;
					_iterRight = (BaseIterator)_iterList.GetByIndex(index);
					_iterList.RemoveAt(index);
					break;
				}
				if (_nextIterRight != null)
				{
					_iterRight = _nextIterRight;
					_nextIterRight = null;
					break;
				}
				if (!_iterLeft.MoveNext())
				{
					_finished = true;
					return false;
				}
				_iterRight = _expr.EvaluateNodeSet(_iterLeft);
			}
			bool flag = true;
			while (flag)
			{
				flag = false;
				if (_nextIterRight == null)
				{
					bool flag2 = false;
					while (_nextIterRight == null || !_nextIterRight.MoveNext())
					{
						if (_iterLeft.MoveNext())
						{
							_nextIterRight = _expr.EvaluateNodeSet(_iterLeft);
							continue;
						}
						flag2 = true;
						break;
					}
					if (flag2)
					{
						_nextIterRight = null;
					}
				}
				if (_nextIterRight == null)
				{
					continue;
				}
				switch (_iterRight.Current.ComparePosition(_nextIterRight.Current))
				{
				case XmlNodeOrder.After:
					_iterList[_iterRight] = _iterRight;
					_iterRight = _nextIterRight;
					_nextIterRight = null;
					flag = true;
					break;
				case XmlNodeOrder.Same:
					if (!_nextIterRight.MoveNext())
					{
						_nextIterRight = null;
					}
					else
					{
						int count = _iterList.Count;
						_iterList[_nextIterRight] = _nextIterRight;
						if (count != _iterList.Count)
						{
							_nextIterRight = (BaseIterator)_iterList.GetByIndex(count);
							_iterList.RemoveAt(count);
						}
					}
					flag = true;
					break;
				}
			}
			return true;
		}
	}
}
