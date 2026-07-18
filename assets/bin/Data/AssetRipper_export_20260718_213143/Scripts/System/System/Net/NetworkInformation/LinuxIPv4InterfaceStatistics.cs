namespace System.Net.NetworkInformation
{
	internal class LinuxIPv4InterfaceStatistics : IPv4InterfaceStatistics
	{
		private System.Net.NetworkInformation.LinuxNetworkInterface linux;

		public override long BytesReceived
		{
			get
			{
				return Read("statistics/rx_bytes");
			}
		}

		public override long BytesSent
		{
			get
			{
				return Read("statistics/tx_bytes");
			}
		}

		public override long IncomingPacketsDiscarded
		{
			get
			{
				return Read("statistics/rx_dropped");
			}
		}

		public override long IncomingPacketsWithErrors
		{
			get
			{
				return Read("statistics/rx_errors");
			}
		}

		public override long IncomingUnknownProtocolPackets
		{
			get
			{
				return 0L;
			}
		}

		public override long NonUnicastPacketsReceived
		{
			get
			{
				return Read("statistics/multicast");
			}
		}

		public override long NonUnicastPacketsSent
		{
			get
			{
				return Read("statistics/multicast");
			}
		}

		public override long OutgoingPacketsDiscarded
		{
			get
			{
				return Read("statistics/tx_dropped");
			}
		}

		public override long OutgoingPacketsWithErrors
		{
			get
			{
				return Read("statistics/tx_errors");
			}
		}

		public override long OutputQueueLength
		{
			get
			{
				return 1024L;
			}
		}

		public override long UnicastPacketsReceived
		{
			get
			{
				return Read("statistics/rx_packets");
			}
		}

		public override long UnicastPacketsSent
		{
			get
			{
				return Read("statistics/tx_packets");
			}
		}

		public LinuxIPv4InterfaceStatistics(System.Net.NetworkInformation.LinuxNetworkInterface parent)
		{
			linux = parent;
		}

		private long Read(string file)
		{
			try
			{
				return long.Parse(NetworkInterface.ReadLine(linux.IfacePath + file));
			}
			catch
			{
				return 0L;
			}
		}
	}
}
