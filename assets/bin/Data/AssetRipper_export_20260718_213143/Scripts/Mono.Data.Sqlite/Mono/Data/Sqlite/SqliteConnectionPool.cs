using System;
using System.Collections.Generic;

namespace Mono.Data.Sqlite
{
	internal static class SqliteConnectionPool
	{
		internal class Pool
		{
			internal readonly Queue<WeakReference> Queue = new Queue<WeakReference>();

			internal int PoolVersion;

			internal int MaxPoolSize;

			internal Pool(int version, int maxSize)
			{
				PoolVersion = version;
				MaxPoolSize = maxSize;
			}
		}

		private static SortedList<string, Pool> _connections = new SortedList<string, Pool>(StringComparer.OrdinalIgnoreCase);

		private static int _poolVersion = 1;

		internal static SqliteConnectionHandle Remove(string fileName, int maxPoolSize, out int version)
		{
			lock (_connections)
			{
				version = _poolVersion;
				Pool value;
				if (!_connections.TryGetValue(fileName, out value))
				{
					value = new Pool(_poolVersion, maxPoolSize);
					_connections.Add(fileName, value);
					return null;
				}
				version = value.PoolVersion;
				value.MaxPoolSize = maxPoolSize;
				ResizePool(value, false);
				while (value.Queue.Count > 0)
				{
					WeakReference weakReference = value.Queue.Dequeue();
					SqliteConnectionHandle sqliteConnectionHandle = weakReference.Target as SqliteConnectionHandle;
					if (sqliteConnectionHandle != null)
					{
						return sqliteConnectionHandle;
					}
				}
				return null;
			}
		}

		internal static void ClearAllPools()
		{
			lock (_connections)
			{
				foreach (KeyValuePair<string, Pool> connection in _connections)
				{
					while (connection.Value.Queue.Count > 0)
					{
						WeakReference weakReference = connection.Value.Queue.Dequeue();
						SqliteConnectionHandle sqliteConnectionHandle = weakReference.Target as SqliteConnectionHandle;
						if (sqliteConnectionHandle != null)
						{
							sqliteConnectionHandle.Dispose();
						}
					}
					if (_poolVersion <= connection.Value.PoolVersion)
					{
						_poolVersion = connection.Value.PoolVersion + 1;
					}
				}
				_connections.Clear();
			}
		}

		internal static void ClearPool(string fileName)
		{
			lock (_connections)
			{
				Pool value;
				if (!_connections.TryGetValue(fileName, out value))
				{
					return;
				}
				value.PoolVersion++;
				while (value.Queue.Count > 0)
				{
					WeakReference weakReference = value.Queue.Dequeue();
					SqliteConnectionHandle sqliteConnectionHandle = weakReference.Target as SqliteConnectionHandle;
					if (sqliteConnectionHandle != null)
					{
						sqliteConnectionHandle.Dispose();
					}
				}
			}
		}

		internal static void Add(string fileName, SqliteConnectionHandle hdl, int version)
		{
			lock (_connections)
			{
				Pool value;
				if (_connections.TryGetValue(fileName, out value) && version == value.PoolVersion)
				{
					ResizePool(value, true);
					value.Queue.Enqueue(new WeakReference(hdl, false));
					GC.KeepAlive(hdl);
				}
				else
				{
					hdl.Close();
				}
			}
		}

		private static void ResizePool(Pool queue, bool forAdding)
		{
			int num = queue.MaxPoolSize;
			if (forAdding && num > 0)
			{
				num--;
			}
			while (queue.Queue.Count > num)
			{
				WeakReference weakReference = queue.Queue.Dequeue();
				SqliteConnectionHandle sqliteConnectionHandle = weakReference.Target as SqliteConnectionHandle;
				if (sqliteConnectionHandle != null)
				{
					sqliteConnectionHandle.Dispose();
				}
			}
		}
	}
}
