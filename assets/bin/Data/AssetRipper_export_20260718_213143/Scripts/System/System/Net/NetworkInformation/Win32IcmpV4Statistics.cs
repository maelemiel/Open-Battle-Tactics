namespace System.Net.NetworkInformation
{
	internal class Win32IcmpV4Statistics : IcmpV4Statistics
	{
		private System.Net.NetworkInformation.Win32_MIBICMPSTATS iin;

		private System.Net.NetworkInformation.Win32_MIBICMPSTATS iout;

		public override long AddressMaskRepliesReceived
		{
			get
			{
				return iin.AddrMaskReps;
			}
		}

		public override long AddressMaskRepliesSent
		{
			get
			{
				return iout.AddrMaskReps;
			}
		}

		public override long AddressMaskRequestsReceived
		{
			get
			{
				return iin.AddrMasks;
			}
		}

		public override long AddressMaskRequestsSent
		{
			get
			{
				return iout.AddrMasks;
			}
		}

		public override long DestinationUnreachableMessagesReceived
		{
			get
			{
				return iin.DestUnreachs;
			}
		}

		public override long DestinationUnreachableMessagesSent
		{
			get
			{
				return iout.DestUnreachs;
			}
		}

		public override long EchoRepliesReceived
		{
			get
			{
				return iin.EchoReps;
			}
		}

		public override long EchoRepliesSent
		{
			get
			{
				return iout.EchoReps;
			}
		}

		public override long EchoRequestsReceived
		{
			get
			{
				return iin.Echos;
			}
		}

		public override long EchoRequestsSent
		{
			get
			{
				return iout.Echos;
			}
		}

		public override long ErrorsReceived
		{
			get
			{
				return iin.Errors;
			}
		}

		public override long ErrorsSent
		{
			get
			{
				return iout.Errors;
			}
		}

		public override long MessagesReceived
		{
			get
			{
				return iin.Msgs;
			}
		}

		public override long MessagesSent
		{
			get
			{
				return iout.Msgs;
			}
		}

		public override long ParameterProblemsReceived
		{
			get
			{
				return iin.ParmProbs;
			}
		}

		public override long ParameterProblemsSent
		{
			get
			{
				return iout.ParmProbs;
			}
		}

		public override long RedirectsReceived
		{
			get
			{
				return iin.Redirects;
			}
		}

		public override long RedirectsSent
		{
			get
			{
				return iout.Redirects;
			}
		}

		public override long SourceQuenchesReceived
		{
			get
			{
				return iin.SrcQuenchs;
			}
		}

		public override long SourceQuenchesSent
		{
			get
			{
				return iout.SrcQuenchs;
			}
		}

		public override long TimeExceededMessagesReceived
		{
			get
			{
				return iin.TimeExcds;
			}
		}

		public override long TimeExceededMessagesSent
		{
			get
			{
				return iout.TimeExcds;
			}
		}

		public override long TimestampRepliesReceived
		{
			get
			{
				return iin.TimestampReps;
			}
		}

		public override long TimestampRepliesSent
		{
			get
			{
				return iout.TimestampReps;
			}
		}

		public override long TimestampRequestsReceived
		{
			get
			{
				return iin.Timestamps;
			}
		}

		public override long TimestampRequestsSent
		{
			get
			{
				return iout.Timestamps;
			}
		}

		public Win32IcmpV4Statistics(System.Net.NetworkInformation.Win32_MIBICMPINFO info)
		{
			iin = info.InStats;
			iout = info.OutStats;
		}
	}
}
