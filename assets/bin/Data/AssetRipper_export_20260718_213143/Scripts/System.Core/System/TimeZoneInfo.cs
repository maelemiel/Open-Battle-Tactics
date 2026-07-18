using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace System
{
	[Serializable]
	public sealed class TimeZoneInfo : ISerializable, IDeserializationCallback, IEquatable<TimeZoneInfo>
	{
		[Serializable]
		public sealed class AdjustmentRule : ISerializable, IDeserializationCallback, IEquatable<AdjustmentRule>
		{
			private DateTime dateEnd;

			private DateTime dateStart;

			private TimeSpan daylightDelta;

			private TransitionTime daylightTransitionEnd;

			private TransitionTime daylightTransitionStart;

			public DateTime DateEnd
			{
				get
				{
					return dateEnd;
				}
			}

			public DateTime DateStart
			{
				get
				{
					return dateStart;
				}
			}

			public TimeSpan DaylightDelta
			{
				get
				{
					return daylightDelta;
				}
			}

			public TransitionTime DaylightTransitionEnd
			{
				get
				{
					return daylightTransitionEnd;
				}
			}

			public TransitionTime DaylightTransitionStart
			{
				get
				{
					return daylightTransitionStart;
				}
			}

			private AdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TransitionTime daylightTransitionStart, TransitionTime daylightTransitionEnd)
			{
				if (dateStart.Kind != DateTimeKind.Unspecified || dateEnd.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException("the Kind property of dateStart or dateEnd parameter does not equal DateTimeKind.Unspecified");
				}
				if (daylightTransitionStart == daylightTransitionEnd)
				{
					throw new ArgumentException("daylightTransitionStart parameter cannot equal daylightTransitionEnd parameter");
				}
				if (dateStart.Ticks % 864000000000L != 0L || dateEnd.Ticks % 864000000000L != 0L)
				{
					throw new ArgumentException("dateStart or dateEnd parameter includes a time of day value");
				}
				if (dateEnd < dateStart)
				{
					throw new ArgumentOutOfRangeException("dateEnd is earlier than dateStart");
				}
				if (daylightDelta > new TimeSpan(14, 0, 0) || daylightDelta < new TimeSpan(-14, 0, 0))
				{
					throw new ArgumentOutOfRangeException("daylightDelta is less than -14 or greater than 14 hours");
				}
				if (daylightDelta.Ticks % 10000000 != 0L)
				{
					throw new ArgumentOutOfRangeException("daylightDelta parameter does not represent a whole number of seconds");
				}
				this.dateStart = dateStart;
				this.dateEnd = dateEnd;
				this.daylightDelta = daylightDelta;
				this.daylightTransitionStart = daylightTransitionStart;
				this.daylightTransitionEnd = daylightTransitionEnd;
			}

			public static AdjustmentRule CreateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TransitionTime daylightTransitionStart, TransitionTime daylightTransitionEnd)
			{
				return new AdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd);
			}

			public bool Equals(AdjustmentRule other)
			{
				return dateStart == other.dateStart && dateEnd == other.dateEnd && daylightDelta == other.daylightDelta && daylightTransitionStart == other.daylightTransitionStart && daylightTransitionEnd == other.daylightTransitionEnd;
			}

			public override int GetHashCode()
			{
				return dateStart.GetHashCode() ^ dateEnd.GetHashCode() ^ daylightDelta.GetHashCode() ^ daylightTransitionStart.GetHashCode() ^ daylightTransitionEnd.GetHashCode();
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new NotImplementedException();
			}

			public void OnDeserialization(object sender)
			{
				throw new NotImplementedException();
			}
		}

		[Serializable]
		public struct TransitionTime : ISerializable, IDeserializationCallback, IEquatable<TransitionTime>
		{
			private DateTime timeOfDay;

			private int month;

			private int day;

			private int week;

			private DayOfWeek dayOfWeek;

			private bool isFixedDateRule;

			public DateTime TimeOfDay
			{
				get
				{
					return timeOfDay;
				}
			}

			public int Month
			{
				get
				{
					return month;
				}
			}

			public int Day
			{
				get
				{
					return day;
				}
			}

			public int Week
			{
				get
				{
					return week;
				}
			}

			public DayOfWeek DayOfWeek
			{
				get
				{
					return dayOfWeek;
				}
			}

			public bool IsFixedDateRule
			{
				get
				{
					return isFixedDateRule;
				}
			}

			private TransitionTime(DateTime timeOfDay, int month, int day)
				: this(timeOfDay, month)
			{
				if (day < 1 || day > 31)
				{
					throw new ArgumentOutOfRangeException("day parameter is less than 1 or greater than 31");
				}
				this.day = day;
				isFixedDateRule = true;
			}

			private TransitionTime(DateTime timeOfDay, int month, int week, DayOfWeek dayOfWeek)
				: this(timeOfDay, month)
			{
				if (week < 1 || week > 5)
				{
					throw new ArgumentOutOfRangeException("week parameter is less than 1 or greater than 5");
				}
				if (dayOfWeek != DayOfWeek.Sunday && dayOfWeek != DayOfWeek.Monday && dayOfWeek != DayOfWeek.Tuesday && dayOfWeek != DayOfWeek.Wednesday && dayOfWeek != DayOfWeek.Thursday && dayOfWeek != DayOfWeek.Friday && dayOfWeek != DayOfWeek.Saturday)
				{
					throw new ArgumentOutOfRangeException("dayOfWeek parameter is not a member od DayOfWeek enumeration");
				}
				this.week = week;
				this.dayOfWeek = dayOfWeek;
				isFixedDateRule = false;
			}

			private TransitionTime(DateTime timeOfDay, int month)
			{
				if (timeOfDay.Year != 1 || timeOfDay.Month != 1 || timeOfDay.Day != 1)
				{
					throw new ArgumentException("timeOfDay parameter has a non-default date component");
				}
				if (timeOfDay.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException("timeOfDay parameter Kind's property is not DateTimeKind.Unspecified");
				}
				if (timeOfDay.Ticks % 10000 != 0L)
				{
					throw new ArgumentException("timeOfDay parameter does not represent a whole number of milliseconds");
				}
				if (month < 1 || month > 12)
				{
					throw new ArgumentOutOfRangeException("month parameter is less than 1 or greater than 12");
				}
				this.timeOfDay = timeOfDay;
				this.month = month;
				week = -1;
				dayOfWeek = (DayOfWeek)(-1);
				day = -1;
				isFixedDateRule = false;
			}

			public static TransitionTime CreateFixedDateRule(DateTime timeOfDay, int month, int day)
			{
				return new TransitionTime(timeOfDay, month, day);
			}

			public static TransitionTime CreateFloatingDateRule(DateTime timeOfDay, int month, int week, DayOfWeek dayOfWeek)
			{
				return new TransitionTime(timeOfDay, month, week, dayOfWeek);
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new NotImplementedException();
			}

			public override bool Equals(object other)
			{
				if (other is TransitionTime)
				{
					return this == (TransitionTime)other;
				}
				return false;
			}

			public bool Equals(TransitionTime other)
			{
				return this == other;
			}

			public override int GetHashCode()
			{
				return (int)((uint)day ^ (uint)dayOfWeek ^ (uint)month ^ (uint)(int)timeOfDay.Ticks) ^ week;
			}

			public void OnDeserialization(object sender)
			{
				throw new NotImplementedException();
			}

			public static bool operator ==(TransitionTime t1, TransitionTime t2)
			{
				return t1.day == t2.day && t1.dayOfWeek == t2.dayOfWeek && t1.isFixedDateRule == t2.isFixedDateRule && t1.month == t2.month && t1.timeOfDay == t2.timeOfDay && t1.week == t2.week;
			}

			public static bool operator !=(TransitionTime t1, TransitionTime t2)
			{
				return !(t1 == t2);
			}
		}

		private struct TimeType
		{
			public readonly int Offset;

			public readonly bool IsDst;

			public string Name;

			public TimeType(int offset, bool is_dst, string abbrev)
			{
				Offset = offset;
				IsDst = is_dst;
				Name = abbrev;
			}

			public override string ToString()
			{
				return "offset: " + Offset + "s, is_dst: " + IsDst + ", zone name: " + Name;
			}
		}

		private const int BUFFER_SIZE = 16384;

		private TimeSpan baseUtcOffset;

		private string daylightDisplayName;

		private string displayName;

		private string id;

		private static TimeZoneInfo local;

		private string standardDisplayName;

		private bool disableDaylightSavingTime;

		private static TimeZoneInfo utc;

		private static string timeZoneDirectory;

		private AdjustmentRule[] adjustmentRules;

		private static List<TimeZoneInfo> systemTimeZones;

		public TimeSpan BaseUtcOffset
		{
			get
			{
				return baseUtcOffset;
			}
		}

		public string DaylightName
		{
			get
			{
				if (disableDaylightSavingTime)
				{
					return string.Empty;
				}
				return daylightDisplayName;
			}
		}

		public string DisplayName
		{
			get
			{
				return displayName;
			}
		}

		public string Id
		{
			get
			{
				return id;
			}
		}

		public static TimeZoneInfo Local
		{
			get
			{
				if (local == null)
				{
					try
					{
						local = FindSystemTimeZoneByFileName("Local", "/etc/localtime");
					}
					catch
					{
						try
						{
							local = FindSystemTimeZoneByFileName("Local", Path.Combine(TimeZoneDirectory, "localtime"));
						}
						catch
						{
							throw new TimeZoneNotFoundException();
						}
					}
				}
				return local;
			}
		}

		public string StandardName
		{
			get
			{
				return standardDisplayName;
			}
		}

		public bool SupportsDaylightSavingTime
		{
			get
			{
				return !disableDaylightSavingTime;
			}
		}

		public static TimeZoneInfo Utc
		{
			get
			{
				if (utc == null)
				{
					utc = CreateCustomTimeZone("UTC", new TimeSpan(0L), "UTC", "UTC");
				}
				return utc;
			}
		}

		private static string TimeZoneDirectory
		{
			get
			{
				if (timeZoneDirectory == null)
				{
					timeZoneDirectory = "/usr/share/zoneinfo";
				}
				return timeZoneDirectory;
			}
			set
			{
				ClearCachedData();
				timeZoneDirectory = value;
			}
		}

		private TimeZoneInfo(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, AdjustmentRule[] adjustmentRules, bool disableDaylightSavingTime)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id == string.Empty)
			{
				throw new ArgumentException("id parameter is an empty string");
			}
			if (baseUtcOffset.Ticks % 600000000 != 0L)
			{
				throw new ArgumentException("baseUtcOffset parameter does not represent a whole number of minutes");
			}
			if (baseUtcOffset > new TimeSpan(14, 0, 0) || baseUtcOffset < new TimeSpan(-14, 0, 0))
			{
				throw new ArgumentOutOfRangeException("baseUtcOffset parameter is greater than 14 hours or less than -14 hours");
			}
			if (adjustmentRules != null && adjustmentRules.Length != 0)
			{
				AdjustmentRule adjustmentRule = null;
				foreach (AdjustmentRule adjustmentRule2 in adjustmentRules)
				{
					if (adjustmentRule2 == null)
					{
						throw new InvalidTimeZoneException("one or more elements in adjustmentRules are null");
					}
					if (baseUtcOffset + adjustmentRule2.DaylightDelta < new TimeSpan(-14, 0, 0) || baseUtcOffset + adjustmentRule2.DaylightDelta > new TimeSpan(14, 0, 0))
					{
						throw new InvalidTimeZoneException("Sum of baseUtcOffset and DaylightDelta of one or more object in adjustmentRules array is greater than 14 or less than -14 hours;");
					}
					if (adjustmentRule != null && adjustmentRule.DateStart > adjustmentRule2.DateStart)
					{
						throw new InvalidTimeZoneException("adjustment rules specified in adjustmentRules parameter are not in chronological order");
					}
					if (adjustmentRule != null && adjustmentRule.DateEnd > adjustmentRule2.DateStart)
					{
						throw new InvalidTimeZoneException("some adjustment rules in the adjustmentRules parameter overlap");
					}
					if (adjustmentRule != null && adjustmentRule.DateEnd == adjustmentRule2.DateStart)
					{
						throw new InvalidTimeZoneException("a date can have multiple adjustment rules applied to it");
					}
					adjustmentRule = adjustmentRule2;
				}
			}
			this.id = id;
			this.baseUtcOffset = baseUtcOffset;
			this.displayName = displayName ?? id;
			this.standardDisplayName = standardDisplayName ?? id;
			this.daylightDisplayName = daylightDisplayName;
			this.disableDaylightSavingTime = disableDaylightSavingTime;
			this.adjustmentRules = adjustmentRules;
		}

		public static void ClearCachedData()
		{
			local = null;
			utc = null;
			systemTimeZones = null;
		}

		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo destinationTimeZone)
		{
			return ConvertTime(dateTime, Local, destinationTimeZone);
		}

		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
		{
			if (dateTime.Kind == DateTimeKind.Local && sourceTimeZone != Local)
			{
				throw new ArgumentException("Kind propery of dateTime is Local but the sourceTimeZone does not equal TimeZoneInfo.Local");
			}
			if (dateTime.Kind == DateTimeKind.Utc && sourceTimeZone != Utc)
			{
				throw new ArgumentException("Kind propery of dateTime is Utc but the sourceTimeZone does not equal TimeZoneInfo.Utc");
			}
			if (sourceTimeZone.IsInvalidTime(dateTime))
			{
				throw new ArgumentException("dateTime parameter is an invalid time");
			}
			if (sourceTimeZone == null)
			{
				throw new ArgumentNullException("sourceTimeZone");
			}
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			if (dateTime.Kind == DateTimeKind.Local && sourceTimeZone == Local && destinationTimeZone == Local)
			{
				return dateTime;
			}
			DateTime dateTime2 = ConvertTimeToUtc(dateTime);
			if (destinationTimeZone == Utc)
			{
				return dateTime2;
			}
			return ConvertTimeFromUtc(dateTime2, destinationTimeZone);
		}

		public static DateTimeOffset ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone)
		{
			throw new NotImplementedException();
		}

		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string destinationTimeZoneId)
		{
			return ConvertTime(dateTime, FindSystemTimeZoneById(destinationTimeZoneId));
		}

		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string sourceTimeZoneId, string destinationTimeZoneId)
		{
			return ConvertTime(dateTime, FindSystemTimeZoneById(sourceTimeZoneId), FindSystemTimeZoneById(destinationTimeZoneId));
		}

		public static DateTimeOffset ConvertTimeBySystemTimeZoneId(DateTimeOffset dateTimeOffset, string destinationTimeZoneId)
		{
			return ConvertTime(dateTimeOffset, FindSystemTimeZoneById(destinationTimeZoneId));
		}

		private DateTime ConvertTimeFromUtc(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Local)
			{
				throw new ArgumentException("Kind property of dateTime is Local");
			}
			if (this == Utc)
			{
				return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			}
			if (this == Local)
			{
				return DateTime.SpecifyKind(dateTime.ToLocalTime(), DateTimeKind.Unspecified);
			}
			AdjustmentRule applicableRule = GetApplicableRule(dateTime);
			if (IsDaylightSavingTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)))
			{
				return DateTime.SpecifyKind(dateTime + BaseUtcOffset + applicableRule.DaylightDelta, DateTimeKind.Unspecified);
			}
			return DateTime.SpecifyKind(dateTime + BaseUtcOffset, DateTimeKind.Unspecified);
		}

		public static DateTime ConvertTimeFromUtc(DateTime dateTime, TimeZoneInfo destinationTimeZone)
		{
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			return destinationTimeZone.ConvertTimeFromUtc(dateTime);
		}

		public static DateTime ConvertTimeToUtc(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime;
			}
			return DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
		}

		public static DateTime ConvertTimeToUtc(DateTime dateTime, TimeZoneInfo sourceTimeZone)
		{
			if (sourceTimeZone == null)
			{
				throw new ArgumentNullException("sourceTimeZone");
			}
			if (dateTime.Kind == DateTimeKind.Utc && sourceTimeZone != Utc)
			{
				throw new ArgumentException("Kind propery of dateTime is Utc but the sourceTimeZone does not equal TimeZoneInfo.Utc");
			}
			if (dateTime.Kind == DateTimeKind.Local && sourceTimeZone != Local)
			{
				throw new ArgumentException("Kind propery of dateTime is Local but the sourceTimeZone does not equal TimeZoneInfo.Local");
			}
			if (sourceTimeZone.IsInvalidTime(dateTime))
			{
				throw new ArgumentException("dateTime parameter is an invalid time");
			}
			if (dateTime.Kind == DateTimeKind.Utc && sourceTimeZone == Utc)
			{
				return dateTime;
			}
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime;
			}
			if (dateTime.Kind == DateTimeKind.Local)
			{
				return ConvertTimeToUtc(dateTime);
			}
			if (sourceTimeZone.IsAmbiguousTime(dateTime) || !sourceTimeZone.IsDaylightSavingTime(dateTime))
			{
				return DateTime.SpecifyKind(dateTime - sourceTimeZone.BaseUtcOffset, DateTimeKind.Utc);
			}
			AdjustmentRule applicableRule = sourceTimeZone.GetApplicableRule(dateTime);
			return DateTime.SpecifyKind(dateTime - sourceTimeZone.BaseUtcOffset - applicableRule.DaylightDelta, DateTimeKind.Utc);
		}

		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName)
		{
			return CreateCustomTimeZone(id, baseUtcOffset, displayName, standardDisplayName, null, null, true);
		}

		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, AdjustmentRule[] adjustmentRules)
		{
			return CreateCustomTimeZone(id, baseUtcOffset, displayName, standardDisplayName, daylightDisplayName, adjustmentRules, false);
		}

		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, AdjustmentRule[] adjustmentRules, bool disableDaylightSavingTime)
		{
			return new TimeZoneInfo(id, baseUtcOffset, displayName, standardDisplayName, daylightDisplayName, adjustmentRules, disableDaylightSavingTime);
		}

		public bool Equals(TimeZoneInfo other)
		{
			if (other == null)
			{
				return false;
			}
			return other.Id == Id && HasSameRules(other);
		}

		public static TimeZoneInfo FindSystemTimeZoneById(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			string filepath = Path.Combine(TimeZoneDirectory, id);
			return FindSystemTimeZoneByFileName(id, filepath);
		}

		private static TimeZoneInfo FindSystemTimeZoneByFileName(string id, string filepath)
		{
			if (!File.Exists(filepath))
			{
				throw new TimeZoneNotFoundException();
			}
			byte[] array = new byte[16384];
			int length;
			using (FileStream fileStream = File.OpenRead(filepath))
			{
				length = fileStream.Read(array, 0, 16384);
			}
			if (!ValidTZFile(array, length))
			{
				throw new InvalidTimeZoneException("TZ file too big for the buffer");
			}
			try
			{
				return ParseTZBuffer(id, array, length);
			}
			catch (Exception ex)
			{
				throw new InvalidTimeZoneException(ex.Message);
			}
		}

		public static TimeZoneInfo FromSerializedString(string source)
		{
			throw new NotImplementedException();
		}

		public AdjustmentRule[] GetAdjustmentRules()
		{
			if (disableDaylightSavingTime)
			{
				return new AdjustmentRule[0];
			}
			return (AdjustmentRule[])adjustmentRules.Clone();
		}

		public TimeSpan[] GetAmbiguousTimeOffsets(DateTime dateTime)
		{
			if (!IsAmbiguousTime(dateTime))
			{
				throw new ArgumentException("dateTime is not an ambiguous time");
			}
			AdjustmentRule applicableRule = GetApplicableRule(dateTime);
			return new TimeSpan[2]
			{
				baseUtcOffset,
				baseUtcOffset + applicableRule.DaylightDelta
			};
		}

		public TimeSpan[] GetAmbiguousTimeOffsets(DateTimeOffset dateTimeOffset)
		{
			if (!IsAmbiguousTime(dateTimeOffset))
			{
				throw new ArgumentException("dateTimeOffset is not an ambiguous time");
			}
			throw new NotImplementedException();
		}

		public override int GetHashCode()
		{
			int num = Id.GetHashCode();
			AdjustmentRule[] array = GetAdjustmentRules();
			foreach (AdjustmentRule adjustmentRule in array)
			{
				num ^= adjustmentRule.GetHashCode();
			}
			return num;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public static ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
		{
			if (systemTimeZones == null)
			{
				systemTimeZones = new List<TimeZoneInfo>();
				string[] array = new string[15]
				{
					"Africa", "America", "Antarctica", "Arctic", "Asia", "Atlantic", "Brazil", "Canada", "Chile", "Europe",
					"Indian", "Mexico", "Mideast", "Pacific", "US"
				};
				string[] array2 = array;
				foreach (string text in array2)
				{
					try
					{
						string[] files = Directory.GetFiles(Path.Combine(TimeZoneDirectory, text));
						foreach (string path in files)
						{
							try
							{
								string text2 = string.Format("{0}/{1}", text, Path.GetFileName(path));
								systemTimeZones.Add(FindSystemTimeZoneById(text2));
							}
							catch (ArgumentNullException)
							{
							}
							catch (TimeZoneNotFoundException)
							{
							}
							catch (InvalidTimeZoneException)
							{
							}
							catch (Exception)
							{
								throw;
							}
						}
					}
					catch
					{
					}
				}
			}
			return new ReadOnlyCollection<TimeZoneInfo>(systemTimeZones);
		}

		public TimeSpan GetUtcOffset(DateTime dateTime)
		{
			if (IsDaylightSavingTime(dateTime))
			{
				AdjustmentRule applicableRule = GetApplicableRule(dateTime);
				return BaseUtcOffset + applicableRule.DaylightDelta;
			}
			return BaseUtcOffset;
		}

		public TimeSpan GetUtcOffset(DateTimeOffset dateTimeOffset)
		{
			throw new NotImplementedException();
		}

		public bool HasSameRules(TimeZoneInfo other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (adjustmentRules == null != (other.adjustmentRules == null))
			{
				return false;
			}
			if (adjustmentRules == null)
			{
				return true;
			}
			if (BaseUtcOffset != other.BaseUtcOffset)
			{
				return false;
			}
			if (adjustmentRules.Length != other.adjustmentRules.Length)
			{
				return false;
			}
			for (int i = 0; i < adjustmentRules.Length; i++)
			{
				if (!adjustmentRules[i].Equals(other.adjustmentRules[i]))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsAmbiguousTime(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Local && IsInvalidTime(dateTime))
			{
				throw new ArgumentException("Kind is Local and time is Invalid");
			}
			if (this == Utc)
			{
				return false;
			}
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				dateTime = ConvertTimeFromUtc(dateTime);
			}
			if (dateTime.Kind == DateTimeKind.Local && this != Local)
			{
				dateTime = ConvertTime(dateTime, Local, this);
			}
			AdjustmentRule applicableRule = GetApplicableRule(dateTime);
			DateTime dateTime2 = TransitionPoint(applicableRule.DaylightTransitionEnd, dateTime.Year);
			if (dateTime > dateTime2 - applicableRule.DaylightDelta && dateTime <= dateTime2)
			{
				return true;
			}
			return false;
		}

		public bool IsAmbiguousTime(DateTimeOffset dateTimeOffset)
		{
			throw new NotImplementedException();
		}

		public bool IsDaylightSavingTime(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Local && IsInvalidTime(dateTime))
			{
				throw new ArgumentException("dateTime is invalid and Kind is Local");
			}
			if (this == Utc)
			{
				return false;
			}
			if (!SupportsDaylightSavingTime)
			{
				return false;
			}
			if ((dateTime.Kind == DateTimeKind.Local || dateTime.Kind == DateTimeKind.Unspecified) && this == Local)
			{
				return dateTime.IsDaylightSavingTime();
			}
			if (dateTime.Kind == DateTimeKind.Local && this != Utc)
			{
				return IsDaylightSavingTime(DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc));
			}
			AdjustmentRule applicableRule = GetApplicableRule(dateTime.Date);
			if (applicableRule == null)
			{
				return false;
			}
			DateTime dateTime2 = TransitionPoint(applicableRule.DaylightTransitionStart, dateTime.Year);
			DateTime dateTime3 = TransitionPoint(applicableRule.DaylightTransitionEnd, dateTime.Year + ((applicableRule.DaylightTransitionStart.Month >= applicableRule.DaylightTransitionEnd.Month) ? 1 : 0));
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				dateTime2 -= BaseUtcOffset;
				dateTime3 -= BaseUtcOffset + applicableRule.DaylightDelta;
			}
			return dateTime >= dateTime2 && dateTime < dateTime3;
		}

		public bool IsDaylightSavingTime(DateTimeOffset dateTimeOffset)
		{
			throw new NotImplementedException();
		}

		public bool IsInvalidTime(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return false;
			}
			if (dateTime.Kind == DateTimeKind.Local && this != Local)
			{
				return false;
			}
			AdjustmentRule applicableRule = GetApplicableRule(dateTime);
			DateTime dateTime2 = TransitionPoint(applicableRule.DaylightTransitionStart, dateTime.Year);
			if (dateTime >= dateTime2 && dateTime < dateTime2 + applicableRule.DaylightDelta)
			{
				return true;
			}
			return false;
		}

		public void OnDeserialization(object sender)
		{
			throw new NotImplementedException();
		}

		public string ToSerializedString()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return DisplayName;
		}

		private AdjustmentRule GetApplicableRule(DateTime dateTime)
		{
			DateTime dateTime2 = dateTime;
			if (dateTime.Kind == DateTimeKind.Local && this != Local)
			{
				dateTime2 = dateTime2.ToUniversalTime() + BaseUtcOffset;
			}
			if (dateTime.Kind == DateTimeKind.Utc && this != Utc)
			{
				dateTime2 += BaseUtcOffset;
			}
			AdjustmentRule[] array = adjustmentRules;
			foreach (AdjustmentRule adjustmentRule in array)
			{
				if (adjustmentRule.DateStart > dateTime2.Date)
				{
					return null;
				}
				if (!(adjustmentRule.DateEnd < dateTime2.Date))
				{
					return adjustmentRule;
				}
			}
			return null;
		}

		private static DateTime TransitionPoint(TransitionTime transition, int year)
		{
			if (transition.IsFixedDateRule)
			{
				return new DateTime(year, transition.Month, transition.Day) + transition.TimeOfDay.TimeOfDay;
			}
			DayOfWeek dayOfWeek = new DateTime(year, transition.Month, 1).DayOfWeek;
			int num = 1 + (transition.Week - 1) * 7 + (transition.DayOfWeek - dayOfWeek) % 7;
			if (num > DateTime.DaysInMonth(year, transition.Month))
			{
				num -= 7;
			}
			return new DateTime(year, transition.Month, num) + transition.TimeOfDay.TimeOfDay;
		}

		private static bool ValidTZFile(byte[] buffer, int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 4; i++)
			{
				stringBuilder.Append((char)buffer[i]);
			}
			if (stringBuilder.ToString() != "TZif")
			{
				return false;
			}
			if (length >= 16384)
			{
				return false;
			}
			return true;
		}

		private static int SwapInt32(int i)
		{
			return ((i >> 24) & 0xFF) | ((i >> 8) & 0xFF00) | ((i << 8) & 0xFF0000) | (i << 24);
		}

		private static int ReadBigEndianInt32(byte[] buffer, int start)
		{
			int num = BitConverter.ToInt32(buffer, start);
			if (!BitConverter.IsLittleEndian)
			{
				return num;
			}
			return SwapInt32(num);
		}

		private static TimeZoneInfo ParseTZBuffer(string id, byte[] buffer, int length)
		{
			int num = ReadBigEndianInt32(buffer, 20);
			int num2 = ReadBigEndianInt32(buffer, 24);
			int num3 = ReadBigEndianInt32(buffer, 28);
			int num4 = ReadBigEndianInt32(buffer, 32);
			int num5 = ReadBigEndianInt32(buffer, 36);
			int num6 = ReadBigEndianInt32(buffer, 40);
			if (length < 44 + num4 * 5 + num5 * 6 + num6 + num3 * 8 + num2 + num)
			{
				throw new InvalidTimeZoneException();
			}
			Dictionary<int, string> abbreviations = ParseAbbreviations(buffer, 44 + 4 * num4 + num4 + 6 * num5, num6);
			Dictionary<int, TimeType> dictionary = ParseTimesTypes(buffer, 44 + 4 * num4 + num4, num5, abbreviations);
			List<KeyValuePair<DateTime, TimeType>> list = ParseTransitions(buffer, 44, num4, dictionary);
			if (dictionary.Count == 0)
			{
				throw new InvalidTimeZoneException();
			}
			if (dictionary.Count == 1 && dictionary[0].IsDst)
			{
				throw new InvalidTimeZoneException();
			}
			TimeSpan timeSpan = new TimeSpan(0L);
			TimeSpan timeSpan2 = new TimeSpan(0L);
			string text = null;
			string text2 = null;
			bool flag = false;
			DateTime dateTime = DateTime.MinValue;
			List<AdjustmentRule> list2 = new List<AdjustmentRule>();
			for (int i = 0; i < list.Count; i++)
			{
				KeyValuePair<DateTime, TimeType> keyValuePair = list[i];
				DateTime key = keyValuePair.Key;
				TimeType value = keyValuePair.Value;
				if (!value.IsDst)
				{
					if (text != value.Name || timeSpan.TotalSeconds != (double)value.Offset)
					{
						text = value.Name;
						text2 = null;
						timeSpan = new TimeSpan(0, 0, value.Offset);
						list2 = new List<AdjustmentRule>();
						flag = false;
					}
					if (flag)
					{
						dateTime += timeSpan;
						DateTime dateTime2 = key + timeSpan + timeSpan2;
						if (dateTime2.Date == new DateTime(dateTime2.Year, 1, 1) && dateTime2.Year > dateTime.Year)
						{
							dateTime2 -= new TimeSpan(24, 0, 0);
						}
						DateTime dateStart = ((dateTime.Month >= 7) ? new DateTime(dateTime.Year, 7, 1) : new DateTime(dateTime.Year, 1, 1));
						DateTime dateEnd = ((dateTime2.Month < 7) ? new DateTime(dateTime2.Year, 6, 30) : new DateTime(dateTime2.Year, 12, 31));
						TransitionTime transitionTime = TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1) + dateTime.TimeOfDay, dateTime.Month, dateTime.Day);
						TransitionTime transitionTime2 = TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1) + dateTime2.TimeOfDay, dateTime2.Month, dateTime2.Day);
						if (transitionTime != transitionTime2)
						{
							list2.Add(AdjustmentRule.CreateAdjustmentRule(dateStart, dateEnd, timeSpan2, transitionTime, transitionTime2));
						}
					}
					flag = false;
				}
				else
				{
					if (text2 != value.Name || timeSpan2.TotalSeconds != (double)value.Offset - timeSpan.TotalSeconds)
					{
						text2 = value.Name;
						timeSpan2 = new TimeSpan(0, 0, value.Offset) - timeSpan;
					}
					dateTime = key;
					flag = true;
				}
			}
			if (list2.Count == 0)
			{
				TimeType timeType = dictionary[0];
				if (text == null)
				{
					text = timeType.Name;
					timeSpan = new TimeSpan(0, 0, timeType.Offset);
				}
				return CreateCustomTimeZone(id, timeSpan, id, text);
			}
			return CreateCustomTimeZone(id, timeSpan, id, text, text2, ValidateRules(list2).ToArray());
		}

		private static List<AdjustmentRule> ValidateRules(List<AdjustmentRule> adjustmentRules)
		{
			AdjustmentRule adjustmentRule = null;
			AdjustmentRule[] array = adjustmentRules.ToArray();
			foreach (AdjustmentRule adjustmentRule2 in array)
			{
				if (adjustmentRule != null && adjustmentRule.DateEnd > adjustmentRule2.DateStart)
				{
					adjustmentRules.Remove(adjustmentRule2);
				}
				adjustmentRule = adjustmentRule2;
			}
			return adjustmentRules;
		}

		private static Dictionary<int, string> ParseAbbreviations(byte[] buffer, int index, int count)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				char c = (char)buffer[index + i];
				if (c != 0)
				{
					stringBuilder.Append(c);
					continue;
				}
				dictionary.Add(num, stringBuilder.ToString());
				for (int j = 1; j < stringBuilder.Length; j++)
				{
					dictionary.Add(num + j, stringBuilder.ToString(j, stringBuilder.Length - j));
				}
				num = i + 1;
				stringBuilder = new StringBuilder();
			}
			return dictionary;
		}

		private static Dictionary<int, TimeType> ParseTimesTypes(byte[] buffer, int index, int count, Dictionary<int, string> abbreviations)
		{
			Dictionary<int, TimeType> dictionary = new Dictionary<int, TimeType>(count);
			for (int i = 0; i < count; i++)
			{
				int offset = ReadBigEndianInt32(buffer, index + 6 * i);
				byte b = buffer[index + 6 * i + 4];
				byte key = buffer[index + 6 * i + 5];
				dictionary.Add(i, new TimeType(offset, b != 0, abbreviations[key]));
			}
			return dictionary;
		}

		private static List<KeyValuePair<DateTime, TimeType>> ParseTransitions(byte[] buffer, int index, int count, Dictionary<int, TimeType> time_types)
		{
			List<KeyValuePair<DateTime, TimeType>> list = new List<KeyValuePair<DateTime, TimeType>>(count);
			for (int i = 0; i < count; i++)
			{
				int num = ReadBigEndianInt32(buffer, index + 4 * i);
				DateTime key = DateTimeFromUnixTime(num);
				byte key2 = buffer[index + 4 * count + i];
				list.Add(new KeyValuePair<DateTime, TimeType>(key, time_types[key2]));
			}
			return list;
		}

		private static DateTime DateTimeFromUnixTime(long unix_time)
		{
			return new DateTime(1970, 1, 1).AddSeconds(unix_time);
		}
	}
}
