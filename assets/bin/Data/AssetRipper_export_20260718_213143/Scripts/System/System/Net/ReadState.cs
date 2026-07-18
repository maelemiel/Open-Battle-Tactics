namespace System.Net
{
	internal enum ReadState
	{
		None = 0,
		Status = 1,
		Headers = 2,
		Content = 3
	}
}
