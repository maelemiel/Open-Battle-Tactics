using System;

public class CrittercismUtil
{
	private static bool _inited;

	public static string APP_ID_ANDROID = "5322ddaaa6d3d73f2a000001";

	public static string APP_ID_IOS = "5322dd9a7c376420f3000007";

	public static void Init()
	{
		if (!_inited)
		{
			CrittercismAndroid.Init(APP_ID_ANDROID, false, false, AppConfig.clientVersion);
			_inited = true;
		}
	}

	public static void Update()
	{
		CrittercismAndroid.Update();
	}

	public static void SetOptOut(bool s)
	{
		if (_inited)
		{
			CrittercismAndroid.SetOptOut(s);
		}
	}

	public static void SetUsername(string username)
	{
		if (_inited)
		{
			CrittercismAndroid.SetUsername(username);
		}
	}

	public static void SetMetadata(string[] key, string[] v)
	{
		if (_inited)
		{
			CrittercismAndroid.SetMetadata(key, v);
		}
	}

	public static void LeaveBreadcrumb(string l)
	{
		if (_inited)
		{
			CrittercismAndroid.LeaveBreadcrumb(l);
		}
	}

	public static void LogHandledException(Exception e)
	{
		if (_inited)
		{
			CrittercismAndroid.LogHandledException(e);
		}
	}
}
