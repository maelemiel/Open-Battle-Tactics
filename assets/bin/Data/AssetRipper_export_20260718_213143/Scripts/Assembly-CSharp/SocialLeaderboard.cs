using System;
using UnityEngine;

public class SocialLeaderboard
{
	private static bool IsLoggedIn()
	{
		return Social.localUser.authenticated;
	}

	public static void Authenticate(Action<bool> onSuccess)
	{
		Social.localUser.Authenticate(onSuccess);
	}

	public static void DisplayLeaderboards()
	{
		if (IsLoggedIn())
		{
			Social.ShowLeaderboardUI();
			return;
		}
		Authenticate(delegate(bool success)
		{
			if (success)
			{
				Social.ShowLeaderboardUI();
			}
			else
			{
				string title = "ui_gamecenter_error_title".Localize("Game Center Error");
				string message = "ui_gamecenter_error_desc".Localize("Could not connect, please try again");
				string buttonLabel = "ui_gamecenter_error_button_label".Localize("OK");
				PopupManager.ShowPopup(PopupDataModel.One(title, message, buttonLabel, delegate
				{
					Social.localUser.Authenticate(null);
				}).ShowCloseButton(false));
			}
		});
	}

	public static void ReportTotalWins(int score)
	{
	}

	public static void ReportTotalKills(int score)
	{
	}
}
