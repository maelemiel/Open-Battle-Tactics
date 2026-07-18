using System.IO;
using System.Runtime.InteropServices;

namespace System.Net
{
	internal class RequestStream : Stream
	{
		private byte[] buffer;

		private int offset;

		private int length;

		private long remaining_body;

		private bool disposed;

		private Stream stream;

		public override bool CanRead
		{
			get
			{
				return true;
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
				return false;
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

		internal RequestStream(Stream stream, byte[] buffer, int offset, int length)
			: this(stream, buffer, offset, length, -1L)
		{
		}

		internal RequestStream(Stream stream, byte[] buffer, int offset, int length, long contentlength)
		{
			this.stream = stream;
			this.buffer = buffer;
			this.offset = offset;
			this.length = length;
			remaining_body = contentlength;
		}

		public override void Close()
		{
			disposed = true;
		}

		public override void Flush()
		{
		}

		private int FillFromBuffer(byte[] buffer, int off, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (off < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			int num = buffer.Length;
			if (off > num)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (off > num - count)
			{
				throw new ArgumentException("Reading would overrun buffer");
			}
			if (remaining_body == 0L)
			{
				return -1;
			}
			if (length == 0)
			{
				return 0;
			}
			int num2 = Math.Min(length, count);
			if (remaining_body > 0)
			{
				num2 = (int)Math.Min(num2, remaining_body);
			}
			if (offset > this.buffer.Length - num2)
			{
				num2 = Math.Min(num2, this.buffer.Length - offset);
			}
			if (num2 == 0)
			{
				return 0;
			}
			Buffer.BlockCopy(this.buffer, offset, buffer, off, num2);
			offset += num2;
			length -= num2;
			if (remaining_body > 0)
			{
				remaining_body -= num2;
			}
			return num2;
		}

		public override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(typeof(System.Net.RequestStream).ToString());
			}
			int num = FillFromBuffer(buffer, offset, count);
			if (num == -1)
			{
				return 0;
			}
			if (num > 0)
			{
				return num;
			}
			num = stream.Read(buffer, offset, count);
			if (num > 0 && remaining_body > 0)
			{
				remaining_body -= num;
			}
			return num;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(typeof(System.Net.RequestStream).ToString());
			}
			int num = FillFromBuffer(buffer, offset, count);
			if (num > 0 || num == -1)
			{
				System.Net.HttpStreamAsyncResult httpStreamAsyncResult = new System.Net.HttpStreamAsyncResult();
				httpStreamAsyncResult.Buffer = buffer;
				httpStreamAsyncResult.Offset = offset;
				httpStreamAsyncResult.Count = count;
				httpStreamAsyncResult.Callback = cback;
				httpStreamAsyncResult.State = state;
				httpStreamAsyncResult.SynchRead = num;
				httpStreamAsyncResult.Complete();
				return httpStreamAsyncResult;
			}
			if (remaining_body >= 0 && count > remaining_body)
			{
				count = (int)Math.Min(2147483647L, remaining_body);
			}
			return stream.BeginRead(buffer, offset, count, cback, state);
		}

		public override int EndRead(IAsyncResult ares)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(typeof(System.Net.RequestStream).ToString());
			}
			if (ares == null)
			{
				throw new ArgumentNullException("async_result");
			}
			if (ares is System.Net.HttpStreamAsyncResult)
			{
				System.Net.HttpStreamAsyncResult httpStreamAsyncResult = (System.Net.HttpStreamAsyncResult)ares;
				if (!ares.IsCompleted)
				{
					ares.AsyncWaitHandle.WaitOne();
				}
				return httpStreamAsyncResult.SynchRead;
			}
			int num = stream.EndRead(ares);
			if (remaining_body > 0 && num > 0)
			{
				remaining_body -= num;
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback cback, object state)
		{
			throw new NotSupportedException();
		}

		public override void EndWrite(IAsyncResult async_result)
		{
			throw new NotSupportedException();
		}
	}
}
