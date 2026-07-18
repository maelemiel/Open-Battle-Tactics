using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Utils
{
	public static class Assert
	{
		[Conditional("ASSERT")]
		public static void ContainsAll<T>(ICollection<T> needles, ICollection<T> haystack)
		{
			foreach (T needle in needles)
			{
			}
		}

		[Conditional("ASSERT")]
		public static void NotNullOrEmpty(object o)
		{
		}

		[Conditional("ASSERT")]
		public static void NotNullOrEmpty(object o, string message)
		{
			string text = o as string;
			if (text == null)
			{
				ICollection collection = o as ICollection;
				if (collection == null)
				{
				}
			}
		}

		[Conditional("ASSERT")]
		public static void NotNull(object o)
		{
		}

		[Conditional("ASSERT")]
		public static void NotNull(object o, string message)
		{
		}

		[Conditional("ASSERT")]
		public static void True(bool test)
		{
		}

		[Conditional("ASSERT")]
		public static void True(bool test, string message)
		{
			if (!test)
			{
				throw new Exception(message);
			}
		}

		[Conditional("ASSERT")]
		public static void False(string message = "")
		{
		}
	}
}
