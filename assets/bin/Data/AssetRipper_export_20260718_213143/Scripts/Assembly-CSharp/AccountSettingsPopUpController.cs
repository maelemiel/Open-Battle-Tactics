using System;
using LCD;
using UnityEngine;

public class AccountSettingsPopUpController : PopupController
{
	private const string BUTTON_ENABLED_SPRITE_NAME = "Secondary_button_down";

	private const string BUTTON_DISABLED_SPRITE_NAME = "Secondary_button_grey";

	[SerializeField]
	private tk2dUIUpDownButton logoutButton;

	[SerializeField]
	private tk2dUIUpDownButton upgradeAccountButton;

	[SerializeField]
	private tk2dTextMesh buildVersionString;

	[SerializeField]
	private tk2dTextMesh mobageIdString;

	[SerializeField]
	private tk2dTextMesh gitHashString;

	protected override void Start()
	{
		base.Start();
		if ((bool)buildVersionString)
		{
			buildVersionString.text = string.Format("ui_settings_version".Localize("Version: {0}"), string.Format("{0}.{1}", AppConfig.clientVersion, AppConfig.buildHash.Substring(0, 5)));
		}
		mobageIdString.text = string.Format("ui_settings_mobageid".Localize("UserID: {0}"), Singleton<LCDController>.instance.UserId);
		gitHashString.text = string.Empty;
	}

	public void OnClickMobage()
	{
		LoadingPopupManager.ShowLoadingPopup(0f);
	}

	public void OnClickUpgradeAccount()
	{
		PopupManager.ShowPopup(new PopupDataModel(), SceneTransitionManager.Scene.UpgradeUserAccountPopUpScene);
	}

	public void OnClickLogoutButton()
	{
	}

	public void OnClickTermsOfService()
	{
		Application.OpenURL("terms_of_service_url".Localize("http://app.mobage.com/terms"));
	}

	public void OnClickPrivacyPolicy()
	{
		Application.OpenURL("privacy_policy_url".Localize("http://app.mobage.com/privacy"));
	}

	public void OnClickSupport()
	{
		Application.OpenURL("support_url".Localize("http://support.mobage.com"));
	}

	public void OnLinkAccount()
	{
		Singleton<LCDController>.instance.OnLinkAccount();
	}

	private void OnLoadAccount()
	{
		PopupDataModel popupDatamodel = PopupDataModel.CancelOk(LocalizationManager.GetString("account_settings_load_account_confirmation_title", "Do you want to load another game?"), LocalizationManager.GetString("account_settings_load_account_confirmation_message", "WARNING : You will lose all progress done in this account!"), null);
		Action<long, long, LCDError> accountLoaded = delegate(long oldId, long newId, LCDError error)
		{
			if (error == null)
			{
				if (oldId != newId)
				{
					PopupManager.ShowPopup(PopupDataModel.Ok(LocalizationManager.GetString("account_settings_load_account_successful_title", "Success!"), LocalizationManager.GetString("account_settings_load_account_successful_message", "Your account is now linked, your game will close."), delegate
					{
						Application.Quit();
					}));
				}
			}
			else
			{
				Log.ErrorTag("Could not load the account error message: " + error.errorMessage, null, "AccountSettingsPopup");
				popupDatamodel.controller.OnCloseButton();
			}
		};
		Action rightAction = delegate
		{
			StartCoroutine(Singleton<LCDController>.instance.OnLoadAccount(accountLoaded));
			popupDatamodel.controller.OnCloseButton();
		};
		Action leftAction = delegate
		{
			popupDatamodel.controller.OnCloseButton();
		};
		popupDatamodel.rightAction = rightAction;
		popupDatamodel.leftAction = leftAction;
		PopupManager.ShowPopup(popupDatamodel);
	}

	private void SetButtonState(tk2dUIUpDownButton button, bool state)
	{
		if (!button)
		{
			return;
		}
		tk2dBaseSprite component = button.upStateGO.GetComponent<tk2dBaseSprite>();
		if ((bool)component)
		{
			button.uiItem.enabled = state;
			if (state)
			{
				component.SetSprite("Secondary_button_down");
			}
			else
			{
				component.SetSprite("Secondary_button_grey");
			}
		}
	}
}
