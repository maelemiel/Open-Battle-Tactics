using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class MulticastIPAddressInformationImplCollection : MulticastIPAddressInformationCollection
	{
		public static readonly System.Net.NetworkInformation.MulticastIPAddressInformationImplCollection Empty = new System.Net.NetworkInformation.MulticastIPAddressInformationImplCollection(true);

		private bool is_readonly;

		public override bool IsReadOnly
		{
			get
			{
				return is_readonly;
			}
		}

		private MulticastIPAddressInformationImplCollection(bool isReadOnly)
		{
			is_readonly = isReadOnly;
		}

		public static MulticastIPAddressInformationCollection Win32FromMulticast(IntPtr ptr)
		{
			System.Net.NetworkInformation.MulticastIPAddressInformationImplCollection multicastIPAddressInformationImplCollection = new System.Net.NetworkInformation.MulticastIPAddressInformationImplCollection(false);
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				System.Net.NetworkInformation.Win32_IP_ADAPTER_MULTICAST_ADDRESS win32_IP_ADAPTER_MULTICAST_ADDRESS = (System.Net.NetworkInformation.Win32_IP_ADAPTER_MULTICAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(System.Net.NetworkInformation.Win32_IP_ADAPTER_MULTICAST_ADDRESS));
				multicastIPAddressInformationImplCollection.Add(new System.Net.NetworkInformation.MulticastIPAddressInformationImpl(win32_IP_ADAPTER_MULTICAST_ADDRESS.Address.GetIPAddress(), win32_IP_ADAPTER_MULTICAST_ADDRESS.LengthFlags.IsDnsEligible, win32_IP_ADAPTER_MULTICAST_ADDRESS.LengthFlags.IsTransient));
				intPtr = win32_IP_ADAPTER_MULTICAST_ADDRESS.Next;
			}
			multicastIPAddressInformationImplCollection.is_readonly = true;
			return multicastIPAddressInformationImplCollection;
		}

		public static System.Net.NetworkInformation.MulticastIPAddressInformationImplCollection LinuxFromList(List<IPAddress> addresses)
		{
			System.Net.NetworkInformation.MulticastIPAddressInformationImplCollection multicastIPAddressInformationImplCollection = new System.Net.NetworkInformation.MulticastIPAddressInformationImplCollection(false);
			foreach (IPAddress address in addresses)
			{
				multicastIPAddressInformationImplCollection.Add(new System.Net.NetworkInformation.MulticastIPAddressInformationImpl(address, true, false));
			}
			multicastIPAddressInformationImplCollection.is_readonly = true;
			return multicastIPAddressInformationImplCollection;
		}
	}
}
