using System.Collections;

namespace System.Xml.XPath
{
	public abstract class XPathNodeIterator : IEnumerable, ICloneable
	{
		private int _count = -1;

		public virtual int Count
		{
			get
			{
				if (_count == -1)
				{
					XPathNodeIterator xPathNodeIterator = Clone();
					while (xPathNodeIterator.MoveNext())
					{
					}
					_count = xPathNodeIterator.CurrentPosition;
				}
				return _count;
			}
		}

		public abstract XPathNavigator Current { get; }

		public abstract int CurrentPosition { get; }

		object ICloneable.Clone()
		{
			return Clone();
		}

		public abstract XPathNodeIterator Clone();

		public virtual IEnumerator GetEnumerator()
		{
			while (MoveNext())
			{
				yield return Current;
			}
		}

		public abstract bool MoveNext();
	}
}
