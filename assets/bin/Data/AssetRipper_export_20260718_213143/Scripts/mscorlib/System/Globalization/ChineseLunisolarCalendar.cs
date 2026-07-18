using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	public class ChineseLunisolarCalendar : EastAsianLunisolarCalendar
	{
		public const int ChineseEra = 1;

		internal static readonly CCEastAsianLunisolarEraHandler era_handler;

		private static DateTime ChineseMin;

		private static DateTime ChineseMax;

		[ComVisible(false)]
		public override int[] Eras
		{
			get
			{
				return (int[])era_handler.Eras.Clone();
			}
		}

		[ComVisible(false)]
		public override DateTime MinSupportedDateTime
		{
			get
			{
				return ChineseMin;
			}
		}

		[ComVisible(false)]
		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return ChineseMax;
			}
		}

		[MonoTODO]
		public ChineseLunisolarCalendar()
			: base(era_handler)
		{
		}

		static ChineseLunisolarCalendar()
		{
			ChineseMin = new DateTime(1901, 2, 19);
			ChineseMax = new DateTime(2101, 1, 28, 23, 59, 59, 999);
			era_handler = new CCEastAsianLunisolarEraHandler();
			era_handler.appendEra(1, CCFixed.FromDateTime(new DateTime(1, 1, 1)));
		}

		[ComVisible(false)]
		public override int GetEra(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			int era;
			era_handler.EraYear(out era, date);
			return era;
		}
	}
}
