namespace System.Net.NetworkInformation
{
	internal class Win32IPGlobalStatistics : IPGlobalStatistics
	{
		private System.Net.NetworkInformation.Win32_MIB_IPSTATS info;

		public override int DefaultTtl
		{
			get
			{
				return info.DefaultTTL;
			}
		}

		public override bool ForwardingEnabled
		{
			get
			{
				return info.Forwarding != 0;
			}
		}

		public override int NumberOfInterfaces
		{
			get
			{
				return info.NumIf;
			}
		}

		public override int NumberOfIPAddresses
		{
			get
			{
				return info.NumAddr;
			}
		}

		public override int NumberOfRoutes
		{
			get
			{
				return info.NumRoutes;
			}
		}

		public override long OutputPacketRequests
		{
			get
			{
				return info.OutRequests;
			}
		}

		public override long OutputPacketRoutingDiscards
		{
			get
			{
				return info.RoutingDiscards;
			}
		}

		public override long OutputPacketsDiscarded
		{
			get
			{
				return info.OutDiscards;
			}
		}

		public override long OutputPacketsWithNoRoute
		{
			get
			{
				return info.OutNoRoutes;
			}
		}

		public override long PacketFragmentFailures
		{
			get
			{
				return info.FragFails;
			}
		}

		public override long PacketReassembliesRequired
		{
			get
			{
				return info.ReasmReqds;
			}
		}

		public override long PacketReassemblyFailures
		{
			get
			{
				return info.ReasmFails;
			}
		}

		public override long PacketReassemblyTimeout
		{
			get
			{
				return info.ReasmTimeout;
			}
		}

		public override long PacketsFragmented
		{
			get
			{
				return info.FragOks;
			}
		}

		public override long PacketsReassembled
		{
			get
			{
				return info.ReasmOks;
			}
		}

		public override long ReceivedPackets
		{
			get
			{
				return info.InReceives;
			}
		}

		public override long ReceivedPacketsDelivered
		{
			get
			{
				return info.InDelivers;
			}
		}

		public override long ReceivedPacketsDiscarded
		{
			get
			{
				return info.InDiscards;
			}
		}

		public override long ReceivedPacketsForwarded
		{
			get
			{
				return info.ForwDatagrams;
			}
		}

		public override long ReceivedPacketsWithAddressErrors
		{
			get
			{
				return info.InAddrErrors;
			}
		}

		public override long ReceivedPacketsWithHeadersErrors
		{
			get
			{
				return info.InHdrErrors;
			}
		}

		public override long ReceivedPacketsWithUnknownProtocol
		{
			get
			{
				return info.InUnknownProtos;
			}
		}

		public Win32IPGlobalStatistics(System.Net.NetworkInformation.Win32_MIB_IPSTATS info)
		{
			this.info = info;
		}
	}
}
