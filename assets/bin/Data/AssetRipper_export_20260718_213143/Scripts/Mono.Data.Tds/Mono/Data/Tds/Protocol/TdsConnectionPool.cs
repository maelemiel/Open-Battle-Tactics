using System;
using System.Collections;
using System.Threading;

namespace Mono.Data.Tds.Protocol
{
	public class TdsConnectionPool
	{
		private TdsConnectionInfo info;

		private bool no_pooling;

		private TdsConnectionPoolManager manager;

		private Queue available;

		private ArrayList conns;

		private int in_progress;

		public bool Pooling
		{
			get
			{
				return !no_pooling;
			}
			set
			{
				no_pooling = !value;
			}
		}

		public TdsConnectionPool(TdsConnectionPoolManager manager, TdsConnectionInfo info)
		{
			this.info = info;
			this.manager = manager;
			conns = new ArrayList(info.PoolMaxSize);
			available = new Queue(info.PoolMaxSize);
			InitializePool();
		}

		private void InitializePool()
		{
			for (int i = conns.Count; i < info.PoolMinSize; i++)
			{
				try
				{
					Tds tds = manager.CreateConnection(info);
					conns.Add(tds);
					available.Enqueue(tds);
				}
				catch
				{
				}
			}
		}

		public Tds GetConnection()
		{
			if (no_pooling)
			{
				return manager.CreateConnection(info);
			}
			Tds tds = null;
			int num = info.PoolMaxSize * 2;
			while (true)
			{
				if (tds == null)
				{
					bool flag = false;
					lock (available)
					{
						if (available.Count > 0)
						{
							tds = (Tds)available.Dequeue();
							goto IL_01a8;
						}
						Monitor.Enter(conns);
						try
						{
							if (conns.Count >= info.PoolMaxSize - in_progress)
							{
								Monitor.Exit(conns);
								if (!Monitor.Wait(available, info.Timeout * 1000))
								{
									throw new InvalidOperationException("Timeout expired. The timeout period elapsed before a connection could be obtained. A possible explanation is that all the connections in the pool are in use, and the maximum pool size is reached.");
								}
								if (available.Count > 0)
								{
									tds = (Tds)available.Dequeue();
									goto IL_01a8;
								}
								continue;
							}
							flag = true;
							in_progress++;
						}
						finally
						{
							Monitor.Exit(conns);
						}
					}
					if (!flag)
					{
						continue;
					}
					try
					{
						tds = manager.CreateConnection(info);
						lock (conns)
						{
							conns.Add(tds);
						}
						return tds;
					}
					finally
					{
						lock (available)
						{
							in_progress--;
						}
					}
				}
				goto IL_01a8;
				IL_01a8:
				bool flag2 = true;
				Exception ex = null;
				try
				{
					flag2 = !tds.IsConnected || !tds.Reset();
				}
				catch (Exception ex2)
				{
					flag2 = true;
					ex = ex2;
				}
				if (!flag2)
				{
					break;
				}
				lock (conns)
				{
					conns.Remove(tds);
				}
				tds.Disconnect();
				num--;
				if (num != 0)
				{
					continue;
				}
				throw ex;
			}
			return tds;
		}

		public void ReleaseConnection(Tds connection)
		{
			if (connection == null)
			{
				return;
			}
			if (no_pooling)
			{
				connection.Disconnect();
				return;
			}
			if (connection.poolStatus == 2)
			{
				lock (conns)
				{
					conns.Remove(connection);
				}
				connection.Disconnect();
				connection = null;
			}
			lock (available)
			{
				if (connection != null)
				{
					available.Enqueue(connection);
				}
				Monitor.Pulse(available);
			}
		}

		public void ResetConnectionPool()
		{
			lock (available)
			{
				lock (conns)
				{
					for (int num = conns.Count - 1; num >= 0; num--)
					{
						Tds tds = (Tds)conns[num];
						tds.poolStatus = 2;
					}
					for (int num = available.Count - 1; num >= 0; num--)
					{
						Tds tds = (Tds)available.Dequeue();
						tds.Disconnect();
						conns.Remove(tds);
					}
					available.Clear();
					InitializePool();
				}
				Monitor.PulseAll(available);
			}
		}
	}
}
