using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public abstract class StringComparer : IComparer<string>, IEqualityComparer<string>, IComparer, IEqualityComparer
	{
		private static StringComparer invariantCultureIgnoreCase = new CultureAwareComparer(CultureInfo.InvariantCulture, true);

		private static StringComparer invariantCulture = new CultureAwareComparer(CultureInfo.InvariantCulture, false);

		private static StringComparer ordinalIgnoreCase = new OrdinalComparer(true);

		private static StringComparer ordinal = new OrdinalComparer(false);

		public static StringComparer CurrentCulture
		{
			get
			{
				return new CultureAwareComparer(CultureInfo.CurrentCulture, false);
			}
		}

		public static StringComparer CurrentCultureIgnoreCase
		{
			get
			{
				return new CultureAwareComparer(CultureInfo.CurrentCulture, true);
			}
		}

		public static StringComparer InvariantCulture
		{
			get
			{
				return invariantCulture;
			}
		}

		public static StringComparer InvariantCultureIgnoreCase
		{
			get
			{
				return invariantCultureIgnoreCase;
			}
		}

		public static StringComparer Ordinal
		{
			get
			{
				return ordinal;
			}
		}

		public static StringComparer OrdinalIgnoreCase
		{
			get
			{
				return ordinalIgnoreCase;
			}
		}

		public static StringComparer Create(CultureInfo culture, bool ignoreCase)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return new CultureAwareComparer(culture, ignoreCase);
		}

		public int Compare(object x, object y)
		{
			if (x == y)
			{
				return 0;
			}
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			string text = x as string;
			if (text != null)
			{
				string text2 = y as string;
				if (text2 != null)
				{
					return Compare(text, text2);
				}
			}
			IComparable comparable = x as IComparable;
			if (comparable == null)
			{
				throw new ArgumentException();
			}
			return comparable.CompareTo(y);
		}

		public new bool Equals(object x, object y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			string text = x as string;
			if (text != null)
			{
				string text2 = y as string;
				if (text2 != null)
				{
					return Equals(text, text2);
				}
			}
			return x.Equals(y);
		}

		public int GetHashCode(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			string text = obj as string;
			return (text != null) ? GetHashCode(text) : obj.GetHashCode();
		}

		public abstract int Compare(string x, string y);

		public abstract bool Equals(string x, string y);

		public abstract int GetHashCode(string obj);
	}
}
