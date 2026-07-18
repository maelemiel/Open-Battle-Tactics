using System;
using UnityEngine;

namespace MobageTools
{
	public class FacebookHelper
	{
		public static string userId;

		private static FacebookHelper _instance;

		private Action<SocialManager.ResponseServerType> facebookConnectionCallback;

		public static FacebookHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new FacebookHelper();
				}
				return _instance;
			}
		}

		public void EstablishFacebookSession(Action<SocialManager.ResponseServerType> callback)
		{
			if (facebookConnectionCallback != null)
			{
				SocialManager instance = Singleton<SocialManager>.instance;
				instance.loginSuccess = (Action<SocialManager.ResponseServerType>)Delegate.Remove(instance.loginSuccess, facebookConnectionCallback);
				facebookConnectionCallback(SocialManager.ResponseServerType.Fail);
			}
			facebookConnectionCallback = callback;
			SocialManager instance2 = Singleton<SocialManager>.instance;
			instance2.loginSuccess = (Action<SocialManager.ResponseServerType>)Delegate.Combine(instance2.loginSuccess, new Action<SocialManager.ResponseServerType>(LoginCallback));
			Singleton<SocialManager>.instance.FacebookLogin();
		}

		private void LoginCallback(SocialManager.ResponseServerType type)
		{
			if (facebookConnectionCallback != null)
			{
				facebookConnectionCallback(type);
				facebookConnectionCallback = null;
			}
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				Time.timeScale = 0f;
			}
			else
			{
				Time.timeScale = 1f;
			}
		}

		public void Logout()
		{
			if (Singleton<SocialApiAdapter>.instance.IsFacebookConnected())
			{
				Singleton<SocialApiAdapter>.instance.FacebookLogout();
			}
		}
	}
}
