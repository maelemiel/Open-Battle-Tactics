using System.Globalization;

namespace System
{
	[Serializable]
	internal sealed class CultureAwareComparer : StringComparer
	{
		private readonly bool _ignoreCase;

		private readonly CompareInfo _compareInfo;

		public CultureAwareComparer(CultureInfo ci, bool ignore_case)
		{
			_compareInfo = ci.CompareInfo;
			_ignoreCase = ignore_case;
		}

		public override int Compare(string x, string y)
		{
			CompareOptions options = (_ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
			return _compareInfo.Compare(x, y, options);
		}

		public override bool Equals(string x, string y)
		{
			return Compare(x, y) == 0;
		}

		public override int GetHashCode(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			CompareOptions options = (_ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
			return _compareInfo.GetSortKey(s, options).GetHashCode();
		}
	}
}
