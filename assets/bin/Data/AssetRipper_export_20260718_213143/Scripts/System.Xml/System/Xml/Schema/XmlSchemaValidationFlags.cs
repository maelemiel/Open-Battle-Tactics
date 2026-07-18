namespace System.Xml.Schema
{
	[Flags]
	public enum XmlSchemaValidationFlags
	{
		None = 0,
		ProcessInlineSchema = 1,
		ProcessSchemaLocation = 2,
		ReportValidationWarnings = 4,
		ProcessIdentityConstraints = 8,
		[Obsolete("It is really idiotic idea to include such validation option that breaks W3C XML Schema specification compliance and interoperability.")]
		AllowXmlAttributes = 0x10
	}
}
