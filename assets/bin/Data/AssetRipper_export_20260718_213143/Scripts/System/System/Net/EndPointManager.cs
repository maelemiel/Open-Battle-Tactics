using System.Collections;

namespace System.Net
{
	internal sealed class EndPointManager
	{
		private static Hashtable ip_to_endpoints = new Hashtable();

		private EndPointManager()
		{
		}

		public static void AddListener(HttpListener listener)
		{
			ArrayList arrayList = new ArrayList();
			try
			{
				lock (ip_to_endpoints)
				{
					foreach (string prefix2 in listener.Prefixes)
					{
						AddPrefixInternal(prefix2, listener);
						arrayList.Add(prefix2);
					}
				}
			}
			catch
			{
				foreach (string item in arrayList)
				{
					RemovePrefix(item, listener);
				}
				throw;
			}
		}

		public static void AddPrefix(string prefix, HttpListener listener)
		{
			lock (ip_to_endpoints)
			{
				AddPrefixInternal(prefix, listener);
			}
		}

		private static void AddPrefixInternal(string p, HttpListener listener)
		{
			System.Net.ListenerPrefix listenerPrefix = new System.Net.ListenerPrefix(p);
			if (listenerPrefix.Path.IndexOf('%') != -1)
			{
				throw new HttpListenerException(400, "Invalid path.");
			}
			if (listenerPrefix.Path.IndexOf("//") != -1)
			{
				throw new HttpListenerException(400, "Invalid path.");
			}
			System.Net.EndPointListener ePListener = GetEPListener(IPAddress.Any, listenerPrefix.Port, listener, listenerPrefix.Secure);
			ePListener.AddPrefix(listenerPrefix, listener);
		}

		private static System.Net.EndPointListener GetEPListener(IPAddress addr, int port, HttpListener listener, bool secure)
		{
			Hashtable hashtable = null;
			if (ip_to_endpoints.ContainsKey(addr))
			{
				hashtable = (Hashtable)ip_to_endpoints[addr];
			}
			else
			{
				hashtable = new Hashtable();
				ip_to_endpoints[addr] = hashtable;
			}
			System.Net.EndPointListener endPointListener = null;
			return (System.Net.EndPointListener)(hashtable.ContainsKey(port) ? ((System.Net.EndPointListener)hashtable[port]) : (hashtable[port] = new System.Net.EndPointListener(addr, port, secure)));
		}

		public static void RemoveEndPoint(System.Net.EndPointListener epl, IPEndPoint ep)
		{
			lock (ip_to_endpoints)
			{
				Hashtable hashtable = null;
				hashtable = (Hashtable)ip_to_endpoints[ep.Address];
				hashtable.Remove(ep.Port);
				if (hashtable.Count == 0)
				{
					ip_to_endpoints.Remove(ep.Address);
				}
				epl.Close();
			}
		}

		public static void RemoveListener(HttpListener listener)
		{
			lock (ip_to_endpoints)
			{
				foreach (string prefix in listener.Prefixes)
				{
					RemovePrefixInternal(prefix, listener);
				}
			}
		}

		public static void RemovePrefix(string prefix, HttpListener listener)
		{
			lock (ip_to_endpoints)
			{
				RemovePrefixInternal(prefix, listener);
			}
		}

		private static void RemovePrefixInternal(string prefix, HttpListener listener)
		{
			System.Net.ListenerPrefix listenerPrefix = new System.Net.ListenerPrefix(prefix);
			if (listenerPrefix.Path.IndexOf('%') == -1 && listenerPrefix.Path.IndexOf("//") == -1)
			{
				System.Net.EndPointListener ePListener = GetEPListener(IPAddress.Any, listenerPrefix.Port, listener, listenerPrefix.Secure);
				ePListener.RemovePrefix(listenerPrefix, listener);
			}
		}
	}
}
