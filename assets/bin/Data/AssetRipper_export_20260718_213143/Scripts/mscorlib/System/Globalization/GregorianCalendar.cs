using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	[MonoTODO("Serialization format not compatible with .NET")]
	public class GregorianCalendar : Calendar
	{
		public const int ADEra = 1;

		[NonSerialized]
		internal GregorianCalendarTypes m_type;

		private static DateTime? Min;

		private static DateTime? Max;

		public override int[] Eras
		{
			get
			{
				return new int[1] { 1 };
			}
		}

		public override int TwoDigitYearMax
		{
			get
			{
				return twoDigitYearMax;
			}
			set
			{
				CheckReadOnly();
				M_ArgumentInRange("value", value, 100, M_MaxYear);
				twoDigitYearMax = value;
			}
		}

		public virtual GregorianCalendarTypes CalendarType
		{
			get
			{
				return m_type;
			}
			set
			{
				CheckReadOnly();
				m_type = value;
			}
		}

		[ComVisible(false)]
		public override DateTime MinSupportedDateTime
		{
			get
			{
				DateTime? min = Min;
				if (!min.HasValue)
				{
					Min = new DateTime(1, 1, 1, 0, 0, 0);
				}
				return Min.Value;
			}
		}

		[ComVisible(false)]
		public override DateTime MaxSupportedDateTime
		{
			get
			{
				DateTime? max = Max;
				if (!max.HasValue)
				{
					Max = new DateTime(9999, 12, 31, 11, 59, 59);
				}
				return Max.Value;
			}
		}

		public GregorianCalendar(GregorianCalendarTypes type)
		{
			CalendarType = type;
			M_AbbrEraNames = new string[1] { "AD" };
			M_EraNames = new string[1] { "A.D." };
			if (twoDigitYearMax == 99)
			{
				twoDigitYearMax = 2029;
			}
		}

		public GregorianCalendar()
			: this(GregorianCalendarTypes.Localized)
		{
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = 1;
			}
			if (era != 1)
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal override void M_CheckYE(int year, ref int era)
		{
			M_CheckEra(ref era);
			M_ArgumentInRange("year", year, 1, 9999);
		}

		internal void M_CheckYME(int year, int month, ref int era)
		{
			M_CheckYE(year, ref era);
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", "Month must be between one and twelve.");
			}
		}

		internal void M_CheckYMDE(int year, int month, int day, ref int era)
		{
			M_CheckYME(year, month, ref era);
			M_ArgumentInRange("day", day, 1, GetDaysInMonth(year, month, era));
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			return CCGregorianCalendar.AddMonths(time, months);
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			return CCGregorianCalendar.AddYears(time, years);
		}

		public override int GetDayOfMonth(DateTime time)
		{
			return CCGregorianCalendar.GetDayOfMonth(time);
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			return CCFixed.day_of_week(date);
		}

		public override int GetDayOfYear(DateTime time)
		{
			return CCGregorianCalendar.GetDayOfYear(time);
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			return CCGregorianCalendar.GetDaysInMonth(year, month);
		}

		public override int GetDaysInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return CCGregorianCalendar.GetDaysInYear(year);
		}

		public override int GetEra(DateTime time)
		{
			return 1;
		}

		[ComVisible(false)]
		public override int GetLeapMonth(int year, int era)
		{
			return 0;
		}

		public override int GetMonth(DateTime time)
		{
			return CCGregorianCalendar.GetMonth(time);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return 12;
		}

		[ComVisible(false)]
		public override int GetWeekOfYear(DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
		{
			return base.GetWeekOfYear(time, rule, firstDayOfWeek);
		}

		public override int GetYear(DateTime time)
		{
			return CCGregorianCalendar.GetYear(time);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			return CCGregorianCalendar.IsLeapDay(year, month, day);
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return CCGregorianCalendar.is_leap_year(year);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			M_CheckHMSM(hour, minute, second, millisecond);
			return CCGregorianCalendar.ToDateTime(year, month, day, hour, minute, second, millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			return base.ToFourDigitYear(year);
		}
	}
}
