using System.Collections.Generic;
using System.Threading;

namespace System.Net.Sockets
{
	public class SocketAsyncEventArgs : EventArgs, IDisposable
	{
		private IList<ArraySegment<byte>> _bufferList;

		private Socket curSocket;

		public Socket AcceptSocket { get; set; }

		public byte[] Buffer { get; private set; }

		[System.MonoTODO("not supported in all cases")]
		public IList<ArraySegment<byte>> BufferList
		{
			get
			{
				return _bufferList;
			}
			set
			{
				if (Buffer != null && value != null)
				{
					throw new ArgumentException("Buffer and BufferList properties cannot both be non-null.");
				}
				_bufferList = value;
			}
		}

		public int BytesTransferred { get; private set; }

		public int Count { get; private set; }

		public bool DisconnectReuseSocket { get; set; }

		public SocketAsyncOperation LastOperation { get; private set; }

		public int Offset { get; private set; }

		public EndPoint RemoteEndPoint { get; set; }

		[System.MonoTODO("unused property")]
		public int SendPacketsSendSize { get; set; }

		public SocketError SocketError { get; set; }

		public SocketFlags SocketFlags { get; set; }

		public object UserToken { get; set; }

		public Socket ConnectSocket
		{
			get
			{
				SocketError socketError = SocketError;
				if (socketError == SocketError.AccessDenied)
				{
					return null;
				}
				return curSocket;
			}
		}

		internal bool PolicyRestricted { get; private set; }

		public event EventHandler<SocketAsyncEventArgs> Completed;

		internal SocketAsyncEventArgs(bool policy)
			: this()
		{
			PolicyRestricted = policy;
		}

		public SocketAsyncEventArgs()
		{
			AcceptSocket = null;
			Buffer = null;
			BufferList = null;
			BytesTransferred = 0;
			Count = 0;
			DisconnectReuseSocket = false;
			LastOperation = SocketAsyncOperation.None;
			Offset = 0;
			RemoteEndPoint = null;
			SendPacketsSendSize = -1;
			SocketError = SocketError.Success;
			SocketFlags = SocketFlags.None;
			UserToken = null;
		}

		~SocketAsyncEventArgs()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			Socket acceptSocket = AcceptSocket;
			if (acceptSocket != null)
			{
				acceptSocket.Close();
			}
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void OnCompleted(SocketAsyncEventArgs e)
		{
			if (e != null)
			{
				EventHandler<SocketAsyncEventArgs> completed = e.Completed;
				if (completed != null)
				{
					completed(e.curSocket, e);
				}
			}
		}

		public void SetBuffer(int offset, int count)
		{
			SetBufferInternal(Buffer, offset, count);
		}

		public void SetBuffer(byte[] buffer, int offset, int count)
		{
			SetBufferInternal(buffer, offset, count);
		}

		private void SetBufferInternal(byte[] buffer, int offset, int count)
		{
			if (buffer != null)
			{
				if (BufferList != null)
				{
					throw new ArgumentException("Buffer and BufferList properties cannot both be non-null.");
				}
				int num = buffer.Length;
				if (offset < 0 || (offset != 0 && offset >= num))
				{
					throw new ArgumentOutOfRangeException("offset");
				}
				if (count < 0 || count > num - offset)
				{
					throw new ArgumentOutOfRangeException("count");
				}
				Count = count;
				Offset = offset;
			}
			Buffer = buffer;
		}

		private void ReceiveCallback()
		{
			SocketError = SocketError.Success;
			LastOperation = SocketAsyncOperation.Receive;
			SocketError error = SocketError.Success;
			if (!curSocket.Connected)
			{
				SocketError = SocketError.NotConnected;
				return;
			}
			try
			{
				BytesTransferred = curSocket.Receive_nochecks(Buffer, Offset, Count, SocketFlags, out error);
			}
			finally
			{
				SocketError = error;
				OnCompleted(this);
			}
		}

		private void ConnectCallback()
		{
			LastOperation = SocketAsyncOperation.Connect;
			SocketError socketError = SocketError.AccessDenied;
			try
			{
				socketError = TryConnect(RemoteEndPoint);
			}
			finally
			{
				SocketError = socketError;
				OnCompleted(this);
			}
		}

		private SocketError TryConnect(EndPoint endpoint)
		{
			curSocket.Connected = false;
			SocketError result = SocketError.Success;
			try
			{
				curSocket.seed_endpoint = endpoint;
				curSocket.Connect(endpoint);
				curSocket.Connected = true;
			}
			catch (SocketException ex)
			{
				result = ex.SocketErrorCode;
			}
			return result;
		}

		private void SendCallback()
		{
			SocketError = SocketError.Success;
			LastOperation = SocketAsyncOperation.Send;
			SocketError error = SocketError.Success;
			if (!curSocket.Connected)
			{
				SocketError = SocketError.NotConnected;
				return;
			}
			try
			{
				if (Buffer != null)
				{
					BytesTransferred = curSocket.Send_nochecks(Buffer, Offset, Count, SocketFlags.None, out error);
				}
				else
				{
					if (BufferList == null)
					{
						return;
					}
					BytesTransferred = 0;
					{
						foreach (ArraySegment<byte> buffer in BufferList)
						{
							BytesTransferred += curSocket.Send_nochecks(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, out error);
							if (error != SocketError.Success)
							{
								break;
							}
						}
						return;
					}
				}
			}
			finally
			{
				SocketError = error;
				OnCompleted(this);
			}
		}

		internal void DoOperation(SocketAsyncOperation operation, Socket socket)
		{
			curSocket = socket;
			ThreadStart start;
			switch (operation)
			{
			case SocketAsyncOperation.Receive:
				start = ReceiveCallback;
				break;
			case SocketAsyncOperation.Connect:
				start = ConnectCallback;
				break;
			case SocketAsyncOperation.Send:
				start = SendCallback;
				break;
			default:
				throw new NotSupportedException();
			}
			Thread thread = new Thread(start);
			thread.IsBackground = true;
			thread.Start();
		}
	}
}
