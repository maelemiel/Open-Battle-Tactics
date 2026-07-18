using System;
using System.Collections.Generic;
using UnityEngine;

public class OtherLevelsHelper
{
	public static string APP_KEY = "3cccf720eb5df607567e6abd7836be1f";

	public static void Instantiate()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.mobage.ww.a1933.Super_Battle_Tactics_Android.OLIDFA"))
		{
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				androidJavaClass.CallStatic("fetchIDFA", androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity"));
			}
		}
	}

	public void OnFetchedIDFA(string trackingID)
	{
		string text = string.Empty;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.mobage.ww.a1933.Super_Battle_Tactics_Android.OLIDFA"))
		{
			text = androidJavaClass.CallStatic<string>("IDFA", new object[0]);
		}
		OtherLevelsSDK.SetTrackingIdWithPortfolioTrackingId(trackingID, text);
		Log.Debug("OL SetTrackingIdWithPortfolioId: " + trackingID + " - " + text);
	}

	public static void UsePlacement(string localtion, string placement, bool resfresh = true)
	{
		string text = UserProfile.player.mobageId.ToString();
		if (!Constants.UseOtherLevelsInterstitials)
		{
			return;
		}
		Log.DebugTag("Calling Interstitial: " + text + ", with placement: " + placement, null, "OtherLevels");
		if (AppConfig.platform != AppConfig.PlatformType.amazon)
		{
			SceneController.resumeCallbackEnable = false;
			OtherLevelsSDK.UsePlacement(text, placement);
			if (resfresh)
			{
				Singleton<OLProfileRefresher>.instance.InterstitialShown();
			}
		}
	}

	public static void SetTrackingID(UserProfile user)
	{
		Log.DebugTag("OL SetTrackingID: " + user.mobageId, null, "OtherLevels");
		OtherLevelsSDK.SetTrackingID(user.mobageId.ToString());
		OtherLevelsHelper otherLevelsHelper = new OtherLevelsHelper();
		otherLevelsHelper.OnFetchedIDFA(user.mobageId.ToString());
		OtherLevelsSDK.SetTagValue(string.Empty, "DisplayName", user.username, "string");
		OtherLevelsSDK.SetTagValue(string.Empty, "NickName", user.nickname, "string");
		OtherLevelsSDK.SetTagValue(string.Empty, "UIDBucket", (user.mobageId % 100).ToString(), "numeric");
		RecordLaunch();
		string deviceTokenString = GetDeviceTokenString();
		OtherLevelsSDK.RegisterDevice(user.mobageId.ToString(), deviceTokenString);
	}

	public static string GetDeviceTokenString()
	{
		string empty = string.Empty;
		return SystemInfo.deviceModel + "|" + SystemInfo.deviceName;
	}

	public static void RecordLaunch()
	{
		string trackingID = OtherLevelsSDK.GetTrackingID();
		LastLoginCheck(trackingID);
		LaunchDaysTotal(trackingID);
	}

	public static void userProfileUpdatedHandler(UserProfile newUserProfile)
	{
		Log.Debug("OL userProfileUpdatedHandler");
	}

	protected static void LastLoginCheck(string trackingID)
	{
		Log.DebugTag("OL LastLoginCheck", null, "OtherLevels");
		long num = 864000000000L;
		long today = DateTime.UtcNow.Ticks / num;
		GetTagValue.Get(APP_KEY, trackingID, "LastLogin", delegate(Dictionary<string, string> obj)
		{
			Log.Debug("OL get LastLogin: " + obj["value"]);
			if (obj["value"] != "null")
			{
				float result;
				if (float.TryParse(obj["value"], out result))
				{
					long num2 = (long)result;
					long num3 = today - num2;
					if (num3 == 1)
					{
						ConsecutiveDay(trackingID);
					}
					else if (num3 > 1)
					{
						NonConsecutiveDay(trackingID);
					}
					else
					{
						Log.Debug("OL LastLogin diff: " + num3);
					}
				}
				else
				{
					Log.Debug("OL LastLogin parse error: " + obj["value"]);
				}
			}
			else
			{
				ConsecutiveDay(trackingID);
			}
			WriteLastLogin(today);
		}, delegate(Dictionary<string, string> obj)
		{
			Log.Debug("OL GetTagValue LastLogin error: " + obj["value"]);
			WriteLastLogin(today);
		});
	}

	protected static void WriteLastLogin(long today)
	{
		Log.DebugTag("write LastLogin: " + today, null, "OtherLevels");
		OtherLevelsSDK.SetTagValue(string.Empty, "LastLogin", today.ToString(), "numeric");
	}

	protected static void ConsecutiveDay(string trackingID)
	{
		Log.DebugTag("ConsecutiveDay", null, "OtherLevels");
		GetTagValue.Get(APP_KEY, trackingID, "LaunchDaysConsecutive", delegate(Dictionary<string, string> obj)
		{
			Log.Debug("OL get LaunchDaysConsecutive: " + obj["value"]);
			if (obj["value"] != "null")
			{
				float result;
				if (float.TryParse(obj["value"], out result))
				{
					WriteConsecutiveDay((long)result + 1);
				}
				else
				{
					Log.Debug("OL LaunchDaysConsecutive parse error");
				}
			}
			else
			{
				WriteConsecutiveDay(1L);
			}
		}, delegate(Dictionary<string, string> obj)
		{
			Log.Debug("GetTagValue LaunchDaysConsecutive error: " + obj["value"]);
		});
	}

	protected static void NonConsecutiveDay(string trackingID)
	{
		Log.DebugTag("OL NonConsecutiveDay", null, "OtherLevels");
		WriteConsecutiveDay(1L);
	}

	protected static void WriteConsecutiveDay(long num)
	{
		Log.DebugTag(" WriteConsecutiveDay: " + num, null, "OtherLevels");
		OtherLevelsSDK.SetTagValue(string.Empty, "LaunchDaysConsecutive", num.ToString(), "numeric");
	}

	protected static void LaunchDaysTotal(string trackingID)
	{
		Log.DebugTag("LaunchDaysTotal", null, "OtherLevels");
		GetTagValue.Get(APP_KEY, trackingID, "LaunchDaysTotal", delegate(Dictionary<string, string> obj)
		{
			Log.DebugTag("OL get LaunchDaysTotal: " + obj["value"], null, "Default");
			if (obj["value"] != "null")
			{
				float result;
				if (float.TryParse(obj["value"], out result))
				{
					WriteLaunchDay((long)result + 1);
				}
				else
				{
					Log.Debug("OL LaunchDaysTotal parse error: " + obj["value"]);
				}
			}
			else
			{
				WriteLaunchDay(1L);
			}
		}, delegate(Dictionary<string, string> obj)
		{
			Log.Debug("OL GetTagValue LaunchDaysTotal error: " + obj["value"]);
		});
	}

	protected static void WriteLaunchDay(long num)
	{
		Log.DebugTag("OL WriteLaunchDay: " + num, null, "Default");
		OtherLevelsSDK.SetTagValue(string.Empty, "LaunchDaysTotal", num.ToString(), "numeric");
	}
}
