using System;
using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public class TdsConnectionPoolManager
	{
		private Hashtable pools = Hashtable.Synchronized(new Hashtable());

		private TdsVersion version;

		public TdsConnectionPoolManager(TdsVersion version)
		{
			this.version = version;
		}

		public TdsConnectionPool GetConnectionPool(string connectionString, TdsConnectionInfo info)
		{
			TdsConnectionPool tdsConnectionPool = (TdsConnectionPool)pools[connectionString];
			if (tdsConnectionPool == null)
			{
				pools[connectionString] = new TdsConnectionPool(this, info);
				tdsConnectionPool = (TdsConnectionPool)pools[connectionString];
			}
			return tdsConnectionPool;
		}

		public TdsConnectionPool GetConnectionPool(string connectionString)
		{
			return (TdsConnectionPool)pools[connectionString];
		}

		public virtual Tds CreateConnection(TdsConnectionInfo info)
		{
			switch (version)
			{
			case TdsVersion.tds42:
				return new Tds42(info.DataSource, info.Port, info.PacketSize, info.Timeout);
			case TdsVersion.tds50:
				return new Tds50(info.DataSource, info.Port, info.PacketSize, info.Timeout);
			case TdsVersion.tds70:
				return new Tds70(info.DataSource, info.Port, info.PacketSize, info.Timeout);
			case TdsVersion.tds80:
				return new Tds80(info.DataSource, info.Port, info.PacketSize, info.Timeout);
			default:
				throw new NotSupportedException();
			}
		}

		public IDictionary GetConnectionPool()
		{
			return pools;
		}
	}
}
