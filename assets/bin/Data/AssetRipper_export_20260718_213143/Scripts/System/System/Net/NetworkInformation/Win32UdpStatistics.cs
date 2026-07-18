namespace System.Net.NetworkInformation
{
	internal class Win32UdpStatistics : UdpStatistics
	{
		private System.Net.NetworkInformation.Win32_MIB_UDPSTATS info;

		public override long DatagramsReceived
		{
			get
			{
				return info.InDatagrams;
			}
		}

		public override long DatagramsSent
		{
			get
			{
				return info.OutDatagrams;
			}
		}

		public override long IncomingDatagramsDiscarded
		{
			get
			{
				return info.NoPorts;
			}
		}

		public override long IncomingDatagramsWithErrors
		{
			get
			{
				return info.InErrors;
			}
		}

		public override int UdpListeners
		{
			get
			{
				return info.NumAddrs;
			}
		}

		public Win32UdpStatistics(System.Net.NetworkInformation.Win32_MIB_UDPSTATS info)
		{
			this.info = info;
		}
	}
}
