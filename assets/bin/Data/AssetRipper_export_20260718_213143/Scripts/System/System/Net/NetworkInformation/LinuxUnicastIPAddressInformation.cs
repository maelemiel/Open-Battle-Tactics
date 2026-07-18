namespace System.Net.NetworkInformation
{
	internal class LinuxUnicastIPAddressInformation : UnicastIPAddressInformation
	{
		private IPAddress address;

		public override IPAddress Address
		{
			get
			{
				return address;
			}
		}

		public override bool IsDnsEligible
		{
			get
			{
				byte[] addressBytes = address.GetAddressBytes();
				return addressBytes[0] != 169 || addressBytes[1] != 254;
			}
		}

		[System.MonoTODO("Always returns false")]
		public override bool IsTransient
		{
			get
			{
				return false;
			}
		}

		public override long AddressPreferredLifetime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long AddressValidLifetime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long DhcpLeaseLifetime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override DuplicateAddressDetectionState DuplicateAddressDetectionState
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override IPAddress IPv4Mask
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override PrefixOrigin PrefixOrigin
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override SuffixOrigin SuffixOrigin
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public LinuxUnicastIPAddressInformation(IPAddress address)
		{
			this.address = address;
		}
	}
}
