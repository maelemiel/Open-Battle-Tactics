using System;
using System.Linq;

namespace MobageEditor
{
	public class BankEvent : AnalyticsEvent
	{
		public static string Evpi_err = "err";

		public static string Evpi_sku = "sku";

		public static string Evpi_vcur = "vcur";

		public static string Evpi_transactionid = "transactionid";

		public static string Evpi_oldbalance = "oldbalance";

		public static string Evpi_newbalance = "newbalance";

		public static string Storvisitid
		{
			get
			{
				string element = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
				int count = 26;
				Random random = new Random();
				return new string((from s in Enumerable.Repeat(element, count)
					select s[random.Next(s.Length)]).ToArray());
			}
		}

		public BankEvent(string eventId, JsonData payload)
			: base(eventId, "REV", "PC", payload)
		{
		}
	}
}
