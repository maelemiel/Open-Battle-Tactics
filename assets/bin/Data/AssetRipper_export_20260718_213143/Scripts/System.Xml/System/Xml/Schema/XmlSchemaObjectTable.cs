using System.Collections;
using System.Collections.Specialized;

namespace System.Xml.Schema
{
	public class XmlSchemaObjectTable
	{
		internal class XmlSchemaObjectTableEnumerator : IEnumerator, IDictionaryEnumerator
		{
			private IDictionaryEnumerator xenum;

			private IEnumerable tmp;

			object IEnumerator.Current
			{
				get
				{
					return xenum.Entry;
				}
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					return xenum.Entry;
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					return (XmlQualifiedName)xenum.Key;
				}
			}

			object IDictionaryEnumerator.Value
			{
				get
				{
					return (XmlSchemaObject)xenum.Value;
				}
			}

			public XmlSchemaObject Current
			{
				get
				{
					return (XmlSchemaObject)xenum.Value;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					return xenum.Entry;
				}
			}

			public XmlQualifiedName Key
			{
				get
				{
					return (XmlQualifiedName)xenum.Key;
				}
			}

			public XmlSchemaObject Value
			{
				get
				{
					return (XmlSchemaObject)xenum.Value;
				}
			}

			internal XmlSchemaObjectTableEnumerator(XmlSchemaObjectTable table)
			{
				tmp = table.table;
				xenum = (IDictionaryEnumerator)tmp.GetEnumerator();
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

		private HybridDictionary table;

		public int Count
		{
			get
			{
				return table.Count;
			}
		}

		public XmlSchemaObject this[XmlQualifiedName name]
		{
			get
			{
				return (XmlSchemaObject)table[name];
			}
		}

		public ICollection Names
		{
			get
			{
				return table.Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				return table.Values;
			}
		}

		internal XmlSchemaObjectTable()
		{
			table = new HybridDictionary();
		}

		public bool Contains(XmlQualifiedName name)
		{
			return table.Contains(name);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new XmlSchemaObjectTableEnumerator(this);
		}

		internal void Add(XmlQualifiedName name, XmlSchemaObject value)
		{
			table[name] = value;
		}

		internal void Clear()
		{
			table.Clear();
		}

		internal void Set(XmlQualifiedName name, XmlSchemaObject value)
		{
			table[name] = value;
		}
	}
}
