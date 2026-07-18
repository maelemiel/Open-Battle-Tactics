using System.Collections.Generic;
using UnityEngine;

public static class OtherLevelsSDK
{
	public const string VERSION_NUMBER = "1.3.5";

	public const string Interstitial_App_Open_Placement = "App Open";

	public const string Interstitial_Feedback_Placement = "Feedback";

	public const string Interstitial_Push_Open_Placement = "Push Open";

	public const string Interstitial_Store_Launch_Placement = "Store Launch";

	public const string Interstitial_Placement_1 = "Placement 1";

	public const string Interstitial_Placement_2 = "Placement 2";

	public const string Interstitial_Placement_3 = "Placement 3";

	public const string Interstitial_Placement_4 = "Placement 4";

	public const string Interstitial_Placement_5 = "Placement 5";

	public static string appKey = "3cccf720eb5df607567e6abd7836be1f";

	public static void SetTrackingIdWithPortfolioTrackingId(string trackingId, string portfolioTrackingId)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject2.Call("setTrackingIdWithPortfolioTrackingId", trackingId, portfolioTrackingId, androidJavaObject);
	}

	public static void SetTrackingID(string trackingId)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject2.Call("setTrackingID", trackingId, androidJavaObject);
	}

	public static void PushPhashForTracking(string phash)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject.Call("pushPhashForTracking", phash);
	}

	public static void TrackLastPhashOpen()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject.Call("trackLastPhashOpen");
	}

	public static void RegisterEvent(string eventType, string eventLabel)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject2.Call("registerAppEvent", appKey, eventType, eventLabel, androidJavaObject);
	}

	public static void RegisterEventWithPhash(string eventType, string eventLabel, string phash)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject2.Call("registerAppEvent", appKey, eventType, eventLabel, androidJavaObject, phash);
	}

	public static void ClearLocalNotificationsPending()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject2.Call("clearAllPendingNotification", androidJavaObject);
	}

	public static void ScheduleLocalNotification(string notification, string campaign, double secondsFromNow)
	{
		ScheduleLocalNotificationWithMetadata(notification, campaign, secondsFromNow, string.Empty);
	}

	public static void ScheduleLocalNotificationWithMetadata(string notification, string campaign, double secondsFromNow, string metaData)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject context = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		string phash = string.Empty;
		string messagetext = string.Empty;
		string messagecontent = string.Empty;
		long millis = (int)(secondsFromNow * 1000.0);
		string mData = metaData;
		SplitTestNotification.Start(notification, campaign, delegate(Dictionary<string, string> obj)
		{
			Debug.Log("OK");
			if (obj.ContainsKey("phash"))
			{
				phash = obj["phash"];
			}
			if (obj.ContainsKey("messagetext"))
			{
				messagetext = WWW.UnEscapeURL(obj["messagetext"]);
			}
			if (obj.ContainsKey("messagecontent"))
			{
				messagecontent = obj["messagecontent"];
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
				{
					androidJavaClass2.CallStatic("setupLocalNotification", phash, messagetext, millis, mData, context);
				}
			}
		}, delegate
		{
			Debug.Log("Error");
		});
	}

	public static void RegisterDevice(string trackingId, string deviceToken)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject.Call("registerUserAndDeviceToken", appKey, trackingId, deviceToken);
	}

	public static void UnregisterDevice(string deviceToken)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject.Call("unregisterUser", appKey, deviceToken);
	}

	public static void SetTagValue(string trackingId, string tagName, string tagValue, string tagType)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject.Call("setTagValue", appKey, trackingId, tagName, tagValue, tagType);
	}

	public static string GetTrackingID()
	{
		string result = string.Empty;
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.lang.String");
				result = androidJavaClass2.CallStatic<string>("getTrackingId", new object[1] { androidJavaObject });
			}
		}
		return result;
	}

	public static void UsePlacement(string trackingId, string placement)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(OtherLevelsCommonProxy.BundleId + ".OLInterstitialActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass2.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.content.Intent", androidJavaObject, androidJavaClass);
		AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject3 = androidJavaClass3.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		AndroidJavaObject androidJavaObject4 = androidJavaObject3.Call<AndroidJavaObject>("getInterstitial", new object[0]);
		androidJavaObject4.Call("usePlacement", trackingId, placement, appKey, androidJavaObject2, androidJavaObject);
	}
}
