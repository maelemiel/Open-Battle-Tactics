namespace System.Xml.XPath
{
	internal class NamespaceIterator : SimpleIterator
	{
		public NamespaceIterator(BaseIterator iter)
			: base(iter)
		{
		}

		private NamespaceIterator(NamespaceIterator other)
			: base(other, true)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new NamespaceIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (CurrentPosition == 0)
			{
				if (_nav.MoveToFirstNamespace())
				{
					return true;
				}
			}
			else if (_nav.MoveToNextNamespace())
			{
				return true;
			}
			return false;
		}
	}
}
