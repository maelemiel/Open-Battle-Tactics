using System.Globalization;
using System.IO;
using System.Text;

namespace System.Net
{
	public sealed class HttpListenerResponse : IDisposable
	{
		private bool disposed;

		private Encoding content_encoding;

		private long content_length;

		private bool cl_set;

		private string content_type;

		private CookieCollection cookies;

		private WebHeaderCollection headers = new WebHeaderCollection();

		private bool keep_alive = true;

		private System.Net.ResponseStream output_stream;

		private Version version = HttpVersion.Version11;

		private string location;

		private int status_code = 200;

		private string status_description = "OK";

		private bool chunked;

		private HttpListenerContext context;

		internal bool HeadersSent;

		private bool force_close_chunked;

		internal bool ForceCloseChunked
		{
			get
			{
				return force_close_chunked;
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
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				content_encoding = value;
			}
		}

		public long ContentLength64
		{
			get
			{
				return content_length;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Must be >= 0", "value");
				}
				cl_set = true;
				content_length = value;
			}
		}

		public string ContentType
		{
			get
			{
				return content_type;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				content_type = value;
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
			set
			{
				cookies = value;
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				return headers;
			}
			set
			{
				headers = value;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return keep_alive;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				keep_alive = value;
			}
		}

		public Stream OutputStream
		{
			get
			{
				if (output_stream == null)
				{
					output_stream = context.Connection.GetResponseStream();
				}
				return output_stream;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return version;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Major != 1 || (value.Minor != 0 && value.Minor != 1))
				{
					throw new ArgumentException("Must be 1.0 or 1.1", "value");
				}
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				version = value;
			}
		}

		public string RedirectLocation
		{
			get
			{
				return location;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				location = value;
			}
		}

		public bool SendChunked
		{
			get
			{
				return chunked;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				chunked = value;
			}
		}

		public int StatusCode
		{
			get
			{
				return status_code;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(GetType().ToString());
				}
				if (HeadersSent)
				{
					throw new InvalidOperationException("Cannot be changed after headers are sent.");
				}
				if (value < 100 || value > 999)
				{
					throw new ProtocolViolationException("StatusCode must be between 100 and 999.");
				}
				status_code = value;
				status_description = GetStatusDescription(value);
			}
		}

		public string StatusDescription
		{
			get
			{
				return status_description;
			}
			set
			{
				status_description = value;
			}
		}

		internal HttpListenerResponse(HttpListenerContext context)
		{
			this.context = context;
		}

		void IDisposable.Dispose()
		{
			Close(true);
		}

		internal static string GetStatusDescription(int code)
		{
			switch (code)
			{
			case 100:
				return "Continue";
			case 101:
				return "Switching Protocols";
			case 102:
				return "Processing";
			case 200:
				return "OK";
			case 201:
				return "Created";
			case 202:
				return "Accepted";
			case 203:
				return "Non-Authoritative Information";
			case 204:
				return "No Content";
			case 205:
				return "Reset Content";
			case 206:
				return "Partial Content";
			case 207:
				return "Multi-Status";
			case 300:
				return "Multiple Choices";
			case 301:
				return "Moved Permanently";
			case 302:
				return "Found";
			case 303:
				return "See Other";
			case 304:
				return "Not Modified";
			case 305:
				return "Use Proxy";
			case 307:
				return "Temporary Redirect";
			case 400:
				return "Bad Request";
			case 401:
				return "Unauthorized";
			case 402:
				return "Payment Required";
			case 403:
				return "Forbidden";
			case 404:
				return "Not Found";
			case 405:
				return "Method Not Allowed";
			case 406:
				return "Not Acceptable";
			case 407:
				return "Proxy Authentication Required";
			case 408:
				return "Request Timeout";
			case 409:
				return "Conflict";
			case 410:
				return "Gone";
			case 411:
				return "Length Required";
			case 412:
				return "Precondition Failed";
			case 413:
				return "Request Entity Too Large";
			case 414:
				return "Request-Uri Too Long";
			case 415:
				return "Unsupported Media Type";
			case 416:
				return "Requested Range Not Satisfiable";
			case 417:
				return "Expectation Failed";
			case 422:
				return "Unprocessable Entity";
			case 423:
				return "Locked";
			case 424:
				return "Failed Dependency";
			case 500:
				return "Internal Server Error";
			case 501:
				return "Not Implemented";
			case 502:
				return "Bad Gateway";
			case 503:
				return "Service Unavailable";
			case 504:
				return "Gateway Timeout";
			case 505:
				return "Http Version Not Supported";
			case 507:
				return "Insufficient Storage";
			default:
				return string.Empty;
			}
		}

		public void Abort()
		{
			if (!disposed)
			{
				Close(true);
			}
		}

		public void AddHeader(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name == string.Empty)
			{
				throw new ArgumentException("'name' cannot be empty", "name");
			}
			if (value.Length > 65535)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			headers.Set(name, value);
		}

		public void AppendCookie(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			Cookies.Add(cookie);
		}

		public void AppendHeader(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name == string.Empty)
			{
				throw new ArgumentException("'name' cannot be empty", "name");
			}
			if (value.Length > 65535)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			headers.Add(name, value);
		}

		private void Close(bool force)
		{
			disposed = true;
			context.Connection.Close(force);
		}

		public void Close()
		{
			if (!disposed)
			{
				Close(false);
			}
		}

		public void Close(byte[] responseEntity, bool willBlock)
		{
			if (!disposed)
			{
				if (responseEntity == null)
				{
					throw new ArgumentNullException("responseEntity");
				}
				ContentLength64 = responseEntity.Length;
				OutputStream.Write(responseEntity, 0, (int)content_length);
				Close(false);
			}
		}

		public void CopyFrom(HttpListenerResponse templateResponse)
		{
			headers.Clear();
			headers.Add(templateResponse.headers);
			content_length = templateResponse.content_length;
			status_code = templateResponse.status_code;
			status_description = templateResponse.status_description;
			keep_alive = templateResponse.keep_alive;
			version = templateResponse.version;
		}

		public void Redirect(string url)
		{
			StatusCode = 302;
			location = url;
		}

		private bool FindCookie(Cookie cookie)
		{
			string name = cookie.Name;
			string domain = cookie.Domain;
			string path = cookie.Path;
			foreach (Cookie cookie2 in cookies)
			{
				if (name != cookie2.Name || domain != cookie2.Domain || !(path == cookie2.Path))
				{
					continue;
				}
				return true;
			}
			return false;
		}

		internal void SendHeaders(bool closing, MemoryStream ms)
		{
			Encoding encoding = content_encoding;
			if (encoding == null)
			{
				encoding = Encoding.Default;
			}
			if (content_type != null)
			{
				if (content_encoding != null && content_type.IndexOf("charset=") == -1)
				{
					string webName = content_encoding.WebName;
					headers.SetInternal("Content-Type", content_type + "; charset=" + webName);
				}
				else
				{
					headers.SetInternal("Content-Type", content_type);
				}
			}
			if (headers["Server"] == null)
			{
				headers.SetInternal("Server", "Mono-HTTPAPI/1.0");
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			if (headers["Date"] == null)
			{
				headers.SetInternal("Date", DateTime.UtcNow.ToString("r", invariantCulture));
			}
			if (!chunked)
			{
				if (!cl_set && closing)
				{
					cl_set = true;
					content_length = 0L;
				}
				if (cl_set)
				{
					headers.SetInternal("Content-Length", content_length.ToString(invariantCulture));
				}
			}
			Version protocolVersion = context.Request.ProtocolVersion;
			if (!cl_set && !chunked && protocolVersion >= HttpVersion.Version11)
			{
				chunked = true;
			}
			bool flag = status_code == 400 || status_code == 408 || status_code == 411 || status_code == 413 || status_code == 414 || status_code == 500 || status_code == 503;
			if (!flag)
			{
				flag = context.Request.Headers["connection"] == "close";
				flag |= protocolVersion <= HttpVersion.Version10;
			}
			if (!keep_alive || flag)
			{
				headers.SetInternal("Connection", "close");
			}
			if (chunked)
			{
				headers.SetInternal("Transfer-Encoding", "chunked");
			}
			int chunkedUses = context.Connection.ChunkedUses;
			if (chunkedUses >= 100)
			{
				force_close_chunked = true;
				if (!flag)
				{
					headers.SetInternal("Connection", "close");
				}
			}
			if (location != null)
			{
				headers.SetInternal("Location", location);
			}
			if (cookies != null)
			{
				foreach (Cookie cookie in cookies)
				{
					headers.SetInternal("Set-Cookie", cookie.ToClientString());
				}
			}
			StreamWriter streamWriter = new StreamWriter(ms, encoding);
			streamWriter.Write("HTTP/{0} {1} {2}\r\n", version, status_code, status_description);
			string value = headers.ToStringMultiValue();
			streamWriter.Write(value);
			streamWriter.Flush();
			int num = encoding.GetPreamble().Length;
			if (output_stream == null)
			{
				output_stream = context.Connection.GetResponseStream();
			}
			ms.Position = num;
			HeadersSent = true;
		}

		public void SetCookie(Cookie cookie)
		{
			if (cookie == null)
			{
				throw new ArgumentNullException("cookie");
			}
			if (cookies != null)
			{
				if (FindCookie(cookie))
				{
					throw new ArgumentException("The cookie already exists.");
				}
			}
			else
			{
				cookies = new CookieCollection();
			}
			cookies.Add(cookie);
		}
	}
}
