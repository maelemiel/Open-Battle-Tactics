using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public enum XmlSchemaForm
	{
		[XmlIgnore]
		None = 0,
		[XmlEnum("qualified")]
		Qualified = 1,
		[XmlEnum("unqualified")]
		Unqualified = 2
	}
}
