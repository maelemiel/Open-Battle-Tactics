using System.Collections;

namespace System.Globalization
{
	[Serializable]
	internal class CCEastAsianLunisolarEraHandler
	{
		[Serializable]
		private struct Era
		{
			private int _nr;

			private int _start;

			private int _gregorianYearStart;

			private int _end;

			private int _maxYear;

			public int Nr
			{
				get
				{
					return _nr;
				}
			}

			public Era(int nr, int start, int end)
			{
				if (nr == 0)
				{
					throw new ArgumentException("Era number shouldn't be zero.");
				}
				_nr = nr;
				if (start > end)
				{
					throw new ArgumentException("Era should start before end.");
				}
				_start = start;
				_end = end;
				_gregorianYearStart = CCGregorianCalendar.year_from_fixed(_start);
				int num = CCGregorianCalendar.year_from_fixed(_end);
				_maxYear = num - _gregorianYearStart + 1;
			}

			public int GregorianYear(int year)
			{
				if (year < 1 || year > _maxYear)
				{
					throw new ArgumentOutOfRangeException("year", string.Format("Valid Values are between {0} and {1}, inclusive.", 1, _maxYear));
				}
				return year + _gregorianYearStart - 1;
			}

			public bool Covers(int date)
			{
				return _start <= date && date <= _end;
			}

			public int EraYear(out int era, int date)
			{
				if (!Covers(date))
				{
					throw new ArgumentOutOfRangeException("date", "Time was out of Era range.");
				}
				int num = CCGregorianCalendar.year_from_fixed(date);
				era = _nr;
				return num - _gregorianYearStart + 1;
			}
		}

		private SortedList _Eras;

		public int[] Eras
		{
			get
			{
				int[] array = new int[_Eras.Count];
				for (int i = 0; i < _Eras.Count; i++)
				{
					array[i] = ((Era)_Eras.GetByIndex(i)).Nr;
				}
				return array;
			}
		}

		public CCEastAsianLunisolarEraHandler()
		{
			_Eras = new SortedList();
		}

		public void appendEra(int nr, int rd_start, int rd_end)
		{
			Era era = new Era(nr, rd_start, rd_end);
			_Eras[nr] = era;
		}

		public void appendEra(int nr, int rd_start)
		{
			appendEra(nr, rd_start, CCFixed.FromDateTime(DateTime.MaxValue));
		}

		public int GregorianYear(int year, int era)
		{
			return ((Era)_Eras[era]).GregorianYear(year);
		}

		public int EraYear(out int era, int date)
		{
			foreach (Era value in _Eras.Values)
			{
				if (value.Covers(date))
				{
					return value.EraYear(out era, date);
				}
			}
			throw new ArgumentOutOfRangeException("date", "Time value was out of era range.");
		}

		public void CheckDateTime(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			if (!ValidDate(date))
			{
				throw new ArgumentOutOfRangeException("time", "Time value was out of era range.");
			}
		}

		public bool ValidDate(int date)
		{
			foreach (Era value in _Eras.Values)
			{
				if (value.Covers(date))
				{
					return true;
				}
			}
			return false;
		}

		public bool ValidEra(int era)
		{
			return _Eras.Contains(era);
		}
	}
}
