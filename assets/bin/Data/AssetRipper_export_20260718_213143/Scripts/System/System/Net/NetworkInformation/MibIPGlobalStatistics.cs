using System.Collections.Specialized;
using System.Globalization;

namespace System.Net.NetworkInformation
{
	internal class MibIPGlobalStatistics : IPGlobalStatistics
	{
		private StringDictionary dic;

		public override int DefaultTtl
		{
			get
			{
				return (int)Get("DefaultTTL");
			}
		}

		public override bool ForwardingEnabled
		{
			get
			{
				return Get("Forwarding") != 0;
			}
		}

		public override int NumberOfInterfaces
		{
			get
			{
				return (int)Get("NumIf");
			}
		}

		public override int NumberOfIPAddresses
		{
			get
			{
				return (int)Get("NumAddr");
			}
		}

		public override int NumberOfRoutes
		{
			get
			{
				return (int)Get("NumRoutes");
			}
		}

		public override long OutputPacketRequests
		{
			get
			{
				return Get("OutRequests");
			}
		}

		public override long OutputPacketRoutingDiscards
		{
			get
			{
				return Get("RoutingDiscards");
			}
		}

		public override long OutputPacketsDiscarded
		{
			get
			{
				return Get("OutDiscards");
			}
		}

		public override long OutputPacketsWithNoRoute
		{
			get
			{
				return Get("OutNoRoutes");
			}
		}

		public override long PacketFragmentFailures
		{
			get
			{
				return Get("FragFails");
			}
		}

		public override long PacketReassembliesRequired
		{
			get
			{
				return Get("ReasmReqds");
			}
		}

		public override long PacketReassemblyFailures
		{
			get
			{
				return Get("ReasmFails");
			}
		}

		public override long PacketReassemblyTimeout
		{
			get
			{
				return Get("ReasmTimeout");
			}
		}

		public override long PacketsFragmented
		{
			get
			{
				return Get("FragOks");
			}
		}

		public override long PacketsReassembled
		{
			get
			{
				return Get("ReasmOks");
			}
		}

		public override long ReceivedPackets
		{
			get
			{
				return Get("InReceives");
			}
		}

		public override long ReceivedPacketsDelivered
		{
			get
			{
				return Get("InDelivers");
			}
		}

		public override long ReceivedPacketsDiscarded
		{
			get
			{
				return Get("InDiscards");
			}
		}

		public override long ReceivedPacketsForwarded
		{
			get
			{
				return Get("ForwDatagrams");
			}
		}

		public override long ReceivedPacketsWithAddressErrors
		{
			get
			{
				return Get("InAddrErrors");
			}
		}

		public override long ReceivedPacketsWithHeadersErrors
		{
			get
			{
				return Get("InHdrErrors");
			}
		}

		public override long ReceivedPacketsWithUnknownProtocol
		{
			get
			{
				return Get("InUnknownProtos");
			}
		}

		public MibIPGlobalStatistics(StringDictionary dic)
		{
			this.dic = dic;
		}

		private long Get(string name)
		{
			return (dic[name] == null) ? 0 : long.Parse(dic[name], NumberFormatInfo.InvariantInfo);
		}
	}
}
