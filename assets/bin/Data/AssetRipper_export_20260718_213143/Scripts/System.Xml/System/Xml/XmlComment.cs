using System.Xml.XPath;

namespace System.Xml
{
	public class XmlComment : XmlCharacterData
	{
		public override string LocalName
		{
			get
			{
				return "#comment";
			}
		}

		public override string Name
		{
			get
			{
				return "#comment";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Comment;
			}
		}

		internal override XPathNodeType XPathNodeType
		{
			get
			{
				return XPathNodeType.Comment;
			}
		}

		protected internal XmlComment(string comment, XmlDocument doc)
			: base(comment, doc)
		{
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlComment(Value, OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteComment(Data);
		}
	}
}
