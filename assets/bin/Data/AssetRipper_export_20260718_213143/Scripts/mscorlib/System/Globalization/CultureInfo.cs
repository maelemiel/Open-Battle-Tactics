using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public class CultureInfo : ICloneable, IFormatProvider
	{
		private const int NumOptionalCalendars = 5;

		private const int GregorianTypeMask = 16777215;

		private const int CalendarTypeBits = 24;

		private const int InvariantCultureId = 127;

		private static volatile CultureInfo invariant_culture_info;

		private static object shared_table_lock;

		internal static int BootstrapCultureID;

		private bool m_isReadOnly;

		private int cultureID;

		[NonSerialized]
		private int parent_lcid;

		[NonSerialized]
		private int specific_lcid;

		[NonSerialized]
		private int datetime_index;

		[NonSerialized]
		private int number_index;

		private bool m_useUserOverride;

		[NonSerialized]
		private volatile NumberFormatInfo numInfo;

		private volatile DateTimeFormatInfo dateTimeInfo;

		private volatile TextInfo textInfo;

		private string m_name;

		[NonSerialized]
		private string displayname;

		[NonSerialized]
		private string englishname;

		[NonSerialized]
		private string nativename;

		[NonSerialized]
		private string iso3lang;

		[NonSerialized]
		private string iso2lang;

		[NonSerialized]
		private string icu_name;

		[NonSerialized]
		private string win3lang;

		[NonSerialized]
		private string territory;

		private volatile CompareInfo compareInfo;

		[NonSerialized]
		private unsafe readonly int* calendar_data;

		[NonSerialized]
		private unsafe readonly void* textinfo_data;

		[NonSerialized]
		private Calendar[] optional_calendars;

		[NonSerialized]
		private CultureInfo parent_culture;

		private int m_dataItem;

		private Calendar calendar;

		[NonSerialized]
		private bool constructed;

		[NonSerialized]
		internal byte[] cached_serialized_form;

		private static readonly string MSG_READONLY;

		private static Hashtable shared_by_number;

		private static Hashtable shared_by_name;

		public static CultureInfo InvariantCulture
		{
			get
			{
				return invariant_culture_info;
			}
		}

		public static CultureInfo CurrentCulture
		{
			get
			{
				return Thread.CurrentThread.CurrentCulture;
			}
		}

		public static CultureInfo CurrentUICulture
		{
			get
			{
				return Thread.CurrentThread.CurrentUICulture;
			}
		}

		internal string Territory
		{
			get
			{
				return territory;
			}
		}

		public virtual int LCID
		{
			get
			{
				return cultureID;
			}
		}

		public virtual string Name
		{
			get
			{
				return m_name;
			}
		}

		public virtual string NativeName
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				return nativename;
			}
		}

		public virtual Calendar Calendar
		{
			get
			{
				return DateTimeFormat.Calendar;
			}
		}

		public virtual Calendar[] OptionalCalendars
		{
			get
			{
				if (optional_calendars == null)
				{
					lock (this)
					{
						if (optional_calendars == null)
						{
							ConstructCalendars();
						}
					}
				}
				return optional_calendars;
			}
		}

		public virtual CultureInfo Parent
		{
			get
			{
				if (parent_culture == null)
				{
					if (!constructed)
					{
						Construct();
					}
					if (parent_lcid == cultureID)
					{
						return null;
					}
					if (parent_lcid == 127)
					{
						parent_culture = InvariantCulture;
					}
					else if (cultureID == 127)
					{
						parent_culture = this;
					}
					else
					{
						parent_culture = new CultureInfo(parent_lcid);
					}
				}
				return parent_culture;
			}
		}

		public virtual TextInfo TextInfo
		{
			get
			{
				if (textInfo == null)
				{
					if (!constructed)
					{
						Construct();
					}
					lock (this)
					{
						if (textInfo == null)
						{
							textInfo = CreateTextInfo(m_isReadOnly);
						}
					}
				}
				return textInfo;
			}
		}

		public virtual string ThreeLetterISOLanguageName
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				return iso3lang;
			}
		}

		public virtual string ThreeLetterWindowsLanguageName
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				return win3lang;
			}
		}

		public virtual string TwoLetterISOLanguageName
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				return iso2lang;
			}
		}

		public bool UseUserOverride
		{
			get
			{
				return m_useUserOverride;
			}
		}

		internal string IcuName
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				return icu_name;
			}
		}

		public virtual CompareInfo CompareInfo
		{
			get
			{
				if (compareInfo == null)
				{
					if (!constructed)
					{
						Construct();
					}
					lock (this)
					{
						if (compareInfo == null)
						{
							compareInfo = new CompareInfo(this);
						}
					}
				}
				return compareInfo;
			}
		}

		public virtual bool IsNeutralCulture
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				if (cultureID == 127)
				{
					return false;
				}
				return (cultureID & 0xFF00) == 0 || specific_lcid == 0;
			}
		}

		public virtual NumberFormatInfo NumberFormat
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				CheckNeutral();
				if (numInfo == null)
				{
					lock (this)
					{
						if (numInfo == null)
						{
							numInfo = new NumberFormatInfo(m_isReadOnly);
							construct_number_format();
						}
					}
				}
				return numInfo;
			}
			set
			{
				if (!constructed)
				{
					Construct();
				}
				if (m_isReadOnly)
				{
					throw new InvalidOperationException(MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException("NumberFormat");
				}
				numInfo = value;
			}
		}

		public virtual DateTimeFormatInfo DateTimeFormat
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				CheckNeutral();
				if (dateTimeInfo == null)
				{
					lock (this)
					{
						if (dateTimeInfo == null)
						{
							dateTimeInfo = new DateTimeFormatInfo(m_isReadOnly);
							construct_datetime_format();
							if (optional_calendars != null)
							{
								dateTimeInfo.Calendar = optional_calendars[0];
							}
						}
					}
				}
				return dateTimeInfo;
			}
			set
			{
				if (!constructed)
				{
					Construct();
				}
				if (m_isReadOnly)
				{
					throw new InvalidOperationException(MSG_READONLY);
				}
				if (value == null)
				{
					throw new ArgumentNullException("DateTimeFormat");
				}
				dateTimeInfo = value;
			}
		}

		public virtual string DisplayName
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				return displayname;
			}
		}

		public virtual string EnglishName
		{
			get
			{
				if (!constructed)
				{
					Construct();
				}
				return englishname;
			}
		}

		public static CultureInfo InstalledUICulture
		{
			get
			{
				return GetCultureInfo(BootstrapCultureID);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return m_isReadOnly;
			}
		}

		public CultureInfo(int culture)
			: this(culture, true)
		{
		}

		public CultureInfo(int culture, bool useUserOverride)
			: this(culture, useUserOverride, false)
		{
		}

		private CultureInfo(int culture, bool useUserOverride, bool read_only)
		{
			if (culture < 0)
			{
				throw new ArgumentOutOfRangeException("culture", "Positive number required.");
			}
			constructed = true;
			m_isReadOnly = read_only;
			m_useUserOverride = useUserOverride;
			if (culture == 127)
			{
				ConstructInvariant(read_only);
			}
			else if (!ConstructInternalLocaleFromLcid(culture))
			{
				throw new ArgumentException(string.Format("Culture ID {0} (0x{0:X4}) is not a supported culture.", culture), "culture");
			}
		}

		public CultureInfo(string name)
			: this(name, true)
		{
		}

		public CultureInfo(string name, bool useUserOverride)
			: this(name, useUserOverride, false)
		{
		}

		private CultureInfo(string name, bool useUserOverride, bool read_only)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			constructed = true;
			m_isReadOnly = read_only;
			m_useUserOverride = useUserOverride;
			if (name.Length == 0)
			{
				ConstructInvariant(read_only);
			}
			else if (!ConstructInternalLocaleFromName(name.ToLowerInvariant()))
			{
				throw new ArgumentException("Culture name " + name + " is not supported.", "name");
			}
		}

		private CultureInfo()
		{
			constructed = true;
		}

		static CultureInfo()
		{
			shared_table_lock = new object();
			MSG_READONLY = "This instance is read only";
			invariant_culture_info = new CultureInfo(127, false, true);
		}

		public static CultureInfo CreateSpecificCulture(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name == string.Empty)
			{
				return InvariantCulture;
			}
			CultureInfo cultureInfo = new CultureInfo();
			if (!ConstructInternalLocaleFromSpecificName(cultureInfo, name.ToLowerInvariant()))
			{
				throw new ArgumentException("Culture name " + name + " is not supported.", name);
			}
			return cultureInfo;
		}

		internal static CultureInfo ConstructCurrentCulture()
		{
			CultureInfo cultureInfo = new CultureInfo();
			if (!ConstructInternalLocaleFromCurrentLocale(cultureInfo))
			{
				cultureInfo = InvariantCulture;
			}
			BootstrapCultureID = cultureInfo.cultureID;
			return cultureInfo;
		}

		internal static CultureInfo ConstructCurrentUICulture()
		{
			return ConstructCurrentCulture();
		}

		public void ClearCachedData()
		{
			Thread.CurrentThread.CurrentCulture = null;
			Thread.CurrentThread.CurrentUICulture = null;
		}

		public virtual object Clone()
		{
			if (!constructed)
			{
				Construct();
			}
			CultureInfo cultureInfo = (CultureInfo)MemberwiseClone();
			cultureInfo.m_isReadOnly = false;
			cultureInfo.cached_serialized_form = null;
			if (!IsNeutralCulture)
			{
				cultureInfo.NumberFormat = (NumberFormatInfo)NumberFormat.Clone();
				cultureInfo.DateTimeFormat = (DateTimeFormatInfo)DateTimeFormat.Clone();
			}
			return cultureInfo;
		}

		public override bool Equals(object value)
		{
			CultureInfo cultureInfo = value as CultureInfo;
			if (cultureInfo != null)
			{
				return cultureInfo.cultureID == cultureID;
			}
			return false;
		}

		public static CultureInfo[] GetCultures(CultureTypes types)
		{
			bool flag = (types & CultureTypes.NeutralCultures) != 0;
			bool specific = (types & CultureTypes.SpecificCultures) != 0;
			bool installed = (types & CultureTypes.InstalledWin32Cultures) != 0;
			CultureInfo[] array = internal_get_cultures(flag, specific, installed);
			if (flag && array.Length > 0 && array[0] == null)
			{
				array[0] = (CultureInfo)InvariantCulture.Clone();
			}
			return array;
		}

		public override int GetHashCode()
		{
			return cultureID;
		}

		public static CultureInfo ReadOnly(CultureInfo ci)
		{
			if (ci == null)
			{
				throw new ArgumentNullException("ci");
			}
			if (ci.m_isReadOnly)
			{
				return ci;
			}
			CultureInfo cultureInfo = (CultureInfo)ci.Clone();
			cultureInfo.m_isReadOnly = true;
			if (cultureInfo.numInfo != null)
			{
				cultureInfo.numInfo = NumberFormatInfo.ReadOnly(cultureInfo.numInfo);
			}
			if (cultureInfo.dateTimeInfo != null)
			{
				cultureInfo.dateTimeInfo = DateTimeFormatInfo.ReadOnly(cultureInfo.dateTimeInfo);
			}
			if (cultureInfo.textInfo != null)
			{
				cultureInfo.textInfo = TextInfo.ReadOnly(cultureInfo.textInfo);
			}
			return cultureInfo;
		}

		public override string ToString()
		{
			return m_name;
		}

		internal static bool IsIDNeutralCulture(int lcid)
		{
			bool is_neutral;
			if (!internal_is_lcid_neutral(lcid, out is_neutral))
			{
				throw new ArgumentException(string.Format("Culture id 0x{:x4} is not supported.", lcid));
			}
			return is_neutral;
		}

		internal void CheckNeutral()
		{
			if (IsNeutralCulture)
			{
				throw new NotSupportedException("Culture \"" + m_name + "\" is a neutral culture. It can not be used in formatting and parsing and therefore cannot be set as the thread's current culture.");
			}
		}

		public virtual object GetFormat(Type formatType)
		{
			object result = null;
			if (formatType == typeof(NumberFormatInfo))
			{
				result = NumberFormat;
			}
			else if (formatType == typeof(DateTimeFormatInfo))
			{
				result = DateTimeFormat;
			}
			return result;
		}

		private void Construct()
		{
			construct_internal_locale_from_lcid(cultureID);
			constructed = true;
		}

		private bool ConstructInternalLocaleFromName(string locale)
		{
			switch (locale)
			{
			case "zh-hans":
				locale = "zh-chs";
				break;
			case "zh-hant":
				locale = "zh-cht";
				break;
			}
			if (!construct_internal_locale_from_name(locale))
			{
				return false;
			}
			return true;
		}

		private bool ConstructInternalLocaleFromLcid(int lcid)
		{
			if (!construct_internal_locale_from_lcid(lcid))
			{
				return false;
			}
			return true;
		}

		private static bool ConstructInternalLocaleFromSpecificName(CultureInfo ci, string name)
		{
			if (!construct_internal_locale_from_specific_name(ci, name))
			{
				return false;
			}
			return true;
		}

		private static bool ConstructInternalLocaleFromCurrentLocale(CultureInfo ci)
		{
			if (!construct_internal_locale_from_current_locale(ci))
			{
				return false;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool construct_internal_locale_from_lcid(int lcid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool construct_internal_locale_from_name(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool construct_internal_locale_from_specific_name(CultureInfo ci, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool construct_internal_locale_from_current_locale(CultureInfo ci);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CultureInfo[] internal_get_cultures(bool neutral, bool specific, bool installed);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void construct_datetime_format();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void construct_number_format();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool internal_is_lcid_neutral(int lcid, out bool is_neutral);

		private void ConstructInvariant(bool read_only)
		{
			cultureID = 127;
			numInfo = NumberFormatInfo.InvariantInfo;
			dateTimeInfo = DateTimeFormatInfo.InvariantInfo;
			if (!read_only)
			{
				numInfo = (NumberFormatInfo)numInfo.Clone();
				dateTimeInfo = (DateTimeFormatInfo)dateTimeInfo.Clone();
			}
			textInfo = CreateTextInfo(read_only);
			m_name = string.Empty;
			displayname = (englishname = (nativename = "Invariant Language (Invariant Country)"));
			iso3lang = "IVL";
			iso2lang = "iv";
			icu_name = "en_US_POSIX";
			win3lang = "IVL";
		}

		private unsafe TextInfo CreateTextInfo(bool readOnly)
		{
			return new TextInfo(this, cultureID, textinfo_data, readOnly);
		}

		private static void insert_into_shared_tables(CultureInfo c)
		{
			if (shared_by_number == null)
			{
				shared_by_number = new Hashtable();
				shared_by_name = new Hashtable();
			}
			shared_by_number[c.cultureID] = c;
			shared_by_name[c.m_name] = c;
		}

		public static CultureInfo GetCultureInfo(int culture)
		{
			lock (shared_table_lock)
			{
				CultureInfo cultureInfo;
				if (shared_by_number != null)
				{
					cultureInfo = shared_by_number[culture] as CultureInfo;
					if (cultureInfo != null)
					{
						return cultureInfo;
					}
				}
				cultureInfo = new CultureInfo(culture, false, true);
				insert_into_shared_tables(cultureInfo);
				return cultureInfo;
			}
		}

		public static CultureInfo GetCultureInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			lock (shared_table_lock)
			{
				CultureInfo cultureInfo;
				if (shared_by_name != null)
				{
					cultureInfo = shared_by_name[name] as CultureInfo;
					if (cultureInfo != null)
					{
						return cultureInfo;
					}
				}
				cultureInfo = new CultureInfo(name, false, true);
				insert_into_shared_tables(cultureInfo);
				return cultureInfo;
			}
		}

		[MonoTODO("Currently it ignores the altName parameter")]
		public static CultureInfo GetCultureInfo(string name, string altName)
		{
			if (name == null)
			{
				throw new ArgumentNullException("null");
			}
			if (altName == null)
			{
				throw new ArgumentNullException("null");
			}
			return GetCultureInfo(name);
		}

		public static CultureInfo GetCultureInfoByIetfLanguageTag(string name)
		{
			switch (name)
			{
			case "zh-Hans":
				return GetCultureInfo("zh-CHS");
			case "zh-Hant":
				return GetCultureInfo("zh-CHT");
			default:
				return GetCultureInfo(name);
			}
		}

		internal static CultureInfo CreateCulture(string name, bool reference)
		{
			bool flag = name.Length == 0;
			bool useUserOverride;
			bool read_only;
			if (reference)
			{
				useUserOverride = !flag;
				read_only = false;
			}
			else
			{
				read_only = false;
				useUserOverride = !flag;
			}
			return new CultureInfo(name, useUserOverride, read_only);
		}

		internal unsafe void ConstructCalendars()
		{
			if (calendar_data == null)
			{
				optional_calendars = new Calendar[1]
				{
					new GregorianCalendar(GregorianCalendarTypes.Localized)
				};
				return;
			}
			optional_calendars = new Calendar[5];
			for (int i = 0; i < 5; i++)
			{
				Calendar calendar = null;
				int num = *(int*)((byte*)calendar_data + i * 4);
				switch (num >> 24)
				{
				case 0:
				{
					GregorianCalendarTypes type = (GregorianCalendarTypes)(num & 0xFFFFFF);
					calendar = new GregorianCalendar(type);
					break;
				}
				case 1:
					calendar = new HijriCalendar();
					break;
				case 2:
					calendar = new ThaiBuddhistCalendar();
					break;
				default:
					throw new Exception("invalid calendar type:  " + num);
				}
				optional_calendars[i] = calendar;
			}
		}
	}
}
