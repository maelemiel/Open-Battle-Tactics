namespace System.Xml
{
	public enum WriteState
	{
		Start = 0,
		Prolog = 1,
		Element = 2,
		Attribute = 3,
		Content = 4,
		Closed = 5,
		Error = 6
	}
}
