namespace System.Net
{
	internal class DigestHeaderParser
	{
		private string header;

		private int length;

		private int pos;

		private static string[] keywords = new string[5] { "realm", "opaque", "nonce", "algorithm", "qop" };

		private string[] values = new string[keywords.Length];

		public string Realm
		{
			get
			{
				return values[0];
			}
		}

		public string Opaque
		{
			get
			{
				return values[1];
			}
		}

		public string Nonce
		{
			get
			{
				return values[2];
			}
		}

		public string Algorithm
		{
			get
			{
				return values[3];
			}
		}

		public string QOP
		{
			get
			{
				return values[4];
			}
		}

		public DigestHeaderParser(string header)
		{
			this.header = header.Trim();
		}

		public bool Parse()
		{
			if (!header.ToLower().StartsWith("digest "))
			{
				return false;
			}
			pos = 6;
			length = header.Length;
			while (pos < length)
			{
				string key;
				string value;
				if (!GetKeywordAndValue(out key, out value))
				{
					return false;
				}
				SkipWhitespace();
				if (pos < length && header[pos] == ',')
				{
					pos++;
				}
				int num = Array.IndexOf(keywords, key);
				if (num != -1)
				{
					if (values[num] != null)
					{
						return false;
					}
					values[num] = value;
				}
			}
			if (Realm == null || Nonce == null)
			{
				return false;
			}
			return true;
		}

		private void SkipWhitespace()
		{
			char c = ' ';
			while (pos < length && (c == ' ' || c == '\t' || c == '\r' || c == '\n'))
			{
				c = header[pos++];
			}
			pos--;
		}

		private string GetKey()
		{
			SkipWhitespace();
			int num = pos;
			while (pos < length && header[pos] != '=')
			{
				pos++;
			}
			return header.Substring(num, pos - num).Trim().ToLower();
		}

		private bool GetKeywordAndValue(out string key, out string value)
		{
			key = null;
			value = null;
			key = GetKey();
			if (pos >= length)
			{
				return false;
			}
			SkipWhitespace();
			if (pos + 1 >= length || header[pos++] != '=')
			{
				return false;
			}
			SkipWhitespace();
			if (pos + 1 >= length)
			{
				return false;
			}
			bool flag = false;
			if (header[pos] == '"')
			{
				pos++;
				flag = true;
			}
			int num = pos;
			if (flag)
			{
				pos = header.IndexOf('"', pos);
				if (pos == -1)
				{
					return false;
				}
			}
			else
			{
				char c;
				do
				{
					c = header[pos];
				}
				while (c != ',' && c != ' ' && c != '\t' && c != '\r' && c != '\n' && ++pos < length);
				if (pos >= length && num == pos)
				{
					return false;
				}
			}
			value = header.Substring(num, pos - num);
			pos += 2;
			return true;
		}
	}
}
