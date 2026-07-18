using System;
using System.Collections.Generic;
using System.Text;

public static class TimeFormats
{
	private static Dictionary<TimeFormat, Func<long, string>> timeFormatters = new Dictionary<TimeFormat, Func<long, string>>
	{
		{
			TimeFormat.LONG,
			GetTimeLongString
		},
		{
			TimeFormat.SHORT,
			GetTimeShortString
		},
		{
			TimeFormat.NUMBER,
			GetTimeNumbersString
		},
		{
			TimeFormat.LEADERBOARD_COUNTDOWN,
			GetTimeLeaderboardCountdownString
		}
	};

	public static string GetTimeLongString(long delta)
	{
		long num = delta / 1000 % 60;
		long num2 = delta / 60000 % 60;
		long num3 = delta / 3600000 % 24;
		long num4 = delta / 86400000;
		string text = string.Format("ui_time_days".Localize("{0:00} Day{1}"), num4, (num4 != 1) ? "s" : string.Empty);
		string text2 = string.Format("ui_time_hours".Localize("{0:00} Hour{1}"), num3, (num3 != 1) ? "s" : string.Empty);
		string text3 = string.Format("ui_time_minutes".Localize("{0:00} Minute{1}"), num2, (num2 != 1) ? "s" : string.Empty);
		string text4 = string.Format("ui_time_seconds".Localize("{0:00} Second{1}"), num, (num != 1) ? "s" : string.Empty);
		if (num4 > 0)
		{
			return text + " " + ((num3 <= 0) ? string.Empty : text2);
		}
		if (num3 > 0)
		{
			return text2 + " " + ((num2 <= 0) ? string.Empty : text3);
		}
		if (num2 > 0)
		{
			return text3 + " " + text4;
		}
		return text4;
	}

	public static string GetTimeShortString(long delta)
	{
		long num = delta / 1000 % 60;
		long num2 = delta / 60000 % 60;
		long num3 = delta / 3600000 % 24;
		long num4 = delta / 86400000;
		string text = string.Format("ui_time_days_short".Localize("{0:00}D"), num4);
		string text2 = string.Format("ui_time_hours_short".Localize("{0:00}h"), num3);
		string text3 = string.Format("ui_time_minutes_short".Localize("{0:00}m"), num2);
		string text4 = string.Format("ui_time_seconds_short".Localize("{0:00}s"), num);
		if (num4 > 0)
		{
			return text + " " + ((num3 <= 0) ? string.Empty : text2);
		}
		if (num3 > 0)
		{
			return text2 + " " + ((num2 <= 0) ? string.Empty : text3);
		}
		if (num2 > 0)
		{
			return text3 + " " + text4;
		}
		return text4;
	}

	public static string GetTimeShortStringComplete(long delta)
	{
		long num = delta / 1000 % 60;
		long num2 = delta / 60000 % 60;
		long num3 = delta / 3600000 % 24;
		long num4 = delta / 86400000;
		string text = string.Format("ui_time_days_short".Localize("{0:00}D"), num4);
		string text2 = string.Format("ui_time_hours_short".Localize("{0:00}h"), num3);
		string text3 = string.Format("ui_time_minutes_short".Localize("{0:00}m"), num2);
		string text4 = string.Format("ui_time_seconds_short".Localize("{0:00}s"), num);
		return ((num4 <= 0) ? string.Empty : text) + " " + ((num3 <= 0) ? string.Empty : text2) + " " + ((num2 <= 0) ? string.Empty : text3) + " " + ((num <= 0 && (num4 != 0L || num3 != 0L || num2 != 0L)) ? string.Empty : text4);
	}

	public static string GetTimeNumbersString(long delta)
	{
		long num = delta / 1000 % 60;
		long num2 = delta / 60000 % 60;
		long num3 = delta / 3600000 % 24;
		long num4 = delta / 86400000;
		string text = string.Format("{0:00}", num4);
		string text2 = string.Format("{0:00}", num3);
		string text3 = string.Format("{0:00}", num2);
		string value = string.Format("{0:00}", num);
		StringBuilder stringBuilder = new StringBuilder();
		if (num4 > 0)
		{
			stringBuilder.Append(text + " ");
		}
		if (num3 > 0)
		{
			stringBuilder.Append(text2 + ":");
		}
		if (num2 > 0)
		{
			stringBuilder.Append(text3 + ":");
		}
		stringBuilder.Append(value);
		return stringBuilder.ToString();
	}

	public static string GetTimeLeaderboardCountdownString(long delta)
	{
		long num = delta / 1000 % 60;
		long num2 = delta / 60000 % 60;
		long num3 = delta / 3600000 % 24;
		long num4 = delta / 86400000;
		string text = string.Format("{0:00}", num4);
		string text2 = string.Format("{0:00}", num3);
		string text3 = string.Format("{0:00}", num2);
		string value = string.Format("{0:00}", num);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(text + ":");
		stringBuilder.Append(text2 + ":");
		stringBuilder.Append(text3 + ":");
		stringBuilder.Append(value);
		return stringBuilder.ToString();
	}

	public static string GetTimeString(long delta, TimeFormat format)
	{
		return timeFormatters[format](delta);
	}
}
