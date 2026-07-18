using System.Collections;
using System.Globalization;
using System.IO;
using Mono.Xml;

namespace System.Xml
{
	internal class XmlParserInput
	{
		private class XmlParserInputSource
		{
			public readonly string BaseURI;

			private readonly TextReader reader;

			public int state;

			public bool isPE;

			private int line;

			private int column;

			public int LineNumber
			{
				get
				{
					return line;
				}
			}

			public int LinePosition
			{
				get
				{
					return column;
				}
			}

			public XmlParserInputSource(TextReader reader, string baseUri, bool pe, int line, int column)
			{
				BaseURI = baseUri;
				this.reader = reader;
				isPE = pe;
				this.line = line;
				this.column = column;
			}

			public void Close()
			{
				reader.Close();
			}

			public int Read()
			{
				if (state == 2)
				{
					return -1;
				}
				if (isPE && state == 0)
				{
					state = 1;
					return 32;
				}
				int num = reader.Read();
				if (num == 10)
				{
					line++;
					column = 1;
				}
				else if (num >= 0)
				{
					column++;
				}
				if (num < 0 && state == 1)
				{
					state = 2;
					return 32;
				}
				return num;
			}
		}

		private Stack sourceStack = new Stack();

		private XmlParserInputSource source;

		private bool has_peek;

		private int peek_char;

		private bool allowTextDecl = true;

		public string BaseURI
		{
			get
			{
				return source.BaseURI;
			}
		}

		public bool HasPEBuffer
		{
			get
			{
				return sourceStack.Count > 0;
			}
		}

		public int LineNumber
		{
			get
			{
				return source.LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return source.LinePosition;
			}
		}

		public bool AllowTextDecl
		{
			get
			{
				return allowTextDecl;
			}
			set
			{
				allowTextDecl = value;
			}
		}

		public XmlParserInput(TextReader reader, string baseURI)
			: this(reader, baseURI, 1, 0)
		{
		}

		public XmlParserInput(TextReader reader, string baseURI, int line, int column)
		{
			source = new XmlParserInputSource(reader, baseURI, false, line, column);
		}

		public void Close()
		{
			while (sourceStack.Count > 0)
			{
				((XmlParserInputSource)sourceStack.Pop()).Close();
			}
			source.Close();
		}

		public void Expect(int expected)
		{
			int num = ReadChar();
			if (num != expected)
			{
				throw ReaderError(string.Format(CultureInfo.InvariantCulture, "expected '{0}' ({1:X}) but found '{2}' ({3:X})", (char)expected, expected, (char)num, num));
			}
		}

		public void Expect(string expected)
		{
			int length = expected.Length;
			for (int i = 0; i < length; i++)
			{
				Expect(expected[i]);
			}
		}

		public void PushPEBuffer(DTDParameterEntityDeclaration pe)
		{
			sourceStack.Push(source);
			source = new XmlParserInputSource(new StringReader(pe.ReplacementText), pe.ActualUri, true, 1, 0);
		}

		private int ReadSourceChar()
		{
			int num = source.Read();
			while (num < 0 && sourceStack.Count > 0)
			{
				source = sourceStack.Pop() as XmlParserInputSource;
				num = source.Read();
			}
			return num;
		}

		public int PeekChar()
		{
			if (has_peek)
			{
				return peek_char;
			}
			peek_char = ReadSourceChar();
			if (peek_char >= 55296 && peek_char <= 56319)
			{
				peek_char = 65536 + (peek_char - 55296 << 10);
				int num = ReadSourceChar();
				if (num >= 56320 && num <= 57343)
				{
					peek_char += num - 56320;
				}
			}
			has_peek = true;
			return peek_char;
		}

		public int ReadChar()
		{
			int result = PeekChar();
			has_peek = false;
			return result;
		}

		private XmlException ReaderError(string message)
		{
			return new XmlException(message, null, LineNumber, LinePosition);
		}
	}
}
