using System.Text;

namespace System
{
	public class UriBuilder
	{
		private string scheme;

		private string host;

		private int port;

		private string path;

		private string query;

		private string fragment;

		private string username;

		private string password;

		private Uri uri;

		private bool modified;

		public string Fragment
		{
			get
			{
				return fragment;
			}
			set
			{
				fragment = value;
				if (fragment == null)
				{
					fragment = string.Empty;
				}
				else if (fragment.Length > 0)
				{
					fragment = "#" + value.Replace("%23", "#");
				}
				modified = true;
			}
		}

		public string Host
		{
			get
			{
				return host;
			}
			set
			{
				host = ((value != null) ? value : string.Empty);
				modified = true;
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				password = ((value != null) ? value : string.Empty);
				modified = true;
			}
		}

		public string Path
		{
			get
			{
				return path;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					path = "/";
				}
				else
				{
					path = Uri.EscapeString(value.Replace('\\', '/'), false, true, true);
				}
				modified = true;
			}
		}

		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				port = value;
				modified = true;
			}
		}

		public string Query
		{
			get
			{
				return query;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					query = string.Empty;
				}
				else
				{
					query = "?" + value;
				}
				modified = true;
			}
		}

		public string Scheme
		{
			get
			{
				return scheme;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				int num = value.IndexOf(':');
				if (num != -1)
				{
					value = value.Substring(0, num);
				}
				scheme = value.ToLower();
				modified = true;
			}
		}

		public Uri Uri
		{
			get
			{
				if (!modified)
				{
					return uri;
				}
				uri = new Uri(ToString(), true);
				modified = false;
				return uri;
			}
		}

		public string UserName
		{
			get
			{
				return username;
			}
			set
			{
				username = ((value != null) ? value : string.Empty);
				modified = true;
			}
		}

		public UriBuilder()
			: this(Uri.UriSchemeHttp, "localhost")
		{
		}

		public UriBuilder(string uri)
			: this(new Uri(uri))
		{
		}

		public UriBuilder(Uri uri)
		{
			scheme = uri.Scheme;
			host = uri.Host;
			port = uri.Port;
			path = uri.AbsolutePath;
			query = uri.Query;
			fragment = uri.Fragment;
			username = uri.UserInfo;
			int num = username.IndexOf(':');
			if (num != -1)
			{
				password = username.Substring(num + 1);
				username = username.Substring(0, num);
			}
			else
			{
				password = string.Empty;
			}
			modified = true;
		}

		public UriBuilder(string schemeName, string hostName)
		{
			Scheme = schemeName;
			Host = hostName;
			port = -1;
			Path = string.Empty;
			query = string.Empty;
			fragment = string.Empty;
			username = string.Empty;
			password = string.Empty;
			modified = true;
		}

		public UriBuilder(string scheme, string host, int portNumber)
			: this(scheme, host)
		{
			Port = portNumber;
		}

		public UriBuilder(string scheme, string host, int port, string pathValue)
			: this(scheme, host, port)
		{
			Path = pathValue;
		}

		public UriBuilder(string scheme, string host, int port, string pathValue, string extraValue)
			: this(scheme, host, port, pathValue)
		{
			if (extraValue == null || extraValue.Length == 0)
			{
				return;
			}
			if (extraValue[0] == '#')
			{
				Fragment = extraValue.Remove(0, 1);
				return;
			}
			if (extraValue[0] == '?')
			{
				Query = extraValue.Remove(0, 1);
				return;
			}
			throw new ArgumentException("extraValue");
		}

		public override bool Equals(object rparam)
		{
			return rparam != null && Uri.Equals(rparam.ToString());
		}

		public override int GetHashCode()
		{
			return Uri.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(scheme);
			stringBuilder.Append("://");
			if (username != string.Empty)
			{
				stringBuilder.Append(username);
				if (password != string.Empty)
				{
					stringBuilder.Append(":" + password);
				}
				stringBuilder.Append('@');
			}
			stringBuilder.Append(host);
			if (port > 0)
			{
				stringBuilder.Append(":" + port);
			}
			if (path != string.Empty && stringBuilder[stringBuilder.Length - 1] != '/' && path.Length > 0 && path[0] != '/')
			{
				stringBuilder.Append('/');
			}
			stringBuilder.Append(path);
			stringBuilder.Append(query);
			stringBuilder.Append(fragment);
			return stringBuilder.ToString();
		}
	}
}
