using System.IO;

namespace System.Globalization
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with .NET")]
	public class UmAlQuraCalendar : Calendar
	{
		public const int UmAlQuraEra = 1;

		internal static readonly int M_MinFixed = CCHijriCalendar.fixed_from_dmy(1, 1, 1);

		internal static readonly int M_MaxFixed = CCGregorianCalendar.fixed_from_dmy(31, 12, 9999);

		internal int M_AddHijriDate;

		private static DateTime Min = new DateTime(622, 7, 18, 0, 0, 0);

		private static DateTime Max = new DateTime(9999, 12, 31, 11, 59, 59);

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

		internal virtual int AddHijriDate
		{
			get
			{
				return M_AddHijriDate;
			}
			set
			{
				CheckReadOnly();
				if (value < -3 && value > 3)
				{
					throw new ArgumentOutOfRangeException("AddHijriDate", "Value should be between -3 and 3.");
				}
				M_AddHijriDate = value;
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

		public UmAlQuraCalendar()
		{
			M_AbbrEraNames = new string[1] { "A.H." };
			M_EraNames = new string[1] { "Anno Hegirae" };
			if (twoDigitYearMax == 99)
			{
				twoDigitYearMax = 1451;
			}
		}

		internal void M_CheckFixedHijri(string param, int rdHijri)
		{
			if (rdHijri < M_MinFixed || rdHijri > M_MaxFixed - AddHijriDate)
			{
				StringWriter stringWriter = new StringWriter();
				int day;
				int month;
				int year;
				CCHijriCalendar.dmy_from_fixed(out day, out month, out year, M_MaxFixed - AddHijriDate);
				if (AddHijriDate != 0)
				{
					stringWriter.Write("This HijriCalendar (AddHijriDate {0}) allows dates from 1. 1. 1 to {1}. {2}. {3}.", AddHijriDate, day, month, year);
				}
				else
				{
					stringWriter.Write("HijriCalendar allows dates from 1.1.1 to {0}.{1}.{2}.", day, month, year);
				}
				throw new ArgumentOutOfRangeException(param, stringWriter.ToString());
			}
		}

		internal void M_CheckDateTime(DateTime time)
		{
			int rdHijri = CCFixed.FromDateTime(time) - AddHijriDate;
			M_CheckFixedHijri("time", rdHijri);
		}

		internal int M_FromDateTime(DateTime time)
		{
			return CCFixed.FromDateTime(time) - AddHijriDate;
		}

		internal DateTime M_ToDateTime(int rd)
		{
			return CCFixed.ToDateTime(rd + AddHijriDate);
		}

		internal DateTime M_ToDateTime(int date, int hour, int minute, int second, int milliseconds)
		{
			return CCFixed.ToDateTime(date + AddHijriDate, hour, minute, second, milliseconds);
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
			M_ArgumentInRange("year", year, 1, 9666);
		}

		internal void M_CheckYME(int year, int month, ref int era)
		{
			M_CheckYE(year, ref era);
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", "Month must be between one and twelve.");
			}
			if (year == 9666)
			{
				int rdHijri = CCHijriCalendar.fixed_from_dmy(1, month, year);
				M_CheckFixedHijri("month", rdHijri);
			}
		}

		internal void M_CheckYMDE(int year, int month, int day, ref int era)
		{
			M_CheckYME(year, month, ref era);
			M_ArgumentInRange("day", day, 1, GetDaysInMonth(year, month, 1));
			if (year == 9666)
			{
				int rdHijri = CCHijriCalendar.fixed_from_dmy(day, month, year);
				M_CheckFixedHijri("day", rdHijri);
			}
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			int date = M_FromDateTime(time);
			int day;
			int month;
			int year;
			CCHijriCalendar.dmy_from_fixed(out day, out month, out year, date);
			month += months;
			year += CCMath.div_mod(out month, month, 12);
			date = CCHijriCalendar.fixed_from_dmy(day, month, year);
			M_CheckFixedHijri("time", date);
			return M_ToDateTime(date).Add(time.TimeOfDay);
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			int date = M_FromDateTime(time);
			int day;
			int month;
			int year;
			CCHijriCalendar.dmy_from_fixed(out day, out month, out year, date);
			year += years;
			date = CCHijriCalendar.fixed_from_dmy(day, month, year);
			M_CheckFixedHijri("time", date);
			return M_ToDateTime(date).Add(time.TimeOfDay);
		}

		public override int GetDayOfMonth(DateTime time)
		{
			int num = M_FromDateTime(time);
			M_CheckFixedHijri("time", num);
			return CCHijriCalendar.day_from_fixed(num);
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			int num = M_FromDateTime(time);
			M_CheckFixedHijri("time", num);
			return CCFixed.day_of_week(num);
		}

		public override int GetDayOfYear(DateTime time)
		{
			int num = M_FromDateTime(time);
			M_CheckFixedHijri("time", num);
			int year = CCHijriCalendar.year_from_fixed(num);
			int num2 = CCHijriCalendar.fixed_from_dmy(1, 1, year);
			return num - num2 + 1;
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			int num = CCHijriCalendar.fixed_from_dmy(1, month, year);
			int num2 = CCHijriCalendar.fixed_from_dmy(1, month + 1, year);
			return num2 - num;
		}

		public override int GetDaysInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			int num = CCHijriCalendar.fixed_from_dmy(1, 1, year);
			int num2 = CCHijriCalendar.fixed_from_dmy(1, 1, year + 1);
			return num2 - num;
		}

		public override int GetEra(DateTime time)
		{
			M_CheckDateTime(time);
			return 1;
		}

		public override int GetLeapMonth(int year, int era)
		{
			return 0;
		}

		public override int GetMonth(DateTime time)
		{
			int num = M_FromDateTime(time);
			M_CheckFixedHijri("time", num);
			return CCHijriCalendar.month_from_fixed(num);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return 12;
		}

		public override int GetYear(DateTime time)
		{
			int num = M_FromDateTime(time);
			M_CheckFixedHijri("time", num);
			return CCHijriCalendar.year_from_fixed(num);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			return IsLeapYear(year) && month == 12 && day == 30;
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			M_CheckYME(year, month, ref era);
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return CCHijriCalendar.is_leap_year(year);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			M_CheckYMDE(year, month, day, ref era);
			M_CheckHMSM(hour, minute, second, millisecond);
			int date = CCHijriCalendar.fixed_from_dmy(day, month, year);
			return M_ToDateTime(date, hour, minute, second, millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			return base.ToFourDigitYear(year);
		}
	}
}
