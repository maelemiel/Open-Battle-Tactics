using System;
using System.Collections;
using System.Data;
using System.Text;
using Mono.Data.SqlExpressions.yyParser;

namespace Mono.Data.SqlExpressions
{
	internal class Tokenizer : yyInput
	{
		private static readonly IDictionary tokenMap;

		private static readonly object[] tokens;

		private char[] input;

		private int pos;

		private int tok;

		private object val;

		public Tokenizer(string strInput)
		{
			input = strInput.ToCharArray();
			pos = 0;
		}

		static Tokenizer()
		{
			tokenMap = new Hashtable();
			tokens = new object[52]
			{
				259, "and", 260, "or", 261, "not", 262, "true", 263, "false",
				264, "null", 265, "parent", 266, "child", 277, "is", 278, "in",
				279, "not in", 280, "like", 281, "not like", 282, "count", 283, "sum",
				284, "avg", 285, "max", 286, "min", 287, "stdev", 288, "var",
				289, "iif", 290, "substring", 291, "isnull", 292, "len", 293, "trim",
				294, "convert"
			};
			for (int i = 0; i < tokens.Length; i += 2)
			{
				tokenMap.Add(tokens[i + 1], tokens[i]);
			}
		}

		private char Current()
		{
			return input[pos];
		}

		private char Next()
		{
			if (pos + 1 >= input.Length)
			{
				return '\0';
			}
			return input[pos + 1];
		}

		private bool MoveNext()
		{
			pos++;
			if (pos >= input.Length)
			{
				return false;
			}
			return true;
		}

		private bool SkipWhiteSpace()
		{
			if (pos >= input.Length)
			{
				return false;
			}
			while (char.IsWhiteSpace(Current()))
			{
				if (!MoveNext())
				{
					return false;
				}
			}
			return true;
		}

		private object ReadNumber()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Current());
			char c;
			while (char.IsDigit(c = Next()) || c == '.')
			{
				stringBuilder.Append(c);
				if (!MoveNext())
				{
					break;
				}
			}
			string text = stringBuilder.ToString();
			if (text.IndexOf(".") == -1)
			{
				return long.Parse(text);
			}
			return double.Parse(text);
		}

		private char ProcessEscapes(char c)
		{
			if (c == '\\')
			{
				c = (MoveNext() ? Current() : '\0');
				switch (c)
				{
				case 'n':
					c = '\n';
					break;
				case 'r':
					c = '\r';
					break;
				case 't':
					c = '\t';
					break;
				case '\\':
					c = '\\';
					break;
				default:
					throw new SyntaxErrorException(string.Format("Invalid escape sequence: '\\{0}'.", c));
				}
			}
			return c;
		}

		private string ReadString(char terminator)
		{
			return ReadString(terminator, false);
		}

		private string ReadString(char terminator, bool canEscape)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			while (MoveNext())
			{
				if (Current() == terminator)
				{
					if (Next() != terminator)
					{
						flag = true;
						break;
					}
					stringBuilder.Append(ProcessEscapes(Current()));
					MoveNext();
				}
				else
				{
					stringBuilder.Append(ProcessEscapes(Current()));
				}
			}
			if (!flag)
			{
				throw new SyntaxErrorException(string.Format("invalid string at {0}{1}<--", terminator, stringBuilder.ToString()));
			}
			return stringBuilder.ToString();
		}

		private string ReadIdentifier()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Current());
			char c;
			while ((c = Next()) == '_' || char.IsLetterOrDigit(c) || c == '\\')
			{
				stringBuilder.Append(ProcessEscapes(c));
				if (!MoveNext())
				{
					break;
				}
			}
			string text = stringBuilder.ToString();
			if (string.Compare(text, "not", StringComparison.OrdinalIgnoreCase) == 0)
			{
				int num = pos;
				while (char.IsWhiteSpace(c = Next()))
				{
					if (!MoveNext())
					{
						pos = num;
						return text;
					}
				}
				MoveNext();
				string text2;
				switch (Current())
				{
				case 'I':
				case 'i':
					text2 = "in";
					break;
				case 'L':
				case 'l':
					text2 = "like";
					break;
				default:
					pos = num;
					return text;
				}
				int length = text2.Length;
				int num2 = 1;
				while (length-- > 0 && char.IsLetter(c = Next()))
				{
					char c2 = char.ToLowerInvariant(c);
					if (text2[num2++] != c2)
					{
						pos = num;
						return text;
					}
					MoveNext();
				}
				stringBuilder.Append(' ');
				stringBuilder.Append(text2);
				text = stringBuilder.ToString();
			}
			return text;
		}

		private int ParseIdentifier()
		{
			string text = ReadIdentifier();
			object obj = tokenMap[text.ToLower()];
			if (obj != null)
			{
				return (int)obj;
			}
			val = text;
			return 298;
		}

		private int ParseToken()
		{
			char c;
			switch (c = Current())
			{
			case '(':
				return 257;
			case ')':
				return 258;
			case '.':
				return 275;
			case ',':
				return 276;
			case '+':
				return 270;
			case '-':
				return 271;
			case '*':
				return 272;
			case '/':
				return 273;
			case '%':
				return 274;
			case '=':
				return 267;
			case '<':
				return 268;
			case '>':
				return 269;
			case '[':
				val = ReadString(']');
				return 298;
			case '#':
			{
				string s = ReadString('#');
				val = DateTime.Parse(s);
				return 297;
			}
			case '"':
			case '\'':
				val = ReadString(c, true);
				return 295;
			default:
				if (char.IsDigit(c))
				{
					val = ReadNumber();
					return 296;
				}
				if (char.IsLetter(c) || c == '_')
				{
					return ParseIdentifier();
				}
				throw new SyntaxErrorException("invalid token: '" + c + "'");
			}
		}

		public bool advance()
		{
			if (!SkipWhiteSpace())
			{
				return false;
			}
			tok = ParseToken();
			MoveNext();
			return true;
		}

		public int token()
		{
			return tok;
		}

		public object value()
		{
			return val;
		}
	}
}
