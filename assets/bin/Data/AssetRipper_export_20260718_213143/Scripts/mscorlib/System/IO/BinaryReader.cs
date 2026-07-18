using System.Runtime.InteropServices;
using System.Text;
using Mono.Security;

namespace System.IO
{
	[ComVisible(true)]
	public class BinaryReader : IDisposable
	{
		private const int MaxBufferSize = 128;

		private Stream m_stream;

		private Encoding m_encoding;

		private byte[] m_buffer;

		private Decoder decoder;

		private char[] charBuffer;

		private bool m_disposed;

		public virtual Stream BaseStream
		{
			get
			{
				return m_stream;
			}
		}

		public BinaryReader(Stream input)
			: this(input, Encoding.UTF8UnmarkedUnsafe)
		{
		}

		public BinaryReader(Stream input, Encoding encoding)
		{
			if (input == null || encoding == null)
			{
				throw new ArgumentNullException(Locale.GetText("Input or Encoding is a null reference."));
			}
			if (!input.CanRead)
			{
				throw new ArgumentException(Locale.GetText("The stream doesn't support reading."));
			}
			m_stream = input;
			m_encoding = encoding;
			decoder = encoding.GetDecoder();
			m_buffer = new byte[32];
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		public virtual void Close()
		{
			Dispose(true);
			m_disposed = true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && m_stream != null)
			{
				m_stream.Close();
			}
			m_disposed = true;
			m_buffer = null;
			m_encoding = null;
			m_stream = null;
			charBuffer = null;
		}

		protected virtual void FillBuffer(int numBytes)
		{
			if (m_disposed)
			{
				throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
			}
			if (m_stream == null)
			{
				throw new IOException("Stream is invalid");
			}
			CheckBuffer(numBytes);
			int num;
			for (int i = 0; i < numBytes; i += num)
			{
				num = m_stream.Read(m_buffer, i, numBytes - i);
				if (num == 0)
				{
					throw new EndOfStreamException();
				}
			}
		}

		public virtual int PeekChar()
		{
			if (m_stream == null)
			{
				if (m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			if (!m_stream.CanSeek)
			{
				return -1;
			}
			char[] array = new char[1];
			int bytes_read;
			int num = ReadCharBytes(array, 0, 1, out bytes_read);
			m_stream.Position -= bytes_read;
			if (num == 0)
			{
				return -1;
			}
			return array[0];
		}

		public virtual int Read()
		{
			if (charBuffer == null)
			{
				charBuffer = new char[128];
			}
			if (Read(charBuffer, 0, 1) == 0)
			{
				return -1;
			}
			return charBuffer[0];
		}

		public virtual int Read(byte[] buffer, int index, int count)
		{
			if (m_stream == null)
			{
				if (m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is null");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index is less than 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count is less than 0");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("buffer is too small");
			}
			return m_stream.Read(buffer, index, count);
		}

		public virtual int Read(char[] buffer, int index, int count)
		{
			if (m_stream == null)
			{
				if (m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is null");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index is less than 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count is less than 0");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("buffer is too small");
			}
			int bytes_read;
			return ReadCharBytes(buffer, index, count, out bytes_read);
		}

		private int ReadCharBytes(char[] buffer, int index, int count, out int bytes_read)
		{
			int i = 0;
			bytes_read = 0;
			for (; i < count; i++)
			{
				int num = 0;
				int chars;
				do
				{
					CheckBuffer(num + 1);
					int num2 = m_stream.ReadByte();
					if (num2 == -1)
					{
						return i;
					}
					m_buffer[num++] = (byte)num2;
					bytes_read++;
					chars = m_encoding.GetChars(m_buffer, 0, num, buffer, index + i);
				}
				while (chars <= 0);
			}
			return i;
		}

		protected int Read7BitEncodedInt()
		{
			int num = 0;
			int num2 = 0;
			int i;
			for (i = 0; i < 5; i++)
			{
				byte b = ReadByte();
				num |= (b & 0x7F) << num2;
				num2 += 7;
				if ((b & 0x80) == 0)
				{
					break;
				}
			}
			if (i < 5)
			{
				return num;
			}
			throw new FormatException("Too many bytes in what should have been a 7 bit encoded Int32.");
		}

		public virtual bool ReadBoolean()
		{
			return ReadByte() != 0;
		}

		public virtual byte ReadByte()
		{
			if (m_stream == null)
			{
				if (m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			int num = m_stream.ReadByte();
			if (num != -1)
			{
				return (byte)num;
			}
			throw new EndOfStreamException();
		}

		public virtual byte[] ReadBytes(int count)
		{
			if (m_stream == null)
			{
				if (m_disposed)
				{
					throw new ObjectDisposedException("BinaryReader", "Cannot read from a closed BinaryReader.");
				}
				throw new IOException("Stream is invalid");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count is less than 0");
			}
			byte[] array = new byte[count];
			int i;
			int num;
			for (i = 0; i < count; i += num)
			{
				num = m_stream.Read(array, i, count - i);
				if (num == 0)
				{
					break;
				}
			}
			if (i != count)
			{
				byte[] array2 = new byte[i];
				Buffer.BlockCopyInternal(array, 0, array2, 0, i);
				return array2;
			}
			return array;
		}

		public virtual char ReadChar()
		{
			int num = Read();
			if (num == -1)
			{
				throw new EndOfStreamException();
			}
			return (char)num;
		}

		public virtual char[] ReadChars(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count is less than 0");
			}
			if (count == 0)
			{
				return new char[0];
			}
			char[] array = new char[count];
			int num = Read(array, 0, count);
			if (num == 0)
			{
				throw new EndOfStreamException();
			}
			if (num != array.Length)
			{
				char[] array2 = new char[num];
				Array.Copy(array, 0, array2, 0, num);
				return array2;
			}
			return array;
		}

		public unsafe virtual decimal ReadDecimal()
		{
			FillBuffer(16);
			decimal result = default(decimal);
			byte* ptr = (byte*)(&result);
			if (BitConverter.IsLittleEndian)
			{
				for (int i = 0; i < 16; i++)
				{
					if (i < 4)
					{
						ptr[i + 8] = m_buffer[i];
					}
					else if (i < 8)
					{
						ptr[i + 8] = m_buffer[i];
					}
					else if (i < 12)
					{
						ptr[i - 4] = m_buffer[i];
					}
					else if (i < 16)
					{
						ptr[i - 12] = m_buffer[i];
					}
				}
			}
			else
			{
				for (int j = 0; j < 16; j++)
				{
					if (j < 4)
					{
						ptr[11 - j] = m_buffer[j];
					}
					else if (j < 8)
					{
						ptr[19 - j] = m_buffer[j];
					}
					else if (j < 12)
					{
						ptr[15 - j] = m_buffer[j];
					}
					else if (j < 16)
					{
						ptr[15 - j] = m_buffer[j];
					}
				}
			}
			return result;
		}

		public virtual double ReadDouble()
		{
			FillBuffer(8);
			return BitConverterLE.ToDouble(m_buffer, 0);
		}

		public virtual short ReadInt16()
		{
			FillBuffer(2);
			return (short)(m_buffer[0] | (m_buffer[1] << 8));
		}

		public virtual int ReadInt32()
		{
			FillBuffer(4);
			return m_buffer[0] | (m_buffer[1] << 8) | (m_buffer[2] << 16) | (m_buffer[3] << 24);
		}

		public virtual long ReadInt64()
		{
			FillBuffer(8);
			uint num = (uint)(m_buffer[0] | (m_buffer[1] << 8) | (m_buffer[2] << 16) | (m_buffer[3] << 24));
			uint num2 = (uint)(m_buffer[4] | (m_buffer[5] << 8) | (m_buffer[6] << 16) | (m_buffer[7] << 24));
			return (long)(((ulong)num2 << 32) | num);
		}

		[CLSCompliant(false)]
		public virtual sbyte ReadSByte()
		{
			return (sbyte)ReadByte();
		}

		public virtual string ReadString()
		{
			int num = Read7BitEncodedInt();
			if (num < 0)
			{
				throw new IOException("Invalid binary file (string len < 0)");
			}
			if (num == 0)
			{
				return string.Empty;
			}
			if (charBuffer == null)
			{
				charBuffer = new char[128];
			}
			StringBuilder stringBuilder = null;
			do
			{
				int num2 = ((num <= 128) ? num : 128);
				FillBuffer(num2);
				int chars = decoder.GetChars(m_buffer, 0, num2, charBuffer, 0);
				if (stringBuilder == null && num2 == num)
				{
					return new string(charBuffer, 0, chars);
				}
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(num);
				}
				stringBuilder.Append(charBuffer, 0, chars);
				num -= num2;
			}
			while (num > 0);
			return stringBuilder.ToString();
		}

		public virtual float ReadSingle()
		{
			FillBuffer(4);
			return BitConverterLE.ToSingle(m_buffer, 0);
		}

		[CLSCompliant(false)]
		public virtual ushort ReadUInt16()
		{
			FillBuffer(2);
			return (ushort)(m_buffer[0] | (m_buffer[1] << 8));
		}

		[CLSCompliant(false)]
		public virtual uint ReadUInt32()
		{
			FillBuffer(4);
			return (uint)(m_buffer[0] | (m_buffer[1] << 8) | (m_buffer[2] << 16) | (m_buffer[3] << 24));
		}

		[CLSCompliant(false)]
		public virtual ulong ReadUInt64()
		{
			FillBuffer(8);
			uint num = (uint)(m_buffer[0] | (m_buffer[1] << 8) | (m_buffer[2] << 16) | (m_buffer[3] << 24));
			uint num2 = (uint)(m_buffer[4] | (m_buffer[5] << 8) | (m_buffer[6] << 16) | (m_buffer[7] << 24));
			return ((ulong)num2 << 32) | num;
		}

		private void CheckBuffer(int length)
		{
			if (m_buffer.Length <= length)
			{
				byte[] array = new byte[length];
				Buffer.BlockCopyInternal(m_buffer, 0, array, 0, m_buffer.Length);
				m_buffer = array;
			}
		}
	}
}
