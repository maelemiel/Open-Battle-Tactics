using System;
using System.Collections;
using Holoville.HOTween;
using MobageTools;
using UnityEngine;

public class LoginController : Singleton<LoginController>
{
	public enum LoginState
	{
		PreLogin = 0,
		Login = 1,
		PostLogin = 2
	}

	private LoginState _currentState;

	private bool _isLogged;

	private int _globalRetries;

	private Action _onSuccess;

	public static string WAIT_UNTIL_VALUE = "wait_login_until";

	private int loadingId = -1;

	public static bool IsLogged
	{
		get
		{
			return Singleton<LoginController>.instance._isLogged;
		}
	}

	public static void StartLoginProcess(Action cb)
	{
		Singleton<LoginController>.instance._onSuccess = cb;
		Singleton<LoginController>.instance.StartCoroutine(Singleton<LoginController>.instance.WaitForLCDSDK());
	}

	private IEnumerator WaitForLCDSDK()
	{
		while (string.IsNullOrEmpty(Singleton<LCDController>.instance.AccessToken))
		{
			yield return new WaitForSeconds(0f);
		}
		PreLogin();
	}

	private void PreLogin()
	{
		bool isUser = false;
		bool accountLinking = false;
		if (!CanUserTryLogin())
		{
			PopupDataModel popupModel = PopupDataModel.Ok("Server Error", "We're currently experiencing issues with our servers please wait some minutes and try again later.", Application.Quit);
			PopupManager.ShowPopup(popupModel);
			return;
		}
		Singleton<SessionManager>.instance.PreLoginLCD(Singleton<LCDController>.instance.UserId, delegate(ServerUtilities.BaseResponse response)
		{
			if (response == null || response.error != null)
			{
				Log.Error("Something went wrong with the PreLogin api call");
			}
			else
			{
				isUser = response.json != null && response.json.GetObject("result") != null && response.json.GetObject("result").GetBoolean("exist");
				accountLinking = response.json != null && response.json.GetObject("result") != null && response.json.GetObject("result").GetBoolean("accountLinking");
				_currentState = LoginState.Login;
				CacheManager.SetConstantString("lcd_account_linking", accountLinking.ToString().ToUpper());
				if (!isUser)
				{
					LoadLoginScene();
				}
				else
				{
					Login(string.Empty);
				}
			}
		});
	}

	public bool CanUserTryLogin()
	{
		KeyValueStorage keyValueStorage = KeyValueStorage.Instance(KeyValueStorage.Storage.USER_PROFILE);
		if (keyValueStorage != null)
		{
			long value = keyValueStorage.GetValue<long>(WAIT_UNTIL_VALUE);
			if (TimeManager.ServerTime < value)
			{
				Log.InfoTag("UserNeeds to wait to try pre login wait is " + (value - TimeManager.ServerTime), null, "LoginController");
				return false;
			}
			return true;
		}
		Log.ErrorTag("KeyValueStorage is null", null, "LoginController");
		return false;
	}

	public static void Login(string fbToken = "", Action callback = null)
	{
		Singleton<LoginController>.instance.ShowLoadingPopup();
		Singleton<SessionManager>.instance.LoginLCD(Singleton<LCDController>.instance.UserId, fbToken, delegate(ServerUtilities.BaseResponse response)
		{
			Singleton<LoginController>.instance.ClearLoadingPopup();
			if (response == null || response.error != null)
			{
				Log.Error("Something went wrong with the PreLogin api call");
			}
			else
			{
				Singleton<LoginController>.instance._currentState = LoginState.PostLogin;
				Singleton<LoginController>.instance._isLogged = true;
				Singleton<LoginController>.instance._onSuccess();
				if (callback != null)
				{
					callback();
				}
			}
		});
	}

	public void LoadLoginScene()
	{
		Application.LoadLevelAdditive(SceneTransitionManager.Scene.LoginScene.ToString());
		ClearLoadingPopup();
	}

	public static void OnLoginWithFacebook(Action callback = null)
	{
		Singleton<LoginController>.instance.FacebookLogin(0, callback);
	}

	public void FacebookLogin(int retries = 0, Action callback = null)
	{
		ShowLoadingPopup();
		FacebookHelper.Instance.EstablishFacebookSession(delegate(SocialManager.ResponseServerType facebookResponse)
		{
			switch (facebookResponse)
			{
			case SocialManager.ResponseServerType.Fail:
			case SocialManager.ResponseServerType.ErrorConnection:
				Log.ErrorTag("LoginController.FacebookLogin: " + facebookResponse, null, "LoginController");
				ShowFacebookConnectionErrorPopup(delegate
				{
					FacebookLogin(_globalRetries);
				});
				break;
			case SocialManager.ResponseServerType.Cancel:
			case SocialManager.ResponseServerType.UserDeniedApp:
				Log.WarningTag("FacebookLogin.FacebookLogin: " + facebookResponse, null, "LoginController");
				break;
			default:
			{
				string facebookAccessToken = Singleton<SocialApiAdapter>.instance.GetFacebookAccessToken();
				ClearLoadingPopup();
				Login(facebookAccessToken, callback);
				break;
			}
			}
		});
	}

	public static void OnLinkAccount()
	{
		Singleton<LCDController>.instance.OnLinkAccount();
	}

	public static void OnLoadAccount()
	{
		Singleton<LoginController>.instance.StartCoroutine(OnLoadAccountCoroutine());
	}

	public static IEnumerator OnLoadAccountCoroutine()
	{
		yield return Singleton<LoginController>.instance.StartCoroutine(Singleton<LCDController>.instance.OnLoadAccount());
		if (Singleton<LCDController>.instance.AccountLoaded)
		{
			Login(string.Empty);
		}
	}

	public void ShowFacebookConnectionErrorPopup(Action cb)
	{
		PopupManager.highDepThPopup = true;
		PopupManager.ShowPopup(PopupDataModel.FacebookConnectionError(delegate
		{
			if (cb != null)
			{
				cb();
			}
		}));
	}

	public void ShowUpgradePopup()
	{
		PopupManager.ShowPopup(new PopupDataModel(), SceneTransitionManager.Scene.UpgradeUserAccountPopUpScene);
	}

	public void ShowUpgradeUserConfirmationPopup()
	{
		PopupManager.highDepThPopup = true;
		PopupManager.ShowPopup(PopupDataModel.MobageUpgradeUserConfirmation(delegate
		{
			ShowUpgradePopup();
		}));
	}

	public void ShowUpgradeConflictPopup()
	{
		PopupManager.highDepThPopup = true;
		PopupManager.ShowPopup(PopupDataModel.MobageUpgradeConflict(delegate
		{
		}));
	}

	public void ShowUpgradeUserSuccessPopup()
	{
		PopupManager.highDepThPopup = true;
		PopupManager.ShowPopup(PopupDataModel.MobageUpgradeUserSuccess(delegate
		{
			PopupManager.DestroyAllPopups();
		}));
	}

	public void ClearAllLoadingPopups()
	{
		LoadingPopupManager.ClearAllLoadingPopups();
	}

	public void ShowLoadingPopup()
	{
		if (loadingId != -1)
		{
			LoadingPopupManager.ClearLoadingPopup(loadingId);
		}
		loadingId = LoadingPopupManager.ShowLoadingPopup(0f);
	}

	public void ClearLoadingPopup()
	{
		LoadingPopupManager.ClearLoadingPopup(loadingId);
		loadingId = -1;
	}

	public void LogoutBehaviour()
	{
		HOTween.Complete();
		QuitUtility.Restart();
	}

	public void ShowLoginScene()
	{
		Application.LoadLevelAdditive(SceneTransitionManager.Scene.LoginScene.ToString());
	}

	public static void UpdateFBCredentials()
	{
		string facebookAccessToken = Singleton<SocialManager>.instance.GetFacebookAccessToken();
	}
}
