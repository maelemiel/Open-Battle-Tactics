using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public struct DateTimeOffset : IFormattable, IComparable, ISerializable, IComparable<DateTimeOffset>, IEquatable<DateTimeOffset>, IDeserializationCallback
	{
		public static readonly DateTimeOffset MaxValue;

		public static readonly DateTimeOffset MinValue;

		private DateTime dt;

		private TimeSpan utc_offset;

		public DateTime Date
		{
			get
			{
				return DateTime.SpecifyKind(dt.Date, DateTimeKind.Unspecified);
			}
		}

		public DateTime DateTime
		{
			get
			{
				return DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
			}
		}

		public int Day
		{
			get
			{
				return dt.Day;
			}
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return dt.DayOfWeek;
			}
		}

		public int DayOfYear
		{
			get
			{
				return dt.DayOfYear;
			}
		}

		public int Hour
		{
			get
			{
				return dt.Hour;
			}
		}

		public DateTime LocalDateTime
		{
			get
			{
				return UtcDateTime.ToLocalTime();
			}
		}

		public int Millisecond
		{
			get
			{
				return dt.Millisecond;
			}
		}

		public int Minute
		{
			get
			{
				return dt.Minute;
			}
		}

		public int Month
		{
			get
			{
				return dt.Month;
			}
		}

		public static DateTimeOffset Now
		{
			get
			{
				return new DateTimeOffset(DateTime.Now);
			}
		}

		public TimeSpan Offset
		{
			get
			{
				return utc_offset;
			}
		}

		public int Second
		{
			get
			{
				return dt.Second;
			}
		}

		public long Ticks
		{
			get
			{
				return dt.Ticks;
			}
		}

		public TimeSpan TimeOfDay
		{
			get
			{
				return dt.TimeOfDay;
			}
		}

		public DateTime UtcDateTime
		{
			get
			{
				return DateTime.SpecifyKind(dt - utc_offset, DateTimeKind.Utc);
			}
		}

		public static DateTimeOffset UtcNow
		{
			get
			{
				return new DateTimeOffset(DateTime.UtcNow);
			}
		}

		public long UtcTicks
		{
			get
			{
				return UtcDateTime.Ticks;
			}
		}

		public int Year
		{
			get
			{
				return dt.Year;
			}
		}

		public DateTimeOffset(DateTime dateTime)
		{
			dt = dateTime;
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				utc_offset = TimeSpan.Zero;
			}
			else
			{
				utc_offset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
			}
			if (UtcDateTime < DateTime.MinValue || UtcDateTime > DateTime.MaxValue)
			{
				throw new ArgumentOutOfRangeException("The UTC date and time that results from applying the offset is earlier than MinValue or later than MaxValue.");
			}
		}

		public DateTimeOffset(DateTime dateTime, TimeSpan offset)
		{
			if (dateTime.Kind == DateTimeKind.Utc && offset != TimeSpan.Zero)
			{
				throw new ArgumentException("dateTime.Kind equals Utc and offset does not equal zero.");
			}
			if (dateTime.Kind == DateTimeKind.Local && offset != TimeZone.CurrentTimeZone.GetUtcOffset(dateTime))
			{
				throw new ArgumentException("dateTime.Kind equals Local and offset does not equal the offset of the system's local time zone.");
			}
			if (offset.Ticks % 600000000 != 0L)
			{
				throw new ArgumentException("offset is not specified in whole minutes.");
			}
			if (offset < new TimeSpan(-14, 0, 0) || offset > new TimeSpan(14, 0, 0))
			{
				throw new ArgumentOutOfRangeException("offset is less than -14 hours or greater than 14 hours.");
			}
			dt = dateTime;
			utc_offset = offset;
			if (UtcDateTime < DateTime.MinValue || UtcDateTime > DateTime.MaxValue)
			{
				throw new ArgumentOutOfRangeException("The UtcDateTime property is earlier than MinValue or later than MaxValue.");
			}
		}

		public DateTimeOffset(long ticks, TimeSpan offset)
			: this(new DateTime(ticks), offset)
		{
		}

		public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, TimeSpan offset)
			: this(new DateTime(year, month, day, hour, minute, second), offset)
		{
		}

		public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeSpan offset)
			: this(new DateTime(year, month, day, hour, minute, second, millisecond), offset)
		{
		}

		public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, TimeSpan offset)
			: this(new DateTime(year, month, day, hour, minute, second, millisecond, calendar), offset)
		{
		}

		private DateTimeOffset(SerializationInfo info, StreamingContext context)
		{
			DateTime dateTime = (DateTime)info.GetValue("DateTime", typeof(DateTime));
			short @int = info.GetInt16("OffsetMinutes");
			utc_offset = TimeSpan.FromMinutes(@int);
			dt = dateTime.Add(utc_offset);
		}

		static DateTimeOffset()
		{
			MaxValue = new DateTimeOffset(DateTime.MaxValue, TimeSpan.Zero);
			MinValue = new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero);
			if (MonoTouchAOTHelper.FalseFlag)
			{
				GenericComparer<DateTimeOffset> genericComparer = new GenericComparer<DateTimeOffset>();
				GenericEqualityComparer<DateTimeOffset> genericEqualityComparer = new GenericEqualityComparer<DateTimeOffset>();
			}
		}

		int IComparable.CompareTo(object obj)
		{
			return CompareTo((DateTimeOffset)obj);
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			DateTime value = new DateTime(dt.Ticks).Subtract(utc_offset);
			info.AddValue("DateTime", value);
			info.AddValue("OffsetMinutes", (short)utc_offset.TotalMinutes);
		}

		[MonoTODO]
		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		public DateTimeOffset Add(TimeSpan timeSpan)
		{
			return new DateTimeOffset(dt.Add(timeSpan), utc_offset);
		}

		public DateTimeOffset AddDays(double days)
		{
			return new DateTimeOffset(dt.AddDays(days), utc_offset);
		}

		public DateTimeOffset AddHours(double hours)
		{
			return new DateTimeOffset(dt.AddHours(hours), utc_offset);
		}

		public DateTimeOffset AddMilliseconds(double milliseconds)
		{
			return new DateTimeOffset(dt.AddMilliseconds(milliseconds), utc_offset);
		}

		public DateTimeOffset AddMinutes(double minutes)
		{
			return new DateTimeOffset(dt.AddMinutes(minutes), utc_offset);
		}

		public DateTimeOffset AddMonths(int months)
		{
			return new DateTimeOffset(dt.AddMonths(months), utc_offset);
		}

		public DateTimeOffset AddSeconds(double seconds)
		{
			return new DateTimeOffset(dt.AddSeconds(seconds), utc_offset);
		}

		public DateTimeOffset AddTicks(long ticks)
		{
			return new DateTimeOffset(dt.AddTicks(ticks), utc_offset);
		}

		public DateTimeOffset AddYears(int years)
		{
			return new DateTimeOffset(dt.AddYears(years), utc_offset);
		}

		public static int Compare(DateTimeOffset first, DateTimeOffset second)
		{
			return first.CompareTo(second);
		}

		public int CompareTo(DateTimeOffset other)
		{
			return UtcDateTime.CompareTo(other.UtcDateTime);
		}

		public bool Equals(DateTimeOffset other)
		{
			return UtcDateTime == other.UtcDateTime;
		}

		public override bool Equals(object obj)
		{
			if (obj is DateTimeOffset)
			{
				return UtcDateTime == ((DateTimeOffset)obj).UtcDateTime;
			}
			return false;
		}

		public static bool Equals(DateTimeOffset first, DateTimeOffset second)
		{
			return first.Equals(second);
		}

		public bool EqualsExact(DateTimeOffset other)
		{
			return dt == other.dt && utc_offset == other.utc_offset;
		}

		public static DateTimeOffset FromFileTime(long fileTime)
		{
			if (fileTime < 0 || fileTime > MaxValue.Ticks)
			{
				throw new ArgumentOutOfRangeException("fileTime is less than zero or greater than DateTimeOffset.MaxValue.Ticks.");
			}
			return new DateTimeOffset(DateTime.FromFileTime(fileTime), TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.FromFileTime(fileTime)));
		}

		public override int GetHashCode()
		{
			return dt.GetHashCode() ^ utc_offset.GetHashCode();
		}

		public static DateTimeOffset Parse(string input)
		{
			return Parse(input, null);
		}

		public static DateTimeOffset Parse(string input, IFormatProvider formatProvider)
		{
			return Parse(input, formatProvider, DateTimeStyles.AllowWhiteSpaces);
		}

		public static DateTimeOffset Parse(string input, IFormatProvider formatProvider, DateTimeStyles styles)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			Exception exception = null;
			DateTime result;
			DateTimeOffset dto;
			if (!DateTime.CoreParse(input, formatProvider, styles, out result, out dto, true, ref exception))
			{
				throw exception;
			}
			if (result.Ticks != 0L && dto.Ticks == 0L)
			{
				throw new ArgumentOutOfRangeException("The UTC representation falls outside the 1-9999 year range");
			}
			return dto;
		}

		public static DateTimeOffset ParseExact(string input, string format, IFormatProvider formatProvider)
		{
			return ParseExact(input, format, formatProvider, DateTimeStyles.AssumeLocal);
		}

		public static DateTimeOffset ParseExact(string input, string format, IFormatProvider formatProvider, DateTimeStyles styles)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (format == string.Empty)
			{
				throw new FormatException("format is an empty string");
			}
			return ParseExact(input, new string[1] { format }, formatProvider, styles);
		}

		public static DateTimeOffset ParseExact(string input, string[] formats, IFormatProvider formatProvider, DateTimeStyles styles)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (input == string.Empty)
			{
				throw new FormatException("input is an empty string");
			}
			if (formats == null)
			{
				throw new ArgumentNullException("formats");
			}
			if (formats.Length == 0)
			{
				throw new FormatException("Invalid format specifier");
			}
			if ((styles & DateTimeStyles.AssumeLocal) != DateTimeStyles.None && (styles & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None)
			{
				throw new ArgumentException("styles parameter contains incompatible flags");
			}
			DateTimeOffset ret;
			if (!ParseExact(input, formats, DateTimeFormatInfo.GetInstance(formatProvider), styles, out ret))
			{
				throw new FormatException("Invalid format string");
			}
			return ret;
		}

		private static bool ParseExact(string input, string[] formats, DateTimeFormatInfo dfi, DateTimeStyles styles, out DateTimeOffset ret)
		{
			foreach (string text in formats)
			{
				if (text == null || text == string.Empty)
				{
					throw new FormatException("Invalid format string");
				}
				DateTimeOffset result;
				if (DoParse(input, text, false, out result, dfi, styles))
				{
					ret = result;
					return true;
				}
			}
			ret = MinValue;
			return false;
		}

		private static bool DoParse(string input, string format, bool exact, out DateTimeOffset result, DateTimeFormatInfo dfi, DateTimeStyles styles)
		{
			if ((styles & DateTimeStyles.AllowLeadingWhite) != DateTimeStyles.None)
			{
				format = format.TrimStart(null);
				input = input.TrimStart(null);
			}
			if ((styles & DateTimeStyles.AllowTrailingWhite) != DateTimeStyles.None)
			{
				format = format.TrimEnd(null);
				input = input.TrimEnd(null);
			}
			bool allow_leading_white = false;
			if ((styles & DateTimeStyles.AllowInnerWhite) != DateTimeStyles.None)
			{
				allow_leading_white = true;
			}
			bool useutc = false;
			bool use_invariant = false;
			if (format.Length == 1)
			{
				format = DateTimeUtils.GetStandardPattern(format[0], dfi, out useutc, out use_invariant, true);
			}
			int result2 = -1;
			int result3 = -1;
			int result4 = -1;
			int num = -1;
			int result5 = -1;
			int result6 = -1;
			int result7 = -1;
			double num2 = -1.0;
			int result8 = -1;
			TimeSpan timeSpan = TimeSpan.MinValue;
			result = MinValue;
			int i = 0;
			int num3 = 0;
			int num4;
			for (; i < format.Length; i += num4)
			{
				char c = format[i];
				switch (c)
				{
				case 'd':
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					if (result4 != -1 || num4 > 4)
					{
						return false;
					}
					num3 = ((num4 > 2) ? (num3 + ParseEnum(input, num3, (num4 != 3) ? dfi.DayNames : dfi.AbbreviatedDayNames, allow_leading_white, out result8)) : (num3 + ParseNumber(input, num3, 2, num4 == 2, allow_leading_white, out result4)));
					break;
				case 'f':
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					num3 += ParseNumber(input, num3, num4, true, allow_leading_white, out result8);
					if (num2 >= 0.0 || num4 > 7 || result8 == -1)
					{
						return false;
					}
					num2 = (double)result8 / Math.Pow(10.0, num4);
					break;
				case 'F':
				{
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					int digit_parsed;
					int num5 = ParseNumber(input, num3, num4, true, allow_leading_white, out result8, out digit_parsed);
					num3 = ((result8 != -1) ? (num3 + num5) : (num3 + ParseNumber(input, num3, digit_parsed, true, allow_leading_white, out result8)));
					if (num2 >= 0.0 || num4 > 7 || result8 == -1)
					{
						return false;
					}
					num2 = (double)result8 / Math.Pow(10.0, digit_parsed);
					break;
				}
				case 'h':
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					if (result5 != -1 || num4 > 2)
					{
						return false;
					}
					num3 += ParseNumber(input, num3, 2, num4 == 2, allow_leading_white, out result8);
					if (result8 == -1)
					{
						return false;
					}
					if (num == -1)
					{
						num = result8;
					}
					else
					{
						result5 = num + result8;
					}
					break;
				case 'H':
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					if (result5 != -1 || num4 > 2)
					{
						return false;
					}
					num3 += ParseNumber(input, num3, 2, num4 == 2, allow_leading_white, out result5);
					break;
				case 'm':
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					if (result6 != -1 || num4 > 2)
					{
						return false;
					}
					num3 += ParseNumber(input, num3, 2, num4 == 2, allow_leading_white, out result6);
					break;
				case 'M':
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					if (result3 != -1 || num4 > 4)
					{
						return false;
					}
					if (num4 <= 2)
					{
						num3 += ParseNumber(input, num3, 2, num4 == 2, allow_leading_white, out result3);
						break;
					}
					num3 += ParseEnum(input, num3, (num4 != 3) ? dfi.MonthNames : dfi.AbbreviatedMonthNames, allow_leading_white, out result3);
					result3++;
					break;
				case 's':
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					if (result7 != -1 || num4 > 2)
					{
						return false;
					}
					num3 += ParseNumber(input, num3, 2, num4 == 2, allow_leading_white, out result7);
					break;
				case 't':
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					if (result5 != -1 || num4 > 2)
					{
						return false;
					}
					num3 += ParseEnum(input, num3, (num4 == 1) ? new string[2]
					{
						new string(dfi.AMDesignator[0], 1),
						new string(dfi.PMDesignator[0], 0)
					} : new string[2] { dfi.AMDesignator, dfi.PMDesignator }, allow_leading_white, out result8);
					if (result8 == -1)
					{
						return false;
					}
					if (num == -1)
					{
						num = result8 * 12;
					}
					else
					{
						result5 = num + result8 * 12;
					}
					break;
				case 'y':
					if (result2 != -1)
					{
						return false;
					}
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					if (num4 <= 2)
					{
						num3 += ParseNumber(input, num3, 2, num4 == 2, allow_leading_white, out result2);
						if (result2 != -1)
						{
							result2 += DateTime.Now.Year - DateTime.Now.Year % 100;
						}
					}
					else if (num4 <= 4)
					{
						int digit_parsed2;
						num3 += ParseNumber(input, num3, 5, false, allow_leading_white, out result2, out digit_parsed2);
						if (digit_parsed2 < num4 || (digit_parsed2 > num4 && (double)result2 / Math.Pow(10.0, digit_parsed2 - 1) < 1.0))
						{
							return false;
						}
					}
					else
					{
						num3 += ParseNumber(input, num3, num4, true, allow_leading_white, out result2);
					}
					break;
				case 'z':
				{
					num4 = DateTimeUtils.CountRepeat(format, i, c);
					if (timeSpan != TimeSpan.MinValue || num4 > 3)
					{
						return false;
					}
					int result9 = 0;
					result8 = 0;
					int result10;
					num3 += ParseEnum(input, num3, new string[2] { "-", "+" }, allow_leading_white, out result10);
					int result11;
					num3 += ParseNumber(input, num3, 2, num4 != 1, false, out result11);
					if (num4 == 3)
					{
						num3 += ParseEnum(input, num3, new string[1] { dfi.TimeSeparator }, false, out result8);
						num3 += ParseNumber(input, num3, 2, true, false, out result9);
					}
					if (result11 == -1 || result9 == -1 || result10 == -1)
					{
						return false;
					}
					if (result10 == 0)
					{
						result10 = -1;
					}
					timeSpan = new TimeSpan(result10 * result11, result10 * result9, 0);
					break;
				}
				case ':':
					num4 = 1;
					num3 += ParseEnum(input, num3, new string[1] { dfi.TimeSeparator }, false, out result8);
					if (result8 == -1)
					{
						return false;
					}
					break;
				case '/':
					num4 = 1;
					num3 += ParseEnum(input, num3, new string[1] { dfi.DateSeparator }, false, out result8);
					if (result8 == -1)
					{
						return false;
					}
					break;
				case '%':
					num4 = 1;
					if (i != 0)
					{
						return false;
					}
					break;
				case ' ':
					num4 = 1;
					num3 += ParseChar(input, num3, ' ', false, out result8);
					if (result8 == -1)
					{
						return false;
					}
					break;
				case '\\':
					num4 = 2;
					num3 += ParseChar(input, num3, format[i + 1], allow_leading_white, out result8);
					if (result8 == -1)
					{
						return false;
					}
					break;
				default:
					num4 = 1;
					num3 += ParseChar(input, num3, format[i], allow_leading_white, out result8);
					if (result8 == -1)
					{
						return false;
					}
					break;
				}
			}
			if (timeSpan == TimeSpan.MinValue && (styles & DateTimeStyles.AssumeLocal) != DateTimeStyles.None)
			{
				timeSpan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
			}
			if (timeSpan == TimeSpan.MinValue && (styles & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None)
			{
				timeSpan = TimeSpan.Zero;
			}
			if (result5 < 0)
			{
				result5 = 0;
			}
			if (result6 < 0)
			{
				result6 = 0;
			}
			if (result7 < 0)
			{
				result7 = 0;
			}
			if (num2 < 0.0)
			{
				num2 = 0.0;
			}
			if (result2 > 0 && result3 > 0 && result4 > 0)
			{
				result = new DateTimeOffset(result2, result3, result4, result5, result6, result7, 0, timeSpan);
				result = result.AddSeconds(num2);
				if ((styles & DateTimeStyles.AdjustToUniversal) != DateTimeStyles.None)
				{
					result = result.ToUniversalTime();
				}
				return true;
			}
			return false;
		}

		private static int ParseNumber(string input, int pos, int digits, bool leading_zero, bool allow_leading_white, out int result)
		{
			int digit_parsed;
			return ParseNumber(input, pos, digits, leading_zero, allow_leading_white, out result, out digit_parsed);
		}

		private static int ParseNumber(string input, int pos, int digits, bool leading_zero, bool allow_leading_white, out int result, out int digit_parsed)
		{
			int num = 0;
			digit_parsed = 0;
			result = 0;
			while (allow_leading_white && pos < input.Length && input[pos] == ' ')
			{
				num++;
				pos++;
			}
			while (pos < input.Length && char.IsDigit(input[pos]) && digits > 0)
			{
				result = 10 * result + (byte)(input[pos] - 48);
				pos++;
				num++;
				digit_parsed++;
				digits--;
			}
			if (leading_zero && digits > 0)
			{
				result = -1;
			}
			if (digit_parsed == 0)
			{
				result = -1;
			}
			return num;
		}

		private static int ParseEnum(string input, int pos, string[] enums, bool allow_leading_white, out int result)
		{
			int num = 0;
			result = -1;
			while (allow_leading_white && pos < input.Length && input[pos] == ' ')
			{
				num++;
				pos++;
			}
			for (int i = 0; i < enums.Length; i++)
			{
				if (input.Substring(pos).StartsWith(enums[i]))
				{
					result = i;
					break;
				}
			}
			if (result >= 0)
			{
				num += enums[result].Length;
			}
			return num;
		}

		private static int ParseChar(string input, int pos, char c, bool allow_leading_white, out int result)
		{
			int num = 0;
			result = -1;
			while (allow_leading_white && pos < input.Length && input[pos] == ' ')
			{
				pos++;
				num++;
			}
			if (pos < input.Length && input[pos] == c)
			{
				result = c;
				num++;
			}
			return num;
		}

		public TimeSpan Subtract(DateTimeOffset value)
		{
			return UtcDateTime - value.UtcDateTime;
		}

		public DateTimeOffset Subtract(TimeSpan value)
		{
			return Add(-value);
		}

		public long ToFileTime()
		{
			return UtcDateTime.ToFileTime();
		}

		public DateTimeOffset ToLocalTime()
		{
			return new DateTimeOffset(UtcDateTime.ToLocalTime(), TimeZone.CurrentTimeZone.GetUtcOffset(UtcDateTime.ToLocalTime()));
		}

		public DateTimeOffset ToOffset(TimeSpan offset)
		{
			return new DateTimeOffset(dt - utc_offset + offset, offset);
		}

		public override string ToString()
		{
			return ToString(null, null);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return ToString(null, formatProvider);
		}

		public string ToString(string format)
		{
			return ToString(format, null);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(formatProvider);
			if (format == null || format == string.Empty)
			{
				format = instance.ShortDatePattern + " " + instance.LongTimePattern + " zzz";
			}
			bool useutc = false;
			bool use_invariant = false;
			if (format.Length == 1)
			{
				char format2 = format[0];
				try
				{
					format = DateTimeUtils.GetStandardPattern(format2, instance, out useutc, out use_invariant, true);
				}
				catch
				{
					format = null;
				}
				if (format == null)
				{
					throw new FormatException("format is not one of the format specifier characters defined for DateTimeFormatInfo");
				}
			}
			return (!useutc) ? DateTimeUtils.ToString(DateTime, Offset, format, instance) : DateTimeUtils.ToString(UtcDateTime, TimeSpan.Zero, format, instance);
		}

		public DateTimeOffset ToUniversalTime()
		{
			return new DateTimeOffset(UtcDateTime, TimeSpan.Zero);
		}

		public static bool TryParse(string input, out DateTimeOffset result)
		{
			try
			{
				result = Parse(input);
				return true;
			}
			catch
			{
				result = MinValue;
				return false;
			}
		}

		public static bool TryParse(string input, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
		{
			try
			{
				result = Parse(input, formatProvider, styles);
				return true;
			}
			catch
			{
				result = MinValue;
				return false;
			}
		}

		public static bool TryParseExact(string input, string format, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
		{
			try
			{
				result = ParseExact(input, format, formatProvider, styles);
				return true;
			}
			catch
			{
				result = MinValue;
				return false;
			}
		}

		public static bool TryParseExact(string input, string[] formats, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
		{
			try
			{
				result = ParseExact(input, formats, formatProvider, styles);
				return true;
			}
			catch
			{
				result = MinValue;
				return false;
			}
		}

		public static DateTimeOffset operator +(DateTimeOffset dateTimeTz, TimeSpan timeSpan)
		{
			return dateTimeTz.Add(timeSpan);
		}

		public static bool operator ==(DateTimeOffset left, DateTimeOffset right)
		{
			return left.Equals(right);
		}

		public static bool operator >(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime > right.UtcDateTime;
		}

		public static bool operator >=(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime >= right.UtcDateTime;
		}

		public static implicit operator DateTimeOffset(DateTime dateTime)
		{
			return new DateTimeOffset(dateTime);
		}

		public static bool operator !=(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime != right.UtcDateTime;
		}

		public static bool operator <(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime < right.UtcDateTime;
		}

		public static bool operator <=(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime <= right.UtcDateTime;
		}

		public static TimeSpan operator -(DateTimeOffset left, DateTimeOffset right)
		{
			return left.Subtract(right);
		}

		public static DateTimeOffset operator -(DateTimeOffset dateTimeTz, TimeSpan timeSpan)
		{
			return dateTimeTz.Subtract(timeSpan);
		}
	}
}
