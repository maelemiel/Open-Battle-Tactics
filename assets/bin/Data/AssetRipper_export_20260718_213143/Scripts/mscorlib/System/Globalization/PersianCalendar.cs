namespace System.Globalization
{
	[Serializable]
	public class PersianCalendar : Calendar
	{
		internal const long M_MinTicks = 196036416000000000L;

		internal const int M_MinYear = 1;

		internal const int epoch = 226895;

		public static readonly int PersianEra = 1;

		private static DateTime PersianMin = new DateTime(622, 3, 21, 0, 0, 0);

		private static DateTime PersianMax = new DateTime(9999, 12, 31, 11, 59, 59);

		public override int[] Eras
		{
			get
			{
				return new int[1] { PersianEra };
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

		public override CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.SolarCalendar;
			}
		}

		public override DateTime MinSupportedDateTime
		{
			get
			{
				return PersianMin;
			}
		}

		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return PersianMax;
			}
		}

		public PersianCalendar()
		{
			M_AbbrEraNames = new string[1] { "A.P." };
			M_EraNames = new string[1] { "Anno Persico" };
			if (twoDigitYearMax == 99)
			{
				twoDigitYearMax = 1410;
			}
		}

		internal void M_CheckDateTime(DateTime time)
		{
			if (time.Ticks < 196036416000000000L)
			{
				throw new ArgumentOutOfRangeException("time", "Only positive Persian years are supported.");
			}
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = PersianEra;
			}
			if (era != PersianEra)
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal override void M_CheckYE(int year, ref int era)
		{
			M_CheckEra(ref era);
			if (year < 1 || year > M_MaxYear)
			{
				throw new ArgumentOutOfRangeException("year", "Only Persian years between 1 and 9378, inclusive, are supported.");
			}
		}

		internal void M_CheckYME(int year, int month, ref int era)
		{
			M_CheckYE(year, ref era);
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", "Month must be between one and twelve.");
			}
			if (year == M_MaxYear && month > 10)
			{
				throw new ArgumentOutOfRangeException("month", "Months in year 9378 must be between one and ten.");
			}
		}

		internal void M_CheckYMDE(int year, int month, int day, ref int era)
		{
			M_CheckYME(year, month, ref era);
			M_ArgumentInRange("day", day, 1, GetDaysInMonth(year, month, era));
			if (year == M_MaxYear && month == 10 && day > 10)
			{
				throw new ArgumentOutOfRangeException("day", "Days in month 10 of year 9378 must be between one and ten.");
			}
		}

		internal int fixed_from_dmy(int day, int month, int year)
		{
			int num = 226894;
			num += 365 * (year - 1);
			num += (8 * year + 21) / 33;
			num = ((month > 7) ? (num + (30 * (month - 1) + 6)) : (num + 31 * (month - 1)));
			return num + day;
		}

		internal int year_from_fixed(int date)
		{
			return (33 * (date - 226895) + 3) / 12053 + 1;
		}

		internal void my_from_fixed(out int month, out int year, int date)
		{
			year = year_from_fixed(date);
			int num = date - fixed_from_dmy(1, 1, year);
			if (num < 216)
			{
				month = num / 31 + 1;
			}
			else
			{
				month = (num - 6) / 30 + 1;
			}
		}

		internal void dmy_from_fixed(out int day, out int month, out int year, int date)
		{
			year = year_from_fixed(date);
			day = date - fixed_from_dmy(1, 1, year);
			if (day < 216)
			{
				month = day / 31 + 1;
				day = day % 31 + 1;
			}
			else
			{
				month = (day - 6) / 30 + 1;
				day = (day - 6) % 30 + 1;
			}
		}

		internal bool is_leap_year(int year)
		{
			return (25 * year + 11) % 33 < 8;
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			int date = CCFixed.FromDateTime(time);
			int day;
			int month;
			int year;
			dmy_from_fixed(out day, out month, out year, date);
			month += months;
			year += CCMath.div_mod(out month, month, 12);
			date = fixed_from_dmy(day, month, year);
			DateTime dateTime = CCFixed.ToDateTime(date).Add(time.TimeOfDay);
			M_CheckDateTime(dateTime);
			return dateTime;
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			int date = CCFixed.FromDateTime(time);
			int day;
			int month;
			int year;
			dmy_from_fixed(out day, out month, out year, date);
			year += years;
			date = fixed_from_dmy(day, month, year);
			DateTime dateTime = CCFixed.ToDateTime(date).Add(time.TimeOfDay);
			M_CheckDateTime(dateTime);
			return dateTime;
		}

		public override int GetDayOfMonth(DateTime time)
		{
			M_CheckDateTime(time);
			int date = CCFixed.FromDateTime(time);
			int day;
			int month;
			int year;
			dmy_from_fixed(out day, out month, out year, date);
			return day;
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			M_CheckDateTime(time);
			int date = CCFixed.FromDateTime(time);
			return CCFixed.day_of_week(date);
		}

		public override int GetDayOfYear(DateTime time)
		{
			M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			int year = year_from_fixed(num);
			int num2 = fixed_from_dmy(1, 1, year);
			return num - num2 + 1;
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			if (month <= 6)
			{
				return 31;
			}
			if (month == 12 && !is_leap_year(year))
			{
				return 29;
			}
			return 30;
		}

		public override int GetDaysInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return (!is_leap_year(year)) ? 365 : 366;
		}

		public override int GetEra(DateTime time)
		{
			M_CheckDateTime(time);
			return PersianEra;
		}

		public override int GetLeapMonth(int year, int era)
		{
			return 0;
		}

		public override int GetMonth(DateTime time)
		{
			M_CheckDateTime(time);
			int date = CCFixed.FromDateTime(time);
			int month;
			int year;
			my_from_fixed(out month, out year, date);
			return month;
		}

		public override int GetMonthsInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return 12;
		}

		public override int GetYear(DateTime time)
		{
			M_CheckDateTime(time);
			int date = CCFixed.FromDateTime(time);
			return year_from_fixed(date);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			return is_leap_year(year) && month == 12 && day == 30;
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return is_leap_year(year);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			M_CheckHMSM(hour, minute, second, millisecond);
			int date = fixed_from_dmy(day, month, year);
			return CCFixed.ToDateTime(date, hour, minute, second, millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			M_ArgumentInRange("year", year, 0, 99);
			int num = twoDigitYearMax % 100;
			int num2 = twoDigitYearMax - num;
			if (year <= num)
			{
				return num2 + year;
			}
			return num2 + year - 100;
		}
	}
}
