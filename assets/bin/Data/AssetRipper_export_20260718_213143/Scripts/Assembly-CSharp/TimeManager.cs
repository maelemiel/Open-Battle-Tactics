using System;
using System.Collections.Generic;

public class TimeManager : NonUnitySingleton<TimeManager>
{
	public class DefaultTimeProvider : ITimeProvider
	{
		private DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public long GetCurrentDevicetime()
		{
			return (long)(DateTime.UtcNow - baseTime).TotalMilliseconds;
		}
	}

	public interface ITimeProvider
	{
		long GetCurrentDevicetime();
	}

	public class TimeEvent
	{
		public long timestamp;

		public event Action OnTime;

		public TimeEvent(long timestamp)
		{
			this.timestamp = timestamp;
		}

		public void Invoke()
		{
			if (this.OnTime != null)
			{
				this.OnTime();
			}
		}
	}

	public static long OUT_OF_SYNC_RANGE = 10000L;

	public static long CLIENT_TIME_LAG = 1000L;

	private Dictionary<long, TimeEvent> timestampEvents = new Dictionary<long, TimeEvent>();

	private long serverTimestamp = -1L;

	private long deviceTimeAtSync;

	private ITimeProvider timeProvider = new DefaultTimeProvider();

	public ITimeProvider TimeProvider
	{
		get
		{
			return timeProvider;
		}
		set
		{
			timeProvider = value;
		}
	}

	private long _ServerTime
	{
		get
		{
			long num = GetCurrentDevicetime() - deviceTimeAtSync;
			return serverTimestamp + num;
		}
		set
		{
			if (serverTimestamp == -1)
			{
				SyncToServer(value);
			}
			else if (!SyncCheck(value))
			{
				Log.Warning("Resyncing to server time. Delta=" + GetTimeDelta(value));
				SyncToServer(value);
			}
		}
	}

	public static long ServerTime
	{
		get
		{
			return NonUnitySingleton<TimeManager>.instance._ServerTime;
		}
		set
		{
			NonUnitySingleton<TimeManager>.instance._ServerTime = value;
		}
	}

	public static DateTime ServerDateTime
	{
		get
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)ServerTime / 1000.0);
		}
	}

	private long GetCurrentDevicetime()
	{
		return timeProvider.GetCurrentDevicetime();
	}

	public static long TimeToUnixTimeStamp(DateTime dateTime)
	{
		DateTime dateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return (long)(dateTime - dateTime2).TotalMilliseconds;
	}

	public string GetCountdownToNextDayString(bool shortForm = false)
	{
		DateTime dateTime = new DateTime(ServerDateTime.Year, ServerDateTime.Month, ServerDateTime.Day, 0, 0, 0, DateTimeKind.Utc);
		dateTime = dateTime.AddDays(1.0);
		long time = TimeToUnixTimeStamp(dateTime);
		return GetCountdownString(time, shortForm);
	}

	public string GetCountdownString(long time, bool shortForm = false, bool timestampInFuture = true)
	{
		long num = Math.Abs(GetTimeDelta(time));
		if (timestampInFuture)
		{
			num = Math.Max(0L, num);
		}
		return GetTimeString(num, shortForm);
	}

	public string GetTimeString(long delta, bool shortForm = false)
	{
		return TimeFormats.GetTimeString(delta, shortForm ? TimeFormat.SHORT : TimeFormat.LONG);
	}

	private void SyncToServer(long serverTimestamp)
	{
		this.serverTimestamp = serverTimestamp - CLIENT_TIME_LAG;
		deviceTimeAtSync = GetCurrentDevicetime();
	}

	public long GetTimeDelta(long timestamp)
	{
		return timestamp - ServerTime;
	}

	public bool IsTimestampInPast(long timestamp)
	{
		return GetTimeDelta(timestamp) < 0;
	}

	public bool IsTimestampInFuture(long timestamp)
	{
		return GetTimeDelta(timestamp) >= 0;
	}

	private TimeEvent GetEventForTimestamp(long timestamp)
	{
		TimeEvent timeEvent;
		if (!timestampEvents.ContainsKey(timestamp))
		{
			timeEvent = new TimeEvent(timestamp);
			timestampEvents.Add(timestamp, timeEvent);
		}
		else
		{
			timeEvent = timestampEvents[timestamp];
		}
		return timeEvent;
	}

	public void RegisterForEvent(long timestamp, Action handler)
	{
		GetEventForTimestamp(timestamp).OnTime += handler;
	}

	public void UnRegisterForEvent(long timestamp, Action handler)
	{
		GetEventForTimestamp(timestamp).OnTime -= handler;
	}

	private bool SyncCheck(long serverTimestamp)
	{
		if (ServerTime >= serverTimestamp)
		{
			return false;
		}
		if (Math.Abs(serverTimestamp - ServerTime) > OUT_OF_SYNC_RANGE)
		{
			return false;
		}
		return true;
	}

	public void CheckEvents()
	{
		foreach (TimeEvent value in timestampEvents.Values)
		{
			if (IsTimestampInPast(value.timestamp))
			{
				value.Invoke();
			}
		}
	}

	public static void Reset()
	{
		NonUnitySingleton<TimeManager>._instance = null;
		NonUnitySingleton<TimeManager>.Instantiate();
	}
}
