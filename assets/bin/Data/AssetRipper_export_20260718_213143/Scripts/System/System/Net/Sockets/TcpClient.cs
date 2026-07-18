namespace System.Net.Sockets
{
	public class TcpClient : IDisposable
	{
		private enum Properties : uint
		{
			LingerState = 1u,
			NoDelay = 2u,
			ReceiveBufferSize = 4u,
			ReceiveTimeout = 8u,
			SendBufferSize = 0x10u,
			SendTimeout = 0x20u
		}

		private NetworkStream stream;

		private bool active;

		private Socket client;

		private bool disposed;

		private Properties values;

		private int recv_timeout;

		private int send_timeout;

		private int recv_buffer_size;

		private int send_buffer_size;

		private LingerOption linger_state;

		private bool no_delay;

		protected bool Active
		{
			get
			{
				return active;
			}
			set
			{
				active = value;
			}
		}

		public Socket Client
		{
			get
			{
				return client;
			}
			set
			{
				client = value;
				stream = null;
			}
		}

		public int Available
		{
			get
			{
				return client.Available;
			}
		}

		public bool Connected
		{
			get
			{
				return client.Connected;
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				return client.ExclusiveAddressUse;
			}
			set
			{
				client.ExclusiveAddressUse = value;
			}
		}

		public LingerOption LingerState
		{
			get
			{
				if ((values & Properties.LingerState) != 0)
				{
					return linger_state;
				}
				return (LingerOption)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
			}
			set
			{
				if (!client.Connected)
				{
					linger_state = value;
					values |= Properties.LingerState;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
				}
			}
		}

		public bool NoDelay
		{
			get
			{
				if ((values & Properties.NoDelay) != 0)
				{
					return no_delay;
				}
				return (bool)client.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug);
			}
			set
			{
				if (!client.Connected)
				{
					no_delay = value;
					values |= Properties.NoDelay;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, value ? 1 : 0);
				}
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				if ((values & Properties.ReceiveBufferSize) != 0)
				{
					return recv_buffer_size;
				}
				return (int)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
			}
			set
			{
				if (!client.Connected)
				{
					recv_buffer_size = value;
					values |= Properties.ReceiveBufferSize;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
				}
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				if ((values & Properties.ReceiveTimeout) != 0)
				{
					return recv_timeout;
				}
				return (int)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
			}
			set
			{
				if (!client.Connected)
				{
					recv_timeout = value;
					values |= Properties.ReceiveTimeout;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
				}
			}
		}

		public int SendBufferSize
		{
			get
			{
				if ((values & Properties.SendBufferSize) != 0)
				{
					return send_buffer_size;
				}
				return (int)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
			}
			set
			{
				if (!client.Connected)
				{
					send_buffer_size = value;
					values |= Properties.SendBufferSize;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
				}
			}
		}

		public int SendTimeout
		{
			get
			{
				if ((values & Properties.SendTimeout) != 0)
				{
					return send_timeout;
				}
				return (int)client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
			}
			set
			{
				if (!client.Connected)
				{
					send_timeout = value;
					values |= Properties.SendTimeout;
				}
				else
				{
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
				}
			}
		}

		public TcpClient()
		{
			Init(AddressFamily.InterNetwork);
			client.Bind(new IPEndPoint(IPAddress.Any, 0));
		}

		public TcpClient(AddressFamily family)
		{
			if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
			}
			Init(family);
			IPAddress address = IPAddress.Any;
			if (family == AddressFamily.InterNetworkV6)
			{
				address = IPAddress.IPv6Any;
			}
			client.Bind(new IPEndPoint(address, 0));
		}

		public TcpClient(IPEndPoint local_end_point)
		{
			Init(local_end_point.AddressFamily);
			client.Bind(local_end_point);
		}

		public TcpClient(string hostname, int port)
		{
			Connect(hostname, port);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Init(AddressFamily family)
		{
			active = false;
			if (client != null)
			{
				client.Close();
				client = null;
			}
			client = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
		}

		internal void SetTcpClient(Socket s)
		{
			Client = s;
		}

		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		public void Connect(IPEndPoint remote_end_point)
		{
			try
			{
				client.Connect(remote_end_point);
				active = true;
			}
			finally
			{
				CheckDisposed();
			}
		}

		public void Connect(IPAddress address, int port)
		{
			Connect(new IPEndPoint(address, port));
		}

		private void SetOptions()
		{
			Properties properties = values;
			values = (Properties)0u;
			if ((properties & Properties.LingerState) != 0)
			{
				LingerState = linger_state;
			}
			if ((properties & Properties.NoDelay) != 0)
			{
				NoDelay = no_delay;
			}
			if ((properties & Properties.ReceiveBufferSize) != 0)
			{
				ReceiveBufferSize = recv_buffer_size;
			}
			if ((properties & Properties.ReceiveTimeout) != 0)
			{
				ReceiveTimeout = recv_timeout;
			}
			if ((properties & Properties.SendBufferSize) != 0)
			{
				SendBufferSize = send_buffer_size;
			}
			if ((properties & Properties.SendTimeout) != 0)
			{
				SendTimeout = send_timeout;
			}
		}

		public void Connect(string hostname, int port)
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
			Connect(hostAddresses, port);
		}

		public void Connect(IPAddress[] ipAddresses, int port)
		{
			CheckDisposed();
			if (ipAddresses == null)
			{
				throw new ArgumentNullException("ipAddresses");
			}
			for (int i = 0; i < ipAddresses.Length; i++)
			{
				try
				{
					IPAddress iPAddress = ipAddresses[i];
					if (iPAddress.Equals(IPAddress.Any) || iPAddress.Equals(IPAddress.IPv6Any))
					{
						throw new SocketException(10049);
					}
					Init(iPAddress.AddressFamily);
					if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						client.Bind(new IPEndPoint(IPAddress.Any, 0));
					}
					else
					{
						if (iPAddress.AddressFamily != AddressFamily.InterNetworkV6)
						{
							throw new NotSupportedException("This method is only valid for sockets in the InterNetwork and InterNetworkV6 families");
						}
						client.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
					}
					Connect(new IPEndPoint(iPAddress, port));
					if (values != 0)
					{
						SetOptions();
					}
					break;
				}
				catch (Exception ex)
				{
					Init(AddressFamily.InterNetwork);
					if (i == ipAddresses.Length - 1)
					{
						throw ex;
					}
				}
			}
		}

		public void EndConnect(IAsyncResult asyncResult)
		{
			client.EndConnect(asyncResult);
		}

		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback callback, object state)
		{
			return client.BeginConnect(address, port, callback, state);
		}

		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback callback, object state)
		{
			return client.BeginConnect(addresses, port, callback, state);
		}

		public IAsyncResult BeginConnect(string host, int port, AsyncCallback callback, object state)
		{
			return client.BeginConnect(host, port, callback, state);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			if (disposing)
			{
				NetworkStream networkStream = stream;
				stream = null;
				if (networkStream != null)
				{
					networkStream.Close();
					active = false;
					networkStream = null;
				}
				else if (client != null)
				{
					client.Close();
					client = null;
				}
			}
		}

		~TcpClient()
		{
			Dispose(false);
		}

		public NetworkStream GetStream()
		{
			try
			{
				if (stream == null)
				{
					stream = new NetworkStream(client, true);
				}
				return stream;
			}
			finally
			{
				CheckDisposed();
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
