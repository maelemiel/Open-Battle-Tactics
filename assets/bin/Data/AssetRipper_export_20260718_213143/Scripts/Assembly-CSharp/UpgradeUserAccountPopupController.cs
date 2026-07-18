using UnityEngine;

public class UpgradeUserAccountPopupController : PopupController
{
	[SerializeField]
	private tk2dTextMesh fbTextLabel;

	protected override void Start()
	{
		base.Start();
		fbTextLabel.text = LocalizationManager.GetString("ui_upgrade_account_facebook_message", "We will NOT spam your friends OR their wall. \nThat annoys us too. ");
	}

	private void UpgradeWithFacebookButton()
	{
		LoginController.OnLoginWithFacebook(CloseButton);
	}

	private void UpgradeWithMobageButton()
	{
	}

	private void CloseButton()
	{
		OnCloseButton();
	}

	public void StartOverAndLogin()
	{
		PopupManager.DestroyAllPopups();
		LoadingPopupManager.ClearAllLoadingPopups();
		SceneTransitionManager.PopToScene(SceneTransitionManager.Scene.TitleScene);
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		if (focusStatus)
		{
			LoadingPopupManager.ClearAllLoadingPopups();
		}
	}
}
