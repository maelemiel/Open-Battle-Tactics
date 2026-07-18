using System;
using UnityEngine;

public class FacebookUIController : MonoBehaviour
{
	public static int RANKING_PAGE_SIZE = 10;

	public Action connectToFacebookCallback;

	public void ConnectToFacebookAction()
	{
		if (!Singleton<SocialManager>.instance.IsFacebookConnected())
		{
			ConnectToFacebook();
		}
	}

	protected void ConnectToFacebook()
	{
		SocialManager instance = Singleton<SocialManager>.instance;
		instance.loginSuccess = (Action<SocialManager.ResponseServerType>)Delegate.Combine(instance.loginSuccess, new Action<SocialManager.ResponseServerType>(LoginSuccessCallback));
		Singleton<SocialManager>.instance.SetOnFacebookIdUpdatedAction(SetFacebookUserIdCallback);
		Singleton<SocialManager>.instance.FacebookLogin();
	}

	protected void LoginSuccessCallback(SocialManager.ResponseServerType responseServerType)
	{
		Debug.Log("FacebookUIController.LoginSuccessCallback >>> responseServerType: " + responseServerType);
		Debug.Log("FacebookUIController.LoginSuccessCallback >>> FacebookAccessToken: " + Singleton<SocialManager>.instance.GetFacebookAccessToken());
		if (connectToFacebookCallback != null)
		{
			connectToFacebookCallback();
		}
	}

	protected void SetFacebookUserIdCallback(string id)
	{
		Log.Debug("FacebookUIController.SetFacebookUserIdCallback {0}", id);
		Singleton<SessionManager>.instance.SendFacebookCredentials(id, Singleton<SocialManager>.instance.GetFacebookAccessToken(), null);
	}

	public void SetConnectToFacebookCallback(Action cb)
	{
		connectToFacebookCallback = cb;
	}
}
