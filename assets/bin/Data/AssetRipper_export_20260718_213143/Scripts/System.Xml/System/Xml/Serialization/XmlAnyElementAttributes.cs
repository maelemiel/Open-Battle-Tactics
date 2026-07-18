using System.Collections;
using System.Text;

namespace System.Xml.Serialization
{
	public class XmlAnyElementAttributes : CollectionBase
	{
		public XmlAnyElementAttribute this[int index]
		{
			get
			{
				return (XmlAnyElementAttribute)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public int Add(XmlAnyElementAttribute attribute)
		{
			return base.List.Add(attribute);
		}

		public bool Contains(XmlAnyElementAttribute attribute)
		{
			return base.List.Contains(attribute);
		}

		public int IndexOf(XmlAnyElementAttribute attribute)
		{
			return base.List.IndexOf(attribute);
		}

		public void Insert(int index, XmlAnyElementAttribute attribute)
		{
			base.List.Insert(index, attribute);
		}

		public void Remove(XmlAnyElementAttribute attribute)
		{
			base.List.Remove(attribute);
		}

		public void CopyTo(XmlAnyElementAttribute[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		internal void AddKeyHash(StringBuilder sb)
		{
			if (Count != 0)
			{
				sb.Append("XAEAS ");
				for (int i = 0; i < Count; i++)
				{
					this[i].AddKeyHash(sb);
				}
				sb.Append('|');
			}
		}
	}
}
