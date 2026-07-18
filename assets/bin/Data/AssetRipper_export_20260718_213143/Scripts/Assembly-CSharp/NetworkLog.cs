using System;
using System.Threading;
using UnityEngine;

public static class NetworkLog
{
	public enum Level
	{
		Debug = 1,
		Success = 2,
		Warning = 4,
		Error = 8
	}

	public static Level ALL = (Level)15;

	public static Level LevelsEnabled = ALL;

	public static void Success(string message, params object[] args)
	{
		Write(Level.Success, message, args);
	}

	public static void Debug(string message, params object[] args)
	{
		Write(Level.Debug, message, args);
	}

	public static void Warning(string message, params object[] args)
	{
		Write(Level.Warning, message, args);
	}

	public static void Error(string message, params object[] args)
	{
		Write(Level.Error, message, args);
	}

	private static string GetMessageWithColor(Level level, string message)
	{
		return message;
	}

	private static string GetMessageWithColor(string colorStr, string message)
	{
		return message;
	}

	private static void Write(Level level, string message, object[] args)
	{
		if ((level & LevelsEnabled) == level)
		{
			if (args != null && args.Length > 0)
			{
				message = string.Format(message, args);
			}
			string text = (string.IsNullOrEmpty(Thread.CurrentThread.Name) ? Thread.CurrentThread.ManagedThreadId.ToString() : Thread.CurrentThread.Name);
			string message2 = "[NW]" + DateTime.Now.ToString("HH:mm:ss,fff") + "  [" + level.ToString() + "][" + text + "]  ";
			message = GetMessageWithColor("#add8ffff", message2) + GetMessageWithColor(level, message);
			switch (level)
			{
			case Level.Debug:
				UnityEngine.Debug.Log(message);
				break;
			case Level.Success:
				UnityEngine.Debug.Log(message);
				break;
			case Level.Warning:
				UnityEngine.Debug.LogWarning(message);
				break;
			case Level.Error:
				UnityEngine.Debug.LogError(message);
				break;
			case (Level)3:
			case (Level)5:
			case (Level)6:
			case (Level)7:
				break;
			}
		}
	}
}
