using System;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace ICSharpCode.SharpZipLib.GZip
{
	public class GZipInputStream : InflaterInputStream
	{
		protected Crc32 crc = new Crc32();

		protected bool eos;

		private bool readGZIPHeader;

		public GZipInputStream(Stream baseInputStream)
			: this(baseInputStream, 4096)
		{
		}

		public GZipInputStream(Stream baseInputStream, int size)
			: base(baseInputStream, new Inflater(true), size)
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!readGZIPHeader)
			{
				ReadHeader();
			}
			if (eos)
			{
				return 0;
			}
			int num = base.Read(buffer, offset, count);
			if (num > 0)
			{
				crc.Update(buffer, offset, num);
			}
			if (inf.IsFinished)
			{
				ReadFooter();
			}
			return num;
		}

		private void ReadHeader()
		{
			Crc32 crc = new Crc32();
			int num = baseInputStream.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			crc.Update(num);
			if (num != 31)
			{
				throw new GZipException("Error GZIP header, first magic byte doesn't match");
			}
			num = baseInputStream.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			if (num != 139)
			{
				throw new GZipException("Error GZIP header,  second magic byte doesn't match");
			}
			crc.Update(num);
			int num2 = baseInputStream.ReadByte();
			if (num2 < 0)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			if (num2 != 8)
			{
				throw new GZipException("Error GZIP header, data not in deflate format");
			}
			crc.Update(num2);
			int num3 = baseInputStream.ReadByte();
			if (num3 < 0)
			{
				throw new EndOfStreamException("EOS reading GZIP header");
			}
			crc.Update(num3);
			if ((num3 & 0xE0) != 0)
			{
				throw new GZipException("Reserved flag bits in GZIP header != 0");
			}
			for (int i = 0; i < 6; i++)
			{
				int num4 = baseInputStream.ReadByte();
				if (num4 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num4);
			}
			if ((num3 & 4) != 0)
			{
				for (int j = 0; j < 2; j++)
				{
					int num5 = baseInputStream.ReadByte();
					if (num5 < 0)
					{
						throw new EndOfStreamException("EOS reading GZIP header");
					}
					crc.Update(num5);
				}
				if (baseInputStream.ReadByte() < 0 || baseInputStream.ReadByte() < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				int num6 = baseInputStream.ReadByte();
				int num7 = baseInputStream.ReadByte();
				if (num6 < 0 || num7 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num6);
				crc.Update(num7);
				int num8 = (num6 << 8) | num7;
				for (int k = 0; k < num8; k++)
				{
					int num9 = baseInputStream.ReadByte();
					if (num9 < 0)
					{
						throw new EndOfStreamException("EOS reading GZIP header");
					}
					crc.Update(num9);
				}
			}
			if ((num3 & 8) != 0)
			{
				int num10;
				while ((num10 = baseInputStream.ReadByte()) > 0)
				{
					crc.Update(num10);
				}
				if (num10 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num10);
			}
			if ((num3 & 0x10) != 0)
			{
				int num11;
				while ((num11 = baseInputStream.ReadByte()) > 0)
				{
					crc.Update(num11);
				}
				if (num11 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				crc.Update(num11);
			}
			if ((num3 & 2) != 0)
			{
				int num12 = baseInputStream.ReadByte();
				if (num12 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				int num13 = baseInputStream.ReadByte();
				if (num13 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				num12 = (num12 << 8) | num13;
				if (num12 != ((int)crc.Value & 0xFFFF))
				{
					throw new GZipException("Header CRC value mismatch");
				}
			}
			readGZIPHeader = true;
		}

		private void ReadFooter()
		{
			byte[] array = new byte[8];
			int num = inf.RemainingInput;
			if (num > 8)
			{
				num = 8;
			}
			Array.Copy(inputBuffer.RawData, inputBuffer.RawLength - inf.RemainingInput, array, 0, num);
			int num2 = 8 - num;
			while (num2 > 0)
			{
				int num3 = baseInputStream.Read(array, 8 - num2, num2);
				if (num3 <= 0)
				{
					throw new EndOfStreamException("EOS reading GZIP footer");
				}
				num2 -= num3;
			}
			int num4 = (array[0] & 0xFF) | ((array[1] & 0xFF) << 8) | ((array[2] & 0xFF) << 16) | (array[3] << 24);
			if (num4 != (int)crc.Value)
			{
				throw new GZipException("GZIP crc sum mismatch, theirs \"" + num4 + "\" and ours \"" + (int)crc.Value);
			}
			uint num5 = (uint)((array[4] & 0xFF) | ((array[5] & 0xFF) << 8) | ((array[6] & 0xFF) << 16) | (array[7] << 24));
			if ((inf.TotalOut & 0xFFFFFFFFu) != num5)
			{
				throw new GZipException("Number of bytes mismatch in footer");
			}
			eos = true;
		}
	}
}
