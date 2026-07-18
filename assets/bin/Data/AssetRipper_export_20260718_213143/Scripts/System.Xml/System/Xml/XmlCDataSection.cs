namespace System.Xml
{
	public class XmlCDataSection : XmlCharacterData
	{
		public override string LocalName
		{
			get
			{
				return "#cdata-section";
			}
		}

		public override string Name
		{
			get
			{
				return "#cdata-section";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.CDATA;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return base.ParentNode;
			}
		}

		protected internal XmlCDataSection(string data, XmlDocument doc)
			: base(data, doc)
		{
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlCDataSection(Data, OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteCData(Data);
		}
	}
}
