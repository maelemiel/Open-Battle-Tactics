using System.IO;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with.NET")]
	[ComVisible(true)]
	public class HebrewCalendar : Calendar
	{
		internal const long M_MinTicks = 499147488000000000L;

		internal const long M_MaxTicks = 706783967999999999L;

		internal const int M_MinYear = 5343;

		public static readonly int HebrewEra = 1;

		private static DateTime Min = new DateTime(1583, 1, 1, 0, 0, 0);

		private static DateTime Max = new DateTime(2239, 9, 29, 11, 59, 59);

		internal override int M_MaxYear
		{
			get
			{
				return 6000;
			}
		}

		public override int[] Eras
		{
			get
			{
				return new int[1] { HebrewEra };
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
				M_ArgumentInRange("value", value, 5343, M_MaxYear);
				twoDigitYearMax = value;
			}
		}

		public override DateTime MinSupportedDateTime
		{
			get
			{
				return Min;
			}
		}

		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return Max;
			}
		}

		public HebrewCalendar()
		{
			M_AbbrEraNames = new string[1] { "A.M." };
			M_EraNames = new string[1] { "Anno Mundi" };
			if (twoDigitYearMax == 99)
			{
				twoDigitYearMax = 5790;
			}
		}

		internal void M_CheckDateTime(DateTime time)
		{
			if (time.Ticks < 499147488000000000L || time.Ticks > 706783967999999999L)
			{
				throw new ArgumentOutOfRangeException("time", "Only hebrew years between 5343 and 6000, inclusive, are supported.");
			}
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = HebrewEra;
			}
			if (era != HebrewEra)
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal override void M_CheckYE(int year, ref int era)
		{
			M_CheckEra(ref era);
			if (year < 5343 || year > M_MaxYear)
			{
				throw new ArgumentOutOfRangeException("year", "Only hebrew years between 5343 and 6000, inclusive, are supported.");
			}
		}

		internal void M_CheckYME(int year, int month, ref int era)
		{
			M_CheckYE(year, ref era);
			int num = CCHebrewCalendar.last_month_of_year(year);
			if (month < 1 || month > num)
			{
				StringWriter stringWriter = new StringWriter();
				stringWriter.Write("Month must be between 1 and {0}.", num);
				throw new ArgumentOutOfRangeException("month", stringWriter.ToString());
			}
		}

		internal void M_CheckYMDE(int year, int month, int day, ref int era)
		{
			M_CheckYME(year, month, ref era);
			M_ArgumentInRange("day", day, 1, GetDaysInMonth(year, month, era));
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			DateTime dateTime;
			if (months == 0)
			{
				dateTime = time;
			}
			else
			{
				int date = CCFixed.FromDateTime(time);
				int day;
				int year;
				int month;
				CCHebrewCalendar.dmy_from_fixed(out day, out month, out year, date);
				month = M_Month(month, year);
				if (months < 0)
				{
					while (months < 0)
					{
						if (month + months > 0)
						{
							month += months;
							months = 0;
						}
						else
						{
							months += month;
							year--;
							month = GetMonthsInYear(year);
						}
					}
				}
				else
				{
					while (months > 0)
					{
						int monthsInYear = GetMonthsInYear(year);
						if (month + months <= monthsInYear)
						{
							month += months;
							months = 0;
						}
						else
						{
							months -= monthsInYear - month + 1;
							month = 1;
							year++;
						}
					}
				}
				dateTime = ToDateTime(year, month, day, 0, 0, 0, 0).Add(time.TimeOfDay);
			}
			M_CheckDateTime(dateTime);
			return dateTime;
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			int date = CCFixed.FromDateTime(time);
			int day;
			int month;
			int year;
			CCHebrewCalendar.dmy_from_fixed(out day, out month, out year, date);
			year += years;
			date = CCHebrewCalendar.fixed_from_dmy(day, month, year);
			DateTime dateTime = CCFixed.ToDateTime(date).Add(time.TimeOfDay);
			M_CheckDateTime(dateTime);
			return dateTime;
		}

		public override int GetDayOfMonth(DateTime time)
		{
			M_CheckDateTime(time);
			int date = CCFixed.FromDateTime(time);
			return CCHebrewCalendar.day_from_fixed(date);
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
			int year = CCHebrewCalendar.year_from_fixed(num);
			int num2 = CCHebrewCalendar.fixed_from_dmy(1, 7, year);
			return num - num2 + 1;
		}

		internal int M_CCMonth(int month, int year)
		{
			if (month <= 6)
			{
				return 6 + month;
			}
			int num = CCHebrewCalendar.last_month_of_year(year);
			if (num == 12)
			{
				return month - 6;
			}
			return (month > 7) ? (month - 7) : (6 + month);
		}

		internal int M_Month(int ccmonth, int year)
		{
			if (ccmonth >= 7)
			{
				return ccmonth - 6;
			}
			int num = CCHebrewCalendar.last_month_of_year(year);
			return ccmonth + ((num != 12) ? 7 : 6);
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			int month2 = M_CCMonth(month, year);
			return CCHebrewCalendar.last_day_of_month(month2, year);
		}

		public override int GetDaysInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			int num = CCHebrewCalendar.fixed_from_dmy(1, 7, year);
			int num2 = CCHebrewCalendar.fixed_from_dmy(1, 7, year + 1);
			return num2 - num;
		}

		public override int GetEra(DateTime time)
		{
			M_CheckDateTime(time);
			return HebrewEra;
		}

		public override int GetLeapMonth(int year, int era)
		{
			return IsLeapMonth(year, 7, era) ? 7 : 0;
		}

		public override int GetMonth(DateTime time)
		{
			M_CheckDateTime(time);
			int date = CCFixed.FromDateTime(time);
			int month;
			int year;
			CCHebrewCalendar.my_from_fixed(out month, out year, date);
			return M_Month(month, year);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return CCHebrewCalendar.last_month_of_year(year);
		}

		public override int GetYear(DateTime time)
		{
			M_CheckDateTime(time);
			int date = CCFixed.FromDateTime(time);
			return CCHebrewCalendar.year_from_fixed(date);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			int result;
			if (IsLeapYear(year))
			{
				switch (month)
				{
				case 6:
					result = ((day == 30) ? 1 : 0);
					break;
				default:
					result = 0;
					break;
				case 7:
					result = 1;
					break;
				}
			}
			else
			{
				result = 0;
			}
			return (byte)result != 0;
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			return IsLeapYear(year) && month == 7;
		}

		public override bool IsLeapYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return CCHebrewCalendar.is_leap_year(year);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			M_CheckHMSM(hour, minute, second, millisecond);
			int month2 = M_CCMonth(month, year);
			int date = CCHebrewCalendar.fixed_from_dmy(day, month2, year);
			return CCFixed.ToDateTime(date, hour, minute, second, millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			M_ArgumentInRange("year", year, 0, M_MaxYear - 1);
			int num = twoDigitYearMax % 100;
			int num2 = twoDigitYearMax - num;
			if (year >= 100)
			{
				return year;
			}
			if (year <= num)
			{
				return num2 + year;
			}
			return num2 + year - 100;
		}
	}
}
