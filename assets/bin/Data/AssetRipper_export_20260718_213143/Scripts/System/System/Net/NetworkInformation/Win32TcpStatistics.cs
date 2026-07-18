namespace System.Net.NetworkInformation
{
	internal class Win32TcpStatistics : TcpStatistics
	{
		private System.Net.NetworkInformation.Win32_MIB_TCPSTATS info;

		public override long ConnectionsAccepted
		{
			get
			{
				return info.PassiveOpens;
			}
		}

		public override long ConnectionsInitiated
		{
			get
			{
				return info.ActiveOpens;
			}
		}

		public override long CumulativeConnections
		{
			get
			{
				return info.NumConns;
			}
		}

		public override long CurrentConnections
		{
			get
			{
				return info.CurrEstab;
			}
		}

		public override long ErrorsReceived
		{
			get
			{
				return info.InErrs;
			}
		}

		public override long FailedConnectionAttempts
		{
			get
			{
				return info.AttemptFails;
			}
		}

		public override long MaximumConnections
		{
			get
			{
				return info.MaxConn;
			}
		}

		public override long MaximumTransmissionTimeout
		{
			get
			{
				return info.RtoMax;
			}
		}

		public override long MinimumTransmissionTimeout
		{
			get
			{
				return info.RtoMin;
			}
		}

		public override long ResetConnections
		{
			get
			{
				return info.EstabResets;
			}
		}

		public override long ResetsSent
		{
			get
			{
				return info.OutRsts;
			}
		}

		public override long SegmentsReceived
		{
			get
			{
				return info.InSegs;
			}
		}

		public override long SegmentsResent
		{
			get
			{
				return info.RetransSegs;
			}
		}

		public override long SegmentsSent
		{
			get
			{
				return info.OutSegs;
			}
		}

		public Win32TcpStatistics(System.Net.NetworkInformation.Win32_MIB_TCPSTATS info)
		{
			this.info = info;
		}
	}
}
