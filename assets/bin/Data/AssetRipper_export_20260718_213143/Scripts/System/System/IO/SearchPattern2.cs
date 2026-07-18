namespace System.IO
{
	internal class SearchPattern2
	{
		private class Op
		{
			public OpCode Code;

			public string Argument;

			public Op Next;

			public Op(OpCode code)
			{
				Code = code;
				Argument = null;
				Next = null;
			}
		}

		private enum OpCode
		{
			ExactString = 0,
			AnyChar = 1,
			AnyString = 2,
			End = 3,
			True = 4
		}

		private Op ops;

		private bool ignore;

		private bool hasWildcard;

		private string pattern;

		internal static readonly char[] WildcardChars = new char[2] { '*', '?' };

		internal static readonly char[] InvalidChars = new char[2]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar
		};

		public bool HasWildcard
		{
			get
			{
				return hasWildcard;
			}
		}

		public SearchPattern2(string pattern)
			: this(pattern, false)
		{
		}

		public SearchPattern2(string pattern, bool ignore)
		{
			this.ignore = ignore;
			this.pattern = pattern;
			Compile(pattern);
		}

		public bool IsMatch(string text, bool ignorecase)
		{
			if (!hasWildcard)
			{
				if (string.Compare(pattern, text, ignorecase) == 0)
				{
					return true;
				}
				int num = text.LastIndexOf('/');
				if (num == -1)
				{
					return false;
				}
				num++;
				if (num == text.Length)
				{
					return false;
				}
				return string.Compare(pattern, text.Substring(num), ignorecase) == 0;
			}
			return Match(ops, text, 0);
		}

		public bool IsMatch(string text)
		{
			return IsMatch(text, ignore);
		}

		private void Compile(string pattern)
		{
			if (pattern == null || pattern.IndexOfAny(InvalidChars) >= 0)
			{
				throw new ArgumentException("Invalid search pattern: '" + pattern + "'");
			}
			if (pattern == "*")
			{
				ops = new Op(OpCode.True);
				hasWildcard = true;
				return;
			}
			ops = null;
			int num = 0;
			Op op = null;
			while (num < pattern.Length)
			{
				Op op2;
				switch (pattern[num])
				{
				case '?':
					op2 = new Op(OpCode.AnyChar);
					num++;
					hasWildcard = true;
					break;
				case '*':
					op2 = new Op(OpCode.AnyString);
					num++;
					hasWildcard = true;
					break;
				default:
				{
					op2 = new Op(OpCode.ExactString);
					int num2 = pattern.IndexOfAny(WildcardChars, num);
					if (num2 < 0)
					{
						num2 = pattern.Length;
					}
					op2.Argument = pattern.Substring(num, num2 - num);
					if (ignore)
					{
						op2.Argument = op2.Argument.ToLower();
					}
					num = num2;
					break;
				}
				}
				if (op == null)
				{
					ops = op2;
				}
				else
				{
					op.Next = op2;
				}
				op = op2;
			}
			if (op == null)
			{
				ops = new Op(OpCode.End);
			}
			else
			{
				op.Next = new Op(OpCode.End);
			}
		}

		private bool Match(Op op, string text, int ptr)
		{
			for (; op != null; op = op.Next)
			{
				switch (op.Code)
				{
				case OpCode.True:
					return true;
				case OpCode.End:
					if (ptr == text.Length)
					{
						return true;
					}
					return false;
				case OpCode.ExactString:
				{
					int length = op.Argument.Length;
					if (ptr + length > text.Length)
					{
						return false;
					}
					string text2 = text.Substring(ptr, length);
					if (ignore)
					{
						text2 = text2.ToLower();
					}
					if (text2 != op.Argument)
					{
						return false;
					}
					ptr += length;
					continue;
				}
				case OpCode.AnyChar:
					if (++ptr > text.Length)
					{
						return false;
					}
					continue;
				case OpCode.AnyString:
					break;
				default:
					continue;
				}
				while (ptr <= text.Length)
				{
					if (Match(op.Next, text, ptr))
					{
						return true;
					}
					ptr++;
				}
				return false;
			}
			return true;
		}
	}
}
