using System.Xml.XPath;

namespace System.Xml
{
	public class XmlProcessingInstruction : XmlLinkedNode
	{
		private string target;

		private string data;

		public string Data
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
			}
		}

		public override string InnerText
		{
			get
			{
				return Data;
			}
			set
			{
				data = value;
			}
		}

		public override string LocalName
		{
			get
			{
				return target;
			}
		}

		public override string Name
		{
			get
			{
				return target;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.ProcessingInstruction;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.ProcessingInstruction;
			}
		}

		public string Target
		{
			get
			{
				return target;
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
				if (IsReadOnly)
				{
					throw new ArgumentException("This node is read-only.");
				}
				data = value;
			}
		}

		protected internal XmlProcessingInstruction(string target, string data, XmlDocument doc)
			: base(doc)
		{
			XmlConvert.VerifyName(target);
			if (data == null)
			{
				data = string.Empty;
			}
			this.target = target;
			this.data = data;
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlProcessingInstruction(target, data, OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteProcessingInstruction(target, data);
		}
	}
}
