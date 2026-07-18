using System.Runtime.InteropServices;
using System.Text;
using Mono.Security;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public class BinaryWriter : IDisposable
	{
		public static readonly BinaryWriter Null = new BinaryWriter();

		protected Stream OutStream;

		private Encoding m_encoding;

		private byte[] buffer;

		private bool disposed;

		private byte[] stringBuffer;

		private int maxCharsPerRound;

		public virtual Stream BaseStream
		{
			get
			{
				return OutStream;
			}
		}

		protected BinaryWriter()
			: this(Stream.Null, Encoding.UTF8UnmarkedUnsafe)
		{
		}

		public BinaryWriter(Stream output)
			: this(output, Encoding.UTF8UnmarkedUnsafe)
		{
		}

		public BinaryWriter(Stream output, Encoding encoding)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (!output.CanWrite)
			{
				throw new ArgumentException(Locale.GetText("Stream does not support writing or already closed."));
			}
			OutStream = output;
			m_encoding = encoding;
			buffer = new byte[16];
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		public virtual void Close()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && OutStream != null)
			{
				OutStream.Close();
			}
			buffer = null;
			m_encoding = null;
			disposed = true;
		}

		public virtual void Flush()
		{
			OutStream.Flush();
		}

		public virtual long Seek(int offset, SeekOrigin origin)
		{
			return OutStream.Seek(offset, origin);
		}

		public virtual void Write(bool value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			buffer[0] = (byte)(value ? 1u : 0u);
			OutStream.Write(buffer, 0, 1);
		}

		public virtual void Write(byte value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			OutStream.WriteByte(value);
		}

		public virtual void Write(byte[] buffer)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			OutStream.Write(buffer, 0, buffer.Length);
		}

		public virtual void Write(byte[] buffer, int index, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			OutStream.Write(buffer, index, count);
		}

		public virtual void Write(char ch)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			char[] chars = new char[1] { ch };
			byte[] bytes = m_encoding.GetBytes(chars, 0, 1);
			OutStream.Write(bytes, 0, bytes.Length);
		}

		public virtual void Write(char[] chars)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			byte[] bytes = m_encoding.GetBytes(chars, 0, chars.Length);
			OutStream.Write(bytes, 0, bytes.Length);
		}

		public virtual void Write(char[] chars, int index, int count)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			if (chars == null)
			{
				throw new ArgumentNullException("chars");
			}
			byte[] bytes = m_encoding.GetBytes(chars, index, count);
			OutStream.Write(bytes, 0, bytes.Length);
		}

		public unsafe virtual void Write(decimal value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			byte* ptr = (byte*)(&value);
			if (BitConverter.IsLittleEndian)
			{
				for (int i = 0; i < 16; i++)
				{
					if (i < 4)
					{
						buffer[i + 12] = ptr[i];
					}
					else if (i < 8)
					{
						buffer[i + 4] = ptr[i];
					}
					else if (i < 12)
					{
						buffer[i - 8] = ptr[i];
					}
					else
					{
						buffer[i - 8] = ptr[i];
					}
				}
			}
			else
			{
				for (int j = 0; j < 16; j++)
				{
					if (j < 4)
					{
						buffer[15 - j] = ptr[j];
					}
					else if (j < 8)
					{
						buffer[15 - j] = ptr[j];
					}
					else if (j < 12)
					{
						buffer[11 - j] = ptr[j];
					}
					else
					{
						buffer[19 - j] = ptr[j];
					}
				}
			}
			OutStream.Write(buffer, 0, 16);
		}

		public virtual void Write(double value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			OutStream.Write(BitConverterLE.GetBytes(value), 0, 8);
		}

		public virtual void Write(short value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			OutStream.Write(buffer, 0, 2);
		}

		public virtual void Write(int value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			buffer[2] = (byte)(value >> 16);
			buffer[3] = (byte)(value >> 24);
			OutStream.Write(buffer, 0, 4);
		}

		public virtual void Write(long value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			int num = 0;
			int num2 = 0;
			while (num < 8)
			{
				buffer[num] = (byte)(value >> num2);
				num++;
				num2 += 8;
			}
			OutStream.Write(buffer, 0, 8);
		}

		[CLSCompliant(false)]
		public virtual void Write(sbyte value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			buffer[0] = (byte)value;
			OutStream.Write(buffer, 0, 1);
		}

		public virtual void Write(float value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			OutStream.Write(BitConverterLE.GetBytes(value), 0, 4);
		}

		public virtual void Write(string value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			int byteCount = m_encoding.GetByteCount(value);
			Write7BitEncodedInt(byteCount);
			if (stringBuffer == null)
			{
				stringBuffer = new byte[512];
				maxCharsPerRound = 512 / m_encoding.GetMaxByteCount(1);
			}
			int num = 0;
			int num2 = value.Length;
			while (num2 > 0)
			{
				int num3 = ((num2 <= maxCharsPerRound) ? num2 : maxCharsPerRound);
				int bytes = m_encoding.GetBytes(value, num, num3, stringBuffer, 0);
				OutStream.Write(stringBuffer, 0, bytes);
				num += num3;
				num2 -= num3;
			}
		}

		[CLSCompliant(false)]
		public virtual void Write(ushort value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			OutStream.Write(buffer, 0, 2);
		}

		[CLSCompliant(false)]
		public virtual void Write(uint value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			buffer[2] = (byte)(value >> 16);
			buffer[3] = (byte)(value >> 24);
			OutStream.Write(buffer, 0, 4);
		}

		[CLSCompliant(false)]
		public virtual void Write(ulong value)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("BinaryWriter", "Cannot write to a closed BinaryWriter");
			}
			int num = 0;
			int num2 = 0;
			while (num < 8)
			{
				buffer[num] = (byte)(value >> num2);
				num++;
				num2 += 8;
			}
			OutStream.Write(buffer, 0, 8);
		}

		protected void Write7BitEncodedInt(int value)
		{
			do
			{
				int num = (value >> 7) & 0x1FFFFFF;
				byte b = (byte)(value & 0x7F);
				if (num != 0)
				{
					b |= 0x80;
				}
				Write(b);
				value = num;
			}
			while (value != 0);
		}
	}
}
