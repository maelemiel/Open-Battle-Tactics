namespace System.Xml.XPath
{
	internal class AttributeIterator : SimpleIterator
	{
		public AttributeIterator(BaseIterator iter)
			: base(iter)
		{
		}

		private AttributeIterator(AttributeIterator other)
			: base(other, true)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new AttributeIterator(this);
		}

		public override bool MoveNextCore()
		{
			if (CurrentPosition == 0)
			{
				if (_nav.MoveToFirstAttribute())
				{
					return true;
				}
			}
			else if (_nav.MoveToNextAttribute())
			{
				return true;
			}
			return false;
		}
	}
}
