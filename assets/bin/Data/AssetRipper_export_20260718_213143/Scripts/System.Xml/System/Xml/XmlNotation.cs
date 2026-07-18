namespace System.Xml
{
	public class XmlNotation : XmlNode
	{
		private string localName;

		private string publicId;

		private string systemId;

		private string prefix;

		public override string InnerXml
		{
			get
			{
				return string.Empty;
			}
			set
			{
				throw new InvalidOperationException("This operation is not allowed.");
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public override string LocalName
		{
			get
			{
				return localName;
			}
		}

		public override string Name
		{
			get
			{
				return (!(prefix != string.Empty)) ? localName : (prefix + ":" + localName);
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Notation;
			}
		}

		public override string OuterXml
		{
			get
			{
				return string.Empty;
			}
		}

		public string PublicId
		{
			get
			{
				if (publicId != null)
				{
					return publicId;
				}
				return null;
			}
		}

		public string SystemId
		{
			get
			{
				if (systemId != null)
				{
					return systemId;
				}
				return null;
			}
		}

		internal XmlNotation(string localName, string prefix, string publicId, string systemId, XmlDocument doc)
			: base(doc)
		{
			this.localName = doc.NameTable.Add(localName);
			this.prefix = doc.NameTable.Add(prefix);
			this.publicId = publicId;
			this.systemId = systemId;
		}

		public override XmlNode CloneNode(bool deep)
		{
			throw new InvalidOperationException("This operation is not allowed.");
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
		}
	}
}
