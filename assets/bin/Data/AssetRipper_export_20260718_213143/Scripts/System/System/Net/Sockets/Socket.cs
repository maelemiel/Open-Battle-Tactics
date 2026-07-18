using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System.Net.Sockets
{
	public class Socket : IDisposable
	{
		private enum SocketOperation
		{
			Accept = 0,
			Connect = 1,
			Receive = 2,
			ReceiveFrom = 3,
			Send = 4,
			SendTo = 5,
			UsedInManaged1 = 6,
			UsedInManaged2 = 7,
			UsedInProcess = 8,
			UsedInConsole2 = 9,
			Disconnect = 10,
			AcceptReceive = 11,
			ReceiveGeneric = 12,
			SendGeneric = 13
		}

		private struct WSABUF
		{
			public int len;

			public IntPtr buf;
		}

		[StructLayout(LayoutKind.Sequential)]
		private sealed class SocketAsyncResult : IAsyncResult
		{
			public Socket Sock;

			public IntPtr handle;

			private object state;

			private AsyncCallback callback;

			private WaitHandle waithandle;

			private Exception delayedException;

			public EndPoint EndPoint;

			public byte[] Buffer;

			public int Offset;

			public int Size;

			public SocketFlags SockFlags;

			public Socket AcceptSocket;

			public IPAddress[] Addresses;

			public int Port;

			public IList<ArraySegment<byte>> Buffers;

			public bool ReuseSocket;

			private Socket acc_socket;

			private int total;

			private bool completed_sync;

			private bool completed;

			public bool blocking;

			internal int error;

			private SocketOperation operation;

			public object ares;

			public int EndCalled;

			public object AsyncState
			{
				get
				{
					return state;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					lock (this)
					{
						if (waithandle == null)
						{
							waithandle = new ManualResetEvent(completed);
						}
					}
					return waithandle;
				}
				set
				{
					waithandle = value;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return completed_sync;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return completed;
				}
				set
				{
					completed = value;
					lock (this)
					{
						if (waithandle != null && value)
						{
							((ManualResetEvent)waithandle).Set();
						}
					}
				}
			}

			public Socket Socket
			{
				get
				{
					return acc_socket;
				}
			}

			public int Total
			{
				get
				{
					return total;
				}
				set
				{
					total = value;
				}
			}

			public SocketError ErrorCode
			{
				get
				{
					SocketException ex = delayedException as SocketException;
					if (ex != null)
					{
						return ex.SocketErrorCode;
					}
					if (error != 0)
					{
						return (SocketError)error;
					}
					return SocketError.Success;
				}
			}

			public SocketAsyncResult(Socket sock, object state, AsyncCallback callback, SocketOperation operation)
			{
				Sock = sock;
				blocking = sock.blocking;
				handle = sock.socket;
				this.state = state;
				this.callback = callback;
				this.operation = operation;
				SockFlags = SocketFlags.None;
			}

			public void CheckIfThrowDelayedException()
			{
				if (delayedException != null)
				{
					Sock.connected = false;
					throw delayedException;
				}
				if (error != 0)
				{
					Sock.connected = false;
					throw new SocketException(error);
				}
			}

			private void CompleteAllOnDispose(Queue queue)
			{
				object[] array = queue.ToArray();
				queue.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					SocketAsyncResult socketAsyncResult = (SocketAsyncResult)array[i];
					WaitCallback callBack = socketAsyncResult.CompleteDisposed;
					ThreadPool.QueueUserWorkItem(callBack, null);
				}
				if (array.Length == 0)
				{
					Buffer = null;
				}
			}

			private void CompleteDisposed(object unused)
			{
				Complete();
			}

			public void Complete()
			{
				if (operation != SocketOperation.Receive && Sock.disposed)
				{
					delayedException = new ObjectDisposedException(Sock.GetType().ToString());
				}
				IsCompleted = true;
				Queue queue = null;
				if (operation == SocketOperation.Receive || operation == SocketOperation.ReceiveFrom || operation == SocketOperation.ReceiveGeneric)
				{
					queue = Sock.readQ;
				}
				else if (operation == SocketOperation.Send || operation == SocketOperation.SendTo || operation == SocketOperation.SendGeneric)
				{
					queue = Sock.writeQ;
				}
				if (queue != null)
				{
					SocketAsyncCall socketAsyncCall = null;
					SocketAsyncResult socketAsyncResult = null;
					lock (queue)
					{
						queue.Dequeue();
						if (queue.Count > 0)
						{
							socketAsyncResult = (SocketAsyncResult)queue.Peek();
							if (!Sock.disposed)
							{
								Worker worker = new Worker(socketAsyncResult);
								socketAsyncCall = GetDelegate(worker, socketAsyncResult.operation);
							}
							else
							{
								CompleteAllOnDispose(queue);
							}
						}
					}
					if (socketAsyncCall != null)
					{
						socketAsyncCall.BeginInvoke(null, socketAsyncResult);
					}
				}
				if (callback != null)
				{
					callback(this);
				}
				Buffer = null;
			}

			private SocketAsyncCall GetDelegate(Worker worker, SocketOperation op)
			{
				switch (op)
				{
				case SocketOperation.Receive:
					return worker.Receive;
				case SocketOperation.ReceiveFrom:
					return worker.ReceiveFrom;
				case SocketOperation.Send:
					return worker.Send;
				case SocketOperation.SendTo:
					return worker.SendTo;
				default:
					return null;
				}
			}

			public void Complete(bool synch)
			{
				completed_sync = synch;
				Complete();
			}

			public void Complete(int total)
			{
				this.total = total;
				Complete();
			}

			public void Complete(Exception e, bool synch)
			{
				completed_sync = synch;
				delayedException = e;
				Complete();
			}

			public void Complete(Exception e)
			{
				delayedException = e;
				Complete();
			}

			public void Complete(Socket s)
			{
				acc_socket = s;
				Complete();
			}

			public void Complete(Socket s, int total)
			{
				acc_socket = s;
				this.total = total;
				Complete();
			}
		}

		private sealed class Worker
		{
			private SocketAsyncResult result;

			private bool requireSocketSecurity;

			private int send_so_far;

			public Worker(SocketAsyncResult ares)
				: this(ares, true)
			{
			}

			public Worker(SocketAsyncResult ares, bool requireSocketSecurity)
			{
				result = ares;
				this.requireSocketSecurity = requireSocketSecurity;
			}

			public void Accept()
			{
				Socket socket = null;
				try
				{
					socket = result.Sock.Accept();
				}
				catch (Exception e)
				{
					result.Complete(e);
					return;
				}
				result.Complete(socket);
			}

			public void AcceptReceive()
			{
				Socket socket = null;
				try
				{
					if (result.AcceptSocket == null)
					{
						socket = result.Sock.Accept();
					}
					else
					{
						socket = result.AcceptSocket;
						result.Sock.Accept(socket);
					}
				}
				catch (Exception e)
				{
					result.Complete(e);
					return;
				}
				int total = 0;
				if (result.Size > 0)
				{
					try
					{
						SocketError error;
						total = socket.Receive_nochecks(result.Buffer, result.Offset, result.Size, result.SockFlags, out error);
					}
					catch (Exception e2)
					{
						result.Complete(e2);
						return;
					}
				}
				result.Complete(socket, total);
			}

			public void Connect()
			{
				if (result.EndPoint != null)
				{
					try
					{
						if (!result.Sock.Blocking)
						{
							int socket_error;
							result.Sock.Poll(-1, SelectMode.SelectWrite, out socket_error);
							if (socket_error != 0)
							{
								result.Complete(new SocketException(socket_error));
								return;
							}
							result.Sock.connected = true;
						}
						else
						{
							result.Sock.seed_endpoint = result.EndPoint;
							result.Sock.Connect(result.EndPoint, requireSocketSecurity);
							result.Sock.connected = true;
						}
					}
					catch (Exception e)
					{
						result.Complete(e);
						return;
					}
					result.Complete();
				}
				else if (result.Addresses != null)
				{
					int error = 10036;
					IPAddress[] addresses = result.Addresses;
					foreach (IPAddress address in addresses)
					{
						IPEndPoint iPEndPoint = new IPEndPoint(address, result.Port);
						SocketAddress sa = iPEndPoint.Serialize();
						try
						{
							Connect_internal(result.Sock.socket, sa, out error, requireSocketSecurity);
						}
						catch (Exception e2)
						{
							result.Complete(e2);
							return;
						}
						switch (error)
						{
						case 0:
							result.Sock.connected = true;
							result.Sock.seed_endpoint = iPEndPoint;
							result.Complete();
							return;
						case 10035:
						case 10036:
							if (!result.Sock.Blocking)
							{
								int socket_error2;
								result.Sock.Poll(-1, SelectMode.SelectWrite, out socket_error2);
								if (socket_error2 == 0)
								{
									result.Sock.connected = true;
									result.Sock.seed_endpoint = iPEndPoint;
									result.Complete();
									return;
								}
							}
							break;
						}
					}
					result.Complete(new SocketException(error));
				}
				else
				{
					result.Complete(new SocketException(10049));
				}
			}

			public void Disconnect()
			{
				try
				{
					result.Sock.Disconnect(result.ReuseSocket);
				}
				catch (Exception e)
				{
					result.Complete(e);
					return;
				}
				result.Complete();
			}

			public void Receive()
			{
				result.Complete();
			}

			public void ReceiveFrom()
			{
				int num = 0;
				try
				{
					num = result.Sock.ReceiveFrom_nochecks(result.Buffer, result.Offset, result.Size, result.SockFlags, ref result.EndPoint);
				}
				catch (Exception e)
				{
					result.Complete(e);
					return;
				}
				result.Complete(num);
			}

			public void ReceiveGeneric()
			{
				int num = 0;
				try
				{
					SocketError errorCode;
					num = result.Sock.Receive(result.Buffers, result.SockFlags, out errorCode);
				}
				catch (Exception e)
				{
					result.Complete(e);
					return;
				}
				result.Complete(num);
			}

			private void UpdateSendValues(int last_sent)
			{
				if (result.error == 0)
				{
					send_so_far += last_sent;
					result.Offset += last_sent;
					result.Size -= last_sent;
				}
			}

			public void Send()
			{
				if (result.error == 0)
				{
					UpdateSendValues(result.Total);
					if (result.Sock.disposed)
					{
						result.Complete();
						return;
					}
					if (result.Size > 0)
					{
						SocketAsyncCall socketAsyncCall = Send;
						socketAsyncCall.BeginInvoke(null, result);
						return;
					}
					result.Total = send_so_far;
				}
				result.Complete();
			}

			public void SendTo()
			{
				int num = 0;
				try
				{
					num = result.Sock.SendTo_nochecks(result.Buffer, result.Offset, result.Size, result.SockFlags, result.EndPoint);
					UpdateSendValues(num);
					if (result.Size > 0)
					{
						SocketAsyncCall socketAsyncCall = SendTo;
						socketAsyncCall.BeginInvoke(null, result);
						return;
					}
					result.Total = send_so_far;
				}
				catch (Exception e)
				{
					result.Complete(e);
					return;
				}
				result.Complete();
			}

			public void SendGeneric()
			{
				int num = 0;
				try
				{
					SocketError errorCode;
					num = result.Sock.Send(result.Buffers, result.SockFlags, out errorCode);
				}
				catch (Exception e)
				{
					result.Complete(e);
					return;
				}
				result.Complete(num);
			}
		}

		private sealed class SendFileAsyncResult : IAsyncResult
		{
			private IAsyncResult ares;

			private SendFileHandler d;

			public object AsyncState
			{
				get
				{
					return ares.AsyncState;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					return ares.AsyncWaitHandle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return ares.CompletedSynchronously;
				}
			}

			public bool IsCompleted
			{
				get
				{
					return ares.IsCompleted;
				}
			}

			public SendFileHandler Delegate
			{
				get
				{
					return d;
				}
			}

			public IAsyncResult Original
			{
				get
				{
					return ares;
				}
			}

			public SendFileAsyncResult(SendFileHandler d, IAsyncResult ares)
			{
				this.d = d;
				this.ares = ares;
			}
		}

		private delegate void SocketAsyncCall();

		private delegate void SendFileHandler(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags);

		private Queue readQ = new Queue(2);

		private Queue writeQ = new Queue(2);

		private bool islistening;

		private bool useoverlappedIO;

		private readonly int MinListenPort = 7100;

		private readonly int MaxListenPort = 7150;

		private static int ipv4Supported;

		private static int ipv6Supported;

		private int linger_timeout;

		private IntPtr socket;

		private AddressFamily address_family;

		private SocketType socket_type;

		private ProtocolType protocol_type;

		internal bool blocking = true;

		private Thread blocking_thread;

		private bool isbound;

		private static int current_bind_count;

		private readonly int max_bind_count = 50;

		private bool connected;

		private bool closed;

		internal bool disposed;

		internal EndPoint seed_endpoint;

		private static MethodInfo check_socket_policy;

		public int Available
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				int error;
				int result = Available_internal(socket, out error);
				if (error != 0)
				{
					throw new SocketException(error);
				}
				return result;
			}
		}

		public bool DontFragment
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (address_family == AddressFamily.InterNetwork)
				{
					return (int)GetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment) != 0;
				}
				if (address_family == AddressFamily.InterNetworkV6)
				{
					return (int)GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DontFragment) != 0;
				}
				throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (address_family == AddressFamily.InterNetwork)
				{
					SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DontFragment, value ? 1 : 0);
					return;
				}
				if (address_family == AddressFamily.InterNetworkV6)
				{
					SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.DontFragment, value ? 1 : 0);
					return;
				}
				throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
			}
		}

		public bool EnableBroadcast
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (protocol_type != ProtocolType.Udp)
				{
					throw new SocketException(10042);
				}
				return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast) != 0;
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (protocol_type != ProtocolType.Udp)
				{
					throw new SocketException(10042);
				}
				SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, value ? 1 : 0);
			}
		}

		public bool ExclusiveAddressUse
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse) != 0;
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (isbound)
				{
					throw new InvalidOperationException("Bind has already been called for this socket");
				}
				SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, value ? 1 : 0);
			}
		}

		public bool IsBound
		{
			get
			{
				return isbound;
			}
		}

		public LingerOption LingerState
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				return (LingerOption)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger);
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, value);
			}
		}

		public bool MulticastLoopback
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (protocol_type == ProtocolType.Tcp)
				{
					throw new SocketException(10042);
				}
				if (address_family == AddressFamily.InterNetwork)
				{
					return (int)GetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback) != 0;
				}
				if (address_family == AddressFamily.InterNetworkV6)
				{
					return (int)GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback) != 0;
				}
				throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (protocol_type == ProtocolType.Tcp)
				{
					throw new SocketException(10042);
				}
				if (address_family == AddressFamily.InterNetwork)
				{
					SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, value ? 1 : 0);
					return;
				}
				if (address_family == AddressFamily.InterNetworkV6)
				{
					SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastLoopback, value ? 1 : 0);
					return;
				}
				throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
			}
		}

		[System.MonoTODO("This doesn't do anything on Mono yet")]
		public bool UseOnlyOverlappedIO
		{
			get
			{
				return useoverlappedIO;
			}
			set
			{
				useoverlappedIO = value;
			}
		}

		public IntPtr Handle
		{
			get
			{
				return socket;
			}
		}

		public EndPoint LocalEndPoint
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (seed_endpoint == null)
				{
					return null;
				}
				int error;
				SocketAddress address = LocalEndPoint_internal(socket, out error);
				if (error != 0)
				{
					throw new SocketException(error);
				}
				return seed_endpoint.Create(address);
			}
		}

		public SocketType SocketType
		{
			get
			{
				return socket_type;
			}
		}

		public int SendTimeout
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout);
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified for a set operation is less than -1");
				}
				if (value == -1)
				{
					value = 0;
				}
				SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);
			}
		}

		public int ReceiveTimeout
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified for a set operation is less than -1");
				}
				if (value == -1)
				{
					value = 0;
				}
				SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
			}
		}

		public static bool SupportsIPv4
		{
			get
			{
				CheckProtocolSupport();
				return ipv4Supported == 1;
			}
		}

		[Obsolete("Use OSSupportsIPv6 instead")]
		public static bool SupportsIPv6
		{
			get
			{
				CheckProtocolSupport();
				return ipv6Supported == 1;
			}
		}

		public static bool OSSupportsIPv4
		{
			get
			{
				CheckProtocolSupport();
				return ipv4Supported == 1;
			}
		}

		public static bool OSSupportsIPv6
		{
			get
			{
				CheckProtocolSupport();
				return ipv6Supported == 1;
			}
		}

		public AddressFamily AddressFamily
		{
			get
			{
				return address_family;
			}
		}

		public bool Blocking
		{
			get
			{
				return blocking;
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				int error;
				Blocking_internal(socket, value, out error);
				if (error != 0)
				{
					throw new SocketException(error);
				}
				blocking = value;
			}
		}

		public bool Connected
		{
			get
			{
				return connected;
			}
			internal set
			{
				connected = value;
			}
		}

		public ProtocolType ProtocolType
		{
			get
			{
				return protocol_type;
			}
		}

		public bool NoDelay
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				ThrowIfUpd();
				return (int)GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug) != 0;
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				ThrowIfUpd();
				SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, value ? 1 : 0);
			}
		}

		public int ReceiveBufferSize
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer);
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified for a set operation is less than zero");
				}
				SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, value);
			}
		}

		public int SendBufferSize
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				return (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer);
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "The value specified for a set operation is less than zero");
				}
				SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, value);
			}
		}

		public short Ttl
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (address_family == AddressFamily.InterNetwork)
				{
					return (short)(int)GetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress);
				}
				if (address_family == AddressFamily.InterNetworkV6)
				{
					return (short)(int)GetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit);
				}
				throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
			}
			set
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (address_family == AddressFamily.InterNetwork)
				{
					SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, value);
					return;
				}
				if (address_family == AddressFamily.InterNetworkV6)
				{
					SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.HopLimit, value);
					return;
				}
				throw new NotSupportedException("This property is only valid for InterNetwork and InterNetworkV6 sockets");
			}
		}

		public EndPoint RemoteEndPoint
		{
			get
			{
				if (disposed && closed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (seed_endpoint == null)
				{
					return null;
				}
				int error;
				SocketAddress address = RemoteEndPoint_internal(socket, out error);
				if (error != 0)
				{
					throw new SocketException(error);
				}
				return seed_endpoint.Create(address);
			}
		}

		private Socket(AddressFamily family, SocketType type, ProtocolType proto, IntPtr sock)
		{
			address_family = family;
			socket_type = type;
			protocol_type = proto;
			socket = sock;
			connected = true;
		}

		[System.MonoTODO]
		public Socket(SocketInformation socketInformation)
		{
			throw new NotImplementedException("SocketInformation not figured out yet");
		}

		public Socket(AddressFamily family, SocketType type, ProtocolType proto)
		{
			if (family == AddressFamily.Unspecified)
			{
				throw new ArgumentException("family");
			}
			address_family = family;
			socket_type = type;
			protocol_type = proto;
			int error;
			socket = Socket_internal(family, type, proto, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
		}

		static Socket()
		{
			ipv4Supported = -1;
			ipv6Supported = -1;
			CheckProtocolSupport();
		}

		private static void AddSockets(ArrayList sockets, IList list, string name)
		{
			if (list != null)
			{
				foreach (Socket item in list)
				{
					if (item == null)
					{
						throw new ArgumentNullException("name", "Contains a null element");
					}
					sockets.Add(item);
				}
			}
			sockets.Add(null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Select_internal(ref Socket[] sockets, int microSeconds, out int error);

		public static void Select(IList checkRead, IList checkWrite, IList checkError, int microSeconds)
		{
			ArrayList arrayList = new ArrayList();
			AddSockets(arrayList, checkRead, "checkRead");
			AddSockets(arrayList, checkWrite, "checkWrite");
			AddSockets(arrayList, checkError, "checkError");
			if (arrayList.Count == 3)
			{
				throw new ArgumentNullException("checkRead, checkWrite, checkError", "All the lists are null or empty.");
			}
			Socket[] sockets = (Socket[])arrayList.ToArray(typeof(Socket));
			int error;
			Select_internal(ref sockets, microSeconds, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
			if (sockets == null)
			{
				if (checkRead != null)
				{
					checkRead.Clear();
				}
				if (checkWrite != null)
				{
					checkWrite.Clear();
				}
				if (checkError != null)
				{
					checkError.Clear();
				}
				return;
			}
			int num = 0;
			int num2 = sockets.Length;
			IList list = checkRead;
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				Socket socket = sockets[i];
				if (socket == null)
				{
					if (list != null)
					{
						int num4 = list.Count - num3;
						for (int j = 0; j < num4; j++)
						{
							list.RemoveAt(num3);
						}
					}
					IList list2;
					if (num == 0)
					{
						list2 = checkWrite;
					}
					else
					{
						list2 = checkError;
					}
					list = list2;
					num3 = 0;
					num++;
				}
				else
				{
					if (num == 1 && list == checkWrite && !socket.connected && (int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error) == 0)
					{
						socket.connected = true;
					}
					int count = list.Count;
					Socket socket2;
					while ((socket2 = (Socket)list[num3]) != socket)
					{
						list.RemoveAt(num3);
					}
					num3++;
				}
			}
		}

		private void SocketDefaults()
		{
			try
			{
				if (address_family == AddressFamily.InterNetwork)
				{
					DontFragment = false;
				}
			}
			catch (SocketException)
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Available_internal(IntPtr socket, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SocketAddress LocalEndPoint_internal(IntPtr socket, out int error);

		public bool AcceptAsync(SocketAsyncEventArgs e)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (!IsBound)
			{
				throw new InvalidOperationException("You must call the Bind method before performing this operation.");
			}
			if (!islistening)
			{
				throw new InvalidOperationException("You must call the Listen method before performing this operation.");
			}
			if (e.BufferList != null)
			{
				throw new ArgumentException("Multiple buffers cannot be used with this method.");
			}
			if (e.Count < 0)
			{
				throw new ArgumentOutOfRangeException("e.Count");
			}
			Socket acceptSocket = e.AcceptSocket;
			if (acceptSocket != null)
			{
				if (acceptSocket.IsBound || acceptSocket.Connected)
				{
					throw new InvalidOperationException("AcceptSocket: The socket must not be bound or connected.");
				}
			}
			else
			{
				e.AcceptSocket = new Socket(AddressFamily, SocketType, ProtocolType);
			}
			try
			{
				e.DoOperation(SocketAsyncOperation.Accept, this);
			}
			catch
			{
				((IDisposable)e).Dispose();
				throw;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Accept_internal(IntPtr sock, out int error, bool blocking);

		public Socket Accept()
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			int error = 0;
			IntPtr sock = (IntPtr)(-1);
			blocking_thread = Thread.CurrentThread;
			try
			{
				sock = Accept_internal(this.socket, out error, blocking);
			}
			catch (ThreadAbortException)
			{
				if (disposed)
				{
					Thread.ResetAbort();
					error = 10004;
				}
			}
			finally
			{
				blocking_thread = null;
			}
			if (error != 0)
			{
				throw new SocketException(error);
			}
			Socket socket = new Socket(AddressFamily, SocketType, ProtocolType, sock);
			socket.seed_endpoint = seed_endpoint;
			socket.Blocking = Blocking;
			return socket;
		}

		internal void Accept(Socket acceptSocket)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			int error = 0;
			IntPtr intPtr = (IntPtr)(-1);
			blocking_thread = Thread.CurrentThread;
			try
			{
				intPtr = Accept_internal(socket, out error, blocking);
			}
			catch (ThreadAbortException)
			{
				if (disposed)
				{
					Thread.ResetAbort();
					error = 10004;
				}
			}
			finally
			{
				blocking_thread = null;
			}
			if (error != 0)
			{
				throw new SocketException(error);
			}
			acceptSocket.address_family = AddressFamily;
			acceptSocket.socket_type = SocketType;
			acceptSocket.protocol_type = ProtocolType;
			acceptSocket.socket = intPtr;
			acceptSocket.connected = true;
			acceptSocket.seed_endpoint = seed_endpoint;
			acceptSocket.Blocking = Blocking;
		}

		public IAsyncResult BeginAccept(AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (!isbound || !islistening)
			{
				throw new InvalidOperationException();
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.Accept);
			Worker worker = new Worker(socketAsyncResult);
			SocketAsyncCall socketAsyncCall = worker.Accept;
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginAccept(int receiveSize, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (receiveSize < 0)
			{
				throw new ArgumentOutOfRangeException("receiveSize", "receiveSize is less than zero");
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.AcceptReceive);
			Worker worker = new Worker(socketAsyncResult);
			SocketAsyncCall socketAsyncCall = worker.AcceptReceive;
			socketAsyncResult.Buffer = new byte[receiveSize];
			socketAsyncResult.Offset = 0;
			socketAsyncResult.Size = receiveSize;
			socketAsyncResult.SockFlags = SocketFlags.None;
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginAccept(Socket acceptSocket, int receiveSize, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (receiveSize < 0)
			{
				throw new ArgumentOutOfRangeException("receiveSize", "receiveSize is less than zero");
			}
			if (acceptSocket != null)
			{
				if (acceptSocket.disposed && acceptSocket.closed)
				{
					throw new ObjectDisposedException(acceptSocket.GetType().ToString());
				}
				if (acceptSocket.IsBound)
				{
					throw new InvalidOperationException();
				}
				if (acceptSocket.ProtocolType != ProtocolType.Tcp)
				{
					throw new SocketException(10022);
				}
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.AcceptReceive);
			Worker worker = new Worker(socketAsyncResult);
			SocketAsyncCall socketAsyncCall = worker.AcceptReceive;
			socketAsyncResult.Buffer = new byte[receiveSize];
			socketAsyncResult.Offset = 0;
			socketAsyncResult.Size = receiveSize;
			socketAsyncResult.SockFlags = SocketFlags.None;
			socketAsyncResult.AcceptSocket = acceptSocket;
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginConnect(EndPoint end_point, AsyncCallback callback, object state)
		{
			return BeginConnect(end_point, callback, state, false);
		}

		internal IAsyncResult BeginConnect(EndPoint end_point, AsyncCallback callback, object state, bool bypassSocketSecurity)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (end_point == null)
			{
				throw new ArgumentNullException("end_point");
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.Connect);
			socketAsyncResult.EndPoint = end_point;
			if (end_point is IPEndPoint)
			{
				IPEndPoint iPEndPoint = (IPEndPoint)end_point;
				if (iPEndPoint.Address.Equals(IPAddress.Any) || iPEndPoint.Address.Equals(IPAddress.IPv6Any))
				{
					socketAsyncResult.Complete(new SocketException(10049), true);
					return socketAsyncResult;
				}
			}
			int error = 0;
			if (!blocking)
			{
				SocketAddress sa = end_point.Serialize();
				Connect_internal(socket, sa, out error);
				switch (error)
				{
				case 0:
					connected = true;
					socketAsyncResult.Complete(true);
					break;
				default:
					connected = false;
					socketAsyncResult.Complete(new SocketException(error), true);
					break;
				case 10035:
				case 10036:
					break;
				}
			}
			if (blocking || error == 10036 || error == 10035)
			{
				connected = false;
				Worker worker = new Worker(socketAsyncResult, bypassSocketSecurity);
				SocketAsyncCall socketAsyncCall = worker.Connect;
				socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			}
			return socketAsyncResult;
		}

		public IAsyncResult BeginConnect(IPAddress address, int port, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.ToString().Length == 0)
			{
				throw new ArgumentException("The length of the IP address is zero");
			}
			if (islistening)
			{
				throw new InvalidOperationException();
			}
			IPEndPoint end_point = new IPEndPoint(address, port);
			return BeginConnect(end_point, callback, state);
		}

		public IAsyncResult BeginConnect(IPAddress[] addresses, int port, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (addresses == null)
			{
				throw new ArgumentNullException("addresses");
			}
			if (AddressFamily != AddressFamily.InterNetwork && AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException("This method is only valid for addresses in the InterNetwork or InterNetworkV6 families");
			}
			if (islistening)
			{
				throw new InvalidOperationException();
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.Connect);
			socketAsyncResult.Addresses = addresses;
			socketAsyncResult.Port = port;
			connected = false;
			Worker worker = new Worker(socketAsyncResult);
			SocketAsyncCall socketAsyncCall = worker.Connect;
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginConnect(string host, int port, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			if (address_family != AddressFamily.InterNetwork && address_family != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException("This method is valid only for sockets in the InterNetwork and InterNetworkV6 families");
			}
			if (islistening)
			{
				throw new InvalidOperationException();
			}
			IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
			return BeginConnect(hostAddresses, port, callback, state);
		}

		public IAsyncResult BeginDisconnect(bool reuseSocket, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			SocketAsyncResult socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.Disconnect);
			socketAsyncResult.ReuseSocket = reuseSocket;
			Worker worker = new Worker(socketAsyncResult);
			SocketAsyncCall socketAsyncCall = worker.Disconnect;
			socketAsyncCall.BeginInvoke(null, socketAsyncResult);
			return socketAsyncResult;
		}

		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags socket_flags, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			SocketAsyncResult socketAsyncResult;
			lock (readQ)
			{
				socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.Receive);
				socketAsyncResult.Buffer = buffer;
				socketAsyncResult.Offset = offset;
				socketAsyncResult.Size = size;
				socketAsyncResult.SockFlags = socket_flags;
				readQ.Enqueue(socketAsyncResult);
				if (readQ.Count == 1)
				{
					Worker worker = new Worker(socketAsyncResult);
					SocketAsyncCall socketAsyncCall = worker.Receive;
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, SocketFlags flags, out SocketError error, AsyncCallback callback, object state)
		{
			error = SocketError.Success;
			return BeginReceive(buffer, offset, size, flags, callback, state);
		}

		[CLSCompliant(false)]
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			SocketAsyncResult socketAsyncResult;
			lock (readQ)
			{
				socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.ReceiveGeneric);
				socketAsyncResult.Buffers = buffers;
				socketAsyncResult.SockFlags = socketFlags;
				readQ.Enqueue(socketAsyncResult);
				if (readQ.Count == 1)
				{
					Worker worker = new Worker(socketAsyncResult);
					SocketAsyncCall socketAsyncCall = worker.ReceiveGeneric;
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		[CLSCompliant(false)]
		public IAsyncResult BeginReceive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			errorCode = SocketError.Success;
			return BeginReceive(buffers, socketFlags, callback, state);
		}

		public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socket_flags, ref EndPoint remote_end, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be >= 0");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", "size must be >= 0");
			}
			if (offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset, size", "offset + size exceeds the buffer length");
			}
			SocketAsyncResult socketAsyncResult;
			lock (readQ)
			{
				socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.ReceiveFrom);
				socketAsyncResult.Buffer = buffer;
				socketAsyncResult.Offset = offset;
				socketAsyncResult.Size = size;
				socketAsyncResult.SockFlags = socket_flags;
				socketAsyncResult.EndPoint = remote_end;
				readQ.Enqueue(socketAsyncResult);
				if (readQ.Count == 1)
				{
					Worker worker = new Worker(socketAsyncResult);
					SocketAsyncCall socketAsyncCall = worker.ReceiveFrom;
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		[System.MonoTODO]
		public IAsyncResult BeginReceiveMessageFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			throw new NotImplementedException();
		}

		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socket_flags, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be >= 0");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", "size must be >= 0");
			}
			if (offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset, size", "offset + size exceeds the buffer length");
			}
			if (!connected)
			{
				throw new SocketException(10057);
			}
			SocketAsyncResult socketAsyncResult;
			lock (writeQ)
			{
				socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.Send);
				socketAsyncResult.Buffer = buffer;
				socketAsyncResult.Offset = offset;
				socketAsyncResult.Size = size;
				socketAsyncResult.SockFlags = socket_flags;
				writeQ.Enqueue(socketAsyncResult);
				if (writeQ.Count == 1)
				{
					Worker worker = new Worker(socketAsyncResult);
					SocketAsyncCall socketAsyncCall = worker.Send;
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		public IAsyncResult BeginSend(byte[] buffer, int offset, int size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			if (!connected)
			{
				errorCode = SocketError.NotConnected;
				throw new SocketException((int)errorCode);
			}
			errorCode = SocketError.Success;
			return BeginSend(buffer, offset, size, socketFlags, callback, state);
		}

		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			if (!connected)
			{
				throw new SocketException(10057);
			}
			SocketAsyncResult socketAsyncResult;
			lock (writeQ)
			{
				socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.SendGeneric);
				socketAsyncResult.Buffers = buffers;
				socketAsyncResult.SockFlags = socketFlags;
				writeQ.Enqueue(socketAsyncResult);
				if (writeQ.Count == 1)
				{
					Worker worker = new Worker(socketAsyncResult);
					SocketAsyncCall socketAsyncCall = worker.SendGeneric;
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		[CLSCompliant(false)]
		public IAsyncResult BeginSend(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, object state)
		{
			if (!connected)
			{
				errorCode = SocketError.NotConnected;
				throw new SocketException((int)errorCode);
			}
			errorCode = SocketError.Success;
			return BeginSend(buffers, socketFlags, callback, state);
		}

		public IAsyncResult BeginSendFile(string fileName, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (!connected)
			{
				throw new NotSupportedException();
			}
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException();
			}
			return BeginSendFile(fileName, null, null, TransmitFileOptions.UseDefaultWorkerThread, callback, state);
		}

		public IAsyncResult BeginSendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (!connected)
			{
				throw new NotSupportedException();
			}
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException();
			}
			SendFileHandler sendFileHandler = SendFile;
			return new SendFileAsyncResult(sendFileHandler, sendFileHandler.BeginInvoke(fileName, preBuffer, postBuffer, flags, callback, state));
		}

		public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size, SocketFlags socket_flags, EndPoint remote_end, AsyncCallback callback, object state)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be >= 0");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size", "size must be >= 0");
			}
			if (offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset, size", "offset + size exceeds the buffer length");
			}
			SocketAsyncResult socketAsyncResult;
			lock (writeQ)
			{
				socketAsyncResult = new SocketAsyncResult(this, state, callback, SocketOperation.SendTo);
				socketAsyncResult.Buffer = buffer;
				socketAsyncResult.Offset = offset;
				socketAsyncResult.Size = size;
				socketAsyncResult.SockFlags = socket_flags;
				socketAsyncResult.EndPoint = remote_end;
				writeQ.Enqueue(socketAsyncResult);
				if (writeQ.Count == 1)
				{
					Worker worker = new Worker(socketAsyncResult);
					SocketAsyncCall socketAsyncCall = worker.SendTo;
					socketAsyncCall.BeginInvoke(null, socketAsyncResult);
				}
			}
			return socketAsyncResult;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Bind_internal(IntPtr sock, SocketAddress sa, out int error);

		public void Bind(EndPoint local_end)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (local_end == null)
			{
				throw new ArgumentNullException("local_end");
			}
			if (Environment.SocketSecurityEnabled && current_bind_count >= max_bind_count)
			{
				throw new SecurityException("Too many sockets are bound, maximum count in the webplayer is " + max_bind_count);
			}
			int error;
			Bind_internal(socket, local_end.Serialize(), out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
			if (error == 0)
			{
				isbound = true;
			}
			if (Environment.SocketSecurityEnabled)
			{
				current_bind_count++;
			}
			seed_endpoint = local_end;
		}

		public bool ConnectAsync(SocketAsyncEventArgs e)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (islistening)
			{
				throw new InvalidOperationException("You may not perform this operation after calling the Listen method.");
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP", "Value cannot be null.");
			}
			if (e.BufferList != null)
			{
				throw new ArgumentException("Multiple buffers cannot be used with this method.");
			}
			e.DoOperation(SocketAsyncOperation.Connect, this);
			return true;
		}

		public void Connect(IPAddress address, int port)
		{
			Connect(new IPEndPoint(address, port));
		}

		public void Connect(IPAddress[] addresses, int port)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (addresses == null)
			{
				throw new ArgumentNullException("addresses");
			}
			if (AddressFamily != AddressFamily.InterNetwork && AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new NotSupportedException("This method is only valid for addresses in the InterNetwork or InterNetworkV6 families");
			}
			if (islistening)
			{
				throw new InvalidOperationException();
			}
			int error = 0;
			foreach (IPAddress address in addresses)
			{
				IPEndPoint iPEndPoint = new IPEndPoint(address, port);
				SocketAddress sa = iPEndPoint.Serialize();
				Connect_internal(socket, sa, out error);
				switch (error)
				{
				case 0:
					connected = true;
					seed_endpoint = iPEndPoint;
					return;
				case 10035:
				case 10036:
					if (!blocking)
					{
						Poll(-1, SelectMode.SelectWrite);
						error = (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);
						if (error == 0)
						{
							connected = true;
							seed_endpoint = iPEndPoint;
							return;
						}
					}
					break;
				}
			}
			if (error != 0)
			{
				throw new SocketException(error);
			}
		}

		public void Connect(string host, int port)
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
			Connect(hostAddresses, port);
		}

		public bool DisconnectAsync(SocketAsyncEventArgs e)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			e.DoOperation(SocketAsyncOperation.Disconnect, this);
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Disconnect_internal(IntPtr sock, bool reuse, out int error);

		public void Disconnect(bool reuseSocket)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			int error = 0;
			Disconnect_internal(socket, reuseSocket, out error);
			switch (error)
			{
			case 50:
				throw new PlatformNotSupportedException();
			default:
				throw new SocketException(error);
			case 0:
				connected = false;
				if (!reuseSocket)
				{
				}
				break;
			}
		}

		[System.MonoTODO("Not implemented")]
		public SocketInformation DuplicateAndClose(int targetProcessId)
		{
			throw new NotImplementedException();
		}

		public Socket EndAccept(IAsyncResult result)
		{
			byte[] buffer;
			int bytesTransferred;
			return EndAccept(out buffer, out bytesTransferred, result);
		}

		public Socket EndAccept(out byte[] buffer, IAsyncResult asyncResult)
		{
			int bytesTransferred;
			return EndAccept(out buffer, out bytesTransferred, asyncResult);
		}

		public Socket EndAccept(out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			SocketAsyncResult socketAsyncResult = asyncResult as SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw InvalidAsyncOp("EndAccept");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
			buffer = socketAsyncResult.Buffer;
			bytesTransferred = socketAsyncResult.Total;
			return socketAsyncResult.Socket;
		}

		public void EndConnect(IAsyncResult result)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			SocketAsyncResult socketAsyncResult = result as SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "result");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw InvalidAsyncOp("EndConnect");
			}
			if (!result.IsCompleted)
			{
				result.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
		}

		public void EndDisconnect(IAsyncResult asyncResult)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			SocketAsyncResult socketAsyncResult = asyncResult as SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw InvalidAsyncOp("EndDisconnect");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
		}

		public int EndReceive(IAsyncResult result)
		{
			SocketError errorCode;
			return EndReceive(result, out errorCode);
		}

		public int EndReceive(IAsyncResult asyncResult, out SocketError errorCode)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			SocketAsyncResult socketAsyncResult = asyncResult as SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw InvalidAsyncOp("EndReceive");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			errorCode = socketAsyncResult.ErrorCode;
			socketAsyncResult.CheckIfThrowDelayedException();
			return socketAsyncResult.Total;
		}

		public int EndReceiveFrom(IAsyncResult result, ref EndPoint end_point)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			SocketAsyncResult socketAsyncResult = result as SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "result");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw InvalidAsyncOp("EndReceiveFrom");
			}
			if (!result.IsCompleted)
			{
				result.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
			end_point = socketAsyncResult.EndPoint;
			return socketAsyncResult.Total;
		}

		[System.MonoTODO]
		public int EndReceiveMessageFrom(IAsyncResult asyncResult, ref SocketFlags socketFlags, ref EndPoint endPoint, out IPPacketInformation ipPacketInformation)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (endPoint == null)
			{
				throw new ArgumentNullException("endPoint");
			}
			SocketAsyncResult socketAsyncResult = asyncResult as SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw InvalidAsyncOp("EndReceiveMessageFrom");
			}
			throw new NotImplementedException();
		}

		public int EndSend(IAsyncResult result)
		{
			SocketError errorCode;
			return EndSend(result, out errorCode);
		}

		public int EndSend(IAsyncResult asyncResult, out SocketError errorCode)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			SocketAsyncResult socketAsyncResult = asyncResult as SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "result");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw InvalidAsyncOp("EndSend");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			errorCode = socketAsyncResult.ErrorCode;
			socketAsyncResult.CheckIfThrowDelayedException();
			return socketAsyncResult.Total;
		}

		public void EndSendFile(IAsyncResult asyncResult)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			SendFileAsyncResult sendFileAsyncResult = asyncResult as SendFileAsyncResult;
			if (sendFileAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			sendFileAsyncResult.Delegate.EndInvoke(sendFileAsyncResult.Original);
		}

		private Exception InvalidAsyncOp(string method)
		{
			return new InvalidOperationException(method + " can only be called once per asynchronous operation");
		}

		public int EndSendTo(IAsyncResult result)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			SocketAsyncResult socketAsyncResult = result as SocketAsyncResult;
			if (socketAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "result");
			}
			if (Interlocked.CompareExchange(ref socketAsyncResult.EndCalled, 1, 0) == 1)
			{
				throw InvalidAsyncOp("EndSendTo");
			}
			if (!result.IsCompleted)
			{
				result.AsyncWaitHandle.WaitOne();
			}
			socketAsyncResult.CheckIfThrowDelayedException();
			return socketAsyncResult.Total;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSocketOption_arr_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, ref byte[] byte_val, out int error);

		public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (optionValue == null)
			{
				throw new SocketException(10014, "Error trying to dereference an invalid pointer");
			}
			int error;
			GetSocketOption_arr_internal(socket, optionLevel, optionName, ref optionValue, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
		}

		public byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int length)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			byte[] byte_val = new byte[length];
			int error;
			GetSocketOption_arr_internal(socket, optionLevel, optionName, ref byte_val, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
			return byte_val;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int WSAIoctl(IntPtr sock, int ioctl_code, byte[] input, byte[] output, out int error);

		public int IOControl(int ioctl_code, byte[] in_value, byte[] out_value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			int error;
			int num = WSAIoctl(socket, ioctl_code, in_value, out_value, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
			if (num == -1)
			{
				throw new InvalidOperationException("Must use Blocking property instead.");
			}
			return num;
		}

		[System.MonoTODO]
		public int IOControl(IOControlCode ioControlCode, byte[] optionInValue, byte[] optionOutValue)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Listen_internal(IntPtr sock, int backlog, out int error);

		public void Listen(int backlog)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (!isbound)
			{
				throw new SocketException(10022);
			}
			if (Environment.SocketSecurityEnabled)
			{
				SecurityException ex = new SecurityException("Listening on TCP sockets is not allowed in the webplayer");
				Console.WriteLine("Throwing the following securityexception: " + ex);
				throw ex;
			}
			int error;
			Listen_internal(socket, backlog, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
			islistening = true;
		}

		public bool Poll(int time_us, SelectMode mode)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (mode != SelectMode.SelectRead && mode != SelectMode.SelectWrite && mode != SelectMode.SelectError)
			{
				throw new NotSupportedException("'mode' parameter is not valid.");
			}
			int error;
			bool flag = Poll_internal(socket, mode, time_us, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
			if (mode == SelectMode.SelectWrite && flag && !connected && (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error) == 0)
			{
				connected = true;
			}
			return flag;
		}

		public int Receive(byte[] buffer)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			SocketError error;
			int result = Receive_nochecks(buffer, 0, buffer.Length, SocketFlags.None, out error);
			if (error != SocketError.Success)
			{
				throw new SocketException((int)error);
			}
			return result;
		}

		public int Receive(byte[] buffer, SocketFlags flags)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			SocketError error;
			int result = Receive_nochecks(buffer, 0, buffer.Length, flags, out error);
			if (error != SocketError.Success)
			{
				if (error == SocketError.WouldBlock && blocking)
				{
					throw new SocketException((int)error, "Operation timed out.");
				}
				throw new SocketException((int)error);
			}
			return result;
		}

		public int Receive(byte[] buffer, int size, SocketFlags flags)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (size < 0 || size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			SocketError error;
			int result = Receive_nochecks(buffer, 0, size, flags, out error);
			if (error != SocketError.Success)
			{
				if (error == SocketError.WouldBlock && blocking)
				{
					throw new SocketException((int)error, "Operation timed out.");
				}
				throw new SocketException((int)error);
			}
			return result;
		}

		public int Receive(byte[] buffer, int offset, int size, SocketFlags flags)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			SocketError error;
			int result = Receive_nochecks(buffer, offset, size, flags, out error);
			if (error != SocketError.Success)
			{
				if (error == SocketError.WouldBlock && blocking)
				{
					throw new SocketException((int)error, "Operation timed out.");
				}
				throw new SocketException((int)error);
			}
			return result;
		}

		public int Receive(byte[] buffer, int offset, int size, SocketFlags flags, out SocketError error)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return Receive_nochecks(buffer, offset, size, flags, out error);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Receive_internal(IntPtr sock, WSABUF[] bufarray, SocketFlags flags, out int error);

		public int Receive(IList<ArraySegment<byte>> buffers)
		{
			SocketError errorCode;
			int result = Receive(buffers, SocketFlags.None, out errorCode);
			if (errorCode != SocketError.Success)
			{
				throw new SocketException((int)errorCode);
			}
			return result;
		}

		[CLSCompliant(false)]
		public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			SocketError errorCode;
			int result = Receive(buffers, socketFlags, out errorCode);
			if (errorCode != SocketError.Success)
			{
				throw new SocketException((int)errorCode);
			}
			return result;
		}

		[CLSCompliant(false)]
		public int Receive(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffers == null || buffers.Count == 0)
			{
				throw new ArgumentNullException("buffers");
			}
			int count = buffers.Count;
			WSABUF[] array = new WSABUF[count];
			GCHandle[] array2 = new GCHandle[count];
			for (int i = 0; i < count; i++)
			{
				ArraySegment<byte> arraySegment = buffers[i];
				array2[i] = GCHandle.Alloc(arraySegment.Array, GCHandleType.Pinned);
				array[i].len = arraySegment.Count;
				array[i].buf = Marshal.UnsafeAddrOfPinnedArrayElement(arraySegment.Array, arraySegment.Offset);
			}
			int result;
			int error;
			try
			{
				result = Receive_internal(socket, array, socketFlags, out error);
			}
			finally
			{
				for (int j = 0; j < count; j++)
				{
					if (array2[j].IsAllocated)
					{
						array2[j].Free();
					}
				}
			}
			errorCode = (SocketError)error;
			return result;
		}

		public bool ReceiveFromAsync(SocketAsyncEventArgs e)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (e.BufferList != null)
			{
				throw new NotSupportedException("Mono doesn't support using BufferList at this point.");
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP", "Value cannot be null.");
			}
			e.DoOperation(SocketAsyncOperation.ReceiveFrom, this);
			return true;
		}

		public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			return ReceiveFrom_nochecks(buffer, 0, buffer.Length, SocketFlags.None, ref remoteEP);
		}

		public int ReceiveFrom(byte[] buffer, SocketFlags flags, ref EndPoint remoteEP)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			return ReceiveFrom_nochecks(buffer, 0, buffer.Length, flags, ref remoteEP);
		}

		public int ReceiveFrom(byte[] buffer, int size, SocketFlags flags, ref EndPoint remoteEP)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			if (size < 0 || size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return ReceiveFrom_nochecks(buffer, 0, size, flags, ref remoteEP);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RecvFrom_internal(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, ref SocketAddress sockaddr, out int error);

		public int ReceiveFrom(byte[] buffer, int offset, int size, SocketFlags flags, ref EndPoint remoteEP)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return ReceiveFrom_nochecks(buffer, offset, size, flags, ref remoteEP);
		}

		internal int ReceiveFrom_nochecks(byte[] buf, int offset, int size, SocketFlags flags, ref EndPoint remote_end)
		{
			int error;
			return ReceiveFrom_nochecks_exc(buf, offset, size, flags, ref remote_end, true, out error);
		}

		internal int ReceiveFrom_nochecks_exc(byte[] buf, int offset, int size, SocketFlags flags, ref EndPoint remote_end, bool throwOnError, out int error)
		{
			SocketAddress sockaddr = remote_end.Serialize();
			int result = RecvFrom_internal(socket, buf, offset, size, flags, ref sockaddr, out error);
			SocketError socketError = (SocketError)error;
			if (socketError != SocketError.Success)
			{
				if (socketError != SocketError.WouldBlock && socketError != SocketError.InProgress)
				{
					connected = false;
				}
				else if (socketError == SocketError.WouldBlock && blocking)
				{
					if (throwOnError)
					{
						throw new SocketException(10060, "Operation timed out");
					}
					error = 10060;
					return 0;
				}
				if (throwOnError)
				{
					throw new SocketException(error);
				}
				return 0;
			}
			if (Environment.SocketSecurityEnabled && !CheckEndPoint(sockaddr))
			{
				buf.Initialize();
				throw new SecurityException("Unable to connect, as no valid crossdomain policy was found");
			}
			connected = true;
			isbound = true;
			if (sockaddr != null)
			{
				remote_end = remote_end.Create(sockaddr);
			}
			seed_endpoint = remote_end;
			return result;
		}

		[System.MonoTODO("Not implemented")]
		public bool ReceiveMessageFromAsync(SocketAsyncEventArgs e)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			throw new NotImplementedException();
		}

		[System.MonoTODO("Not implemented")]
		public int ReceiveMessageFrom(byte[] buffer, int offset, int size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			throw new NotImplementedException();
		}

		[System.MonoTODO("Not implemented")]
		public bool SendPacketsAsync(SocketAsyncEventArgs e)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			throw new NotImplementedException();
		}

		public int Send(byte[] buf)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			SocketError error;
			int result = Send_nochecks(buf, 0, buf.Length, SocketFlags.None, out error);
			if (error != SocketError.Success)
			{
				throw new SocketException((int)error);
			}
			return result;
		}

		public int Send(byte[] buf, SocketFlags flags)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			SocketError error;
			int result = Send_nochecks(buf, 0, buf.Length, flags, out error);
			if (error != SocketError.Success)
			{
				throw new SocketException((int)error);
			}
			return result;
		}

		public int Send(byte[] buf, int size, SocketFlags flags)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			if (size < 0 || size > buf.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			SocketError error;
			int result = Send_nochecks(buf, 0, size, flags, out error);
			if (error != SocketError.Success)
			{
				throw new SocketException((int)error);
			}
			return result;
		}

		public int Send(byte[] buf, int offset, int size, SocketFlags flags)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buf.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buf.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			SocketError error;
			int result = Send_nochecks(buf, offset, size, flags, out error);
			if (error != SocketError.Success)
			{
				throw new SocketException((int)error);
			}
			return result;
		}

		public int Send(byte[] buf, int offset, int size, SocketFlags flags, out SocketError error)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset > buf.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buf.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return Send_nochecks(buf, offset, size, flags, out error);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Send_internal(IntPtr sock, WSABUF[] bufarray, SocketFlags flags, out int error);

		public int Send(IList<ArraySegment<byte>> buffers)
		{
			SocketError errorCode;
			int result = Send(buffers, SocketFlags.None, out errorCode);
			if (errorCode != SocketError.Success)
			{
				throw new SocketException((int)errorCode);
			}
			return result;
		}

		public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
		{
			SocketError errorCode;
			int result = Send(buffers, socketFlags, out errorCode);
			if (errorCode != SocketError.Success)
			{
				throw new SocketException((int)errorCode);
			}
			return result;
		}

		[CLSCompliant(false)]
		public int Send(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags, out SocketError errorCode)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffers == null)
			{
				throw new ArgumentNullException("buffers");
			}
			if (buffers.Count == 0)
			{
				throw new ArgumentException("Buffer is empty", "buffers");
			}
			int count = buffers.Count;
			WSABUF[] array = new WSABUF[count];
			GCHandle[] array2 = new GCHandle[count];
			for (int i = 0; i < count; i++)
			{
				ArraySegment<byte> arraySegment = buffers[i];
				array2[i] = GCHandle.Alloc(arraySegment.Array, GCHandleType.Pinned);
				array[i].len = arraySegment.Count;
				array[i].buf = Marshal.UnsafeAddrOfPinnedArrayElement(arraySegment.Array, arraySegment.Offset);
			}
			int result;
			int error;
			try
			{
				result = Send_internal(socket, array, socketFlags, out error);
			}
			finally
			{
				for (int j = 0; j < count; j++)
				{
					if (array2[j].IsAllocated)
					{
						array2[j].Free();
					}
				}
			}
			errorCode = (SocketError)error;
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SendFile(IntPtr sock, string filename, byte[] pre_buffer, byte[] post_buffer, TransmitFileOptions flags);

		public void SendFile(string fileName)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (!connected)
			{
				throw new NotSupportedException();
			}
			if (!blocking)
			{
				throw new InvalidOperationException();
			}
			SendFile(fileName, null, null, TransmitFileOptions.UseDefaultWorkerThread);
		}

		public void SendFile(string fileName, byte[] preBuffer, byte[] postBuffer, TransmitFileOptions flags)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (!connected)
			{
				throw new NotSupportedException();
			}
			if (!blocking)
			{
				throw new InvalidOperationException();
			}
			if (!SendFile(socket, fileName, preBuffer, postBuffer, flags))
			{
				SocketException ex = new SocketException();
				if (ex.ErrorCode == 2 || ex.ErrorCode == 3)
				{
					throw new FileNotFoundException();
				}
				throw ex;
			}
		}

		public bool SendToAsync(SocketAsyncEventArgs e)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (e.RemoteEndPoint == null)
			{
				throw new ArgumentNullException("remoteEP", "Value cannot be null.");
			}
			e.DoOperation(SocketAsyncOperation.SendTo, this);
			return true;
		}

		public int SendTo(byte[] buffer, EndPoint remote_end)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remote_end == null)
			{
				throw new ArgumentNullException("remote_end");
			}
			return SendTo_nochecks(buffer, 0, buffer.Length, SocketFlags.None, remote_end);
		}

		public int SendTo(byte[] buffer, SocketFlags flags, EndPoint remote_end)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remote_end == null)
			{
				throw new ArgumentNullException("remote_end");
			}
			return SendTo_nochecks(buffer, 0, buffer.Length, flags, remote_end);
		}

		public int SendTo(byte[] buffer, int size, SocketFlags flags, EndPoint remote_end)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remote_end == null)
			{
				throw new ArgumentNullException("remote_end");
			}
			if (size < 0 || size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return SendTo_nochecks(buffer, 0, size, flags, remote_end);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SendTo_internal_real(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, SocketAddress sa, out int error);

		private static int SendTo_internal(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, SocketAddress sa, out int error)
		{
			if (Environment.SocketSecurityEnabled && !CheckEndPoint(sa))
			{
				SecurityException ex = new SecurityException("SendTo request refused by Unity webplayer security model");
				Console.WriteLine("Throwing the following security exception: " + ex);
				throw ex;
			}
			return SendTo_internal_real(sock, buffer, offset, count, flags, sa, out error);
		}

		public int SendTo(byte[] buffer, int offset, int size, SocketFlags flags, EndPoint remote_end)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (remote_end == null)
			{
				throw new ArgumentNullException("remote_end");
			}
			if (offset < 0 || offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || offset + size > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			return SendTo_nochecks(buffer, offset, size, flags, remote_end);
		}

		internal int SendTo_nochecks(byte[] buffer, int offset, int size, SocketFlags flags, EndPoint remote_end)
		{
			SocketAddress sa = remote_end.Serialize();
			int error;
			int result = SendTo_internal(socket, buffer, offset, size, flags, sa, out error);
			SocketError socketError = (SocketError)error;
			if (socketError != SocketError.Success)
			{
				if (socketError != SocketError.WouldBlock && socketError != SocketError.InProgress)
				{
					connected = false;
				}
				throw new SocketException(error);
			}
			connected = true;
			isbound = true;
			seed_endpoint = remote_end;
			return result;
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, byte[] optionValue)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (optionValue == null)
			{
				throw new SocketException(10014, "Error trying to dereference an invalid pointer");
			}
			int error;
			SetSocketOption_internal(socket, optionLevel, optionName, null, optionValue, 0, out error);
			switch (error)
			{
			case 10022:
				throw new ArgumentException();
			default:
				throw new SocketException(error);
			case 0:
				break;
			}
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, object optionValue)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (optionValue == null)
			{
				throw new ArgumentNullException("optionValue");
			}
			int error;
			if (optionLevel == SocketOptionLevel.Socket && optionName == SocketOptionName.Linger)
			{
				LingerOption lingerOption = optionValue as LingerOption;
				if (lingerOption == null)
				{
					throw new ArgumentException("A 'LingerOption' value must be specified.", "optionValue");
				}
				SetSocketOption_internal(socket, optionLevel, optionName, lingerOption, null, 0, out error);
			}
			else if (optionLevel == SocketOptionLevel.IP && (optionName == SocketOptionName.AddMembership || optionName == SocketOptionName.DropMembership))
			{
				MulticastOption multicastOption = optionValue as MulticastOption;
				if (multicastOption == null)
				{
					throw new ArgumentException("A 'MulticastOption' value must be specified.", "optionValue");
				}
				SetSocketOption_internal(socket, optionLevel, optionName, multicastOption, null, 0, out error);
			}
			else
			{
				if (optionLevel != SocketOptionLevel.IPv6 || (optionName != SocketOptionName.AddMembership && optionName != SocketOptionName.DropMembership))
				{
					throw new ArgumentException("Invalid value specified.", "optionValue");
				}
				IPv6MulticastOption pv6MulticastOption = optionValue as IPv6MulticastOption;
				if (pv6MulticastOption == null)
				{
					throw new ArgumentException("A 'IPv6MulticastOption' value must be specified.", "optionValue");
				}
				SetSocketOption_internal(socket, optionLevel, optionName, pv6MulticastOption, null, 0, out error);
			}
			switch (error)
			{
			case 10022:
				throw new ArgumentException();
			default:
				throw new SocketException(error);
			case 0:
				break;
			}
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, bool optionValue)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			int int_val = (optionValue ? 1 : 0);
			int error;
			SetSocketOption_internal(socket, optionLevel, optionName, null, null, int_val, out error);
			switch (error)
			{
			case 10022:
				throw new ArgumentException();
			default:
				throw new SocketException(error);
			case 0:
				break;
			}
		}

		internal static void CheckProtocolSupport()
		{
			if (ipv4Supported == -1)
			{
				try
				{
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Close();
					ipv4Supported = 1;
				}
				catch
				{
					ipv4Supported = 0;
				}
			}
			if (ipv6Supported == -1 && ipv6Supported != 0)
			{
				try
				{
					Socket socket2 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
					socket2.Close();
					ipv6Supported = 1;
				}
				catch
				{
					ipv6Supported = 0;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr Socket_internal(AddressFamily family, SocketType type, ProtocolType proto, out int error);

		~Socket()
		{
			Dispose(false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Blocking_internal(IntPtr socket, bool block, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SocketAddress RemoteEndPoint_internal(IntPtr socket, out int error);

		private void Linger(IntPtr handle)
		{
			if (!connected || linger_timeout <= 0)
			{
				return;
			}
			int error;
			Shutdown_internal(handle, SocketShutdown.Receive, out error);
			if (error != 0)
			{
				return;
			}
			int num = linger_timeout / 1000;
			int num2 = linger_timeout % 1000;
			if (num2 > 0)
			{
				Poll_internal(handle, SelectMode.SelectRead, num2 * 1000, out error);
				if (error != 0)
				{
					return;
				}
			}
			if (num > 0)
			{
				LingerOption obj_val = new LingerOption(true, num);
				SetSocketOption_internal(handle, SocketOptionLevel.Socket, SocketOptionName.Linger, obj_val, null, 0, out error);
			}
		}

		protected virtual void Dispose(bool explicitDisposing)
		{
			if (disposed)
			{
				return;
			}
			disposed = true;
			bool flag = connected;
			connected = false;
			if ((int)socket != -1)
			{
				if (Environment.SocketSecurityEnabled && current_bind_count > 0)
				{
					current_bind_count--;
				}
				closed = true;
				IntPtr handle = socket;
				socket = (IntPtr)(-1);
				Thread thread = blocking_thread;
				if (thread != null)
				{
					thread.Abort();
					blocking_thread = null;
				}
				if (flag)
				{
					Linger(handle);
				}
				int error;
				Close_internal(handle, out error);
				if (error != 0)
				{
					throw new SocketException(error);
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Close_internal(IntPtr socket, out int error);

		public void Close()
		{
			linger_timeout = 0;
			((IDisposable)this).Dispose();
		}

		public void Close(int timeout)
		{
			linger_timeout = timeout;
			((IDisposable)this).Dispose();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Connect_internal_real(IntPtr sock, SocketAddress sa, out int error);

		private static void Connect_internal(IntPtr sock, SocketAddress sa, out int error)
		{
			Connect_internal(sock, sa, out error, true);
		}

		private static void Connect_internal(IntPtr sock, SocketAddress sa, out int error, bool requireSocketPolicyFile)
		{
			if (requireSocketPolicyFile && !CheckEndPoint(sa))
			{
				throw new SecurityException("Unable to connect, as no valid crossdomain policy was found");
			}
			Connect_internal_real(sock, sa, out error);
		}

		internal static bool CheckEndPoint(SocketAddress sa)
		{
			if (!Environment.SocketSecurityEnabled)
			{
				return true;
			}
			try
			{
				IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Loopback, 123);
				IPEndPoint iPEndPoint2 = (IPEndPoint)iPEndPoint.Create(sa);
				if (check_socket_policy == null)
				{
					check_socket_policy = GetUnityCrossDomainHelperMethod("CheckSocketEndPoint");
				}
				return (bool)check_socket_policy.Invoke(null, new object[2]
				{
					iPEndPoint2.Address.ToString(),
					iPEndPoint2.Port
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unexpected error while trying to CheckEndPoint() : " + ex);
				return false;
			}
		}

		private static MethodInfo GetUnityCrossDomainHelperMethod(string methodname)
		{
			Type type = Type.GetType("UnityEngine.UnityCrossDomainHelper, CrossDomainPolicyParser, Version=1.0.0.0, Culture=neutral");
			if (type == null)
			{
				throw new SecurityException("Cant find type UnityCrossDomainHelper");
			}
			MethodInfo method = type.GetMethod(methodname);
			if (method == null)
			{
				throw new SecurityException("Cant find " + methodname);
			}
			return method;
		}

		public void Connect(EndPoint remoteEP)
		{
			Connect(remoteEP, true);
		}

		internal void Connect(EndPoint remoteEP, bool requireSocketPolicy)
		{
			SocketAddress socketAddress = null;
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (remoteEP == null)
			{
				throw new ArgumentNullException("remoteEP");
			}
			IPEndPoint iPEndPoint = remoteEP as IPEndPoint;
			if (iPEndPoint != null && (iPEndPoint.Address.Equals(IPAddress.Any) || iPEndPoint.Address.Equals(IPAddress.IPv6Any)))
			{
				throw new SocketException(10049);
			}
			if (islistening)
			{
				throw new InvalidOperationException();
			}
			socketAddress = remoteEP.Serialize();
			int error = 0;
			blocking_thread = Thread.CurrentThread;
			try
			{
				Connect_internal(socket, socketAddress, out error, requireSocketPolicy);
			}
			catch (ThreadAbortException)
			{
				if (disposed)
				{
					Thread.ResetAbort();
					error = 10004;
				}
			}
			finally
			{
				blocking_thread = null;
			}
			if (error != 0)
			{
				throw new SocketException(error);
			}
			connected = true;
			isbound = true;
			seed_endpoint = remoteEP;
		}

		public bool ReceiveAsync(SocketAsyncEventArgs e)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (e.BufferList != null)
			{
				throw new NotSupportedException("Mono doesn't support using BufferList at this point.");
			}
			e.DoOperation(SocketAsyncOperation.Receive, this);
			return true;
		}

		public bool SendAsync(SocketAsyncEventArgs e)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (e.Buffer == null && e.BufferList == null)
			{
				throw new ArgumentException("Either e.Buffer or e.BufferList must be valid buffers.");
			}
			e.DoOperation(SocketAsyncOperation.Send, this);
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Poll_internal(IntPtr socket, SelectMode mode, int timeout, out int error);

		internal bool Poll(int time_us, SelectMode mode, out int socket_error)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (mode != SelectMode.SelectRead && mode != SelectMode.SelectWrite && mode != SelectMode.SelectError)
			{
				throw new NotSupportedException("'mode' parameter is not valid.");
			}
			int error;
			bool flag = Poll_internal(socket, mode, time_us, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
			socket_error = (int)GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);
			if (mode == SelectMode.SelectWrite && flag && socket_error == 0)
			{
				connected = true;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Receive_internal(IntPtr sock, byte[] buffer, int offset, int count, SocketFlags flags, out int error);

		internal int Receive_nochecks(byte[] buf, int offset, int size, SocketFlags flags, out SocketError error)
		{
			if (protocol_type == ProtocolType.Udp)
			{
				EndPoint remote_end = new IPEndPoint(IPAddress.Any, 0);
				int error2 = 0;
				int result = ReceiveFrom_nochecks_exc(buf, offset, size, flags, ref remote_end, false, out error2);
				error = (SocketError)error2;
				return result;
			}
			int error3;
			int result2 = Receive_internal(socket, buf, offset, size, flags, out error3);
			error = (SocketError)error3;
			if (error != SocketError.Success && error != SocketError.WouldBlock && error != SocketError.InProgress)
			{
				connected = false;
			}
			else
			{
				connected = true;
			}
			return result2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSocketOption_obj_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, out object obj_val, out int error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Send_internal(IntPtr sock, byte[] buf, int offset, int count, SocketFlags flags, out int error);

		internal int Send_nochecks(byte[] buf, int offset, int size, SocketFlags flags, out SocketError error)
		{
			if (size == 0)
			{
				error = SocketError.Success;
				return 0;
			}
			int error2;
			int result = Send_internal(socket, buf, offset, size, flags, out error2);
			error = (SocketError)error2;
			if (error != SocketError.Success && error != SocketError.WouldBlock && error != SocketError.InProgress)
			{
				connected = false;
			}
			else
			{
				connected = true;
			}
			return result;
		}

		public object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			object obj_val;
			int error;
			GetSocketOption_obj_internal(socket, optionLevel, optionName, out obj_val, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
			switch (optionName)
			{
			case SocketOptionName.Linger:
				return (LingerOption)obj_val;
			case SocketOptionName.AddMembership:
			case SocketOptionName.DropMembership:
				return (MulticastOption)obj_val;
			default:
				if (obj_val is int)
				{
					return (int)obj_val;
				}
				return obj_val;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Shutdown_internal(IntPtr socket, SocketShutdown how, out int error);

		public void Shutdown(SocketShutdown how)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			if (!connected)
			{
				throw new SocketException(10057);
			}
			int error;
			Shutdown_internal(socket, how, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSocketOption_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, object obj_val, byte[] byte_val, int int_val, out int error);

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, int optionValue)
		{
			if (disposed && closed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			int error;
			SetSocketOption_internal(socket, optionLevel, optionName, null, null, optionValue, out error);
			if (error != 0)
			{
				throw new SocketException(error);
			}
		}

		private void ThrowIfUpd()
		{
		}
	}
}
