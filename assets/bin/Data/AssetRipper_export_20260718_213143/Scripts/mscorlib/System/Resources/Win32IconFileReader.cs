using System.IO;

namespace System.Resources
{
	internal class Win32IconFileReader
	{
		private Stream iconFile;

		public Win32IconFileReader(Stream s)
		{
			iconFile = s;
		}

		public ICONDIRENTRY[] ReadIcons()
		{
			ICONDIRENTRY[] array = null;
			using (BinaryReader binaryReader = new BinaryReader(iconFile))
			{
				int num = binaryReader.ReadInt16();
				int num2 = binaryReader.ReadInt16();
				if (num != 0 || num2 != 1)
				{
					throw new Exception("Invalid .ico file format");
				}
				long num3 = binaryReader.ReadInt16();
				array = new ICONDIRENTRY[num3];
				for (int i = 0; i < num3; i++)
				{
					ICONDIRENTRY iCONDIRENTRY = new ICONDIRENTRY();
					iCONDIRENTRY.bWidth = binaryReader.ReadByte();
					iCONDIRENTRY.bHeight = binaryReader.ReadByte();
					iCONDIRENTRY.bColorCount = binaryReader.ReadByte();
					iCONDIRENTRY.bReserved = binaryReader.ReadByte();
					iCONDIRENTRY.wPlanes = binaryReader.ReadInt16();
					iCONDIRENTRY.wBitCount = binaryReader.ReadInt16();
					int num4 = binaryReader.ReadInt32();
					int num5 = binaryReader.ReadInt32();
					iCONDIRENTRY.image = new byte[num4];
					long position = iconFile.Position;
					iconFile.Position = num5;
					iconFile.Read(iCONDIRENTRY.image, 0, num4);
					iconFile.Position = position;
					if (iCONDIRENTRY.wPlanes == 0)
					{
						iCONDIRENTRY.wPlanes = (short)(iCONDIRENTRY.image[12] | (iCONDIRENTRY.image[13] << 8));
					}
					if (iCONDIRENTRY.wBitCount == 0)
					{
						iCONDIRENTRY.wBitCount = (short)(iCONDIRENTRY.image[14] | (iCONDIRENTRY.image[15] << 8));
					}
					array[i] = iCONDIRENTRY;
				}
				return array;
			}
		}
	}
}
