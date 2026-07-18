using System;
using UnityEngine;

public class GCM
{
	private const string CLASS_NAME = "com.otherlevels.androidportal.UnityGCMRegister";

	private static GameObject _receiver;

	public static void Initialize()
	{
		if (Application.platform == RuntimePlatform.Android && _receiver == null)
		{
			_receiver = new GameObject("GCMReceiver");
			_receiver.AddComponent("GCMReceiver");
		}
	}

	public static void ShowToast(string message)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.androidportal.Util"))
			{
				androidJavaClass.CallStatic("showToast", message);
			}
		}
	}

	public static void Register(params string[] senderIds)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				string text = string.Join(",", senderIds);
				androidJavaClass.CallStatic("register", text);
			}
		}
	}

	public static void SetupLaunchActivity(string activityName)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				androidJavaClass2.CallStatic("setupLaunchActivity", activityName, androidJavaObject);
			}
		}
	}

	public static void Unregister()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				androidJavaClass.CallStatic("unregister");
			}
		}
	}

	public static bool IsRegistered()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				return androidJavaClass.CallStatic<bool>("isRegistered", new object[0]);
			}
		}
		return false;
	}

	public static string GetRegistrationId()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				return androidJavaClass.CallStatic<string>("getRegistrationId", new object[0]);
			}
		}
		return null;
	}

	public static void SetRegisteredOnServer(bool isRegistered)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				androidJavaClass.CallStatic("setRegisteredOnServer", isRegistered);
			}
		}
	}

	public static bool IsRegisteredOnServer()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				return androidJavaClass.CallStatic<bool>("isRegisteredOnServer", new object[0]);
			}
		}
		return false;
	}

	public static void SetRegisterOnServerLifespan(long lifespan)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				androidJavaClass.CallStatic("setRegisterOnServerLifespan", lifespan);
			}
		}
	}

	public static long GetRegisterOnServerLifespan()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.otherlevels.androidportal.UnityGCMRegister"))
			{
				return androidJavaClass.CallStatic<long>("getRegisterOnServerLifespan", new object[0]);
			}
		}
		return 0L;
	}

	public static void SetErrorCallback(Action<string> onError)
	{
		GCMReceiver._onError = onError;
	}

	public static void SetMessageCallback(Action<string> onMessage)
	{
		GCMReceiver._onMessage = onMessage;
	}

	public static void SetRegisteredCallback(Action<string> onRegistered)
	{
		GCMReceiver._onRegistered = onRegistered;
	}

	public static void SetUnregisteredCallback(Action<string> onUnregistered)
	{
		GCMReceiver._onUnregistered = onUnregistered;
	}

	public static void SetDeleteMessagesCallback(Action<int> onDeleteMessages)
	{
		GCMReceiver._onDeleteMessages = onDeleteMessages;
	}
}
