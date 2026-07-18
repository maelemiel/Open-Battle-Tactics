using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Xml
{
	internal class NonBlockingStreamReader : TextReader
	{
		private const int DefaultBufferSize = 1024;

		private const int DefaultFileBufferSize = 4096;

		private const int MinimumBufferSize = 128;

		private byte[] input_buffer;

		private char[] decoded_buffer;

		private int decoded_count;

		private int pos;

		private int buffer_size;

		private Encoding encoding;

		private Decoder decoder;

		private Stream base_stream;

		private bool mayBlock;

		private StringBuilder line_builder;

		private bool foundCR;

		public Encoding Encoding
		{
			get
			{
				return encoding;
			}
		}

		public NonBlockingStreamReader(Stream stream, Encoding encoding)
		{
			int num = 1024;
			base_stream = stream;
			input_buffer = new byte[num];
			buffer_size = num;
			this.encoding = encoding;
			decoder = encoding.GetDecoder();
			decoded_buffer = new char[encoding.GetMaxCharCount(num)];
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

		public void DiscardBufferedData()
		{
			pos = (decoded_count = 0);
			mayBlock = false;
			decoder.Reset();
		}

		private int ReadBuffer()
		{
			pos = 0;
			int num = 0;
			decoded_count = 0;
			int byteIndex = 0;
			do
			{
				num = base_stream.Read(input_buffer, 0, buffer_size);
				if (num == 0)
				{
					return 0;
				}
				mayBlock = num < buffer_size;
				decoded_count += decoder.GetChars(input_buffer, byteIndex, num, decoded_buffer, 0);
				byteIndex = 0;
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
			if (pos >= decoded_count && (mayBlock || ReadBuffer() == 0))
			{
				return -1;
			}
			return decoded_buffer[pos];
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

		public override int Read([In][Out] char[] dest_buffer, int index, int count)
		{
			if (base_stream == null)
			{
				throw new ObjectDisposedException("StreamReader", "Cannot read from a closed StreamReader");
			}
			if (dest_buffer == null)
			{
				throw new ArgumentNullException("dest_buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (index > dest_buffer.Length - count)
			{
				throw new ArgumentException("index + count > dest_buffer.Length");
			}
			int num = 0;
			if (pos >= decoded_count && ReadBuffer() == 0)
			{
				return (num > 0) ? num : 0;
			}
			int num2 = Math.Min(decoded_count - pos, count);
			Array.Copy(decoded_buffer, pos, dest_buffer, index, num2);
			pos += num2;
			index += num2;
			count -= num2;
			return num + num2;
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
				line_builder.Append(new string(decoded_buffer, num, decoded_count - num));
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
			}
			while (num2 >= decoded_count || num2 < num);
			line_builder.Append(new string(decoded_buffer, num, num2 - num));
			if (line_builder.Capacity > 32768)
			{
				StringBuilder stringBuilder2 = line_builder;
				line_builder = null;
				return stringBuilder2.ToString(0, stringBuilder2.Length);
			}
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
			while ((charCount = Read(array, 0, num)) != 0)
			{
				stringBuilder.Append(array, 0, charCount);
			}
			return stringBuilder.ToString();
		}
	}
}
