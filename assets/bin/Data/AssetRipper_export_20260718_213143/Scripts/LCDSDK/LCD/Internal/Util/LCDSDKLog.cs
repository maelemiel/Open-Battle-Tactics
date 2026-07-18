using System;
using LCD.Internal.Model;
using UnityEngine;

namespace LCD.Internal.Util
{
	public class LCDSDKLog
	{
		public static void Debug(string TAG, string message)
		{
			if (Capabilities.sharedManager.debugLog)
			{
				UnityEngine.Debug.Log(LogPreamble("DEBUG", TAG) + message);
			}
		}

		public static void Info(string TAG, string message)
		{
			UnityEngine.Debug.Log(LogPreamble("INFO", TAG) + message);
		}

		public static void Warn(string TAG, string message, LCDError error)
		{
			UnityEngine.Debug.LogWarning(LogPreamble("WARN", TAG) + message + " | " + error.errorCode + " | " + error.errorMessage);
		}

		public static void Error(string TAG, string message, LCDError error)
		{
			UnityEngine.Debug.LogError(LogPreamble("ERROR", TAG) + message + " | " + error.errorCode + " | " + error.errorMessage);
		}

		private static string LogPreamble(string logType, string TAG)
		{
			return string.Concat("LCDLOG | ", logType, " | ", DateTime.Now, " | ", TAG, " | ");
		}
	}
}
