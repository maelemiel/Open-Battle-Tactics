using System;
using UnityEngine;

public static class CrittercismAndroid
{
	private static bool _ShowDebugOnOnRelease = true;

	private static bool _IsPluginInited;

	private static bool _IsUnityPluginInited;

	private static AndroidJavaClass mCrittercismsPlugin;

	private static void CLog(string log)
	{
		if (_ShowDebugOnOnRelease || (Debug.isDebugBuild && log != null))
		{
			Debug.Log("CrittercismAndroid: " + log);
		}
	}

	public static void Init()
	{
		Init(string.Empty);
	}

	public static void Init(string appID)
	{
		Init(appID, false, false, null);
	}

	public static void Init(string appID, bool bDelay, bool bSendLogcat, string customVersion)
	{
		try
		{
			_IsPluginInited = IsInited();
			if (_IsPluginInited && _IsUnityPluginInited)
			{
				return;
			}
			if (!_IsUnityPluginInited)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				mCrittercismsPlugin = new AndroidJavaClass("com.crittercism.unity.CrittercismAndroid");
				if (mCrittercismsPlugin == null)
				{
					CLog("To find Crittercism Plugin");
					throw new Exception("ExitError");
				}
				mCrittercismsPlugin.CallStatic("SetConfig", bDelay, bSendLogcat, customVersion);
				mCrittercismsPlugin.CallStatic("Init", androidJavaObject, appID);
			}
			EnableDebugLog(_ShowDebugOnOnRelease);
		}
		catch (Exception ex)
		{
			CLog("Failed to initialize Crittercisms: " + ex.Message);
			_IsUnityPluginInited = false;
		}
	}

	public static void EnableDebugLog(bool b)
	{
		if (mCrittercismsPlugin == null || !_IsPluginInited)
		{
			return;
		}
		try
		{
			mCrittercismsPlugin.CallStatic("EnableDebugLog", b);
			_ShowDebugOnOnRelease = b;
		}
		catch (Exception ex)
		{
			CLog(ex.Message);
		}
	}

	public static void Update()
	{
		if (mCrittercismsPlugin == null || _IsUnityPluginInited)
		{
			return;
		}
		CLog("TryRegister");
		_IsPluginInited = IsInited();
		if (_IsPluginInited)
		{
			Debug.LogWarning("Crittercism Attach Log Callback");
			if (RegisterLogTool.Instance.AddLogListener(_OnDebugLogCallbackHandler))
			{
				AppDomain.CurrentDomain.UnhandledException += _OnUnresolvedExceptionHandler;
			}
			_IsUnityPluginInited = true;
			CLog("Registered");
		}
	}

	public static void LogHandledException(Exception e)
	{
		if (mCrittercismsPlugin == null || !_IsPluginInited)
		{
			return;
		}
		try
		{
			mCrittercismsPlugin.CallStatic("LogHandledException", e.Source, e.Message, e.StackTrace);
		}
		catch (Exception ex)
		{
			CLog(ex.Message);
		}
	}

	public static void SetOptOut(bool s)
	{
		if (mCrittercismsPlugin == null || !_IsPluginInited)
		{
			return;
		}
		try
		{
			mCrittercismsPlugin.CallStatic<bool>("SetOptOutStatus", new object[1] { s });
		}
		catch (Exception ex)
		{
			CLog(ex.Message);
		}
	}

	public static void SetUsername(string username)
	{
		if (mCrittercismsPlugin == null || !_IsPluginInited)
		{
			return;
		}
		try
		{
			mCrittercismsPlugin.CallStatic("SetUsername", username);
		}
		catch (Exception ex)
		{
			CLog(ex.Message);
		}
	}

	public static void SetMetadata(string[] key, string[] v)
	{
		if (mCrittercismsPlugin == null || !_IsPluginInited)
		{
			return;
		}
		try
		{
			mCrittercismsPlugin.CallStatic("SetMetadata", key, v);
		}
		catch (Exception ex)
		{
			CLog(ex.Message);
		}
	}

	public static void LeaveBreadcrumb(string l)
	{
		if (mCrittercismsPlugin == null || !_IsPluginInited)
		{
			return;
		}
		try
		{
			mCrittercismsPlugin.CallStatic("LeaveBreadcrumb", l);
		}
		catch (Exception ex)
		{
			CLog(ex.Message);
		}
	}

	public static bool IsInited()
	{
		bool result = false;
		if (mCrittercismsPlugin == null)
		{
			return false;
		}
		try
		{
			result = mCrittercismsPlugin.CallStatic<bool>("IsInited", new object[0]);
		}
		catch (Exception ex)
		{
			CLog(ex.Message);
		}
		return result;
	}

	private static void _OnUnresolvedExceptionHandler(object sender, UnhandledExceptionEventArgs args)
	{
		if (mCrittercismsPlugin == null || !_IsPluginInited || args == null || args.ExceptionObject == null || args.ExceptionObject.GetType() != typeof(Exception))
		{
			return;
		}
		try
		{
			Exception ex = (Exception)args.ExceptionObject;
			if (args.IsTerminating)
			{
				mCrittercismsPlugin.CallStatic("LogUnhandledException", ex.Source, ex.Message, ex.StackTrace);
			}
			else
			{
				LogHandledException(ex);
			}
		}
		catch (Exception ex2)
		{
			CLog(ex2.Message);
		}
	}

	private static void _OnDebugLogCallbackHandler(string name, string stack, LogType type)
	{
		if ((type != LogType.Assert && type != LogType.Exception) || mCrittercismsPlugin == null || !_IsPluginInited)
		{
			return;
		}
		try
		{
			mCrittercismsPlugin.CallStatic("LogUnhandledException", name, name, stack);
		}
		catch (Exception ex)
		{
			CLog(ex.Message);
		}
	}
}
