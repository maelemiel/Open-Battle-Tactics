using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Net
{
	internal class ResponseStream : Stream
	{
		private HttpListenerResponse response;

		private bool ignore_errors;

		private bool disposed;

		private bool trailer_sent;

		private Stream stream;

		private static byte[] crlf = new byte[2] { 13, 10 };

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal ResponseStream(Stream stream, HttpListenerResponse response, bool ignore_errors)
		{
			this.response = response;
			this.ignore_errors = ignore_errors;
			this.stream = stream;
		}

		public override void Close()
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			byte[] array = null;
			MemoryStream headers = GetHeaders(true);
			bool sendChunked = response.SendChunked;
			if (headers != null)
			{
				long position = headers.Position;
				if (sendChunked && !trailer_sent)
				{
					array = GetChunkSizeBytes(0, true);
					headers.Position = headers.Length;
					headers.Write(array, 0, array.Length);
				}
				InternalWrite(headers.GetBuffer(), (int)position, (int)(headers.Length - position));
				trailer_sent = true;
			}
			else if (sendChunked && !trailer_sent)
			{
				array = GetChunkSizeBytes(0, true);
				InternalWrite(array, 0, array.Length);
				trailer_sent = true;
			}
			response.Close();
		}

		private MemoryStream GetHeaders(bool closing)
		{
			if (response.HeadersSent)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			response.SendHeaders(closing, memoryStream);
			return memoryStream;
		}

		public override void Flush()
		{
		}

		private static byte[] GetChunkSizeBytes(int size, bool final)
		{
			string s = string.Format("{0:x}\r\n{1}", size, (!final) ? string.Empty : "\r\n");
			return Encoding.ASCII.GetBytes(s);
		}

		internal void InternalWrite(byte[] buffer, int offset, int count)
		{
			if (ignore_errors)
			{
				try
				{
					stream.Write(buffer, offset, count);
					return;
				}
				catch
				{
					return;
				}
			}
			stream.Write(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			byte[] array = null;
			MemoryStream headers = GetHeaders(false);
			bool sendChunked = response.SendChunked;
			if (headers != null)
			{
				long position = headers.Position;
				headers.Position = headers.Length;
				if (sendChunked)
				{
					array = GetChunkSizeBytes(count, false);
					headers.Write(array, 0, array.Length);
				}
				int num = Math.Min(count, 16384 - (int)headers.Position + (int)position);
				headers.Write(buffer, offset, num);
				count -= num;
				offset += num;
				InternalWrite(headers.GetBuffer(), (int)position, (int)(headers.Length - position));
				headers.SetLength(0L);
				headers.Capacity = 0;
			}
			else if (sendChunked)
			{
				array = GetChunkSizeBytes(count, false);
				InternalWrite(array, 0, array.Length);
			}
			if (count > 0)
			{
				InternalWrite(buffer, offset, count);
			}
			if (sendChunked)
			{
				InternalWrite(crlf, 0, 2);
			}
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			byte[] array = null;
			MemoryStream headers = GetHeaders(false);
			bool sendChunked = response.SendChunked;
			if (headers != null)
			{
				long position = headers.Position;
				headers.Position = headers.Length;
				if (sendChunked)
				{
					array = GetChunkSizeBytes(count, false);
					headers.Write(array, 0, array.Length);
				}
				headers.Write(buffer, offset, count);
				buffer = headers.GetBuffer();
				offset = (int)position;
				count = (int)(headers.Position - position);
			}
			else if (sendChunked)
			{
				array = GetChunkSizeBytes(count, false);
				InternalWrite(array, 0, array.Length);
			}
			return stream.BeginWrite(buffer, offset, count, cback, state);
		}

		public override void EndWrite(IAsyncResult ares)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (ignore_errors)
			{
				try
				{
					stream.EndWrite(ares);
					if (response.SendChunked)
					{
						stream.Write(crlf, 0, 2);
					}
					return;
				}
				catch
				{
					return;
				}
			}
			stream.EndWrite(ares);
			if (response.SendChunked)
			{
				stream.Write(crlf, 0, 2);
			}
		}

		public override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			throw new NotSupportedException();
		}

		public override int EndRead(IAsyncResult ares)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}
}
