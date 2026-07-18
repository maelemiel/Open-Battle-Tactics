using Facebook;

internal interface ISocialApiAdapter
{
	void InitFacebook(InitDelegate callback);

	void FacebookLogout();

	bool IsFacebookConnected();

	string GetFacebookAccessToken();

	void LoginWithReadPermissions(string scope, FacebookDelegate callback);

	void ReauthorizeWithPublishPermissions(string scope, FacebookDelegate callback);
}
