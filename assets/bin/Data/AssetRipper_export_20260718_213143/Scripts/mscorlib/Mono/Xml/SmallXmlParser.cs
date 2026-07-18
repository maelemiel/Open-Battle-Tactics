using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace Mono.Xml
{
	internal class SmallXmlParser
	{
		public interface IContentHandler
		{
			void OnStartParsing(SmallXmlParser parser);

			void OnEndParsing(SmallXmlParser parser);

			void OnStartElement(string name, IAttrList attrs);

			void OnEndElement(string name);

			void OnProcessingInstruction(string name, string text);

			void OnChars(string text);

			void OnIgnorableWhitespace(string text);
		}

		public interface IAttrList
		{
			int Length { get; }

			bool IsEmpty { get; }

			string[] Names { get; }

			string[] Values { get; }

			string GetName(int i);

			string GetValue(int i);

			string GetValue(string name);
		}

		private class AttrListImpl : IAttrList
		{
			private ArrayList attrNames = new ArrayList();

			private ArrayList attrValues = new ArrayList();

			public int Length
			{
				get
				{
					return attrNames.Count;
				}
			}

			public bool IsEmpty
			{
				get
				{
					return attrNames.Count == 0;
				}
			}

			public string[] Names
			{
				get
				{
					return (string[])attrNames.ToArray(typeof(string));
				}
			}

			public string[] Values
			{
				get
				{
					return (string[])attrValues.ToArray(typeof(string));
				}
			}

			public string GetName(int i)
			{
				return (string)attrNames[i];
			}

			public string GetValue(int i)
			{
				return (string)attrValues[i];
			}

			public string GetValue(string name)
			{
				for (int i = 0; i < attrNames.Count; i++)
				{
					if ((string)attrNames[i] == name)
					{
						return (string)attrValues[i];
					}
				}
				return null;
			}

			internal void Clear()
			{
				attrNames.Clear();
				attrValues.Clear();
			}

			internal void Add(string name, string value)
			{
				attrNames.Add(name);
				attrValues.Add(value);
			}
		}

		private IContentHandler handler;

		private TextReader reader;

		private Stack elementNames = new Stack();

		private Stack xmlSpaces = new Stack();

		private string xmlSpace;

		private StringBuilder buffer = new StringBuilder(200);

		private char[] nameBuffer = new char[30];

		private bool isWhitespace;

		private AttrListImpl attributes = new AttrListImpl();

		private int line = 1;

		private int column;

		private bool resetColumn;

		private Exception Error(string msg)
		{
			return new SmallXmlParserException(msg, line, column);
		}

		private Exception UnexpectedEndError()
		{
			string[] array = new string[elementNames.Count];
			elementNames.CopyTo(array, 0);
			return Error(string.Format("Unexpected end of stream. Element stack content is {0}", string.Join(",", array)));
		}

		private bool IsNameChar(char c, bool start)
		{
			switch (c)
			{
			case ':':
			case '_':
				return true;
			case '-':
			case '.':
				return !start;
			default:
				if (c > 'Ā')
				{
					char c2 = c;
					if (c2 == 'ۥ' || c2 == 'ۦ' || c2 == 'ՙ')
					{
						return true;
					}
					if ('ʻ' <= c && c <= 'ˁ')
					{
						return true;
					}
				}
				switch (char.GetUnicodeCategory(c))
				{
				case UnicodeCategory.UppercaseLetter:
				case UnicodeCategory.LowercaseLetter:
				case UnicodeCategory.TitlecaseLetter:
				case UnicodeCategory.OtherLetter:
				case UnicodeCategory.LetterNumber:
					return true;
				case UnicodeCategory.ModifierLetter:
				case UnicodeCategory.NonSpacingMark:
				case UnicodeCategory.SpacingCombiningMark:
				case UnicodeCategory.EnclosingMark:
				case UnicodeCategory.DecimalDigitNumber:
					return !start;
				default:
					return false;
				}
			}
		}

		private bool IsWhitespace(int c)
		{
			switch (c)
			{
			case 9:
			case 10:
			case 13:
			case 32:
				return true;
			default:
				return false;
			}
		}

		public void SkipWhitespaces()
		{
			SkipWhitespaces(false);
		}

		private void HandleWhitespaces()
		{
			while (IsWhitespace(Peek()))
			{
				buffer.Append((char)Read());
			}
			if (Peek() != 60 && Peek() >= 0)
			{
				isWhitespace = false;
			}
		}

		public void SkipWhitespaces(bool expected)
		{
			while (true)
			{
				switch (Peek())
				{
				case 9:
				case 10:
				case 13:
				case 32:
					Read();
					if (expected)
					{
						expected = false;
					}
					break;
				default:
					if (expected)
					{
						throw Error("Whitespace is expected.");
					}
					return;
				}
			}
		}

		private int Peek()
		{
			return reader.Peek();
		}

		private int Read()
		{
			int num = reader.Read();
			if (num == 10)
			{
				resetColumn = true;
			}
			if (resetColumn)
			{
				line++;
				resetColumn = false;
				column = 1;
			}
			else
			{
				column++;
			}
			return num;
		}

		public void Expect(int c)
		{
			int num = Read();
			if (num < 0)
			{
				throw UnexpectedEndError();
			}
			if (num != c)
			{
				throw Error(string.Format("Expected '{0}' but got {1}", (char)c, (char)num));
			}
		}

		private string ReadUntil(char until, bool handleReferences)
		{
			while (true)
			{
				if (Peek() < 0)
				{
					throw UnexpectedEndError();
				}
				char c = (char)Read();
				if (c == until)
				{
					break;
				}
				if (handleReferences && c == '&')
				{
					ReadReference();
				}
				else
				{
					buffer.Append(c);
				}
			}
			string result = buffer.ToString();
			buffer.Length = 0;
			return result;
		}

		public string ReadName()
		{
			int num = 0;
			if (Peek() < 0 || !IsNameChar((char)Peek(), true))
			{
				throw Error("XML name start character is expected.");
			}
			for (int num2 = Peek(); num2 >= 0; num2 = Peek())
			{
				char c = (char)num2;
				if (!IsNameChar(c, false))
				{
					break;
				}
				if (num == nameBuffer.Length)
				{
					char[] destinationArray = new char[num * 2];
					Array.Copy(nameBuffer, destinationArray, num);
					nameBuffer = destinationArray;
				}
				nameBuffer[num++] = c;
				Read();
			}
			if (num == 0)
			{
				throw Error("Valid XML name is expected.");
			}
			return new string(nameBuffer, 0, num);
		}

		public void Parse(TextReader input, IContentHandler handler)
		{
			reader = input;
			this.handler = handler;
			handler.OnStartParsing(this);
			while (Peek() >= 0)
			{
				ReadContent();
			}
			HandleBufferedContent();
			if (elementNames.Count > 0)
			{
				throw Error(string.Format("Insufficient close tag: {0}", elementNames.Peek()));
			}
			handler.OnEndParsing(this);
			Cleanup();
		}

		private void Cleanup()
		{
			line = 1;
			column = 0;
			handler = null;
			reader = null;
			elementNames.Clear();
			xmlSpaces.Clear();
			attributes.Clear();
			buffer.Length = 0;
			xmlSpace = null;
			isWhitespace = false;
		}

		public void ReadContent()
		{
			if (IsWhitespace(Peek()))
			{
				if (buffer.Length == 0)
				{
					isWhitespace = true;
				}
				HandleWhitespaces();
			}
			if (Peek() == 60)
			{
				Read();
				switch (Peek())
				{
				case 33:
					Read();
					if (Peek() == 91)
					{
						Read();
						if (ReadName() != "CDATA")
						{
							throw Error("Invalid declaration markup");
						}
						Expect(91);
						ReadCDATASection();
						break;
					}
					if (Peek() == 45)
					{
						ReadComment();
						break;
					}
					if (ReadName() != "DOCTYPE")
					{
						throw Error("Invalid declaration markup.");
					}
					throw Error("This parser does not support document type.");
				case 63:
				{
					HandleBufferedContent();
					Read();
					string text = ReadName();
					SkipWhitespaces();
					string text3 = string.Empty;
					if (Peek() != 63)
					{
						while (true)
						{
							text3 += ReadUntil('?', false);
							if (Peek() == 62)
							{
								break;
							}
							text3 += "?";
						}
					}
					handler.OnProcessingInstruction(text, text3);
					Expect(62);
					break;
				}
				case 47:
				{
					HandleBufferedContent();
					if (elementNames.Count == 0)
					{
						throw UnexpectedEndError();
					}
					Read();
					string text = ReadName();
					SkipWhitespaces();
					string text2 = (string)elementNames.Pop();
					xmlSpaces.Pop();
					if (xmlSpaces.Count > 0)
					{
						xmlSpace = (string)xmlSpaces.Peek();
					}
					else
					{
						xmlSpace = null;
					}
					if (text != text2)
					{
						throw Error(string.Format("End tag mismatch: expected {0} but found {1}", text2, text));
					}
					handler.OnEndElement(text);
					Expect(62);
					break;
				}
				default:
				{
					HandleBufferedContent();
					string text = ReadName();
					while (Peek() != 62 && Peek() != 47)
					{
						ReadAttribute(attributes);
					}
					handler.OnStartElement(text, attributes);
					attributes.Clear();
					SkipWhitespaces();
					if (Peek() == 47)
					{
						Read();
						handler.OnEndElement(text);
					}
					else
					{
						elementNames.Push(text);
						xmlSpaces.Push(xmlSpace);
					}
					Expect(62);
					break;
				}
				}
			}
			else
			{
				ReadCharacters();
			}
		}

		private void HandleBufferedContent()
		{
			if (buffer.Length != 0)
			{
				if (isWhitespace)
				{
					handler.OnIgnorableWhitespace(buffer.ToString());
				}
				else
				{
					handler.OnChars(buffer.ToString());
				}
				buffer.Length = 0;
				isWhitespace = false;
			}
		}

		private void ReadCharacters()
		{
			isWhitespace = false;
			while (true)
			{
				switch (Peek())
				{
				case -1:
				case 60:
					return;
				case 38:
					Read();
					ReadReference();
					break;
				default:
					buffer.Append((char)Read());
					break;
				}
			}
		}

		private void ReadReference()
		{
			if (Peek() == 35)
			{
				Read();
				ReadCharacterReference();
				return;
			}
			string text = ReadName();
			Expect(59);
			switch (text)
			{
			case "amp":
				buffer.Append('&');
				break;
			case "quot":
				buffer.Append('"');
				break;
			case "apos":
				buffer.Append('\'');
				break;
			case "lt":
				buffer.Append('<');
				break;
			case "gt":
				buffer.Append('>');
				break;
			default:
				throw Error("General non-predefined entity reference is not supported in this parser.");
			}
		}

		private int ReadCharacterReference()
		{
			int num = 0;
			if (Peek() == 120)
			{
				Read();
				for (int num2 = Peek(); num2 >= 0; num2 = Peek())
				{
					if (48 <= num2 && num2 <= 57)
					{
						num <<= 4 + num2 - 48;
					}
					else if (65 <= num2 && num2 <= 70)
					{
						num <<= 4 + num2 - 65 + 10;
					}
					else
					{
						if (97 > num2 || num2 > 102)
						{
							break;
						}
						num <<= 4 + num2 - 97 + 10;
					}
					Read();
				}
			}
			else
			{
				int num3 = Peek();
				while (num3 >= 0 && 48 <= num3 && num3 <= 57)
				{
					num <<= 4 + num3 - 48;
					Read();
					num3 = Peek();
				}
			}
			return num;
		}

		private void ReadAttribute(AttrListImpl a)
		{
			SkipWhitespaces(true);
			if (Peek() != 47 && Peek() != 62)
			{
				string text = ReadName();
				SkipWhitespaces();
				Expect(61);
				SkipWhitespaces();
				string value;
				switch (Read())
				{
				case 39:
					value = ReadUntil('\'', true);
					break;
				case 34:
					value = ReadUntil('"', true);
					break;
				default:
					throw Error("Invalid attribute value markup.");
				}
				if (text == "xml:space")
				{
					xmlSpace = value;
				}
				a.Add(text, value);
			}
		}

		private void ReadCDATASection()
		{
			int num = 0;
			while (Peek() >= 0)
			{
				char c = (char)Read();
				switch (c)
				{
				case ']':
					num++;
					continue;
				case '>':
					if (num > 1)
					{
						for (int num2 = num; num2 > 2; num2--)
						{
							buffer.Append(']');
						}
						return;
					}
					break;
				}
				for (int i = 0; i < num; i++)
				{
					buffer.Append(']');
				}
				num = 0;
				buffer.Append(c);
			}
			throw UnexpectedEndError();
		}

		private void ReadComment()
		{
			Expect(45);
			Expect(45);
			while (Read() != 45 || Read() != 45)
			{
			}
			if (Read() != 62)
			{
				throw Error("'--' is not allowed inside comment markup.");
			}
		}
	}
}
