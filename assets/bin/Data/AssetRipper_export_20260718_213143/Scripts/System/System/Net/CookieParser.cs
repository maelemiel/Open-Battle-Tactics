namespace System.Net
{
	internal class CookieParser
	{
		private string header;

		private int pos;

		private int length;

		public CookieParser(string header)
			: this(header, 0)
		{
		}

		public CookieParser(string header, int position)
		{
			this.header = header;
			pos = position;
			length = header.Length;
		}

		public bool GetNextNameValue(out string name, out string val)
		{
			name = null;
			val = null;
			if (pos >= length)
			{
				return false;
			}
			name = GetCookieName();
			if (pos < header.Length && header[pos] == '=')
			{
				pos++;
				val = GetCookieValue();
			}
			if (pos < length && header[pos] == ';')
			{
				pos++;
			}
			return true;
		}

		private string GetCookieName()
		{
			int i;
			for (i = pos; i < length && char.IsWhiteSpace(header[i]); i++)
			{
			}
			int num = i;
			for (; i < length && header[i] != ';' && header[i] != '='; i++)
			{
			}
			pos = i;
			return header.Substring(num, i - num).Trim();
		}

		private string GetCookieValue()
		{
			if (pos >= length)
			{
				return null;
			}
			int i;
			for (i = pos; i < length && char.IsWhiteSpace(header[i]); i++)
			{
			}
			int num;
			if (header[i] == '"')
			{
				num = ++i;
				for (; i < length && header[i] != '"'; i++)
				{
				}
				int j;
				for (j = i; j < length && header[j] != ';'; j++)
				{
				}
				pos = j;
			}
			else
			{
				num = i;
				for (; i < length && header[i] != ';'; i++)
				{
				}
				pos = i;
			}
			return header.Substring(num, i - num).Trim();
		}
	}
}
