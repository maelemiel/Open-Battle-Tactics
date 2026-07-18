using System.Xml.XPath;

namespace System.Xml
{
	public class XmlSignificantWhitespace : XmlCharacterData
	{
		public override string LocalName
		{
			get
			{
				return "#significant-whitespace";
			}
		}

		public override string Name
		{
			get
			{
				return "#significant-whitespace";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.SignificantWhitespace;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.SignificantWhitespace;
			}
		}

		public override string Value
		{
			get
			{
				return Data;
			}
			set
			{
				if (!XmlChar.IsWhitespace(value))
				{
					throw new ArgumentException("Invalid whitespace characters.");
				}
				base.Data = value;
			}
		}

		public override XmlNode ParentNode
		{
			get
			{
				return base.ParentNode;
			}
		}

		protected internal XmlSignificantWhitespace(string strData, XmlDocument doc)
			: base(strData, doc)
		{
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlSignificantWhitespace(Data, OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteWhitespace(Data);
		}
	}
}
