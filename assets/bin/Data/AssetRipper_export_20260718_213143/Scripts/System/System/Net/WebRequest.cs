using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Principal;

namespace System.Net
{
	[Serializable]
	public abstract class WebRequest : MarshalByRefObject, ISerializable
	{
		private static HybridDictionary prefixes;

		private static bool isDefaultWebProxySet;

		private static IWebProxy defaultWebProxy;

		private AuthenticationLevel authentication_level = AuthenticationLevel.MutualAuthRequested;

		private static readonly object lockobj;

		public AuthenticationLevel AuthenticationLevel
		{
			get
			{
				return authentication_level;
			}
			set
			{
				authentication_level = value;
			}
		}

		public virtual RequestCachePolicy CachePolicy
		{
			get
			{
				throw GetMustImplement();
			}
			set
			{
			}
		}

		public virtual string ConnectionGroupName
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

		public virtual long ContentLength
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

		public virtual string ContentType
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

		public virtual ICredentials Credentials
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

		public static RequestCachePolicy DefaultCachePolicy
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

		public virtual WebHeaderCollection Headers
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

		public TokenImpersonationLevel ImpersonationLevel
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

		public virtual string Method
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

		public virtual bool PreAuthenticate
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

		public virtual IWebProxy Proxy
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

		public virtual Uri RequestUri
		{
			get
			{
				throw GetMustImplement();
			}
		}

		public virtual int Timeout
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

		public virtual bool UseDefaultCredentials
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

		public static IWebProxy DefaultWebProxy
		{
			get
			{
				if (!isDefaultWebProxySet)
				{
					lock (lockobj)
					{
						if (defaultWebProxy == null)
						{
							defaultWebProxy = GetDefaultWebProxy();
						}
					}
				}
				return defaultWebProxy;
			}
			set
			{
				defaultWebProxy = value;
				isDefaultWebProxySet = true;
			}
		}

		protected WebRequest()
		{
		}

		protected WebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
		}

		static WebRequest()
		{
			prefixes = new HybridDictionary();
			lockobj = new object();
			AddDynamicPrefix("http", "HttpRequestCreator");
			AddDynamicPrefix("https", "HttpRequestCreator");
			AddDynamicPrefix("file", "FileWebRequestCreator");
			AddDynamicPrefix("ftp", "FtpRequestCreator");
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw new NotSupportedException();
		}

		private static void AddDynamicPrefix(string protocol, string implementor)
		{
			Type type = typeof(WebRequest).Assembly.GetType("System.Net." + implementor);
			if (type != null)
			{
				AddPrefix(protocol, type);
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException("This method must be implemented in derived classes");
		}

		[System.MonoTODO("Needs to respect Module, Proxy.AutoDetect, and Proxy.ScriptLocation config settings")]
		private static IWebProxy GetDefaultWebProxy()
		{
			return null;
		}

		public virtual void Abort()
		{
			throw GetMustImplement();
		}

		public virtual IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			throw GetMustImplement();
		}

		public virtual IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			throw GetMustImplement();
		}

		public static WebRequest Create(string requestUriString)
		{
			if (requestUriString == null)
			{
				throw new ArgumentNullException("requestUriString");
			}
			return Create(new Uri(requestUriString));
		}

		public static WebRequest Create(Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			return GetCreator(requestUri.AbsoluteUri).Create(requestUri);
		}

		public static WebRequest CreateDefault(Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			return GetCreator(requestUri.Scheme).Create(requestUri);
		}

		public virtual Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			throw GetMustImplement();
		}

		public virtual WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			throw GetMustImplement();
		}

		public virtual Stream GetRequestStream()
		{
			throw GetMustImplement();
		}

		public virtual WebResponse GetResponse()
		{
			throw GetMustImplement();
		}

		[System.MonoTODO("Look in other places for proxy config info")]
		public static IWebProxy GetSystemWebProxy()
		{
			string text = Environment.GetEnvironmentVariable("http_proxy");
			if (text == null)
			{
				text = Environment.GetEnvironmentVariable("HTTP_PROXY");
			}
			if (text != null)
			{
				try
				{
					if (!text.StartsWith("http://"))
					{
						text = "http://" + text;
					}
					Uri uri = new Uri(text);
					IPAddress address;
					if (IPAddress.TryParse(uri.Host, out address))
					{
						if (IPAddress.Any.Equals(address))
						{
							UriBuilder uriBuilder = new UriBuilder(uri);
							uriBuilder.Host = "127.0.0.1";
							uri = uriBuilder.Uri;
						}
						else if (IPAddress.IPv6Any.Equals(address))
						{
							UriBuilder uriBuilder2 = new UriBuilder(uri);
							uriBuilder2.Host = "[::1]";
							uri = uriBuilder2.Uri;
						}
					}
					return new WebProxy(uri);
				}
				catch (UriFormatException)
				{
				}
			}
			return new WebProxy();
		}

		protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw GetMustImplement();
		}

		public static bool RegisterPrefix(string prefix, IWebRequestCreate creator)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			lock (prefixes.SyncRoot)
			{
				string key = prefix.ToLower(CultureInfo.InvariantCulture);
				if (prefixes.Contains(key))
				{
					return false;
				}
				prefixes.Add(key, creator);
			}
			return true;
		}

		private static IWebRequestCreate GetCreator(string prefix)
		{
			int num = -1;
			IWebRequestCreate webRequestCreate = null;
			prefix = prefix.ToLower(CultureInfo.InvariantCulture);
			IDictionaryEnumerator enumerator = prefixes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = enumerator.Key as string;
				if (text.Length > num && prefix.StartsWith(text))
				{
					num = text.Length;
					webRequestCreate = (IWebRequestCreate)enumerator.Value;
				}
			}
			if (webRequestCreate == null)
			{
				throw new NotSupportedException(prefix);
			}
			return webRequestCreate;
		}

		internal static void ClearPrefixes()
		{
			prefixes.Clear();
		}

		internal static void RemovePrefix(string prefix)
		{
			prefixes.Remove(prefix);
		}

		internal static void AddPrefix(string prefix, string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type == null)
			{
				throw new ArgumentException(string.Format("Type {0} not found", typeName));
			}
			AddPrefix(prefix, type);
		}

		internal static void AddPrefix(string prefix, Type type)
		{
			object value = Activator.CreateInstance(type, true);
			prefixes[prefix] = value;
		}
	}
}
