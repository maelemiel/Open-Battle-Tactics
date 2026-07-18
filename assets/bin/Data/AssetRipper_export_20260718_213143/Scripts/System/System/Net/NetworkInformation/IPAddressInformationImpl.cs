namespace System.Net.NetworkInformation
{
	internal class IPAddressInformationImpl : IPAddressInformation
	{
		private IPAddress address;

		private bool is_dns_eligible;

		private bool is_transient;

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
				return is_dns_eligible;
			}
		}

		[System.MonoTODO("Always false on Linux")]
		public override bool IsTransient
		{
			get
			{
				return is_transient;
			}
		}

		public IPAddressInformationImpl(IPAddress address, bool isDnsEligible, bool isTransient)
		{
			this.address = address;
			is_dns_eligible = isDnsEligible;
			is_transient = isTransient;
		}
	}
}
