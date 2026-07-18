using System.Collections;

namespace System.Data.Common
{
	internal class DBComparerFactory
	{
		private class ComparebleComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == DBNull.Value)
				{
					if (y == DBNull.Value)
					{
						return 0;
					}
					return -1;
				}
				if (y == DBNull.Value)
				{
					return 1;
				}
				return ((IComparable)x).CompareTo(y);
			}
		}

		private class CaseComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == DBNull.Value)
				{
					if (y == DBNull.Value)
					{
						return 0;
					}
					return -1;
				}
				if (y == DBNull.Value)
				{
					return 1;
				}
				return string.Compare((string)x, (string)y, false);
			}
		}

		private class IgnoreCaseComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == DBNull.Value)
				{
					if (y == DBNull.Value)
					{
						return 0;
					}
					return -1;
				}
				if (y == DBNull.Value)
				{
					return 1;
				}
				return string.Compare((string)x, (string)y, true);
			}
		}

		private class ByteArrayComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == DBNull.Value)
				{
					if (y == DBNull.Value)
					{
						return 0;
					}
					return -1;
				}
				if (y == DBNull.Value)
				{
					return 1;
				}
				byte[] array = (byte[])x;
				byte[] array2 = (byte[])y;
				int num = array.Length;
				int num2 = array2.Length;
				int num3 = 0;
				while (true)
				{
					int num4 = 0;
					int num5 = 0;
					if (num3 < num)
					{
						num4 = array[num3];
					}
					else if (num3 >= num2)
					{
						return 0;
					}
					if (num3 < num2)
					{
						num5 = array2[num3];
					}
					if (num4 > num5)
					{
						return 1;
					}
					if (num5 > num4)
					{
						break;
					}
					num3++;
				}
				return -1;
			}
		}

		private static IComparer comparableComparer = new ComparebleComparer();

		private static IComparer ignoreCaseComparer = new IgnoreCaseComparer();

		private static IComparer caseComparer = new CaseComparer();

		private static IComparer byteArrayComparer = new ByteArrayComparer();

		private static Type icomparerType = typeof(IComparable);

		public static IComparer GetComparer(Type type, bool ignoreCase)
		{
			if (type == typeof(string))
			{
				if (ignoreCase)
				{
					return ignoreCaseComparer;
				}
				return caseComparer;
			}
			if (icomparerType.IsAssignableFrom(type))
			{
				return comparableComparer;
			}
			if (type == typeof(byte[]))
			{
				return byteArrayComparer;
			}
			return null;
		}
	}
}
