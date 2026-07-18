using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MobageEditor
{
	public class NativeAPI
	{
		private static Action<JsonData> callback;

		private static readonly NativeAPI instance = new NativeAPI();

		public static NativeAPI Instance
		{
			get
			{
				return instance;
			}
		}

		public string UserId { get; private set; }

		public string AuthToken { get; private set; }

		public string OauthToken { get; private set; }

		public string OauthSecret { get; private set; }

		public string Oauth2Token { get; private set; }

		public string Cookie { get; private set; }

		public string FacebookId { get; private set; }

		public string UserNickname { get; private set; }

		public string GuestNickname { get; private set; }

		public string GuestPassword { get; private set; }

		public string EnvironmentString
		{
			get
			{
				return "SANDBOX";
			}
		}

		private NativeAPI()
		{
			LoadAuthCredential();
		}

		public void GetCapabilities(Action<JsonData> callback)
		{
			Characteristics defaultCharacteristics = Characteristics.DefaultCharacteristics;
			JsonData iDsForAnalytics = defaultCharacteristics.IDsForAnalytics;
			iDsForAnalytics["AID"] = string.Empty;
			iDsForAnalytics["TID"] = string.Empty;
			iDsForAnalytics["MAC"] = string.Empty;
			string empty = string.Empty;
			string text = "wifi";
			string empty2 = string.Empty;
			string text2 = Mobage.sharedInstance.AppVersion ?? string.Empty;
			string text3 = Characteristics.DefaultCharacteristics.Carrier ?? string.Empty;
			JsonData jsonData = new JsonData();
			jsonData["deviceName"] = defaultCharacteristics.DeviceModel;
			string appId = Mobage.sharedInstance.AppId;
			if (appId != null)
			{
				appId = appId.ToLower();
				if (appId.Contains("-android"))
				{
					jsonData["osType"] = "android";
				}
				else
				{
					jsonData["osType"] = "iOS";
				}
			}
			jsonData["osVersion"] = defaultCharacteristics.PlatformOSVersion;
			jsonData["locale"] = defaultCharacteristics.Locale;
			jsonData["timezone"] = defaultCharacteristics.Timezone;
			jsonData["deviceIds"] = iDsForAnalytics;
			jsonData["sdkVersion"] = Mobage.sharedInstance.SDKVersion;
			jsonData["displayWidth"] = Screen.width;
			jsonData["displayHeight"] = Screen.height;
			jsonData["deviceToken"] = empty;
			jsonData["environment"] = EnvironmentString;
			jsonData["networkType"] = text;
			jsonData["carrier"] = text3;
			jsonData["appKey"] = Mobage.sharedInstance.AppId;
			jsonData["appVersion"] = text2;
			jsonData["appIcon"] = empty2;
			jsonData["consumerKey"] = Mobage.sharedInstance.ConsumerKey;
			jsonData["consumerSecret"] = Mobage.sharedInstance.ConsumerSecret;
			jsonData["distributionName"] = string.Empty;
			jsonData["hasSamsungApps"] = false;
			jsonData["deviceEmail"] = string.Empty;
			NativeAPI nativeAPI = Instance;
			jsonData["userId"] = nativeAPI.UserId ?? string.Empty;
			jsonData["authToken"] = nativeAPI.AuthToken ?? string.Empty;
			jsonData["oauthToken"] = nativeAPI.OauthToken ?? string.Empty;
			jsonData["oauthSecret"] = nativeAPI.OauthSecret ?? string.Empty;
			jsonData["oauth2Token"] = nativeAPI.Oauth2Token ?? string.Empty;
			jsonData["cookie"] = nativeAPI.Cookie ?? string.Empty;
			jsonData["facebookId"] = nativeAPI.FacebookId ?? string.Empty;
			jsonData["userNickname"] = nativeAPI.UserNickname ?? string.Empty;
			jsonData["analyticsSessionId"] = MobageSession.CurrentSession.AnalyticsSessionId ?? string.Empty;
			jsonData["guestNickname"] = nativeAPI.GuestNickname ?? string.Empty;
			jsonData["guestPassword"] = nativeAPI.GuestPassword ?? string.Empty;
			callback(JsonMapper.ToObject("[\"200\"," + jsonData.ToJson() + "]"));
		}

		public static void LaunchApp(string url, Action<JsonData> callback)
		{
			callback(JsonMapper.ToObject("[\"404\",{}]"));
		}

		public static void CanLaunchApp(string url, Action<JsonData> callback)
		{
			callback(JsonMapper.ToObject("[\"200\",{\"installed\":false}]"));
		}

		public void SaveAuthCredential(string userId, string authToken, string oauthToken, string oauthSecret, string oauth2Token, string cookie, string facebookId, string userNickname, string guestNickname, string guestPassword, Action<JsonData> callback)
		{
			UserId = userId;
			AuthToken = authToken;
			OauthToken = oauthToken;
			OauthSecret = oauthSecret;
			Oauth2Token = oauth2Token;
			Cookie = cookie;
			FacebookId = facebookId;
			UserNickname = userNickname;
			GuestNickname = guestNickname;
			GuestPassword = guestPassword;
			StoreAuthCredential();
			callback(JsonMapper.ToObject("[\"200\",{}]"));
		}

		public void SendNotification(string type, JsonData data, Action<JsonData> callback)
		{
			if ("BalanceUpdate".Equals(type))
			{
				SocialService.BalanceUpdatePost();
				callback(JsonMapper.ToObject("[\"200\",{}]"));
			}
			else if ("UserSessionReestablished".Equals(type))
			{
				Auth.NotifySessionReestablished();
				callback(JsonMapper.ToObject("[\"200\",{}]"));
			}
			else if ("UserLogin".Equals(type))
			{
				Auth.NotifyLogin();
				callback(JsonMapper.ToObject("[\"200\",{}]"));
			}
			else if ("UserLogout".Equals(type))
			{
				Auth.NotifyLogout();
				callback(JsonMapper.ToObject("[\"200\",{}]"));
			}
			else
			{
				if (!"UserGradeUpgrade".Equals(type))
				{
					return;
				}
				try
				{
					string text = WWW.UnEscapeURL(data.ToString());
					if (text != null)
					{
						text = text.Replace("{", string.Empty);
						text = text.Replace("}", string.Empty);
						text = text.Replace("\"", string.Empty);
						text = text.Replace(" ", string.Empty);
						string[] array = text.Split(',');
						object obj = null;
						object obj2 = null;
						object obj3 = null;
						object obj4 = null;
						for (int i = 0; i < array.Length; i++)
						{
							string[] array2 = array[i].Split(':');
							if (array2 != null && array2.Length > 1)
							{
								string text2 = array2[0];
								string text3 = array2[1];
								Debug.LogError("Key" + text2);
								if ("previousNickname".Equals(text2))
								{
									obj = text3;
								}
								else if ("previousGrade".Equals(text2))
								{
									obj2 = text3;
								}
								else if ("currentNickname".Equals(text2))
								{
									obj3 = text3;
								}
								else if ("currentGrade".Equals(text2))
								{
									obj4 = text3;
								}
							}
						}
						if (obj != null && obj.GetType() == typeof(string) && obj3 != null && obj3.GetType() == typeof(string) && obj2 != null && obj2.GetType() == typeof(string) && obj4 != null && obj4.GetType() == typeof(string))
						{
							Auth.UserGradeUpgradeNotification userGradeUpgradeNotification = new Auth.UserGradeUpgradeNotification();
							userGradeUpgradeNotification.currentGrade = int.Parse(obj4.ToString());
							userGradeUpgradeNotification.previousGrade = int.Parse(obj2.ToString());
							userGradeUpgradeNotification.currentNickname = obj3.ToString();
							userGradeUpgradeNotification.previousNickname = obj.ToString();
							Auth.NotifyUserGradeUpgrade(userGradeUpgradeNotification);
						}
						else
						{
							callback(JsonMapper.ToObject("[\"500\",{}]"));
						}
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				callback(JsonMapper.ToObject("[\"200\",{}]"));
			}
		}

		public static void PurchaseCoin(string sku, string price, string value, string currency, Action<JsonData> callback)
		{
			IAPManager.PurchaseCoin(sku, price, value, currency, delegate(JsonData resp)
			{
				callback(resp);
			});
		}

		public static void TakePicture(Action<JsonData> callback)
		{
			Debug.Log("Not supported in Editor");
			callback(JsonMapper.ToObject("[\"500\",{}]"));
		}

		private void LoadAuthCredential()
		{
			UserId = PlayerPrefs.GetString("userId");
			AuthToken = PlayerPrefs.GetString("authToken");
			OauthToken = PlayerPrefs.GetString("oauthToken");
			OauthSecret = PlayerPrefs.GetString("oauthSecret");
			Oauth2Token = PlayerPrefs.GetString("oauth2Token");
			Cookie = PlayerPrefs.GetString("cookie");
			FacebookId = PlayerPrefs.GetString("facebookId");
			UserNickname = PlayerPrefs.GetString("userNickname");
			GuestNickname = PlayerPrefs.GetString("guestNickname");
			GuestPassword = PlayerPrefs.GetString("guestPassword");
		}

		private void StoreAuthCredential()
		{
			PlayerPrefs.SetString("userId", UserId);
			PlayerPrefs.SetString("authToken", AuthToken);
			PlayerPrefs.SetString("oauthToken", OauthToken);
			PlayerPrefs.SetString("oauthSecret", OauthSecret);
			PlayerPrefs.SetString("oauth2Token", Oauth2Token);
			PlayerPrefs.SetString("cookie", Cookie);
			PlayerPrefs.SetString("facebookId", FacebookId);
			PlayerPrefs.SetString("userNickname", UserNickname);
			PlayerPrefs.SetString("guestNickname", GuestNickname);
			PlayerPrefs.SetString("guestPassword", GuestPassword);
		}

		public static void FBResultMethod(object result)
		{
			Type type = Type.GetType("FB, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
			if (type == null)
			{
				return;
			}
			MethodInfo method = type.GetMethod("AccessToken");
			object obj = method.Invoke(null, null);
			if (obj != null)
			{
				Type type2 = Type.GetType("HttpMethod, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
				Type type3 = Type.GetType("FacebookDelegate, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
				Type[] types = new Type[4]
				{
					typeof(string),
					type2,
					type3,
					typeof(Dictionary<string, string>)
				};
				MethodInfo method2 = type.GetMethod("API", types);
				if (method2 != null)
				{
					object obj2 = null;
					Type type4 = Type.GetType("Facebook.HttpMethod.GET, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
					object[] parameters = new object[4] { "me", type4, obj2, null };
					result = method2.Invoke(null, parameters);
					callback(JsonMapper.ToObject(string.Concat("[\"200\",{", result, "}]")));
				}
				else
				{
					callback(JsonMapper.ToObject("[\"404\",{}]"));
				}
			}
		}

		public static void GetFacebookUser(Dictionary<string, string> parameters, Action<JsonData> callback)
		{
			try
			{
				Type type = Type.GetType("FB, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
				if (type == null)
				{
					return;
				}
				MethodInfo method = type.GetMethod("AccessToken");
				if (method == null)
				{
					return;
				}
				object obj = method.Invoke(null, null);
				if (obj != null)
				{
					string text = (string)obj;
					return;
				}
				MethodInfo method2 = type.GetMethod("Login");
				if (method2 != null)
				{
					object[] array = new object[1];
					NativeAPI.callback = callback;
					array[0] = null;
					obj = method2.Invoke(null, array);
				}
			}
			catch
			{
				callback(JsonMapper.ToObject("[\"404\",{}]"));
			}
		}

		public static void CloseFacebookSession(Action<JsonData> callback)
		{
			Type type = Type.GetType("FB, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("Logout");
				if (method != null)
				{
					object obj = method.Invoke(null, null);
					if (obj != null)
					{
						callback(JsonMapper.ToObject("[\"200\",{}]"));
					}
					else
					{
						callback(JsonMapper.ToObject("[\"401\",{}]"));
					}
				}
				else
				{
					callback(JsonMapper.ToObject("[\"404\",{}]"));
				}
			}
			else
			{
				callback(JsonMapper.ToObject("[\"404\",{}]"));
			}
		}
	}
}
