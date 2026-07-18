using System;
using System.Collections.Generic;
using UnityEngine;
using com.adjust.sdk;

public class Adjust : MonoBehaviour
{
	public const string sdkPrefix = "unity3.4.4";

	private static IAdjust instance;

	private static string errorMessage = "adjust: SDK not started. Start it manually using the 'appDidLaunch' method";

	private static Action<ResponseData> responseDelegate;

	public string appToken = "{Your App Token}";

	public AdjustUtil.LogLevel logLevel = AdjustUtil.LogLevel.Info;

	public AdjustUtil.AdjustEnvironment environment;

	public bool eventBuffering;

	public bool startManually;

	private void Awake()
	{
		if (!startManually)
		{
			appDidLaunch(appToken, environment, logLevel, eventBuffering);
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (instance != null)
		{
			if (pauseStatus)
			{
				instance.onPause();
			}
			else
			{
				instance.onResume();
			}
		}
	}

	public static void appDidLaunch(string appToken, AdjustUtil.AdjustEnvironment environment, AdjustUtil.LogLevel logLevel, bool eventBuffering)
	{
		if (instance != null)
		{
			Debug.Log("adjust: error, SDK already started.");
			return;
		}
		instance = new AdjustAndroid();
		if (instance == null)
		{
			Debug.Log("adjust: SDK can only be used in Android, iOS, Windows Phone 8 or Windows Store apps");
		}
		else
		{
			instance.appDidLaunch(appToken, environment, "unity3.4.4", logLevel, eventBuffering);
		}
	}

	public static void trackEvent(string eventToken, Dictionary<string, string> parameters = null)
	{
		if (instance == null)
		{
			Debug.Log(errorMessage);
		}
		else
		{
			instance.trackEvent(eventToken, parameters);
		}
	}

	public static void trackRevenue(double cents, string eventToken = null, Dictionary<string, string> parameters = null)
	{
		if (instance == null)
		{
			Debug.Log(errorMessage);
		}
		else
		{
			instance.trackRevenue(cents, eventToken, parameters);
		}
	}

	public static void setResponseDelegate(Action<ResponseData> responseDelegate, string sceneName = "Adjust")
	{
		if (instance == null)
		{
			Debug.Log(errorMessage);
			return;
		}
		Adjust.responseDelegate = responseDelegate;
		instance.setResponseDelegate(sceneName);
		instance.setResponseDelegateString(runResponseDelegate);
	}

	public static void setEnabled(bool enabled)
	{
		if (instance == null)
		{
			Debug.Log(errorMessage);
		}
		else
		{
			instance.setEnabled(enabled);
		}
	}

	public static bool isEnabled()
	{
		if (instance == null)
		{
			Debug.Log(errorMessage);
			return false;
		}
		return instance.isEnabled();
	}

	public void getNativeMessage(string sResponseData)
	{
		runResponseDelegate(sResponseData);
	}

	public static void runResponseDelegate(string sResponseData)
	{
		if (instance == null)
		{
			Debug.Log(errorMessage);
			return;
		}
		if (responseDelegate == null)
		{
			Debug.Log("adjust: response delegate not set to receive callbacks");
			return;
		}
		ResponseData obj = new ResponseData(sResponseData);
		responseDelegate(obj);
	}
}
