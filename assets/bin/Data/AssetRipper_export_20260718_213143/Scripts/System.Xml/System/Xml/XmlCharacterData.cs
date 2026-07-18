using System.Xml.XPath;

namespace System.Xml
{
	public abstract class XmlCharacterData : XmlLinkedNode
	{
		private string data;

		public virtual string Data
		{
			get
			{
				return data;
			}
			set
			{
				string oldValue = data;
				OwnerDocument.onNodeChanging(this, ParentNode, oldValue, value);
				data = value;
				OwnerDocument.onNodeChanged(this, ParentNode, oldValue, value);
			}
		}

		public override string InnerText
		{
			get
			{
				return data;
			}
			set
			{
				Data = value;
			}
		}

		public virtual int Length
		{
			get
			{
				return (data != null) ? data.Length : 0;
			}
		}

		public override string Value
		{
			get
			{
				return data;
			}
			set
			{
				Data = value;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Text;
			}
		}

		protected internal XmlCharacterData(string data, XmlDocument doc)
			: base(doc)
		{
			if (data == null)
			{
				data = string.Empty;
			}
			this.data = data;
		}

		public virtual void AppendData(string strData)
		{
			string oldValue = data;
			string newValue = (data += strData);
			OwnerDocument.onNodeChanging(this, ParentNode, oldValue, newValue);
			data = newValue;
			OwnerDocument.onNodeChanged(this, ParentNode, oldValue, newValue);
		}

		public virtual void DeleteData(int offset, int count)
		{
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be non-negative and must not be greater than the length of this instance.");
			}
			int count2 = data.Length - offset;
			if (offset + count < data.Length)
			{
				count2 = count;
			}
			string oldValue = data;
			string newValue = data.Remove(offset, count2);
			OwnerDocument.onNodeChanging(this, ParentNode, oldValue, newValue);
			data = newValue;
			OwnerDocument.onNodeChanged(this, ParentNode, oldValue, newValue);
		}

		public virtual void InsertData(int offset, string strData)
		{
			if (offset < 0 || offset > data.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be non-negative and must not be greater than the length of this instance.");
			}
			string oldValue = data;
			string newValue = data.Insert(offset, strData);
			OwnerDocument.onNodeChanging(this, ParentNode, oldValue, newValue);
			data = newValue;
			OwnerDocument.onNodeChanged(this, ParentNode, oldValue, newValue);
		}

		public virtual void ReplaceData(int offset, int count, string strData)
		{
			if (offset < 0 || offset > data.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be non-negative and must not be greater than the length of this instance.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Must be non-negative.");
			}
			if (strData == null)
			{
				throw new ArgumentNullException("strData", "Must be non-null.");
			}
			string oldValue = data;
			string text = data.Substring(0, offset) + strData;
			if (offset + count < data.Length)
			{
				text += data.Substring(offset + count);
			}
			OwnerDocument.onNodeChanging(this, ParentNode, oldValue, text);
			data = text;
			OwnerDocument.onNodeChanged(this, ParentNode, oldValue, text);
		}

		public virtual string Substring(int offset, int count)
		{
			if (data.Length < offset + count)
			{
				return data.Substring(offset);
			}
			return data.Substring(offset, count);
		}
	}
}
