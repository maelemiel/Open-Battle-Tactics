using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public class StreamWriter : TextWriter
	{
		private const int DefaultBufferSize = 1024;

		private const int DefaultFileBufferSize = 4096;

		private const int MinimumBufferSize = 256;

		private Encoding internalEncoding;

		private Stream internalStream;

		private bool iflush;

		private byte[] byte_buf;

		private int byte_pos;

		private char[] decode_buf;

		private int decode_pos;

		private bool DisposedAlready;

		private bool preamble_done;

		public new static readonly StreamWriter Null = new StreamWriter(Stream.Null, Encoding.UTF8Unmarked, 1);

		public virtual bool AutoFlush
		{
			get
			{
				return iflush;
			}
			set
			{
				iflush = value;
				if (iflush)
				{
					Flush();
				}
			}
		}

		public virtual Stream BaseStream
		{
			get
			{
				return internalStream;
			}
		}

		public override Encoding Encoding
		{
			get
			{
				return internalEncoding;
			}
		}

		public StreamWriter(Stream stream)
			: this(stream, Encoding.UTF8Unmarked, 1024)
		{
		}

		public StreamWriter(Stream stream, Encoding encoding)
			: this(stream, encoding, 1024)
		{
		}

		public StreamWriter(Stream stream, Encoding encoding, int bufferSize)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException("Can not write to stream");
			}
			internalStream = stream;
			Initialize(encoding, bufferSize);
		}

		public StreamWriter(string path)
			: this(path, false, Encoding.UTF8Unmarked, 4096)
		{
		}

		public StreamWriter(string path, bool append)
			: this(path, append, Encoding.UTF8Unmarked, 4096)
		{
		}

		public StreamWriter(string path, bool append, Encoding encoding)
			: this(path, append, encoding, 4096)
		{
		}

		public StreamWriter(string path, bool append, Encoding encoding, int bufferSize)
		{
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			internalStream = new FileStream(path, (!append) ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Read);
			if (append)
			{
				internalStream.Position = internalStream.Length;
			}
			else
			{
				internalStream.SetLength(0L);
			}
			Initialize(encoding, bufferSize);
		}

		internal void Initialize(Encoding encoding, int bufferSize)
		{
			internalEncoding = encoding;
			decode_pos = (byte_pos = 0);
			int num = Math.Max(bufferSize, 256);
			decode_buf = new char[num];
			byte_buf = new byte[encoding.GetMaxByteCount(num)];
			if (internalStream.CanSeek && internalStream.Position > 0)
			{
				preamble_done = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			Exception ex = null;
			if (!DisposedAlready && disposing && internalStream != null)
			{
				try
				{
					Flush();
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				DisposedAlready = true;
				try
				{
					internalStream.Close();
				}
				catch (Exception ex3)
				{
					if (ex == null)
					{
						ex = ex3;
					}
				}
			}
			internalStream = null;
			byte_buf = null;
			internalEncoding = null;
			decode_buf = null;
			if (ex != null)
			{
				throw ex;
			}
		}

		public override void Flush()
		{
			if (DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			Decode();
			if (byte_pos > 0)
			{
				FlushBytes();
				internalStream.Flush();
			}
		}

		private void FlushBytes()
		{
			if (!preamble_done && byte_pos > 0)
			{
				byte[] preamble = internalEncoding.GetPreamble();
				if (preamble.Length > 0)
				{
					internalStream.Write(preamble, 0, preamble.Length);
				}
				preamble_done = true;
			}
			internalStream.Write(byte_buf, 0, byte_pos);
			byte_pos = 0;
		}

		private void Decode()
		{
			if (byte_pos > 0)
			{
				FlushBytes();
			}
			if (decode_pos > 0)
			{
				int bytes = internalEncoding.GetBytes(decode_buf, 0, decode_pos, byte_buf, byte_pos);
				byte_pos += bytes;
				decode_pos = 0;
			}
		}

		public override void Write(char[] buffer, int index, int count)
		{
			if (DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (index > buffer.Length - count)
			{
				throw new ArgumentException("index + count > buffer.Length");
			}
			LowLevelWrite(buffer, index, count);
			if (iflush)
			{
				Flush();
			}
		}

		private void LowLevelWrite(char[] buffer, int index, int count)
		{
			while (count > 0)
			{
				int num = decode_buf.Length - decode_pos;
				if (num == 0)
				{
					Decode();
					num = decode_buf.Length;
				}
				if (num > count)
				{
					num = count;
				}
				Buffer.BlockCopy(buffer, index * 2, decode_buf, decode_pos * 2, num * 2);
				count -= num;
				index += num;
				decode_pos += num;
			}
		}

		private void LowLevelWrite(string s)
		{
			int num = s.Length;
			int num2 = 0;
			while (num > 0)
			{
				int num3 = decode_buf.Length - decode_pos;
				if (num3 == 0)
				{
					Decode();
					num3 = decode_buf.Length;
				}
				if (num3 > num)
				{
					num3 = num;
				}
				for (int i = 0; i < num3; i++)
				{
					decode_buf[i + decode_pos] = s[i + num2];
				}
				num -= num3;
				num2 += num3;
				decode_pos += num3;
			}
		}

		public override void Write(char value)
		{
			if (DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			if (decode_pos >= decode_buf.Length)
			{
				Decode();
			}
			decode_buf[decode_pos++] = value;
			if (iflush)
			{
				Flush();
			}
		}

		public override void Write(char[] buffer)
		{
			if (DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			if (buffer != null)
			{
				LowLevelWrite(buffer, 0, buffer.Length);
			}
			if (iflush)
			{
				Flush();
			}
		}

		public override void Write(string value)
		{
			if (DisposedAlready)
			{
				throw new ObjectDisposedException("StreamWriter");
			}
			if (value != null)
			{
				LowLevelWrite(value);
			}
			if (iflush)
			{
				Flush();
			}
		}

		public override void Close()
		{
			Dispose(true);
		}

		~StreamWriter()
		{
			Dispose(false);
		}
	}
}
