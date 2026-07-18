using System;

namespace Mono.Security.Protocol.Tls
{
	internal class ClientSessionInfo : IDisposable
	{
		private const int DefaultValidityInterval = 180;

		private static readonly int ValidityInterval;

		private bool disposed;

		private DateTime validuntil;

		private string host;

		private byte[] sid;

		private byte[] masterSecret;

		public string HostName
		{
			get
			{
				return host;
			}
		}

		public byte[] Id
		{
			get
			{
				return sid;
			}
		}

		public bool Valid
		{
			get
			{
				return masterSecret != null && validuntil > DateTime.UtcNow;
			}
		}

		public ClientSessionInfo(string hostname, byte[] id)
		{
			host = hostname;
			sid = id;
			KeepAlive();
		}

		static ClientSessionInfo()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MONO_TLS_SESSION_CACHE_TIMEOUT");
			if (environmentVariable == null)
			{
				ValidityInterval = 180;
				return;
			}
			try
			{
				ValidityInterval = int.Parse(environmentVariable);
			}
			catch
			{
				ValidityInterval = 180;
			}
		}

		~ClientSessionInfo()
		{
			Dispose(false);
		}

		public void GetContext(Context context)
		{
			CheckDisposed();
			if (context.MasterSecret != null)
			{
				masterSecret = (byte[])context.MasterSecret.Clone();
			}
		}

		public void SetContext(Context context)
		{
			CheckDisposed();
			if (masterSecret != null)
			{
				context.MasterSecret = (byte[])masterSecret.Clone();
			}
		}

		public void KeepAlive()
		{
			CheckDisposed();
			validuntil = DateTime.UtcNow.AddSeconds(ValidityInterval);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				validuntil = DateTime.MinValue;
				host = null;
				sid = null;
				if (masterSecret != null)
				{
					Array.Clear(masterSecret, 0, masterSecret.Length);
					masterSecret = null;
				}
			}
			disposed = true;
		}

		private void CheckDisposed()
		{
			if (disposed)
			{
				string text = global::Locale.GetText("Cache session information were disposed.");
				throw new ObjectDisposedException(text);
			}
		}
	}
}
