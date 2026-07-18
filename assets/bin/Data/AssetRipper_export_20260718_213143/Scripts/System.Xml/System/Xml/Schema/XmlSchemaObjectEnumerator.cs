using System.Collections;

namespace System.Xml.Schema
{
	public class XmlSchemaObjectEnumerator : IEnumerator
	{
		private IEnumerator ienum;

		object IEnumerator.Current
		{
			get
			{
				return (XmlSchemaObject)ienum.Current;
			}
		}

		public XmlSchemaObject Current
		{
			get
			{
				return (XmlSchemaObject)ienum.Current;
			}
		}

		internal XmlSchemaObjectEnumerator(IList list)
		{
			ienum = list.GetEnumerator();
		}

		bool IEnumerator.MoveNext()
		{
			return ienum.MoveNext();
		}

		void IEnumerator.Reset()
		{
			ienum.Reset();
		}

		public bool MoveNext()
		{
			return ienum.MoveNext();
		}

		public void Reset()
		{
			ienum.Reset();
		}
	}
}
