namespace System.Net.Sockets
{
	public class UdpClient : IDisposable
	{
		private bool disposed;

		private bool active;

		private Socket socket;

		private AddressFamily family = AddressFamily.InterNetwork;

		private byte[] recvbuffer;

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
				return socket;
			}
			set
			{
				socket = value;
			}
		}

		public int Available
		{
			get
			{
				return socket.Available;
			}
		}

		public bool DontFragment
		{
			get
			{
				return socket.DontFragment;
			}
			set
			{
				socket.DontFragment = value;
			}
		}

		public bool EnableBroadcast
		{
			get
			{
				return socket.EnableBroadcast;
			}
			set
			{
				socket.EnableBroadcast = value;
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				return socket.ExclusiveAddressUse;
			}
			set
			{
				socket.ExclusiveAddressUse = value;
			}
		}

		public bool MulticastLoopback
		{
			get
			{
				return socket.MulticastLoopback;
			}
			set
			{
				socket.MulticastLoopback = value;
			}
		}

		public short Ttl
		{
			get
			{
				return socket.Ttl;
			}
			set
			{
				socket.Ttl = value;
			}
		}

		public UdpClient()
			: this(AddressFamily.InterNetwork)
		{
		}

		public UdpClient(AddressFamily family)
		{
			if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
			}
			this.family = family;
			InitSocket(null);
		}

		public UdpClient(int port)
		{
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			family = AddressFamily.InterNetwork;
			IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
			InitSocket(localEP);
		}

		public UdpClient(IPEndPoint localEP)
		{
			if (localEP == null)
			{
				throw new ArgumentNullException("localEP");
			}
			family = localEP.AddressFamily;
			InitSocket(localEP);
		}

		public UdpClient(int port, AddressFamily family)
		{
			if (family != AddressFamily.InterNetwork && family != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("Family must be InterNetwork or InterNetworkV6", "family");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			this.family = family;
			InitSocket((family != AddressFamily.InterNetwork) ? new IPEndPoint(IPAddress.IPv6Any, port) : new IPEndPoint(IPAddress.Any, port));
		}

		public UdpClient(string hostname, int port)
		{
			if (hostname == null)
			{
				throw new ArgumentNullException("hostname");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			InitSocket(null);
			Connect(hostname, port);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void InitSocket(EndPoint localEP)
		{
			if (socket != null)
			{
				socket.Close();
				socket = null;
			}
			socket = new Socket(family, SocketType.Dgram, ProtocolType.Udp);
			if (localEP != null)
			{
				socket.Bind(localEP);
			}
		}

		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		private void DoConnect(IPEndPoint endPoint)
		{
			try
			{
				socket.Connect(endPoint);
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode == 10013)
				{
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
					socket.Connect(endPoint);
					return;
				}
				throw;
			}
		}

		public void Connect(IPEndPoint endPoint)
		{
			CheckDisposed();
			if (endPoint == null)
			{
				throw new ArgumentNullException("endPoint");
			}
			DoConnect(endPoint);
			active = true;
		}

		public void Connect(IPAddress addr, int port)
		{
			if (addr == null)
			{
				throw new ArgumentNullException("addr");
			}
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			Connect(new IPEndPoint(addr, port));
		}

		public void Connect(string hostname, int port)
		{
			if (port < 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);
			for (int i = 0; i < hostAddresses.Length; i++)
			{
				try
				{
					family = hostAddresses[i].AddressFamily;
					Connect(new IPEndPoint(hostAddresses[i], port));
					break;
				}
				catch (Exception ex)
				{
					if (i == hostAddresses.Length - 1)
					{
						if (socket != null)
						{
							socket.Close();
							socket = null;
						}
						throw ex;
					}
				}
			}
		}

		public void DropMulticastGroup(IPAddress multicastAddr)
		{
			CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (family == AddressFamily.InterNetwork)
			{
				socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(multicastAddr));
			}
			else
			{
				socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, new IPv6MulticastOption(multicastAddr));
			}
		}

		public void DropMulticastGroup(IPAddress multicastAddr, int ifindex)
		{
			CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (family == AddressFamily.InterNetworkV6)
			{
				socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DropMembership, new IPv6MulticastOption(multicastAddr, ifindex));
			}
		}

		public void JoinMulticastGroup(IPAddress multicastAddr)
		{
			CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (family == AddressFamily.InterNetwork)
			{
				socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr));
			}
			else
			{
				socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(multicastAddr));
			}
		}

		public void JoinMulticastGroup(int ifindex, IPAddress multicastAddr)
		{
			CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (family == AddressFamily.InterNetworkV6)
			{
				socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(multicastAddr, ifindex));
				return;
			}
			throw new SocketException(10045);
		}

		public void JoinMulticastGroup(IPAddress multicastAddr, int timeToLive)
		{
			CheckDisposed();
			if (multicastAddr == null)
			{
				throw new ArgumentNullException("multicastAddr");
			}
			if (timeToLive < 0 || timeToLive > 255)
			{
				throw new ArgumentOutOfRangeException("timeToLive");
			}
			JoinMulticastGroup(multicastAddr);
			if (family == AddressFamily.InterNetwork)
			{
				socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, timeToLive);
			}
			else
			{
				socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, timeToLive);
			}
		}

		public void JoinMulticastGroup(IPAddress multicastAddr, IPAddress localAddress)
		{
			CheckDisposed();
			if (family == AddressFamily.InterNetwork)
			{
				socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr, localAddress));
				return;
			}
			throw new SocketException(10045);
		}

		public byte[] Receive(ref IPEndPoint remoteEP)
		{
			CheckDisposed();
			byte[] array = new byte[65536];
			EndPoint remoteEP2 = new IPEndPoint(IPAddress.Any, 0);
			int num = socket.ReceiveFrom(array, ref remoteEP2);
			if (num < array.Length)
			{
				array = CutArray(array, num);
			}
			remoteEP = (IPEndPoint)remoteEP2;
			return array;
		}

		private int DoSend(byte[] dgram, int bytes, IPEndPoint endPoint)
		{
			try
			{
				if (endPoint == null)
				{
					return socket.Send(dgram, 0, bytes, SocketFlags.None);
				}
				return socket.SendTo(dgram, 0, bytes, SocketFlags.None, endPoint);
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode == 10013)
				{
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
					if (endPoint == null)
					{
						return socket.Send(dgram, 0, bytes, SocketFlags.None);
					}
					return socket.SendTo(dgram, 0, bytes, SocketFlags.None, endPoint);
				}
				throw;
			}
		}

		public int Send(byte[] dgram, int bytes)
		{
			CheckDisposed();
			if (dgram == null)
			{
				throw new ArgumentNullException("dgram");
			}
			if (!active)
			{
				throw new InvalidOperationException("Operation not allowed on non-connected sockets.");
			}
			return DoSend(dgram, bytes, null);
		}

		public int Send(byte[] dgram, int bytes, IPEndPoint endPoint)
		{
			CheckDisposed();
			if (dgram == null)
			{
				throw new ArgumentNullException("dgram is null");
			}
			if (active)
			{
				if (endPoint != null)
				{
					throw new InvalidOperationException("Cannot send packets to an arbitrary host while connected.");
				}
				return DoSend(dgram, bytes, null);
			}
			return DoSend(dgram, bytes, endPoint);
		}

		public int Send(byte[] dgram, int bytes, string hostname, int port)
		{
			return Send(dgram, bytes, new IPEndPoint(Dns.GetHostAddresses(hostname)[0], port));
		}

		private byte[] CutArray(byte[] orig, int length)
		{
			byte[] array = new byte[length];
			Buffer.BlockCopy(orig, 0, array, 0, length);
			return array;
		}

		private IAsyncResult DoBeginSend(byte[] datagram, int bytes, IPEndPoint endPoint, AsyncCallback requestCallback, object state)
		{
			try
			{
				if (endPoint == null)
				{
					return socket.BeginSend(datagram, 0, bytes, SocketFlags.None, requestCallback, state);
				}
				return socket.BeginSendTo(datagram, 0, bytes, SocketFlags.None, endPoint, requestCallback, state);
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode == 10013)
				{
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
					if (endPoint == null)
					{
						return socket.BeginSend(datagram, 0, bytes, SocketFlags.None, requestCallback, state);
					}
					return socket.BeginSendTo(datagram, 0, bytes, SocketFlags.None, endPoint, requestCallback, state);
				}
				throw;
			}
		}

		public IAsyncResult BeginSend(byte[] datagram, int bytes, AsyncCallback requestCallback, object state)
		{
			return BeginSend(datagram, bytes, null, requestCallback, state);
		}

		public IAsyncResult BeginSend(byte[] datagram, int bytes, IPEndPoint endPoint, AsyncCallback requestCallback, object state)
		{
			CheckDisposed();
			if (datagram == null)
			{
				throw new ArgumentNullException("datagram");
			}
			return DoBeginSend(datagram, bytes, endPoint, requestCallback, state);
		}

		public IAsyncResult BeginSend(byte[] datagram, int bytes, string hostname, int port, AsyncCallback requestCallback, object state)
		{
			return BeginSend(datagram, bytes, new IPEndPoint(Dns.GetHostAddresses(hostname)[0], port), requestCallback, state);
		}

		public int EndSend(IAsyncResult asyncResult)
		{
			CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult is a null reference");
			}
			return socket.EndSend(asyncResult);
		}

		public IAsyncResult BeginReceive(AsyncCallback callback, object state)
		{
			CheckDisposed();
			recvbuffer = new byte[8192];
			EndPoint remote_end = ((family != AddressFamily.InterNetwork) ? new IPEndPoint(IPAddress.IPv6Any, 0) : new IPEndPoint(IPAddress.Any, 0));
			return socket.BeginReceiveFrom(recvbuffer, 0, 8192, SocketFlags.None, ref remote_end, callback, state);
		}

		public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEP)
		{
			CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult is a null reference");
			}
			EndPoint end_point = ((family != AddressFamily.InterNetwork) ? new IPEndPoint(IPAddress.IPv6Any, 0) : new IPEndPoint(IPAddress.Any, 0));
			int num = socket.EndReceiveFrom(asyncResult, ref end_point);
			remoteEP = (IPEndPoint)end_point;
			byte[] array = new byte[num];
			Array.Copy(recvbuffer, array, num);
			return array;
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
				if (socket != null)
				{
					socket.Close();
				}
				socket = null;
			}
		}

		~UdpClient()
		{
			Dispose(false);
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
