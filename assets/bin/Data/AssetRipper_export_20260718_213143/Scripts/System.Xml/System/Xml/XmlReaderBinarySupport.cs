using System.Text;

namespace System.Xml
{
	internal class XmlReaderBinarySupport
	{
		public enum CommandState
		{
			None = 0,
			ReadElementContentAsBase64 = 1,
			ReadContentAsBase64 = 2,
			ReadElementContentAsBinHex = 3,
			ReadContentAsBinHex = 4
		}

		public delegate int CharGetter(char[] buffer, int offset, int length);

		private XmlReader reader;

		private CharGetter getter;

		private byte[] base64Cache = new byte[3];

		private int base64CacheStartsAt;

		private CommandState state;

		private StringBuilder textCache;

		private bool hasCache;

		private bool dontReset;

		public CharGetter Getter
		{
			get
			{
				return getter;
			}
			set
			{
				getter = value;
			}
		}

		public XmlReaderBinarySupport(XmlReader reader)
		{
			this.reader = reader;
			Reset();
		}

		public void Reset()
		{
			if (dontReset)
			{
				return;
			}
			dontReset = true;
			if (hasCache)
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
				{
					reader.Read();
				}
				switch (state)
				{
				case CommandState.ReadElementContentAsBase64:
				case CommandState.ReadElementContentAsBinHex:
					reader.Read();
					break;
				}
			}
			base64CacheStartsAt = -1;
			state = CommandState.None;
			hasCache = false;
			dontReset = false;
		}

		private InvalidOperationException StateError(CommandState action)
		{
			return new InvalidOperationException(string.Format("Invalid attempt to read binary content by {0}, while once binary reading was started by {1}", action, state));
		}

		private void CheckState(bool element, CommandState action)
		{
			if (state == CommandState.None)
			{
				if (textCache == null)
				{
					textCache = new StringBuilder();
				}
				else
				{
					textCache.Length = 0;
				}
				if (action == CommandState.None || reader.ReadState != ReadState.Interactive)
				{
					return;
				}
				switch (reader.NodeType)
				{
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					if (!element)
					{
						state = action;
						return;
					}
					break;
				case XmlNodeType.Element:
					if (element)
					{
						if (!reader.IsEmptyElement)
						{
							reader.Read();
						}
						state = action;
						return;
					}
					break;
				}
				throw new XmlException((!element) ? "Reader is not positioned on a text node." : "Reader is not positioned on an element.");
			}
			if (state == action)
			{
				return;
			}
			throw StateError(action);
		}

		public int ReadElementContentAsBase64(byte[] buffer, int offset, int length)
		{
			CheckState(true, CommandState.ReadElementContentAsBase64);
			return ReadBase64(buffer, offset, length);
		}

		public int ReadContentAsBase64(byte[] buffer, int offset, int length)
		{
			CheckState(false, CommandState.ReadContentAsBase64);
			return ReadBase64(buffer, offset, length);
		}

		public int ReadElementContentAsBinHex(byte[] buffer, int offset, int length)
		{
			CheckState(true, CommandState.ReadElementContentAsBinHex);
			return ReadBinHex(buffer, offset, length);
		}

		public int ReadContentAsBinHex(byte[] buffer, int offset, int length)
		{
			CheckState(false, CommandState.ReadContentAsBinHex);
			return ReadBinHex(buffer, offset, length);
		}

		public int ReadBase64(byte[] buffer, int offset, int length)
		{
			if (offset < 0)
			{
				throw CreateArgumentOutOfRangeException("offset", offset, "Offset must be non-negative integer.");
			}
			if (length < 0)
			{
				throw CreateArgumentOutOfRangeException("length", length, "Length must be non-negative integer.");
			}
			if (buffer.Length < offset + length)
			{
				throw new ArgumentOutOfRangeException("buffer length is smaller than the sum of offset and length.");
			}
			if (reader.IsEmptyElement)
			{
				return 0;
			}
			if (length == 0)
			{
				return 0;
			}
			int num = offset;
			int num2 = offset + length;
			if (base64CacheStartsAt >= 0)
			{
				for (int i = base64CacheStartsAt; i < 3; i++)
				{
					buffer[num++] = base64Cache[base64CacheStartsAt++];
					if (num == num2)
					{
						return num2 - offset;
					}
				}
			}
			for (int j = 0; j < 3; j++)
			{
				base64Cache[j] = 0;
			}
			base64CacheStartsAt = -1;
			int num3 = (int)Math.Ceiling(1.3333333333333333 * (double)length);
			int num4 = num3 % 4;
			if (num4 > 0)
			{
				num3 += 4 - num4;
			}
			char[] array = new char[num3];
			int num5 = ((getter == null) ? ReadValueChunk(array, 0, num3) : getter(array, 0, num3));
			byte b = 0;
			byte b2 = 0;
			int num6 = 0;
			while (num6 < num5 - 3 && (num6 = SkipIgnorableBase64Chars(array, num5, num6)) != num5)
			{
				b = (byte)(GetBase64Byte(array[num6]) << 2);
				if (num < num2)
				{
					buffer[num] = b;
				}
				else
				{
					if (base64CacheStartsAt < 0)
					{
						base64CacheStartsAt = 0;
					}
					base64Cache[0] = b;
				}
				if (++num6 == num5 || (num6 = SkipIgnorableBase64Chars(array, num5, num6)) == num5)
				{
					break;
				}
				b = GetBase64Byte(array[num6]);
				b2 = (byte)(b >> 4);
				if (num < num2)
				{
					buffer[num] += b2;
					num++;
				}
				else
				{
					base64Cache[0] += b2;
				}
				b2 = (byte)((b & 0xF) << 4);
				if (num < num2)
				{
					buffer[num] = b2;
				}
				else
				{
					if (base64CacheStartsAt < 0)
					{
						base64CacheStartsAt = 1;
					}
					base64Cache[1] = b2;
				}
				if (++num6 == num5 || (num6 = SkipIgnorableBase64Chars(array, num5, num6)) == num5)
				{
					break;
				}
				b = GetBase64Byte(array[num6]);
				b2 = (byte)(b >> 2);
				if (num < num2)
				{
					buffer[num] += b2;
					num++;
				}
				else
				{
					base64Cache[1] += b2;
				}
				b2 = (byte)((b & 3) << 6);
				if (num < num2)
				{
					buffer[num] = b2;
				}
				else
				{
					if (base64CacheStartsAt < 0)
					{
						base64CacheStartsAt = 2;
					}
					base64Cache[2] = b2;
				}
				if (++num6 == num5 || (num6 = SkipIgnorableBase64Chars(array, num5, num6)) == num5)
				{
					break;
				}
				b2 = GetBase64Byte(array[num6]);
				if (num < num2)
				{
					buffer[num] += b2;
					num++;
				}
				else
				{
					base64Cache[2] += b2;
				}
				num6++;
			}
			int num7 = Math.Min(num2 - offset, num - offset);
			if (num7 < length && num5 > 0)
			{
				return num7 + ReadBase64(buffer, offset + num7, length - num7);
			}
			return num7;
		}

		private byte GetBase64Byte(char ch)
		{
			switch (ch)
			{
			case '+':
				return 62;
			case '/':
				return 63;
			default:
				if (ch >= 'A' && ch <= 'Z')
				{
					return (byte)(ch - 65);
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return (byte)(ch - 97 + 26);
				}
				if (ch >= '0' && ch <= '9')
				{
					return (byte)(ch - 48 + 52);
				}
				throw new XmlException("Invalid Base64 character was found.");
			}
		}

		private int SkipIgnorableBase64Chars(char[] chars, int charsLength, int i)
		{
			while ((chars[i] == '=' || XmlChar.IsWhitespace(chars[i])) && charsLength != ++i)
			{
			}
			return i;
		}

		private static Exception CreateArgumentOutOfRangeException(string name, object value, string message)
		{
			return new ArgumentOutOfRangeException(message);
		}

		public int ReadBinHex(byte[] buffer, int offset, int length)
		{
			if (offset < 0)
			{
				throw CreateArgumentOutOfRangeException("offset", offset, "Offset must be non-negative integer.");
			}
			if (length < 0)
			{
				throw CreateArgumentOutOfRangeException("length", length, "Length must be non-negative integer.");
			}
			if (buffer.Length < offset + length)
			{
				throw new ArgumentOutOfRangeException("buffer length is smaller than the sum of offset and length.");
			}
			if (length == 0)
			{
				return 0;
			}
			char[] array = new char[length * 2];
			int charLength = ((getter == null) ? ReadValueChunk(array, 0, length * 2) : getter(array, 0, length * 2));
			return XmlConvert.FromBinHexString(array, offset, charLength, buffer);
		}

		public int ReadValueChunk(char[] buffer, int offset, int length)
		{
			CommandState commandState = state;
			if (state == CommandState.None)
			{
				CheckState(false, CommandState.None);
			}
			if (offset < 0)
			{
				throw CreateArgumentOutOfRangeException("offset", offset, "Offset must be non-negative integer.");
			}
			if (length < 0)
			{
				throw CreateArgumentOutOfRangeException("length", length, "Length must be non-negative integer.");
			}
			if (buffer.Length < offset + length)
			{
				throw new ArgumentOutOfRangeException("buffer length is smaller than the sum of offset and length.");
			}
			if (length == 0)
			{
				return 0;
			}
			if (!hasCache && reader.IsEmptyElement)
			{
				return 0;
			}
			bool flag = true;
			while (flag && textCache.Length < length)
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
				{
					if (hasCache)
					{
						XmlNodeType nodeType2 = reader.NodeType;
						if (nodeType2 == XmlNodeType.Text || nodeType2 == XmlNodeType.CDATA || nodeType2 == XmlNodeType.Whitespace || nodeType2 == XmlNodeType.SignificantWhitespace)
						{
							Read();
						}
						else
						{
							flag = false;
						}
					}
					textCache.Append(reader.Value);
					hasCache = true;
				}
				else
				{
					flag = false;
				}
			}
			state = commandState;
			int num = textCache.Length;
			if (num > length)
			{
				num = length;
			}
			string text = textCache.ToString(0, num);
			textCache.Remove(0, text.Length);
			text.CopyTo(0, buffer, offset, text.Length);
			if (num < length && flag)
			{
				return num + ReadValueChunk(buffer, offset + num, length - num);
			}
			return num;
		}

		private bool Read()
		{
			dontReset = true;
			bool result = reader.Read();
			dontReset = false;
			return result;
		}
	}
}
