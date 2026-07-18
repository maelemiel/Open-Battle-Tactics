namespace System.Globalization
{
	internal class CCGregorianCalendar
	{
		public enum Month
		{
			january = 1,
			february = 2,
			march = 3,
			april = 4,
			may = 5,
			june = 6,
			july = 7,
			august = 8,
			september = 9,
			october = 10,
			november = 11,
			december = 12
		}

		private const int epoch = 1;

		public static bool is_leap_year(int year)
		{
			if (CCMath.mod(year, 4) != 0)
			{
				return false;
			}
			switch (CCMath.mod(year, 400))
			{
			case 100:
				return false;
			case 200:
				return false;
			case 300:
				return false;
			default:
				return true;
			}
		}

		public static int fixed_from_dmy(int day, int month, int year)
		{
			int num = 0;
			num += 365 * (year - 1);
			num += CCMath.div(year - 1, 4);
			num -= CCMath.div(year - 1, 100);
			num += CCMath.div(year - 1, 400);
			num += CCMath.div(367 * month - 362, 12);
			if (month > 2)
			{
				num += ((!is_leap_year(year)) ? (-2) : (-1));
			}
			return num + day;
		}

		public static int year_from_fixed(int date)
		{
			int remainder = date - 1;
			int num = CCMath.div_mod(out remainder, remainder, 146097);
			int num2 = CCMath.div_mod(out remainder, remainder, 36524);
			int num3 = CCMath.div_mod(out remainder, remainder, 1461);
			int num4 = CCMath.div(remainder, 365);
			int num5 = 400 * num + 100 * num2 + 4 * num3 + num4;
			return (num2 != 4 && num4 != 4) ? (num5 + 1) : num5;
		}

		public static void my_from_fixed(out int month, out int year, int date)
		{
			year = year_from_fixed(date);
			int num = date - fixed_from_dmy(1, 1, year);
			int num2 = ((date >= fixed_from_dmy(1, 3, year)) ? (is_leap_year(year) ? 1 : 2) : 0);
			month = CCMath.div(12 * (num + num2) + 373, 367);
		}

		public static void dmy_from_fixed(out int day, out int month, out int year, int date)
		{
			my_from_fixed(out month, out year, date);
			day = date - fixed_from_dmy(1, month, year) + 1;
		}

		public static int month_from_fixed(int date)
		{
			int month;
			int year;
			my_from_fixed(out month, out year, date);
			return month;
		}

		public static int day_from_fixed(int date)
		{
			int day;
			int month;
			int year;
			dmy_from_fixed(out day, out month, out year, date);
			return day;
		}

		public static int date_difference(int dayA, int monthA, int yearA, int dayB, int monthB, int yearB)
		{
			return fixed_from_dmy(dayB, monthB, yearB) - fixed_from_dmy(dayA, monthA, yearA);
		}

		public static int day_number(int day, int month, int year)
		{
			return date_difference(31, 12, year - 1, day, month, year);
		}

		public static int days_remaining(int day, int month, int year)
		{
			return date_difference(day, month, year, 31, 12, year);
		}

		public static DateTime AddMonths(DateTime time, int months)
		{
			int date = CCFixed.FromDateTime(time);
			int day;
			int month;
			int year;
			dmy_from_fixed(out day, out month, out year, date);
			month += months;
			year += CCMath.div_mod(out month, month, 12);
			int daysInMonth = GetDaysInMonth(year, month);
			if (day > daysInMonth)
			{
				day = daysInMonth;
			}
			date = fixed_from_dmy(day, month, year);
			return CCFixed.ToDateTime(date).Add(time.TimeOfDay);
		}

		public static DateTime AddYears(DateTime time, int years)
		{
			int date = CCFixed.FromDateTime(time);
			int day;
			int month;
			int year;
			dmy_from_fixed(out day, out month, out year, date);
			year += years;
			int daysInMonth = GetDaysInMonth(year, month);
			if (day > daysInMonth)
			{
				day = daysInMonth;
			}
			date = fixed_from_dmy(day, month, year);
			return CCFixed.ToDateTime(date).Add(time.TimeOfDay);
		}

		public static int GetDayOfMonth(DateTime time)
		{
			return day_from_fixed(CCFixed.FromDateTime(time));
		}

		public static int GetDayOfYear(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int year = year_from_fixed(num);
			int num2 = fixed_from_dmy(1, 1, year);
			return num - num2 + 1;
		}

		public static int GetDaysInMonth(int year, int month)
		{
			int num = fixed_from_dmy(1, month, year);
			int num2 = fixed_from_dmy(1, month + 1, year);
			return num2 - num;
		}

		public static int GetDaysInYear(int year)
		{
			int num = fixed_from_dmy(1, 1, year);
			int num2 = fixed_from_dmy(1, 1, year + 1);
			return num2 - num;
		}

		public static int GetMonth(DateTime time)
		{
			return month_from_fixed(CCFixed.FromDateTime(time));
		}

		public static int GetYear(DateTime time)
		{
			return year_from_fixed(CCFixed.FromDateTime(time));
		}

		public static bool IsLeapDay(int year, int month, int day)
		{
			return is_leap_year(year) && month == 2 && day == 29;
		}

		public static DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int milliseconds)
		{
			return CCFixed.ToDateTime(fixed_from_dmy(day, month, year), hour, minute, second, milliseconds);
		}
	}
}
