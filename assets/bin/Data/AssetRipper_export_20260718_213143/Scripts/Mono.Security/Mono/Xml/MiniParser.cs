using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Mono.Xml
{
	[CLSCompliant(false)]
	public class MiniParser
	{
		public interface IReader
		{
			int Read();
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

			void ChangeValue(string name, string newValue);
		}

		public interface IMutableAttrList : IAttrList
		{
			void Clear();

			void Add(string name, string value);

			void CopyFrom(IAttrList attrs);

			void Remove(int i);

			void Remove(string name);
		}

		public interface IHandler
		{
			void OnStartParsing(MiniParser parser);

			void OnStartElement(string name, IAttrList attrs);

			void OnEndElement(string name);

			void OnChars(string ch);

			void OnEndParsing(MiniParser parser);
		}

		public class HandlerAdapter : IHandler
		{
			public void OnStartParsing(MiniParser parser)
			{
			}

			public void OnStartElement(string name, IAttrList attrs)
			{
			}

			public void OnEndElement(string name)
			{
			}

			public void OnChars(string ch)
			{
			}

			public void OnEndParsing(MiniParser parser)
			{
			}
		}

		private enum CharKind : byte
		{
			LEFT_BR = 0,
			RIGHT_BR = 1,
			SLASH = 2,
			PI_MARK = 3,
			EQ = 4,
			AMP = 5,
			SQUOTE = 6,
			DQUOTE = 7,
			BANG = 8,
			LEFT_SQBR = 9,
			SPACE = 10,
			RIGHT_SQBR = 11,
			TAB = 12,
			CR = 13,
			EOL = 14,
			CHARS = 15,
			UNKNOWN = 31
		}

		private enum ActionCode : byte
		{
			START_ELEM = 0,
			END_ELEM = 1,
			END_NAME = 2,
			SET_ATTR_NAME = 3,
			SET_ATTR_VAL = 4,
			SEND_CHARS = 5,
			START_CDATA = 6,
			END_CDATA = 7,
			ERROR = 8,
			STATE_CHANGE = 9,
			FLUSH_CHARS_STATE_CHANGE = 10,
			ACC_CHARS_STATE_CHANGE = 11,
			ACC_CDATA = 12,
			PROC_CHAR_REF = 13,
			UNKNOWN = 15
		}

		public class AttrListImpl : IAttrList, IMutableAttrList
		{
			protected ArrayList names;

			protected ArrayList values;

			public int Length
			{
				get
				{
					return names.Count;
				}
			}

			public bool IsEmpty
			{
				get
				{
					return Length != 0;
				}
			}

			public string[] Names
			{
				get
				{
					return names.ToArray(typeof(string)) as string[];
				}
			}

			public string[] Values
			{
				get
				{
					return values.ToArray(typeof(string)) as string[];
				}
			}

			public AttrListImpl()
				: this(0)
			{
			}

			public AttrListImpl(int initialCapacity)
			{
				if (initialCapacity <= 0)
				{
					names = new ArrayList();
					values = new ArrayList();
				}
				else
				{
					names = new ArrayList(initialCapacity);
					values = new ArrayList(initialCapacity);
				}
			}

			public AttrListImpl(IAttrList attrs)
				: this((attrs != null) ? attrs.Length : 0)
			{
				if (attrs != null)
				{
					CopyFrom(attrs);
				}
			}

			public string GetName(int i)
			{
				string result = null;
				if (i >= 0 && i < Length)
				{
					result = names[i] as string;
				}
				return result;
			}

			public string GetValue(int i)
			{
				string result = null;
				if (i >= 0 && i < Length)
				{
					result = values[i] as string;
				}
				return result;
			}

			public string GetValue(string name)
			{
				return GetValue(names.IndexOf(name));
			}

			public void ChangeValue(string name, string newValue)
			{
				int num = names.IndexOf(name);
				if (num >= 0 && num < Length)
				{
					values[num] = newValue;
				}
			}

			public void Clear()
			{
				names.Clear();
				values.Clear();
			}

			public void Add(string name, string value)
			{
				names.Add(name);
				values.Add(value);
			}

			public void Remove(int i)
			{
				if (i >= 0)
				{
					names.RemoveAt(i);
					values.RemoveAt(i);
				}
			}

			public void Remove(string name)
			{
				Remove(names.IndexOf(name));
			}

			public void CopyFrom(IAttrList attrs)
			{
				if (attrs != null && this == attrs)
				{
					Clear();
					int length = attrs.Length;
					for (int i = 0; i < length; i++)
					{
						Add(attrs.GetName(i), attrs.GetValue(i));
					}
				}
			}
		}

		public class XMLError : Exception
		{
			protected string descr;

			protected int line;

			protected int column;

			public int Line
			{
				get
				{
					return line;
				}
			}

			public int Column
			{
				get
				{
					return column;
				}
			}

			public XMLError()
				: this("Unknown")
			{
			}

			public XMLError(string descr)
				: this(descr, -1, -1)
			{
			}

			public XMLError(string descr, int line, int column)
				: base(descr)
			{
				this.descr = descr;
				this.line = line;
				this.column = column;
			}

			public override string ToString()
			{
				return string.Format("{0} @ (line = {1}, col = {2})", descr, line, column);
			}
		}

		private static readonly int INPUT_RANGE = 13;

		private static readonly ushort[] tbl = new ushort[262]
		{
			2305, 43264, 63616, 10368, 6272, 14464, 18560, 22656, 26752, 34944,
			39040, 47232, 30848, 2177, 10498, 6277, 14595, 18561, 22657, 26753,
			35088, 39041, 43137, 47233, 30849, 64004, 4352, 43266, 64258, 2177,
			10369, 14465, 18561, 22657, 26753, 34945, 39041, 47233, 30849, 14597,
			2307, 10499, 6403, 18691, 22787, 26883, 35075, 39171, 43267, 47363,
			30979, 63747, 64260, 8710, 4615, 41480, 2177, 14465, 18561, 22657,
			26753, 34945, 39041, 47233, 30849, 6400, 2307, 10499, 14595, 18691,
			22787, 26883, 35075, 39171, 43267, 47363, 30979, 63747, 6400, 2177,
			10369, 14465, 18561, 22657, 26753, 34945, 39041, 43137, 47233, 30849,
			63617, 2561, 23818, 11274, 7178, 15370, 19466, 27658, 35850, 39946,
			43783, 48138, 31754, 64522, 64265, 8198, 4103, 43272, 2177, 14465,
			18561, 22657, 26753, 34945, 39041, 47233, 30849, 64265, 17163, 43276,
			2178, 10370, 6274, 14466, 22658, 26754, 34946, 39042, 47234, 30850,
			2317, 23818, 11274, 7178, 15370, 19466, 27658, 35850, 39946, 44042,
			48138, 31754, 64522, 26894, 30991, 43275, 2180, 10372, 6276, 14468,
			18564, 22660, 34948, 39044, 47236, 63620, 17163, 43276, 2178, 10370,
			6274, 14466, 22658, 26754, 34946, 39042, 47234, 30850, 63618, 9474,
			35088, 2182, 6278, 14470, 18566, 22662, 26758, 39046, 43142, 47238,
			30854, 63622, 25617, 23822, 2830, 11022, 6926, 15118, 19214, 35598,
			39694, 43790, 47886, 31502, 64270, 29713, 23823, 2831, 11023, 6927,
			15119, 19215, 27407, 35599, 39695, 43791, 47887, 64271, 38418, 6400,
			1555, 9747, 13843, 17939, 22035, 26131, 34323, 42515, 46611, 30227,
			62995, 8198, 4103, 43281, 64265, 2177, 14465, 18561, 22657, 26753,
			34945, 39041, 47233, 30849, 46858, 3090, 11282, 7186, 15378, 19474,
			23570, 27666, 35858, 39954, 44050, 31762, 64530, 3091, 11283, 7187,
			15379, 19475, 23571, 27667, 35859, 39955, 44051, 48147, 31763, 64531,
			65535, 65535
		};

		protected static string[] errors = new string[8] { "Expected element", "Invalid character in tag", "No '='", "Invalid character entity", "Invalid attr value", "Empty tag", "No end tag", "Bad entity ref" };

		protected int line;

		protected int col;

		protected int[] twoCharBuff;

		protected bool splitCData;

		public MiniParser()
		{
			twoCharBuff = new int[2];
			splitCData = false;
			Reset();
		}

		public void Reset()
		{
			line = 0;
			col = 0;
		}

		protected static bool StrEquals(string str, StringBuilder sb, int sbStart, int len)
		{
			if (len != str.Length)
			{
				return false;
			}
			for (int i = 0; i < len; i++)
			{
				if (str[i] != sb[sbStart + i])
				{
					return false;
				}
			}
			return true;
		}

		protected void FatalErr(string descr)
		{
			throw new XMLError(descr, line, col);
		}

		protected static int Xlat(int charCode, int state)
		{
			int num = state * INPUT_RANGE;
			int num2 = System.Math.Min(tbl.Length - num, INPUT_RANGE);
			while (--num2 >= 0)
			{
				ushort num3 = tbl[num];
				if (charCode == num3 >> 12)
				{
					return num3 & 0xFFF;
				}
				num++;
			}
			return 4095;
		}

		public void Parse(IReader reader, IHandler handler)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (handler == null)
			{
				handler = new HandlerAdapter();
			}
			AttrListImpl attrListImpl = new AttrListImpl();
			string text = null;
			Stack stack = new Stack();
			string text2 = null;
			line = 1;
			col = 0;
			int num = 0;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int num3 = 0;
			handler.OnStartParsing(this);
			while (true)
			{
				col++;
				num = reader.Read();
				if (num == -1)
				{
					break;
				}
				int num4 = "<>/?=&'\"![ ]\t\r\n".IndexOf((char)num) & 0xF;
				switch (num4)
				{
				case 13:
					continue;
				case 12:
					num4 = 10;
					break;
				}
				if (num4 == 14)
				{
					col = 0;
					line++;
					num4 = 10;
				}
				int num5 = Xlat(num4, num2);
				num2 = num5 & 0xFF;
				if (num == 10 && (num2 == 14 || num2 == 15))
				{
					continue;
				}
				num5 >>= 8;
				if (num2 >= 128)
				{
					if (num2 == 255)
					{
						FatalErr("State dispatch error.");
					}
					else
					{
						FatalErr(errors[num2 ^ 0x80]);
					}
				}
				switch (num5)
				{
				case 9:
					break;
				case 0:
					handler.OnStartElement(text2, attrListImpl);
					if (num != 47)
					{
						stack.Push(text2);
					}
					else
					{
						handler.OnEndElement(text2);
					}
					attrListImpl.Clear();
					break;
				case 1:
				{
					text2 = stringBuilder.ToString();
					stringBuilder = new StringBuilder();
					string text6 = null;
					if (stack.Count == 0 || text2 != (text6 = stack.Pop() as string))
					{
						if (text6 == null)
						{
							FatalErr("Tag stack underflow");
						}
						else
						{
							FatalErr(string.Format("Expected end tag '{0}' but found '{1}'", text2, text6));
						}
					}
					handler.OnEndElement(text2);
					break;
				}
				case 2:
					text2 = stringBuilder.ToString();
					stringBuilder = new StringBuilder();
					if (num != 47 && num != 62)
					{
						break;
					}
					goto case 0;
				case 3:
					text = stringBuilder.ToString();
					stringBuilder = new StringBuilder();
					break;
				case 4:
					if (text == null)
					{
						FatalErr("Internal error.");
					}
					attrListImpl.Add(text, stringBuilder.ToString());
					stringBuilder = new StringBuilder();
					text = null;
					break;
				case 5:
					handler.OnChars(stringBuilder.ToString());
					stringBuilder = new StringBuilder();
					break;
				case 6:
				{
					string text5 = "CDATA[";
					flag2 = false;
					flag3 = false;
					switch (num)
					{
					case 45:
						num = reader.Read();
						if (num != 45)
						{
							FatalErr("Invalid comment");
						}
						col++;
						flag2 = true;
						twoCharBuff[0] = -1;
						twoCharBuff[1] = -1;
						break;
					default:
						flag3 = true;
						num3 = 0;
						break;
					case 91:
					{
						for (int k = 0; k < text5.Length; k++)
						{
							if (reader.Read() != text5[k])
							{
								col += k + 1;
								break;
							}
						}
						col += text5.Length;
						flag = true;
						break;
					}
					}
					break;
				}
				case 7:
				{
					int num20 = 0;
					num = 93;
					while (true)
					{
						switch (num)
						{
						case 93:
							goto IL_039b;
						default:
						{
							for (int j = 0; j < num20; j++)
							{
								stringBuilder.Append(']');
							}
							stringBuilder.Append((char)num);
							num2 = 18;
							break;
						}
						case 62:
						{
							for (int i = 0; i < num20 - 2; i++)
							{
								stringBuilder.Append(']');
							}
							flag = false;
							break;
						}
						}
						break;
						IL_039b:
						num = reader.Read();
						num20++;
					}
					col += num20;
					break;
				}
				case 8:
					FatalErr(string.Format("Error {0}", num2));
					break;
				case 10:
					stringBuilder = new StringBuilder();
					if (num != 60)
					{
						goto case 11;
					}
					break;
				case 11:
					stringBuilder.Append((char)num);
					break;
				case 12:
					if (flag2)
					{
						if (num == 62 && twoCharBuff[0] == 45 && twoCharBuff[1] == 45)
						{
							flag2 = false;
							num2 = 0;
						}
						else
						{
							twoCharBuff[0] = twoCharBuff[1];
							twoCharBuff[1] = num;
						}
					}
					else if (flag3)
					{
						if (num == 60 || num == 62)
						{
							num3 ^= 1;
						}
						if (num == 62 && num3 != 0)
						{
							flag3 = false;
							num2 = 0;
						}
					}
					else
					{
						if (splitCData && stringBuilder.Length > 0 && flag)
						{
							handler.OnChars(stringBuilder.ToString());
							stringBuilder = new StringBuilder();
						}
						flag = false;
						stringBuilder.Append((char)num);
					}
					break;
				case 13:
				{
					num = reader.Read();
					int num6 = col + 1;
					if (num == 35)
					{
						int num7 = 10;
						int num8 = 0;
						int num9 = 0;
						num = reader.Read();
						num6++;
						if (num == 120)
						{
							num = reader.Read();
							num6++;
							num7 = 16;
						}
						NumberStyles style = ((num7 != 16) ? NumberStyles.Integer : NumberStyles.HexNumber);
						while (true)
						{
							int num10 = -1;
							if (char.IsNumber((char)num) || "abcdef".IndexOf(char.ToLower((char)num)) != -1)
							{
								try
								{
									num10 = int.Parse(new string((char)num, 1), style);
								}
								catch (FormatException)
								{
									num10 = -1;
								}
							}
							if (num10 == -1)
							{
								break;
							}
							num8 *= num7;
							num8 += num10;
							num9++;
							num = reader.Read();
							num6++;
						}
						if (num == 59 && num9 > 0)
						{
							stringBuilder.Append((char)num8);
						}
						else
						{
							FatalErr("Bad char ref");
						}
					}
					else
					{
						string text3 = "aglmopqstu";
						string text4 = "&'\"><";
						int num11 = 0;
						int num12 = 15;
						int num13 = 0;
						int length = stringBuilder.Length;
						while (true)
						{
							if (num11 != 15)
							{
								num11 = text3.IndexOf((char)num) & 0xF;
							}
							if (num11 == 15)
							{
								FatalErr(errors[7]);
							}
							stringBuilder.Append((char)num);
							int num14 = "Ｕ㾏侏ཟｸ\ue1f4⊙\ueeff\ueeffｏ"[num11];
							int num15 = (num14 >> 4) & 0xF;
							int num16 = num14 & 0xF;
							int num17 = num14 >> 12;
							int num18 = (num14 >> 8) & 0xF;
							num = reader.Read();
							num6++;
							num11 = 15;
							if (num15 != 15 && num == text3[num15])
							{
								if (num17 < 14)
								{
									num12 = num17;
								}
								num13 = 12;
							}
							else if (num16 != 15 && num == text3[num16])
							{
								if (num18 < 14)
								{
									num12 = num18;
								}
								num13 = 8;
							}
							else if (num == 59)
							{
								if (num12 != 15 && num13 != 0 && ((num14 >> num13) & 0xF) == 14)
								{
									break;
								}
								continue;
							}
							num11 = 0;
						}
						int num19 = num6 - col - 1;
						if (num19 > 0 && num19 < 5 && (StrEquals("amp", stringBuilder, length, num19) || StrEquals("apos", stringBuilder, length, num19) || StrEquals("quot", stringBuilder, length, num19) || StrEquals("lt", stringBuilder, length, num19) || StrEquals("gt", stringBuilder, length, num19)))
						{
							stringBuilder.Length = length;
							stringBuilder.Append(text4[num12]);
						}
						else
						{
							FatalErr(errors[7]);
						}
					}
					col = num6;
					break;
				}
				default:
					FatalErr(string.Format("Unexpected action code - {0}.", num5));
					break;
				}
			}
			if (num2 != 0)
			{
				FatalErr("Unexpected EOF");
			}
			handler.OnEndParsing(this);
		}
	}
}
