namespace System.Net.NetworkInformation
{
	internal class Win32IcmpV6Statistics : IcmpV6Statistics
	{
		private System.Net.NetworkInformation.Win32_MIBICMPSTATS_EX iin;

		private System.Net.NetworkInformation.Win32_MIBICMPSTATS_EX iout;

		public override long DestinationUnreachableMessagesReceived
		{
			get
			{
				return iin.Counts[1];
			}
		}

		public override long DestinationUnreachableMessagesSent
		{
			get
			{
				return iout.Counts[1];
			}
		}

		public override long EchoRepliesReceived
		{
			get
			{
				return iin.Counts[129];
			}
		}

		public override long EchoRepliesSent
		{
			get
			{
				return iout.Counts[129];
			}
		}

		public override long EchoRequestsReceived
		{
			get
			{
				return iin.Counts[128];
			}
		}

		public override long EchoRequestsSent
		{
			get
			{
				return iout.Counts[128];
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

		public override long MembershipQueriesReceived
		{
			get
			{
				return iin.Counts[130];
			}
		}

		public override long MembershipQueriesSent
		{
			get
			{
				return iout.Counts[130];
			}
		}

		public override long MembershipReductionsReceived
		{
			get
			{
				return iin.Counts[132];
			}
		}

		public override long MembershipReductionsSent
		{
			get
			{
				return iout.Counts[132];
			}
		}

		public override long MembershipReportsReceived
		{
			get
			{
				return iin.Counts[131];
			}
		}

		public override long MembershipReportsSent
		{
			get
			{
				return iout.Counts[131];
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

		public override long NeighborAdvertisementsReceived
		{
			get
			{
				return iin.Counts[136];
			}
		}

		public override long NeighborAdvertisementsSent
		{
			get
			{
				return iout.Counts[136];
			}
		}

		public override long NeighborSolicitsReceived
		{
			get
			{
				return iin.Counts[135];
			}
		}

		public override long NeighborSolicitsSent
		{
			get
			{
				return iout.Counts[135];
			}
		}

		public override long PacketTooBigMessagesReceived
		{
			get
			{
				return iin.Counts[2];
			}
		}

		public override long PacketTooBigMessagesSent
		{
			get
			{
				return iout.Counts[2];
			}
		}

		public override long ParameterProblemsReceived
		{
			get
			{
				return iin.Counts[4];
			}
		}

		public override long ParameterProblemsSent
		{
			get
			{
				return iout.Counts[4];
			}
		}

		public override long RedirectsReceived
		{
			get
			{
				return iin.Counts[137];
			}
		}

		public override long RedirectsSent
		{
			get
			{
				return iout.Counts[137];
			}
		}

		public override long RouterAdvertisementsReceived
		{
			get
			{
				return iin.Counts[134];
			}
		}

		public override long RouterAdvertisementsSent
		{
			get
			{
				return iout.Counts[134];
			}
		}

		public override long RouterSolicitsReceived
		{
			get
			{
				return iin.Counts[133];
			}
		}

		public override long RouterSolicitsSent
		{
			get
			{
				return iout.Counts[133];
			}
		}

		public override long TimeExceededMessagesReceived
		{
			get
			{
				return iin.Counts[3];
			}
		}

		public override long TimeExceededMessagesSent
		{
			get
			{
				return iout.Counts[3];
			}
		}

		public Win32IcmpV6Statistics(System.Net.NetworkInformation.Win32_MIB_ICMP_EX info)
		{
			iin = info.InStats;
			iout = info.OutStats;
		}
	}
}
