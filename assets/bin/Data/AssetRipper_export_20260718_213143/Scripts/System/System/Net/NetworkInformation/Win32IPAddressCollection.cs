using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	internal class Win32IPAddressCollection : IPAddressCollection
	{
		public static readonly System.Net.NetworkInformation.Win32IPAddressCollection Empty = new System.Net.NetworkInformation.Win32IPAddressCollection(IntPtr.Zero);

		private bool is_readonly;

		public override bool IsReadOnly
		{
			get
			{
				return is_readonly;
			}
		}

		private Win32IPAddressCollection()
		{
		}

		public Win32IPAddressCollection(params IntPtr[] heads)
		{
			foreach (IntPtr head in heads)
			{
				AddSubsequentlyString(head);
			}
			is_readonly = true;
		}

		public Win32IPAddressCollection(params System.Net.NetworkInformation.Win32_IP_ADDR_STRING[] al)
		{
			for (int i = 0; i < al.Length; i++)
			{
				System.Net.NetworkInformation.Win32_IP_ADDR_STRING win32_IP_ADDR_STRING = al[i];
				if (!string.IsNullOrEmpty(win32_IP_ADDR_STRING.IpAddress))
				{
					Add(IPAddress.Parse(win32_IP_ADDR_STRING.IpAddress));
					AddSubsequentlyString(win32_IP_ADDR_STRING.Next);
				}
			}
			is_readonly = true;
		}

		public static System.Net.NetworkInformation.Win32IPAddressCollection FromAnycast(IntPtr ptr)
		{
			System.Net.NetworkInformation.Win32IPAddressCollection win32IPAddressCollection = new System.Net.NetworkInformation.Win32IPAddressCollection();
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				System.Net.NetworkInformation.Win32_IP_ADAPTER_ANYCAST_ADDRESS win32_IP_ADAPTER_ANYCAST_ADDRESS = (System.Net.NetworkInformation.Win32_IP_ADAPTER_ANYCAST_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(System.Net.NetworkInformation.Win32_IP_ADAPTER_ANYCAST_ADDRESS));
				win32IPAddressCollection.Add(win32_IP_ADAPTER_ANYCAST_ADDRESS.Address.GetIPAddress());
				intPtr = win32_IP_ADAPTER_ANYCAST_ADDRESS.Next;
			}
			win32IPAddressCollection.is_readonly = true;
			return win32IPAddressCollection;
		}

		public static System.Net.NetworkInformation.Win32IPAddressCollection FromDnsServer(IntPtr ptr)
		{
			System.Net.NetworkInformation.Win32IPAddressCollection win32IPAddressCollection = new System.Net.NetworkInformation.Win32IPAddressCollection();
			IntPtr intPtr = ptr;
			while (intPtr != IntPtr.Zero)
			{
				System.Net.NetworkInformation.Win32_IP_ADAPTER_DNS_SERVER_ADDRESS win32_IP_ADAPTER_DNS_SERVER_ADDRESS = (System.Net.NetworkInformation.Win32_IP_ADAPTER_DNS_SERVER_ADDRESS)Marshal.PtrToStructure(intPtr, typeof(System.Net.NetworkInformation.Win32_IP_ADAPTER_DNS_SERVER_ADDRESS));
				win32IPAddressCollection.Add(win32_IP_ADAPTER_DNS_SERVER_ADDRESS.Address.GetIPAddress());
				intPtr = win32_IP_ADAPTER_DNS_SERVER_ADDRESS.Next;
			}
			win32IPAddressCollection.is_readonly = true;
			return win32IPAddressCollection;
		}

		private void AddSubsequentlyString(IntPtr head)
		{
			IntPtr intPtr = head;
			while (intPtr != IntPtr.Zero)
			{
				System.Net.NetworkInformation.Win32_IP_ADDR_STRING win32_IP_ADDR_STRING = (System.Net.NetworkInformation.Win32_IP_ADDR_STRING)Marshal.PtrToStructure(intPtr, typeof(System.Net.NetworkInformation.Win32_IP_ADDR_STRING));
				Add(IPAddress.Parse(win32_IP_ADDR_STRING.IpAddress));
				intPtr = win32_IP_ADDR_STRING.Next;
			}
		}
	}
}
