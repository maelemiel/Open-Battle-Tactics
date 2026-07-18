namespace System.Xml.XPath
{
	public enum XPathResultType
	{
		Number = 0,
		String = 1,
		Boolean = 2,
		NodeSet = 3,
		[MonoFIX("MS.NET: 1")]
		Navigator = 4,
		Any = 5,
		Error = 6
	}
}
