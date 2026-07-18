using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
	public class LinkedResource : AttachmentBase
	{
		private Uri contentLink;

		public Uri ContentLink
		{
			get
			{
				return contentLink;
			}
			set
			{
				contentLink = value;
			}
		}

		public LinkedResource(string fileName)
			: base(fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(string fileName, ContentType contentType)
			: base(fileName, contentType)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(string fileName, string mediaType)
			: base(fileName, mediaType)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(Stream contentStream)
			: base(contentStream)
		{
			if (contentStream == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(Stream contentStream, ContentType contentType)
			: base(contentStream, contentType)
		{
			if (contentStream == null)
			{
				throw new ArgumentNullException();
			}
		}

		public LinkedResource(Stream contentStream, string mediaType)
			: base(contentStream, mediaType)
		{
			if (contentStream == null)
			{
				throw new ArgumentNullException();
			}
		}

		public static LinkedResource CreateLinkedResourceFromString(string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(content));
			LinkedResource linkedResource = new LinkedResource(memoryStream);
			linkedResource.TransferEncoding = TransferEncoding.QuotedPrintable;
			return linkedResource;
		}

		public static LinkedResource CreateLinkedResourceFromString(string content, ContentType contentType)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			MemoryStream memoryStream = new MemoryStream(Encoding.Default.GetBytes(content));
			LinkedResource linkedResource = new LinkedResource(memoryStream, contentType);
			linkedResource.TransferEncoding = TransferEncoding.QuotedPrintable;
			return linkedResource;
		}

		public static LinkedResource CreateLinkedResourceFromString(string content, Encoding contentEncoding, string mediaType)
		{
			if (content == null)
			{
				throw new ArgumentNullException();
			}
			MemoryStream memoryStream = new MemoryStream(contentEncoding.GetBytes(content));
			LinkedResource linkedResource = new LinkedResource(memoryStream, mediaType);
			linkedResource.TransferEncoding = TransferEncoding.QuotedPrintable;
			return linkedResource;
		}
	}
}
