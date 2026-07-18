namespace LitJson
{
	public enum JsonToken
	{
		None = 0,
		ObjectStart = 1,
		PropertyName = 2,
		ObjectEnd = 3,
		ArrayStart = 4,
		ArrayEnd = 5,
		Int = 6,
		Long = 7,
		Float = 8,
		Double = 9,
		String = 10,
		Boolean = 11,
		Null = 12
	}
}
