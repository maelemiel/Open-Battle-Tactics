namespace System.Net.NetworkInformation
{
	public class PingReply
	{
		private IPAddress address;

		private byte[] buffer;

		private PingOptions options;

		private long rtt;

		private IPStatus status;

		public IPAddress Address
		{
			get
			{
				return address;
			}
		}

		public byte[] Buffer
		{
			get
			{
				return buffer;
			}
		}

		public PingOptions Options
		{
			get
			{
				return options;
			}
		}

		public long RoundtripTime
		{
			get
			{
				return rtt;
			}
		}

		public IPStatus Status
		{
			get
			{
				return status;
			}
		}

		internal PingReply(IPAddress address, byte[] buffer, PingOptions options, long roundtripTime, IPStatus status)
		{
			this.address = address;
			this.buffer = buffer;
			this.options = options;
			rtt = roundtripTime;
			this.status = status;
		}
	}
}
