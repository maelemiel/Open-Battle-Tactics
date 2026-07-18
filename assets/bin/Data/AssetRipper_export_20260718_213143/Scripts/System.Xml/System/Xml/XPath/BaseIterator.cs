namespace System.Xml.XPath
{
	internal abstract class BaseIterator : XPathNodeIterator
	{
		private IXmlNamespaceResolver _nsm;

		private int position;

		public IXmlNamespaceResolver NamespaceManager
		{
			get
			{
				return _nsm;
			}
			set
			{
				_nsm = value;
			}
		}

		public virtual bool ReverseAxis
		{
			get
			{
				return false;
			}
		}

		public int ComparablePosition
		{
			get
			{
				if (ReverseAxis)
				{
					int num = Count - CurrentPosition + 1;
					return (num < 1) ? 1 : num;
				}
				return CurrentPosition;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return position;
			}
		}

		internal BaseIterator(BaseIterator other)
		{
			_nsm = other._nsm;
			position = other.position;
		}

		internal BaseIterator(IXmlNamespaceResolver nsm)
		{
			_nsm = nsm;
		}

		internal void SetPosition(int pos)
		{
			position = pos;
		}

		public override bool MoveNext()
		{
			if (!MoveNextCore())
			{
				return false;
			}
			position++;
			return true;
		}

		public abstract bool MoveNextCore();

		internal XPathNavigator PeekNext()
		{
			XPathNodeIterator xPathNodeIterator = Clone();
			return (!xPathNodeIterator.MoveNext()) ? null : xPathNodeIterator.Current;
		}

		public override string ToString()
		{
			if (Current != null)
			{
				return Current.NodeType.ToString() + "[" + CurrentPosition + "] : " + Current.Name + " = " + Current.Value;
			}
			return GetType().ToString() + "[" + CurrentPosition + "]";
		}
	}
}
