namespace Mobage
{
	public class BankHelper
	{
		private static BankHelper instance;

		public static BankHelper Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new BankHelper();
				}
				return instance;
			}
		}

		private BankHelper()
		{
		}

		public static void CacheCreditItemData()
		{
			Instance.RefreshCreditItemData();
		}

		public void RefreshCreditItemData()
		{
		}
	}
}
