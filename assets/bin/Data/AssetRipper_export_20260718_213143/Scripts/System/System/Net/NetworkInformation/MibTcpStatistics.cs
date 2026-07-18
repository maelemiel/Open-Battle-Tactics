using System.Collections.Specialized;
using System.Globalization;

namespace System.Net.NetworkInformation
{
	internal class MibTcpStatistics : TcpStatistics
	{
		private StringDictionary dic;

		public override long ConnectionsAccepted
		{
			get
			{
				return Get("PassiveOpens");
			}
		}

		public override long ConnectionsInitiated
		{
			get
			{
				return Get("ActiveOpens");
			}
		}

		public override long CumulativeConnections
		{
			get
			{
				return Get("NumConns");
			}
		}

		public override long CurrentConnections
		{
			get
			{
				return Get("CurrEstab");
			}
		}

		public override long ErrorsReceived
		{
			get
			{
				return Get("InErrs");
			}
		}

		public override long FailedConnectionAttempts
		{
			get
			{
				return Get("AttemptFails");
			}
		}

		public override long MaximumConnections
		{
			get
			{
				return Get("MaxConn");
			}
		}

		public override long MaximumTransmissionTimeout
		{
			get
			{
				return Get("RtoMax");
			}
		}

		public override long MinimumTransmissionTimeout
		{
			get
			{
				return Get("RtoMin");
			}
		}

		public override long ResetConnections
		{
			get
			{
				return Get("EstabResets");
			}
		}

		public override long ResetsSent
		{
			get
			{
				return Get("OutRsts");
			}
		}

		public override long SegmentsReceived
		{
			get
			{
				return Get("InSegs");
			}
		}

		public override long SegmentsResent
		{
			get
			{
				return Get("RetransSegs");
			}
		}

		public override long SegmentsSent
		{
			get
			{
				return Get("OutSegs");
			}
		}

		public MibTcpStatistics(StringDictionary dic)
		{
			this.dic = dic;
		}

		private long Get(string name)
		{
			return (dic[name] == null) ? 0 : long.Parse(dic[name], NumberFormatInfo.InvariantInfo);
		}
	}
}
