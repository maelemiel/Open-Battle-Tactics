using System.IO;

namespace System.Data.SqlClient
{
	internal sealed class SqlXmlTextReader : TextReader, IDisposable
	{
		private bool disposed;

		private bool eof;

		private SqlDataReader reader;

		private string localBuffer = "<results>";

		private int position;

		internal SqlXmlTextReader(SqlDataReader reader)
		{
			this.reader = reader;
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override void Close()
		{
			reader.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					Close();
					((IDisposable)reader).Dispose();
				}
				disposed = true;
			}
		}

		private bool GetNextBuffer()
		{
			if (eof)
			{
				localBuffer = null;
				return false;
			}
			position = 0;
			if (reader.Read())
			{
				localBuffer = reader.GetString(0);
			}
			else if (reader.NextResult() && reader.Read())
			{
				localBuffer = reader.GetString(0);
			}
			else
			{
				eof = true;
				localBuffer = "</results>";
			}
			return true;
		}

		public override int Peek()
		{
			if ((localBuffer == null || localBuffer.Length == 0) && !GetNextBuffer())
			{
				return -1;
			}
			if (eof && position >= localBuffer.Length)
			{
				return -1;
			}
			return localBuffer[position];
		}

		public override int Read()
		{
			int result = Peek();
			position++;
			if (!eof && position >= localBuffer.Length)
			{
				GetNextBuffer();
			}
			return result;
		}

		public override int Read(char[] buffer, int index, int count)
		{
			bool flag = true;
			int num = 0;
			if (localBuffer == null)
			{
				flag = GetNextBuffer();
			}
			while (flag && count - num > localBuffer.Length - position)
			{
				localBuffer.CopyTo(position, buffer, index + num, localBuffer.Length);
				num += localBuffer.Length;
				flag = GetNextBuffer();
			}
			if (flag && num < count)
			{
				localBuffer.CopyTo(position, buffer, index + num, count - num);
				position += count - num;
			}
			return num;
		}

		public override int ReadBlock(char[] buffer, int index, int count)
		{
			return Read(buffer, index, count);
		}

		public override string ReadLine()
		{
			bool flag = true;
			if (localBuffer == null)
			{
				flag = GetNextBuffer();
			}
			if (!flag)
			{
				return null;
			}
			string result = localBuffer;
			GetNextBuffer();
			return result;
		}

		public override string ReadToEnd()
		{
			string text = string.Empty;
			bool flag = true;
			if (localBuffer == null)
			{
				flag = GetNextBuffer();
			}
			while (flag)
			{
				text += localBuffer;
				flag = GetNextBuffer();
			}
			return text;
		}
	}
}
