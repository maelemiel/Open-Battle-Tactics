using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Mono.Xml.XPath.yyParser;

namespace Mono.Xml.XPath
{
	internal class Tokenizer : yyInput
	{
		private const char EOL = '\0';

		private string m_rgchInput;

		private int m_ich;

		private int m_cch;

		private int m_iToken;

		private int m_iTokenPrev = 258;

		private object m_objToken;

		private bool m_fPrevWasOperator;

		private bool m_fThisIsOperator;

		private static readonly Hashtable s_mapTokens;

		private static readonly object[] s_rgTokenMap;

		private bool IsFirstToken
		{
			get
			{
				return m_iTokenPrev == 258;
			}
		}

		public Tokenizer(string strInput)
		{
			m_rgchInput = strInput;
			m_ich = 0;
			m_cch = strInput.Length;
			SkipWhitespace();
		}

		static Tokenizer()
		{
			s_mapTokens = new Hashtable();
			s_rgTokenMap = new object[42]
			{
				274, "and", 276, "or", 278, "div", 280, "mod", 296, "ancestor",
				298, "ancestor-or-self", 300, "attribute", 302, "child", 304, "descendant", 306, "descendant-or-self",
				308, "following", 310, "following-sibling", 312, "namespace", 314, "parent", 316, "preceding",
				318, "preceding-sibling", 320, "self", 322, "comment", 324, "text", 326, "processing-instruction",
				328, "node"
			};
			for (int i = 0; i < s_rgTokenMap.Length; i += 2)
			{
				s_mapTokens.Add(s_rgTokenMap[i + 1], s_rgTokenMap[i]);
			}
		}

		private char Peek(int iOffset)
		{
			if (m_ich + iOffset >= m_cch)
			{
				return '\0';
			}
			return m_rgchInput[m_ich + iOffset];
		}

		private char Peek()
		{
			return Peek(0);
		}

		private char GetChar()
		{
			if (m_ich >= m_cch)
			{
				return '\0';
			}
			return m_rgchInput[m_ich++];
		}

		private char PutBack()
		{
			if (m_ich == 0)
			{
				throw new XPathException("XPath parser returned an error status: invalid tokenizer state.");
			}
			return m_rgchInput[--m_ich];
		}

		private bool SkipWhitespace()
		{
			if (!IsWhitespace(Peek()))
			{
				return false;
			}
			while (IsWhitespace(Peek()))
			{
				GetChar();
			}
			return true;
		}

		private int ParseNumber()
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (IsDigit(Peek()))
			{
				stringBuilder.Append(GetChar());
			}
			if (Peek() == '.')
			{
				stringBuilder.Append(GetChar());
				while (IsDigit(Peek()))
				{
					stringBuilder.Append(GetChar());
				}
			}
			m_objToken = double.Parse(stringBuilder.ToString(), NumberFormatInfo.InvariantInfo);
			return 331;
		}

		private int ParseLiteral()
		{
			StringBuilder stringBuilder = new StringBuilder();
			char c = GetChar();
			char c2;
			while ((c2 = Peek()) != c)
			{
				if (c2 == '\0')
				{
					throw new XPathException("unmatched " + c + " in expression");
				}
				stringBuilder.Append(GetChar());
			}
			GetChar();
			m_objToken = stringBuilder.ToString();
			return 332;
		}

		private string ReadIdentifier()
		{
			StringBuilder stringBuilder = new StringBuilder();
			char c = Peek();
			if (!char.IsLetter(c) && c != '_')
			{
				return null;
			}
			stringBuilder.Append(GetChar());
			while ((c = Peek()) == '_' || c == '-' || c == '.' || char.IsLetterOrDigit(c))
			{
				stringBuilder.Append(GetChar());
			}
			SkipWhitespace();
			return stringBuilder.ToString();
		}

		private int ParseIdentifier()
		{
			string text = ReadIdentifier();
			object obj = s_mapTokens[text];
			int num = ((obj == null) ? 333 : ((int)obj));
			m_objToken = text;
			char c = Peek();
			if (c == ':')
			{
				if (Peek(1) == ':')
				{
					if (obj == null || !IsAxisName(num))
					{
						throw new XPathException("invalid axis name: '" + text + "'");
					}
					return num;
				}
				GetChar();
				SkipWhitespace();
				c = Peek();
				if (c == '*')
				{
					GetChar();
					m_objToken = new XmlQualifiedName(string.Empty, text);
					return 333;
				}
				string text2 = ReadIdentifier();
				if (text2 == null)
				{
					throw new XPathException("invalid QName: " + text + ":" + c);
				}
				c = Peek();
				m_objToken = new XmlQualifiedName(text2, text);
				if (c == '(')
				{
					return 269;
				}
				return 333;
			}
			if (!IsFirstToken && !m_fPrevWasOperator)
			{
				if (obj == null || !IsOperatorName(num))
				{
					throw new XPathException("invalid operator name: '" + text + "'");
				}
				return num;
			}
			if (c == '(')
			{
				if (obj == null)
				{
					m_objToken = new XmlQualifiedName(text, string.Empty);
					return 269;
				}
				if (IsNodeType(num))
				{
					return num;
				}
				throw new XPathException("invalid function name: '" + text + "'");
			}
			m_objToken = new XmlQualifiedName(text, string.Empty);
			return 333;
		}

		private static bool IsWhitespace(char ch)
		{
			return ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
		}

		private static bool IsDigit(char ch)
		{
			return ch >= '0' && ch <= '9';
		}

		private int ParseToken()
		{
			char c = Peek();
			switch (c)
			{
			case '\0':
				return 258;
			case '/':
				m_fThisIsOperator = true;
				GetChar();
				if (Peek() == '/')
				{
					GetChar();
					return 260;
				}
				return 259;
			case '.':
				GetChar();
				if (Peek() == '.')
				{
					GetChar();
					return 263;
				}
				if (IsDigit(Peek()))
				{
					PutBack();
					return ParseNumber();
				}
				return 262;
			case ':':
				GetChar();
				if (Peek() == ':')
				{
					m_fThisIsOperator = true;
					GetChar();
					return 265;
				}
				return 257;
			case ',':
				m_fThisIsOperator = true;
				GetChar();
				return 267;
			case '@':
				m_fThisIsOperator = true;
				GetChar();
				return 268;
			case '[':
				m_fThisIsOperator = true;
				GetChar();
				return 270;
			case ']':
				GetChar();
				return 271;
			case '(':
				m_fThisIsOperator = true;
				GetChar();
				return 272;
			case ')':
				GetChar();
				return 273;
			case '+':
				m_fThisIsOperator = true;
				GetChar();
				return 282;
			case '-':
				m_fThisIsOperator = true;
				GetChar();
				return 283;
			case '*':
				GetChar();
				if (!IsFirstToken && !m_fPrevWasOperator)
				{
					m_fThisIsOperator = true;
					return 330;
				}
				return 284;
			case '$':
				GetChar();
				m_fThisIsOperator = true;
				return 285;
			case '|':
				m_fThisIsOperator = true;
				GetChar();
				return 286;
			case '=':
				m_fThisIsOperator = true;
				GetChar();
				return 287;
			case '!':
				GetChar();
				if (Peek() == '=')
				{
					m_fThisIsOperator = true;
					GetChar();
					return 288;
				}
				break;
			case '>':
				m_fThisIsOperator = true;
				GetChar();
				if (Peek() == '=')
				{
					GetChar();
					return 292;
				}
				return 295;
			case '<':
				m_fThisIsOperator = true;
				GetChar();
				if (Peek() == '=')
				{
					GetChar();
					return 290;
				}
				return 294;
			case '\'':
				return ParseLiteral();
			case '"':
				return ParseLiteral();
			default:
				if (IsDigit(c))
				{
					return ParseNumber();
				}
				if (char.IsLetter(c) || c == '_')
				{
					int num = ParseIdentifier();
					if (IsOperatorName(num))
					{
						m_fThisIsOperator = true;
					}
					return num;
				}
				break;
			}
			throw new XPathException("invalid token: '" + c + "'");
		}

		public bool advance()
		{
			m_fThisIsOperator = false;
			m_objToken = null;
			m_iToken = ParseToken();
			SkipWhitespace();
			m_iTokenPrev = m_iToken;
			m_fPrevWasOperator = m_fThisIsOperator;
			return m_iToken != 258;
		}

		public int token()
		{
			return m_iToken;
		}

		public object value()
		{
			return m_objToken;
		}

		private bool IsNodeType(int iToken)
		{
			switch (iToken)
			{
			case 322:
			case 324:
			case 326:
			case 328:
				return true;
			default:
				return false;
			}
		}

		private bool IsOperatorName(int iToken)
		{
			switch (iToken)
			{
			case 274:
			case 276:
			case 278:
			case 280:
				return true;
			default:
				return false;
			}
		}

		private bool IsAxisName(int iToken)
		{
			switch (iToken)
			{
			case 296:
			case 298:
			case 300:
			case 302:
			case 304:
			case 306:
			case 308:
			case 310:
			case 312:
			case 314:
			case 316:
			case 318:
			case 320:
				return true;
			default:
				return false;
			}
		}
	}
}
