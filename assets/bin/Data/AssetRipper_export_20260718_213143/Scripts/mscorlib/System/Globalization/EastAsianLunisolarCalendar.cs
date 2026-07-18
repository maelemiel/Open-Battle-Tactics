using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public abstract class EastAsianLunisolarCalendar : Calendar
	{
		internal readonly CCEastAsianLunisolarEraHandler M_EraHandler;

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

		internal virtual int ActualCurrentEra
		{
			get
			{
				return 1;
			}
		}

		public override CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.LunisolarCalendar;
			}
		}

		internal EastAsianLunisolarCalendar(CCEastAsianLunisolarEraHandler eraHandler)
		{
			M_EraHandler = eraHandler;
		}

		internal void M_CheckDateTime(DateTime time)
		{
			M_EraHandler.CheckDateTime(time);
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = ActualCurrentEra;
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

		[MonoTODO]
		public override DateTime AddMonths(DateTime time, int months)
		{
			DateTime dateTime = CCEastAsianLunisolarCalendar.AddMonths(time, months);
			M_CheckDateTime(dateTime);
			return dateTime;
		}

		[MonoTODO]
		public override DateTime AddYears(DateTime time, int years)
		{
			DateTime dateTime = CCEastAsianLunisolarCalendar.AddYears(time, years);
			M_CheckDateTime(dateTime);
			return dateTime;
		}

		[MonoTODO]
		public override int GetDayOfMonth(DateTime time)
		{
			M_CheckDateTime(time);
			return CCEastAsianLunisolarCalendar.GetDayOfMonth(time);
		}

		[MonoTODO]
		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			M_CheckDateTime(time);
			int date = CCFixed.FromDateTime(time);
			return CCFixed.day_of_week(date);
		}

		[MonoTODO]
		public override int GetDayOfYear(DateTime time)
		{
			M_CheckDateTime(time);
			return CCEastAsianLunisolarCalendar.GetDayOfYear(time);
		}

		[MonoTODO]
		public override int GetDaysInMonth(int year, int month, int era)
		{
			int gyear = M_CheckYMEG(year, month, ref era);
			return CCEastAsianLunisolarCalendar.GetDaysInMonth(gyear, month);
		}

		[MonoTODO]
		public override int GetDaysInYear(int year, int era)
		{
			int year2 = M_CheckYEG(year, ref era);
			return CCEastAsianLunisolarCalendar.GetDaysInYear(year2);
		}

		[MonoTODO]
		public override int GetLeapMonth(int year, int era)
		{
			return base.GetLeapMonth(year, era);
		}

		[MonoTODO]
		public override int GetMonth(DateTime time)
		{
			M_CheckDateTime(time);
			return CCEastAsianLunisolarCalendar.GetMonth(time);
		}

		[MonoTODO]
		public override int GetMonthsInYear(int year, int era)
		{
			M_CheckYE(year, ref era);
			return (!IsLeapYear(year, era)) ? 12 : 13;
		}

		public override int GetYear(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			int era;
			return M_EraHandler.EraYear(out era, date);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			int gyear = M_CheckYMDEG(year, month, day, ref era);
			return CCEastAsianLunisolarCalendar.IsLeapMonth(gyear, month);
		}

		[MonoTODO]
		public override bool IsLeapMonth(int year, int month, int era)
		{
			int gyear = M_CheckYMEG(year, month, ref era);
			return CCEastAsianLunisolarCalendar.IsLeapMonth(gyear, month);
		}

		public override bool IsLeapYear(int year, int era)
		{
			int gyear = M_CheckYEG(year, ref era);
			return CCEastAsianLunisolarCalendar.IsLeapYear(gyear);
		}

		[MonoTODO]
		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			int year2 = M_CheckYMDEG(year, month, day, ref era);
			M_CheckHMSM(hour, minute, second, millisecond);
			return CCGregorianCalendar.ToDateTime(year2, month, day, hour, minute, second, millisecond);
		}

		[MonoTODO]
		public override int ToFourDigitYear(int year)
		{
			if (year < 0)
			{
				throw new ArgumentOutOfRangeException("year", "Non-negative number required.");
			}
			int era = 0;
			M_CheckYE(year, ref era);
			return year;
		}

		public int GetCelestialStem(int sexagenaryYear)
		{
			if (sexagenaryYear < 1 || 60 < sexagenaryYear)
			{
				throw new ArgumentOutOfRangeException("sexagendaryYear is less than 0 or greater than 60");
			}
			return (sexagenaryYear - 1) % 10 + 1;
		}

		public virtual int GetSexagenaryYear(DateTime time)
		{
			return (GetYear(time) - 1900) % 60;
		}

		public int GetTerrestrialBranch(int sexagenaryYear)
		{
			if (sexagenaryYear < 1 || 60 < sexagenaryYear)
			{
				throw new ArgumentOutOfRangeException("sexagendaryYear is less than 0 or greater than 60");
			}
			return (sexagenaryYear - 1) % 12 + 1;
		}
	}
}
