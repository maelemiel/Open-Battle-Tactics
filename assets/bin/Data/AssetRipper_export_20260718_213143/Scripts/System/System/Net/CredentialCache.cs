using System.Collections;

namespace System.Net
{
	public class CredentialCache : IEnumerable, ICredentials, ICredentialsByHost
	{
		private class CredentialCacheKey
		{
			private Uri uriPrefix;

			private string authType;

			private string absPath;

			private int len;

			private int hash;

			public int Length
			{
				get
				{
					return len;
				}
			}

			public string AbsPath
			{
				get
				{
					return absPath;
				}
			}

			public Uri UriPrefix
			{
				get
				{
					return uriPrefix;
				}
			}

			public string AuthType
			{
				get
				{
					return authType;
				}
			}

			internal CredentialCacheKey(Uri uriPrefix, string authType)
			{
				this.uriPrefix = uriPrefix;
				this.authType = authType;
				absPath = uriPrefix.AbsolutePath;
				absPath = absPath.Substring(0, absPath.LastIndexOf('/'));
				len = uriPrefix.AbsoluteUri.Length;
				hash = uriPrefix.GetHashCode() + authType.GetHashCode();
			}

			public override int GetHashCode()
			{
				return hash;
			}

			public override bool Equals(object obj)
			{
				CredentialCacheKey credentialCacheKey = obj as CredentialCacheKey;
				return credentialCacheKey != null && hash == credentialCacheKey.hash;
			}

			public override string ToString()
			{
				return absPath + " : " + authType + " : len=" + len;
			}
		}

		private class CredentialCacheForHostKey
		{
			private string host;

			private int port;

			private string authType;

			private int hash;

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

			public string AuthType
			{
				get
				{
					return authType;
				}
			}

			internal CredentialCacheForHostKey(string host, int port, string authType)
			{
				this.host = host;
				this.port = port;
				this.authType = authType;
				hash = host.GetHashCode() + port.GetHashCode() + authType.GetHashCode();
			}

			public override int GetHashCode()
			{
				return hash;
			}

			public override bool Equals(object obj)
			{
				CredentialCacheForHostKey credentialCacheForHostKey = obj as CredentialCacheForHostKey;
				return credentialCacheForHostKey != null && hash == credentialCacheForHostKey.hash;
			}

			public override string ToString()
			{
				return host + " : " + authType;
			}
		}

		private static NetworkCredential empty = new NetworkCredential(string.Empty, string.Empty, string.Empty);

		private Hashtable cache;

		private Hashtable cacheForHost;

		[System.MonoTODO("Need EnvironmentPermission implementation first")]
		public static ICredentials DefaultCredentials
		{
			get
			{
				return empty;
			}
		}

		public static NetworkCredential DefaultNetworkCredentials
		{
			get
			{
				return empty;
			}
		}

		public CredentialCache()
		{
			cache = new Hashtable();
			cacheForHost = new Hashtable();
		}

		public NetworkCredential GetCredential(Uri uriPrefix, string authType)
		{
			int num = -1;
			NetworkCredential result = null;
			if (uriPrefix == null || authType == null)
			{
				return null;
			}
			string absolutePath = uriPrefix.AbsolutePath;
			absolutePath = absolutePath.Substring(0, absolutePath.LastIndexOf('/'));
			IDictionaryEnumerator enumerator = cache.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CredentialCacheKey credentialCacheKey = enumerator.Key as CredentialCacheKey;
				if (credentialCacheKey.Length > num && string.Compare(credentialCacheKey.AuthType, authType, true) == 0)
				{
					Uri uriPrefix2 = credentialCacheKey.UriPrefix;
					if (!(uriPrefix2.Scheme != uriPrefix.Scheme) && uriPrefix2.Port == uriPrefix.Port && !(uriPrefix2.Host != uriPrefix.Host) && absolutePath.StartsWith(credentialCacheKey.AbsPath))
					{
						num = credentialCacheKey.Length;
						result = (NetworkCredential)enumerator.Value;
					}
				}
			}
			return result;
		}

		public IEnumerator GetEnumerator()
		{
			return cache.Values.GetEnumerator();
		}

		public void Add(Uri uriPrefix, string authType, NetworkCredential cred)
		{
			if (uriPrefix == null)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			if (authType == null)
			{
				throw new ArgumentNullException("authType");
			}
			cache.Add(new CredentialCacheKey(uriPrefix, authType), cred);
		}

		public void Remove(Uri uriPrefix, string authType)
		{
			if (uriPrefix == null)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			if (authType == null)
			{
				throw new ArgumentNullException("authType");
			}
			cache.Remove(new CredentialCacheKey(uriPrefix, authType));
		}

		public NetworkCredential GetCredential(string host, int port, string authenticationType)
		{
			NetworkCredential result = null;
			if (host == null || port < 0 || authenticationType == null)
			{
				return null;
			}
			IDictionaryEnumerator enumerator = cacheForHost.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CredentialCacheForHostKey credentialCacheForHostKey = enumerator.Key as CredentialCacheForHostKey;
				if (string.Compare(credentialCacheForHostKey.AuthType, authenticationType, true) == 0 && !(credentialCacheForHostKey.Host != host) && credentialCacheForHostKey.Port == port)
				{
					result = (NetworkCredential)enumerator.Value;
				}
			}
			return result;
		}

		public void Add(string host, int port, string authenticationType, NetworkCredential credential)
		{
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			if (port < 0)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			if (authenticationType == null)
			{
				throw new ArgumentOutOfRangeException("authenticationType");
			}
			cacheForHost.Add(new CredentialCacheForHostKey(host, port, authenticationType), credential);
		}

		public void Remove(string host, int port, string authenticationType)
		{
			if (host != null && authenticationType != null)
			{
				cacheForHost.Remove(new CredentialCacheForHostKey(host, port, authenticationType));
			}
		}
	}
}
