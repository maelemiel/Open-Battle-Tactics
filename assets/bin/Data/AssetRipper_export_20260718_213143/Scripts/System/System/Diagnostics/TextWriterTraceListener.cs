using System.IO;

namespace System.Diagnostics
{
	public class TextWriterTraceListener : TraceListener
	{
		private TextWriter writer;

		public TextWriter Writer
		{
			get
			{
				return writer;
			}
			set
			{
				writer = value;
			}
		}

		public TextWriterTraceListener()
			: base("TextWriter")
		{
		}

		public TextWriterTraceListener(Stream stream)
			: this(stream, string.Empty)
		{
		}

		public TextWriterTraceListener(string fileName)
			: this(fileName, string.Empty)
		{
		}

		public TextWriterTraceListener(TextWriter writer)
			: this(writer, string.Empty)
		{
		}

		public TextWriterTraceListener(Stream stream, string name)
			: base((name == null) ? string.Empty : name)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			writer = new StreamWriter(stream);
		}

		public TextWriterTraceListener(string fileName, string name)
			: base((name == null) ? string.Empty : name)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			writer = new StreamWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
		}

		public TextWriterTraceListener(TextWriter writer, string name)
			: base((name == null) ? string.Empty : name)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
		}

		public override void Close()
		{
			if (writer != null)
			{
				writer.Flush();
				writer.Close();
				writer = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Close();
			}
			base.Dispose(disposing);
		}

		public override void Flush()
		{
			if (writer != null)
			{
				writer.Flush();
			}
		}

		public override void Write(string message)
		{
			if (writer != null)
			{
				if (base.NeedIndent)
				{
					WriteIndent();
				}
				writer.Write(message);
			}
		}

		public override void WriteLine(string message)
		{
			if (writer != null)
			{
				if (base.NeedIndent)
				{
					WriteIndent();
				}
				writer.WriteLine(message);
				base.NeedIndent = true;
			}
		}
	}
}
