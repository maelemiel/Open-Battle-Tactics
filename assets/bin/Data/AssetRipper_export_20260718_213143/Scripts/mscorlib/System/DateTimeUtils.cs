using System.Globalization;
using System.Text;

namespace System
{
	internal static class DateTimeUtils
	{
		public static int CountRepeat(string fmt, int p, char c)
		{
			int length = fmt.Length;
			int i;
			for (i = p + 1; i < length && fmt[i] == c; i++)
			{
			}
			return i - p;
		}

		public unsafe static void ZeroPad(StringBuilder output, int digits, int len)
		{
			char* ptr = stackalloc char[16];
			int num = 16;
			do
			{
				*(ushort*)((byte*)ptr + --num * 2) = (ushort)(48 + digits % 10);
				digits /= 10;
				len--;
			}
			while (digits > 0);
			while (len-- > 0)
			{
				*(short*)((byte*)ptr + --num * 2) = 48;
			}
			output.Append(new string(ptr, num, 16 - num));
		}

		public static int ParseQuotedString(string fmt, int pos, StringBuilder output)
		{
			int length = fmt.Length;
			int num = pos;
			char c = fmt[pos++];
			while (pos < length)
			{
				char c2 = fmt[pos++];
				if (c2 == c)
				{
					return pos - num;
				}
				if (c2 == '\\')
				{
					if (pos >= length)
					{
						throw new FormatException("Un-ended quote");
					}
					output.Append(fmt[pos++]);
				}
				else
				{
					output.Append(c2);
				}
			}
			throw new FormatException("Un-ended quote");
		}

		public static string GetStandardPattern(char format, DateTimeFormatInfo dfi, out bool useutc, out bool use_invariant)
		{
			return GetStandardPattern(format, dfi, out useutc, out use_invariant, false);
		}

		public static string GetStandardPattern(char format, DateTimeFormatInfo dfi, out bool useutc, out bool use_invariant, bool date_time_offset)
		{
			useutc = false;
			use_invariant = false;
			string result;
			switch (format)
			{
			case 'd':
				result = dfi.ShortDatePattern;
				break;
			case 'D':
				result = dfi.LongDatePattern;
				break;
			case 'f':
				result = dfi.LongDatePattern + " " + dfi.ShortTimePattern;
				break;
			case 'F':
				result = dfi.FullDateTimePattern;
				break;
			case 'g':
				result = dfi.ShortDatePattern + " " + dfi.ShortTimePattern;
				break;
			case 'G':
				result = dfi.ShortDatePattern + " " + dfi.LongTimePattern;
				break;
			case 'M':
			case 'm':
				result = dfi.MonthDayPattern;
				break;
			case 'O':
			case 'o':
				result = dfi.RoundtripPattern;
				use_invariant = true;
				break;
			case 'R':
			case 'r':
				result = dfi.RFC1123Pattern;
				if (date_time_offset)
				{
					useutc = true;
				}
				use_invariant = true;
				break;
			case 's':
				result = dfi.SortableDateTimePattern;
				use_invariant = true;
				break;
			case 't':
				result = dfi.ShortTimePattern;
				break;
			case 'T':
				result = dfi.LongTimePattern;
				break;
			case 'u':
				result = dfi.UniversalSortableDateTimePattern;
				if (date_time_offset)
				{
					useutc = true;
				}
				use_invariant = true;
				break;
			case 'U':
				if (date_time_offset)
				{
					result = null;
					break;
				}
				result = dfi.FullDateTimePattern;
				useutc = true;
				break;
			case 'Y':
			case 'y':
				result = dfi.YearMonthPattern;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		public static string ToString(DateTime dt, string format, DateTimeFormatInfo dfi)
		{
			return ToString(dt, null, format, dfi);
		}

		public static string ToString(DateTime dt, TimeSpan? utc_offset, string format, DateTimeFormatInfo dfi)
		{
			StringBuilder stringBuilder = new StringBuilder(format.Length + 10);
			DateTimeFormatInfo invariantInfo = DateTimeFormatInfo.InvariantInfo;
			if (format == invariantInfo.RFC1123Pattern)
			{
				dfi = invariantInfo;
			}
			else if (format == invariantInfo.UniversalSortableDateTimePattern)
			{
				dfi = invariantInfo;
			}
			int num;
			for (int i = 0; i < format.Length; i += num)
			{
				bool flag = false;
				char c = format[i];
				switch (c)
				{
				case 'h':
				{
					num = CountRepeat(format, i, c);
					int num3 = dt.Hour % 12;
					if (num3 == 0)
					{
						num3 = 12;
					}
					ZeroPad(stringBuilder, num3, (num == 1) ? 1 : 2);
					break;
				}
				case 'H':
					num = CountRepeat(format, i, c);
					ZeroPad(stringBuilder, dt.Hour, (num == 1) ? 1 : 2);
					break;
				case 'm':
					num = CountRepeat(format, i, c);
					ZeroPad(stringBuilder, dt.Minute, (num == 1) ? 1 : 2);
					break;
				case 's':
					num = CountRepeat(format, i, c);
					ZeroPad(stringBuilder, dt.Second, (num == 1) ? 1 : 2);
					break;
				case 'F':
					flag = true;
					goto case 'f';
				case 'f':
				{
					num = CountRepeat(format, i, c);
					if (num > 7)
					{
						throw new FormatException("Invalid Format String");
					}
					int num2 = (int)(dt.Ticks % 10000000 / (long)Math.Pow(10.0, 7 - num));
					int length = stringBuilder.Length;
					ZeroPad(stringBuilder, num2, num);
					if (flag)
					{
						while (stringBuilder.Length > length && stringBuilder[stringBuilder.Length - 1] == '0')
						{
							stringBuilder.Length--;
						}
						if (num2 == 0 && length > 0 && stringBuilder[length - 1] == '.')
						{
							stringBuilder.Length--;
						}
					}
					break;
				}
				case 't':
				{
					num = CountRepeat(format, i, c);
					string text = ((dt.Hour >= 12) ? dfi.PMDesignator : dfi.AMDesignator);
					if (num == 1)
					{
						if (text.Length >= 1)
						{
							stringBuilder.Append(text[0]);
						}
					}
					else
					{
						stringBuilder.Append(text);
					}
					break;
				}
				case 'z':
				{
					num = CountRepeat(format, i, c);
					TimeSpan timeSpan = ((!utc_offset.HasValue) ? TimeZone.CurrentTimeZone.GetUtcOffset(dt) : utc_offset.Value);
					if (timeSpan.Ticks >= 0)
					{
						stringBuilder.Append('+');
					}
					else
					{
						stringBuilder.Append('-');
					}
					switch (num)
					{
					case 1:
						stringBuilder.Append(Math.Abs(timeSpan.Hours));
						break;
					case 2:
						stringBuilder.Append(Math.Abs(timeSpan.Hours).ToString("00"));
						break;
					default:
						stringBuilder.Append(Math.Abs(timeSpan.Hours).ToString("00"));
						stringBuilder.Append(':');
						stringBuilder.Append(Math.Abs(timeSpan.Minutes).ToString("00"));
						break;
					}
					break;
				}
				case 'K':
					num = 1;
					if (utc_offset.HasValue || dt.Kind == DateTimeKind.Local)
					{
						TimeSpan timeSpan = ((!utc_offset.HasValue) ? TimeZone.CurrentTimeZone.GetUtcOffset(dt) : utc_offset.Value);
						if (timeSpan.Ticks >= 0)
						{
							stringBuilder.Append('+');
						}
						else
						{
							stringBuilder.Append('-');
						}
						stringBuilder.Append(Math.Abs(timeSpan.Hours).ToString("00"));
						stringBuilder.Append(':');
						stringBuilder.Append(Math.Abs(timeSpan.Minutes).ToString("00"));
					}
					else if (dt.Kind == DateTimeKind.Utc)
					{
						stringBuilder.Append('Z');
					}
					break;
				case 'd':
					num = CountRepeat(format, i, c);
					if (num <= 2)
					{
						ZeroPad(stringBuilder, dfi.Calendar.GetDayOfMonth(dt), (num == 1) ? 1 : 2);
					}
					else if (num == 3)
					{
						stringBuilder.Append(dfi.GetAbbreviatedDayName(dfi.Calendar.GetDayOfWeek(dt)));
					}
					else
					{
						stringBuilder.Append(dfi.GetDayName(dfi.Calendar.GetDayOfWeek(dt)));
					}
					break;
				case 'M':
				{
					num = CountRepeat(format, i, c);
					int month = dfi.Calendar.GetMonth(dt);
					if (num <= 2)
					{
						ZeroPad(stringBuilder, month, num);
					}
					else if (num == 3)
					{
						stringBuilder.Append(dfi.GetAbbreviatedMonthName(month));
					}
					else
					{
						stringBuilder.Append(dfi.GetMonthName(month));
					}
					break;
				}
				case 'y':
					num = CountRepeat(format, i, c);
					if (num <= 2)
					{
						ZeroPad(stringBuilder, dfi.Calendar.GetYear(dt) % 100, num);
					}
					else
					{
						ZeroPad(stringBuilder, dfi.Calendar.GetYear(dt), num);
					}
					break;
				case 'g':
					num = CountRepeat(format, i, c);
					stringBuilder.Append(dfi.GetEraName(dfi.Calendar.GetEra(dt)));
					break;
				case ':':
					stringBuilder.Append(dfi.TimeSeparator);
					num = 1;
					break;
				case '/':
					stringBuilder.Append(dfi.DateSeparator);
					num = 1;
					break;
				case '"':
				case '\'':
					num = ParseQuotedString(format, i, stringBuilder);
					break;
				case '%':
					if (i >= format.Length - 1)
					{
						throw new FormatException("% at end of date time string");
					}
					if (format[i + 1] == '%')
					{
						throw new FormatException("%% in date string");
					}
					num = 1;
					break;
				case '\\':
					if (i >= format.Length - 1)
					{
						throw new FormatException("\\ at end of date time string");
					}
					stringBuilder.Append(format[i + 1]);
					num = 2;
					break;
				default:
					stringBuilder.Append(c);
					num = 1;
					break;
				}
			}
			return stringBuilder.ToString();
		}
	}
}
