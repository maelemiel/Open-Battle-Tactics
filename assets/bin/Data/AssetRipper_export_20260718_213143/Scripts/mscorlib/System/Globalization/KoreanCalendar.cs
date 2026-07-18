using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	public class KoreanCalendar : Calendar
	{
		public const int KoreanEra = 1;

		internal static readonly CCGregorianEraHandler M_EraHandler;

		private static DateTime KoreanMin;

		private static DateTime KoreanMax;

		public override int[] Eras
		{
			get
			{
				return (int[])M_EraHandler.Eras.Clone();
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
		public override DateTime MinSupportedDateTime
		{
			get
			{
				return KoreanMin;
			}
		}

		[ComVisible(false)]
		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return KoreanMax;
			}
		}

		public KoreanCalendar()
		{
			M_AbbrEraNames = new string[1] { "K.C.E." };
			M_EraNames = new string[1] { "Korean Current Era" };
			if (twoDigitYearMax == 99)
			{
				twoDigitYearMax = 4362;
			}
		}

		static KoreanCalendar()
		{
			KoreanMin = new DateTime(1, 1, 1, 0, 0, 0);
			KoreanMax = new DateTime(9999, 12, 31, 11, 59, 59);
			M_EraHandler = new CCGregorianEraHandler();
			M_EraHandler.appendEra(1, CCGregorianCalendar.fixed_from_dmy(1, 1, -2332));
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = 1;
			}
			if (!M_EraHandler.ValidEra(era))
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal int M_CheckYEG(int year, ref int era)
		{
			M_CheckEra(ref era);
			return M_EraHandler.GregorianYear(year, era);
		}

		internal override void M_CheckYE(int year, ref int era)
		{
			M_CheckYEG(year, ref era);
		}

		internal int M_CheckYMEG(int year, int month, ref int era)
		{
			int result = M_CheckYEG(year, ref era);
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", "Month must be between one and twelve.");
			}
			return result;
		}

		internal int M_CheckYMDEG(int year, int month, int day, ref int era)
		{
			int result = M_CheckYMEG(year, month, ref era);
			M_ArgumentInRange("day", day, 1, GetDaysInMonth(year, month, era));
			return result;
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
			int year2 = M_CheckYMEG(year, month, ref era);
			return CCGregorianCalendar.GetDaysInMonth(year2, month);
		}

		public override int GetDaysInYear(int year, int era)
		{
			int year2 = M_CheckYEG(year, ref era);
			return CCGregorianCalendar.GetDaysInYear(year2);
		}

		public override int GetEra(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			int era;
			M_EraHandler.EraYear(out era, date);
			return era;
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
			M_CheckYEG(year, ref era);
			return 12;
		}

		[ComVisible(false)]
		public override int GetWeekOfYear(DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
		{
			return base.GetWeekOfYear(time, rule, firstDayOfWeek);
		}

		public override int GetYear(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			int era;
			return M_EraHandler.EraYear(out era, date);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			int year2 = M_CheckYMDEG(year, month, day, ref era);
			return CCGregorianCalendar.IsLeapDay(year2, month, day);
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			M_CheckYMEG(year, month, ref era);
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			int year2 = M_CheckYEG(year, ref era);
			return CCGregorianCalendar.is_leap_year(year2);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			int year2 = M_CheckYMDEG(year, month, day, ref era);
			M_CheckHMSM(hour, minute, second, millisecond);
			return CCGregorianCalendar.ToDateTime(year2, month, day, hour, minute, second, millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			return base.ToFourDigitYear(year);
		}
	}
}
