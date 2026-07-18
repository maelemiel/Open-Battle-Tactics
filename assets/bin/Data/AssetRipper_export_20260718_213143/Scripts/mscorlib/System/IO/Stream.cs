using System.Runtime.InteropServices;
using System.Threading;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public abstract class Stream : IDisposable
	{
		public static readonly Stream Null = new NullStream();

		public abstract bool CanRead { get; }

		public abstract bool CanSeek { get; }

		public abstract bool CanWrite { get; }

		[ComVisible(false)]
		public virtual bool CanTimeout
		{
			get
			{
				return false;
			}
		}

		public abstract long Length { get; }

		public abstract long Position { get; set; }

		[ComVisible(false)]
		public virtual int ReadTimeout
		{
			get
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
			set
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
		}

		[ComVisible(false)]
		public virtual int WriteTimeout
		{
			get
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
			set
			{
				throw new InvalidOperationException("Timeouts are not supported on this stream.");
			}
		}

		public void Dispose()
		{
			Close();
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public virtual void Close()
		{
			Dispose(true);
		}

		public static Stream Synchronized(Stream stream)
		{
			throw new NotImplementedException();
		}

		[Obsolete("CreateWaitHandle is due for removal.  Use \"new ManualResetEvent(false)\" instead.")]
		protected virtual WaitHandle CreateWaitHandle()
		{
			return new ManualResetEvent(false);
		}

		public abstract void Flush();

		public abstract int Read([In][Out] byte[] buffer, int offset, int count);

		public virtual int ReadByte()
		{
			byte[] array = new byte[1];
			if (Read(array, 0, 1) == 1)
			{
				return array[0];
			}
			return -1;
		}

		public abstract long Seek(long offset, SeekOrigin origin);

		public abstract void SetLength(long value);

		public abstract void Write(byte[] buffer, int offset, int count);

		public virtual void WriteByte(byte value)
		{
			Write(new byte[1] { value }, 0, 1);
		}

		public virtual IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (!CanRead)
			{
				throw new NotSupportedException("This stream does not support reading");
			}
			StreamAsyncResult streamAsyncResult = new StreamAsyncResult(state);
			try
			{
				int nbytes = Read(buffer, offset, count);
				streamAsyncResult.SetComplete(null, nbytes);
			}
			catch (Exception e)
			{
				streamAsyncResult.SetComplete(e, 0);
			}
			if (callback != null)
			{
				callback(streamAsyncResult);
			}
			return streamAsyncResult;
		}

		public virtual IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			if (!CanWrite)
			{
				throw new NotSupportedException("This stream does not support writing");
			}
			StreamAsyncResult streamAsyncResult = new StreamAsyncResult(state);
			try
			{
				Write(buffer, offset, count);
				streamAsyncResult.SetComplete(null);
			}
			catch (Exception complete)
			{
				streamAsyncResult.SetComplete(complete);
			}
			if (callback != null)
			{
				callback.BeginInvoke(streamAsyncResult, null, null);
			}
			return streamAsyncResult;
		}

		public virtual int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			StreamAsyncResult streamAsyncResult = asyncResult as StreamAsyncResult;
			if (streamAsyncResult == null || streamAsyncResult.NBytes == -1)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (streamAsyncResult.Done)
			{
				throw new InvalidOperationException("EndRead already called.");
			}
			streamAsyncResult.Done = true;
			if (streamAsyncResult.Exception != null)
			{
				throw streamAsyncResult.Exception;
			}
			return streamAsyncResult.NBytes;
		}

		public virtual void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			StreamAsyncResult streamAsyncResult = asyncResult as StreamAsyncResult;
			if (streamAsyncResult == null || streamAsyncResult.NBytes != -1)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (streamAsyncResult.Done)
			{
				throw new InvalidOperationException("EndWrite already called.");
			}
			streamAsyncResult.Done = true;
			if (streamAsyncResult.Exception != null)
			{
				throw streamAsyncResult.Exception;
			}
		}
	}
}
