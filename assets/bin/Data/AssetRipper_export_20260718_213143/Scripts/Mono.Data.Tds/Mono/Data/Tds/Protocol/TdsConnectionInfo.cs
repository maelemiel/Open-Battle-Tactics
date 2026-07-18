using System.Text;

namespace Mono.Data.Tds.Protocol
{
	public class TdsConnectionInfo
	{
		public string DataSource;

		public int Port;

		public int PacketSize;

		public int Timeout;

		public int PoolMinSize;

		public int PoolMaxSize;

		public TdsConnectionInfo(string dataSource, int port, int packetSize, int timeout, int minSize, int maxSize)
		{
			DataSource = dataSource;
			Port = port;
			PacketSize = packetSize;
			Timeout = timeout;
			PoolMinSize = minSize;
			PoolMaxSize = maxSize;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("DataSouce: {0}\n", DataSource);
			stringBuilder.AppendFormat("Port: {0}\n", Port);
			stringBuilder.AppendFormat("PacketSize: {0}\n", PacketSize);
			stringBuilder.AppendFormat("Timeout: {0}\n", Timeout);
			stringBuilder.AppendFormat("PoolMinSize: {0}\n", PoolMinSize);
			stringBuilder.AppendFormat("PoolMaxSize: {0}", PoolMaxSize);
			return stringBuilder.ToString();
		}
	}
}
