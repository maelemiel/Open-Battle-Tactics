namespace System.Net.Sockets
{
	public class TcpListener
	{
		private bool active;

		private Socket server;

		private EndPoint savedEP;

		protected bool Active
		{
			get
			{
				return active;
			}
		}

		public EndPoint LocalEndpoint
		{
			get
			{
				if (active)
				{
					return server.LocalEndPoint;
				}
				return savedEP;
			}
		}

		public Socket Server
		{
			get
			{
				return server;
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				if (server == null)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (active)
				{
					throw new InvalidOperationException("The TcpListener has been started");
				}
				return server.ExclusiveAddressUse;
			}
			set
			{
				if (server == null)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (active)
				{
					throw new InvalidOperationException("The TcpListener has been started");
				}
				server.ExclusiveAddressUse = value;
			}
		}

		[Obsolete("Use TcpListener (IPAddress address, int port) instead")]
		public TcpListener(int port)
		{
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			Init(AddressFamily.InterNetwork, new IPEndPoint(IPAddress.Any, port));
		}

		public TcpListener(IPEndPoint local_end_point)
		{
			if (local_end_point == null)
			{
				throw new ArgumentNullException("local_end_point");
			}
			Init(local_end_point.AddressFamily, local_end_point);
		}

		public TcpListener(IPAddress listen_ip, int port)
		{
			if (listen_ip == null)
			{
				throw new ArgumentNullException("listen_ip");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			Init(listen_ip.AddressFamily, new IPEndPoint(listen_ip, port));
		}

		private void Init(AddressFamily family, EndPoint ep)
		{
			active = false;
			server = new Socket(family, SocketType.Stream, ProtocolType.Tcp);
			savedEP = ep;
		}

		public Socket AcceptSocket()
		{
			if (!active)
			{
				throw new InvalidOperationException("Socket is not listening");
			}
			return server.Accept();
		}

		public TcpClient AcceptTcpClient()
		{
			if (!active)
			{
				throw new InvalidOperationException("Socket is not listening");
			}
			Socket tcpClient = server.Accept();
			TcpClient tcpClient2 = new TcpClient();
			tcpClient2.SetTcpClient(tcpClient);
			return tcpClient2;
		}

		~TcpListener()
		{
			if (active)
			{
				Stop();
			}
		}

		public bool Pending()
		{
			if (!active)
			{
				throw new InvalidOperationException("Socket is not listening");
			}
			return server.Poll(0, SelectMode.SelectRead);
		}

		public void Start()
		{
			Start(5);
		}

		public void Start(int backlog)
		{
			if (!active)
			{
				if (server == null)
				{
					throw new InvalidOperationException("Invalid server socket");
				}
				server.Bind(savedEP);
				server.Listen(backlog);
				active = true;
			}
		}

		public IAsyncResult BeginAcceptSocket(AsyncCallback callback, object state)
		{
			if (server == null)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			return server.BeginAccept(callback, state);
		}

		public IAsyncResult BeginAcceptTcpClient(AsyncCallback callback, object state)
		{
			if (server == null)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			return server.BeginAccept(callback, state);
		}

		public Socket EndAcceptSocket(IAsyncResult asyncResult)
		{
			return server.EndAccept(asyncResult);
		}

		public TcpClient EndAcceptTcpClient(IAsyncResult asyncResult)
		{
			Socket tcpClient = server.EndAccept(asyncResult);
			TcpClient tcpClient2 = new TcpClient();
			tcpClient2.SetTcpClient(tcpClient);
			return tcpClient2;
		}

		public void Stop()
		{
			if (active)
			{
				server.Close();
				server = null;
			}
			Init(AddressFamily.InterNetwork, savedEP);
		}
	}
}
