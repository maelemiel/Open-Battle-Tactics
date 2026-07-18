namespace System.Net.Mime
{
	public static class MediaTypeNames
	{
		public static class Application
		{
			private const string prefix = "application/";

			public const string Octet = "application/octet-stream";

			public const string Pdf = "application/pdf";

			public const string Rtf = "application/rtf";

			public const string Soap = "application/soap+xml";

			public const string Zip = "application/zip";
		}

		public static class Image
		{
			private const string prefix = "image/";

			public const string Gif = "image/gif";

			public const string Jpeg = "image/jpeg";

			public const string Tiff = "image/tiff";
		}

		public static class Text
		{
			private const string prefix = "text/";

			public const string Html = "text/html";

			public const string Plain = "text/plain";

			public const string RichText = "text/richtext";

			public const string Xml = "text/xml";
		}
	}
}
