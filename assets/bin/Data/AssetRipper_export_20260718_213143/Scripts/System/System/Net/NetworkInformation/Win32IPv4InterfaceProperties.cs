using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal sealed class Win32IPv4InterfaceProperties : IPv4InterfaceProperties
	{
		private System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO ainfo;

		private System.Net.NetworkInformation.Win32_IP_PER_ADAPTER_INFO painfo;

		private System.Net.NetworkInformation.Win32_MIB_IFROW mib;

		public override int Index
		{
			get
			{
				return mib.Index;
			}
		}

		public override bool IsAutomaticPrivateAddressingActive
		{
			get
			{
				return painfo.AutoconfigActive != 0;
			}
		}

		public override bool IsAutomaticPrivateAddressingEnabled
		{
			get
			{
				return painfo.AutoconfigEnabled != 0;
			}
		}

		public override bool IsDhcpEnabled
		{
			get
			{
				return ainfo.DhcpEnabled != 0;
			}
		}

		public override bool IsForwardingEnabled
		{
			get
			{
				return System.Net.NetworkInformation.Win32_FIXED_INFO.Instance.EnableRouting != 0;
			}
		}

		public override int Mtu
		{
			get
			{
				return mib.Mtu;
			}
		}

		public override bool UsesWins
		{
			get
			{
				return ainfo.HaveWins;
			}
		}

		public Win32IPv4InterfaceProperties(System.Net.NetworkInformation.Win32_IP_ADAPTER_INFO ainfo, System.Net.NetworkInformation.Win32_MIB_IFROW mib)
		{
			this.ainfo = ainfo;
			this.mib = mib;
			int pOutBufLen = 0;
			GetPerAdapterInfo(mib.Index, null, ref pOutBufLen);
			painfo = new System.Net.NetworkInformation.Win32_IP_PER_ADAPTER_INFO();
			int perAdapterInfo = GetPerAdapterInfo(mib.Index, painfo, ref pOutBufLen);
			if (perAdapterInfo != 0)
			{
				throw new NetworkInformationException(perAdapterInfo);
			}
		}

		[DllImport("iphlpapi.dll")]
		private static extern int GetPerAdapterInfo(int IfIndex, System.Net.NetworkInformation.Win32_IP_PER_ADAPTER_INFO pPerAdapterInfo, ref int pOutBufLen);
	}
}
