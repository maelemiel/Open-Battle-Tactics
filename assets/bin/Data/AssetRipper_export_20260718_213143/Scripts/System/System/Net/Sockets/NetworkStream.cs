using System.IO;
using System.Runtime.InteropServices;

namespace System.Net.Sockets
{
	public class NetworkStream : Stream, IDisposable
	{
		public static class Timeout
		{
			public static readonly float Infinite = -1f;
		}

		private FileAccess access;

		private Socket socket;

		private bool owns_socket;

		private bool readable;

		private bool writeable;

		private bool disposed;

		public override bool CanRead
		{
			get
			{
				return access == FileAccess.ReadWrite || access == FileAccess.Read;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return access == FileAccess.ReadWrite || access == FileAccess.Write;
			}
		}

		public virtual bool DataAvailable
		{
			get
			{
				CheckDisposed();
				return socket.Available > 0;
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

		protected bool Readable
		{
			get
			{
				return readable;
			}
			set
			{
				readable = value;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return socket.ReceiveTimeout;
			}
			set
			{
				if (value <= 0 && (float)value != Timeout.Infinite)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified is less than or equal to zero and is not Infinite.");
				}
				socket.ReceiveTimeout = value;
			}
		}

		protected Socket Socket
		{
			get
			{
				return socket;
			}
		}

		protected bool Writeable
		{
			get
			{
				return writeable;
			}
			set
			{
				writeable = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return socket.SendTimeout;
			}
			set
			{
				if (value <= 0 && (float)value != Timeout.Infinite)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified is less than or equal to zero and is not Infinite");
				}
				socket.SendTimeout = value;
			}
		}

		public NetworkStream(Socket socket)
			: this(socket, FileAccess.ReadWrite, false)
		{
		}

		public NetworkStream(Socket socket, bool owns_socket)
			: this(socket, FileAccess.ReadWrite, owns_socket)
		{
		}

		public NetworkStream(Socket socket, FileAccess access)
			: this(socket, access, false)
		{
		}

		public NetworkStream(Socket socket, FileAccess access, bool owns_socket)
		{
			if (socket == null)
			{
				throw new ArgumentNullException("socket is null");
			}
			if (socket.SocketType != SocketType.Stream)
			{
				throw new ArgumentException("Socket is not of type Stream", "socket");
			}
			if (!socket.Connected)
			{
				throw new IOException("Not connected");
			}
			if (!socket.Blocking)
			{
				throw new IOException("Operation not allowed on a non-blocking socket.");
			}
			this.socket = socket;
			this.owns_socket = owns_socket;
			this.access = access;
			readable = CanRead;
			writeable = CanWrite;
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is null");
			}
			int num = buffer.Length;
			if (offset < 0 || offset > num)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
			}
			if (size < 0 || offset + size > num)
			{
				throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
			}
			try
			{
				return socket.BeginReceive(buffer, offset, size, SocketFlags.None, callback, state);
			}
			catch (Exception innerException)
			{
				throw new IOException("BeginReceive failure", innerException);
			}
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is null");
			}
			int num = buffer.Length;
			if (offset < 0 || offset > num)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
			}
			if (size < 0 || offset + size > num)
			{
				throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
			}
			try
			{
				return socket.BeginSend(buffer, offset, size, SocketFlags.None, callback, state);
			}
			catch
			{
				throw new IOException("BeginWrite failure");
			}
		}

		~NetworkStream()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			if (owns_socket)
			{
				Socket socket = this.socket;
				if (socket != null)
				{
					socket.Close();
				}
			}
			this.socket = null;
			access = (FileAccess)0;
		}

		public override int EndRead(IAsyncResult ar)
		{
			CheckDisposed();
			if (ar == null)
			{
				throw new ArgumentNullException("async result is null");
			}
			try
			{
				return socket.EndReceive(ar);
			}
			catch (Exception innerException)
			{
				throw new IOException("EndRead failure", innerException);
			}
		}

		public override void EndWrite(IAsyncResult ar)
		{
			CheckDisposed();
			if (ar == null)
			{
				throw new ArgumentNullException("async result is null");
			}
			try
			{
				socket.EndSend(ar);
			}
			catch (Exception innerException)
			{
				throw new IOException("EndWrite failure", innerException);
			}
		}

		public override void Flush()
		{
		}

		public override int Read([In][Out] byte[] buffer, int offset, int size)
		{
			CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is null");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
			}
			try
			{
				return socket.Receive(buffer, offset, size, SocketFlags.None);
			}
			catch (Exception innerException)
			{
				throw new IOException("Read failure", innerException);
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			CheckDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset exceeds the size of buffer");
			}
			if (size < 0 || size > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("offset+size exceeds the size of buffer");
			}
			try
			{
				for (int i = 0; size - i > 0; i += socket.Send(buffer, offset + i, size - i, SocketFlags.None))
				{
				}
			}
			catch (Exception innerException)
			{
				throw new IOException("Write failure", innerException);
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
