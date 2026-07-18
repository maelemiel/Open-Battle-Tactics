using System.IO;
using System.Runtime.Remoting.Messaging;

namespace System.Net
{
	internal class FtpDataStream : Stream, IDisposable
	{
		private delegate void WriteDelegate(byte[] buffer, int offset, int size);

		private delegate int ReadDelegate(byte[] buffer, int offset, int size);

		private FtpWebRequest request;

		private Stream networkStream;

		private bool disposed;

		private bool isRead;

		private int totalRead;

		public override bool CanRead
		{
			get
			{
				return isRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !isRead;
			}
		}

		public override bool CanSeek
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

		internal Stream NetworkStream
		{
			get
			{
				CheckDisposed();
				return networkStream;
			}
		}

		internal FtpDataStream(FtpWebRequest request, Stream stream, bool isRead)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			this.request = request;
			networkStream = stream;
			this.isRead = isRead;
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override void Close()
		{
			Dispose(true);
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		private int ReadInternal(byte[] buffer, int offset, int size)
		{
			int num = 0;
			request.CheckIfAborted();
			try
			{
				num = networkStream.Read(buffer, offset, size);
			}
			catch (IOException)
			{
				throw new ProtocolViolationException("Server commited a protocol violation");
			}
			totalRead += num;
			if (num == 0)
			{
				networkStream = null;
				request.CloseDataConnection();
				request.SetTransferCompleted();
			}
			return num;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			CheckDisposed();
			if (!isRead)
			{
				throw new NotSupportedException();
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || size > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("offset+size");
			}
			ReadDelegate readDelegate = ReadInternal;
			return readDelegate.BeginInvoke(buffer, offset, size, cb, state);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid asyncResult", "asyncResult");
			}
			ReadDelegate readDelegate = asyncResult2.AsyncDelegate as ReadDelegate;
			if (readDelegate == null)
			{
				throw new ArgumentException("Invalid asyncResult", "asyncResult");
			}
			return readDelegate.EndInvoke(asyncResult);
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			request.CheckIfAborted();
			IAsyncResult asyncResult = BeginRead(buffer, offset, size, null, null);
			if (!asyncResult.IsCompleted && !asyncResult.AsyncWaitHandle.WaitOne(request.ReadWriteTimeout, false))
			{
				throw new WebException("Read timed out.", WebExceptionStatus.Timeout);
			}
			return EndRead(asyncResult);
		}

		private void WriteInternal(byte[] buffer, int offset, int size)
		{
			request.CheckIfAborted();
			try
			{
				networkStream.Write(buffer, offset, size);
			}
			catch (IOException)
			{
				throw new ProtocolViolationException();
			}
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			CheckDisposed();
			if (isRead)
			{
				throw new NotSupportedException();
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || size > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("offset+size");
			}
			WriteDelegate writeDelegate = WriteInternal;
			return writeDelegate.BeginInvoke(buffer, offset, size, cb, state);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid asyncResult.", "asyncResult");
			}
			WriteDelegate writeDelegate = asyncResult2.AsyncDelegate as WriteDelegate;
			if (writeDelegate == null)
			{
				throw new ArgumentException("Invalid asyncResult.", "asyncResult");
			}
			writeDelegate.EndInvoke(asyncResult);
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			request.CheckIfAborted();
			IAsyncResult asyncResult = BeginWrite(buffer, offset, size, null, null);
			if (!asyncResult.IsCompleted && !asyncResult.AsyncWaitHandle.WaitOne(request.ReadWriteTimeout, false))
			{
				throw new WebException("Read timed out.", WebExceptionStatus.Timeout);
			}
			EndWrite(asyncResult);
		}

		~FtpDataStream()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				disposed = true;
				if (networkStream != null)
				{
					request.CloseDataConnection();
					request.SetTransferCompleted();
					request = null;
					networkStream = null;
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
