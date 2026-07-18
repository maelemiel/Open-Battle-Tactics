using UnityEngine;

namespace LCD
{
	public class LCDNative
	{
		public static bool Init(LCDSDK.EventHandler eventHandler, bool registerForNotifications)
		{
			bool result = false;
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.dena.west.lcd.sdk.internal.unity.UnityEventHandler");
			AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("com.dena.west.lcd.sdk.LCDSDK");
			androidJavaClass3.CallStatic("init", androidJavaObject, androidJavaObject2);
			return result;
		}

		public static bool Resume()
		{
			bool result = false;
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.dena.west.lcd.sdk.LCDSDK");
			androidJavaClass2.CallStatic("resume", androidJavaObject);
			return result;
		}

		public static bool Pause()
		{
			bool result = false;
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.dena.west.lcd.sdk.LCDSDK");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			androidJavaClass2.CallStatic("pause", androidJavaObject);
			return result;
		}
	}
}
