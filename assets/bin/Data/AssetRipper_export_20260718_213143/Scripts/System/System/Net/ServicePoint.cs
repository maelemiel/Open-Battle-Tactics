using System.Collections;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace System.Net
{
	public class ServicePoint
	{
		private Uri uri;

		private int connectionLimit;

		private int maxIdleTime;

		private int currentConnections;

		private DateTime idleSince;

		private Version protocolVersion;

		private X509Certificate certificate;

		private X509Certificate clientCertificate;

		private IPHostEntry host;

		private bool usesProxy;

		private Hashtable groups;

		private bool sendContinue = true;

		private bool useConnect;

		private object locker = new object();

		private object hostE = new object();

		private bool useNagle;

		private BindIPEndPoint endPointCallback;

		public Uri Address
		{
			get
			{
				return uri;
			}
		}

		public BindIPEndPoint BindIPEndPointDelegate
		{
			get
			{
				return endPointCallback;
			}
			set
			{
				endPointCallback = value;
			}
		}

		public X509Certificate Certificate
		{
			get
			{
				return certificate;
			}
		}

		public X509Certificate ClientCertificate
		{
			get
			{
				return clientCertificate;
			}
		}

		[System.MonoTODO]
		public int ConnectionLeaseTimeout
		{
			get
			{
				throw GetMustImplement();
			}
			set
			{
				throw GetMustImplement();
			}
		}

		public int ConnectionLimit
		{
			get
			{
				return connectionLimit;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				connectionLimit = value;
			}
		}

		public string ConnectionName
		{
			get
			{
				return uri.Scheme;
			}
		}

		public int CurrentConnections
		{
			get
			{
				return currentConnections;
			}
		}

		public DateTime IdleSince
		{
			get
			{
				return idleSince;
			}
			internal set
			{
				lock (locker)
				{
					idleSince = value;
				}
			}
		}

		public int MaxIdleTime
		{
			get
			{
				return maxIdleTime;
			}
			set
			{
				if (value < -1 || value > int.MaxValue)
				{
					throw new ArgumentOutOfRangeException();
				}
				maxIdleTime = value;
			}
		}

		public virtual Version ProtocolVersion
		{
			get
			{
				return protocolVersion;
			}
		}

		[System.MonoTODO]
		public int ReceiveBufferSize
		{
			get
			{
				throw GetMustImplement();
			}
			set
			{
				throw GetMustImplement();
			}
		}

		public bool SupportsPipelining
		{
			get
			{
				return HttpVersion.Version11.Equals(protocolVersion);
			}
		}

		public bool Expect100Continue
		{
			get
			{
				return SendContinue;
			}
			set
			{
				SendContinue = value;
			}
		}

		public bool UseNagleAlgorithm
		{
			get
			{
				return useNagle;
			}
			set
			{
				useNagle = value;
			}
		}

		internal bool SendContinue
		{
			get
			{
				return sendContinue && (protocolVersion == null || protocolVersion == HttpVersion.Version11);
			}
			set
			{
				sendContinue = value;
			}
		}

		internal bool UsesProxy
		{
			get
			{
				return usesProxy;
			}
			set
			{
				usesProxy = value;
			}
		}

		internal bool UseConnect
		{
			get
			{
				return useConnect;
			}
			set
			{
				useConnect = value;
			}
		}

		internal bool AvailableForRecycling
		{
			get
			{
				return CurrentConnections == 0 && maxIdleTime != -1 && DateTime.Now >= IdleSince.AddMilliseconds(maxIdleTime);
			}
		}

		internal Hashtable Groups
		{
			get
			{
				if (groups == null)
				{
					groups = new Hashtable();
				}
				return groups;
			}
		}

		internal IPHostEntry HostEntry
		{
			get
			{
				lock (hostE)
				{
					if (host != null)
					{
						return host;
					}
					string text = uri.Host;
					if (uri.HostNameType == UriHostNameType.IPv6 || uri.HostNameType == UriHostNameType.IPv4)
					{
						if (uri.HostNameType == UriHostNameType.IPv6)
						{
							text = text.Substring(1, text.Length - 2);
						}
						host = new IPHostEntry();
						host.AddressList = new IPAddress[1] { IPAddress.Parse(text) };
						return host;
					}
					try
					{
						host = Dns.GetHostByName(text);
					}
					catch
					{
						return null;
					}
				}
				return host;
			}
		}

		internal ServicePoint(Uri uri, int connectionLimit, int maxIdleTime)
		{
			this.uri = uri;
			this.connectionLimit = connectionLimit;
			this.maxIdleTime = maxIdleTime;
			currentConnections = 0;
			idleSince = DateTime.Now;
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		internal void SetVersion(Version version)
		{
			protocolVersion = version;
		}

		private System.Net.WebConnectionGroup GetConnectionGroup(string name)
		{
			if (name == null)
			{
				name = string.Empty;
			}
			System.Net.WebConnectionGroup webConnectionGroup = Groups[name] as System.Net.WebConnectionGroup;
			if (webConnectionGroup != null)
			{
				return webConnectionGroup;
			}
			webConnectionGroup = new System.Net.WebConnectionGroup(this, name);
			Groups[name] = webConnectionGroup;
			return webConnectionGroup;
		}

		internal EventHandler SendRequest(HttpWebRequest request, string groupName)
		{
			System.Net.WebConnection connection;
			lock (locker)
			{
				System.Net.WebConnectionGroup connectionGroup = GetConnectionGroup(groupName);
				connection = connectionGroup.GetConnection(request);
			}
			return connection.SendRequest(request);
		}

		public bool CloseConnectionGroup(string connectionGroupName)
		{
			lock (locker)
			{
				System.Net.WebConnectionGroup connectionGroup = GetConnectionGroup(connectionGroupName);
				if (connectionGroup != null)
				{
					connectionGroup.Close();
					return true;
				}
			}
			return false;
		}

		internal void IncrementConnection()
		{
			lock (locker)
			{
				currentConnections++;
				idleSince = DateTime.Now.AddMilliseconds(1000000.0);
			}
		}

		internal void DecrementConnection()
		{
			lock (locker)
			{
				currentConnections--;
				if (currentConnections == 0)
				{
					idleSince = DateTime.Now;
				}
			}
		}

		internal void SetCertificates(X509Certificate client, X509Certificate server)
		{
			certificate = server;
			clientCertificate = client;
		}

		internal bool CallEndPointDelegate(Socket sock, IPEndPoint remote)
		{
			if (endPointCallback == null)
			{
				return true;
			}
			int num = 0;
			while (true)
			{
				IPEndPoint iPEndPoint = null;
				try
				{
					iPEndPoint = endPointCallback(this, remote, num);
				}
				catch
				{
					return false;
				}
				if (iPEndPoint == null)
				{
					return true;
				}
				try
				{
					sock.Bind(iPEndPoint);
				}
				catch (SocketException)
				{
					num = checked(num + 1);
					continue;
				}
				break;
			}
			return true;
		}
	}
}
