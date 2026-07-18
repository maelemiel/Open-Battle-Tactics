using System.Collections;

namespace System.Net
{
	internal class DigestClient : IAuthenticationModule
	{
		private static readonly Hashtable cache = Hashtable.Synchronized(new Hashtable());

		private static Hashtable Cache
		{
			get
			{
				lock (cache.SyncRoot)
				{
					CheckExpired(cache.Count);
				}
				return cache;
			}
		}

		public string AuthenticationType
		{
			get
			{
				return "Digest";
			}
		}

		public bool CanPreAuthenticate
		{
			get
			{
				return true;
			}
		}

		private static void CheckExpired(int count)
		{
			if (count < 10)
			{
				return;
			}
			DateTime dateTime = DateTime.MaxValue;
			DateTime now = DateTime.Now;
			ArrayList arrayList = null;
			foreach (int key in cache.Keys)
			{
				System.Net.DigestSession digestSession = (System.Net.DigestSession)cache[key];
				if (digestSession.LastUse < dateTime && (digestSession.LastUse - now).Ticks > 6000000000L)
				{
					dateTime = digestSession.LastUse;
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(key);
				}
			}
			if (arrayList == null)
			{
				return;
			}
			foreach (int item in arrayList)
			{
				cache.Remove(item);
			}
		}

		public Authorization Authenticate(string challenge, WebRequest webRequest, ICredentials credentials)
		{
			if (credentials == null || challenge == null)
			{
				return null;
			}
			string text = challenge.Trim();
			if (text.ToLower().IndexOf("digest") == -1)
			{
				return null;
			}
			HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
			if (httpWebRequest == null)
			{
				return null;
			}
			int num = httpWebRequest.Address.GetHashCode() ^ credentials.GetHashCode();
			System.Net.DigestSession digestSession = (System.Net.DigestSession)Cache[num];
			bool flag = digestSession == null;
			if (flag)
			{
				digestSession = new System.Net.DigestSession();
			}
			if (!digestSession.Parse(challenge))
			{
				return null;
			}
			if (flag)
			{
				Cache.Add(num, digestSession);
			}
			return digestSession.Authenticate(webRequest, credentials);
		}

		public Authorization PreAuthenticate(WebRequest webRequest, ICredentials credentials)
		{
			HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
			if (httpWebRequest == null)
			{
				return null;
			}
			if (credentials == null)
			{
				return null;
			}
			int num = httpWebRequest.Address.GetHashCode() ^ credentials.GetHashCode();
			System.Net.DigestSession digestSession = (System.Net.DigestSession)Cache[num];
			if (digestSession == null)
			{
				return null;
			}
			return digestSession.Authenticate(webRequest, credentials);
		}
	}
}
