using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
	public class AlternateView : AttachmentBase
	{
		private Uri baseUri;

		private LinkedResourceCollection linkedResources = new LinkedResourceCollection();

		public Uri BaseUri
		{
			get
			{
				return baseUri;
			}
			set
			{
				baseUri = value;
			}
		}

		public LinkedResourceCollection LinkedResources
		{
			get
			{
				return linkedResources;
			}
		}

		public AlternateView(string fileName)
			: base(fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public AlternateView(string fileName, ContentType contentType)
			: base(fileName, contentType)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public AlternateView(string fileName, string mediaType)
			: base(fileName, mediaType)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public AlternateView(Stream contentStream)
			: base(contentStream)
		{
		}

		public AlternateView(Stream contentStream, string mediaType)
			: base(contentStream, mediaType)
		{
		}

		public AlternateView(Stream contentStream, ContentType contentType)
			: base(contentStream, contentType)
		{
		}

		public static AlternateView CreateAlternateViewFromString(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
			AlternateView alternateView = new AlternateView(memoryStream);
			alternateView.TransferEncoding = TransferEncoding.QuotedPrintable;
			return alternateView;
		}

		public static AlternateView CreateAlternateViewFromString(string content, ContentType contentType)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			Encoding encoding = ((contentType.CharSet == null) ? Encoding.UTF8 : Encoding.GetEncoding(contentType.CharSet));
			MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(content));
			AlternateView alternateView = new AlternateView(memoryStream, contentType);
			alternateView.TransferEncoding = TransferEncoding.QuotedPrintable;
			return alternateView;
		}

		public static AlternateView CreateAlternateViewFromString(string content, Encoding encoding, string mediaType)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}
			MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(content));
			ContentType contentType = new ContentType();
			contentType.MediaType = mediaType;
			contentType.CharSet = encoding.HeaderName;
			AlternateView alternateView = new AlternateView(memoryStream, contentType);
			alternateView.TransferEncoding = TransferEncoding.QuotedPrintable;
			return alternateView;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (LinkedResource linkedResource in linkedResources)
				{
					linkedResource.Dispose();
				}
			}
			base.Dispose(disposing);
		}
	}
}
