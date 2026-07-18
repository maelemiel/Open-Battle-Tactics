using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.Net
{
	[Serializable]
	[ComVisible(true)]
	public class WebHeaderCollection : NameValueCollection, ISerializable
	{
		private static readonly Hashtable restricted;

		private static readonly Hashtable multiValue;

		private static readonly Dictionary<string, bool> restricted_response;

		private bool internallyCreated;

		private static bool[] allowed_chars;

		public override string[] AllKeys
		{
			get
			{
				return base.AllKeys;
			}
		}

		public override int Count
		{
			get
			{
				return base.Count;
			}
		}

		public override KeysCollection Keys
		{
			get
			{
				return base.Keys;
			}
		}

		public string this[HttpRequestHeader hrh]
		{
			get
			{
				return Get(RequestHeaderToString(hrh));
			}
			set
			{
				Add(RequestHeaderToString(hrh), value);
			}
		}

		public string this[HttpResponseHeader hrh]
		{
			get
			{
				return Get(ResponseHeaderToString(hrh));
			}
			set
			{
				Add(ResponseHeaderToString(hrh), value);
			}
		}

		public WebHeaderCollection()
		{
		}

		protected WebHeaderCollection(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			try
			{
				int @int = serializationInfo.GetInt32("Count");
				for (int i = 0; i < @int; i++)
				{
					Add(serializationInfo.GetString(i.ToString()), serializationInfo.GetString((@int + i).ToString()));
				}
			}
			catch (SerializationException)
			{
				int @int = serializationInfo.GetInt32("count");
				for (int j = 0; j < @int; j++)
				{
					Add(serializationInfo.GetString("k" + j), serializationInfo.GetString("v" + j));
				}
			}
		}

		internal WebHeaderCollection(bool internallyCreated)
		{
			this.internallyCreated = internallyCreated;
		}

		static WebHeaderCollection()
		{
			allowed_chars = new bool[126]
			{
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, false, false, false, false, false, false, false,
				false, false, false, true, false, true, true, true, true, false,
				false, false, true, true, false, true, true, false, true, true,
				true, true, true, true, true, true, true, true, false, false,
				false, false, false, false, false, true, true, true, true, true,
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, true, true, true, true, true, true,
				true, false, false, false, true, true, true, true, true, true,
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, false, true, false
			};
			restricted = new Hashtable(CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
			restricted.Add("accept", true);
			restricted.Add("connection", true);
			restricted.Add("content-length", true);
			restricted.Add("content-type", true);
			restricted.Add("date", true);
			restricted.Add("expect", true);
			restricted.Add("host", true);
			restricted.Add("if-modified-since", true);
			restricted.Add("range", true);
			restricted.Add("referer", true);
			restricted.Add("transfer-encoding", true);
			restricted.Add("user-agent", true);
			restricted.Add("proxy-connection", true);
			restricted_response = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
			restricted_response.Add("Content-Length", true);
			restricted_response.Add("Transfer-Encoding", true);
			restricted_response.Add("WWW-Authenticate", true);
			multiValue = new Hashtable(CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
			multiValue.Add("accept", true);
			multiValue.Add("accept-charset", true);
			multiValue.Add("accept-encoding", true);
			multiValue.Add("accept-language", true);
			multiValue.Add("accept-ranges", true);
			multiValue.Add("allow", true);
			multiValue.Add("authorization", true);
			multiValue.Add("cache-control", true);
			multiValue.Add("connection", true);
			multiValue.Add("content-encoding", true);
			multiValue.Add("content-language", true);
			multiValue.Add("expect", true);
			multiValue.Add("if-match", true);
			multiValue.Add("if-none-match", true);
			multiValue.Add("proxy-authenticate", true);
			multiValue.Add("public", true);
			multiValue.Add("range", true);
			multiValue.Add("transfer-encoding", true);
			multiValue.Add("upgrade", true);
			multiValue.Add("vary", true);
			multiValue.Add("via", true);
			multiValue.Add("warning", true);
			multiValue.Add("www-authenticate", true);
			multiValue.Add("set-cookie", true);
			multiValue.Add("set-cookie2", true);
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			GetObjectData(serializationInfo, streamingContext);
		}

		public void Add(string header)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			int num = header.IndexOf(':');
			if (num == -1)
			{
				throw new ArgumentException("no colon found", "header");
			}
			Add(header.Substring(0, num), header.Substring(num + 1));
		}

		public override void Add(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (internallyCreated && IsRestricted(name))
			{
				throw new ArgumentException("This header must be modified with the appropiate property.");
			}
			AddWithoutValidate(name, value);
		}

		protected void AddWithoutValidate(string headerName, string headerValue)
		{
			if (!IsHeaderName(headerName))
			{
				throw new ArgumentException("invalid header name: " + headerName, "headerName");
			}
			headerValue = ((headerValue != null) ? headerValue.Trim() : string.Empty);
			if (!IsHeaderValue(headerValue))
			{
				throw new ArgumentException("invalid header value: " + headerValue, "headerValue");
			}
			base.Add(headerName, headerValue);
		}

		public override string[] GetValues(string header)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			string[] values = base.GetValues(header);
			if (values == null || values.Length == 0)
			{
				return null;
			}
			return values;
		}

		public override string[] GetValues(int index)
		{
			string[] values = base.GetValues(index);
			if (values == null || values.Length == 0)
			{
				return null;
			}
			return values;
		}

		public static bool IsRestricted(string headerName)
		{
			if (headerName == null)
			{
				throw new ArgumentNullException("headerName");
			}
			if (headerName == string.Empty)
			{
				throw new ArgumentException("empty string", "headerName");
			}
			if (!IsHeaderName(headerName))
			{
				throw new ArgumentException("Invalid character in header");
			}
			return restricted.ContainsKey(headerName);
		}

		public static bool IsRestricted(string headerName, bool response)
		{
			if (string.IsNullOrEmpty(headerName))
			{
				throw new ArgumentNullException("headerName");
			}
			if (!IsHeaderName(headerName))
			{
				throw new ArgumentException("Invalid character in header");
			}
			if (response)
			{
				return restricted_response.ContainsKey(headerName);
			}
			return restricted.ContainsKey(headerName);
		}

		public override void OnDeserialization(object sender)
		{
		}

		public override void Remove(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (internallyCreated && IsRestricted(name))
			{
				throw new ArgumentException("restricted header");
			}
			base.Remove(name);
		}

		public override void Set(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (internallyCreated && IsRestricted(name))
			{
				throw new ArgumentException("restricted header");
			}
			if (!IsHeaderName(name))
			{
				throw new ArgumentException("invalid header name");
			}
			value = ((value != null) ? value.Trim() : string.Empty);
			if (!IsHeaderValue(value))
			{
				throw new ArgumentException("invalid header value");
			}
			base.Set(name, value);
		}

		public byte[] ToByteArray()
		{
			return Encoding.UTF8.GetBytes(ToString());
		}

		internal string ToStringMultiValue()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = base.Count;
			for (int i = 0; i < count; i++)
			{
				string key = GetKey(i);
				if (IsMultiValue(key))
				{
					string[] values = GetValues(i);
					foreach (string value in values)
					{
						stringBuilder.Append(key).Append(": ").Append(value)
							.Append("\r\n");
					}
				}
				else
				{
					stringBuilder.Append(key).Append(": ").Append(Get(i))
						.Append("\r\n");
				}
			}
			return stringBuilder.Append("\r\n").ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = base.Count;
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(GetKey(i)).Append(": ").Append(Get(i))
					.Append("\r\n");
			}
			return stringBuilder.Append("\r\n").ToString();
		}

		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			int count = base.Count;
			serializationInfo.AddValue("Count", count);
			for (int i = 0; i < count; i++)
			{
				serializationInfo.AddValue(i.ToString(), GetKey(i));
				serializationInfo.AddValue((count + i).ToString(), Get(i));
			}
		}

		public override string Get(int index)
		{
			return base.Get(index);
		}

		public override string Get(string name)
		{
			return base.Get(name);
		}

		public override string GetKey(int index)
		{
			return base.GetKey(index);
		}

		public void Add(HttpRequestHeader header, string value)
		{
			Add(RequestHeaderToString(header), value);
		}

		public void Remove(HttpRequestHeader header)
		{
			Remove(RequestHeaderToString(header));
		}

		public void Set(HttpRequestHeader header, string value)
		{
			Set(RequestHeaderToString(header), value);
		}

		public void Add(HttpResponseHeader header, string value)
		{
			Add(ResponseHeaderToString(header), value);
		}

		public void Remove(HttpResponseHeader header)
		{
			Remove(ResponseHeaderToString(header));
		}

		public void Set(HttpResponseHeader header, string value)
		{
			Set(ResponseHeaderToString(header), value);
		}

		private string RequestHeaderToString(HttpRequestHeader value)
		{
			switch (value)
			{
			case HttpRequestHeader.CacheControl:
				return "cache-control";
			case HttpRequestHeader.Connection:
				return "connection";
			case HttpRequestHeader.Date:
				return "date";
			case HttpRequestHeader.KeepAlive:
				return "keep-alive";
			case HttpRequestHeader.Pragma:
				return "pragma";
			case HttpRequestHeader.Trailer:
				return "trailer";
			case HttpRequestHeader.TransferEncoding:
				return "transfer-encoding";
			case HttpRequestHeader.Upgrade:
				return "upgrade";
			case HttpRequestHeader.Via:
				return "via";
			case HttpRequestHeader.Warning:
				return "warning";
			case HttpRequestHeader.Allow:
				return "allow";
			case HttpRequestHeader.ContentLength:
				return "content-length";
			case HttpRequestHeader.ContentType:
				return "content-type";
			case HttpRequestHeader.ContentEncoding:
				return "content-encoding";
			case HttpRequestHeader.ContentLanguage:
				return "content-language";
			case HttpRequestHeader.ContentLocation:
				return "content-location";
			case HttpRequestHeader.ContentMd5:
				return "content-md5";
			case HttpRequestHeader.ContentRange:
				return "content-range";
			case HttpRequestHeader.Expires:
				return "expires";
			case HttpRequestHeader.LastModified:
				return "last-modified";
			case HttpRequestHeader.Accept:
				return "accept";
			case HttpRequestHeader.AcceptCharset:
				return "accept-charset";
			case HttpRequestHeader.AcceptEncoding:
				return "accept-encoding";
			case HttpRequestHeader.AcceptLanguage:
				return "accept-language";
			case HttpRequestHeader.Authorization:
				return "authorization";
			case HttpRequestHeader.Cookie:
				return "cookie";
			case HttpRequestHeader.Expect:
				return "expect";
			case HttpRequestHeader.From:
				return "from";
			case HttpRequestHeader.Host:
				return "host";
			case HttpRequestHeader.IfMatch:
				return "if-match";
			case HttpRequestHeader.IfModifiedSince:
				return "if-modified-since";
			case HttpRequestHeader.IfNoneMatch:
				return "if-none-match";
			case HttpRequestHeader.IfRange:
				return "if-range";
			case HttpRequestHeader.IfUnmodifiedSince:
				return "if-unmodified-since";
			case HttpRequestHeader.MaxForwards:
				return "max-forwards";
			case HttpRequestHeader.ProxyAuthorization:
				return "proxy-authorization";
			case HttpRequestHeader.Referer:
				return "referer";
			case HttpRequestHeader.Range:
				return "range";
			case HttpRequestHeader.Te:
				return "te";
			case HttpRequestHeader.Translate:
				return "translate";
			case HttpRequestHeader.UserAgent:
				return "user-agent";
			default:
				throw new InvalidOperationException();
			}
		}

		private string ResponseHeaderToString(HttpResponseHeader value)
		{
			switch (value)
			{
			case HttpResponseHeader.CacheControl:
				return "cache-control";
			case HttpResponseHeader.Connection:
				return "connection";
			case HttpResponseHeader.Date:
				return "date";
			case HttpResponseHeader.KeepAlive:
				return "keep-alive";
			case HttpResponseHeader.Pragma:
				return "pragma";
			case HttpResponseHeader.Trailer:
				return "trailer";
			case HttpResponseHeader.TransferEncoding:
				return "transfer-encoding";
			case HttpResponseHeader.Upgrade:
				return "upgrade";
			case HttpResponseHeader.Via:
				return "via";
			case HttpResponseHeader.Warning:
				return "warning";
			case HttpResponseHeader.Allow:
				return "allow";
			case HttpResponseHeader.ContentLength:
				return "content-length";
			case HttpResponseHeader.ContentType:
				return "content-type";
			case HttpResponseHeader.ContentEncoding:
				return "content-encoding";
			case HttpResponseHeader.ContentLanguage:
				return "content-language";
			case HttpResponseHeader.ContentLocation:
				return "content-location";
			case HttpResponseHeader.ContentMd5:
				return "content-md5";
			case HttpResponseHeader.ContentRange:
				return "content-range";
			case HttpResponseHeader.Expires:
				return "expires";
			case HttpResponseHeader.LastModified:
				return "last-modified";
			case HttpResponseHeader.AcceptRanges:
				return "accept-ranges";
			case HttpResponseHeader.Age:
				return "age";
			case HttpResponseHeader.ETag:
				return "etag";
			case HttpResponseHeader.Location:
				return "location";
			case HttpResponseHeader.ProxyAuthenticate:
				return "proxy-authenticate";
			case HttpResponseHeader.RetryAfter:
				return "RetryAfter";
			case HttpResponseHeader.Server:
				return "server";
			case HttpResponseHeader.SetCookie:
				return "set-cookie";
			case HttpResponseHeader.Vary:
				return "vary";
			case HttpResponseHeader.WwwAuthenticate:
				return "www-authenticate";
			default:
				throw new InvalidOperationException();
			}
		}

		public override void Clear()
		{
			base.Clear();
		}

		public override IEnumerator GetEnumerator()
		{
			return base.GetEnumerator();
		}

		internal void SetInternal(string header)
		{
			int num = header.IndexOf(':');
			if (num == -1)
			{
				throw new ArgumentException("no colon found", "header");
			}
			SetInternal(header.Substring(0, num), header.Substring(num + 1));
		}

		internal void SetInternal(string name, string value)
		{
			value = ((value != null) ? value.Trim() : string.Empty);
			if (!IsHeaderValue(value))
			{
				throw new ArgumentException("invalid header value");
			}
			if (IsMultiValue(name))
			{
				base.Add(name, value);
				return;
			}
			base.Remove(name);
			base.Set(name, value);
		}

		internal void RemoveAndAdd(string name, string value)
		{
			value = ((value != null) ? value.Trim() : string.Empty);
			base.Remove(name);
			base.Set(name, value);
		}

		internal void RemoveInternal(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			base.Remove(name);
		}

		internal static bool IsMultiValue(string headerName)
		{
			if (headerName == null || headerName == string.Empty)
			{
				return false;
			}
			return multiValue.ContainsKey(headerName);
		}

		internal static bool IsHeaderValue(string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				if (c == '\u007f')
				{
					return false;
				}
				if (c < ' ' && c != '\r' && c != '\n' && c != '\t')
				{
					return false;
				}
				if (c == '\n' && ++i < length)
				{
					c = value[i];
					if (c != ' ' && c != '\t')
					{
						return false;
					}
				}
			}
			return true;
		}

		internal static bool IsHeaderName(string name)
		{
			if (name == null || name.Length == 0)
			{
				return false;
			}
			int length = name.Length;
			for (int i = 0; i < length; i++)
			{
				char c = name[i];
				if (c > '~' || !allowed_chars[(uint)c])
				{
					return false;
				}
			}
			return true;
		}
	}
}
