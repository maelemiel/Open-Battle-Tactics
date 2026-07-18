using System.Collections;

namespace System.Xml.Schema
{
	public sealed class XmlSchemaCollectionEnumerator : IEnumerator
	{
		private IEnumerator xenum;

		object IEnumerator.Current
		{
			get
			{
				return xenum.Current;
			}
		}

		public XmlSchema Current
		{
			get
			{
				return (XmlSchema)xenum.Current;
			}
		}

		internal XmlSchemaCollectionEnumerator(ICollection col)
		{
			xenum = col.GetEnumerator();
		}

		bool IEnumerator.MoveNext()
		{
			return xenum.MoveNext();
		}

		void IEnumerator.Reset()
		{
			xenum.Reset();
		}

		public bool MoveNext()
		{
			return xenum.MoveNext();
		}
	}
}
