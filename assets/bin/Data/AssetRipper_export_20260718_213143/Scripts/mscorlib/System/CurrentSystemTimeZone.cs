using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	internal class CurrentSystemTimeZone : TimeZone, IDeserializationCallback
	{
		internal enum TimeZoneData
		{
			DaylightSavingStartIdx = 0,
			DaylightSavingEndIdx = 1,
			UtcOffsetIdx = 2,
			AdditionalDaylightOffsetIdx = 3
		}

		internal enum TimeZoneNames
		{
			StandardNameIdx = 0,
			DaylightNameIdx = 1
		}

		private string m_standardName;

		private string m_daylightName;

		private Hashtable m_CachedDaylightChanges = new Hashtable(1);

		private long m_ticksOffset;

		[NonSerialized]
		private TimeSpan utcOffsetWithOutDLS;

		[NonSerialized]
		private TimeSpan utcOffsetWithDLS;

		private static int this_year;

		private static DaylightTime this_year_dlt;

		public override string DaylightName
		{
			get
			{
				return m_daylightName;
			}
		}

		public override string StandardName
		{
			get
			{
				return m_standardName;
			}
		}

		internal CurrentSystemTimeZone()
		{
		}

		internal CurrentSystemTimeZone(long lnow)
		{
			DateTime dateTime = new DateTime(lnow);
			long[] data;
			string[] names;
			if (!GetTimeZoneData(dateTime.Year, out data, out names))
			{
				throw new NotSupportedException(Locale.GetText("Can't get timezone name."));
			}
			m_standardName = Locale.GetText(names[0]);
			m_daylightName = Locale.GetText(names[1]);
			m_ticksOffset = data[2];
			DaylightTime daylightTimeFromData = GetDaylightTimeFromData(data);
			m_CachedDaylightChanges.Add(dateTime.Year, daylightTimeFromData);
			OnDeserialization(daylightTimeFromData);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			OnDeserialization(null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetTimeZoneData(int year, out long[] data, out string[] names);

		public override DaylightTime GetDaylightChanges(int year)
		{
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException("year", year + Locale.GetText(" is not in a range between 1 and 9999."));
			}
			if (year == this_year)
			{
				return this_year_dlt;
			}
			lock (m_CachedDaylightChanges)
			{
				DaylightTime daylightTime = (DaylightTime)m_CachedDaylightChanges[year];
				if (daylightTime == null)
				{
					long[] data;
					string[] names;
					if (!GetTimeZoneData(year, out data, out names))
					{
						throw new ArgumentException(Locale.GetText("Can't get timezone data for " + year));
					}
					daylightTime = GetDaylightTimeFromData(data);
					m_CachedDaylightChanges.Add(year, daylightTime);
				}
				return daylightTime;
			}
		}

		public override TimeSpan GetUtcOffset(DateTime time)
		{
			if (IsDaylightSavingTime(time))
			{
				return utcOffsetWithDLS;
			}
			return utcOffsetWithOutDLS;
		}

		private void OnDeserialization(DaylightTime dlt)
		{
			if (dlt == null)
			{
				this_year = DateTime.Now.Year;
				long[] data;
				string[] names;
				if (!GetTimeZoneData(this_year, out data, out names))
				{
					throw new ArgumentException(Locale.GetText("Can't get timezone data for " + this_year));
				}
				dlt = GetDaylightTimeFromData(data);
			}
			else
			{
				this_year = dlt.Start.Year;
			}
			utcOffsetWithOutDLS = new TimeSpan(m_ticksOffset);
			utcOffsetWithDLS = new TimeSpan(m_ticksOffset + dlt.Delta.Ticks);
			this_year_dlt = dlt;
		}

		private DaylightTime GetDaylightTimeFromData(long[] data)
		{
			return new DaylightTime(new DateTime(data[0]), new DateTime(data[1]), new TimeSpan(data[3]));
		}
	}
}
