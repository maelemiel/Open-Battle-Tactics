namespace System.Globalization
{
	internal class CCHebrewCalendar
	{
		public enum Month
		{
			nisan = 1,
			iyyar = 2,
			sivan = 3,
			tammuz = 4,
			av = 5,
			elul = 6,
			tishri = 7,
			heshvan = 8,
			kislev = 9,
			teveth = 10,
			shevat = 11,
			adar = 12,
			adar_I = 12,
			adar_II = 13
		}

		private const int epoch = -1373427;

		public static bool is_leap_year(int year)
		{
			return CCMath.mod(7 * year + 1, 19) < 7;
		}

		public static int last_month_of_year(int year)
		{
			return (!is_leap_year(year)) ? 12 : 13;
		}

		public static int elapsed_days(int year)
		{
			int num = CCMath.div(235 * year - 234, 19);
			int remainder;
			int num2 = CCMath.div_mod(out remainder, num, 1080);
			int x = 204 + 793 * remainder;
			int x2 = 11 + 12 * num + 793 * num2 + CCMath.div(x, 1080);
			int num3 = 29 * num + CCMath.div(x2, 24);
			if (CCMath.mod(3 * (num3 + 1), 7) < 3)
			{
				num3++;
			}
			return num3;
		}

		public static int new_year_delay(int year)
		{
			int num = elapsed_days(year);
			int num2 = elapsed_days(year + 1);
			if (num2 - num == 356)
			{
				return 2;
			}
			int num3 = elapsed_days(year - 1);
			if (num - num3 == 382)
			{
				return 1;
			}
			return 0;
		}

		public static int last_day_of_month(int month, int year)
		{
			switch (month)
			{
			default:
				throw new ArgumentOutOfRangeException("month", "Month should be between One and Thirteen.");
			case 2:
				return 29;
			case 4:
				return 29;
			case 6:
				return 29;
			case 8:
				if (!long_heshvan(year))
				{
					return 29;
				}
				break;
			case 9:
				if (short_kislev(year))
				{
					return 29;
				}
				break;
			case 10:
				return 29;
			case 12:
				if (!is_leap_year(year))
				{
					return 29;
				}
				break;
			case 13:
				return 29;
			case 1:
			case 3:
			case 5:
			case 7:
			case 11:
				break;
			}
			return 30;
		}

		public static bool long_heshvan(int year)
		{
			return CCMath.mod(days_in_year(year), 10) == 5;
		}

		public static bool short_kislev(int year)
		{
			return CCMath.mod(days_in_year(year), 10) == 3;
		}

		public static int days_in_year(int year)
		{
			return fixed_from_dmy(1, 7, year + 1) - fixed_from_dmy(1, 7, year);
		}

		public static int fixed_from_dmy(int day, int month, int year)
		{
			int num = -1373428;
			num += elapsed_days(year);
			num += new_year_delay(year);
			if (month < 7)
			{
				int num2 = last_month_of_year(year);
				for (int i = 7; i <= num2; i++)
				{
					num += last_day_of_month(i, year);
				}
				for (int i = 1; i < month; i++)
				{
					num += last_day_of_month(i, year);
				}
			}
			else
			{
				for (int i = 7; i < month; i++)
				{
					num += last_day_of_month(i, year);
				}
			}
			return num + day;
		}

		public static int year_from_fixed(int date)
		{
			int num = (int)Math.Floor((double)(date - -1373427) / 365.24682220597794);
			int i;
			for (i = num; date >= fixed_from_dmy(1, 7, i); i++)
			{
			}
			return i - 1;
		}

		public static void my_from_fixed(out int month, out int year, int date)
		{
			year = year_from_fixed(date);
			int num = ((date >= fixed_from_dmy(1, 1, year)) ? 1 : 7);
			month = num;
			while (date > fixed_from_dmy(last_day_of_month(month, year), month, year))
			{
				month++;
			}
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
			return date_difference(1, 7, year, day, month, year) + 1;
		}

		public static int days_remaining(int day, int month, int year)
		{
			return date_difference(day, month, year, 1, 7, year + 1) - 1;
		}
	}
}
