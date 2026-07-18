using System.Collections.Specialized;
using System.Globalization;

namespace System.Net.NetworkInformation
{
	internal class MibIcmpV6Statistics : IcmpV6Statistics
	{
		private StringDictionary dic;

		public override long DestinationUnreachableMessagesReceived
		{
			get
			{
				return Get("InDestUnreachs");
			}
		}

		public override long DestinationUnreachableMessagesSent
		{
			get
			{
				return Get("OutDestUnreachs");
			}
		}

		public override long EchoRepliesReceived
		{
			get
			{
				return Get("InEchoReplies");
			}
		}

		public override long EchoRepliesSent
		{
			get
			{
				return Get("OutEchoReplies");
			}
		}

		public override long EchoRequestsReceived
		{
			get
			{
				return Get("InEchos");
			}
		}

		public override long EchoRequestsSent
		{
			get
			{
				return Get("OutEchos");
			}
		}

		public override long ErrorsReceived
		{
			get
			{
				return Get("InErrors");
			}
		}

		public override long ErrorsSent
		{
			get
			{
				return Get("OutErrors");
			}
		}

		public override long MembershipQueriesReceived
		{
			get
			{
				return Get("InGroupMembQueries");
			}
		}

		public override long MembershipQueriesSent
		{
			get
			{
				return Get("OutGroupMembQueries");
			}
		}

		public override long MembershipReductionsReceived
		{
			get
			{
				return Get("InGroupMembReductiions");
			}
		}

		public override long MembershipReductionsSent
		{
			get
			{
				return Get("OutGroupMembReductiions");
			}
		}

		public override long MembershipReportsReceived
		{
			get
			{
				return Get("InGroupMembRespons");
			}
		}

		public override long MembershipReportsSent
		{
			get
			{
				return Get("OutGroupMembRespons");
			}
		}

		public override long MessagesReceived
		{
			get
			{
				return Get("InMsgs");
			}
		}

		public override long MessagesSent
		{
			get
			{
				return Get("OutMsgs");
			}
		}

		public override long NeighborAdvertisementsReceived
		{
			get
			{
				return Get("InNeighborAdvertisements");
			}
		}

		public override long NeighborAdvertisementsSent
		{
			get
			{
				return Get("OutNeighborAdvertisements");
			}
		}

		public override long NeighborSolicitsReceived
		{
			get
			{
				return Get("InNeighborSolicits");
			}
		}

		public override long NeighborSolicitsSent
		{
			get
			{
				return Get("OutNeighborSolicits");
			}
		}

		public override long PacketTooBigMessagesReceived
		{
			get
			{
				return Get("InPktTooBigs");
			}
		}

		public override long PacketTooBigMessagesSent
		{
			get
			{
				return Get("OutPktTooBigs");
			}
		}

		public override long ParameterProblemsReceived
		{
			get
			{
				return Get("InParmProblems");
			}
		}

		public override long ParameterProblemsSent
		{
			get
			{
				return Get("OutParmProblems");
			}
		}

		public override long RedirectsReceived
		{
			get
			{
				return Get("InRedirects");
			}
		}

		public override long RedirectsSent
		{
			get
			{
				return Get("OutRedirects");
			}
		}

		public override long RouterAdvertisementsReceived
		{
			get
			{
				return Get("InRouterAdvertisements");
			}
		}

		public override long RouterAdvertisementsSent
		{
			get
			{
				return Get("OutRouterAdvertisements");
			}
		}

		public override long RouterSolicitsReceived
		{
			get
			{
				return Get("InRouterSolicits");
			}
		}

		public override long RouterSolicitsSent
		{
			get
			{
				return Get("OutRouterSolicits");
			}
		}

		public override long TimeExceededMessagesReceived
		{
			get
			{
				return Get("InTimeExcds");
			}
		}

		public override long TimeExceededMessagesSent
		{
			get
			{
				return Get("OutTimeExcds");
			}
		}

		public MibIcmpV6Statistics(StringDictionary dic)
		{
			this.dic = dic;
		}

		private long Get(string name)
		{
			return (dic[name] == null) ? 0 : long.Parse(dic[name], NumberFormatInfo.InvariantInfo);
		}
	}
}
