using System.Net.Sockets;

namespace System.Net
{
	public class SocketAddress
	{
		private byte[] data;

		public AddressFamily Family
		{
			get
			{
				return (AddressFamily)(data[0] + (data[1] << 8));
			}
		}

		public int Size
		{
			get
			{
				return data.Length;
			}
		}

		public byte this[int offset]
		{
			get
			{
				return data[offset];
			}
			set
			{
				data[offset] = value;
			}
		}

		public SocketAddress(AddressFamily family, int size)
		{
			if (size < 2)
			{
				throw new ArgumentOutOfRangeException("size is too small");
			}
			data = new byte[size];
			data[0] = (byte)family;
			data[1] = (byte)((int)family >> 8);
		}

		public SocketAddress(AddressFamily family)
			: this(family, 32)
		{
		}

		public override string ToString()
		{
			string text = ((AddressFamily)data[0]).ToString();
			int num = data.Length;
			string text2 = text + ":" + num + ":{";
			for (int i = 2; i < num; i++)
			{
				int num2 = data[i];
				text2 += num2;
				if (i < num - 1)
				{
					text2 += ",";
				}
			}
			return text2 + "}";
		}

		public override bool Equals(object obj)
		{
			SocketAddress socketAddress = obj as SocketAddress;
			if (socketAddress != null && socketAddress.data.Length == data.Length)
			{
				byte[] array = socketAddress.data;
				for (int i = 0; i < data.Length; i++)
				{
					if (array[i] != data[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < data.Length; i++)
			{
				num += data[i] + i;
			}
			return num;
		}
	}
}
