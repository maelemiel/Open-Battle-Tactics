namespace System.Net
{
	internal sealed class ListenerPrefix
	{
		private string original;

		private string host;

		private ushort port;

		private string path;

		private bool secure;

		private IPAddress[] addresses;

		public HttpListener Listener;

		public IPAddress[] Addresses
		{
			get
			{
				return addresses;
			}
			set
			{
				addresses = value;
			}
		}

		public bool Secure
		{
			get
			{
				return secure;
			}
		}

		public string Host
		{
			get
			{
				return host;
			}
		}

		public int Port
		{
			get
			{
				return port;
			}
		}

		public string Path
		{
			get
			{
				return path;
			}
		}

		public ListenerPrefix(string prefix)
		{
			original = prefix;
			Parse(prefix);
		}

		public override string ToString()
		{
			return original;
		}

		public override bool Equals(object o)
		{
			System.Net.ListenerPrefix listenerPrefix = o as System.Net.ListenerPrefix;
			if (listenerPrefix == null)
			{
				return false;
			}
			return original == listenerPrefix.original;
		}

		public override int GetHashCode()
		{
			return original.GetHashCode();
		}

		private void Parse(string uri)
		{
			int num = ((!uri.StartsWith("http://")) ? (-1) : 80);
			if (num == -1)
			{
				num = ((!uri.StartsWith("https://")) ? (-1) : 443);
				secure = true;
			}
			int length = uri.Length;
			int num2 = uri.IndexOf(':') + 3;
			if (num2 >= length)
			{
				throw new ArgumentException("No host specified.");
			}
			int num3 = uri.IndexOf(':', num2, length - num2);
			if (num3 > 0)
			{
				host = uri.Substring(num2, num3 - num2);
				int num4 = uri.IndexOf('/', num3, length - num3);
				port = (ushort)int.Parse(uri.Substring(num3 + 1, num4 - num3 - 1));
				path = uri.Substring(num4);
			}
			else
			{
				int num4 = uri.IndexOf('/', num2, length - num2);
				host = uri.Substring(num2, num4 - num2);
				path = uri.Substring(num4);
			}
		}

		public static void CheckUri(string uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			int num = ((!uri.StartsWith("http://")) ? (-1) : 80);
			if (num == -1)
			{
				num = ((!uri.StartsWith("https://")) ? (-1) : 443);
			}
			if (num == -1)
			{
				throw new ArgumentException("Only 'http' and 'https' schemes are supported.");
			}
			int length = uri.Length;
			int num2 = uri.IndexOf(':') + 3;
			if (num2 >= length)
			{
				throw new ArgumentException("No host specified.");
			}
			int num3 = uri.IndexOf(':', num2, length - num2);
			if (num2 == num3)
			{
				throw new ArgumentException("No host specified.");
			}
			if (num3 > 0)
			{
				int num4 = uri.IndexOf('/', num3, length - num3);
				if (num4 == -1)
				{
					throw new ArgumentException("No path specified.");
				}
				try
				{
					int num5 = int.Parse(uri.Substring(num3 + 1, num4 - num3 - 1));
					if (num5 <= 0 || num5 >= 65536)
					{
						throw new Exception();
					}
				}
				catch
				{
					throw new ArgumentException("Invalid port.");
				}
			}
			else
			{
				int num4 = uri.IndexOf('/', num2, length - num2);
				if (num4 == -1)
				{
					throw new ArgumentException("No path specified.");
				}
			}
			if (uri[uri.Length - 1] != '/')
			{
				throw new ArgumentException("The prefix must end with '/'");
			}
		}
	}
}
