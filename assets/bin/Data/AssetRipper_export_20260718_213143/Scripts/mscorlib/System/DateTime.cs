using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public struct DateTime : IFormattable, IConvertible, IComparable, IComparable<DateTime>, IEquatable<DateTime>
	{
		private enum Which
		{
			Day = 0,
			DayYear = 1,
			Month = 2,
			Year = 3
		}

		private const int dp400 = 146097;

		private const int dp100 = 36524;

		private const int dp4 = 1461;

		private const long w32file_epoch = 504911232000000000L;

		private const long MAX_VALUE_TICKS = 3155378975999999999L;

		internal const long UnixEpoch = 621355968000000000L;

		private const long ticks18991230 = 599264352000000000L;

		private const double OAMinValue = -657435.0;

		private const double OAMaxValue = 2958466.0;

		private const string formatExceptionMessage = "String was not recognized as a valid DateTime.";

		private TimeSpan ticks;

		private DateTimeKind kind;

		public static readonly DateTime MaxValue;

		public static readonly DateTime MinValue;

		private static readonly string[] ParseTimeFormats;

		private static readonly string[] ParseYearDayMonthFormats;

		private static readonly string[] ParseYearMonthDayFormats;

		private static readonly string[] ParseDayMonthYearFormats;

		private static readonly string[] ParseMonthDayYearFormats;

		private static readonly string[] MonthDayShortFormats;

		private static readonly string[] DayMonthShortFormats;

		private static readonly int[] daysmonth;

		private static readonly int[] daysmonthleap;

		private static object to_local_time_span_object;

		private static long last_now;

		public DateTime Date
		{
			get
			{
				DateTime result = new DateTime(Year, Month, Day);
				result.kind = kind;
				return result;
			}
		}

		public int Month
		{
			get
			{
				return FromTicks(Which.Month);
			}
		}

		public int Day
		{
			get
			{
				return FromTicks(Which.Day);
			}
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return (DayOfWeek)((ticks.Days + 1) % 7);
			}
		}

		public int DayOfYear
		{
			get
			{
				return FromTicks(Which.DayYear);
			}
		}

		public TimeSpan TimeOfDay
		{
			get
			{
				return new TimeSpan(ticks.Ticks % 864000000000L);
			}
		}

		public int Hour
		{
			get
			{
				return ticks.Hours;
			}
		}

		public int Minute
		{
			get
			{
				return ticks.Minutes;
			}
		}

		public int Second
		{
			get
			{
				return ticks.Seconds;
			}
		}

		public int Millisecond
		{
			get
			{
				return ticks.Milliseconds;
			}
		}

		public static DateTime Now
		{
			get
			{
				long now = GetNow();
				DateTime dateTime = new DateTime(now);
				if (now - last_now > 600000000)
				{
					to_local_time_span_object = TimeZone.CurrentTimeZone.GetLocalTimeDiff(dateTime);
					last_now = now;
				}
				DateTime result = dateTime + (TimeSpan)to_local_time_span_object;
				result.kind = DateTimeKind.Local;
				return result;
			}
		}

		public long Ticks
		{
			get
			{
				return ticks.Ticks;
			}
		}

		public static DateTime Today
		{
			get
			{
				DateTime now = Now;
				DateTime result = new DateTime(now.Year, now.Month, now.Day);
				result.kind = now.kind;
				return result;
			}
		}

		public static DateTime UtcNow
		{
			get
			{
				return new DateTime(GetNow(), DateTimeKind.Utc);
			}
		}

		public int Year
		{
			get
			{
				return FromTicks(Which.Year);
			}
		}

		public DateTimeKind Kind
		{
			get
			{
				return kind;
			}
		}

		public DateTime(long ticks)
		{
			this.ticks = new TimeSpan(ticks);
			if (ticks < MinValue.Ticks || ticks > MaxValue.Ticks)
			{
				string text = Locale.GetText("Value {0} is outside the valid range [{1},{2}].", ticks, MinValue.Ticks, MaxValue.Ticks);
				throw new ArgumentOutOfRangeException("ticks", text);
			}
			kind = DateTimeKind.Unspecified;
		}

		public DateTime(int year, int month, int day)
			: this(year, month, day, 0, 0, 0, 0)
		{
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second)
			: this(year, month, day, hour, minute, second, 0)
		{
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			if (year < 1 || year > 9999 || month < 1 || month > 12 || day < 1 || day > DaysInMonth(year, month) || hour < 0 || hour > 23 || minute < 0 || minute > 59 || second < 0 || second > 59 || millisecond < 0 || millisecond > 999)
			{
				throw new ArgumentOutOfRangeException("Parameters describe an unrepresentable DateTime.");
			}
			ticks = new TimeSpan(AbsoluteDays(year, month, day), hour, minute, second, millisecond);
			kind = DateTimeKind.Unspecified;
		}

		public DateTime(int year, int month, int day, Calendar calendar)
			: this(year, month, day, 0, 0, 0, 0, calendar)
		{
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, Calendar calendar)
			: this(year, month, day, hour, minute, second, 0, calendar)
		{
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar)
		{
			if (calendar == null)
			{
				throw new ArgumentNullException("calendar");
			}
			ticks = calendar.ToDateTime(year, month, day, hour, minute, second, millisecond).ticks;
			kind = DateTimeKind.Unspecified;
		}

		internal DateTime(bool check, TimeSpan value)
		{
			if (check && (value.Ticks < MinValue.Ticks || value.Ticks > MaxValue.Ticks))
			{
				throw new ArgumentOutOfRangeException();
			}
			ticks = value;
			kind = DateTimeKind.Unspecified;
		}

		public DateTime(long ticks, DateTimeKind kind)
			: this(ticks)
		{
			CheckDateTimeKind(kind);
			this.kind = kind;
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
			: this(year, month, day, hour, minute, second)
		{
			CheckDateTimeKind(kind);
			this.kind = kind;
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
			: this(year, month, day, hour, minute, second, millisecond)
		{
			CheckDateTimeKind(kind);
			this.kind = kind;
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, DateTimeKind kind)
			: this(year, month, day, hour, minute, second, millisecond, calendar)
		{
			CheckDateTimeKind(kind);
			this.kind = kind;
		}

		static DateTime()
		{
			MaxValue = new DateTime(false, new TimeSpan(3155378975999999999L));
			MinValue = new DateTime(false, new TimeSpan(0L));
			ParseTimeFormats = new string[9] { "H:m:s.fffffffzzz", "H:m:s.fffffff", "H:m:s tt zzz", "H:m:szzz", "H:m:s", "H:mzzz", "H:m", "H tt", "H'時'm'分's'秒'" };
			ParseYearDayMonthFormats = new string[10] { "yyyy/M/dT", "M/yyyy/dT", "yyyy'年'M'月'd'日", "yyyy/d/MMMM", "yyyy/MMM/d", "d/MMMM/yyyy", "MMM/d/yyyy", "d/yyyy/MMMM", "MMM/yyyy/d", "yy/d/M" };
			ParseYearMonthDayFormats = new string[12]
			{
				"yyyy/M/dT", "M/yyyy/dT", "yyyy'年'M'月'd'日", "yyyy/MMMM/d", "yyyy/d/MMM", "MMMM/d/yyyy", "d/MMM/yyyy", "MMMM/yyyy/d", "d/yyyy/MMM", "yy/MMMM/d",
				"yy/d/MMM", "MMM/yy/d"
			};
			ParseDayMonthYearFormats = new string[15]
			{
				"yyyy/M/dT", "M/yyyy/dT", "yyyy'年'M'月'd'日", "yyyy/MMMM/d", "yyyy/d/MMM", "d/MMMM/yyyy", "MMM/d/yyyy", "MMMM/yyyy/d", "d/yyyy/MMM", "d/MMMM/yy",
				"yy/MMM/d", "d/yy/MMM", "yy/d/MMM", "MMM/d/yy", "MMM/yy/d"
			};
			ParseMonthDayYearFormats = new string[15]
			{
				"yyyy/M/dT", "M/yyyy/dT", "yyyy'年'M'月'd'日", "yyyy/MMMM/d", "yyyy/d/MMM", "MMMM/d/yyyy", "d/MMM/yyyy", "MMMM/yyyy/d", "d/yyyy/MMM", "MMMM/d/yy",
				"MMM/yy/d", "d/MMM/yy", "yy/MMM/d", "d/yy/MMM", "yy/d/MMM"
			};
			MonthDayShortFormats = new string[3] { "MMMM/d", "d/MMM", "yyyy/MMMM" };
			DayMonthShortFormats = new string[3] { "d/MMMM", "MMM/yy", "yyyy/MMMM" };
			daysmonth = new int[13]
			{
				0, 31, 28, 31, 30, 31, 30, 31, 31, 30,
				31, 30, 31
			};
			daysmonthleap = new int[13]
			{
				0, 31, 29, 31, 30, 31, 30, 31, 31, 30,
				31, 30, 31
			};
			if (MonoTouchAOTHelper.FalseFlag)
			{
				GenericComparer<DateTime> genericComparer = new GenericComparer<DateTime>();
				GenericEqualityComparer<DateTime> genericEqualityComparer = new GenericEqualityComparer<DateTime>();
			}
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return this;
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (targetType == typeof(DateTime))
			{
				return this;
			}
			if (targetType == typeof(string))
			{
				return ToString(provider);
			}
			if (targetType == typeof(object))
			{
				return this;
			}
			throw new InvalidCastException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		private static int AbsoluteDays(int year, int month, int day)
		{
			int num = 0;
			int num2 = 1;
			int[] array = ((!IsLeapYear(year)) ? daysmonth : daysmonthleap);
			while (num2 < month)
			{
				num += array[num2++];
			}
			return day - 1 + num + 365 * (year - 1) + (year - 1) / 4 - (year - 1) / 100 + (year - 1) / 400;
		}

		private int FromTicks(Which what)
		{
			int num = 1;
			int[] array = daysmonth;
			int days = ticks.Days;
			int num2 = days / 146097;
			days -= num2 * 146097;
			int num3 = days / 36524;
			if (num3 == 4)
			{
				num3 = 3;
			}
			days -= num3 * 36524;
			int num4 = days / 1461;
			days -= num4 * 1461;
			int num5 = days / 365;
			if (num5 == 4)
			{
				num5 = 3;
			}
			if (what == Which.Year)
			{
				return num2 * 400 + num3 * 100 + num4 * 4 + num5 + 1;
			}
			days -= num5 * 365;
			if (what == Which.DayYear)
			{
				return days + 1;
			}
			if (num5 == 3 && (num3 == 3 || num4 != 24))
			{
				array = daysmonthleap;
			}
			while (days >= array[num])
			{
				days -= array[num++];
			}
			if (what == Which.Month)
			{
				return num;
			}
			return days + 1;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long GetTimeMonotonic();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long GetNow();

		public DateTime Add(TimeSpan value)
		{
			DateTime result = AddTicks(value.Ticks);
			result.kind = kind;
			return result;
		}

		public DateTime AddDays(double value)
		{
			return AddMilliseconds(Math.Round(value * 86400000.0));
		}

		public DateTime AddTicks(long value)
		{
			if (value + ticks.Ticks > 3155378975999999999L || value + ticks.Ticks < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			DateTime result = new DateTime(value + ticks.Ticks);
			result.kind = kind;
			return result;
		}

		public DateTime AddHours(double value)
		{
			return AddMilliseconds(value * 3600000.0);
		}

		public DateTime AddMilliseconds(double value)
		{
			if (value * 10000.0 > 9.223372036854776E+18 || value * 10000.0 < -9.223372036854776E+18)
			{
				throw new ArgumentOutOfRangeException();
			}
			long value2 = (long)Math.Round(value * 10000.0);
			return AddTicks(value2);
		}

		private DateTime AddRoundedMilliseconds(double ms)
		{
			if (ms * 10000.0 > 9.223372036854776E+18 || ms * 10000.0 < -9.223372036854776E+18)
			{
				throw new ArgumentOutOfRangeException();
			}
			long value = (long)(ms += ((!(ms > 0.0)) ? (-0.5) : 0.5)) * 10000;
			return AddTicks(value);
		}

		public DateTime AddMinutes(double value)
		{
			return AddMilliseconds(value * 60000.0);
		}

		public DateTime AddMonths(int months)
		{
			int num = Day;
			int num2 = Month + months % 12;
			int num3 = Year + months / 12;
			if (num2 < 1)
			{
				num2 = 12 + num2;
				num3--;
			}
			else if (num2 > 12)
			{
				num2 -= 12;
				num3++;
			}
			int num4 = DaysInMonth(num3, num2);
			if (num > num4)
			{
				num = num4;
			}
			DateTime dateTime = new DateTime(num3, num2, num);
			dateTime.kind = kind;
			return dateTime.Add(TimeOfDay);
		}

		public DateTime AddSeconds(double value)
		{
			return AddMilliseconds(value * 1000.0);
		}

		public DateTime AddYears(int value)
		{
			return AddMonths(value * 12);
		}

		public static int Compare(DateTime t1, DateTime t2)
		{
			if (t1.ticks < t2.ticks)
			{
				return -1;
			}
			if (t1.ticks > t2.ticks)
			{
				return 1;
			}
			return 0;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is DateTime))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.DateTime"));
			}
			return Compare(this, (DateTime)value);
		}

		public bool IsDaylightSavingTime()
		{
			if (kind == DateTimeKind.Utc)
			{
				return false;
			}
			return TimeZone.CurrentTimeZone.IsDaylightSavingTime(this);
		}

		public int CompareTo(DateTime value)
		{
			return Compare(this, value);
		}

		public bool Equals(DateTime value)
		{
			return value.ticks == ticks;
		}

		public long ToBinary()
		{
			switch (kind)
			{
			case DateTimeKind.Utc:
				return Ticks | 0x4000000000000000L;
			case DateTimeKind.Local:
				return ToUniversalTime().Ticks | long.MinValue;
			default:
				return Ticks;
			}
		}

		public static DateTime FromBinary(long dateData)
		{
			switch ((ulong)dateData >> 62)
			{
			case 1uL:
				return new DateTime(dateData ^ 0x4000000000000000L, DateTimeKind.Utc);
			case 0uL:
				return new DateTime(dateData, DateTimeKind.Unspecified);
			default:
				return new DateTime(dateData & 0x3FFFFFFFFFFFFFFFL, DateTimeKind.Utc).ToLocalTime();
			}
		}

		public static DateTime SpecifyKind(DateTime value, DateTimeKind kind)
		{
			return new DateTime(value.Ticks, kind);
		}

		public static int DaysInMonth(int year, int month)
		{
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException();
			}
			int[] array = ((!IsLeapYear(year)) ? daysmonth : daysmonthleap);
			return array[month];
		}

		public override bool Equals(object value)
		{
			if (!(value is DateTime))
			{
				return false;
			}
			return ((DateTime)value).ticks == ticks;
		}

		public static bool Equals(DateTime t1, DateTime t2)
		{
			return t1.ticks == t2.ticks;
		}

		public static DateTime FromFileTime(long fileTime)
		{
			if (fileTime < 0)
			{
				throw new ArgumentOutOfRangeException("fileTime", "< 0");
			}
			return new DateTime(504911232000000000L + fileTime).ToLocalTime();
		}

		public static DateTime FromFileTimeUtc(long fileTime)
		{
			if (fileTime < 0)
			{
				throw new ArgumentOutOfRangeException("fileTime", "< 0");
			}
			return new DateTime(504911232000000000L + fileTime);
		}

		public static DateTime FromOADate(double d)
		{
			if (d <= -657435.0 || d >= 2958466.0)
			{
				throw new ArgumentException("d", "[-657435,2958466]");
			}
			DateTime dateTime = new DateTime(599264352000000000L);
			if (d < 0.0)
			{
				double num = Math.Ceiling(d);
				dateTime = dateTime.AddRoundedMilliseconds(num * 86400000.0);
				double num2 = num - d;
				return dateTime.AddRoundedMilliseconds(num2 * 86400000.0);
			}
			return dateTime.AddRoundedMilliseconds(d * 86400000.0);
		}

		public string[] GetDateTimeFormats()
		{
			return GetDateTimeFormats(CultureInfo.CurrentCulture);
		}

		public string[] GetDateTimeFormats(char format)
		{
			if ("dDgGfFmMrRstTuUyY".IndexOf(format) < 0)
			{
				throw new FormatException("Invalid format character.");
			}
			return new string[1] { ToString(format.ToString()) };
		}

		public string[] GetDateTimeFormats(IFormatProvider provider)
		{
			DateTimeFormatInfo provider2 = (DateTimeFormatInfo)provider.GetFormat(typeof(DateTimeFormatInfo));
			ArrayList arrayList = new ArrayList();
			string text = "dDgGfFmMrRstTuUyY";
			foreach (char format in text)
			{
				arrayList.AddRange(GetDateTimeFormats(format, provider2));
			}
			return arrayList.ToArray(typeof(string)) as string[];
		}

		public string[] GetDateTimeFormats(char format, IFormatProvider provider)
		{
			if ("dDgGfFmMrRstTuUyY".IndexOf(format) < 0)
			{
				throw new FormatException("Invalid format character.");
			}
			bool adjustutc = false;
			char c = format;
			if (c == 'U')
			{
				adjustutc = true;
			}
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)provider.GetFormat(typeof(DateTimeFormatInfo));
			return GetDateTimeFormats(adjustutc, dateTimeFormatInfo.GetAllRawDateTimePatterns(format), dateTimeFormatInfo);
		}

		private string[] GetDateTimeFormats(bool adjustutc, string[] patterns, DateTimeFormatInfo dfi)
		{
			string[] array = new string[patterns.Length];
			DateTime dateTime = ((!adjustutc) ? this : ToUniversalTime());
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = DateTimeUtils.ToString(dateTime, patterns[i], dfi);
			}
			return array;
		}

		private void CheckDateTimeKind(DateTimeKind kind)
		{
			if (kind != DateTimeKind.Unspecified && kind != DateTimeKind.Utc && kind != DateTimeKind.Local)
			{
				throw new ArgumentException("Invalid DateTimeKind value.", "kind");
			}
		}

		public override int GetHashCode()
		{
			return (int)ticks.Ticks;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.DateTime;
		}

		public static bool IsLeapYear(int year)
		{
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException();
			}
			return (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;
		}

		public static DateTime Parse(string s)
		{
			return Parse(s, null);
		}

		public static DateTime Parse(string s, IFormatProvider provider)
		{
			return Parse(s, provider, DateTimeStyles.AllowWhiteSpaces);
		}

		public static DateTime Parse(string s, IFormatProvider provider, DateTimeStyles styles)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			Exception exception = null;
			DateTime result;
			DateTimeOffset dto;
			if (!CoreParse(s, provider, styles, out result, out dto, true, ref exception))
			{
				throw exception;
			}
			return result;
		}

		internal static bool CoreParse(string s, IFormatProvider provider, DateTimeStyles styles, out DateTime result, out DateTimeOffset dto, bool setExceptionOnError, ref Exception exception)
		{
			dto = new DateTimeOffset(0L, TimeSpan.Zero);
			if (s == null || s.Length == 0)
			{
				if (setExceptionOnError)
				{
					exception = new FormatException("String was not recognized as a valid DateTime.");
				}
				result = MinValue;
				return false;
			}
			if (provider == null)
			{
				provider = CultureInfo.CurrentCulture;
			}
			DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(provider);
			string[] array = YearMonthDayFormats(instance, setExceptionOnError, ref exception);
			if (array == null)
			{
				result = MinValue;
				return false;
			}
			bool longYear = false;
			foreach (string firstPart in array)
			{
				bool incompleteFormat = false;
				if (_DoParse(s, firstPart, string.Empty, false, out result, out dto, instance, styles, true, ref incompleteFormat, ref longYear))
				{
					return true;
				}
				if (!incompleteFormat)
				{
					continue;
				}
				for (int j = 0; j < ParseTimeFormats.Length; j++)
				{
					if (_DoParse(s, firstPart, ParseTimeFormats[j], false, out result, out dto, instance, styles, true, ref incompleteFormat, ref longYear))
					{
						return true;
					}
				}
			}
			int num = instance.MonthDayPattern.IndexOf('d');
			int num2 = instance.MonthDayPattern.IndexOf('M');
			if (num == -1 || num2 == -1)
			{
				result = MinValue;
				if (setExceptionOnError)
				{
					exception = new FormatException(Locale.GetText("Order of month and date is not defined by {0}", instance.MonthDayPattern));
				}
				return false;
			}
			string[] array2 = ((num >= num2) ? MonthDayShortFormats : DayMonthShortFormats);
			for (int k = 0; k < array2.Length; k++)
			{
				bool incompleteFormat2 = false;
				if (_DoParse(s, array2[k], string.Empty, false, out result, out dto, instance, styles, true, ref incompleteFormat2, ref longYear))
				{
					return true;
				}
			}
			for (int l = 0; l < ParseTimeFormats.Length; l++)
			{
				string firstPart2 = ParseTimeFormats[l];
				bool incompleteFormat3 = false;
				if (_DoParse(s, firstPart2, string.Empty, false, out result, out dto, instance, styles, false, ref incompleteFormat3, ref longYear))
				{
					return true;
				}
				if (!incompleteFormat3)
				{
					continue;
				}
				for (int m = 0; m < array2.Length; m++)
				{
					if (_DoParse(s, firstPart2, array2[m], false, out result, out dto, instance, styles, false, ref incompleteFormat3, ref longYear))
					{
						return true;
					}
				}
				foreach (string text in array)
				{
					if (text[text.Length - 1] != 'T' && _DoParse(s, firstPart2, text, false, out result, out dto, instance, styles, false, ref incompleteFormat3, ref longYear))
					{
						return true;
					}
				}
			}
			if (ParseExact(s, instance.GetAllDateTimePatternsInternal(), instance, styles, out result, false, ref longYear, setExceptionOnError, ref exception))
			{
				return true;
			}
			if (!setExceptionOnError)
			{
				return false;
			}
			exception = new FormatException("String was not recognized as a valid DateTime.");
			return false;
		}

		public static DateTime ParseExact(string s, string format, IFormatProvider provider)
		{
			return ParseExact(s, format, provider, DateTimeStyles.None);
		}

		private static string[] YearMonthDayFormats(DateTimeFormatInfo dfi, bool setExceptionOnError, ref Exception exc)
		{
			int num = dfi.ShortDatePattern.IndexOf('d');
			int num2 = dfi.ShortDatePattern.IndexOf('M');
			int num3 = dfi.ShortDatePattern.IndexOf('y');
			if (num == -1 || num2 == -1 || num3 == -1)
			{
				if (setExceptionOnError)
				{
					exc = new FormatException(Locale.GetText("Order of year, month and date is not defined by {0}", dfi.ShortDatePattern));
				}
				return null;
			}
			if (num3 < num2)
			{
				if (num2 < num)
				{
					return ParseYearMonthDayFormats;
				}
				if (num3 < num)
				{
					return ParseYearDayMonthFormats;
				}
				if (setExceptionOnError)
				{
					exc = new FormatException(Locale.GetText("Order of date, year and month defined by {0} is not supported", dfi.ShortDatePattern));
				}
				return null;
			}
			if (num < num2)
			{
				return ParseDayMonthYearFormats;
			}
			if (num < num3)
			{
				return ParseMonthDayYearFormats;
			}
			if (setExceptionOnError)
			{
				exc = new FormatException(Locale.GetText("Order of month, year and date defined by {0} is not supported", dfi.ShortDatePattern));
			}
			return null;
		}

		private static int _ParseNumber(string s, int valuePos, int min_digits, int digits, bool leadingzero, bool sloppy_parsing, out int num_parsed)
		{
			int num = 0;
			if (sloppy_parsing)
			{
				leadingzero = false;
			}
			if (!leadingzero)
			{
				int num2 = 0;
				for (int i = valuePos; i < s.Length && i < digits + valuePos && char.IsDigit(s[i]); i++)
				{
					num2++;
				}
				digits = num2;
			}
			if (digits < min_digits)
			{
				num_parsed = -1;
				return 0;
			}
			if (s.Length - valuePos < digits)
			{
				num_parsed = -1;
				return 0;
			}
			for (int i = valuePos; i < digits + valuePos; i++)
			{
				char c = s[i];
				if (!char.IsDigit(c))
				{
					num_parsed = -1;
					return 0;
				}
				num = num * 10 + (byte)(c - 48);
			}
			num_parsed = digits;
			return num;
		}

		private static int _ParseEnum(string s, int sPos, string[] values, string[] invValues, bool exact, out int num_parsed)
		{
			for (int num = values.Length - 1; num >= 0; num--)
			{
				if (!exact && invValues[num].Length > values[num].Length)
				{
					if (invValues[num].Length > 0 && _ParseString(s, sPos, 0, invValues[num], out num_parsed))
					{
						return num;
					}
					if (values[num].Length > 0 && _ParseString(s, sPos, 0, values[num], out num_parsed))
					{
						return num;
					}
				}
				else
				{
					if (values[num].Length > 0 && _ParseString(s, sPos, 0, values[num], out num_parsed))
					{
						return num;
					}
					if (!exact && invValues[num].Length > 0 && _ParseString(s, sPos, 0, invValues[num], out num_parsed))
					{
						return num;
					}
				}
			}
			num_parsed = -1;
			return -1;
		}

		private static bool _ParseString(string s, int sPos, int maxlength, string value, out int num_parsed)
		{
			if (maxlength <= 0)
			{
				maxlength = value.Length;
			}
			if (sPos + maxlength <= s.Length && string.Compare(s, sPos, value, 0, maxlength, true, CultureInfo.InvariantCulture) == 0)
			{
				num_parsed = maxlength;
				return true;
			}
			num_parsed = -1;
			return false;
		}

		private static bool _ParseAmPm(string s, int valuePos, int num, DateTimeFormatInfo dfi, bool exact, out int num_parsed, ref int ampm)
		{
			num_parsed = -1;
			if (ampm != -1)
			{
				return false;
			}
			if (!IsLetter(s, valuePos))
			{
				if (dfi.AMDesignator != string.Empty)
				{
					return false;
				}
				if (exact)
				{
					ampm = 0;
				}
				num_parsed = 0;
				return true;
			}
			DateTimeFormatInfo invariantInfo = DateTimeFormatInfo.InvariantInfo;
			if ((!exact && _ParseString(s, valuePos, num, invariantInfo.PMDesignator, out num_parsed)) || (dfi.PMDesignator != string.Empty && _ParseString(s, valuePos, num, dfi.PMDesignator, out num_parsed)))
			{
				ampm = 1;
			}
			else
			{
				if ((exact || !_ParseString(s, valuePos, num, invariantInfo.AMDesignator, out num_parsed)) && !_ParseString(s, valuePos, num, dfi.AMDesignator, out num_parsed))
				{
					return false;
				}
				if (exact || num_parsed != 0)
				{
					ampm = 0;
				}
			}
			return true;
		}

		private static bool _ParseTimeSeparator(string s, int sPos, DateTimeFormatInfo dfi, bool exact, out int num_parsed)
		{
			return _ParseString(s, sPos, 0, dfi.TimeSeparator, out num_parsed) || (!exact && _ParseString(s, sPos, 0, ":", out num_parsed));
		}

		private static bool _ParseDateSeparator(string s, int sPos, DateTimeFormatInfo dfi, bool exact, out int num_parsed)
		{
			num_parsed = -1;
			if (exact && s[sPos] != '/')
			{
				return false;
			}
			if (_ParseTimeSeparator(s, sPos, dfi, exact, out num_parsed) || char.IsDigit(s[sPos]) || char.IsLetter(s[sPos]))
			{
				return false;
			}
			num_parsed = 1;
			return true;
		}

		private static bool IsLetter(string s, int pos)
		{
			return pos < s.Length && char.IsLetter(s[pos]);
		}

		private static bool _DoParse(string s, string firstPart, string secondPart, bool exact, out DateTime result, out DateTimeOffset dto, DateTimeFormatInfo dfi, DateTimeStyles style, bool firstPartIsDate, ref bool incompleteFormat, ref bool longYear)
		{
			bool useutc = false;
			bool use_invariant = false;
			bool sloppy_parsing = false;
			dto = new DateTimeOffset(0L, TimeSpan.Zero);
			bool flag = !exact && secondPart != null;
			incompleteFormat = false;
			int num = 0;
			string text = firstPart;
			bool flag2 = false;
			DateTimeFormatInfo invariantInfo = DateTimeFormatInfo.InvariantInfo;
			if (text.Length == 1)
			{
				text = DateTimeUtils.GetStandardPattern(text[0], dfi, out useutc, out use_invariant);
			}
			result = new DateTime(0L);
			if (text == null)
			{
				return false;
			}
			if (s == null)
			{
				return false;
			}
			if ((style & DateTimeStyles.AllowLeadingWhite) != DateTimeStyles.None)
			{
				text = text.TrimStart(null);
				s = s.TrimStart(null);
			}
			if ((style & DateTimeStyles.AllowTrailingWhite) != DateTimeStyles.None)
			{
				text = text.TrimEnd(null);
				s = s.TrimEnd(null);
			}
			if (use_invariant)
			{
				dfi = invariantInfo;
			}
			if ((style & DateTimeStyles.AllowInnerWhite) != DateTimeStyles.None)
			{
				sloppy_parsing = true;
			}
			string text2 = text;
			int length = text.Length;
			int i = 0;
			int num2 = 0;
			if (length == 0)
			{
				return false;
			}
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
			int num9 = -1;
			double num10 = -1.0;
			int ampm = -1;
			int num11 = -1;
			int num12 = -1;
			int num13 = -1;
			bool flag3 = true;
			while (num != s.Length)
			{
				int num_parsed = 0;
				if (flag && i + num2 == 0)
				{
					bool flag4 = IsLetter(s, num);
					if (flag4)
					{
						if (s[num] == 'Z')
						{
							num_parsed = 1;
						}
						else
						{
							_ParseString(s, num, 0, "GMT", out num_parsed);
						}
						if (num_parsed > 0 && !IsLetter(s, num + num_parsed))
						{
							num += num_parsed;
							useutc = true;
							continue;
						}
					}
					if (!flag2 && _ParseAmPm(s, num, 0, dfi, exact, out num_parsed, ref ampm))
					{
						if (IsLetter(s, num + num_parsed))
						{
							ampm = -1;
						}
						else if (num_parsed > 0)
						{
							num += num_parsed;
							continue;
						}
					}
					if (!flag2 && num4 == -1 && flag4)
					{
						num4 = _ParseEnum(s, num, dfi.RawDayNames, invariantInfo.RawDayNames, exact, out num_parsed);
						if (num4 == -1)
						{
							num4 = _ParseEnum(s, num, dfi.RawAbbreviatedDayNames, invariantInfo.RawAbbreviatedDayNames, exact, out num_parsed);
						}
						if (num4 != -1 && !IsLetter(s, num + num_parsed))
						{
							num += num_parsed;
							continue;
						}
						num4 = -1;
					}
					if (char.IsWhiteSpace(s[num]) || s[num] == ',')
					{
						num++;
						continue;
					}
					num_parsed = 0;
				}
				if (i + num2 >= length)
				{
					if (flag && num2 == 0)
					{
						flag2 = flag3 && firstPart[firstPart.Length - 1] == 'T';
						if (!flag3 && text == string.Empty)
						{
							break;
						}
						i = 0;
						text = ((!flag3) ? string.Empty : secondPart);
						text2 = text;
						length = text2.Length;
						flag3 = false;
						continue;
					}
					break;
				}
				bool leadingzero = true;
				if (text2[i] == '\'')
				{
					for (num2 = 1; i + num2 < length && text2[i + num2] != '\''; num2++)
					{
						if (num == s.Length || s[num] != text2[i + num2])
						{
							return false;
						}
						num++;
					}
					i += num2 + 1;
					num2 = 0;
					continue;
				}
				if (text2[i] == '"')
				{
					for (num2 = 1; i + num2 < length && text2[i + num2] != '"'; num2++)
					{
						if (num == s.Length || s[num] != text2[i + num2])
						{
							return false;
						}
						num++;
					}
					i += num2 + 1;
					num2 = 0;
					continue;
				}
				if (text2[i] == '\\')
				{
					i += num2 + 1;
					num2 = 0;
					if (i >= length)
					{
						return false;
					}
					if (s[num] != text2[i])
					{
						return false;
					}
					num++;
					i++;
					continue;
				}
				if (text2[i] == '%')
				{
					i++;
					continue;
				}
				if (char.IsWhiteSpace(s[num]) || (s[num] == ',' && ((!exact && text2[i] == '/') || char.IsWhiteSpace(text2[i]))))
				{
					num++;
					num2 = 0;
					if (exact && (style & DateTimeStyles.AllowInnerWhite) == 0)
					{
						if (!char.IsWhiteSpace(text2[i]))
						{
							return false;
						}
						i++;
						continue;
					}
					int j;
					for (j = num; j < s.Length && (char.IsWhiteSpace(s[j]) || s[j] == ','); j++)
					{
					}
					num = j;
					for (j = i; j < text2.Length && (char.IsWhiteSpace(text2[j]) || text2[j] == ','); j++)
					{
					}
					i = j;
					if (!exact && i < text2.Length && text2[i] == '/' && !_ParseDateSeparator(s, num, dfi, exact, out num_parsed))
					{
						i++;
					}
					continue;
				}
				if (i + num2 + 1 < length && text2[i + num2 + 1] == text2[i + num2])
				{
					num2++;
					continue;
				}
				switch (text2[i])
				{
				case 'd':
					if ((num2 < 2 && num3 != -1) || (num2 >= 2 && num4 != -1))
					{
						return false;
					}
					switch (num2)
					{
					case 0:
						num3 = _ParseNumber(s, num, 1, 2, false, sloppy_parsing, out num_parsed);
						break;
					case 1:
						num3 = _ParseNumber(s, num, 1, 2, true, sloppy_parsing, out num_parsed);
						break;
					case 2:
						num4 = _ParseEnum(s, num, dfi.RawAbbreviatedDayNames, invariantInfo.RawAbbreviatedDayNames, exact, out num_parsed);
						break;
					default:
						num4 = _ParseEnum(s, num, dfi.RawDayNames, invariantInfo.RawDayNames, exact, out num_parsed);
						break;
					}
					break;
				case 'M':
					if (num5 != -1)
					{
						return false;
					}
					if (flag)
					{
						num_parsed = -1;
						if (num2 == 0 || num2 == 3)
						{
							num5 = _ParseNumber(s, num, 1, 2, false, sloppy_parsing, out num_parsed);
						}
						if (num2 > 1 && num_parsed == -1)
						{
							num5 = _ParseEnum(s, num, dfi.RawMonthNames, invariantInfo.RawMonthNames, exact, out num_parsed) + 1;
						}
						if (num2 > 1 && num_parsed == -1)
						{
							num5 = _ParseEnum(s, num, dfi.RawAbbreviatedMonthNames, invariantInfo.RawAbbreviatedMonthNames, exact, out num_parsed) + 1;
						}
						break;
					}
					switch (num2)
					{
					case 0:
						num5 = _ParseNumber(s, num, 1, 2, false, sloppy_parsing, out num_parsed);
						break;
					case 1:
						num5 = _ParseNumber(s, num, 1, 2, true, sloppy_parsing, out num_parsed);
						break;
					case 2:
						num5 = _ParseEnum(s, num, dfi.RawAbbreviatedMonthNames, invariantInfo.RawAbbreviatedMonthNames, exact, out num_parsed) + 1;
						break;
					default:
						num5 = _ParseEnum(s, num, dfi.RawMonthNames, invariantInfo.RawMonthNames, exact, out num_parsed) + 1;
						break;
					}
					break;
				case 'y':
					if (num6 != -1)
					{
						return false;
					}
					if (num2 == 0)
					{
						num6 = _ParseNumber(s, num, 1, 2, false, sloppy_parsing, out num_parsed);
					}
					else if (num2 < 3)
					{
						num6 = _ParseNumber(s, num, 1, 2, true, sloppy_parsing, out num_parsed);
					}
					else
					{
						num6 = _ParseNumber(s, num, (!exact) ? 3 : 4, 4, false, sloppy_parsing, out num_parsed);
						if (num6 >= 1000 && num_parsed == 4 && !longYear && s.Length > 4 + num)
						{
							int num_parsed2 = 0;
							int num14 = _ParseNumber(s, num, 5, 5, false, sloppy_parsing, out num_parsed2);
							longYear = num14 > 9999;
						}
						num2 = 3;
					}
					if (num_parsed <= 2)
					{
						num6 += ((num6 >= 30) ? 1900 : 2000);
					}
					break;
				case 'h':
					if (num7 != -1)
					{
						return false;
					}
					num7 = ((num2 != 0) ? _ParseNumber(s, num, 1, 2, true, sloppy_parsing, out num_parsed) : _ParseNumber(s, num, 1, 2, false, sloppy_parsing, out num_parsed));
					if (num7 > 12)
					{
						return false;
					}
					if (num7 == 12)
					{
						num7 = 0;
					}
					break;
				case 'H':
					if (num7 != -1 || (!flag && ampm >= 0))
					{
						return false;
					}
					num7 = ((num2 != 0) ? _ParseNumber(s, num, 1, 2, true, sloppy_parsing, out num_parsed) : _ParseNumber(s, num, 1, 2, false, sloppy_parsing, out num_parsed));
					if (num7 >= 24)
					{
						return false;
					}
					break;
				case 'm':
					if (num8 != -1)
					{
						return false;
					}
					num8 = ((num2 != 0) ? _ParseNumber(s, num, 1, 2, true, sloppy_parsing, out num_parsed) : _ParseNumber(s, num, 1, 2, false, sloppy_parsing, out num_parsed));
					if (num8 >= 60)
					{
						return false;
					}
					break;
				case 's':
					if (num9 != -1)
					{
						return false;
					}
					num9 = ((num2 != 0) ? _ParseNumber(s, num, 1, 2, true, sloppy_parsing, out num_parsed) : _ParseNumber(s, num, 1, 2, false, sloppy_parsing, out num_parsed));
					if (num9 >= 60)
					{
						return false;
					}
					break;
				case 'F':
					leadingzero = false;
					goto case 'f';
				case 'f':
				{
					if (num2 > 6 || num10 != -1.0)
					{
						return false;
					}
					double num15 = _ParseNumber(s, num, 0, num2 + 1, leadingzero, sloppy_parsing, out num_parsed);
					if (num_parsed == -1)
					{
						return false;
					}
					num10 = num15 / Math.Pow(10.0, num_parsed);
					break;
				}
				case 't':
					if (!_ParseAmPm(s, num, (num2 <= 0) ? 1 : 0, dfi, exact, out num_parsed, ref ampm))
					{
						return false;
					}
					break;
				case 'z':
					if (num11 != -1)
					{
						return false;
					}
					if (s[num] == '+')
					{
						num11 = 0;
					}
					else
					{
						if (s[num] != '-')
						{
							return false;
						}
						num11 = 1;
					}
					num++;
					switch (num2)
					{
					case 0:
						num12 = _ParseNumber(s, num, 1, 2, false, sloppy_parsing, out num_parsed);
						break;
					case 1:
						num12 = _ParseNumber(s, num, 1, 2, true, sloppy_parsing, out num_parsed);
						break;
					default:
						num12 = _ParseNumber(s, num, 1, 2, true, true, out num_parsed);
						num += num_parsed;
						if (num_parsed < 0)
						{
							return false;
						}
						num_parsed = 0;
						if ((num < s.Length && char.IsDigit(s[num])) || _ParseTimeSeparator(s, num, dfi, exact, out num_parsed))
						{
							num += num_parsed;
							num13 = _ParseNumber(s, num, 1, 2, true, sloppy_parsing, out num_parsed);
							if (num_parsed < 0)
							{
								return false;
							}
						}
						else
						{
							if (!flag)
							{
								return false;
							}
							num_parsed = 0;
						}
						break;
					}
					break;
				case 'K':
					if (s[num] == 'Z')
					{
						num++;
						useutc = true;
					}
					else if (s[num] == '+' || s[num] == '-')
					{
						if (num11 != -1)
						{
							return false;
						}
						if (s[num] == '+')
						{
							num11 = 0;
						}
						else if (s[num] == '-')
						{
							num11 = 1;
						}
						num++;
						num12 = _ParseNumber(s, num, 0, 2, true, sloppy_parsing, out num_parsed);
						num += num_parsed;
						if (num_parsed < 0)
						{
							return false;
						}
						if (char.IsDigit(s[num]))
						{
							num_parsed = 0;
						}
						else if (!_ParseString(s, num, 0, dfi.TimeSeparator, out num_parsed))
						{
							return false;
						}
						num += num_parsed;
						num13 = _ParseNumber(s, num, 0, 2, true, sloppy_parsing, out num_parsed);
						num2 = 2;
						if (num_parsed < 0)
						{
							return false;
						}
					}
					break;
				case 'Z':
					if (s[num] != 'Z')
					{
						return false;
					}
					num2 = 0;
					num_parsed = 1;
					useutc = true;
					break;
				case 'G':
					if (s[num] != 'G')
					{
						return false;
					}
					if (i + 2 < length && num + 2 < s.Length && text2[i + 1] == 'M' && s[num + 1] == 'M' && text2[i + 2] == 'T' && s[num + 2] == 'T')
					{
						useutc = true;
						num2 = 2;
						num_parsed = 3;
					}
					else
					{
						num2 = 0;
						num_parsed = 1;
					}
					break;
				case ':':
					if (!_ParseTimeSeparator(s, num, dfi, exact, out num_parsed))
					{
						return false;
					}
					break;
				case '/':
					if (!_ParseDateSeparator(s, num, dfi, exact, out num_parsed))
					{
						return false;
					}
					num2 = 0;
					break;
				default:
					if (s[num] != text2[i])
					{
						return false;
					}
					num2 = 0;
					num_parsed = 1;
					break;
				}
				if (num_parsed < 0)
				{
					return false;
				}
				num += num_parsed;
				if (!exact && !flag)
				{
					char c = text2[i];
					if ((c == 'F' || c == 'f' || c == 'm' || c == 's' || c == 'z') && s.Length > num && s[num] == 'Z' && (i + 1 == text2.Length || text2[i + 1] != 'Z'))
					{
						useutc = true;
						num++;
					}
				}
				i = i + num2 + 1;
				num2 = 0;
			}
			if (i + 1 < length && text2[i] == '.' && text2[i + 1] == 'F')
			{
				for (i++; i < length && text2[i] == 'F'; i++)
				{
				}
			}
			for (; i < length && text2[i] == 'K'; i++)
			{
			}
			if (i < length)
			{
				return false;
			}
			if (s.Length > num)
			{
				if (num == 0)
				{
					return false;
				}
				if (char.IsDigit(s[num]) && char.IsDigit(s[num - 1]))
				{
					return false;
				}
				if (char.IsLetter(s[num]) && char.IsLetter(s[num - 1]))
				{
					return false;
				}
				incompleteFormat = true;
				return false;
			}
			if (num7 == -1)
			{
				num7 = 0;
			}
			if (num8 == -1)
			{
				num8 = 0;
			}
			if (num9 == -1)
			{
				num9 = 0;
			}
			if (num10 == -1.0)
			{
				num10 = 0.0;
			}
			if (num3 == -1 && num5 == -1 && num6 == -1)
			{
				if ((style & DateTimeStyles.NoCurrentDateDefault) != DateTimeStyles.None)
				{
					num3 = 1;
					num5 = 1;
					num6 = 1;
				}
				else
				{
					num3 = Today.Day;
					num5 = Today.Month;
					num6 = Today.Year;
				}
			}
			if (num3 == -1)
			{
				num3 = 1;
			}
			if (num5 == -1)
			{
				num5 = 1;
			}
			if (num6 == -1)
			{
				num6 = (((style & DateTimeStyles.NoCurrentDateDefault) != DateTimeStyles.None) ? 1 : Today.Year);
			}
			if (ampm == 0 && num7 == 12)
			{
				num7 = 0;
			}
			if (ampm == 1 && (!flag || num7 < 12))
			{
				num7 += 12;
			}
			if (num6 < 1 || num6 > 9999 || num5 < 1 || num5 > 12 || num3 < 1 || num3 > DaysInMonth(num6, num5) || num7 < 0 || num7 > 23 || num8 < 0 || num8 > 59 || num9 < 0 || num9 > 59)
			{
				return false;
			}
			result = new DateTime(num6, num5, num3, num7, num8, num9, 0);
			result = result.AddSeconds(num10);
			if (num4 != -1 && num4 != (int)result.DayOfWeek)
			{
				return false;
			}
			if (num11 == -1)
			{
				if (result != MinValue)
				{
					try
					{
						dto = new DateTimeOffset(result);
					}
					catch
					{
					}
				}
			}
			else
			{
				if (num13 == -1)
				{
					num13 = 0;
				}
				if (num12 == -1)
				{
					num12 = 0;
				}
				if (num11 == 1)
				{
					num12 = -num12;
					num13 = -num13;
				}
				try
				{
					dto = new DateTimeOffset(result, new TimeSpan(num12, num13, 0));
				}
				catch
				{
				}
			}
			bool flag5 = (style & DateTimeStyles.AdjustToUniversal) != 0;
			if (num11 != -1)
			{
				long num16 = (result.ticks - dto.Offset).Ticks;
				if (num16 < 0)
				{
					num16 += 864000000000L;
				}
				result = new DateTime(false, new TimeSpan(num16));
				result.kind = DateTimeKind.Utc;
				if ((style & DateTimeStyles.RoundtripKind) != DateTimeStyles.None)
				{
					result = result.ToLocalTime();
				}
			}
			else if (useutc || (style & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None)
			{
				result.kind = DateTimeKind.Utc;
			}
			else if ((style & DateTimeStyles.AssumeLocal) != DateTimeStyles.None)
			{
				result.kind = DateTimeKind.Local;
			}
			bool flag6 = !flag5 && (style & DateTimeStyles.RoundtripKind) == 0;
			if (result.kind != DateTimeKind.Unspecified)
			{
				if (flag5)
				{
					result = result.ToUniversalTime();
				}
				else if (flag6)
				{
					result = result.ToLocalTime();
				}
			}
			return true;
		}

		public static DateTime ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			return ParseExact(s, new string[1] { format }, provider, style);
		}

		public static DateTime ParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style)
		{
			DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(provider);
			CheckStyle(style);
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (formats == null)
			{
				throw new ArgumentNullException("formats");
			}
			if (formats.Length == 0)
			{
				throw new FormatException("Format specifier was invalid.");
			}
			bool longYear = false;
			Exception exception = null;
			DateTime ret;
			if (!ParseExact(s, formats, instance, style, out ret, true, ref longYear, true, ref exception))
			{
				throw exception;
			}
			return ret;
		}

		private static void CheckStyle(DateTimeStyles style)
		{
			if ((style & DateTimeStyles.RoundtripKind) != DateTimeStyles.None && ((style & DateTimeStyles.AdjustToUniversal) != DateTimeStyles.None || (style & DateTimeStyles.AssumeLocal) != DateTimeStyles.None || (style & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None))
			{
				throw new ArgumentException("The DateTimeStyles value RoundtripKind cannot be used with the values AssumeLocal, Asersal or AdjustToUniversal.", "style");
			}
			if ((style & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None && (style & DateTimeStyles.AssumeLocal) != DateTimeStyles.None)
			{
				throw new ArgumentException("The DateTimeStyles values AssumeLocal and AssumeUniversal cannot be used together.", "style");
			}
		}

		public static bool TryParse(string s, out DateTime result)
		{
			if (s != null)
			{
				try
				{
					Exception exception = null;
					DateTimeOffset dto;
					return CoreParse(s, null, DateTimeStyles.AllowWhiteSpaces, out result, out dto, false, ref exception);
				}
				catch
				{
				}
			}
			result = MinValue;
			return false;
		}

		public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out DateTime result)
		{
			if (s != null)
			{
				try
				{
					Exception exception = null;
					DateTimeOffset dto;
					return CoreParse(s, provider, styles, out result, out dto, false, ref exception);
				}
				catch
				{
				}
			}
			result = MinValue;
			return false;
		}

		public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result)
		{
			return TryParseExact(s, new string[1] { format }, provider, style, out result);
		}

		public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style, out DateTime result)
		{
			try
			{
				DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(provider);
				bool longYear = false;
				Exception exception = null;
				return ParseExact(s, formats, instance, style, out result, true, ref longYear, false, ref exception);
			}
			catch
			{
				result = MinValue;
				return false;
			}
		}

		private static bool ParseExact(string s, string[] formats, DateTimeFormatInfo dfi, DateTimeStyles style, out DateTime ret, bool exact, ref bool longYear, bool setExceptionOnError, ref Exception exception)
		{
			bool incompleteFormat = false;
			for (int i = 0; i < formats.Length; i++)
			{
				string text = formats[i];
				if (text == null || text == string.Empty)
				{
					break;
				}
				DateTime result;
				DateTimeOffset dto;
				if (_DoParse(s, formats[i], null, exact, out result, out dto, dfi, style, false, ref incompleteFormat, ref longYear))
				{
					ret = result;
					return true;
				}
			}
			if (setExceptionOnError)
			{
				exception = new FormatException("Invalid format string");
			}
			ret = MinValue;
			return false;
		}

		public TimeSpan Subtract(DateTime value)
		{
			return new TimeSpan(ticks.Ticks) - value.ticks;
		}

		public DateTime Subtract(TimeSpan value)
		{
			TimeSpan value2 = new TimeSpan(ticks.Ticks) - value;
			DateTime result = new DateTime(true, value2);
			result.kind = kind;
			return result;
		}

		public long ToFileTime()
		{
			DateTime dateTime = ToUniversalTime();
			if (dateTime.Ticks < 504911232000000000L)
			{
				throw new ArgumentOutOfRangeException("file time is not valid");
			}
			return dateTime.Ticks - 504911232000000000L;
		}

		public long ToFileTimeUtc()
		{
			if (Ticks < 504911232000000000L)
			{
				throw new ArgumentOutOfRangeException("file time is not valid");
			}
			return Ticks - 504911232000000000L;
		}

		public string ToLongDateString()
		{
			return ToString("D");
		}

		public string ToLongTimeString()
		{
			return ToString("T");
		}

		public double ToOADate()
		{
			long num = Ticks;
			if (num == 0L)
			{
				return 0.0;
			}
			if (num < 31242239136000000L)
			{
				return -657434.999;
			}
			double num2 = new TimeSpan(Ticks - 599264352000000000L).TotalDays;
			if (num < 599264352000000000L)
			{
				double num3 = Math.Ceiling(num2);
				num2 = num3 - 2.0 - (num2 - num3);
			}
			else if (num2 >= 2958466.0)
			{
				num2 = 2958465.99999999;
			}
			return num2;
		}

		public string ToShortDateString()
		{
			return ToString("d");
		}

		public string ToShortTimeString()
		{
			return ToString("t");
		}

		public override string ToString()
		{
			return ToString("G", null);
		}

		public string ToString(IFormatProvider provider)
		{
			return ToString(null, provider);
		}

		public string ToString(string format)
		{
			return ToString(format, null);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(provider);
			if (format == null || format == string.Empty)
			{
				format = "G";
			}
			bool useutc = false;
			bool use_invariant = false;
			if (format.Length == 1)
			{
				char c = format[0];
				format = DateTimeUtils.GetStandardPattern(c, instance, out useutc, out use_invariant);
				if (c == 'U')
				{
					return DateTimeUtils.ToString(ToUniversalTime(), format, instance);
				}
				if (format == null)
				{
					throw new FormatException("format is not one of the format specifier characters defined for DateTimeFormatInfo");
				}
			}
			return DateTimeUtils.ToString(this, format, instance);
		}

		public DateTime ToLocalTime()
		{
			return TimeZone.CurrentTimeZone.ToLocalTime(this);
		}

		public DateTime ToUniversalTime()
		{
			return TimeZone.CurrentTimeZone.ToUniversalTime(this);
		}

		public static DateTime operator +(DateTime d, TimeSpan t)
		{
			DateTime result = new DateTime(true, d.ticks + t);
			result.kind = d.kind;
			return result;
		}

		public static bool operator ==(DateTime d1, DateTime d2)
		{
			return d1.ticks == d2.ticks;
		}

		public static bool operator >(DateTime t1, DateTime t2)
		{
			return t1.ticks > t2.ticks;
		}

		public static bool operator >=(DateTime t1, DateTime t2)
		{
			return t1.ticks >= t2.ticks;
		}

		public static bool operator !=(DateTime d1, DateTime d2)
		{
			return d1.ticks != d2.ticks;
		}

		public static bool operator <(DateTime t1, DateTime t2)
		{
			return t1.ticks < t2.ticks;
		}

		public static bool operator <=(DateTime t1, DateTime t2)
		{
			return t1.ticks <= t2.ticks;
		}

		public static TimeSpan operator -(DateTime d1, DateTime d2)
		{
			return new TimeSpan((d1.ticks - d2.ticks).Ticks);
		}

		public static DateTime operator -(DateTime d, TimeSpan t)
		{
			DateTime result = new DateTime(true, d.ticks - t);
			result.kind = d.kind;
			return result;
		}
	}
}
