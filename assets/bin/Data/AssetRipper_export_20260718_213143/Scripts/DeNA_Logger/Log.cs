using System;
using System.Threading;
using UnityEngine;

public static class Log
{
	public enum Level
	{
		Debug = 1,
		Info = 2,
		Warning = 4,
		Error = 8
	}

	public static Level LevelsEnabled = (Level)12;

	private static IMessageFormatter Formatter;

	private static IMessageFilter Filter;

	private static BaseLog _currentLog;

	private static BaseLog CurrentLogger
	{
		get
		{
			if (_currentLog == null)
			{
				_currentLog = new EditorLog();
			}
			return _currentLog;
		}
	}

	public static void InitializeLogger(string platform, bool isDevelopment, IMessageFormatter formatter = null, IMessageFilter filter = null)
	{
		switch (platform)
		{
		case "ANDROID":
			InitializeLogger(new AndroidLog(), isDevelopment, formatter, filter);
			break;
		case "IOS":
			InitializeLogger(new IOSLog(), isDevelopment, formatter, filter);
			break;
		default:
			InitializeLogger(new EditorLog(), isDevelopment, formatter, filter);
			break;
		}
	}

	public static void InitializeLogger(BaseLog platform, bool isDevelopment, IMessageFormatter formatter = null, IMessageFilter filter = null)
	{
		_currentLog = platform;
		Formatter = formatter;
		Filter = filter;
		if (isDevelopment)
		{
			LevelsEnabled = (Level)15;
		}
		else
		{
			LevelsEnabled = (Level)12;
		}
	}

	public static void DebugNoTrace(string message, string tag = "Default", params object[] args)
	{
		if (IsMessageAllowed(Level.Debug, tag))
		{
			CurrentLogger.DebugNoTrace(GetFormattedString(Level.Debug, tag, message, args));
		}
	}

	public static void InfoNoTrace(string message, string tag = "Default", params object[] args)
	{
		if (IsMessageAllowed(Level.Info, tag))
		{
			CurrentLogger.InfoNoTrace(GetFormattedString(Level.Info, tag, message, args));
		}
	}

	public static void WarningNoTrace(string message, string tag = "Default", params object[] args)
	{
		if (IsMessageAllowed(Level.Warning, tag))
		{
			CurrentLogger.WarningNoTrace(GetFormattedString(Level.Warning, tag, message, args));
		}
	}

	public static void ErrorNoTrace(string message, string tag = "Default", params object[] args)
	{
		if (IsMessageAllowed(Level.Error, tag))
		{
			CurrentLogger.ErrorNoTrace(GetFormattedString(Level.Error, tag, message, args));
		}
	}

	public static void Debug(string message, params object[] args)
	{
		DebugTag(message, null, "Default", args);
	}

	public static void Info(string message, params object[] args)
	{
		InfoTag(message, null, "Default", args);
	}

	public static void Warning(string message, params object[] args)
	{
		WarningTag(message, null, "Default", args);
	}

	public static void Error(string message, params object[] args)
	{
		ErrorTag(message, null, "Default", args);
	}

	public static void DebugTag(string message, UnityEngine.Object context = null, string tag = "Default", params object[] args)
	{
		if (IsMessageAllowed(Level.Debug, tag))
		{
			CurrentLogger.Debug(GetFormattedString(Level.Debug, tag, message, args), context);
		}
	}

	public static void InfoTag(string message, UnityEngine.Object context = null, string tag = "Default", params object[] args)
	{
		if (IsMessageAllowed(Level.Info, tag))
		{
			CurrentLogger.Info(GetFormattedString(Level.Info, tag, message, args), context);
		}
	}

	public static void WarningTag(string message, UnityEngine.Object context = null, string tag = "Default", params object[] args)
	{
		if (IsMessageAllowed(Level.Warning, tag))
		{
			CurrentLogger.Warning(GetFormattedString(Level.Warning, tag, message, args), context);
		}
	}

	public static void ErrorTag(string message, UnityEngine.Object context = null, string tag = "Default", params object[] args)
	{
		if (IsMessageAllowed(Level.Error, tag))
		{
			CurrentLogger.Error(GetFormattedString(Level.Error, tag, message, args), context);
		}
	}

	private static bool IsMessageAllowed(Level level, string tag)
	{
		if ((level & LevelsEnabled) != level)
		{
			return false;
		}
		if (Filter != null && !Filter.IsTagAllowed(level, tag))
		{
			return false;
		}
		return true;
	}

	private static string GetFormattedString(Level level, string tag, string message, params object[] args)
	{
		if (Formatter != null)
		{
			return Formatter.GetFormattedString(level, tag, message, args);
		}
		string text = "";
		string text2 = (string.IsNullOrEmpty(Thread.CurrentThread.Name) ? Thread.CurrentThread.ManagedThreadId.ToString() : Thread.CurrentThread.Name);
		text += DateTime.UtcNow.ToString("ss,fff");
		string text3 = text;
		text = text3 + "  [" + level.ToString() + "][" + text2 + "]  ";
		if (args != null && args.Length > 0)
		{
			return text + string.Format(message, args);
		}
		return text + message;
	}
}
