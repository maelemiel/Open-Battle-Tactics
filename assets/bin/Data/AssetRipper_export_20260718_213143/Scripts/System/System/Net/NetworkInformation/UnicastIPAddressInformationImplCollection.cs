using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class UnicastIPAddressInformationImplCollection : UnicastIPAddressInformationCollection
	{
		public static readonly System.Net.NetworkInformation.UnicastIPAddressInformationImplCollection Empty = new System.Net.NetworkInformation.UnicastIPAddressInformationImplCollection(true);

		private bool is_readonly;

		public override bool IsReadOnly
		{
			get
			{
				return is_readonly;
			}
		}

		private UnicastIPAddressInformationImplCollection(bool isReadOnly)
		{
			is_readonly = isReadOnly;
		}

		public static UnicastIPAddressInformationCollection Win32FromUnicast(int ifIndex, IntPtr ptr)
		{
			System.Net.NetworkInformation.UnicastIPAddressInformationImplCollection unicastIPAddressInformationImplCollection = new System.Net.NetworkInformation.UnicastIPAddressInformationImplCollection(false);
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				System.Net.NetworkInformation.Win32_IP_ADAPTER_UNICAST_ADDRESS info = (System.Net.NetworkInformation.Win32_IP_ADAPTER_UNICAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(System.Net.NetworkInformation.Win32_IP_ADAPTER_UNICAST_ADDRESS));
				unicastIPAddressInformationImplCollection.Add(new System.Net.NetworkInformation.Win32UnicastIPAddressInformation(ifIndex, info));
				intPtr = info.Next;
			}
			unicastIPAddressInformationImplCollection.is_readonly = true;
			return unicastIPAddressInformationImplCollection;
		}

		public static UnicastIPAddressInformationCollection LinuxFromList(List<IPAddress> addresses)
		{
			System.Net.NetworkInformation.UnicastIPAddressInformationImplCollection unicastIPAddressInformationImplCollection = new System.Net.NetworkInformation.UnicastIPAddressInformationImplCollection(false);
			foreach (IPAddress address in addresses)
			{
				unicastIPAddressInformationImplCollection.Add(new System.Net.NetworkInformation.LinuxUnicastIPAddressInformation(address));
			}
			unicastIPAddressInformationImplCollection.is_readonly = true;
			return unicastIPAddressInformationImplCollection;
		}
	}
}
