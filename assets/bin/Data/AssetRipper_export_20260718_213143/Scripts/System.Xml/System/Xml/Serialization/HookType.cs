namespace System.Xml.Serialization
{
	[XmlType("hookType")]
	internal enum HookType
	{
		attributes = 0,
		elements = 1,
		unknownAttribute = 2,
		unknownElement = 3,
		member = 4,
		type = 5
	}
}
