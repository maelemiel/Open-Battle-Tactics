using System;

public class UserFacebookData
{
	private const string KEY_LAST_LIKE_SHOW = "lastLikeShow";

	private string _token;

	private string _name;

	private KeyValueStorage _facebookKVS;

	private bool _likeFanPage;

	private long _lastTimeLikeShow = 1L;

	public string Token
	{
		get
		{
			return _token;
		}
		set
		{
			_token = value;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public bool LikeFanPage
	{
		get
		{
			return _likeFanPage;
		}
		set
		{
			_likeFanPage = value;
		}
	}

	public long LastTimeLikeShow
	{
		get
		{
			return _lastTimeLikeShow;
		}
		set
		{
			_facebookKVS.SetValue("lastLikeShow", value.ToString());
			_lastTimeLikeShow = value;
		}
	}

	public UserFacebookData()
	{
		_facebookKVS = KeyValueStorage.Instance(KeyValueStorage.Storage.FACEBOOK_DATA);
		if (_facebookKVS.ContainsKey("lastLikeShow"))
		{
			_lastTimeLikeShow = Convert.ToInt64(_facebookKVS.GetValue<string>("lastLikeShow"));
		}
		if (!Singleton<SocialManager>.instance.IsFacebookConnected())
		{
		}
	}
}
