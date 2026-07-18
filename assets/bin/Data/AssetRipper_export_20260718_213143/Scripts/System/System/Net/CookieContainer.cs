using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Net
{
	[Serializable]
	public sealed class CookieContainer
	{
		public const int DefaultCookieLengthLimit = 4096;

		public const int DefaultCookieLimit = 300;

		public const int DefaultPerDomainCookieLimit = 20;

		private int capacity = 300;

		private int perDomainCapacity = 20;

		private int maxCookieSize = 4096;

		private CookieCollection cookies;

		public int Count
		{
			get
			{
				return (cookies != null) ? cookies.Count : 0;
			}
		}

		public int Capacity
		{
			get
			{
				return capacity;
			}
			set
			{
				if (value < 0 || (value < perDomainCapacity && perDomainCapacity != int.MaxValue))
				{
					throw new ArgumentOutOfRangeException("value", string.Format("Capacity must be greater than {0} and less than {1}.", 0, perDomainCapacity));
				}
				capacity = value;
			}
		}

		public int MaxCookieSize
		{
			get
			{
				return maxCookieSize;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				maxCookieSize = value;
			}
		}

		public int PerDomainCapacity
		{
			get
			{
				return perDomainCapacity;
			}
			set
			{
				if (value != int.MaxValue && (value <= 0 || value > capacity))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				perDomainCapacity = value;
			}
		}

		public CookieContainer()
		{
		}

		public CookieContainer(int capacity)
		{
			if (capacity <= 0)
			{
				throw new ArgumentException("Must be greater than zero", "Capacity");
			}
			this.capacity = capacity;
		}

		public CookieContainer(int capacity, int perDomainCapacity, int maxCookieSize)
			: this(capacity)
		{
			if (perDomainCapacity != int.MaxValue && (perDomainCapacity <= 0 || perDomainCapacity > capacity))
			{
				throw new ArgumentOutOfRangeException("perDomainCapacity", string.Format("PerDomainCapacity must be greater than {0} and less than {1}.", 0, capacity));
			}
			if (maxCookieSize <= 0)
			{
				throw new ArgumentException("Must be greater than zero", "MaxCookieSize");
			}
			this.perDomainCapacity = perDomainCapacity;
			this.maxCookieSize = maxCookieSize;
		}

		public void Add(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			if (cookie.Domain.Length == 0)
			{
				throw new ArgumentException("Cookie domain not set.", "cookie.Domain");
			}
			if (cookie.Value.Length > maxCookieSize)
			{
				throw new CookieException("value is larger than MaxCookieSize.");
			}
			Cookie cookie2 = new Cookie(cookie.Name, cookie.Value);
			cookie2.Path = ((cookie.Path.Length != 0) ? cookie.Path : "/");
			cookie2.Domain = cookie.Domain;
			cookie2.ExactDomain = cookie.ExactDomain;
			cookie2.Version = cookie.Version;
			AddCookie(cookie2);
		}

		private void AddCookie(Cookie cookie)
		{
			if (cookies == null)
			{
				cookies = new CookieCollection();
			}
			if (cookies.Count >= capacity)
			{
				RemoveOldest(null);
			}
			if (cookies.Count >= perDomainCapacity && CountDomain(cookie.Domain) >= perDomainCapacity)
			{
				RemoveOldest(cookie.Domain);
			}
			Cookie cookie2 = new Cookie(cookie.Name, cookie.Value);
			cookie2.Path = ((cookie.Path.Length != 0) ? cookie.Path : "/");
			cookie2.Domain = cookie.Domain;
			cookie2.ExactDomain = cookie.ExactDomain;
			cookie2.Version = cookie.Version;
			cookie2.Expires = cookie.Expires;
			cookie2.CommentUri = cookie.CommentUri;
			cookie2.Comment = cookie.Comment;
			cookie2.Discard = cookie.Discard;
			cookie2.HttpOnly = cookie.HttpOnly;
			cookie2.Secure = cookie.Secure;
			cookies.Add(cookie2);
			CheckExpiration();
		}

		private int CountDomain(string domain)
		{
			int num = 0;
			foreach (Cookie cookie in cookies)
			{
				if (CheckDomain(domain, cookie.Domain, true))
				{
					num++;
				}
			}
			return num;
		}

		private void RemoveOldest(string domain)
		{
			int index = 0;
			DateTime dateTime = DateTime.MaxValue;
			for (int i = 0; i < cookies.Count; i++)
			{
				Cookie cookie = cookies[i];
				if (cookie.TimeStamp < dateTime && (domain == null || domain == cookie.Domain))
				{
					dateTime = cookie.TimeStamp;
					index = i;
				}
			}
			cookies.List.RemoveAt(index);
		}

		private void CheckExpiration()
		{
			if (cookies == null)
			{
				return;
			}
			for (int num = cookies.Count - 1; num >= 0; num--)
			{
				Cookie cookie = cookies[num];
				if (cookie.Expired)
				{
					cookies.List.RemoveAt(num);
				}
			}
		}

		public void Add(CookieCollection cookies)
		{
			if (cookies == null)
			{
				throw new ArgumentNullException("cookies");
			}
			foreach (Cookie cookie in cookies)
			{
				Add(cookie);
			}
		}

		private void Cook(Uri uri, Cookie cookie)
		{
			if (IsNullOrEmpty(cookie.Name))
			{
				throw new CookieException("Invalid cookie: name");
			}
			if (cookie.Value == null)
			{
				throw new CookieException("Invalid cookie: value");
			}
			if (uri != null && cookie.Domain.Length == 0)
			{
				cookie.Domain = uri.Host;
			}
			if (cookie.Version == 0 && IsNullOrEmpty(cookie.Path))
			{
				if (uri != null)
				{
					cookie.Path = uri.AbsolutePath;
				}
				else
				{
					cookie.Path = "/";
				}
			}
			if (cookie.Port.Length == 0 && uri != null && !uri.IsDefaultPort)
			{
				cookie.Port = "\"" + uri.Port + "\"";
			}
		}

		public void Add(Uri uri, Cookie cookie)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			if (!cookie.Expired)
			{
				Cook(uri, cookie);
				AddCookie(cookie);
			}
		}

		public void Add(Uri uri, CookieCollection cookies)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (cookies == null)
			{
				throw new ArgumentNullException("cookies");
			}
			foreach (Cookie cookie in cookies)
			{
				if (!cookie.Expired)
				{
					Cook(uri, cookie);
					AddCookie(cookie);
				}
			}
		}

		public string GetCookieHeader(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			CookieCollection cookieCollection = GetCookies(uri);
			if (cookieCollection.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Cookie item in cookieCollection)
			{
				stringBuilder.Append(item.ToString(uri));
				stringBuilder.Append("; ");
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Length -= 2;
			}
			return stringBuilder.ToString();
		}

		private static bool CheckDomain(string domain, string host, bool exact)
		{
			if (domain.Length == 0)
			{
				return false;
			}
			if (exact)
			{
				return string.Compare(host, domain, true, CultureInfo.InvariantCulture) == 0;
			}
			if (!CultureInfo.InvariantCulture.CompareInfo.IsSuffix(host, domain, CompareOptions.IgnoreCase))
			{
				return false;
			}
			if (domain[0] == '.')
			{
				return true;
			}
			int num = host.Length - domain.Length - 1;
			if (num < 0)
			{
				return false;
			}
			return host[num] == '.';
		}

		public CookieCollection GetCookies(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			CheckExpiration();
			CookieCollection cookieCollection = new CookieCollection();
			if (cookies == null)
			{
				return cookieCollection;
			}
			foreach (Cookie cookie in cookies)
			{
				string domain = cookie.Domain;
				if (CheckDomain(domain, uri.Host, cookie.ExactDomain) && (cookie.Port.Length <= 0 || cookie.Ports == null || uri.Port == -1 || Array.IndexOf(cookie.Ports, uri.Port) != -1))
				{
					string path = cookie.Path;
					string absolutePath = uri.AbsolutePath;
					if ((!(path != string.Empty) || !(path != "/") || !(absolutePath != path) || (absolutePath.StartsWith(path) && (path[path.Length - 1] == '/' || absolutePath.Length <= path.Length || absolutePath[path.Length] == '/'))) && (!cookie.Secure || !(uri.Scheme != "https")))
					{
						cookieCollection.Add(cookie);
					}
				}
			}
			cookieCollection.Sort();
			return cookieCollection;
		}

		public void SetCookies(Uri uri, string cookieHeader)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (cookieHeader == null)
			{
				throw new ArgumentNullException("cookieHeader");
			}
			if (cookieHeader.Length == 0)
			{
				return;
			}
			string[] array = cookieHeader.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (array.Length > i + 1 && Regex.IsMatch(array[i], ".*expires\\s*=\\s*(Mon|Tue|Wed|Thu|Fri|Sat|Sun)", RegexOptions.IgnoreCase) && Regex.IsMatch(array[i + 1], "\\s\\d{2}-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)-\\d{4} \\d{2}:\\d{2}:\\d{2} GMT", RegexOptions.IgnoreCase))
				{
					text = new StringBuilder(text).Append(",").Append(array[++i]).ToString();
				}
				try
				{
					Cookie cookie = Parse(text);
					if (cookie.Path.Length == 0)
					{
						cookie.Path = uri.AbsolutePath;
					}
					else if (!uri.AbsolutePath.StartsWith(cookie.Path))
					{
						string msg = string.Format("'Path'='{0}' is invalid with URI", cookie.Path);
						throw new CookieException(msg);
					}
					if (cookie.Domain.Length == 0)
					{
						cookie.Domain = uri.Host;
						cookie.ExactDomain = true;
					}
					AddCookie(cookie);
				}
				catch (Exception e)
				{
					string msg2 = string.Format("Could not parse cookies for '{0}'.", uri);
					throw new CookieException(msg2, e);
				}
			}
		}

		private static Cookie Parse(string s)
		{
			string[] array = s.Split(';');
			Cookie cookie = new Cookie();
			for (int i = 0; i < array.Length; i++)
			{
				int num = array[i].IndexOf('=');
				string text;
				string text2;
				if (num == -1)
				{
					text = array[i].Trim();
					text2 = string.Empty;
				}
				else
				{
					text = array[i].Substring(0, num).Trim();
					text2 = array[i].Substring(num + 1).Trim();
				}
				switch (text.ToLower(CultureInfo.InvariantCulture))
				{
				case "path":
				case "$path":
					if (cookie.Path.Length == 0)
					{
						cookie.Path = text2;
					}
					break;
				case "domain":
				case "$domain":
					if (cookie.Domain.Length == 0)
					{
						cookie.Domain = text2;
						cookie.ExactDomain = false;
					}
					break;
				case "expires":
				case "$expires":
					if (cookie.Expires == DateTime.MinValue)
					{
						cookie.Expires = DateTime.SpecifyKind(DateTime.ParseExact(text2, "ddd, dd-MMM-yyyy HH:mm:ss G\\MT", CultureInfo.InvariantCulture), DateTimeKind.Utc);
					}
					break;
				case "httponly":
					cookie.HttpOnly = true;
					break;
				case "secure":
					cookie.Secure = true;
					break;
				default:
					if (cookie.Name.Length == 0)
					{
						cookie.Name = text;
						cookie.Value = text2;
					}
					break;
				}
			}
			return cookie;
		}

		private static bool IsNullOrEmpty(string s)
		{
			return s == null || s.Length == 0;
		}
	}
}
