namespace System.Net
{
	internal class FtpRequestCreator : IWebRequestCreate
	{
		public WebRequest Create(Uri uri)
		{
			return new FtpWebRequest(uri);
		}
	}
}
