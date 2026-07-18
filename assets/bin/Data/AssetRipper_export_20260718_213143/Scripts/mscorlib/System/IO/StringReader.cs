using System.Runtime.InteropServices;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public class StringReader : TextReader
	{
		private string source;

		private int nextChar;

		private int sourceLength;

		public StringReader(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			source = s;
			nextChar = 0;
			sourceLength = s.Length;
		}

		public override void Close()
		{
			Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			source = null;
			base.Dispose(disposing);
		}

		public override int Peek()
		{
			CheckObjectDisposedException();
			if (nextChar >= sourceLength)
			{
				return -1;
			}
			return source[nextChar];
		}

		public override int Read()
		{
			CheckObjectDisposedException();
			if (nextChar >= sourceLength)
			{
				return -1;
			}
			return source[nextChar++];
		}

		public override int Read([In][Out] char[] buffer, int index, int count)
		{
			CheckObjectDisposedException();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException();
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = ((nextChar <= sourceLength - count) ? count : (sourceLength - nextChar));
			source.CopyTo(nextChar, buffer, index, num);
			nextChar += num;
			return num;
		}

		public override string ReadLine()
		{
			CheckObjectDisposedException();
			if (nextChar >= source.Length)
			{
				return null;
			}
			int num = source.IndexOf('\r', nextChar);
			int num2 = source.IndexOf('\n', nextChar);
			bool flag = false;
			int num3;
			if (num == -1)
			{
				if (num2 == -1)
				{
					return ReadToEnd();
				}
				num3 = num2;
			}
			else if (num2 == -1)
			{
				num3 = num;
			}
			else
			{
				num3 = ((num <= num2) ? num : num2);
				flag = num + 1 == num2;
			}
			string result = source.Substring(nextChar, num3 - nextChar);
			nextChar = num3 + ((!flag) ? 1 : 2);
			return result;
		}

		public override string ReadToEnd()
		{
			CheckObjectDisposedException();
			string result = source.Substring(nextChar, sourceLength - nextChar);
			nextChar = sourceLength;
			return result;
		}

		private void CheckObjectDisposedException()
		{
			if (source == null)
			{
				throw new ObjectDisposedException("StringReader", Locale.GetText("Cannot read from a closed StringReader"));
			}
		}
	}
}
