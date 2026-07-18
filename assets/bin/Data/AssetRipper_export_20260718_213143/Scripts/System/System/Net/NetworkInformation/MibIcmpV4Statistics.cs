using System.Collections.Specialized;
using System.Globalization;

namespace System.Net.NetworkInformation
{
	internal class MibIcmpV4Statistics : IcmpV4Statistics
	{
		private StringDictionary dic;

		public override long AddressMaskRepliesReceived
		{
			get
			{
				return Get("InAddrMaskReps");
			}
		}

		public override long AddressMaskRepliesSent
		{
			get
			{
				return Get("OutAddrMaskReps");
			}
		}

		public override long AddressMaskRequestsReceived
		{
			get
			{
				return Get("InAddrMasks");
			}
		}

		public override long AddressMaskRequestsSent
		{
			get
			{
				return Get("OutAddrMasks");
			}
		}

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
				return Get("InEchoReps");
			}
		}

		public override long EchoRepliesSent
		{
			get
			{
				return Get("OutEchoReps");
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

		public override long ParameterProblemsReceived
		{
			get
			{
				return Get("InParmProbs");
			}
		}

		public override long ParameterProblemsSent
		{
			get
			{
				return Get("OutParmProbs");
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

		public override long SourceQuenchesReceived
		{
			get
			{
				return Get("InSrcQuenchs");
			}
		}

		public override long SourceQuenchesSent
		{
			get
			{
				return Get("OutSrcQuenchs");
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

		public override long TimestampRepliesReceived
		{
			get
			{
				return Get("InTimestampReps");
			}
		}

		public override long TimestampRepliesSent
		{
			get
			{
				return Get("OutTimestampReps");
			}
		}

		public override long TimestampRequestsReceived
		{
			get
			{
				return Get("InTimestamps");
			}
		}

		public override long TimestampRequestsSent
		{
			get
			{
				return Get("OutTimestamps");
			}
		}

		public MibIcmpV4Statistics(StringDictionary dic)
		{
			this.dic = dic;
		}

		private long Get(string name)
		{
			return (dic[name] == null) ? 0 : long.Parse(dic[name], NumberFormatInfo.InvariantInfo);
		}
	}
}
