using UnityEngine;

public class LikeUsFBPopUpController : PopupController
{
	protected override void Start()
	{
		base.Start();
	}

	private void LikeUsFBButton()
	{
		if (Singleton<SocialManager>.instance.IsFacebookConnected())
		{
			UserProfile.player.FacebookData.LikeFanPage = true;
			Application.OpenURL("fb://page/" + Constants.FacebookFanPageId);
			OnCloseButton();
		}
		else
		{
			PopupManager.ShowPopup(new PopupDataModel(), SceneTransitionManager.Scene.UpgradeUserAccountPopUpScene);
		}
	}

	private void NoThanksButton()
	{
		UserProfile.player.FacebookData.LastTimeLikeShow = 0L;
		OnCloseButton();
	}

	private void RemindMeLaterButton()
	{
		UserProfile.player.FacebookData.LastTimeLikeShow = TimeUtility.ServerTs;
		OnCloseButton();
	}
}
