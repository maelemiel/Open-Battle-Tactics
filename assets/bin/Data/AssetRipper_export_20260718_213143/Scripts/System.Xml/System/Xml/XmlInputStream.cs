using System.IO;
using System.Text;

namespace System.Xml
{
	internal class XmlInputStream : Stream
	{
		public static readonly Encoding StrictUTF8;

		private Encoding enc;

		private Stream stream;

		private byte[] buffer;

		private int bufLength;

		private int bufPos;

		private static XmlException encodingException;

		public Encoding ActualEncoding
		{
			get
			{
				return enc;
			}
		}

		public override bool CanRead
		{
			get
			{
				if (bufLength > bufPos)
				{
					return true;
				}
				return stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return stream.Position - bufLength + bufPos;
			}
			set
			{
				if (value < bufLength)
				{
					bufPos = (int)value;
				}
				else
				{
					stream.Position = value - bufLength;
				}
			}
		}

		public XmlInputStream(Stream stream)
		{
			Initialize(stream);
		}

		static XmlInputStream()
		{
			encodingException = new XmlException("invalid encoding specification.");
			StrictUTF8 = new UTF8Encoding(false, true);
		}

		private static string GetStringFromBytes(byte[] bytes, int index, int count)
		{
			return Encoding.ASCII.GetString(bytes, index, count);
		}

		private void Initialize(Stream stream)
		{
			buffer = new byte[64];
			this.stream = stream;
			enc = StrictUTF8;
			bufLength = stream.Read(buffer, 0, buffer.Length);
			if (bufLength == -1 || bufLength == 0)
			{
				return;
			}
			switch (ReadByteSpecial())
			{
			case 255:
			{
				int num = ReadByteSpecial();
				if (num == 254)
				{
					enc = Encoding.Unicode;
				}
				else
				{
					bufPos = 0;
				}
				break;
			}
			case 254:
			{
				int num = ReadByteSpecial();
				if (num == 255)
				{
					enc = Encoding.BigEndianUnicode;
				}
				else
				{
					bufPos = 0;
				}
				break;
			}
			case 239:
			{
				int num = ReadByteSpecial();
				if (num == 187)
				{
					num = ReadByteSpecial();
					if (num != 191)
					{
						bufPos = 0;
					}
				}
				else
				{
					buffer[--bufPos] = 239;
				}
				break;
			}
			case 60:
				if (bufLength >= 5 && GetStringFromBytes(buffer, 1, 4) == "?xml")
				{
					bufPos += 4;
					int num = SkipWhitespace();
					if (num == 118)
					{
						while (num >= 0)
						{
							num = ReadByteSpecial();
							if (num == 48)
							{
								ReadByteSpecial();
								break;
							}
						}
						num = SkipWhitespace();
					}
					if (num == 101)
					{
						int num2 = bufLength - bufPos;
						if (num2 >= 7 && GetStringFromBytes(buffer, bufPos, 7) == "ncoding")
						{
							bufPos += 7;
							num = SkipWhitespace();
							if (num != 61)
							{
								throw encodingException;
							}
							num = SkipWhitespace();
							int num3 = num;
							StringBuilder stringBuilder = new StringBuilder();
							while (true)
							{
								num = ReadByteSpecial();
								if (num == num3)
								{
									break;
								}
								if (num < 0)
								{
									throw encodingException;
								}
								stringBuilder.Append((char)num);
							}
							string text = stringBuilder.ToString();
							if (!XmlChar.IsValidIANAEncoding(text))
							{
								throw encodingException;
							}
							enc = Encoding.GetEncoding(text);
						}
					}
				}
				bufPos = 0;
				break;
			default:
				bufPos = 0;
				break;
			}
		}

		private int ReadByteSpecial()
		{
			if (bufLength > bufPos)
			{
				return buffer[bufPos++];
			}
			byte[] dst = new byte[buffer.Length * 2];
			Buffer.BlockCopy(buffer, 0, dst, 0, bufLength);
			int num = stream.Read(dst, bufLength, buffer.Length);
			if (num == -1 || num == 0)
			{
				return -1;
			}
			bufLength += num;
			buffer = dst;
			return buffer[bufPos++];
		}

		private int SkipWhitespace()
		{
			while (true)
			{
				int num = ReadByteSpecial();
				switch ((char)(ushort)num)
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
					continue;
				}
				return num;
			}
		}

		public override void Close()
		{
			stream.Close();
		}

		public override void Flush()
		{
			stream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (count <= bufLength - bufPos)
			{
				Buffer.BlockCopy(this.buffer, bufPos, buffer, offset, count);
				bufPos += count;
				return count;
			}
			int num = bufLength - bufPos;
			if (bufLength > bufPos)
			{
				Buffer.BlockCopy(this.buffer, bufPos, buffer, offset, num);
				bufPos += num;
			}
			return num + stream.Read(buffer, offset + num, count - num);
		}

		public override int ReadByte()
		{
			if (bufLength > bufPos)
			{
				return buffer[bufPos++];
			}
			return stream.ReadByte();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			int num = bufLength - bufPos;
			if (origin == SeekOrigin.Current)
			{
				if (offset < num)
				{
					return (int)buffer[bufPos + offset];
				}
				return stream.Seek(offset - num, origin);
			}
			return stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}
