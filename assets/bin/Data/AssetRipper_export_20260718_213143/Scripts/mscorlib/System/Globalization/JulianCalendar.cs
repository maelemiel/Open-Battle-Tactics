using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	public class JulianCalendar : Calendar
	{
		public static readonly int JulianEra = 1;

		private static DateTime JulianMin = new DateTime(1, 1, 1, 0, 0, 0);

		private static DateTime JulianMax = new DateTime(9999, 12, 31, 11, 59, 59);

		public override int[] Eras
		{
			get
			{
				return new int[1] { JulianEra };
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

		[ComVisible(false)]
		public override CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.SolarCalendar;
			}
		}

		[ComVisible(false)]
		public override DateTime MinSupportedDateTime
		{
			get
			{
				return JulianMin;
			}
		}

		[ComVisible(false)]
		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return JulianMax;
			}
		}

		public JulianCalendar()
		{
			M_AbbrEraNames = new string[1] { "C.E." };
			M_EraNames = new string[1] { "Common Era" };
			if (twoDigitYearMax == 99)
			{
				twoDigitYearMax = 2029;
			}
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = JulianEra;
			}
			if (era != JulianEra)
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
			if (year == 9999 && ((month == 10 && day > 19) || month > 10))
			{
				throw new ArgumentOutOfRangeException("The maximum Julian date is 19. 10. 9999.");
			}
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			int date = CCFixed.FromDateTime(time);
			int day;
			int month;
			int year;
			CCJulianCalendar.dmy_from_fixed(out day, out month, out year, date);
			month += months;
			year += CCMath.div_mod(out month, month, 12);
			date = CCJulianCalendar.fixed_from_dmy(day, month, year);
			return CCFixed.ToDateTime(date).Add(time.TimeOfDay);
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			int date = CCFixed.FromDateTime(time);
			int day;
			int month;
			int year;
			CCJulianCalendar.dmy_from_fixed(out day, out month, out year, date);
			year += years;
			date = CCJulianCalendar.fixed_from_dmy(day, month, year);
			return CCFixed.ToDateTime(date).Add(time.TimeOfDay);
		}

		public override int GetDayOfMonth(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			return CCJulianCalendar.day_from_fixed(date);
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			return CCFixed.day_of_week(date);
		}

		public override int GetDayOfYear(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int year = CCJulianCalendar.year_from_fixed(num);
			int num2 = CCJulianCalendar.fixed_from_dmy(1, 1, year);
			return num - num2 + 1;
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			int num = CCJulianCalendar.fixed_from_dmy(1, month, year);
			int num2 = CCJulianCalendar.fixed_from_dmy(1, month + 1, year);
			return num2 - num;
		}

		public override int GetDaysInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			int num = CCJulianCalendar.fixed_from_dmy(1, 1, year);
			int num2 = CCJulianCalendar.fixed_from_dmy(1, 1, year + 1);
			return num2 - num;
		}

		public override int GetEra(DateTime time)
		{
			return JulianEra;
		}

		[ComVisible(false)]
		public override int GetLeapMonth(int year, int era)
		{
			return 0;
		}

		public override int GetMonth(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			return CCJulianCalendar.month_from_fixed(date);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return 12;
		}

		public override int GetYear(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			return CCJulianCalendar.year_from_fixed(date);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			return IsLeapYear(year) && month == 2 && day == 29;
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return CCJulianCalendar.is_leap_year(year);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			M_CheckHMSM(hour, minute, second, millisecond);
			int date = CCJulianCalendar.fixed_from_dmy(day, month, year);
			return CCFixed.ToDateTime(date, hour, minute, second, millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			return base.ToFourDigitYear(year);
		}
	}
}
