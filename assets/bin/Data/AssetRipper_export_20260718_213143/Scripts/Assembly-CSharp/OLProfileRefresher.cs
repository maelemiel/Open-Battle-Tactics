using System.Collections;
using UnityEngine;

public class OLProfileRefresher : Singleton<OLProfileRefresher>
{
	public void InterstitialShown()
	{
		Log.DebugTag("OL InterstitialShown", null, "OtherLevels");
		StartCoroutine(RefreshUserProfile());
	}

	private IEnumerator RefreshUserProfile()
	{
		Log.DebugTag("RefreshUserProfile - wait", null, "OtherLevels");
		yield return new WaitForSeconds(Constants.OtherLevelsRefreshUserProfileSeconds);
		Log.DebugTag("RefreshUserProfile - request", null, "OtherLevels");
		SceneController.resumeCallbackEnable = true;
		if (UserProfile.player == null)
		{
			yield break;
		}
		Singleton<SessionManager>.instance.GetUser(delegate
		{
			Singleton<BankService>.instance.GetCurrencyBalance(delegate(int balance)
			{
				UserProfile.player.gems = balance;
			});
		});
	}
}
