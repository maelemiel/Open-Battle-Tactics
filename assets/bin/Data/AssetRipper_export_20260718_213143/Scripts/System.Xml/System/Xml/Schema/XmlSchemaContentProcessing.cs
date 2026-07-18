using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public enum XmlSchemaContentProcessing
	{
		[XmlIgnore]
		None = 0,
		[XmlEnum("skip")]
		Skip = 1,
		[XmlEnum("lax")]
		Lax = 2,
		[XmlEnum("strict")]
		Strict = 3
	}
}
