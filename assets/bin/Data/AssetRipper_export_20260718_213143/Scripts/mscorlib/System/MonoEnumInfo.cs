using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
	internal struct MonoEnumInfo
	{
		internal class SByteComparer : IComparer<sbyte>, IComparer
		{
			public int Compare(object x, object y)
			{
				sbyte b = (sbyte)x;
				sbyte b2 = (sbyte)y;
				return (byte)b - (byte)b2;
			}

			public int Compare(sbyte ix, sbyte iy)
			{
				return (byte)ix - (byte)iy;
			}
		}

		internal class ShortComparer : IComparer<short>, IComparer
		{
			public int Compare(object x, object y)
			{
				short num = (short)x;
				short num2 = (short)y;
				return (ushort)num - (ushort)num2;
			}

			public int Compare(short ix, short iy)
			{
				return (ushort)ix - (ushort)iy;
			}
		}

		internal class IntComparer : IComparer<int>, IComparer
		{
			public int Compare(object x, object y)
			{
				int num = (int)x;
				int num2 = (int)y;
				if (num == num2)
				{
					return 0;
				}
				if ((uint)num < (uint)num2)
				{
					return -1;
				}
				return 1;
			}

			public int Compare(int ix, int iy)
			{
				if (ix == iy)
				{
					return 0;
				}
				if ((uint)ix < (uint)iy)
				{
					return -1;
				}
				return 1;
			}
		}

		internal class LongComparer : IComparer<long>, IComparer
		{
			public int Compare(object x, object y)
			{
				long num = (long)x;
				long num2 = (long)y;
				if (num == num2)
				{
					return 0;
				}
				if ((ulong)num < (ulong)num2)
				{
					return -1;
				}
				return 1;
			}

			public int Compare(long ix, long iy)
			{
				if (ix == iy)
				{
					return 0;
				}
				if ((ulong)ix < (ulong)iy)
				{
					return -1;
				}
				return 1;
			}
		}

		internal Type utype;

		internal Array values;

		internal string[] names;

		internal Hashtable name_hash;

		[ThreadStatic]
		private static Hashtable cache;

		private static Hashtable global_cache;

		private static object global_cache_monitor;

		internal static SByteComparer sbyte_comparer;

		internal static ShortComparer short_comparer;

		internal static IntComparer int_comparer;

		internal static LongComparer long_comparer;

		private static Hashtable Cache
		{
			get
			{
				if (cache == null)
				{
					cache = new Hashtable();
				}
				return cache;
			}
		}

		private MonoEnumInfo(MonoEnumInfo other)
		{
			utype = other.utype;
			values = other.values;
			names = other.names;
			name_hash = other.name_hash;
		}

		static MonoEnumInfo()
		{
			sbyte_comparer = new SByteComparer();
			short_comparer = new ShortComparer();
			int_comparer = new IntComparer();
			long_comparer = new LongComparer();
			global_cache_monitor = new object();
			global_cache = new Hashtable();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_enum_info(Type enumType, out MonoEnumInfo info);

		internal static void GetInfo(Type enumType, out MonoEnumInfo info)
		{
			if (Cache.ContainsKey(enumType))
			{
				info = (MonoEnumInfo)cache[enumType];
				return;
			}
			lock (global_cache_monitor)
			{
				if (global_cache.ContainsKey(enumType))
				{
					object obj = global_cache[enumType];
					cache[enumType] = obj;
					info = (MonoEnumInfo)obj;
					return;
				}
			}
			get_enum_info(enumType, out info);
			IComparer comparer = null;
			if (info.values is int[])
			{
				comparer = int_comparer;
			}
			else if (info.values is short[])
			{
				comparer = short_comparer;
			}
			else if (info.values is sbyte[])
			{
				comparer = sbyte_comparer;
			}
			else if (info.values is long[])
			{
				comparer = long_comparer;
			}
			Array.Sort(info.values, info.names, comparer);
			if (info.names.Length > 50)
			{
				info.name_hash = new Hashtable(info.names.Length);
				for (int i = 0; i < info.names.Length; i++)
				{
					info.name_hash[info.names[i]] = i;
				}
			}
			MonoEnumInfo monoEnumInfo = new MonoEnumInfo(info);
			lock (global_cache_monitor)
			{
				global_cache[enumType] = monoEnumInfo;
			}
		}
	}
}
