namespace System.Globalization
{
	internal class CCJulianCalendar
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

		private const int epoch = -1;

		public static bool is_leap_year(int year)
		{
			return CCMath.mod(year, 4) == ((year <= 0) ? 3 : 0);
		}

		public static int fixed_from_dmy(int day, int month, int year)
		{
			int num = ((year >= 0) ? year : (year + 1));
			int num2 = -2;
			num2 += 365 * (num - 1);
			num2 += CCMath.div(num - 1, 4);
			num2 += CCMath.div(367 * month - 362, 12);
			if (month > 2)
			{
				num2 += ((!is_leap_year(year)) ? (-2) : (-1));
			}
			return num2 + day;
		}

		public static int year_from_fixed(int date)
		{
			int num = CCMath.div(4 * (date - -1) + 1464, 1461);
			return (num > 0) ? num : (num - 1);
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
	}
}
