using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class CrittercismIOS
{
	private static readonly int crittercismUnityPlatformId;

	[DllImport("__Internal")]
	private static extern void Crittercism_EnableWithAppID(string appID);

	[DllImport("__Internal")]
	private static extern bool Crittercism_LogHandledException(string name, string reason, string stack, int platformId);

	[DllImport("__Internal")]
	private static extern void Crittercism_LogUnhandledException(string name, string reason, string stack, int platformId);

	[DllImport("__Internal")]
	private static extern void Crittercism_SetAsyncBreadcrumbMode(bool writeAsync);

	[DllImport("__Internal")]
	private static extern void Crittercism_LeaveBreadcrumb(string breadcrumb);

	[DllImport("__Internal")]
	private static extern void Crittercism_SetUsername(string key);

	[DllImport("__Internal")]
	private static extern void Crittercism_SetValue(string value, string key);

	[DllImport("__Internal")]
	private static extern void Crittercism_SetOptOutStatus(bool status);

	[DllImport("__Internal")]
	private static extern bool Crittercism_GetOptOutStatus();

	private static void CLog(string log)
	{
	}

	public static void Init(string appID)
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
			Debug.Log("CrittercismIOS only supports the iOS platform. Crittercism will not be enabled");
			return;
		}
		if (appID == null)
		{
			Debug.Log("Crittercism given a null app ID");
			return;
		}
		try
		{
			Crittercism_EnableWithAppID(appID);
			AppDomain.CurrentDomain.UnhandledException += _OnUnresolvedExceptionHandler;
			RegisterLogTool.Instance.AddLogListener(_OnDebugLogCallbackHandler);
			Debug.Log("CrittercismIOS: Sucessfully Initialized");
		}
		catch
		{
			Debug.Log("Crittercism Unity plugin failed to initialize.");
		}
	}

	public static void Update()
	{
	}

	public static void LogHandledException(Exception e)
	{
		if (e != null)
		{
			string message = e.Message;
			string message2 = e.Message;
			string stackTrace = e.StackTrace;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				Crittercism_LogHandledException(message, message2, stackTrace, crittercismUnityPlatformId);
			}
		}
	}

	public static bool GetOptOut()
	{
		bool result = true;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			result = Crittercism_GetOptOutStatus();
		}
		return result;
	}

	public static void SetOptOut(bool isOptedOut)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Crittercism_SetOptOutStatus(isOptedOut);
		}
	}

	public static void SetUsername(string username)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Crittercism_SetUsername(username);
		}
	}

	public static void SetValue(string val, string key)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Crittercism_SetValue(val, key);
		}
	}

	public static void LeaveBreadcrumb(string breadcrumb)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Crittercism_LeaveBreadcrumb(breadcrumb);
		}
	}

	private static void _OnUnresolvedExceptionHandler(object sender, UnhandledExceptionEventArgs args)
	{
		if (args == null || args.ExceptionObject == null)
		{
			return;
		}
		try
		{
			Type type = args.ExceptionObject.GetType();
			if (type != typeof(Exception))
			{
				return;
			}
			Exception ex = (Exception)args.ExceptionObject;
			string message = ex.Message;
			string message2 = ex.Message;
			string stackTrace = ex.StackTrace;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				if (args.IsTerminating)
				{
					Crittercism_LogUnhandledException(message, message2, stackTrace, crittercismUnityPlatformId);
				}
				else
				{
					LogHandledException(ex);
				}
			}
		}
		catch
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("CrittercismIOS: Failed to log exception");
			}
		}
	}

	private static void _OnDebugLogCallbackHandler(string name, string stack, LogType type)
	{
		if ((type == LogType.Exception || type == LogType.Assert) && Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Crittercism_LogUnhandledException(name, name, stack, crittercismUnityPlatformId);
		}
	}
}
