namespace System.Xml.XPath
{
	internal class AxisIterator : BaseIterator
	{
		private BaseIterator _iter;

		private NodeTest _test;

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

		public AxisIterator(BaseIterator iter, NodeTest test)
			: base(iter.NamespaceManager)
		{
			_iter = iter;
			_test = test;
		}

		private AxisIterator(AxisIterator other)
			: base(other)
		{
			_iter = (BaseIterator)other._iter.Clone();
			_test = other._test;
		}

		public override XPathNodeIterator Clone()
		{
			return new AxisIterator(this);
		}

		public override bool MoveNextCore()
		{
			while (_iter.MoveNext())
			{
				if (_test.Match(base.NamespaceManager, _iter.Current))
				{
					return true;
				}
			}
			return false;
		}
	}
}
