using UnityEngine;

public class AskForRating : MonoBehaviour
{
	private static bool shouldAskForTier(int tier)
	{
		int firstTierToAskForRating = Constants.FirstTierToAskForRating;
		int tierAskForRatingInterval = Constants.TierAskForRatingInterval;
		if (tier >= firstTierToAskForRating && (tier - 1) % tierAskForRatingInterval == 0)
		{
			return true;
		}
		return false;
	}

	public static bool ShouldPromptForRating()
	{
		if (UserProfile.player.tutorial.HasRated || UserProfile.player.energy > 0)
		{
			return false;
		}
		int lastTierAskedRating = UserProfile.player.tutorial.LastTierAskedRating;
		int num = int.Parse(UserProfile.player.CurrentDivision.id);
		return shouldAskForTier(num) && lastTierAskedRating != num;
	}

	public static void PromptForRating()
	{
		PopupManager.ShowPopup(PopupDataModel.Full("ui_ask_rate_title".Localize("We Need Your Help!"), "ui_ask_rate_description".Localize("Your reviews help us to continue improving Super Battle Tactics!"), "ui_ask_rate_no".Localize("Maybe Later"), delegate
		{
			UserProfile.player.tutorial.LastTierAskedRating = int.Parse(UserProfile.player.CurrentDivision.id);
		}, "ui_ask_rate_yes".Localize("Review Now"), delegate
		{
			Application.OpenURL("ui_ask_rating_url_android".Localize("market://details?id=com.mobage.ww.a1933.Super_Battle_Tactics_Android"));
			UserProfile.player.tutorial.HasRated = true;
		}).ShowCloseButton(false));
	}
}
