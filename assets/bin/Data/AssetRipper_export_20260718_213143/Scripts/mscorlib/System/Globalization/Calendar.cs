using System.IO;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public abstract class Calendar : ICloneable
	{
		public const int CurrentEra = 0;

		[NonSerialized]
		private bool m_isReadOnly;

		[NonSerialized]
		internal int twoDigitYearMax;

		[NonSerialized]
		private int M_MaxYearValue;

		[NonSerialized]
		internal string[] M_AbbrEraNames;

		[NonSerialized]
		internal string[] M_EraNames;

		internal int m_currentEraValue;

		internal virtual int M_DaysInWeek
		{
			get
			{
				return 7;
			}
		}

		public abstract int[] Eras { get; }

		[ComVisible(false)]
		public virtual CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.Unknown;
			}
		}

		[ComVisible(false)]
		public virtual DateTime MaxSupportedDateTime
		{
			get
			{
				return DateTime.MaxValue;
			}
		}

		[ComVisible(false)]
		public virtual DateTime MinSupportedDateTime
		{
			get
			{
				return DateTime.MinValue;
			}
		}

		[ComVisible(false)]
		public bool IsReadOnly
		{
			get
			{
				return m_isReadOnly;
			}
		}

		internal virtual int M_MaxYear
		{
			get
			{
				if (M_MaxYearValue == 0)
				{
					M_MaxYearValue = GetYear(DateTime.MaxValue);
				}
				return M_MaxYearValue;
			}
		}

		public virtual int TwoDigitYearMax
		{
			get
			{
				return twoDigitYearMax;
			}
			set
			{
				CheckReadOnly();
				M_ArgumentInRange("year", value, 100, M_MaxYear);
				int era = 0;
				M_CheckYE(value, ref era);
				twoDigitYearMax = value;
			}
		}

		internal string[] AbbreviatedEraNames
		{
			get
			{
				if (M_AbbrEraNames == null || M_AbbrEraNames.Length != Eras.Length)
				{
					throw new Exception("Internal: M_AbbrEraNames wrong initialized!");
				}
				return (string[])M_AbbrEraNames.Clone();
			}
			set
			{
				CheckReadOnly();
				if (value.Length != Eras.Length)
				{
					StringWriter stringWriter = new StringWriter();
					stringWriter.Write("Array length must be equal Eras length {0}.", Eras.Length);
					throw new ArgumentException(stringWriter.ToString());
				}
				M_AbbrEraNames = (string[])value.Clone();
			}
		}

		internal string[] EraNames
		{
			get
			{
				if (M_EraNames == null || M_EraNames.Length != Eras.Length)
				{
					throw new Exception("Internal: M_EraNames not initialized!");
				}
				return (string[])M_EraNames.Clone();
			}
			set
			{
				CheckReadOnly();
				if (value.Length != Eras.Length)
				{
					StringWriter stringWriter = new StringWriter();
					stringWriter.Write("Array length must be equal Eras length {0}.", Eras.Length);
					throw new ArgumentException(stringWriter.ToString());
				}
				M_EraNames = (string[])value.Clone();
			}
		}

		protected Calendar()
		{
			twoDigitYearMax = 99;
		}

		internal string M_ValidValues(object a, object b)
		{
			StringWriter stringWriter = new StringWriter();
			stringWriter.Write("Valid values are between {0} and {1}, inclusive.", a, b);
			return stringWriter.ToString();
		}

		internal void M_ArgumentInRange(string param, int arg, int a, int b)
		{
			if (a <= arg && arg <= b)
			{
				return;
			}
			throw new ArgumentOutOfRangeException(param, M_ValidValues(a, b));
		}

		internal void M_CheckHMSM(int hour, int minute, int second, int milliseconds)
		{
			M_ArgumentInRange("hour", hour, 0, 23);
			M_ArgumentInRange("minute", minute, 0, 59);
			M_ArgumentInRange("second", second, 0, 59);
			M_ArgumentInRange("milliseconds", milliseconds, 0, 999999);
		}

		[ComVisible(false)]
		public virtual object Clone()
		{
			Calendar calendar = (Calendar)MemberwiseClone();
			calendar.m_isReadOnly = false;
			return calendar;
		}

		[ComVisible(false)]
		public virtual int GetLeapMonth(int year)
		{
			return GetLeapMonth(year, GetEra(ToDateTime(year, 1, 1, 0, 0, 0, 0)));
		}

		[ComVisible(false)]
		public virtual int GetLeapMonth(int year, int era)
		{
			int monthsInYear = GetMonthsInYear(year, era);
			for (int i = 1; i <= monthsInYear; i++)
			{
				if (IsLeapMonth(year, i, era))
				{
					return i;
				}
			}
			return 0;
		}

		[ComVisible(false)]
		public static Calendar ReadOnly(Calendar calendar)
		{
			if (calendar.m_isReadOnly)
			{
				return calendar;
			}
			Calendar calendar2 = (Calendar)calendar.Clone();
			calendar2.m_isReadOnly = true;
			return calendar2;
		}

		internal void CheckReadOnly()
		{
			if (m_isReadOnly)
			{
				throw new InvalidOperationException("This Calendar is read-only.");
			}
		}

		internal virtual void M_CheckYE(int year, ref int era)
		{
		}

		public virtual DateTime AddDays(DateTime time, int days)
		{
			return time.Add(TimeSpan.FromDays(days));
		}

		public virtual DateTime AddHours(DateTime time, int hours)
		{
			return time.Add(TimeSpan.FromHours(hours));
		}

		public virtual DateTime AddMilliseconds(DateTime time, double milliseconds)
		{
			return time.Add(TimeSpan.FromMilliseconds(milliseconds));
		}

		public virtual DateTime AddMinutes(DateTime time, int minutes)
		{
			return time.Add(TimeSpan.FromMinutes(minutes));
		}

		public abstract DateTime AddMonths(DateTime time, int months);

		public virtual DateTime AddSeconds(DateTime time, int seconds)
		{
			return time.Add(TimeSpan.FromSeconds(seconds));
		}

		public virtual DateTime AddWeeks(DateTime time, int weeks)
		{
			return time.AddDays(weeks * M_DaysInWeek);
		}

		public abstract DateTime AddYears(DateTime time, int years);

		public abstract int GetDayOfMonth(DateTime time);

		public abstract DayOfWeek GetDayOfWeek(DateTime time);

		public abstract int GetDayOfYear(DateTime time);

		public virtual int GetDaysInMonth(int year, int month)
		{
			return GetDaysInMonth(year, month, 0);
		}

		public abstract int GetDaysInMonth(int year, int month, int era);

		public virtual int GetDaysInYear(int year)
		{
			return GetDaysInYear(year, 0);
		}

		public abstract int GetDaysInYear(int year, int era);

		public abstract int GetEra(DateTime time);

		public virtual int GetHour(DateTime time)
		{
			return time.TimeOfDay.Hours;
		}

		public virtual double GetMilliseconds(DateTime time)
		{
			return time.TimeOfDay.Milliseconds;
		}

		public virtual int GetMinute(DateTime time)
		{
			return time.TimeOfDay.Minutes;
		}

		public abstract int GetMonth(DateTime time);

		public virtual int GetMonthsInYear(int year)
		{
			return GetMonthsInYear(year, 0);
		}

		public abstract int GetMonthsInYear(int year, int era);

		public virtual int GetSecond(DateTime time)
		{
			return time.TimeOfDay.Seconds;
		}

		internal int M_DiffDays(DateTime timeA, DateTime timeB)
		{
			long num = timeA.Ticks - timeB.Ticks;
			if (num >= 0)
			{
				return (int)(num / 864000000000L);
			}
			num++;
			return -1 + (int)(num / 864000000000L);
		}

		internal DateTime M_GetFirstDayOfSecondWeekOfYear(int year, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
		{
			DateTime time = ToDateTime(year, 1, 1, 0, 0, 0, 0);
			int dayOfWeek = (int)GetDayOfWeek(time);
			int num = 0;
			switch (rule)
			{
			case CalendarWeekRule.FirstDay:
				num = (int)(((int)firstDayOfWeek <= dayOfWeek) ? (num + (firstDayOfWeek + M_DaysInWeek - dayOfWeek)) : (num + (firstDayOfWeek - dayOfWeek)));
				break;
			case CalendarWeekRule.FirstFullWeek:
				num = M_DaysInWeek;
				num = (int)(((int)firstDayOfWeek < dayOfWeek) ? (num + (firstDayOfWeek + M_DaysInWeek - dayOfWeek)) : (num + (firstDayOfWeek - dayOfWeek)));
				break;
			case CalendarWeekRule.FirstFourDayWeek:
			{
				int num2 = (dayOfWeek + 3) % M_DaysInWeek;
				num = 3;
				num = (int)(((int)firstDayOfWeek <= num2) ? (num + (firstDayOfWeek + M_DaysInWeek - num2)) : (num + (firstDayOfWeek - num2)));
				break;
			}
			}
			return AddDays(time, num);
		}

		public virtual int GetWeekOfYear(DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
		{
			if (firstDayOfWeek < DayOfWeek.Sunday || DayOfWeek.Saturday < firstDayOfWeek)
			{
				throw new ArgumentOutOfRangeException("firstDayOfWeek", "Value is not a valid day of week.");
			}
			int num = GetYear(time);
			int num2;
			while (true)
			{
				DateTime timeB = M_GetFirstDayOfSecondWeekOfYear(num, rule, firstDayOfWeek);
				num2 = M_DiffDays(time, timeB) + M_DaysInWeek;
				if (num2 >= 0)
				{
					break;
				}
				num--;
			}
			return 1 + num2 / M_DaysInWeek;
		}

		public abstract int GetYear(DateTime time);

		public virtual bool IsLeapDay(int year, int month, int day)
		{
			return IsLeapDay(year, month, day, 0);
		}

		public abstract bool IsLeapDay(int year, int month, int day, int era);

		public virtual bool IsLeapMonth(int year, int month)
		{
			return IsLeapMonth(year, month, 0);
		}

		public abstract bool IsLeapMonth(int year, int month, int era);

		public virtual bool IsLeapYear(int year)
		{
			return IsLeapYear(year, 0);
		}

		public abstract bool IsLeapYear(int year, int era);

		public virtual DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			return ToDateTime(year, month, day, hour, minute, second, millisecond, 0);
		}

		public abstract DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era);

		public virtual int ToFourDigitYear(int year)
		{
			if (year < 0)
			{
				throw new ArgumentOutOfRangeException("year", "Non-negative number required.");
			}
			if (year <= 99)
			{
				int num = TwoDigitYearMax % 100;
				int num2 = year - num;
				year = TwoDigitYearMax + num2 + ((num2 > 0) ? (-100) : 0);
			}
			int era = 0;
			M_CheckYE(year, ref era);
			return year;
		}
	}
}
