using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace System.Net
{
	public sealed class HttpListenerRequest
	{
		private string[] accept_types;

		private Encoding content_encoding;

		private long content_length;

		private bool cl_set;

		private CookieCollection cookies;

		private WebHeaderCollection headers;

		private string method;

		private Stream input_stream;

		private Version version;

		private NameValueCollection query_string;

		private string raw_url;

		private Guid identifier;

		private Uri url;

		private Uri referrer;

		private string[] user_languages;

		private HttpListenerContext context;

		private bool is_chunked;

		private static byte[] _100continue = Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");

		private static readonly string[] no_body_methods = new string[3] { "GET", "HEAD", "DELETE" };

		private static char[] separators = new char[1] { ' ' };

		public string[] AcceptTypes
		{
			get
			{
				return accept_types;
			}
		}

		[System.MonoTODO("Always returns 0")]
		public int ClientCertificateError
		{
			get
			{
				return 0;
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				if (content_encoding == null)
				{
					content_encoding = Encoding.Default;
				}
				return content_encoding;
			}
		}

		public long ContentLength64
		{
			get
			{
				return content_length;
			}
		}

		public string ContentType
		{
			get
			{
				return headers["content-type"];
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				if (cookies == null)
				{
					cookies = new CookieCollection();
				}
				return cookies;
			}
		}

		public bool HasEntityBody
		{
			get
			{
				return content_length > 0 || is_chunked;
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return headers;
			}
		}

		public string HttpMethod
		{
			get
			{
				return method;
			}
		}

		public Stream InputStream
		{
			get
			{
				return input_stream;
			}
		}

		[System.MonoTODO("Always returns false")]
		public bool IsAuthenticated
		{
			get
			{
				return false;
			}
		}

		public bool IsLocal
		{
			get
			{
				return IPAddress.IsLoopback(RemoteEndPoint.Address);
			}
		}

		public bool IsSecureConnection
		{
			get
			{
				return context.Connection.IsSecure;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return false;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return context.Connection.LocalEndPoint;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return version;
			}
		}

		public NameValueCollection QueryString
		{
			get
			{
				return query_string;
			}
		}

		public string RawUrl
		{
			get
			{
				return raw_url;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return context.Connection.RemoteEndPoint;
			}
		}

		public Guid RequestTraceIdentifier
		{
			get
			{
				return identifier;
			}
		}

		public Uri Url
		{
			get
			{
				return url;
			}
		}

		public Uri UrlReferrer
		{
			get
			{
				return referrer;
			}
		}

		public string UserAgent
		{
			get
			{
				return headers["user-agent"];
			}
		}

		public string UserHostAddress
		{
			get
			{
				return LocalEndPoint.ToString();
			}
		}

		public string UserHostName
		{
			get
			{
				return headers["host"];
			}
		}

		public string[] UserLanguages
		{
			get
			{
				return user_languages;
			}
		}

		internal HttpListenerRequest(HttpListenerContext context)
		{
			this.context = context;
			headers = new WebHeaderCollection();
			input_stream = Stream.Null;
			version = HttpVersion.Version10;
		}

		internal void SetRequestLine(string req)
		{
			string[] array = req.Split(separators, 3);
			if (array.Length != 3)
			{
				context.ErrorMessage = "Invalid request line (parts).";
				return;
			}
			method = array[0];
			string text = method;
			foreach (char c in text)
			{
				int num = c;
				if ((num < 65 || num > 90) && (num <= 32 || c >= '\u007f' || c == '(' || c == ')' || c == '<' || c == '<' || c == '>' || c == '@' || c == ',' || c == ';' || c == ':' || c == '\\' || c == '"' || c == '/' || c == '[' || c == ']' || c == '?' || c == '=' || c == '{' || c == '}'))
				{
					context.ErrorMessage = "(Invalid verb)";
					return;
				}
			}
			raw_url = array[1];
			if (array[2].Length != 8 || !array[2].StartsWith("HTTP/"))
			{
				context.ErrorMessage = "Invalid request line (version).";
				return;
			}
			try
			{
				version = new Version(array[2].Substring(5));
				if (version.Major < 1)
				{
					throw new Exception();
				}
			}
			catch
			{
				context.ErrorMessage = "Invalid request line (version).";
			}
		}

		private void CreateQueryString(string query)
		{
			query_string = new NameValueCollection();
			if (query == null || query.Length == 0)
			{
				return;
			}
			if (query[0] == '?')
			{
				query = query.Substring(1);
			}
			string[] array = query.Split('&');
			string[] array2 = array;
			foreach (string text in array2)
			{
				int num = text.IndexOf('=');
				if (num == -1)
				{
					query_string.Add(null, System.Net.HttpUtility.UrlDecode(text));
					continue;
				}
				string name = System.Net.HttpUtility.UrlDecode(text.Substring(0, num));
				string val = System.Net.HttpUtility.UrlDecode(text.Substring(num + 1));
				query_string.Add(name, val);
			}
		}

		internal void FinishInitialization()
		{
			string text = UserHostName;
			if (version > HttpVersion.Version10 && (text == null || text.Length == 0))
			{
				context.ErrorMessage = "Invalid host name";
				return;
			}
			Uri result = default(Uri);
			string text2 = ((!Uri.MaybeUri(raw_url) || !Uri.TryCreate(raw_url, UriKind.Absolute, out result)) ? raw_url : result.PathAndQuery);
			if (text == null || text.Length == 0)
			{
				text = UserHostAddress;
			}
			if (result != null)
			{
				text = result.Host;
			}
			int num = text.IndexOf(':');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			string text3 = string.Format("{0}://{1}:{2}", (!IsSecureConnection) ? "http" : "https", text, LocalEndPoint.Port);
			if (!Uri.TryCreate(text3 + text2, UriKind.Absolute, out url))
			{
				context.ErrorMessage = "Invalid url: " + text3 + text2;
				return;
			}
			CreateQueryString(url.Query);
			string text4 = null;
			if (version >= HttpVersion.Version11)
			{
				text4 = Headers["Transfer-Encoding"];
				if (text4 != null && text4 != "chunked")
				{
					context.Connection.SendError(null, 501);
					return;
				}
			}
			is_chunked = text4 == "chunked";
			string[] array = no_body_methods;
			foreach (string strB in array)
			{
				if (string.Compare(method, strB, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return;
				}
			}
			if (!is_chunked && !cl_set)
			{
				context.Connection.SendError(null, 411);
				return;
			}
			if (is_chunked || content_length > 0)
			{
				input_stream = context.Connection.GetRequestStream(is_chunked, content_length);
			}
			if (Headers["Expect"] == "100-continue")
			{
				System.Net.ResponseStream responseStream = context.Connection.GetResponseStream();
				responseStream.InternalWrite(_100continue, 0, _100continue.Length);
			}
		}

		internal static string Unquote(string str)
		{
			int num = str.IndexOf('"');
			int num2 = str.LastIndexOf('"');
			if (num >= 0 && num2 >= 0)
			{
				str = str.Substring(num + 1, num2 - 1);
			}
			return str.Trim();
		}

		internal void AddHeader(string header)
		{
			int num = header.IndexOf(':');
			if (num == -1 || num == 0)
			{
				context.ErrorMessage = "Bad Request";
				context.ErrorStatus = 400;
				return;
			}
			string text = header.Substring(0, num).Trim();
			string text2 = header.Substring(num + 1).Trim();
			string text3 = text.ToLower(CultureInfo.InvariantCulture);
			headers.SetInternal(text, text2);
			switch (text3)
			{
			case "accept-language":
				user_languages = text2.Split(',');
				break;
			case "accept":
				accept_types = text2.Split(',');
				break;
			case "content-length":
				try
				{
					content_length = long.Parse(text2.Trim());
					if (content_length < 0)
					{
						context.ErrorMessage = "Invalid Content-Length.";
					}
					cl_set = true;
					break;
				}
				catch
				{
					context.ErrorMessage = "Invalid Content-Length.";
					break;
				}
			case "referer":
				try
				{
					referrer = new Uri(text2);
					break;
				}
				catch
				{
					referrer = new Uri("http://someone.is.screwing.with.the.headers.com/");
					break;
				}
			case "cookie":
			{
				if (cookies == null)
				{
					cookies = new CookieCollection();
				}
				string[] array = text2.Split(',', ';');
				Cookie cookie = null;
				int num2 = 0;
				string[] array2 = array;
				foreach (string text4 in array2)
				{
					string text5 = text4.Trim();
					if (text5.Length == 0)
					{
						continue;
					}
					if (text5.StartsWith("$Version"))
					{
						num2 = int.Parse(Unquote(text5.Substring(text5.IndexOf("=") + 1)));
						continue;
					}
					if (text5.StartsWith("$Path"))
					{
						if (cookie != null)
						{
							cookie.Path = text5.Substring(text5.IndexOf("=") + 1).Trim();
						}
						continue;
					}
					if (text5.StartsWith("$Domain"))
					{
						if (cookie != null)
						{
							cookie.Domain = text5.Substring(text5.IndexOf("=") + 1).Trim();
						}
						continue;
					}
					if (text5.StartsWith("$Port"))
					{
						if (cookie != null)
						{
							cookie.Port = text5.Substring(text5.IndexOf("=") + 1).Trim();
						}
						continue;
					}
					if (cookie != null)
					{
						cookies.Add(cookie);
					}
					cookie = new Cookie();
					int num3 = text5.IndexOf("=");
					if (num3 > 0)
					{
						cookie.Name = text5.Substring(0, num3).Trim();
						cookie.Value = text5.Substring(num3 + 1).Trim();
					}
					else
					{
						cookie.Name = text5.Trim();
						cookie.Value = string.Empty;
					}
					cookie.Version = num2;
				}
				if (cookie != null)
				{
					cookies.Add(cookie);
				}
				break;
			}
			}
		}

		internal bool FlushInput()
		{
			if (!HasEntityBody)
			{
				return true;
			}
			int num = 2048;
			if (content_length > 0)
			{
				num = (int)Math.Min(content_length, num);
			}
			byte[] buffer = new byte[num];
			while (true)
			{
				try
				{
					if (InputStream.Read(buffer, 0, num) <= 0)
					{
						return true;
					}
				}
				catch
				{
					return false;
				}
			}
		}

		public IAsyncResult BeginGetClientCertificate(AsyncCallback requestCallback, object state)
		{
			return null;
		}

		public X509Certificate2 EndGetClientCertificate(IAsyncResult asyncResult)
		{
			return null;
		}

		public X509Certificate2 GetClientCertificate()
		{
			return null;
		}
	}
}
