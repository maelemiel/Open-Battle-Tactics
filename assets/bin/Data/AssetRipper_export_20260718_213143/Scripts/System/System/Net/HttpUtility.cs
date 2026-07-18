using System.Globalization;
using System.IO;
using System.Text;

namespace System.Net
{
	internal sealed class HttpUtility
	{
		private HttpUtility()
		{
		}

		public static string UrlDecode(string s)
		{
			return UrlDecode(s, null);
		}

		private static char[] GetChars(MemoryStream b, Encoding e)
		{
			return e.GetChars(b.GetBuffer(), 0, (int)b.Length);
		}

		public static string UrlDecode(string s, Encoding e)
		{
			if (s == null)
			{
				return null;
			}
			if (s.IndexOf('%') == -1 && s.IndexOf('+') == -1)
			{
				return s;
			}
			if (e == null)
			{
				e = Encoding.GetEncoding(28591);
			}
			StringBuilder stringBuilder = new StringBuilder();
			long num = s.Length;
			NumberStyles style = NumberStyles.HexNumber;
			MemoryStream memoryStream = new MemoryStream();
			for (int i = 0; i < num; i++)
			{
				if (s[i] == '%' && i + 2 < num)
				{
					if (s[i + 1] == 'u' && i + 5 < num)
					{
						if (memoryStream.Length > 0)
						{
							stringBuilder.Append(GetChars(memoryStream, e));
							memoryStream.SetLength(0L);
						}
						stringBuilder.Append((char)int.Parse(s.Substring(i + 2, 4), style));
						i += 5;
					}
					else
					{
						memoryStream.WriteByte((byte)int.Parse(s.Substring(i + 1, 2), style));
						i += 2;
					}
				}
				else
				{
					if (memoryStream.Length > 0)
					{
						stringBuilder.Append(GetChars(memoryStream, e));
						memoryStream.SetLength(0L);
					}
					if (s[i] == '+')
					{
						stringBuilder.Append(' ');
					}
					else
					{
						stringBuilder.Append(s[i]);
					}
				}
			}
			if (memoryStream.Length > 0)
			{
				stringBuilder.Append(GetChars(memoryStream, e));
			}
			memoryStream = null;
			return stringBuilder.ToString();
		}
	}
}
