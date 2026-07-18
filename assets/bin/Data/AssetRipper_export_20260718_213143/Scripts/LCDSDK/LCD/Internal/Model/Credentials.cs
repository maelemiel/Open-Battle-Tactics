using System;
using System.Collections.Generic;
using System.Reflection;
using LCD.Internal.Util;
using LCD.Internal.Web;
using LCD.User;
using UnityEngine;

namespace LCD.Internal.Model
{
	public class Credentials
	{
		public enum SessionAction
		{
			START = 0,
			RESUME = 1,
			UPDATE = 2,
			PAUSE = 3
		}

		private static string TAG = "Credentials";

		public static string accessToken;

		public static long accessTokenIat;

		public static long accessTokenExp;

		private long duration;

		private long startTimestamp;

		private bool paused;

		private string keyChainPlayerId = string.Empty;

		private string keyChainPassword = string.Empty;

		private static Credentials instance;

		internal string mobageId { get; private set; }

		internal string mobageGamerTAG { get; private set; }

		internal string mobagePassword { get; private set; }

		internal string mobageFacebookId { get; private set; }

		internal string KeyChainPlayerId
		{
			get
			{
				keyChainPlayerId = SharedManager.keyChainPlayerId;
				return keyChainPlayerId;
			}
			private set
			{
				bool flag = false;
				if (value != null && !value.Equals(keyChainPlayerId))
				{
					flag = true;
				}
				keyChainPlayerId = value;
				SharedManager.keyChainPlayerId = value;
				if (flag)
				{
					SaveKeyStore();
				}
			}
		}

		internal string KeyChainPassword
		{
			get
			{
				keyChainPassword = SharedManager.keyChainPassword;
				return keyChainPassword;
			}
			private set
			{
				bool flag = false;
				if (value != null && !value.Equals(keyChainPassword))
				{
					flag = true;
				}
				keyChainPassword = value;
				SharedManager.keyChainPassword = value;
				if (flag)
				{
					SaveKeyStore();
				}
			}
		}

		public static Credentials SharedManager
		{
			get
			{
				if (instance == null)
				{
					try
					{
						instance = new Credentials();
						instance.ReadKeyStore();
					}
					catch (Exception)
					{
					}
				}
				return instance;
			}
		}

		internal void ReadKeyStore()
		{
			try
			{
				Type type = Type.GetType("LCDKeyChain,Assembly-CSharp-Editor");
				if (type != null)
				{
					MethodInfo method = type.GetMethod("ReadKeyStore", BindingFlags.Static | BindingFlags.NonPublic);
					method.Invoke(null, null);
				}
			}
			catch (Exception)
			{
			}
		}

		internal void SaveKeyStore()
		{
			try
			{
				Type type = Type.GetType("LCDKeyChain,Assembly-CSharp-Editor");
				if (type != null)
				{
					MethodInfo method = type.GetMethod("SaveKeyStore", BindingFlags.Static | BindingFlags.NonPublic);
					method.Invoke(null, new object[2] { SharedManager.KeyChainPlayerId, SharedManager.KeyChainPassword });
				}
			}
			catch (Exception)
			{
			}
		}

		internal static void GetSession(SessionAction action, SDKWebCallbackHandler callback)
		{
			if (action == SessionAction.START)
			{
				accessToken = null;
			}
			if (accessToken == null)
			{
				SharedManager.CreateSession(callback);
			}
			else
			{
				SharedManager.UpdateSession(action, callback);
			}
		}

		internal void CreateSession(SDKWebCallbackHandler callback)
		{
			if (SDKWebUtil.currentProcess() == LCDSDK.SDKWebViewProcess.FINISHED)
			{
				SDKWebUtil.openSDKWebView("createSession", null, new SessionCallbackHandler(callback), false);
			}
		}

		internal void UpdateSession(SessionAction action, SDKWebCallbackHandler callback)
		{
			long num = NowInMillisecondsSinceEpoch();
			if (!paused)
			{
				duration += num - startTimestamp;
			}
			startTimestamp = num;
			if (action == SessionAction.PAUSE)
			{
				paused = true;
			}
			else
			{
				paused = false;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("duration", duration.ToString());
			dictionary.Add("action", action.ToString());
			SDKWebUtil.Execute("GET", "/session", dictionary, null, new SessionCallbackHandler(callback));
		}

		private void resetDuration()
		{
			SharedManager.duration = 0L;
			SharedManager.startTimestamp = NowInMillisecondsSinceEpoch();
		}

		private static long NowInMillisecondsSinceEpoch()
		{
			return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
		}

		internal void setSessionResponse(Dictionary<string, object> data)
		{
			try
			{
				if (data == null)
				{
					LCDSDKLog.Debug(TAG, "Set SessionResponse Dictionary: " + data);
					return;
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			if (data.ContainsKey("accessToken"))
			{
				accessToken = (string)data["accessToken"];
			}
			if (data.ContainsKey("accessTokenIat"))
			{
				accessTokenIat = (long)data["accessTokenIat"];
			}
			if (data.ContainsKey("accessTokenExp"))
			{
				accessTokenExp = (long)data["accessTokenExp"];
			}
			if (data.ContainsKey("newSession") && (bool)data["newSession"])
			{
				resetDuration();
			}
			if (data.ContainsKey("publishedAppVersion"))
			{
				Capabilities.sharedManager.publishedAppVersion = (string)data["publishedAppVersion"];
			}
			long userId = -1L;
			if (data.ContainsKey("userId"))
			{
				userId = (long)data["userId"];
			}
			string country = string.Empty;
			if (data.ContainsKey("country"))
			{
				country = (string)data["country"];
			}
			string region = string.Empty;
			if (data.ContainsKey("region"))
			{
				region = (string)data["region"];
			}
			string city = string.Empty;
			if (data.ContainsKey("city"))
			{
				city = (string)data["city"];
			}
			bool developer = false;
			if (data.ContainsKey("developer"))
			{
				developer = (bool)data["developer"];
			}
			StoreAccount.StoreType storeType = StoreAccount.StoreType.UNITY_EDITOR;
			if (data.ContainsKey("storeType"))
			{
				storeType = StoreAccount.getStoreType((string)data["storeType"]);
			}
			string storeUserId = null;
			if (data.ContainsKey("storeUserId"))
			{
				storeUserId = (string)data["storeUserId"];
			}
			string advertisingId = string.Empty;
			if (data.ContainsKey("advertisingId"))
			{
				advertisingId = (string)data["advertisingId"];
			}
			string deviceToken = null;
			if (data.ContainsKey("deviceToken"))
			{
				deviceToken = (string)data["deviceToken"];
			}
			StoreAccount storeAccount = new StoreAccount(storeType, storeUserId, advertisingId, deviceToken);
			setKeyChainData(data);
			LCD.User.User.setInstance(userId, country, region, city, developer, storeAccount);
		}

		internal void setKeyChainData(Dictionary<string, object> data)
		{
			try
			{
				if (data == null)
				{
					LCDSDKLog.Debug(TAG, "Set KeyChainData Dictionary: " + data);
					return;
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			if (data.ContainsKey("storeUserId"))
			{
				KeyChainPlayerId = (string)data["storeUserId"];
			}
			if (data.ContainsKey("keyChainPassword"))
			{
				KeyChainPassword = (string)data["keyChainPassword"];
			}
		}

		public static void setKeyChain(string json)
		{
			if (json != null && json.Length > 0)
			{
				Dictionary<string, object> keyChainData = (Dictionary<string, object>)Json.Deserialize(json);
				SharedManager.setKeyChainData(keyChainData);
			}
		}

		public static void setSessionResponse(string json)
		{
			LCDSDKLog.Debug(TAG, "Set SessionResponse: " + json);
			if (json != null && json.Length > 0)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(json);
				if (dictionary != null && dictionary.Count >= 1)
				{
					if (dictionary.Count == 1 && dictionary["requestId"] != null)
					{
						LCDSDKLog.Info(TAG, "setSessionResponse skipping only has requestId: " + dictionary["requestId"]);
					}
					else
					{
						SharedManager.setSessionResponse(dictionary);
					}
				}
			}
			else
			{
				LCDSDKLog.Debug(TAG, "Set SessionResponse has invalid json data");
			}
		}

		internal Dictionary<string, object> ToJson()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (mobageId != null)
			{
				dictionary.Add("mobageId", mobageId);
			}
			if (mobageGamerTAG != null)
			{
				dictionary.Add("mobageGamerTAG", mobageGamerTAG);
			}
			if (mobagePassword != null)
			{
				dictionary.Add("mobagePassword", mobagePassword);
			}
			if (mobageFacebookId != null)
			{
				dictionary.Add("mobageFacebookId", mobageFacebookId);
			}
			if (keyChainPlayerId != null)
			{
				dictionary.Add("keyChainId", keyChainPlayerId);
			}
			if (keyChainPassword != null)
			{
				dictionary.Add("keyChainPassword", keyChainPassword);
			}
			return dictionary;
		}

		internal void resetKeyChain()
		{
			Credentials sharedManager = SharedManager;
			sharedManager.keyChainPlayerId = string.Empty;
			sharedManager.keyChainPassword = string.Empty;
			Type type = Type.GetType("KeyChain,Assembly-CSharp");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("ResetKeyChain", BindingFlags.Static | BindingFlags.NonPublic);
				method.Invoke(null, null);
			}
		}

		public static void Init()
		{
			if (instance == null)
			{
				instance = new Credentials();
				instance.ReadKeyStore();
			}
		}
	}
}
