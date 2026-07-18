using System;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TimeUtility
{
	public enum RoundMethod
	{
		Normal = 0,
		Floor = 1,
		Ceiling = 2
	}

	public const long MILISECONDS_IN_SECOND = 1000L;

	public const long MILISECONDS_IN_MINUTE = 60000L;

	public const long MILISECONDS_IN_HOUR = 3600000L;

	public const long MILISECONDS_IN_DAY = 86400000L;

	public const int SECONDS_IN_MINUTE = 60;

	public const int SECONDS_IN_HOUR = 3600;

	public const int SECONDS_IN_DAY = 86400;

	public const int MINUTES_IN_HOUR = 60;

	public const int MINUTES_IN_DAY = 1440;

	public const int HOURS_IN_DAY = 24;

	private static DateTime epochTime = new DateTime(1970, 1, 1, 0, 0, 0);

	private static long serverDelta;

	public static long DeviceTs
	{
		get
		{
			return (long)(DateTime.UtcNow - epochTime).TotalMilliseconds;
		}
	}

	public static long ServerTs
	{
		get
		{
			return DeviceTs + serverDelta;
		}
	}

	public static void SetServerTime(long serverTs)
	{
		serverDelta = serverTs - DeviceTs;
	}

	public static string GetDateString(long timeStampDate, string format)
	{
		timeStampDate += serverDelta;
		DateTime dateTime = default(DateTime);
		dateTime = epochTime;
		dateTime.AddSeconds(timeStampDate);
		return string.Format(format, dateTime);
	}

	public static long GetTimeAgoString(long timeStampDate)
	{
		return ServerTs - timeStampDate;
	}

	public static long StringDateToTimeStamp(string stringDate)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
		string[] array = Regex.Split(stringDate, "/");
		DateTime dateTime2 = new DateTime(int.Parse(array[2]), int.Parse(array[0]), int.Parse(array[1]));
		return (long)(dateTime2 - dateTime).TotalMilliseconds / 1000;
	}

	public static DateTime SetDateTimeToStringDate(string year, string month, string day)
	{
		DateTime dateTime = default(DateTime);
		dateTime = epochTime;
		dateTime.AddDays(Convert.ToDouble(day));
		dateTime.AddMonths(Convert.ToInt32(month));
		dateTime.AddYears(Convert.ToInt32(year) - epochTime.Year);
		return dateTime;
	}

	public static int GetLeftDays(long timeStampDate, RoundMethod roundMethod)
	{
		long value = timeStampDate - ServerTs;
		double value2 = Convert.ToDouble(value) / Convert.ToDouble(86400000L);
		return Convert.ToInt32(RoundValue(value2, roundMethod));
	}

	public static int GetElapsedDays(long timeStampDate, RoundMethod roundMethod)
	{
		return -GetLeftDays(timeStampDate, roundMethod);
	}

	public static int GetLeftHours(long timeStampDate, RoundMethod roundMethod)
	{
		long value = timeStampDate - ServerTs;
		double value2 = Convert.ToDouble(value) / 3600000.0;
		return Convert.ToInt32(RoundValue(value2, roundMethod));
	}

	public static int GetElapsedHours(long timeStampDate, RoundMethod roundMethod)
	{
		return -GetLeftHours(timeStampDate, roundMethod);
	}

	public static int GetLeftMinutes(long timeStampDate, RoundMethod roundMethod)
	{
		long value = timeStampDate - ServerTs;
		double value2 = Convert.ToDouble(value) / 60000.0;
		return Convert.ToInt32(RoundValue(value2, roundMethod));
	}

	public static int GetElapsedMinutes(long timeStampDate, RoundMethod roundMethod)
	{
		return -GetLeftMinutes(timeStampDate, roundMethod);
	}

	public static int GetLeftSeconds(long timeStampDate, RoundMethod roundMethod)
	{
		long value = timeStampDate - ServerTs;
		double value2 = Convert.ToDouble(value) / 1000.0;
		return Convert.ToInt32(RoundValue(value2, roundMethod));
	}

	public static int GetElapsedSeconds(long timeStampDate, RoundMethod roundMethod)
	{
		return -GetLeftSeconds(timeStampDate, roundMethod);
	}

	private static double RoundValue(double value, RoundMethod roundMethod)
	{
		double result = 0.0;
		switch (roundMethod)
		{
		case RoundMethod.Normal:
			result = Math.Round(value);
			break;
		case RoundMethod.Floor:
			result = Math.Floor(value);
			break;
		case RoundMethod.Ceiling:
			result = Math.Ceiling(value);
			break;
		}
		return result;
	}

	public static string ConvertSecondsToTextTime(int time)
	{
		int num = Mathf.FloorToInt(Mathf.Abs((float)time / 3600f));
		int num2 = Mathf.FloorToInt(Mathf.Abs((float)(time - num * 3600) / 60f));
		int num3 = time - num2 * 60 - num * 3600;
		string text = string.Empty;
		if (num > 0)
		{
			if (num < 10)
			{
				text += "0";
			}
			text = text + num + ":";
		}
		if (num2 < 10)
		{
			text += "0";
		}
		text = text + num2 + ":";
		if (num3 < 10)
		{
			text += "0";
		}
		return text + num3;
	}

	public static string ConvertMilisecondsToTextTime(long miliseconds)
	{
		try
		{
			return ConvertSecondsToTextTime(Convert.ToInt32(ConvertMilisecondsToSeconds(miliseconds)));
		}
		catch (Exception ex)
		{
			Log.Error("Error converting text to time: " + ex.Message);
			return string.Empty;
		}
	}

	public static long ConvertDaysToMiliseconds(float days)
	{
		return (long)(days * 86400000f);
	}

	public static long ConvertHoursToMiliseconds(float hours)
	{
		return (long)(hours * 3600000f);
	}

	public static long ConvertMinutesToMiliseconds(float minutes)
	{
		return (long)(minutes * 60000f);
	}

	public static long ConvertSecondsToMiliseconds(float minutes)
	{
		return (long)(minutes * 1000f);
	}

	public static float ConvertMinutesToSeconds(float minutes)
	{
		return minutes * 60f;
	}

	public static float ConvertHoursToSeconds(float hours)
	{
		return hours * 3600f;
	}

	public static float ConvertDaysToSeconds(float days)
	{
		return days * 86400f;
	}

	public static float ConvertMilisecondsToSeconds(long miliseconds)
	{
		return miliseconds / 1000;
	}

	public static float ConvertHoursToMinutes(float hours)
	{
		return hours * 60f;
	}

	public static float ConvertDaysToMinutes(float days)
	{
		return days * 1440f;
	}

	public static float ConvertMilisecondsToMinutes(long miliseconds)
	{
		return miliseconds / 60000;
	}

	public static float ConvertSecondsToMinutes(float seconds)
	{
		return seconds / 60f;
	}

	public static float ConvertDaysToHours(float days)
	{
		return days * 24f;
	}

	public static float ConvertMilisecondsToHours(long miliseconds)
	{
		return miliseconds / 3600000;
	}

	public static float ConvertSecondsToHours(float seconds)
	{
		return seconds / 3600f;
	}

	public static float ConvertMinutesToHours(float minutes)
	{
		return minutes / 60f;
	}

	public static bool IsToday(long timeStamp)
	{
		DateTime dateTime = new DateTime(timeStamp);
		DateTime dateTime2 = new DateTime(ServerTs);
		if (dateTime2.Year != dateTime.Year)
		{
			return false;
		}
		if (dateTime2.Month != dateTime.Month)
		{
			return false;
		}
		if (dateTime2.Day != dateTime.Day)
		{
			return false;
		}
		return true;
	}
}
