using System.Collections;

namespace System.Text.RegularExpressions
{
	internal class QuickSearch
	{
		private string str;

		private int len;

		private bool ignore;

		private bool reverse;

		private byte[] shift;

		private Hashtable shiftExtended;

		private static readonly int THRESHOLD = 5;

		public string String
		{
			get
			{
				return str;
			}
		}

		public int Length
		{
			get
			{
				return len;
			}
		}

		public bool IgnoreCase
		{
			get
			{
				return ignore;
			}
		}

		public QuickSearch(string str, bool ignore)
			: this(str, ignore, false)
		{
		}

		public QuickSearch(string str, bool ignore, bool reverse)
		{
			this.str = str;
			len = str.Length;
			this.ignore = ignore;
			this.reverse = reverse;
			if (ignore)
			{
				str = str.ToLower();
			}
			if (len > THRESHOLD)
			{
				SetupShiftTable();
			}
		}

		public int Search(string text, int start, int end)
		{
			int i = start;
			if (reverse)
			{
				if (start < end)
				{
					return -1;
				}
				if (i > text.Length)
				{
					i = text.Length;
				}
				if (len == 1)
				{
					while (--i >= end)
					{
						if (str[0] == GetChar(text[i]))
						{
							return i;
						}
					}
					return -1;
				}
				if (end < len)
				{
					end = len - 1;
				}
				i--;
				while (i >= end)
				{
					int num = len - 1;
					while (str[num] == GetChar(text[i - len + 1 + num]))
					{
						if (--num < 0)
						{
							return i - len + 1;
						}
					}
					if (i > end)
					{
						i -= GetShiftDistance(text[i - len]);
						continue;
					}
					break;
				}
			}
			else
			{
				if (len == 1)
				{
					for (; i <= end; i++)
					{
						if (str[0] == GetChar(text[i]))
						{
							return i;
						}
					}
					return -1;
				}
				if (end > text.Length - len)
				{
					end = text.Length - len;
				}
				for (; i <= end; i += GetShiftDistance(text[i + len]))
				{
					int num2 = len - 1;
					while (str[num2] == GetChar(text[i + num2]))
					{
						if (--num2 < 0)
						{
							return i;
						}
					}
					if (i >= end)
					{
						break;
					}
				}
			}
			return -1;
		}

		private void SetupShiftTable()
		{
			bool flag = len > 254;
			byte b = 0;
			for (int i = 0; i < len; i++)
			{
				char c = str[i];
				if (c <= 'ÿ')
				{
					if ((byte)c > b)
					{
						b = (byte)c;
					}
				}
				else
				{
					flag = true;
				}
			}
			shift = new byte[b + 1];
			if (flag)
			{
				shiftExtended = new Hashtable();
			}
			int j = 0;
			for (int num = len; j < len; j++, num--)
			{
				char c2 = str[reverse ? (num - 1) : j];
				if (c2 < shift.Length)
				{
					if (num < 255)
					{
						shift[(uint)c2] = (byte)num;
						continue;
					}
					shift[(uint)c2] = byte.MaxValue;
				}
				shiftExtended[c2] = num;
			}
		}

		private int GetShiftDistance(char c)
		{
			if (shift == null)
			{
				return 1;
			}
			c = GetChar(c);
			if (c < shift.Length)
			{
				int num = shift[(uint)c];
				switch (num)
				{
				case 0:
					return len + 1;
				default:
					return num;
				case 255:
					break;
				}
			}
			else if (c < 'ÿ')
			{
				return len + 1;
			}
			if (shiftExtended == null)
			{
				return len + 1;
			}
			object obj = shiftExtended[c];
			return (obj == null) ? (len + 1) : ((int)obj);
		}

		private char GetChar(char c)
		{
			return ignore ? char.ToLower(c) : c;
		}
	}
}
