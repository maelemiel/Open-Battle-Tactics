using System.IO;
using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public class FileWebResponse : WebResponse, IDisposable, ISerializable
	{
		private Uri responseUri;

		private FileStream fileStream;

		private long contentLength;

		private WebHeaderCollection webHeaders;

		private bool disposed;

		public override long ContentLength
		{
			get
			{
				CheckDisposed();
				return contentLength;
			}
		}

		public override string ContentType
		{
			get
			{
				CheckDisposed();
				return "application/octet-stream";
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				CheckDisposed();
				return webHeaders;
			}
		}

		public override Uri ResponseUri
		{
			get
			{
				CheckDisposed();
				return responseUri;
			}
		}

		internal FileWebResponse(Uri responseUri, FileStream fileStream)
		{
			try
			{
				this.responseUri = responseUri;
				this.fileStream = fileStream;
				contentLength = fileStream.Length;
				webHeaders = new WebHeaderCollection();
				webHeaders.Add("Content-Length", Convert.ToString(contentLength));
				webHeaders.Add("Content-Type", "application/octet-stream");
			}
			catch (Exception ex)
			{
				throw new WebException(ex.Message, ex);
			}
		}

		[Obsolete("Serialization is obsoleted for this type", false)]
		protected FileWebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			responseUri = (Uri)serializationInfo.GetValue("responseUri", typeof(Uri));
			contentLength = serializationInfo.GetInt64("contentLength");
			webHeaders = (WebHeaderCollection)serializationInfo.GetValue("webHeaders", typeof(WebHeaderCollection));
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			GetObjectData(serializationInfo, streamingContext);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("responseUri", responseUri, typeof(Uri));
			serializationInfo.AddValue("contentLength", contentLength);
			serializationInfo.AddValue("webHeaders", webHeaders, typeof(WebHeaderCollection));
		}

		public override Stream GetResponseStream()
		{
			CheckDisposed();
			return fileStream;
		}

		~FileWebResponse()
		{
			Dispose(false);
		}

		public override void Close()
		{
			((IDisposable)this).Dispose();
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				if (disposing)
				{
					responseUri = null;
					webHeaders = null;
				}
				FileStream fileStream = this.fileStream;
				this.fileStream = null;
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
		}

		private void CheckDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().FullName);
			}
		}
	}
}
