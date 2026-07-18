namespace System.Net.NetworkInformation
{
	internal class Win32IPv4InterfaceStatistics : IPv4InterfaceStatistics
	{
		private System.Net.NetworkInformation.Win32_MIB_IFROW info;

		public override long BytesReceived
		{
			get
			{
				return info.InOctets;
			}
		}

		public override long BytesSent
		{
			get
			{
				return info.OutOctets;
			}
		}

		public override long IncomingPacketsDiscarded
		{
			get
			{
				return info.InDiscards;
			}
		}

		public override long IncomingPacketsWithErrors
		{
			get
			{
				return info.InErrors;
			}
		}

		public override long IncomingUnknownProtocolPackets
		{
			get
			{
				return info.InUnknownProtos;
			}
		}

		public override long NonUnicastPacketsReceived
		{
			get
			{
				return info.InNUcastPkts;
			}
		}

		public override long NonUnicastPacketsSent
		{
			get
			{
				return info.OutNUcastPkts;
			}
		}

		public override long OutgoingPacketsDiscarded
		{
			get
			{
				return info.OutDiscards;
			}
		}

		public override long OutgoingPacketsWithErrors
		{
			get
			{
				return info.OutErrors;
			}
		}

		public override long OutputQueueLength
		{
			get
			{
				return info.OutQLen;
			}
		}

		public override long UnicastPacketsReceived
		{
			get
			{
				return info.InUcastPkts;
			}
		}

		public override long UnicastPacketsSent
		{
			get
			{
				return info.OutUcastPkts;
			}
		}

		public Win32IPv4InterfaceStatistics(System.Net.NetworkInformation.Win32_MIB_IFROW info)
		{
			this.info = info;
		}
	}
}
