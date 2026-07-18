using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class Win32GatewayIPAddressInformationCollection : GatewayIPAddressInformationCollection
	{
		public static readonly System.Net.NetworkInformation.Win32GatewayIPAddressInformationCollection Empty = new System.Net.NetworkInformation.Win32GatewayIPAddressInformationCollection(true);

		private bool is_readonly;

		public override bool IsReadOnly
		{
			get
			{
				return is_readonly;
			}
		}

		private Win32GatewayIPAddressInformationCollection(bool isReadOnly)
		{
			is_readonly = isReadOnly;
		}

		public Win32GatewayIPAddressInformationCollection(params System.Net.NetworkInformation.Win32_IP_ADDR_STRING[] al)
		{
			for (int i = 0; i < al.Length; i++)
			{
				System.Net.NetworkInformation.Win32_IP_ADDR_STRING win32_IP_ADDR_STRING = al[i];
				if (!string.IsNullOrEmpty(win32_IP_ADDR_STRING.IpAddress))
				{
					Add(new System.Net.NetworkInformation.GatewayIPAddressInformationImpl(IPAddress.Parse(win32_IP_ADDR_STRING.IpAddress)));
					AddSubsequently(win32_IP_ADDR_STRING.Next);
				}
			}
			is_readonly = true;
		}

		private void AddSubsequently(IntPtr head)
		{
			IntPtr intPtr = head;
			while (intPtr != IntPtr.Zero)
			{
				System.Net.NetworkInformation.Win32_IP_ADDR_STRING win32_IP_ADDR_STRING = (System.Net.NetworkInformation.Win32_IP_ADDR_STRING)Marshal.PtrToStructure(intPtr, typeof(System.Net.NetworkInformation.Win32_IP_ADDR_STRING));
				Add(new System.Net.NetworkInformation.GatewayIPAddressInformationImpl(IPAddress.Parse(win32_IP_ADDR_STRING.IpAddress)));
				intPtr = win32_IP_ADDR_STRING.Next;
			}
		}
	}
}
