using System;
using System.IO;

namespace Mono.Security.Protocol.Tls
{
	internal class TlsStream : Stream
	{
		private const int temp_size = 4;

		private bool canRead;

		private bool canWrite;

		private MemoryStream buffer;

		private byte[] temp;

		public bool EOF
		{
			get
			{
				if (Position < Length)
				{
					return false;
				}
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return canWrite;
			}
		}

		public override bool CanRead
		{
			get
			{
				return canRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return buffer.CanSeek;
			}
		}

		public override long Position
		{
			get
			{
				return buffer.Position;
			}
			set
			{
				buffer.Position = value;
			}
		}

		public override long Length
		{
			get
			{
				return buffer.Length;
			}
		}

		public TlsStream()
		{
			buffer = new MemoryStream(0);
			canRead = false;
			canWrite = true;
		}

		public TlsStream(byte[] data)
		{
			if (data != null)
			{
				buffer = new MemoryStream(data);
			}
			else
			{
				buffer = new MemoryStream();
			}
			canRead = true;
			canWrite = false;
		}

		private byte[] ReadSmallValue(int length)
		{
			if (length > 4)
			{
				throw new ArgumentException("8 bytes maximum");
			}
			if (temp == null)
			{
				temp = new byte[4];
			}
			if (Read(temp, 0, length) != length)
			{
				throw new TlsException(string.Format("buffer underrun"));
			}
			return temp;
		}

		public new byte ReadByte()
		{
			byte[] array = ReadSmallValue(1);
			return array[0];
		}

		public short ReadInt16()
		{
			byte[] array = ReadSmallValue(2);
			return (short)((array[0] << 8) | array[1]);
		}

		public int ReadInt24()
		{
			byte[] array = ReadSmallValue(3);
			return (array[0] << 16) | (array[1] << 8) | array[2];
		}

		public int ReadInt32()
		{
			byte[] array = ReadSmallValue(4);
			return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
		}

		public byte[] ReadBytes(int count)
		{
			byte[] result = new byte[count];
			if (Read(result, 0, count) != count)
			{
				throw new TlsException("buffer underrun");
			}
			return result;
		}

		public void Write(byte value)
		{
			if (temp == null)
			{
				temp = new byte[4];
			}
			temp[0] = value;
			Write(temp, 0, 1);
		}

		public void Write(short value)
		{
			if (temp == null)
			{
				temp = new byte[4];
			}
			temp[0] = (byte)(value >> 8);
			temp[1] = (byte)value;
			Write(temp, 0, 2);
		}

		public void WriteInt24(int value)
		{
			if (temp == null)
			{
				temp = new byte[4];
			}
			temp[0] = (byte)(value >> 16);
			temp[1] = (byte)(value >> 8);
			temp[2] = (byte)value;
			Write(temp, 0, 3);
		}

		public void Write(int value)
		{
			if (temp == null)
			{
				temp = new byte[4];
			}
			temp[0] = (byte)(value >> 24);
			temp[1] = (byte)(value >> 16);
			temp[2] = (byte)(value >> 8);
			temp[3] = (byte)value;
			Write(temp, 0, 4);
		}

		public void Write(ulong value)
		{
			Write((int)(value >> 32));
			Write((int)value);
		}

		public void Write(byte[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}

		public void Reset()
		{
			buffer.SetLength(0L);
			buffer.Position = 0L;
		}

		public byte[] ToArray()
		{
			return buffer.ToArray();
		}

		public override void Flush()
		{
			buffer.Flush();
		}

		public override void SetLength(long length)
		{
			buffer.SetLength(length);
		}

		public override long Seek(long offset, SeekOrigin loc)
		{
			return buffer.Seek(offset, loc);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (canRead)
			{
				return this.buffer.Read(buffer, offset, count);
			}
			throw new InvalidOperationException("Read operations are not allowed by this stream");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (canWrite)
			{
				this.buffer.Write(buffer, offset, count);
				return;
			}
			throw new InvalidOperationException("Write operations are not allowed by this stream");
		}
	}
}
