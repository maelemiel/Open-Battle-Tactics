using System;
using System.Collections;
using System.Text.RegularExpressions;
using Facebook;
using LitJson0;
using UnityEngine;

public class SocialManager : Singleton<SocialManager>
{
	private enum SocialNetwork
	{
		Facebook = 0
	}

	public enum ResponseServerType
	{
		Success = 0,
		Fail = 1,
		Cancel = 2,
		ErrorConnection = 3,
		UserDeniedApp = 4
	}

	public class SocialDataModel
	{
		public string id;

		public ResponseServerType type;

		public JsonObject infoDictionary;
	}

	public class InviteFacebookDataModel : SocialDataModel
	{
		public string[] users;
	}

	public class FacebookFriendsDataModel : SocialDataModel
	{
		public ArrayList users;
	}

	public class FacebookFriend
	{
		public int rank;

		public string nickname;

		public float score;

		public string avatar;
	}

	protected bool facebookLoginProcessingFinished;

	protected bool isProccesingLogin;

	protected ResponseServerType responseServerType;

	public Action<SocialDataModel> actionSucess;

	public Action<ResponseServerType> loginSuccess;

	protected Action<string> onFacebookIdUpdated;

	protected bool hasFacebookIdUpdated;

	public void ConnectToFacebook()
	{
		if (!isProccesingLogin)
		{
			Log.Debug("SocialManager.ConnectToFacebook");
			loginSuccess = (Action<ResponseServerType>)Delegate.Combine(loginSuccess, new Action<ResponseServerType>(LoginFacebookCallback));
			SetOnFacebookIdUpdatedAction(SetFacebookUserIdCallback);
			FacebookLogin();
		}
	}

	public string GetFacebookAccessToken()
	{
		return Singleton<SocialApiAdapter>.instance.GetFacebookAccessToken();
	}

	public void FacebookLogin()
	{
		isProccesingLogin = true;
		Log.Debug("SocialManager.FacebookLogin");
		string scope = "email";
		if (!Singleton<SocialApiAdapter>.instance.IsFacebookConnected())
		{
			Log.Debug("SocialManager.FacebookLogin: connecting");
			Singleton<SocialApiAdapter>.instance.LoginWithReadPermissions(scope, FacebookLoginCallback);
		}
		else
		{
			Log.Debug("SocialManager.FacebookLogin: connected");
			SocialNetworkLoginSuccess(ResponseServerType.Success);
			Singleton<SocialApiAdapter>.instance.ReauthorizeWithPublishPermissions(scope, FacebookReauthorizeCallback);
		}
	}

	protected void SetFacebookUserIdCallback(string id)
	{
		Log.Debug("SocialManager.SetFacebookUserIdCallback {0}", id);
	}

	protected void LoginFacebookCallback(ResponseServerType responseServerType)
	{
		Log.Debug("SocialManager.LoginFacebookCallback");
		isProccesingLogin = false;
	}

	public void SetOnFacebookIdUpdatedAction(Action<string> action)
	{
		onFacebookIdUpdated = action;
	}

	public void ConfigureFacebook(InitDelegate callback)
	{
		Log.Debug("SocialManager.ConfigureFacebook");
		Singleton<SocialApiAdapter>.instance.InitFacebook(callback);
	}

	public bool IsFacebookConnected()
	{
		return Singleton<SocialApiAdapter>.instance.IsFacebookConnected();
	}

	protected void FacebookLoginCallback(FBResult result)
	{
		JsonObject jsonObject = new JsonObject(result.Text);
		if (!jsonObject.GetBooleanOrDefault("is_logged_in", false))
		{
			responseServerType = ResponseServerType.Cancel;
			facebookLoginProcessingFinished = true;
			SocialNetworkLoginSuccess(responseServerType);
		}
		else if (result.Error != null)
		{
			Log.Debug("SocialManager.FacebookLoginFailed: " + result.Error);
			Singleton<SocialApiAdapter>.instance.FacebookLogout();
			if (result.Error.Equals("The user denied the app"))
			{
				responseServerType = ResponseServerType.UserDeniedApp;
			}
			else if (result.Error.Contains("refused"))
			{
				responseServerType = ResponseServerType.Cancel;
			}
			else if (result.Error.Contains("api.facebook.com"))
			{
				responseServerType = ResponseServerType.Cancel;
			}
			else if (result.Error.Contains("canceled"))
			{
				responseServerType = ResponseServerType.Cancel;
			}
			else
			{
				responseServerType = ResponseServerType.Fail;
			}
			facebookLoginProcessingFinished = true;
			SocialNetworkLoginSuccess(responseServerType);
		}
		else
		{
			Log.Debug("SocialManager.FacebookLoginSucceed");
			FacebookReauthorizeCallback(result);
		}
	}

	protected void FacebookReauthorizeCallback(FBResult result)
	{
		if (result.Error != null)
		{
			return;
		}
		Log.Debug("SocialManager.FacebookReauthorizeSucceed");
		facebookLoginProcessingFinished = true;
		SocialNetworkLoginSuccess(ResponseServerType.Success);
		if (onFacebookIdUpdated == null || hasFacebookIdUpdated)
		{
			return;
		}
		hasFacebookIdUpdated = true;
		GetOwnFacebookId(delegate(SocialDataModel model)
		{
			if (!string.IsNullOrEmpty(model.id))
			{
				onFacebookIdUpdated(model.id);
			}
		});
	}

	protected void FacebookDialogCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Log.Debug(" SocialManager.FacebookDialogFailed: " + result.Error);
			SocialDataModel socialDataModel = new SocialDataModel();
			if (result.Error.Contains("URL"))
			{
				socialDataModel.type = ResponseServerType.ErrorConnection;
			}
			else if (result.Error.Equals(string.Empty))
			{
				socialDataModel.type = ResponseServerType.Cancel;
			}
			else
			{
				socialDataModel.type = ResponseServerType.Fail;
			}
			SocialNetworkShareSuccess(socialDataModel);
			return;
		}
		Log.Debug(" SocialManager.FacebookDialogSucceed: " + result.Text);
		SocialDataModel socialDataModel2 = new SocialDataModel();
		if (result.Text.Contains("post_id="))
		{
			socialDataModel2.type = ResponseServerType.Success;
		}
		else if (result.Text.Contains("request="))
		{
			InviteFacebookDataModel inviteFacebookDataModel = new InviteFacebookDataModel();
			inviteFacebookDataModel.type = ResponseServerType.Success;
			inviteFacebookDataModel.users = UsersFrom(result.Text);
			socialDataModel2 = inviteFacebookDataModel;
		}
		else if (result.Text.Equals(string.Empty))
		{
			socialDataModel2.type = ResponseServerType.Cancel;
		}
		SocialNetworkShareSuccess(socialDataModel2);
	}

	protected void SocialNetworkLoginSuccess(ResponseServerType type)
	{
		Log.Debug("SocialManager.SocialNetworkLoginSuccess loginSucess:" + loginSuccess);
		if (loginSuccess != null)
		{
			loginSuccess(type);
		}
		loginSuccess = null;
	}

	public void GetOwnFacebookId(Action<SocialDataModel> action)
	{
		GetOwnFacebookInfo(null, action);
	}

	protected string[] UsersFrom(string message)
	{
		string[] array = Regex.Split(message, "=");
		string[] array2 = new string[array.Length - 2];
		for (int i = 2; i < array.Length; i++)
		{
			array2[i - 2] = Regex.Split(array[i], "&")[0];
		}
		return array2;
	}

	protected void SocialNetworkShareSuccess(SocialDataModel model)
	{
		Log.Debug("SocialManager.SocialNetworkShareSuccess actionSucess: " + actionSucess);
		if (actionSucess != null)
		{
			actionSucess(model);
		}
		actionSucess = null;
	}

	protected IEnumerator FacebookOpenGraph(string url, HttpMethod type, Action<string, JsonObject> callback)
	{
		Log.Debug("SocialManager.FacebookOpenGraph");
		FacebookFriendsDataModel model = new FacebookFriendsDataModel();
		while (!Singleton<SocialApiAdapter>.instance.IsFacebookConnected() && !facebookLoginProcessingFinished)
		{
			yield return new WaitForSeconds(0f);
		}
		Log.Debug("SocialManager.FacebookOpenGraph session is valid or facebookLoginProcessingFinished");
		if (Singleton<SocialApiAdapter>.instance.IsFacebookConnected())
		{
			Log.Debug("Facebook.instance.graphRequest: {0}", url + "&access_token=" + Singleton<SocialApiAdapter>.instance.GetFacebookAccessToken());
			Action<string, JsonObject> callback2 = default(Action<string, JsonObject>);
			FB.API(url + "&access_token=" + Singleton<SocialApiAdapter>.instance.GetFacebookAccessToken(), type, delegate(FBResult result)
			{
				if (result.Error == null)
				{
					JsonObject arg = new JsonObject(result.Text);
					callback2(result.Error, arg);
				}
				else
				{
					callback2(result.Error, null);
				}
			});
		}
		else
		{
			model.type = responseServerType;
			SocialNetworkShareSuccess(model);
		}
	}

	public void GetOwnFacebookInfo(string[] aditionalFields, Action<SocialDataModel> action)
	{
		Log.Debug("SocialManager.GetOwnFacebookId");
		facebookLoginProcessingFinished = false;
		string text = "?fields=id";
		if (aditionalFields != null)
		{
			foreach (string text2 in aditionalFields)
			{
				text = text + "," + text2;
			}
		}
		StartCoroutine(FacebookOpenGraph("me" + text, HttpMethod.GET, delegate(string error, JsonObject answer)
		{
			SocialDataModel socialDataModel = new SocialDataModel();
			Debug.Log("me Graph Request finished.");
			if (error == null && answer != null)
			{
				Log.Debug("SocialManager.GetOwnFacebookInfo result: {0}", answer);
				socialDataModel.type = ResponseServerType.Success;
				socialDataModel.id = answer.GetString("id");
				socialDataModel.infoDictionary = answer;
				Log.Debug("SocialManager.GetOwnFacebookInfo: {0}", socialDataModel.id);
			}
			else
			{
				Log.Debug("SocialManager.GetOwnFacebookInfo error: {0}", error);
				socialDataModel.type = ResponseServerType.Fail;
			}
			Log.Debug("SocialManager.GetOwnFacebookInfo callback: {0}", socialDataModel);
			action(socialDataModel);
		}));
	}

	public void FacebookUserLikePage(string pageId, Action<bool> action)
	{
	}

	public void RequestFacebook(string message, string title, string[] to, Action<SocialDataModel> action)
	{
		actionSucess = action;
		Log.Debug("SocialManager.RequestFacebook");
		StartCoroutine(FacebookAPI(delegate
		{
			FB.AppRequest(message, to, null, null, null, string.Empty, title, FacebookDialogCallback);
		}));
	}

	protected IEnumerator FacebookAPI(Action apiCall)
	{
		Log.Debug("SocialManager.FacebookApi");
		FacebookLogin();
		while (!Singleton<SocialApiAdapter>.instance.IsFacebookConnected() && !facebookLoginProcessingFinished)
		{
			yield return new WaitForFixedUpdate();
		}
		Log.Debug(" SocialManager.FacebookAPI session is valid or facebookLoginProcessingFinished");
		if (Singleton<SocialApiAdapter>.instance.IsFacebookConnected())
		{
			apiCall();
			yield break;
		}
		SocialNetworkShareSuccess(new SocialDataModel
		{
			type = responseServerType
		});
	}
}
