using System;
using UnityEngine;

public class OtherLevelsAndroidProxy : MonoBehaviour
{
	private string appKey = "3cccf720eb5df607567e6abd7836be1f";

	private bool push_enabled;

	private string portfolioAppKey = "8d0d22b3ced81a4ab9159ea7eb22c940";

	private bool lostFocus;

	private bool isPaused;

	private bool firstStart = true;

	private static string[] SENDER_IDS = new string[1] { "282528277972" };

	private string mainActivity = "com.unity3d.player.UnityPlayerNativeActivity";

	private void Awake()
	{
		if (Array.Exists(UnityEngine.Object.FindObjectsOfType(typeof(OtherLevelsAndroidProxy)), (UnityEngine.Object obj) => obj != this))
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		OtherLevelsSDK.appKey = appKey;
		Debug.Log("Proxy:Create");
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		androidJavaObject2.Call("registerCreate", OtherLevelsSDK.appKey, androidJavaObject);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnApplicationFocus(bool focus)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		if (!focus)
		{
			Debug.Log("Proxy:Stop");
			androidJavaObject2.Call("registerStop", OtherLevelsSDK.appKey, androidJavaObject);
			lostFocus = true;
			return;
		}
		if (lostFocus)
		{
			Debug.Log("Proxy:Restart");
			androidJavaObject2.Call("registerRestart", OtherLevelsSDK.appKey, androidJavaObject);
			lostFocus = false;
		}
		Debug.Log("Proxy:Start");
		androidJavaObject2.Call("registerStart", OtherLevelsSDK.appKey, androidJavaObject);
		if (firstStart)
		{
			androidJavaObject2.Call("setAppKeyWithPortfolioAppKey", OtherLevelsSDK.appKey, portfolioAppKey, androidJavaObject);
			Debug.Log("Proxy:Resume");
			androidJavaObject2.Call("registerResume", OtherLevelsSDK.appKey, androidJavaObject);
			firstStart = false;
			if (push_enabled)
			{
				GCM.Initialize();
				GCM.Register(SENDER_IDS);
			}
			GCM.SetupLaunchActivity(mainActivity);
		}
	}

	private void OnApplicationPause(bool paused)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		if (paused)
		{
			Debug.Log("Proxy:Pause");
			androidJavaObject2.Call("registerPause", OtherLevelsSDK.appKey, androidJavaObject);
		}
		else
		{
			Debug.Log("Proxy:Resume");
			androidJavaObject2.Call("registerResume", OtherLevelsSDK.appKey, androidJavaObject);
		}
		isPaused = paused;
	}

	private void OnApplicationQuit()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.android.library.OlAndroidLibrary");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
		if (!isPaused)
		{
			Debug.Log("Proxy:Pause");
			androidJavaObject2.Call("registerPause", OtherLevelsSDK.appKey, androidJavaObject);
		}
		Debug.Log("Proxy:Stop");
		androidJavaObject2.Call("registerStop", OtherLevelsSDK.appKey, androidJavaObject);
		Debug.Log("Proxy:Destroy");
		androidJavaObject2.Call("registerDestroy", OtherLevelsSDK.appKey, androidJavaObject);
	}
}
