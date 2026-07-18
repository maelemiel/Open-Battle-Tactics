using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace System.Net
{
	[Serializable]
	public class WebProxy : ISerializable, IWebProxy
	{
		private Uri address;

		private bool bypassOnLocal;

		private ArrayList bypassList;

		private ICredentials credentials;

		private bool useDefaultCredentials;

		public Uri Address
		{
			get
			{
				return address;
			}
			set
			{
				address = value;
			}
		}

		public ArrayList BypassArrayList
		{
			get
			{
				if (bypassList == null)
				{
					bypassList = new ArrayList();
				}
				return bypassList;
			}
		}

		public string[] BypassList
		{
			get
			{
				return (string[])BypassArrayList.ToArray(typeof(string));
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				bypassList = new ArrayList(value);
				CheckBypassList();
			}
		}

		public bool BypassProxyOnLocal
		{
			get
			{
				return bypassOnLocal;
			}
			set
			{
				bypassOnLocal = value;
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return credentials;
			}
			set
			{
				credentials = value;
			}
		}

		[System.MonoTODO("Does not affect Credentials, since CredentialCache.DefaultCredentials is not implemented.")]
		public bool UseDefaultCredentials
		{
			get
			{
				return useDefaultCredentials;
			}
			set
			{
				useDefaultCredentials = value;
			}
		}

		public WebProxy()
			: this((Uri)null, false, (string[])null, (ICredentials)null)
		{
		}

		public WebProxy(string address)
			: this(ToUri(address), false, null, null)
		{
		}

		public WebProxy(Uri address)
			: this(address, false, null, null)
		{
		}

		public WebProxy(string address, bool bypassOnLocal)
			: this(ToUri(address), bypassOnLocal, null, null)
		{
		}

		public WebProxy(string host, int port)
			: this(new Uri("http://" + host + ":" + port))
		{
		}

		public WebProxy(Uri address, bool bypassOnLocal)
			: this(address, bypassOnLocal, null, null)
		{
		}

		public WebProxy(string address, bool bypassOnLocal, string[] bypassList)
			: this(ToUri(address), bypassOnLocal, bypassList, null)
		{
		}

		public WebProxy(Uri address, bool bypassOnLocal, string[] bypassList)
			: this(address, bypassOnLocal, bypassList, null)
		{
		}

		public WebProxy(string address, bool bypassOnLocal, string[] bypassList, ICredentials credentials)
			: this(ToUri(address), bypassOnLocal, bypassList, credentials)
		{
		}

		public WebProxy(Uri address, bool bypassOnLocal, string[] bypassList, ICredentials credentials)
		{
			this.address = address;
			this.bypassOnLocal = bypassOnLocal;
			if (bypassList != null)
			{
				this.bypassList = new ArrayList(bypassList);
			}
			this.credentials = credentials;
			CheckBypassList();
		}

		protected WebProxy(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			address = (Uri)serializationInfo.GetValue("_ProxyAddress", typeof(Uri));
			bypassOnLocal = serializationInfo.GetBoolean("_BypassOnLocal");
			bypassList = (ArrayList)serializationInfo.GetValue("_BypassList", typeof(ArrayList));
			useDefaultCredentials = serializationInfo.GetBoolean("_UseDefaultCredentials");
			credentials = null;
			CheckBypassList();
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			GetObjectData(serializationInfo, streamingContext);
		}

		[Obsolete("This method has been deprecated", false)]
		[System.MonoTODO("Can we get this info under windows from the system?")]
		public static WebProxy GetDefaultProxy()
		{
			IWebProxy webProxy = GlobalProxySelection.Select;
			if (webProxy is WebProxy)
			{
				return (WebProxy)webProxy;
			}
			return new WebProxy();
		}

		public Uri GetProxy(Uri destination)
		{
			if (IsBypassed(destination))
			{
				return destination;
			}
			return address;
		}

		public bool IsBypassed(Uri host)
		{
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			if (host.IsLoopback && bypassOnLocal)
			{
				return true;
			}
			if (address == null)
			{
				return true;
			}
			string host2 = host.Host;
			if (bypassOnLocal && host2.IndexOf('.') == -1)
			{
				return true;
			}
			if (!bypassOnLocal)
			{
				if (string.Compare(host2, "localhost", true, CultureInfo.InvariantCulture) == 0)
				{
					return true;
				}
				if (string.Compare(host2, "loopback", true, CultureInfo.InvariantCulture) == 0)
				{
					return true;
				}
				IPAddress addr = null;
				if (IPAddress.TryParse(host2, out addr) && IPAddress.IsLoopback(addr))
				{
					return true;
				}
			}
			if (bypassList == null || bypassList.Count == 0)
			{
				return false;
			}
			try
			{
				string input = host.Scheme + "://" + host.Authority;
				int i;
				for (i = 0; i < bypassList.Count; i++)
				{
					Regex regex = new Regex((string)bypassList[i], RegexOptions.IgnoreCase | RegexOptions.Singleline);
					if (regex.IsMatch(input))
					{
						break;
					}
				}
				if (i == bypassList.Count)
				{
					return false;
				}
				for (; i < bypassList.Count; i++)
				{
					new Regex((string)bypassList[i]);
				}
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("_BypassOnLocal", bypassOnLocal);
			serializationInfo.AddValue("_ProxyAddress", address);
			serializationInfo.AddValue("_BypassList", bypassList);
			serializationInfo.AddValue("_UseDefaultCredentials", UseDefaultCredentials);
		}

		private void CheckBypassList()
		{
			if (bypassList != null)
			{
				for (int i = 0; i < bypassList.Count; i++)
				{
					new Regex((string)bypassList[i]);
				}
			}
		}

		private static Uri ToUri(string address)
		{
			if (address == null)
			{
				return null;
			}
			if (address.IndexOf("://") == -1)
			{
				address = "http://" + address;
			}
			return new Uri(address);
		}
	}
}
