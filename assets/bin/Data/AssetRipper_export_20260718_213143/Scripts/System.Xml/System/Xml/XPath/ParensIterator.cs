namespace System.Xml.XPath
{
	internal class ParensIterator : BaseIterator
	{
		private BaseIterator _iter;

		public override XPathNavigator Current
		{
			get
			{
				return _iter.Current;
			}
		}

		public override int Count
		{
			get
			{
				return _iter.Count;
			}
		}

		public ParensIterator(BaseIterator iter)
			: base(iter.NamespaceManager)
		{
			_iter = iter;
		}

		private ParensIterator(ParensIterator other)
			: base(other)
		{
			_iter = (BaseIterator)other._iter.Clone();
		}

		public override XPathNodeIterator Clone()
		{
			return new ParensIterator(this);
		}

		public override bool MoveNextCore()
		{
			return _iter.MoveNext();
		}
	}
}
