using System;
using System.Collections;
using LCD;
using LCD.User;
using UnityEngine;

public class LCDController : Singleton<LCDController>, LCDSDK.EventHandler
{
	private static int loadingPopupId;

	private static bool accountLoaded;

	public string AccessToken
	{
		get
		{
			return LCDSDK.GetAccessToken();
		}
	}

	public string UserId
	{
		get
		{
			return LCDSDK.GetCurrentUser().userId.ToString();
		}
	}

	public bool AccountLoaded
	{
		get
		{
			return accountLoaded;
		}
		set
		{
			accountLoaded = value;
		}
	}

	public event Action<string> OnTokenUpdated;

	public void Awake()
	{
		Log.DebugTag("Init!", null, "LCDController");
		LCDSDK.Init(this);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		Log.DebugTag("OnApplicationPause : " + pauseStatus, null, "LCDController");
		if (pauseStatus)
		{
			LCDSDK.Pause();
		}
		else
		{
			LCDSDK.Resume();
		}
	}

	public void OnRemoteMessage(string message, string extra)
	{
		Log.DebugTag("Remote message :: message " + message + " extra: " + extra, null, "LCDController");
	}

	public void OnSDKWebViewProcess(LCDSDK.SDKWebViewProcess process)
	{
		if (process == LCDSDK.SDKWebViewProcess.STARTED && LCDSDK.GetAccessToken() != null)
		{
			ShowLoading();
		}
		else
		{
			HideLoading();
		}
		Log.DebugTag("Status: " + process, null, "LCDController");
	}

	public void OnSessionError(LCDError error)
	{
		Log.ErrorTag(string.Concat("LCD OnSessionError ErrorCode", error.errorCode, " ErrorType: ", error.errorType, " ErrorMessage: ", error.errorMessage), null, "LCDController");
		if (LCDSDK.GetAccessToken() == null || error.errorCode == -1)
		{
			LCDSDK.Resume();
		}
	}

	public void OnSessionUpdate(string accessToken, User user)
	{
		Log.InfoTag("OnSessionUpdate. User: " + user.userId, null, "LCDController");
		if (this.OnTokenUpdated != null)
		{
			this.OnTokenUpdated(accessToken);
		}
	}

	private void ShowLoading()
	{
		if (loadingPopupId == 0)
		{
			Log.DebugTag("LCD Showloading: ", null, "LCDController");
			loadingPopupId = LoadingPopupManager.ShowLoadingPopup(0f);
		}
	}

	private void HideLoading()
	{
		if (loadingPopupId != 0)
		{
			Log.DebugTag("LCD HideLoading: ", null, "LCDController");
			LoadingPopupManager.ClearLoadingPopup(loadingPopupId);
			loadingPopupId = 0;
		}
	}

	public void ForceTokenRefresh()
	{
		StartCoroutine(ForceTokenRefreshCoroutine());
	}

	private IEnumerator ForceTokenRefreshCoroutine()
	{
		bool tokenIsUpdated = false;
		Action<string> OnTokenUpdatedLocal = delegate
		{
			tokenIsUpdated = true;
		};
		LCDController lCDController = this;
		lCDController.OnTokenUpdated = (Action<string>)Delegate.Combine(lCDController.OnTokenUpdated, OnTokenUpdatedLocal);
		while (!tokenIsUpdated)
		{
			LCDSDK.Resume();
			yield return new WaitForSeconds(5f);
		}
		LCDController lCDController2 = this;
		lCDController2.OnTokenUpdated = (Action<string>)Delegate.Remove(lCDController2.OnTokenUpdated, OnTokenUpdatedLocal);
	}

	public void OnLinkAccount()
	{
		User currentUser = LCDSDK.GetCurrentUser();
		currentUser.LinkAccount(delegate(LCDError error)
		{
			if (error != null)
			{
				Log.ErrorTag("LCD OnLinkAccount: " + error.errorMessage, null, "LCDController");
			}
		});
	}

	public IEnumerator OnLoadAccount(Action<long, long, LCDError> cb = null)
	{
		User user = LCDSDK.GetCurrentUser();
		bool loadingAccountComplete = false;
		Action<long, long, LCDError> cb2 = default(Action<long, long, LCDError>);
		user.LoadAccount(delegate(long newUserId, long oldUserId, LCDError error)
		{
			if (error != null)
			{
				Log.ErrorTag("LCD OnLoadAccount Error: " + error.errorMessage, null, "LCDController");
			}
			else
			{
				Log.InfoTag("LCD OnLoadAccount: new UserId :: " + newUserId + " oldUser :: " + oldUserId, null, "LCDController");
				if (newUserId != oldUserId)
				{
					AccountLoaded = true;
				}
			}
			loadingAccountComplete = true;
			if (cb2 != null)
			{
				cb2(newUserId, oldUserId, error);
			}
		});
		while (!loadingAccountComplete)
		{
			yield return null;
		}
	}
}
