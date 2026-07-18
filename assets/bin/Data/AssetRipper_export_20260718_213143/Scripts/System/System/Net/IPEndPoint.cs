using System.Net.Sockets;

namespace System.Net
{
	[Serializable]
	public class IPEndPoint : EndPoint
	{
		public const int MaxPort = 65535;

		public const int MinPort = 0;

		private IPAddress address;

		private int port;

		public IPAddress Address
		{
			get
			{
				return address;
			}
			set
			{
				address = value;
			}
		}

		public override AddressFamily AddressFamily
		{
			get
			{
				return address.AddressFamily;
			}
		}

		public int Port
		{
			get
			{
				return port;
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new ArgumentOutOfRangeException("Invalid port");
				}
				port = value;
			}
		}

		public IPEndPoint(IPAddress address, int port)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			Address = address;
			Port = port;
		}

		public IPEndPoint(long iaddr, int port)
		{
			Address = new IPAddress(iaddr);
			Port = port;
		}

		public override EndPoint Create(SocketAddress socketAddress)
		{
			if (socketAddress == null)
			{
				throw new ArgumentNullException("socketAddress");
			}
			if (socketAddress.Family != AddressFamily)
			{
				throw new ArgumentException(string.Concat("The IPEndPoint was created using ", AddressFamily, " AddressFamily but SocketAddress contains ", socketAddress.Family, " instead, please use the same type."));
			}
			int size = socketAddress.Size;
			AddressFamily family = socketAddress.Family;
			IPEndPoint iPEndPoint = null;
			switch (family)
			{
			case AddressFamily.InterNetwork:
			{
				if (size < 8)
				{
					return null;
				}
				int num = (socketAddress[2] << 8) + socketAddress[3];
				long iaddr = ((long)(int)socketAddress[7] << 24) + ((long)(int)socketAddress[6] << 16) + ((long)(int)socketAddress[5] << 8) + (int)socketAddress[4];
				return new IPEndPoint(iaddr, num);
			}
			case AddressFamily.InterNetworkV6:
			{
				if (size < 28)
				{
					return null;
				}
				int num = (socketAddress[2] << 8) + socketAddress[3];
				int num2 = socketAddress[24] + (socketAddress[25] << 8) + (socketAddress[26] << 16) + (socketAddress[27] << 24);
				ushort[] array = new ushort[8];
				for (int i = 0; i < 8; i++)
				{
					array[i] = (ushort)((socketAddress[8 + i * 2] << 8) + socketAddress[8 + i * 2 + 1]);
				}
				return new IPEndPoint(new IPAddress(array, num2), num);
			}
			default:
				return null;
			}
		}

		public override SocketAddress Serialize()
		{
			SocketAddress socketAddress = null;
			switch (address.AddressFamily)
			{
			case AddressFamily.InterNetwork:
			{
				socketAddress = new SocketAddress(AddressFamily.InterNetwork, 16);
				socketAddress[2] = (byte)((port >> 8) & 0xFF);
				socketAddress[3] = (byte)(port & 0xFF);
				long internalIPv4Address = address.InternalIPv4Address;
				socketAddress[4] = (byte)(internalIPv4Address & 0xFF);
				socketAddress[5] = (byte)((internalIPv4Address >> 8) & 0xFF);
				socketAddress[6] = (byte)((internalIPv4Address >> 16) & 0xFF);
				socketAddress[7] = (byte)((internalIPv4Address >> 24) & 0xFF);
				break;
			}
			case AddressFamily.InterNetworkV6:
			{
				socketAddress = new SocketAddress(AddressFamily.InterNetworkV6, 28);
				socketAddress[2] = (byte)((port >> 8) & 0xFF);
				socketAddress[3] = (byte)(port & 0xFF);
				byte[] addressBytes = address.GetAddressBytes();
				for (int i = 0; i < 16; i++)
				{
					socketAddress[8 + i] = addressBytes[i];
				}
				socketAddress[24] = (byte)(address.ScopeId & 0xFF);
				socketAddress[25] = (byte)((address.ScopeId >> 8) & 0xFF);
				socketAddress[26] = (byte)((address.ScopeId >> 16) & 0xFF);
				socketAddress[27] = (byte)((address.ScopeId >> 24) & 0xFF);
				break;
			}
			}
			return socketAddress;
		}

		public override string ToString()
		{
			return address.ToString() + ":" + port;
		}

		public override bool Equals(object obj)
		{
			IPEndPoint iPEndPoint = obj as IPEndPoint;
			return iPEndPoint != null && iPEndPoint.port == port && iPEndPoint.address.Equals(address);
		}

		public override int GetHashCode()
		{
			return address.GetHashCode() + port;
		}
	}
}
