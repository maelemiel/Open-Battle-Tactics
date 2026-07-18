namespace System.Net.NetworkInformation
{
	internal class Win32IPv6InterfaceProperties : IPv6InterfaceProperties
	{
		private System.Net.NetworkInformation.Win32_MIB_IFROW mib;

		public override int Index
		{
			get
			{
				return mib.Index;
			}
		}

		public override int Mtu
		{
			get
			{
				return mib.Mtu;
			}
		}

		public Win32IPv6InterfaceProperties(System.Net.NetworkInformation.Win32_MIB_IFROW mib)
		{
			this.mib = mib;
		}
	}
}
