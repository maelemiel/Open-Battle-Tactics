namespace System.Xml.XPath
{
	internal class WrapperIterator : BaseIterator
	{
		private XPathNodeIterator iter;

		public override XPathNavigator Current
		{
			get
			{
				return iter.Current;
			}
		}

		public WrapperIterator(XPathNodeIterator iter, IXmlNamespaceResolver nsm)
			: base(nsm)
		{
			this.iter = iter;
		}

		private WrapperIterator(WrapperIterator other)
			: base(other)
		{
			iter = other.iter.Clone();
		}

		public override XPathNodeIterator Clone()
		{
			return new WrapperIterator(this);
		}

		public override bool MoveNextCore()
		{
			return iter.MoveNext();
		}
	}
}
