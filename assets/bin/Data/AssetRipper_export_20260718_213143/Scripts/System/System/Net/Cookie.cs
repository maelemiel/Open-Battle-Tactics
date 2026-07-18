using System.Collections;
using System.Globalization;
using System.Text;

namespace System.Net
{
	[Serializable]
	public sealed class Cookie
	{
		private string comment;

		private Uri commentUri;

		private bool discard;

		private string domain;

		private DateTime expires;

		private bool httpOnly;

		private string name;

		private string path;

		private string port;

		private int[] ports;

		private bool secure;

		private DateTime timestamp;

		private string val;

		private int version;

		private static char[] reservedCharsName = new char[7] { ' ', '=', ';', ',', '\n', '\r', '\t' };

		private static char[] portSeparators = new char[2] { '"', ',' };

		private static string tspecials = "()<>@,;:\\\"/[]?={} \t";

		private bool exact_domain;

		public string Comment
		{
			get
			{
				return comment;
			}
			set
			{
				comment = ((value != null) ? value : string.Empty);
			}
		}

		public Uri CommentUri
		{
			get
			{
				return commentUri;
			}
			set
			{
				commentUri = value;
			}
		}

		public bool Discard
		{
			get
			{
				return discard;
			}
			set
			{
				discard = value;
			}
		}

		public string Domain
		{
			get
			{
				return domain;
			}
			set
			{
				if (IsNullOrEmpty(value))
				{
					domain = string.Empty;
					ExactDomain = true;
				}
				else
				{
					domain = value;
					ExactDomain = value[0] != '.';
				}
			}
		}

		internal bool ExactDomain
		{
			get
			{
				return exact_domain;
			}
			set
			{
				exact_domain = value;
			}
		}

		public bool Expired
		{
			get
			{
				return expires <= DateTime.Now && expires != DateTime.MinValue;
			}
			set
			{
				if (value)
				{
					expires = DateTime.Now;
				}
			}
		}

		public DateTime Expires
		{
			get
			{
				return expires;
			}
			set
			{
				expires = value;
			}
		}

		public bool HttpOnly
		{
			get
			{
				return httpOnly;
			}
			set
			{
				httpOnly = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (IsNullOrEmpty(value))
				{
					throw new CookieException("Name cannot be empty");
				}
				if (value[0] == '$' || value.IndexOfAny(reservedCharsName) != -1)
				{
					name = string.Empty;
					throw new CookieException("Name contains invalid characters");
				}
				name = value;
			}
		}

		public string Path
		{
			get
			{
				return (path != null) ? path : string.Empty;
			}
			set
			{
				path = ((value != null) ? value : string.Empty);
			}
		}

		public string Port
		{
			get
			{
				return port;
			}
			set
			{
				if (IsNullOrEmpty(value))
				{
					port = string.Empty;
					return;
				}
				if (value[0] != '"' || value[value.Length - 1] != '"')
				{
					throw new CookieException("The 'Port'='" + value + "' part of the cookie is invalid. Port must be enclosed by double quotes.");
				}
				port = value;
				string[] array = port.Split(portSeparators);
				ports = new int[array.Length];
				for (int i = 0; i < ports.Length; i++)
				{
					ports[i] = int.MinValue;
					if (array[i].Length != 0)
					{
						try
						{
							ports[i] = int.Parse(array[i]);
						}
						catch (Exception e)
						{
							throw new CookieException("The 'Port'='" + value + "' part of the cookie is invalid. Invalid value: " + array[i], e);
						}
					}
				}
				Version = 1;
			}
		}

		internal int[] Ports
		{
			get
			{
				return ports;
			}
		}

		public bool Secure
		{
			get
			{
				return secure;
			}
			set
			{
				secure = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return timestamp;
			}
		}

		public string Value
		{
			get
			{
				return val;
			}
			set
			{
				if (value == null)
				{
					val = string.Empty;
				}
				else
				{
					val = value;
				}
			}
		}

		public int Version
		{
			get
			{
				return version;
			}
			set
			{
				if (value < 0 || value > 10)
				{
					version = 0;
				}
				else
				{
					version = value;
				}
			}
		}

		public Cookie()
		{
			expires = DateTime.MinValue;
			timestamp = DateTime.Now;
			domain = string.Empty;
			name = string.Empty;
			val = string.Empty;
			comment = string.Empty;
			port = string.Empty;
		}

		public Cookie(string name, string value)
			: this()
		{
			Name = name;
			Value = value;
		}

		public Cookie(string name, string value, string path)
			: this(name, value)
		{
			Path = path;
		}

		public Cookie(string name, string value, string path, string domain)
			: this(name, value, path)
		{
			Domain = domain;
		}

		public override bool Equals(object obj)
		{
			Cookie cookie = obj as Cookie;
			return cookie != null && string.Compare(name, cookie.name, true, CultureInfo.InvariantCulture) == 0 && string.Compare(val, cookie.val, false, CultureInfo.InvariantCulture) == 0 && string.Compare(Path, cookie.Path, false, CultureInfo.InvariantCulture) == 0 && string.Compare(domain, cookie.domain, true, CultureInfo.InvariantCulture) == 0 && version == cookie.version;
		}

		public override int GetHashCode()
		{
			return hash(CaseInsensitiveHashCodeProvider.DefaultInvariant.GetHashCode(name), val.GetHashCode(), Path.GetHashCode(), CaseInsensitiveHashCodeProvider.DefaultInvariant.GetHashCode(domain), version);
		}

		private static int hash(int i, int j, int k, int l, int m)
		{
			return i ^ ((j << 13) | (j >> 19)) ^ ((k << 26) | (k >> 6)) ^ ((l << 7) | (l >> 25)) ^ ((m << 20) | (m >> 12));
		}

		public override string ToString()
		{
			return ToString(null);
		}

		internal string ToString(Uri uri)
		{
			if (name.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			if (version > 0)
			{
				stringBuilder.Append("$Version=").Append(version).Append("; ");
			}
			stringBuilder.Append(name).Append("=").Append(val);
			if (version == 0)
			{
				return stringBuilder.ToString();
			}
			if (!IsNullOrEmpty(path))
			{
				stringBuilder.Append("; $Path=").Append(path);
			}
			else if (uri != null)
			{
				stringBuilder.Append("; $Path=/").Append(path);
			}
			if ((uri == null || uri.Host != domain) && !IsNullOrEmpty(domain))
			{
				stringBuilder.Append("; $Domain=").Append(domain);
			}
			if (port != null && port.Length != 0)
			{
				stringBuilder.Append("; $Port=").Append(port);
			}
			return stringBuilder.ToString();
		}

		internal string ToClientString()
		{
			if (name.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			if (version > 0)
			{
				stringBuilder.Append("Version=").Append(version).Append(";");
			}
			stringBuilder.Append(name).Append("=").Append(val);
			if (path != null && path.Length != 0)
			{
				stringBuilder.Append(";Path=").Append(QuotedString(path));
			}
			if (domain != null && domain.Length != 0)
			{
				stringBuilder.Append(";Domain=").Append(QuotedString(domain));
			}
			if (port != null && port.Length != 0)
			{
				stringBuilder.Append(";Port=").Append(port);
			}
			return stringBuilder.ToString();
		}

		private string QuotedString(string value)
		{
			if (version == 0 || IsToken(value))
			{
				return value;
			}
			return "\"" + value.Replace("\"", "\\\"") + "\"";
		}

		private bool IsToken(string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				if (c < ' ' || c >= '\u007f' || tspecials.IndexOf(c) != -1)
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsNullOrEmpty(string s)
		{
			return s == null || s.Length == 0;
		}
	}
}
