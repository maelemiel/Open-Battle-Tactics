using System.Collections;

namespace System.Xml.Schema
{
	public class XmlSchemaObjectCollection : CollectionBase
	{
		public virtual XmlSchemaObject this[int index]
		{
			get
			{
				return (XmlSchemaObject)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public XmlSchemaObjectCollection()
		{
		}

		public XmlSchemaObjectCollection(XmlSchemaObject parent)
		{
		}

		public int Add(XmlSchemaObject item)
		{
			return base.List.Add(item);
		}

		public bool Contains(XmlSchemaObject item)
		{
			return base.List.Contains(item);
		}

		public void CopyTo(XmlSchemaObject[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public new XmlSchemaObjectEnumerator GetEnumerator()
		{
			return new XmlSchemaObjectEnumerator(base.List);
		}

		public int IndexOf(XmlSchemaObject item)
		{
			return base.List.IndexOf(item);
		}

		public void Insert(int index, XmlSchemaObject item)
		{
			base.List.Insert(index, item);
		}

		protected override void OnClear()
		{
		}

		protected override void OnInsert(int index, object item)
		{
		}

		protected override void OnRemove(int index, object item)
		{
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
		}

		public void Remove(XmlSchemaObject item)
		{
			base.List.Remove(item);
		}
	}
}
