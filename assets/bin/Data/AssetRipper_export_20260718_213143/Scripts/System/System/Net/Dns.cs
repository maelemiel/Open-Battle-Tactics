using System.Collections;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

namespace System.Net
{
	public static class Dns
	{
		private delegate IPHostEntry GetHostByNameCallback(string hostName);

		private delegate IPHostEntry ResolveCallback(string hostName);

		private delegate IPHostEntry GetHostEntryNameCallback(string hostName);

		private delegate IPHostEntry GetHostEntryIPCallback(IPAddress hostAddress);

		private delegate IPAddress[] GetHostAddressesCallback(string hostName);

		static Dns()
		{
			Socket.CheckProtocolSupport();
		}

		[Obsolete("Use BeginGetHostEntry instead")]
		public static IAsyncResult BeginGetHostByName(string hostName, AsyncCallback requestCallback, object stateObject)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			GetHostByNameCallback getHostByNameCallback = GetHostByName;
			return getHostByNameCallback.BeginInvoke(hostName, requestCallback, stateObject);
		}

		[Obsolete("Use BeginGetHostEntry instead")]
		public static IAsyncResult BeginResolve(string hostName, AsyncCallback requestCallback, object stateObject)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			ResolveCallback resolveCallback = Resolve;
			return resolveCallback.BeginInvoke(hostName, requestCallback, stateObject);
		}

		public static IAsyncResult BeginGetHostAddresses(string hostNameOrAddress, AsyncCallback requestCallback, object stateObject)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostName");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			GetHostAddressesCallback getHostAddressesCallback = GetHostAddresses;
			return getHostAddressesCallback.BeginInvoke(hostNameOrAddress, requestCallback, stateObject);
		}

		public static IAsyncResult BeginGetHostEntry(string hostNameOrAddress, AsyncCallback requestCallback, object stateObject)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostName");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			GetHostEntryNameCallback getHostEntryNameCallback = GetHostEntry;
			return getHostEntryNameCallback.BeginInvoke(hostNameOrAddress, requestCallback, stateObject);
		}

		public static IAsyncResult BeginGetHostEntry(IPAddress address, AsyncCallback requestCallback, object stateObject)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			GetHostEntryIPCallback getHostEntryIPCallback = GetHostEntry;
			return getHostEntryIPCallback.BeginInvoke(address, requestCallback, stateObject);
		}

		[Obsolete("Use EndGetHostEntry instead")]
		public static IPHostEntry EndGetHostByName(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			GetHostByNameCallback getHostByNameCallback = (GetHostByNameCallback)asyncResult2.AsyncDelegate;
			return getHostByNameCallback.EndInvoke(asyncResult);
		}

		[Obsolete("Use EndGetHostEntry instead")]
		public static IPHostEntry EndResolve(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			ResolveCallback resolveCallback = (ResolveCallback)asyncResult2.AsyncDelegate;
			return resolveCallback.EndInvoke(asyncResult);
		}

		public static IPAddress[] EndGetHostAddresses(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			GetHostAddressesCallback getHostAddressesCallback = (GetHostAddressesCallback)asyncResult2.AsyncDelegate;
			return getHostAddressesCallback.EndInvoke(asyncResult);
		}

		public static IPHostEntry EndGetHostEntry(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			if (asyncResult2.AsyncDelegate is GetHostEntryIPCallback)
			{
				return ((GetHostEntryIPCallback)asyncResult2.AsyncDelegate).EndInvoke(asyncResult);
			}
			GetHostEntryNameCallback getHostEntryNameCallback = (GetHostEntryNameCallback)asyncResult2.AsyncDelegate;
			return getHostEntryNameCallback.EndInvoke(asyncResult);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHostByName_internal(string host, out string h_name, out string[] h_aliases, out string[] h_addr_list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHostByAddr_internal(string addr, out string h_name, out string[] h_aliases, out string[] h_addr_list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetHostName_internal(out string h_name);

		private static IPHostEntry hostent_to_IPHostEntry(string h_name, string[] h_aliases, string[] h_addrlist)
		{
			IPHostEntry iPHostEntry = new IPHostEntry();
			ArrayList arrayList = new ArrayList();
			iPHostEntry.HostName = h_name;
			iPHostEntry.Aliases = h_aliases;
			for (int i = 0; i < h_addrlist.Length; i++)
			{
				try
				{
					IPAddress iPAddress = IPAddress.Parse(h_addrlist[i]);
					if ((Socket.SupportsIPv6 && iPAddress.AddressFamily == AddressFamily.InterNetworkV6) || (Socket.SupportsIPv4 && iPAddress.AddressFamily == AddressFamily.InterNetwork))
					{
						arrayList.Add(iPAddress);
					}
				}
				catch (ArgumentNullException)
				{
				}
			}
			if (arrayList.Count == 0)
			{
				throw new SocketException(11001);
			}
			iPHostEntry.AddressList = arrayList.ToArray(typeof(IPAddress)) as IPAddress[];
			return iPHostEntry;
		}

		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry GetHostByAddress(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return GetHostByAddressFromString(address.ToString(), false);
		}

		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry GetHostByAddress(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return GetHostByAddressFromString(address, true);
		}

		private static IPHostEntry GetHostByAddressFromString(string address, bool parse)
		{
			if (address.Equals("0.0.0.0"))
			{
				address = "127.0.0.1";
				parse = false;
			}
			if (parse)
			{
				IPAddress.Parse(address);
			}
			string h_name;
			string[] h_aliases;
			string[] h_addr_list;
			if (!GetHostByAddr_internal(address, out h_name, out h_aliases, out h_addr_list))
			{
				throw new SocketException(11001);
			}
			return hostent_to_IPHostEntry(h_name, h_aliases, h_addr_list);
		}

		public static IPHostEntry GetHostEntry(string hostNameOrAddress)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostNameOrAddress");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			IPAddress address;
			if (hostNameOrAddress.Length > 0 && IPAddress.TryParse(hostNameOrAddress, out address))
			{
				return GetHostEntry(address);
			}
			return GetHostByName(hostNameOrAddress);
		}

		public static IPHostEntry GetHostEntry(IPAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return GetHostByAddressFromString(address.ToString(), false);
		}

		public static IPAddress[] GetHostAddresses(string hostNameOrAddress)
		{
			if (hostNameOrAddress == null)
			{
				throw new ArgumentNullException("hostNameOrAddress");
			}
			if (hostNameOrAddress == "0.0.0.0" || hostNameOrAddress == "::0")
			{
				throw new ArgumentException("Addresses 0.0.0.0 (IPv4) and ::0 (IPv6) are unspecified addresses. You cannot use them as target address.", "hostNameOrAddress");
			}
			IPAddress address;
			if (hostNameOrAddress.Length > 0 && IPAddress.TryParse(hostNameOrAddress, out address))
			{
				return new IPAddress[1] { address };
			}
			return GetHostEntry(hostNameOrAddress).AddressList;
		}

		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry GetHostByName(string hostName)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			string h_name;
			string[] h_aliases;
			string[] h_addr_list;
			if (!GetHostByName_internal(hostName, out h_name, out h_aliases, out h_addr_list))
			{
				throw new SocketException(11001);
			}
			return hostent_to_IPHostEntry(h_name, h_aliases, h_addr_list);
		}

		public static string GetHostName()
		{
			string h_name;
			if (!GetHostName_internal(out h_name))
			{
				throw new SocketException(11001);
			}
			return h_name;
		}

		[Obsolete("Use GetHostEntry instead")]
		public static IPHostEntry Resolve(string hostName)
		{
			if (hostName == null)
			{
				throw new ArgumentNullException("hostName");
			}
			IPHostEntry iPHostEntry = null;
			try
			{
				iPHostEntry = GetHostByAddress(hostName);
			}
			catch
			{
			}
			if (iPHostEntry == null)
			{
				iPHostEntry = GetHostByName(hostName);
			}
			return iPHostEntry;
		}
	}
}
