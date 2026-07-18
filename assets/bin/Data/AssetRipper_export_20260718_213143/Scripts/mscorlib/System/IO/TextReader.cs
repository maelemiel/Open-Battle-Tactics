using System.Runtime.InteropServices;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public abstract class TextReader : IDisposable
	{
		private class NullTextReader : TextReader
		{
			public override string ReadLine()
			{
				return null;
			}
		}

		public static readonly TextReader Null;

		static TextReader()
		{
			Null = new NullTextReader();
		}

		public virtual void Close()
		{
			Dispose(true);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
		}

		public virtual int Peek()
		{
			return -1;
		}

		public virtual int Read()
		{
			return -1;
		}

		public virtual int Read([In][Out] char[] buffer, int index, int count)
		{
			int i;
			for (i = 0; i < count; i++)
			{
				int num;
				if ((num = Read()) == -1)
				{
					return i;
				}
				buffer[index + i] = (char)num;
			}
			return i;
		}

		public virtual int ReadBlock([In][Out] char[] buffer, int index, int count)
		{
			int num = 0;
			int num2 = 0;
			do
			{
				num2 = Read(buffer, index, count);
				index += num2;
				num += num2;
				count -= num2;
			}
			while (num2 != 0 && count > 0);
			return num;
		}

		public virtual string ReadLine()
		{
			return string.Empty;
		}

		public virtual string ReadToEnd()
		{
			return string.Empty;
		}

		public static TextReader Synchronized(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader is null");
			}
			if (reader is SynchronizedReader)
			{
				return reader;
			}
			return new SynchronizedReader(reader);
		}
	}
}
