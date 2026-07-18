namespace System.Net.NetworkInformation
{
	internal class LinuxGatewayIPAddressInformationCollection : GatewayIPAddressInformationCollection
	{
		public static readonly System.Net.NetworkInformation.LinuxGatewayIPAddressInformationCollection Empty = new System.Net.NetworkInformation.LinuxGatewayIPAddressInformationCollection(true);

		private bool is_readonly;

		public override bool IsReadOnly
		{
			get
			{
				return is_readonly;
			}
		}

		private LinuxGatewayIPAddressInformationCollection(bool isReadOnly)
		{
			is_readonly = isReadOnly;
		}

		public LinuxGatewayIPAddressInformationCollection(IPAddressCollection col)
		{
			foreach (IPAddress item in col)
			{
				Add(new System.Net.NetworkInformation.GatewayIPAddressInformationImpl(item));
			}
			is_readonly = true;
		}
	}
}
