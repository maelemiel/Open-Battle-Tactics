namespace System.Globalization
{
	[Serializable]
	public class KoreanLunisolarCalendar : EastAsianLunisolarCalendar
	{
		public const int GregorianEra = 1;

		internal static readonly CCEastAsianLunisolarEraHandler era_handler;

		private static DateTime KoreanMin;

		private static DateTime KoreanMax;

		public override int[] Eras
		{
			get
			{
				return (int[])era_handler.Eras.Clone();
			}
		}

		public override DateTime MinSupportedDateTime
		{
			get
			{
				return KoreanMin;
			}
		}

		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return KoreanMax;
			}
		}

		[MonoTODO]
		public KoreanLunisolarCalendar()
			: base(era_handler)
		{
		}

		static KoreanLunisolarCalendar()
		{
			KoreanMin = new DateTime(918, 2, 14, 0, 0, 0);
			KoreanMax = new DateTime(2051, 2, 10, 23, 59, 59);
			era_handler = new CCEastAsianLunisolarEraHandler();
			era_handler.appendEra(1, CCFixed.FromDateTime(new DateTime(1, 1, 1)));
		}

		public override int GetEra(DateTime time)
		{
			int date = CCFixed.FromDateTime(time);
			int era;
			era_handler.EraYear(out era, date);
			return era;
		}
	}
}
