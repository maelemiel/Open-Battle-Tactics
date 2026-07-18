using Facebook;

internal class SocialApiAdapter : Singleton<SocialApiAdapter>, ISocialApiAdapter
{
	public void InitFacebook(InitDelegate callback)
	{
		Log.Debug("SocialManager.InitFacebook");
		FB.Init(callback);
	}

	public void FacebookLogout()
	{
		Log.Debug("SocialManager.FacebookLogout");
		FB.Logout();
	}

	public bool IsFacebookConnected()
	{
		Log.Debug("SocialManager.IsFacebookConnected");
		return FB.IsLoggedIn;
	}

	public string GetFacebookAccessToken()
	{
		return FB.AccessToken;
	}

	public void LoginWithReadPermissions(string scope, FacebookDelegate callback)
	{
		Log.Debug("SocialApiAdapter.LoginWithReadPermissions");
		FB.Login(scope, callback);
	}

	public void ReauthorizeWithPublishPermissions(string scope, FacebookDelegate callback)
	{
		FB.Login(scope, callback);
	}
}
