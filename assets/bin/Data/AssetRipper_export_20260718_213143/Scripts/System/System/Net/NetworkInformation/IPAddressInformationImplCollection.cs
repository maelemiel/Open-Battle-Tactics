using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class IPAddressInformationImplCollection : IPAddressInformationCollection
	{
		public static readonly System.Net.NetworkInformation.IPAddressInformationImplCollection Empty = new System.Net.NetworkInformation.IPAddressInformationImplCollection(true);

		private bool is_readonly;

		public override bool IsReadOnly
		{
			get
			{
				return is_readonly;
			}
		}

		private IPAddressInformationImplCollection(bool isReadOnly)
		{
			is_readonly = isReadOnly;
		}

		public static IPAddressInformationCollection Win32FromAnycast(IntPtr ptr)
		{
			System.Net.NetworkInformation.IPAddressInformationImplCollection iPAddressInformationImplCollection = new System.Net.NetworkInformation.IPAddressInformationImplCollection(false);
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				System.Net.NetworkInformation.Win32_IP_ADAPTER_ANYCAST_ADDRESS win32_IP_ADAPTER_ANYCAST_ADDRESS = (System.Net.NetworkInformation.Win32_IP_ADAPTER_ANYCAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(System.Net.NetworkInformation.Win32_IP_ADAPTER_ANYCAST_ADDRESS));
				iPAddressInformationImplCollection.Add(new System.Net.NetworkInformation.IPAddressInformationImpl(win32_IP_ADAPTER_ANYCAST_ADDRESS.Address.GetIPAddress(), win32_IP_ADAPTER_ANYCAST_ADDRESS.LengthFlags.IsDnsEligible, win32_IP_ADAPTER_ANYCAST_ADDRESS.LengthFlags.IsTransient));
				intPtr = win32_IP_ADAPTER_ANYCAST_ADDRESS.Next;
			}
			iPAddressInformationImplCollection.is_readonly = true;
			return iPAddressInformationImplCollection;
		}

		public static System.Net.NetworkInformation.IPAddressInformationImplCollection LinuxFromAnycast(IList<IPAddress> addresses)
		{
			System.Net.NetworkInformation.IPAddressInformationImplCollection iPAddressInformationImplCollection = new System.Net.NetworkInformation.IPAddressInformationImplCollection(false);
			foreach (IPAddress address in addresses)
			{
				iPAddressInformationImplCollection.Add(new System.Net.NetworkInformation.IPAddressInformationImpl(address, false, false));
			}
			iPAddressInformationImplCollection.is_readonly = true;
			return iPAddressInformationImplCollection;
		}
	}
}
