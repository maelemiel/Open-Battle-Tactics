using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public class StreamReader : TextReader
	{
		private class NullStreamReader : StreamReader
		{
			public override Stream BaseStream
			{
				get
				{
					return Stream.Null;
				}
			}

			public override Encoding CurrentEncoding
			{
				get
				{
					return Encoding.Unicode;
				}
			}

			public override int Peek()
			{
				return -1;
			}

			public override int Read()
			{
				return -1;
			}

			public override int Read([In][Out] char[] buffer, int index, int count)
			{
				return 0;
			}

			public override string ReadLine()
			{
				return null;
			}

			public override string ReadToEnd()
			{
				return string.Empty;
			}
		}

		private const int DefaultBufferSize = 1024;

		private const int DefaultFileBufferSize = 4096;

		private const int MinimumBufferSize = 128;

		private byte[] input_buffer;

		private char[] decoded_buffer;

		private int decoded_count;

		private int pos;

		private int buffer_size;

		private int do_checks;

		private Encoding encoding;

		private Decoder decoder;

		private Stream base_stream;

		private bool mayBlock;

		private StringBuilder line_builder;

		public new static readonly StreamReader Null = new NullStreamReader();

		private bool foundCR;

		public virtual Stream BaseStream
		{
			get
			{
				return base_stream;
			}
		}

		public virtual Encoding CurrentEncoding
		{
			get
			{
				if (encoding == null)
				{
					throw new Exception();
				}
				return encoding;
			}
		}

		public bool EndOfStream
		{
			get
			{
				return Peek() < 0;
			}
		}

		internal StreamReader()
		{
		}

		public StreamReader(Stream stream)
			: this(stream, Encoding.UTF8Unmarked, true, 1024)
		{
		}

		public StreamReader(Stream stream, bool detectEncodingFromByteOrderMarks)
			: this(stream, Encoding.UTF8Unmarked, detectEncodingFromByteOrderMarks, 1024)
		{
		}

		public StreamReader(Stream stream, Encoding encoding)
			: this(stream, encoding, true, 1024)
		{
		}

		public StreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
			: this(stream, encoding, detectEncodingFromByteOrderMarks, 1024)
		{
		}

		public StreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
		{
			Initialize(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize);
		}

		public StreamReader(string path)
			: this(path, Encoding.UTF8Unmarked, true, 4096)
		{
		}

		public StreamReader(string path, bool detectEncodingFromByteOrderMarks)
			: this(path, Encoding.UTF8Unmarked, detectEncodingFromByteOrderMarks, 4096)
		{
		}

		public StreamReader(string path, Encoding encoding)
			: this(path, encoding, true, 4096)
		{
		}

		public StreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
			: this(path, encoding, detectEncodingFromByteOrderMarks, 4096)
		{
		}

		public StreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (string.Empty == path)
			{
				throw new ArgumentException("Empty path not allowed");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("path contains invalid characters");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "The minimum size of the buffer must be positive");
			}
			Stream stream = File.OpenRead(path);
			Initialize(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize);
		}

		internal void Initialize(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("Cannot read stream");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "The minimum size of the buffer must be positive");
			}
			if (bufferSize < 128)
			{
				bufferSize = 128;
			}
			base_stream = stream;
			input_buffer = new byte[bufferSize];
			buffer_size = bufferSize;
			this.encoding = encoding;
			decoder = encoding.GetDecoder();
			byte[] preamble = encoding.GetPreamble();
			do_checks = (detectEncodingFromByteOrderMarks ? 1 : 0);
			do_checks += ((preamble.Length != 0) ? 2 : 0);
			decoded_buffer = new char[encoding.GetMaxCharCount(bufferSize) + 1];
			decoded_count = 0;
			pos = 0;
		}

		public override void Close()
		{
			Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && base_stream != null)
			{
				base_stream.Close();
			}
			input_buffer = null;
			decoded_buffer = null;
			encoding = null;
			decoder = null;
			base_stream = null;
			base.Dispose(disposing);
		}

		private int DoChecks(int count)
		{
			if ((do_checks & 2) == 2)
			{
				byte[] preamble = encoding.GetPreamble();
				int num = preamble.Length;
				if (count >= num)
				{
					int i;
					for (i = 0; i < num && input_buffer[i] == preamble[i]; i++)
					{
					}
					if (i == num)
					{
						return i;
					}
				}
			}
			if ((do_checks & 1) == 1)
			{
				if (count < 2)
				{
					return 0;
				}
				if (input_buffer[0] == 254 && input_buffer[1] == byte.MaxValue)
				{
					encoding = Encoding.BigEndianUnicode;
					return 2;
				}
				if (count < 3)
				{
					return 0;
				}
				if (input_buffer[0] == 239 && input_buffer[1] == 187 && input_buffer[2] == 191)
				{
					encoding = Encoding.UTF8Unmarked;
					return 3;
				}
				if (count < 4)
				{
					if (input_buffer[0] == byte.MaxValue && input_buffer[1] == 254 && input_buffer[2] != 0)
					{
						encoding = Encoding.Unicode;
						return 2;
					}
					return 0;
				}
				if (input_buffer[0] == 0 && input_buffer[1] == 0 && input_buffer[2] == 254 && input_buffer[3] == byte.MaxValue)
				{
					encoding = Encoding.BigEndianUTF32;
					return 4;
				}
				if (input_buffer[0] == byte.MaxValue && input_buffer[1] == 254)
				{
					if (input_buffer[2] == 0 && input_buffer[3] == 0)
					{
						encoding = Encoding.UTF32;
						return 4;
					}
					encoding = Encoding.Unicode;
					return 2;
				}
			}
			return 0;
		}

		public void DiscardBufferedData()
		{
			pos = (decoded_count = 0);
			mayBlock = false;
			decoder = encoding.GetDecoder();
		}

		private int ReadBuffer()
		{
			pos = 0;
			int num = 0;
			decoded_count = 0;
			int num2 = 0;
			do
			{
				num = base_stream.Read(input_buffer, 0, buffer_size);
				if (num <= 0)
				{
					return 0;
				}
				mayBlock = num < buffer_size;
				if (do_checks > 0)
				{
					Encoding encoding = this.encoding;
					num2 = DoChecks(num);
					if (encoding != this.encoding)
					{
						int num3 = encoding.GetMaxCharCount(buffer_size) + 1;
						int num4 = this.encoding.GetMaxCharCount(buffer_size) + 1;
						if (num3 != num4)
						{
							decoded_buffer = new char[num4];
						}
						decoder = this.encoding.GetDecoder();
					}
					do_checks = 0;
					num -= num2;
				}
				decoded_count += decoder.GetChars(input_buffer, num2, num, decoded_buffer, 0);
				num2 = 0;
			}
			while (decoded_count == 0);
			return decoded_count;
		}

		public override int Peek()
		{
			if (base_stream == null)
			{
				throw new ObjectDisposedException("StreamReader", "Cannot read from a closed StreamReader");
			}
			if (pos >= decoded_count && ReadBuffer() == 0)
			{
				return -1;
			}
			return decoded_buffer[pos];
		}

		internal bool DataAvailable()
		{
			return pos < decoded_count;
		}

		public override int Read()
		{
			if (base_stream == null)
			{
				throw new ObjectDisposedException("StreamReader", "Cannot read from a closed StreamReader");
			}
			if (pos >= decoded_count && ReadBuffer() == 0)
			{
				return -1;
			}
			return decoded_buffer[pos++];
		}

		public override int Read([In][Out] char[] buffer, int index, int count)
		{
			if (base_stream == null)
			{
				throw new ObjectDisposedException("StreamReader", "Cannot read from a closed StreamReader");
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
			int num = 0;
			while (count > 0)
			{
				if (pos >= decoded_count && ReadBuffer() == 0)
				{
					return (num > 0) ? num : 0;
				}
				int num2 = Math.Min(decoded_count - pos, count);
				Array.Copy(decoded_buffer, pos, buffer, index, num2);
				pos += num2;
				index += num2;
				count -= num2;
				num += num2;
				if (mayBlock)
				{
					break;
				}
			}
			return num;
		}

		private int FindNextEOL()
		{
			char c = '\0';
			while (pos < decoded_count)
			{
				c = decoded_buffer[pos];
				if (c == '\n')
				{
					pos++;
					int num = ((!foundCR) ? (pos - 1) : (pos - 2));
					if (num < 0)
					{
						num = 0;
					}
					foundCR = false;
					return num;
				}
				if (foundCR)
				{
					foundCR = false;
					if (pos == 0)
					{
						return -2;
					}
					return pos - 1;
				}
				foundCR = c == '\r';
				pos++;
			}
			return -1;
		}

		public override string ReadLine()
		{
			if (base_stream == null)
			{
				throw new ObjectDisposedException("StreamReader", "Cannot read from a closed StreamReader");
			}
			if (pos >= decoded_count && ReadBuffer() == 0)
			{
				return null;
			}
			int num = pos;
			int num2 = FindNextEOL();
			if (num2 < decoded_count && num2 >= num)
			{
				return new string(decoded_buffer, num, num2 - num);
			}
			if (num2 == -2)
			{
				return line_builder.ToString(0, line_builder.Length);
			}
			if (line_builder == null)
			{
				line_builder = new StringBuilder();
			}
			else
			{
				line_builder.Length = 0;
			}
			do
			{
				if (foundCR)
				{
					decoded_count--;
				}
				line_builder.Append(decoded_buffer, num, decoded_count - num);
				if (ReadBuffer() == 0)
				{
					if (line_builder.Capacity > 32768)
					{
						StringBuilder stringBuilder = line_builder;
						line_builder = null;
						return stringBuilder.ToString(0, stringBuilder.Length);
					}
					return line_builder.ToString(0, line_builder.Length);
				}
				num = pos;
				num2 = FindNextEOL();
				if (num2 < decoded_count && num2 >= num)
				{
					line_builder.Append(decoded_buffer, num, num2 - num);
					if (line_builder.Capacity > 32768)
					{
						StringBuilder stringBuilder2 = line_builder;
						line_builder = null;
						return stringBuilder2.ToString(0, stringBuilder2.Length);
					}
					return line_builder.ToString(0, line_builder.Length);
				}
			}
			while (num2 != -2);
			return line_builder.ToString(0, line_builder.Length);
		}

		public override string ReadToEnd()
		{
			if (base_stream == null)
			{
				throw new ObjectDisposedException("StreamReader", "Cannot read from a closed StreamReader");
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = decoded_buffer.Length;
			char[] array = new char[num];
			int charCount;
			while ((charCount = Read(array, 0, num)) > 0)
			{
				stringBuilder.Append(array, 0, charCount);
			}
			return stringBuilder.ToString();
		}
	}
}
