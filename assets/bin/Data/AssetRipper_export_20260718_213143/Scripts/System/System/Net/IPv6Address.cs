using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace System.Net
{
	[Serializable]
	internal class IPv6Address
	{
		private ushort[] address;

		private int prefixLength;

		private long scopeId;

		public static readonly System.Net.IPv6Address Loopback = Parse("::1");

		public static readonly System.Net.IPv6Address Unspecified = Parse("::");

		public ushort[] Address
		{
			get
			{
				return address;
			}
		}

		public int PrefixLength
		{
			get
			{
				return prefixLength;
			}
		}

		public long ScopeId
		{
			get
			{
				return scopeId;
			}
			set
			{
				scopeId = value;
			}
		}

		public ushort this[int index]
		{
			get
			{
				return address[index];
			}
		}

		public AddressFamily AddressFamily
		{
			get
			{
				return AddressFamily.InterNetworkV6;
			}
		}

		public IPv6Address(ushort[] addr)
		{
			if (addr == null)
			{
				throw new ArgumentNullException("addr");
			}
			if (addr.Length != 8)
			{
				throw new ArgumentException("addr");
			}
			address = addr;
		}

		public IPv6Address(ushort[] addr, int prefixLength)
			: this(addr)
		{
			if (prefixLength < 0 || prefixLength > 128)
			{
				throw new ArgumentException("prefixLength");
			}
			this.prefixLength = prefixLength;
		}

		public IPv6Address(ushort[] addr, int prefixLength, int scopeId)
			: this(addr, prefixLength)
		{
			this.scopeId = scopeId;
		}

		public static System.Net.IPv6Address Parse(string ipString)
		{
			if (ipString == null)
			{
				throw new ArgumentNullException("ipString");
			}
			System.Net.IPv6Address result;
			if (TryParse(ipString, out result))
			{
				return result;
			}
			throw new FormatException("Not a valid IPv6 address");
		}

		private static int Fill(ushort[] addr, string ipString)
		{
			int num = 0;
			int num2 = 0;
			if (ipString.Length == 0)
			{
				return 0;
			}
			if (ipString.IndexOf("::") != -1)
			{
				return -1;
			}
			for (int i = 0; i < ipString.Length; i++)
			{
				char c = ipString[i];
				if (c == ':')
				{
					if (i == ipString.Length - 1)
					{
						return -1;
					}
					if (num2 == 8)
					{
						return -1;
					}
					addr[num2++] = (ushort)num;
					num = 0;
					continue;
				}
				int num3;
				if ('0' <= c && c <= '9')
				{
					num3 = c - 48;
				}
				else if ('a' <= c && c <= 'f')
				{
					num3 = c - 97 + 10;
				}
				else
				{
					if ('A' > c || c > 'F')
					{
						return -1;
					}
					num3 = c - 65 + 10;
				}
				num = (num << 4) + num3;
				if (num > 65535)
				{
					return -1;
				}
			}
			if (num2 == 8)
			{
				return -1;
			}
			addr[num2++] = (ushort)num;
			return num2;
		}

		private static bool TryParse(string prefix, out int res)
		{
			return int.TryParse(prefix, NumberStyles.Integer, CultureInfo.InvariantCulture, out res);
		}

		public static bool TryParse(string ipString, out System.Net.IPv6Address result)
		{
			result = null;
			if (ipString == null)
			{
				return false;
			}
			if (ipString.Length > 2 && ipString[0] == '[' && ipString[ipString.Length - 1] == ']')
			{
				ipString = ipString.Substring(1, ipString.Length - 2);
			}
			if (ipString.Length < 2)
			{
				return false;
			}
			int res = 0;
			int res2 = 0;
			int num = ipString.LastIndexOf('/');
			if (num != -1)
			{
				string prefix = ipString.Substring(num + 1);
				if (!TryParse(prefix, out res))
				{
					res = -1;
				}
				if (res < 0 || res > 128)
				{
					return false;
				}
				ipString = ipString.Substring(0, num);
			}
			else
			{
				num = ipString.LastIndexOf('%');
				if (num != -1)
				{
					string prefix2 = ipString.Substring(num + 1);
					if (!TryParse(prefix2, out res2))
					{
						res2 = 0;
					}
					ipString = ipString.Substring(0, num);
				}
			}
			ushort[] array = new ushort[8];
			bool flag = false;
			int num2 = ipString.LastIndexOf(':');
			if (num2 == -1)
			{
				return false;
			}
			int num3 = 0;
			if (num2 < ipString.Length - 1)
			{
				string text = ipString.Substring(num2 + 1);
				if (text.IndexOf('.') != -1)
				{
					IPAddress iPAddress;
					if (!IPAddress.TryParse(text, out iPAddress))
					{
						return false;
					}
					long internalIPv4Address = iPAddress.InternalIPv4Address;
					array[6] = (ushort)(((int)(internalIPv4Address & 0xFF) << 8) + (int)((internalIPv4Address >> 8) & 0xFF));
					array[7] = (ushort)(((int)((internalIPv4Address >> 16) & 0xFF) << 8) + (int)((internalIPv4Address >> 24) & 0xFF));
					ipString = ((num2 <= 0 || ipString[num2 - 1] != ':') ? ipString.Substring(0, num2) : ipString.Substring(0, num2 + 1));
					flag = true;
					num3 = 2;
				}
			}
			int num4 = ipString.IndexOf("::");
			if (num4 != -1)
			{
				int num5 = Fill(array, ipString.Substring(num4 + 2));
				if (num5 == -1)
				{
					return false;
				}
				if (num5 + num3 > 8)
				{
					return false;
				}
				int num6 = 8 - num3 - num5;
				for (int num7 = num5; num7 > 0; num7--)
				{
					array[num7 + num6 - 1] = array[num7 - 1];
					array[num7 - 1] = 0;
				}
				int num8 = Fill(array, ipString.Substring(0, num4));
				if (num8 == -1)
				{
					return false;
				}
				if (num8 + num5 + num3 > 7)
				{
					return false;
				}
			}
			else if (Fill(array, ipString) != 8 - num3)
			{
				return false;
			}
			bool flag2 = false;
			for (int i = 0; i < num3; i++)
			{
				if (array[i] != 0 || (i == 5 && array[i] != ushort.MaxValue))
				{
					flag2 = true;
				}
			}
			if (flag && !flag2)
			{
				for (int j = 0; j < 5; j++)
				{
					if (array[j] != 0)
					{
						return false;
					}
				}
				if (array[5] != 0 && array[5] != ushort.MaxValue)
				{
					return false;
				}
			}
			result = new System.Net.IPv6Address(array, res, res2);
			return true;
		}

		public static bool IsLoopback(System.Net.IPv6Address addr)
		{
			if (addr.address[7] != 1)
			{
				return false;
			}
			int num = addr.address[6] >> 8;
			if (num != 127 && num != 0)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if (addr.address[i] != 0)
				{
					return false;
				}
			}
			if (addr.address[5] != 0 && addr.address[5] != ushort.MaxValue)
			{
				return false;
			}
			return true;
		}

		private static ushort SwapUShort(ushort number)
		{
			return (ushort)(((number >> 8) & 0xFF) + ((number << 8) & 0xFF00));
		}

		private int AsIPv4Int()
		{
			return (SwapUShort(address[7]) << 16) + SwapUShort(address[6]);
		}

		public bool IsIPv4Compatible()
		{
			for (int i = 0; i < 6; i++)
			{
				if (address[i] != 0)
				{
					return false;
				}
			}
			return AsIPv4Int() > 1;
		}

		public bool IsIPv4Mapped()
		{
			for (int i = 0; i < 5; i++)
			{
				if (address[i] != 0)
				{
					return false;
				}
			}
			return address[5] == ushort.MaxValue;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (IsIPv4Compatible() || IsIPv4Mapped())
			{
				stringBuilder.Append("::");
				if (IsIPv4Mapped())
				{
					stringBuilder.Append("ffff:");
				}
				stringBuilder.Append(new IPAddress(AsIPv4Int()).ToString());
				return stringBuilder.ToString();
			}
			int num = -1;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < 8; i++)
			{
				if (address[i] != 0)
				{
					if (num3 > num2 && num3 > 1)
					{
						num2 = num3;
						num = i - num3;
					}
					num3 = 0;
				}
				else
				{
					num3++;
				}
			}
			if (num3 > num2 && num3 > 1)
			{
				num2 = num3;
				num = 8 - num3;
			}
			if (num == 0)
			{
				stringBuilder.Append(":");
			}
			for (int j = 0; j < 8; j++)
			{
				if (j == num)
				{
					stringBuilder.Append(":");
					j += num2 - 1;
					continue;
				}
				stringBuilder.AppendFormat("{0:x}", address[j]);
				if (j < 7)
				{
					stringBuilder.Append(':');
				}
			}
			if (scopeId != 0L)
			{
				stringBuilder.Append('%').Append(scopeId);
			}
			return stringBuilder.ToString();
		}

		public string ToString(bool fullLength)
		{
			if (!fullLength)
			{
				return ToString();
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < address.Length - 1; i++)
			{
				stringBuilder.AppendFormat("{0:X4}:", address[i]);
			}
			stringBuilder.AppendFormat("{0:X4}", address[address.Length - 1]);
			return stringBuilder.ToString();
		}

		public override bool Equals(object other)
		{
			System.Net.IPv6Address pv6Address = other as System.Net.IPv6Address;
			if (pv6Address != null)
			{
				for (int i = 0; i < 8; i++)
				{
					if (address[i] != pv6Address.address[i])
					{
						return false;
					}
				}
				return true;
			}
			IPAddress iPAddress = other as IPAddress;
			if (iPAddress != null)
			{
				for (int j = 0; j < 5; j++)
				{
					if (address[j] != 0)
					{
						return false;
					}
				}
				if (address[5] != 0 && address[5] != ushort.MaxValue)
				{
					return false;
				}
				long internalIPv4Address = iPAddress.InternalIPv4Address;
				if (address[6] != (ushort)(((int)(internalIPv4Address & 0xFF) << 8) + (int)((internalIPv4Address >> 8) & 0xFF)) || address[7] != (ushort)(((int)((internalIPv4Address >> 16) & 0xFF) << 8) + (int)((internalIPv4Address >> 24) & 0xFF)))
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Hash((address[0] << 16) + address[1], (address[2] << 16) + address[3], (address[4] << 16) + address[5], (address[6] << 16) + address[7]);
		}

		private static int Hash(int i, int j, int k, int l)
		{
			return i ^ ((j << 13) | (j >> 19)) ^ ((k << 26) | (k >> 6)) ^ ((l << 7) | (l >> 25));
		}
	}
}
