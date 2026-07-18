using System.Collections.Generic;
using LCD.Internal.Util;
using UnityEngine;

namespace LCD.Internal.Web
{
	public class SDKHttpClient
	{
		public static void Execute(string method, string path, Dictionary<string, string> param, Dictionary<string, object> data, SDKWebCallbackHandler callback)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.dena.west.lcd.sdk.internal.unity.UnityWrapper");
			string text = ((callback == null) ? string.Empty : callback.GetType().Name);
			androidJavaClass2.CallStatic("executeSDKHttpRequest", androidJavaObject, method, path, Json.Serialize(param), Json.Serialize(data), text);
		}
	}
}
