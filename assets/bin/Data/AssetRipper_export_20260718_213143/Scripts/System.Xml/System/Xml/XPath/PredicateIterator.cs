namespace System.Xml.XPath
{
	internal class PredicateIterator : BaseIterator
	{
		private BaseIterator _iter;

		private Expression _pred;

		private XPathResultType resType;

		private bool finished;

		public override XPathNavigator Current
		{
			get
			{
				return (CurrentPosition != 0) ? _iter.Current : null;
			}
		}

		public override bool ReverseAxis
		{
			get
			{
				return _iter.ReverseAxis;
			}
		}

		public PredicateIterator(BaseIterator iter, Expression pred)
			: base(iter.NamespaceManager)
		{
			_iter = iter;
			_pred = pred;
			resType = pred.GetReturnType(iter);
		}

		private PredicateIterator(PredicateIterator other)
			: base(other)
		{
			_iter = (BaseIterator)other._iter.Clone();
			_pred = other._pred;
			resType = other.resType;
			finished = other.finished;
		}

		public override XPathNodeIterator Clone()
		{
			return new PredicateIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (finished)
			{
				return false;
			}
			while (_iter.MoveNext())
			{
				switch (resType)
				{
				case XPathResultType.Number:
					if (_pred.EvaluateNumber(_iter) != (double)_iter.ComparablePosition)
					{
						continue;
					}
					break;
				case XPathResultType.Any:
				{
					object obj = _pred.Evaluate(_iter);
					if (obj is double)
					{
						if ((double)obj != (double)_iter.ComparablePosition)
						{
							continue;
						}
					}
					else if (!XPathFunctions.ToBoolean(obj))
					{
						continue;
					}
					break;
				}
				default:
					if (!_pred.EvaluateBoolean(_iter))
					{
						continue;
					}
					break;
				}
				return true;
			}
			finished = true;
			return false;
		}

		public override string ToString()
		{
			return _iter.GetType().FullName;
		}
	}
}
