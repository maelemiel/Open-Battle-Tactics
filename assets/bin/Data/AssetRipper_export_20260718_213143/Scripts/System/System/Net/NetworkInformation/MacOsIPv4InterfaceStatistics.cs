namespace System.Net.NetworkInformation
{
	internal class MacOsIPv4InterfaceStatistics : IPv4InterfaceStatistics
	{
		private System.Net.NetworkInformation.MacOsNetworkInterface macos;

		public override long BytesReceived
		{
			get
			{
				return 0L;
			}
		}

		public override long BytesSent
		{
			get
			{
				return 0L;
			}
		}

		public override long IncomingPacketsDiscarded
		{
			get
			{
				return 0L;
			}
		}

		public override long IncomingPacketsWithErrors
		{
			get
			{
				return 0L;
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
				return 0L;
			}
		}

		public override long NonUnicastPacketsSent
		{
			get
			{
				return 0L;
			}
		}

		public override long OutgoingPacketsDiscarded
		{
			get
			{
				return 0L;
			}
		}

		public override long OutgoingPacketsWithErrors
		{
			get
			{
				return 0L;
			}
		}

		public override long OutputQueueLength
		{
			get
			{
				return 0L;
			}
		}

		public override long UnicastPacketsReceived
		{
			get
			{
				return 0L;
			}
		}

		public override long UnicastPacketsSent
		{
			get
			{
				return 0L;
			}
		}

		public MacOsIPv4InterfaceStatistics(System.Net.NetworkInformation.MacOsNetworkInterface parent)
		{
			macos = parent;
		}
	}
}
