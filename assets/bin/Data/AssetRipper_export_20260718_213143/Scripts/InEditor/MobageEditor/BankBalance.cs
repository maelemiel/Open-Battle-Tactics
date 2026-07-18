using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class BankBalance
	{
		public static void GetBalanceWithCallback(Action<SimpleAPIStatus, Error, int, string, string> completeCB)
		{
			MobageRequest mobageRequest = MobageRequest.NewBankRequestWithContext(MobageSession.CurrentSession);
			string appId = Mobage.sharedInstance.AppId;
			if (appId != null)
			{
				appId = appId.ToLower();
				if (appId.Contains("-android"))
				{
					mobageRequest.APIMethod = "bank/balance?os=android";
				}
				else
				{
					mobageRequest.APIMethod = "bank/balance?os=ios";
				}
			}
			mobageRequest.HTTPMethod = "GET";
			mobageRequest.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (data != null && error == null)
				{
					completeCB(SimpleAPIStatus.Success, null, (int)data["balance"], (string)data["currency"], (string)data["currency_icon"]);
				}
				else
				{
					Debug.LogError("Mobage: GetBalance Error " + error.localizedDescription);
					completeCB(SimpleAPIStatus.Error, error, -1, null, null);
				}
			});
		}
	}
}
