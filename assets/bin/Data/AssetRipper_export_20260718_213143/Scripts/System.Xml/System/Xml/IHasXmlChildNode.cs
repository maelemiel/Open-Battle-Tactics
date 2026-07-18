namespace System.Xml
{
	internal interface IHasXmlChildNode
	{
		XmlLinkedNode LastLinkedChild { get; set; }
	}
}
