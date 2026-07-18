using System.Text;

namespace System.IO
{
	[Serializable]
	internal class SynchronizedWriter : TextWriter
	{
		private TextWriter writer;

		private bool neverClose;

		public override Encoding Encoding
		{
			get
			{
				lock (this)
				{
					return writer.Encoding;
				}
			}
		}

		public override IFormatProvider FormatProvider
		{
			get
			{
				lock (this)
				{
					return writer.FormatProvider;
				}
			}
		}

		public override string NewLine
		{
			get
			{
				lock (this)
				{
					return writer.NewLine;
				}
			}
			set
			{
				lock (this)
				{
					writer.NewLine = value;
				}
			}
		}

		public SynchronizedWriter(TextWriter writer)
			: this(writer, false)
		{
		}

		public SynchronizedWriter(TextWriter writer, bool neverClose)
		{
			this.writer = writer;
			this.neverClose = neverClose;
		}

		public override void Close()
		{
			if (neverClose)
			{
				return;
			}
			lock (this)
			{
				writer.Close();
			}
		}

		public override void Flush()
		{
			lock (this)
			{
				writer.Flush();
			}
		}

		public override void Write(bool value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(char value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(char[] value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(decimal value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(int value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(long value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(object value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(float value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(string value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(uint value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(ulong value)
		{
			lock (this)
			{
				writer.Write(value);
			}
		}

		public override void Write(string format, object value)
		{
			lock (this)
			{
				writer.Write(format, value);
			}
		}

		public override void Write(string format, object[] value)
		{
			lock (this)
			{
				writer.Write(format, value);
			}
		}

		public override void Write(char[] buffer, int index, int count)
		{
			lock (this)
			{
				writer.Write(buffer, index, count);
			}
		}

		public override void Write(string format, object arg0, object arg1)
		{
			lock (this)
			{
				writer.Write(format, arg0, arg1);
			}
		}

		public override void Write(string format, object arg0, object arg1, object arg2)
		{
			lock (this)
			{
				writer.Write(format, arg0, arg1, arg2);
			}
		}

		public override void WriteLine()
		{
			lock (this)
			{
				writer.WriteLine();
			}
		}

		public override void WriteLine(bool value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(char value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(char[] value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(decimal value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(double value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(int value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(long value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(object value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(float value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(string value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(uint value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(ulong value)
		{
			lock (this)
			{
				writer.WriteLine(value);
			}
		}

		public override void WriteLine(string format, object value)
		{
			lock (this)
			{
				writer.WriteLine(format, value);
			}
		}

		public override void WriteLine(string format, object[] value)
		{
			lock (this)
			{
				writer.WriteLine(format, value);
			}
		}

		public override void WriteLine(char[] buffer, int index, int count)
		{
			lock (this)
			{
				writer.WriteLine(buffer, index, count);
			}
		}

		public override void WriteLine(string format, object arg0, object arg1)
		{
			lock (this)
			{
				writer.WriteLine(format, arg0, arg1);
			}
		}

		public override void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			lock (this)
			{
				writer.WriteLine(format, arg0, arg1, arg2);
			}
		}
	}
}
