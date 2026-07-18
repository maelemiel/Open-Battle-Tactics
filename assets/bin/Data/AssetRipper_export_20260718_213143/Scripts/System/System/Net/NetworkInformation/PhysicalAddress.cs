using System.Globalization;
using System.Text;

namespace System.Net.NetworkInformation
{
	public class PhysicalAddress
	{
		private const int numberOfBytes = 6;

		public static readonly PhysicalAddress None = new PhysicalAddress(new byte[0]);

		private byte[] bytes;

		public PhysicalAddress(byte[] address)
		{
			bytes = address;
		}

		internal static PhysicalAddress ParseEthernet(string address)
		{
			if (address == null)
			{
				return None;
			}
			string[] array = address.Split(':');
			byte[] array2 = new byte[array.Length];
			int num = 0;
			string[] array3 = array;
			foreach (string s in array3)
			{
				array2[num++] = byte.Parse(s, NumberStyles.HexNumber);
			}
			return new PhysicalAddress(array2);
		}

		public static PhysicalAddress Parse(string address)
		{
			if (address == null)
			{
				return None;
			}
			if (address == string.Empty)
			{
				throw new FormatException("An invalid physical address was specified.");
			}
			string[] array = address.Split('-');
			if (array.Length == 1)
			{
				if (address.Length != 12)
				{
					throw new FormatException("An invalid physical address was specified.");
				}
				array = new string[6];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = address.Substring(i * 2, 2);
				}
			}
			if (array.Length == 6)
			{
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (text.Length > 2)
					{
						throw new FormatException("An invalid physical address was specified.");
					}
					if (text.Length < 2)
					{
						throw new IndexOutOfRangeException("An invalid physical address was specified.");
					}
				}
				byte[] array3 = new byte[6];
				for (int k = 0; k < 6; k++)
				{
					byte b = (byte)(GetValue(array[k][0]) << 4);
					b += GetValue(array[k][1]);
					array3[k] = b;
				}
				return new PhysicalAddress(array3);
			}
			throw new FormatException("An invalid physical address was specified.");
		}

		private static byte GetValue(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return (byte)(c - 48);
			}
			if (c >= 'a' && c <= 'f')
			{
				return (byte)(c - 97 + 10);
			}
			if (c >= 'A' && c <= 'F')
			{
				return (byte)(c - 65 + 10);
			}
			throw new FormatException("Invalid physical address.");
		}

		public override bool Equals(object comparand)
		{
			PhysicalAddress physicalAddress = comparand as PhysicalAddress;
			if (physicalAddress == null)
			{
				return false;
			}
			if (bytes.Length != physicalAddress.bytes.Length)
			{
				return false;
			}
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] != physicalAddress.bytes[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return (bytes[5] << 8) ^ bytes[4] ^ (bytes[3] << 24) ^ (bytes[2] << 16) ^ (bytes[1] << 8) ^ bytes[0];
		}

		public byte[] GetAddressBytes()
		{
			return bytes;
		}

		public override string ToString()
		{
			if (bytes == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = bytes;
			foreach (byte b in array)
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			return stringBuilder.ToString();
		}
	}
}
