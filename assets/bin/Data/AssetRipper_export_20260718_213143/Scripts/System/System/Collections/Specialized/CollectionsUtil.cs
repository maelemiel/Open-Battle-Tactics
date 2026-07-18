namespace System.Collections.Specialized
{
	internal class CollectionsUtil
	{
		public static Hashtable CreateCaseInsensitiveHashtable()
		{
			return new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
		}

		public static Hashtable CreateCaseInsensitiveHashtable(IDictionary d)
		{
			return new Hashtable(d, CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
		}

		public static Hashtable CreateCaseInsensitiveHashtable(int capacity)
		{
			return new Hashtable(capacity, CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
		}

		public static SortedList CreateCaseInsensitiveSortedList()
		{
			return new SortedList(CaseInsensitiveComparer.Default);
		}
	}
}
