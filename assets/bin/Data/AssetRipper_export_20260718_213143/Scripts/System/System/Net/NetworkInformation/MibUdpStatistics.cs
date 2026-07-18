using System.Collections.Specialized;
using System.Globalization;

namespace System.Net.NetworkInformation
{
	internal class MibUdpStatistics : UdpStatistics
	{
		private StringDictionary dic;

		public override long DatagramsReceived
		{
			get
			{
				return Get("InDatagrams");
			}
		}

		public override long DatagramsSent
		{
			get
			{
				return Get("OutDatagrams");
			}
		}

		public override long IncomingDatagramsDiscarded
		{
			get
			{
				return Get("NoPorts");
			}
		}

		public override long IncomingDatagramsWithErrors
		{
			get
			{
				return Get("InErrors");
			}
		}

		public override int UdpListeners
		{
			get
			{
				return (int)Get("NumAddrs");
			}
		}

		public MibUdpStatistics(StringDictionary dic)
		{
			this.dic = dic;
		}

		private long Get(string name)
		{
			return (dic[name] == null) ? 0 : long.Parse(dic[name], NumberFormatInfo.InvariantInfo);
		}
	}
}
