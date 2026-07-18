using System;
using System.Collections;

namespace Mono.Security.Protocol.Tls
{
	internal class ClientSessionCache
	{
		private static Hashtable cache;

		private static object locker;

		static ClientSessionCache()
		{
			cache = new Hashtable();
			locker = new object();
		}

		public static void Add(string host, byte[] id)
		{
			lock (locker)
			{
				string key = BitConverter.ToString(id);
				ClientSessionInfo clientSessionInfo = (ClientSessionInfo)cache[key];
				if (clientSessionInfo == null)
				{
					cache.Add(key, new ClientSessionInfo(host, id));
					return;
				}
				if (clientSessionInfo.HostName == host)
				{
					clientSessionInfo.KeepAlive();
					return;
				}
				clientSessionInfo.Dispose();
				cache.Remove(key);
				cache.Add(key, new ClientSessionInfo(host, id));
			}
		}

		public static byte[] FromHost(string host)
		{
			lock (locker)
			{
				foreach (ClientSessionInfo value in cache.Values)
				{
					if (value.HostName == host && value.Valid)
					{
						value.KeepAlive();
						return value.Id;
					}
				}
				return null;
			}
		}

		private static ClientSessionInfo FromContext(Context context, bool checkValidity)
		{
			if (context == null)
			{
				return null;
			}
			byte[] sessionId = context.SessionId;
			if (sessionId == null || sessionId.Length == 0)
			{
				return null;
			}
			string key = BitConverter.ToString(sessionId);
			ClientSessionInfo clientSessionInfo = (ClientSessionInfo)cache[key];
			if (clientSessionInfo == null)
			{
				return null;
			}
			if (context.ClientSettings.TargetHost != clientSessionInfo.HostName)
			{
				return null;
			}
			if (checkValidity && !clientSessionInfo.Valid)
			{
				clientSessionInfo.Dispose();
				cache.Remove(key);
				return null;
			}
			return clientSessionInfo;
		}

		public static bool SetContextInCache(Context context)
		{
			lock (locker)
			{
				ClientSessionInfo clientSessionInfo = FromContext(context, false);
				if (clientSessionInfo == null)
				{
					return false;
				}
				clientSessionInfo.GetContext(context);
				clientSessionInfo.KeepAlive();
				return true;
			}
		}

		public static bool SetContextFromCache(Context context)
		{
			lock (locker)
			{
				ClientSessionInfo clientSessionInfo = FromContext(context, true);
				if (clientSessionInfo == null)
				{
					return false;
				}
				clientSessionInfo.SetContext(context);
				clientSessionInfo.KeepAlive();
				return true;
			}
		}
	}
}
