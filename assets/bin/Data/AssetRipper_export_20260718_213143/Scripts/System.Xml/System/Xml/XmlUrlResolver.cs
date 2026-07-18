using System.IO;
using System.Net;

namespace System.Xml
{
	public class XmlUrlResolver : XmlResolver
	{
		private ICredentials credential;

		public override ICredentials Credentials
		{
			set
			{
				credential = value;
			}
		}

		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (ofObjectToReturn == null)
			{
				ofObjectToReturn = typeof(Stream);
			}
			if (ofObjectToReturn != typeof(Stream))
			{
				throw new XmlException("This object type is not supported.");
			}
			if (!absoluteUri.IsAbsoluteUri)
			{
				throw new ArgumentException("uri must be absolute.", "absoluteUri");
			}
			if (absoluteUri.Scheme == "file")
			{
				if (absoluteUri.AbsolutePath == string.Empty)
				{
					throw new ArgumentException("uri must be absolute.", "absoluteUri");
				}
				return new FileStream(UnescapeRelativeUriBody(absoluteUri.LocalPath), FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			WebRequest webRequest = WebRequest.Create(absoluteUri);
			if (credential != null)
			{
				webRequest.Credentials = credential;
			}
			return webRequest.GetResponse().GetResponseStream();
		}

		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			return base.ResolveUri(baseUri, relativeUri);
		}

		private string UnescapeRelativeUriBody(string src)
		{
			return src.Replace("%3C", "<").Replace("%3E", ">").Replace("%23", "#")
				.Replace("%22", "\"")
				.Replace("%20", " ")
				.Replace("%25", "%");
		}
	}
}
