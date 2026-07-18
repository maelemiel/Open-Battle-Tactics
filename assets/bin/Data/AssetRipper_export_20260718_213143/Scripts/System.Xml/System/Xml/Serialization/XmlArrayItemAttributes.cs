using System.Collections;
using System.Text;

namespace System.Xml.Serialization
{
	public class XmlArrayItemAttributes : CollectionBase
	{
		public XmlArrayItemAttribute this[int index]
		{
			get
			{
				return (XmlArrayItemAttribute)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public int Add(XmlArrayItemAttribute attribute)
		{
			return base.List.Add(attribute);
		}

		public bool Contains(XmlArrayItemAttribute attribute)
		{
			return base.List.Contains(attribute);
		}

		public void CopyTo(XmlArrayItemAttribute[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		public int IndexOf(XmlArrayItemAttribute attribute)
		{
			return base.List.IndexOf(attribute);
		}

		public void Insert(int index, XmlArrayItemAttribute attribute)
		{
			base.List.Insert(index, attribute);
		}

		public void Remove(XmlArrayItemAttribute attribute)
		{
			base.List.Remove(attribute);
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			if (Count != 0)
			{
				sb.Append("XAIAS ");
				for (int i = 0; i < Count; i++)
				{
					this[i].AddKeyHash(sb);
				}
				sb.Append('|');
			}
		}
	}
}
