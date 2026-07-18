using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[Serializable]
	[ComVisible(true)]
	public class CaseInsensitiveComparer : IComparer
	{
		private static CaseInsensitiveComparer defaultComparer = new CaseInsensitiveComparer();

		private static CaseInsensitiveComparer defaultInvariantComparer = new CaseInsensitiveComparer(true);

		private CultureInfo culture;

		public static CaseInsensitiveComparer Default
		{
			get
			{
				return defaultComparer;
			}
		}

		public static CaseInsensitiveComparer DefaultInvariant
		{
			get
			{
				return defaultInvariantComparer;
			}
		}

		public CaseInsensitiveComparer()
		{
			culture = CultureInfo.CurrentCulture;
		}

		private CaseInsensitiveComparer(bool invariant)
		{
		}

		public CaseInsensitiveComparer(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (culture.LCID != CultureInfo.InvariantCulture.LCID)
			{
				this.culture = culture;
			}
		}

		public int Compare(object a, object b)
		{
			string text = a as string;
			string text2 = b as string;
			if (text != null && text2 != null)
			{
				if (culture != null)
				{
					return culture.CompareInfo.Compare(text, text2, CompareOptions.IgnoreCase);
				}
				return CultureInfo.InvariantCulture.CompareInfo.Compare(text, text2, CompareOptions.IgnoreCase);
			}
			return Comparer.Default.Compare(a, b);
		}
	}
}
