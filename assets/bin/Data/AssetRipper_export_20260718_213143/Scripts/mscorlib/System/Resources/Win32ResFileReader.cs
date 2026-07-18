using System.Collections;
using System.IO;
using System.Text;

namespace System.Resources
{
	internal class Win32ResFileReader
	{
		private Stream res_file;

		public Win32ResFileReader(Stream s)
		{
			res_file = s;
		}

		private int read_int16()
		{
			int num = res_file.ReadByte();
			if (num == -1)
			{
				return -1;
			}
			int num2 = res_file.ReadByte();
			if (num2 == -1)
			{
				return -1;
			}
			return num | (num2 << 8);
		}

		private int read_int32()
		{
			int num = read_int16();
			if (num == -1)
			{
				return -1;
			}
			int num2 = read_int16();
			if (num2 == -1)
			{
				return -1;
			}
			return num | (num2 << 16);
		}

		private void read_padding()
		{
			while (res_file.Position % 4 != 0L)
			{
				read_int16();
			}
		}

		private NameOrId read_ordinal()
		{
			int num = read_int16();
			if ((num & 0xFFFF) != 0)
			{
				int id = read_int16();
				return new NameOrId(id);
			}
			byte[] array = new byte[16];
			int num2 = 0;
			while (true)
			{
				int num3 = read_int16();
				if (num3 == 0)
				{
					break;
				}
				if (num2 == array.Length)
				{
					byte[] array2 = new byte[array.Length * 2];
					Array.Copy(array, array2, array.Length);
					array = array2;
				}
				array[num2] = (byte)(num3 >> 8);
				array[num2 + 1] = (byte)(num3 & 0xFF);
				num2 += 2;
			}
			return new NameOrId(new string(Encoding.Unicode.GetChars(array, 0, num2)));
		}

		public ICollection ReadResources()
		{
			ArrayList arrayList = new ArrayList();
			while (true)
			{
				read_padding();
				int num = read_int32();
				if (num == -1)
				{
					break;
				}
				read_int32();
				NameOrId type = read_ordinal();
				NameOrId name = read_ordinal();
				read_padding();
				read_int32();
				read_int16();
				int language = read_int16();
				read_int32();
				read_int32();
				if (num != 0)
				{
					byte[] array = new byte[num];
					res_file.Read(array, 0, num);
					arrayList.Add(new Win32EncodedResource(type, name, language, array));
				}
			}
			return arrayList;
		}
	}
}
