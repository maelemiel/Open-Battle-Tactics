namespace System.Net.Sockets
{
	public class SendPacketsElement
	{
		public byte[] Buffer { get; private set; }

		public int Count { get; private set; }

		public bool EndOfPacket { get; private set; }

		public string FilePath { get; private set; }

		public int Offset { get; private set; }

		public SendPacketsElement(byte[] buffer)
			: this(buffer, 0, (buffer != null) ? buffer.Length : 0)
		{
		}

		public SendPacketsElement(byte[] buffer, int offset, int count)
			: this(buffer, offset, count, false)
		{
		}

		public SendPacketsElement(byte[] buffer, int offset, int count, bool endOfPacket)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = buffer.Length;
			if (offset < 0 || offset >= num)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0 || offset + count >= num)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			Buffer = buffer;
			Offset = offset;
			Count = count;
			EndOfPacket = endOfPacket;
			FilePath = null;
		}

		public SendPacketsElement(string filepath)
			: this(filepath, 0, 0, false)
		{
		}

		public SendPacketsElement(string filepath, int offset, int count)
			: this(filepath, offset, count, false)
		{
		}

		public SendPacketsElement(string filepath, int offset, int count, bool endOfPacket)
		{
			if (filepath == null)
			{
				throw new ArgumentNullException("filepath");
			}
			Buffer = null;
			Offset = offset;
			Count = count;
			EndOfPacket = endOfPacket;
			FilePath = filepath;
		}
	}
}
