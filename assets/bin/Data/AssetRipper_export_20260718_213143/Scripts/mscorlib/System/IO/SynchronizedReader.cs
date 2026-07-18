namespace System.IO
{
	[Serializable]
	internal class SynchronizedReader : TextReader
	{
		private TextReader reader;

		public SynchronizedReader(TextReader reader)
		{
			this.reader = reader;
		}

		public override void Close()
		{
			lock (this)
			{
				reader.Close();
			}
		}

		public override int Peek()
		{
			lock (this)
			{
				return reader.Peek();
			}
		}

		public override int ReadBlock(char[] buffer, int index, int count)
		{
			lock (this)
			{
				return reader.ReadBlock(buffer, index, count);
			}
		}

		public override string ReadLine()
		{
			lock (this)
			{
				return reader.ReadLine();
			}
		}

		public override string ReadToEnd()
		{
			lock (this)
			{
				return reader.ReadToEnd();
			}
		}

		public override int Read()
		{
			lock (this)
			{
				return reader.Read();
			}
		}

		public override int Read(char[] buffer, int index, int count)
		{
			lock (this)
			{
				return reader.Read(buffer, index, count);
			}
		}
	}
}
