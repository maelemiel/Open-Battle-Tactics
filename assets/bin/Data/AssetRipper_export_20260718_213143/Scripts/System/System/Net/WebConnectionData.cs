using System.IO;

namespace System.Net
{
	internal class WebConnectionData
	{
		public HttpWebRequest request;

		public int StatusCode;

		public string StatusDescription;

		public WebHeaderCollection Headers;

		public Version Version;

		public Stream stream;

		public string Challenge;

		public void Init()
		{
			request = null;
			StatusCode = 0;
			StatusDescription = null;
			Headers = null;
			stream = null;
		}
	}
}
