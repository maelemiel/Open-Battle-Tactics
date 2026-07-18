using System.IO;
using System.Net;

namespace System.Xml
{
	public abstract class XmlResolver
	{
		public abstract ICredentials Credentials { set; }

		public abstract object GetEntity(Uri absoluteUri, string role, Type type);

		public virtual Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			if (baseUri == null)
			{
				if (relativeUri == null)
				{
					throw new ArgumentNullException("Either baseUri or relativeUri are required.");
				}
				if (relativeUri.StartsWith("http:") || relativeUri.StartsWith("https:") || relativeUri.StartsWith("ftp:") || relativeUri.StartsWith("file:"))
				{
					return new Uri(relativeUri);
				}
				return new Uri(Path.GetFullPath(relativeUri));
			}
			if (relativeUri == null)
			{
				return baseUri;
			}
			return new Uri(baseUri, EscapeRelativeUriBody(relativeUri));
		}

		private string EscapeRelativeUriBody(string src)
		{
			return src.Replace("<", "%3C").Replace(">", "%3E").Replace("#", "%23")
				.Replace("%", "%25")
				.Replace("\"", "%22");
		}
	}
}
