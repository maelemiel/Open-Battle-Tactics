using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Mono.Globalization.Unicode;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public class CompareInfo : IDeserializationCallback
	{
		private const CompareOptions ValidCompareOptions_NoStringSort = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.Ordinal | CompareOptions.OrdinalIgnoreCase;

		private const CompareOptions ValidCompareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort | CompareOptions.Ordinal | CompareOptions.OrdinalIgnoreCase;

		private static readonly bool useManagedCollation = Environment.internalGetEnvironmentVariable("MONO_DISABLE_MANAGED_COLLATION") != "yes" && MSCompatUnicodeTable.IsReady;

		private int culture;

		[NonSerialized]
		private string icu_name;

		private int win32LCID;

		private string m_name;

		[NonSerialized]
		private SimpleCollator collator;

		private static Hashtable collators;

		[NonSerialized]
		private static object monitor = new object();

		internal static bool UseManagedCollation
		{
			get
			{
				return useManagedCollation;
			}
		}

		public int LCID
		{
			get
			{
				return culture;
			}
		}

		[ComVisible(false)]
		public virtual string Name
		{
			get
			{
				return icu_name;
			}
		}

		private CompareInfo()
		{
		}

		internal CompareInfo(CultureInfo ci)
		{
			culture = ci.LCID;
			if (UseManagedCollation)
			{
				lock (monitor)
				{
					if (collators == null)
					{
						collators = new Hashtable();
					}
					collator = (SimpleCollator)collators[ci.LCID];
					if (collator == null)
					{
						collator = new SimpleCollator(ci);
						collators[ci.LCID] = collator;
					}
					return;
				}
			}
			icu_name = ci.IcuName;
			construct_compareinfo(icu_name);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			if (UseManagedCollation)
			{
				collator = new SimpleCollator(new CultureInfo(culture));
				return;
			}
			try
			{
				construct_compareinfo(icu_name);
			}
			catch
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void construct_compareinfo(string locale);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void free_internal_collator();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int internal_compare(string str1, int offset1, int length1, string str2, int offset2, int length2, CompareOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void assign_sortkey(object key, string source, CompareOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int internal_index(string source, int sindex, int count, char value, CompareOptions options, bool first);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int internal_index(string source, int sindex, int count, string value, CompareOptions options, bool first);

		~CompareInfo()
		{
			free_internal_collator();
		}

		private int internal_compare_managed(string str1, int offset1, int length1, string str2, int offset2, int length2, CompareOptions options)
		{
			return collator.Compare(str1, offset1, length1, str2, offset2, length2, options);
		}

		private int internal_compare_switch(string str1, int offset1, int length1, string str2, int offset2, int length2, CompareOptions options)
		{
			return (!UseManagedCollation) ? internal_compare(str1, offset1, length1, str2, offset2, length2, options) : internal_compare_managed(str1, offset1, length1, str2, offset2, length2, options);
		}

		public virtual int Compare(string string1, string string2)
		{
			return Compare(string1, string2, CompareOptions.None);
		}

		public virtual int Compare(string string1, string string2, CompareOptions options)
		{
			if ((options & (CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort | CompareOptions.Ordinal | CompareOptions.OrdinalIgnoreCase)) != options)
			{
				throw new ArgumentException("options");
			}
			if (string1 == null)
			{
				if (string2 == null)
				{
					return 0;
				}
				return -1;
			}
			if (string2 == null)
			{
				return 1;
			}
			if (string1.Length == 0 && string2.Length == 0)
			{
				return 0;
			}
			return internal_compare_switch(string1, 0, string1.Length, string2, 0, string2.Length, options);
		}

		public virtual int Compare(string string1, int offset1, string string2, int offset2)
		{
			return Compare(string1, offset1, string2, offset2, CompareOptions.None);
		}

		public virtual int Compare(string string1, int offset1, string string2, int offset2, CompareOptions options)
		{
			if ((options & (CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort | CompareOptions.Ordinal | CompareOptions.OrdinalIgnoreCase)) != options)
			{
				throw new ArgumentException("options");
			}
			if (string1 == null)
			{
				if (string2 == null)
				{
					return 0;
				}
				return -1;
			}
			if (string2 == null)
			{
				return 1;
			}
			if ((string1.Length == 0 || offset1 == string1.Length) && (string2.Length == 0 || offset2 == string2.Length))
			{
				return 0;
			}
			if (offset1 < 0 || offset2 < 0)
			{
				throw new ArgumentOutOfRangeException("Offsets must not be less than zero");
			}
			if (offset1 > string1.Length)
			{
				throw new ArgumentOutOfRangeException("Offset1 is greater than or equal to the length of string1");
			}
			if (offset2 > string2.Length)
			{
				throw new ArgumentOutOfRangeException("Offset2 is greater than or equal to the length of string2");
			}
			return internal_compare_switch(string1, offset1, string1.Length - offset1, string2, offset2, string2.Length - offset2, options);
		}

		public virtual int Compare(string string1, int offset1, int length1, string string2, int offset2, int length2)
		{
			return Compare(string1, offset1, length1, string2, offset2, length2, CompareOptions.None);
		}

		public virtual int Compare(string string1, int offset1, int length1, string string2, int offset2, int length2, CompareOptions options)
		{
			if ((options & (CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.StringSort | CompareOptions.Ordinal | CompareOptions.OrdinalIgnoreCase)) != options)
			{
				throw new ArgumentException("options");
			}
			if (string1 == null)
			{
				if (string2 == null)
				{
					return 0;
				}
				return -1;
			}
			if (string2 == null)
			{
				return 1;
			}
			if ((string1.Length == 0 || offset1 == string1.Length || length1 == 0) && (string2.Length == 0 || offset2 == string2.Length || length2 == 0))
			{
				return 0;
			}
			if (offset1 < 0 || length1 < 0 || offset2 < 0 || length2 < 0)
			{
				throw new ArgumentOutOfRangeException("Offsets and lengths must not be less than zero");
			}
			if (offset1 > string1.Length)
			{
				throw new ArgumentOutOfRangeException("Offset1 is greater than or equal to the length of string1");
			}
			if (offset2 > string2.Length)
			{
				throw new ArgumentOutOfRangeException("Offset2 is greater than or equal to the length of string2");
			}
			if (length1 > string1.Length - offset1)
			{
				throw new ArgumentOutOfRangeException("Length1 is greater than the number of characters from offset1 to the end of string1");
			}
			if (length2 > string2.Length - offset2)
			{
				throw new ArgumentOutOfRangeException("Length2 is greater than the number of characters from offset2 to the end of string2");
			}
			return internal_compare_switch(string1, offset1, length1, string2, offset2, length2, options);
		}

		public override bool Equals(object value)
		{
			CompareInfo compareInfo = value as CompareInfo;
			if (compareInfo == null)
			{
				return false;
			}
			return compareInfo.culture == culture;
		}

		public static CompareInfo GetCompareInfo(int culture)
		{
			return new CultureInfo(culture).CompareInfo;
		}

		public static CompareInfo GetCompareInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return new CultureInfo(name).CompareInfo;
		}

		public static CompareInfo GetCompareInfo(int culture, Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (assembly != typeof(object).Module.Assembly)
			{
				throw new ArgumentException("Assembly is an invalid type");
			}
			return GetCompareInfo(culture);
		}

		public static CompareInfo GetCompareInfo(string name, Assembly assembly)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (assembly != typeof(object).Module.Assembly)
			{
				throw new ArgumentException("Assembly is an invalid type");
			}
			return GetCompareInfo(name);
		}

		public override int GetHashCode()
		{
			return LCID;
		}

		public virtual SortKey GetSortKey(string source)
		{
			return GetSortKey(source, CompareOptions.None);
		}

		public virtual SortKey GetSortKey(string source, CompareOptions options)
		{
			if (options == CompareOptions.OrdinalIgnoreCase || options == CompareOptions.Ordinal)
			{
				throw new ArgumentException("Now allowed CompareOptions.", "options");
			}
			if (UseManagedCollation)
			{
				return collator.GetSortKey(source, options);
			}
			SortKey sortKey = new SortKey(culture, source, options);
			assign_sortkey(sortKey, source, options);
			return sortKey;
		}

		public virtual int IndexOf(string source, char value)
		{
			return IndexOf(source, value, 0, source.Length, CompareOptions.None);
		}

		public virtual int IndexOf(string source, string value)
		{
			return IndexOf(source, value, 0, source.Length, CompareOptions.None);
		}

		public virtual int IndexOf(string source, char value, CompareOptions options)
		{
			return IndexOf(source, value, 0, source.Length, options);
		}

		public virtual int IndexOf(string source, char value, int startIndex)
		{
			return IndexOf(source, value, startIndex, source.Length - startIndex, CompareOptions.None);
		}

		public virtual int IndexOf(string source, string value, CompareOptions options)
		{
			return IndexOf(source, value, 0, source.Length, options);
		}

		public virtual int IndexOf(string source, string value, int startIndex)
		{
			return IndexOf(source, value, startIndex, source.Length - startIndex, CompareOptions.None);
		}

		public virtual int IndexOf(string source, char value, int startIndex, CompareOptions options)
		{
			return IndexOf(source, value, startIndex, source.Length - startIndex, options);
		}

		public virtual int IndexOf(string source, char value, int startIndex, int count)
		{
			return IndexOf(source, value, startIndex, count, CompareOptions.None);
		}

		public virtual int IndexOf(string source, string value, int startIndex, CompareOptions options)
		{
			return IndexOf(source, value, startIndex, source.Length - startIndex, options);
		}

		public virtual int IndexOf(string source, string value, int startIndex, int count)
		{
			return IndexOf(source, value, startIndex, count, CompareOptions.None);
		}

		private int internal_index_managed(string s, int sindex, int count, char c, CompareOptions opt, bool first)
		{
			return (!first) ? collator.LastIndexOf(s, c, sindex, count, opt) : collator.IndexOf(s, c, sindex, count, opt);
		}

		private int internal_index_switch(string s, int sindex, int count, char c, CompareOptions opt, bool first)
		{
			return (!UseManagedCollation || (first && opt == CompareOptions.Ordinal)) ? internal_index(s, sindex, count, c, opt, first) : internal_index_managed(s, sindex, count, c, opt, first);
		}

		public virtual int IndexOf(string source, char value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || source.Length - startIndex < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((options & (CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.Ordinal | CompareOptions.OrdinalIgnoreCase)) != options)
			{
				throw new ArgumentException("options");
			}
			if (count == 0)
			{
				return -1;
			}
			if ((options & CompareOptions.Ordinal) != CompareOptions.None)
			{
				for (int i = startIndex; i < startIndex + count; i++)
				{
					if (source[i] == value)
					{
						return i;
					}
				}
				return -1;
			}
			return internal_index_switch(source, startIndex, count, value, options, true);
		}

		private int internal_index_managed(string s1, int sindex, int count, string s2, CompareOptions opt, bool first)
		{
			return (!first) ? collator.LastIndexOf(s1, s2, sindex, count, opt) : collator.IndexOf(s1, s2, sindex, count, opt);
		}

		private int internal_index_switch(string s1, int sindex, int count, string s2, CompareOptions opt, bool first)
		{
			return (!UseManagedCollation || (first && opt == CompareOptions.Ordinal)) ? internal_index(s1, sindex, count, s2, opt, first) : internal_index_managed(s1, sindex, count, s2, opt, first);
		}

		public virtual int IndexOf(string source, string value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || source.Length - startIndex < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((options & (CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.Ordinal | CompareOptions.OrdinalIgnoreCase)) != options)
			{
				throw new ArgumentException("options");
			}
			if (value.Length == 0)
			{
				return startIndex;
			}
			if (count == 0)
			{
				return -1;
			}
			return internal_index_switch(source, startIndex, count, value, options, true);
		}

		public virtual bool IsPrefix(string source, string prefix)
		{
			return IsPrefix(source, prefix, CompareOptions.None);
		}

		public virtual bool IsPrefix(string source, string prefix, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (UseManagedCollation)
			{
				return collator.IsPrefix(source, prefix, options);
			}
			if (source.Length < prefix.Length)
			{
				return false;
			}
			return Compare(source, 0, prefix.Length, prefix, 0, prefix.Length, options) == 0;
		}

		public virtual bool IsSuffix(string source, string suffix)
		{
			return IsSuffix(source, suffix, CompareOptions.None);
		}

		public virtual bool IsSuffix(string source, string suffix, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (suffix == null)
			{
				throw new ArgumentNullException("suffix");
			}
			if (UseManagedCollation)
			{
				return collator.IsSuffix(source, suffix, options);
			}
			if (source.Length < suffix.Length)
			{
				return false;
			}
			return Compare(source, source.Length - suffix.Length, suffix.Length, suffix, 0, suffix.Length, options) == 0;
		}

		public virtual int LastIndexOf(string source, char value)
		{
			return LastIndexOf(source, value, source.Length - 1, source.Length, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, string value)
		{
			return LastIndexOf(source, value, source.Length - 1, source.Length, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, char value, CompareOptions options)
		{
			return LastIndexOf(source, value, source.Length - 1, source.Length, options);
		}

		public virtual int LastIndexOf(string source, char value, int startIndex)
		{
			return LastIndexOf(source, value, startIndex, startIndex + 1, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, string value, CompareOptions options)
		{
			return LastIndexOf(source, value, source.Length - 1, source.Length, options);
		}

		public virtual int LastIndexOf(string source, string value, int startIndex)
		{
			return LastIndexOf(source, value, startIndex, startIndex + 1, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, char value, int startIndex, CompareOptions options)
		{
			return LastIndexOf(source, value, startIndex, startIndex + 1, options);
		}

		public virtual int LastIndexOf(string source, char value, int startIndex, int count)
		{
			return LastIndexOf(source, value, startIndex, count, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, string value, int startIndex, CompareOptions options)
		{
			return LastIndexOf(source, value, startIndex, startIndex + 1, options);
		}

		public virtual int LastIndexOf(string source, string value, int startIndex, int count)
		{
			return LastIndexOf(source, value, startIndex, count, CompareOptions.None);
		}

		public virtual int LastIndexOf(string source, char value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || startIndex - count < -1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((options & (CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.Ordinal | CompareOptions.OrdinalIgnoreCase)) != options)
			{
				throw new ArgumentException("options");
			}
			if (count == 0)
			{
				return -1;
			}
			if ((options & CompareOptions.Ordinal) != CompareOptions.None)
			{
				for (int num = startIndex; num > startIndex - count; num--)
				{
					if (source[num] == value)
					{
						return num;
					}
				}
				return -1;
			}
			return internal_index_switch(source, startIndex, count, value, options, false);
		}

		public virtual int LastIndexOf(string source, string value, int startIndex, int count, CompareOptions options)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || startIndex - count < -1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if ((options & (CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.Ordinal | CompareOptions.OrdinalIgnoreCase)) != options)
			{
				throw new ArgumentException("options");
			}
			if (count == 0)
			{
				return -1;
			}
			if (value.Length == 0)
			{
				return 0;
			}
			return internal_index_switch(source, startIndex, count, value, options, false);
		}

		[ComVisible(false)]
		public static bool IsSortable(char ch)
		{
			return MSCompatUnicodeTable.IsSortable(ch);
		}

		[ComVisible(false)]
		public static bool IsSortable(string text)
		{
			return MSCompatUnicodeTable.IsSortable(text);
		}

		public override string ToString()
		{
			return "CompareInfo - " + culture;
		}
	}
}
