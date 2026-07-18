using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public abstract class TimeZone
	{
		private static TimeZone currentTimeZone = new CurrentSystemTimeZone(DateTime.GetNow());

		public static TimeZone CurrentTimeZone
		{
			get
			{
				return currentTimeZone;
			}
		}

		public abstract string DaylightName { get; }

		public abstract string StandardName { get; }

		public abstract DaylightTime GetDaylightChanges(int year);

		public abstract TimeSpan GetUtcOffset(DateTime time);

		public virtual bool IsDaylightSavingTime(DateTime time)
		{
			return IsDaylightSavingTime(time, GetDaylightChanges(time.Year));
		}

		public static bool IsDaylightSavingTime(DateTime time, DaylightTime daylightTimes)
		{
			if (daylightTimes == null)
			{
				throw new ArgumentNullException("daylightTimes");
			}
			if (daylightTimes.Start.Ticks == daylightTimes.End.Ticks)
			{
				return false;
			}
			if (daylightTimes.Start.Ticks < daylightTimes.End.Ticks)
			{
				if (daylightTimes.Start.Ticks < time.Ticks && daylightTimes.End.Ticks > time.Ticks)
				{
					return true;
				}
			}
			else if (time.Year == daylightTimes.Start.Year && time.Year == daylightTimes.End.Year && (time.Ticks < daylightTimes.End.Ticks || time.Ticks > daylightTimes.Start.Ticks))
			{
				return true;
			}
			return false;
		}

		public virtual DateTime ToLocalTime(DateTime time)
		{
			if (time.Kind == DateTimeKind.Local)
			{
				return time;
			}
			TimeSpan utcOffset = GetUtcOffset(time);
			if (utcOffset.Ticks > 0)
			{
				if (DateTime.MaxValue - utcOffset < time)
				{
					return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Local);
				}
			}
			else if (utcOffset.Ticks < 0 && time.Ticks + utcOffset.Ticks < DateTime.MinValue.Ticks)
			{
				return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Local);
			}
			DateTime dateTime = time.Add(utcOffset);
			DaylightTime daylightChanges = GetDaylightChanges(time.Year);
			if (daylightChanges.Delta.Ticks == 0L)
			{
				return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
			}
			if (dateTime < daylightChanges.End && daylightChanges.End.Subtract(daylightChanges.Delta) <= dateTime)
			{
				return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
			}
			TimeSpan utcOffset2 = GetUtcOffset(dateTime);
			return DateTime.SpecifyKind(time.Add(utcOffset2), DateTimeKind.Local);
		}

		public virtual DateTime ToUniversalTime(DateTime time)
		{
			if (time.Kind == DateTimeKind.Utc)
			{
				return time;
			}
			TimeSpan utcOffset = GetUtcOffset(time);
			if (utcOffset.Ticks < 0)
			{
				if (DateTime.MaxValue + utcOffset < time)
				{
					return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
				}
			}
			else if (utcOffset.Ticks > 0 && DateTime.MinValue + utcOffset > time)
			{
				return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
			}
			return DateTime.SpecifyKind(new DateTime(time.Ticks - utcOffset.Ticks), DateTimeKind.Utc);
		}

		internal TimeSpan GetLocalTimeDiff(DateTime time)
		{
			return GetLocalTimeDiff(time, GetUtcOffset(time));
		}

		internal TimeSpan GetLocalTimeDiff(DateTime time, TimeSpan utc_offset)
		{
			DaylightTime daylightChanges = GetDaylightChanges(time.Year);
			if (daylightChanges.Delta.Ticks == 0L)
			{
				return utc_offset;
			}
			DateTime dateTime = time.Add(utc_offset);
			if (dateTime < daylightChanges.End && daylightChanges.End.Subtract(daylightChanges.Delta) <= dateTime)
			{
				return utc_offset;
			}
			if (dateTime >= daylightChanges.Start && daylightChanges.Start.Add(daylightChanges.Delta) > dateTime)
			{
				return utc_offset - daylightChanges.Delta;
			}
			return GetUtcOffset(dateTime);
		}
	}
}
