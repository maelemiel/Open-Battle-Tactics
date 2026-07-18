using System.Text.RegularExpressions.Syntax;

namespace System.Text.RegularExpressions
{
	internal class ReplacementEvaluator
	{
		private Regex regex;

		private int n_pieces;

		private int[] pieces;

		private string replacement;

		public bool NeedsGroupsOrCaptures
		{
			get
			{
				if (n_pieces == 0)
				{
					return false;
				}
				return true;
			}
		}

		public ReplacementEvaluator(Regex regex, string replacement)
		{
			this.regex = regex;
			this.replacement = replacement;
			pieces = null;
			n_pieces = 0;
			Compile();
		}

		public static string Evaluate(string replacement, Match match)
		{
			System.Text.RegularExpressions.ReplacementEvaluator replacementEvaluator = new System.Text.RegularExpressions.ReplacementEvaluator(match.Regex, replacement);
			return replacementEvaluator.Evaluate(match);
		}

		public string Evaluate(Match match)
		{
			if (n_pieces == 0)
			{
				return replacement;
			}
			StringBuilder stringBuilder = new StringBuilder();
			EvaluateAppend(match, stringBuilder);
			return stringBuilder.ToString();
		}

		public void EvaluateAppend(Match match, StringBuilder sb)
		{
			if (n_pieces == 0)
			{
				sb.Append(replacement);
				return;
			}
			int num = 0;
			while (num < n_pieces)
			{
				int num2 = pieces[num++];
				if (num2 >= 0)
				{
					int count = pieces[num++];
					sb.Append(replacement, num2, count);
					continue;
				}
				if (num2 < -3)
				{
					Group obj = match.Groups[-(num2 + 4)];
					sb.Append(obj.Text, obj.Index, obj.Length);
					continue;
				}
				switch (num2)
				{
				case -1:
					sb.Append(match.Text);
					break;
				case -2:
					sb.Append(match.Text, 0, match.Index);
					break;
				default:
				{
					int num3 = match.Index + match.Length;
					sb.Append(match.Text, num3, match.Text.Length - num3);
					break;
				}
				}
			}
		}

		private void Ensure(int size)
		{
			if (pieces == null)
			{
				int num = 4;
				if (num < size)
				{
					num = size;
				}
				pieces = new int[num];
			}
			else if (size >= pieces.Length)
			{
				int num = pieces.Length + (pieces.Length >> 1);
				if (num < size)
				{
					num = size;
				}
				int[] destinationArray = new int[num];
				Array.Copy(pieces, destinationArray, n_pieces);
				pieces = destinationArray;
			}
		}

		private void AddFromReplacement(int start, int end)
		{
			if (start != end)
			{
				Ensure(n_pieces + 2);
				pieces[n_pieces++] = start;
				pieces[n_pieces++] = end - start;
			}
		}

		private void AddInt(int i)
		{
			Ensure(n_pieces + 1);
			pieces[n_pieces++] = i;
		}

		private void Compile()
		{
			int num = 0;
			int ptr = 0;
			while (ptr < replacement.Length)
			{
				char c = replacement[ptr++];
				if (c != '$')
				{
					continue;
				}
				if (ptr == replacement.Length)
				{
					break;
				}
				if (replacement[ptr] == '$')
				{
					AddFromReplacement(num, ptr);
					num = ++ptr;
					continue;
				}
				int end = ptr - 1;
				int num2 = CompileTerm(ref ptr);
				if (num2 < 0)
				{
					AddFromReplacement(num, end);
					AddInt(num2);
					num = ptr;
				}
			}
			if (num != 0)
			{
				AddFromReplacement(num, ptr);
			}
		}

		private int CompileTerm(ref int ptr)
		{
			char c = replacement[ptr];
			if (char.IsDigit(c))
			{
				int num = System.Text.RegularExpressions.Syntax.Parser.ParseDecimal(replacement, ref ptr);
				if (num < 0 || num > regex.GroupCount)
				{
					return 0;
				}
				return -num - 4;
			}
			ptr++;
			switch (c)
			{
			case '{':
			{
				int num2 = -1;
				string text;
				try
				{
					if (char.IsDigit(replacement[ptr]))
					{
						num2 = System.Text.RegularExpressions.Syntax.Parser.ParseDecimal(replacement, ref ptr);
						text = string.Empty;
					}
					else
					{
						text = System.Text.RegularExpressions.Syntax.Parser.ParseName(replacement, ref ptr);
					}
				}
				catch (IndexOutOfRangeException)
				{
					ptr = replacement.Length;
					return 0;
				}
				if (ptr == replacement.Length || replacement[ptr] != '}' || text == null)
				{
					return 0;
				}
				ptr++;
				if (text != string.Empty)
				{
					num2 = regex.GroupNumberFromName(text);
				}
				if (num2 < 0 || num2 > regex.GroupCount)
				{
					return 0;
				}
				return -num2 - 4;
			}
			case '&':
				return -4;
			case '`':
				return -2;
			case '\'':
				return -3;
			case '+':
				return -regex.GroupCount - 4;
			case '_':
				return -1;
			default:
				return 0;
			}
		}
	}
}
