using UnityEngine;

namespace MobageEditor
{
	public class BankViewController : ExperienceViewController
	{
		public static BankViewController Instance = new BankViewController();

		private string storevisitid;

		public string Storvisitid
		{
			set
			{
				Debug.Log("BankViewController storvisitid=" + value);
				storevisitid = value;
			}
		}

		private BankViewController()
		{
		}
	}
}
