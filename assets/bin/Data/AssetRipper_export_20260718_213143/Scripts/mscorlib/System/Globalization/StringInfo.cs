using System.Collections;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public class StringInfo
	{
		private string s;

		private int length;

		public int LengthInTextElements
		{
			get
			{
				if (length < 0)
				{
					length = 0;
					int num = 0;
					while (num < s.Length)
					{
						num += GetNextTextElementLength(s, num);
						length++;
					}
				}
				return length;
			}
		}

		public string String
		{
			get
			{
				return s;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				length = -1;
				s = value;
			}
		}

		public StringInfo()
		{
		}

		public StringInfo(string value)
		{
			String = value;
		}

		[ComVisible(false)]
		public override bool Equals(object value)
		{
			StringInfo stringInfo = value as StringInfo;
			return stringInfo != null && s == stringInfo.s;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return s.GetHashCode();
		}

		public string SubstringByTextElements(int startingTextElement)
		{
			if (startingTextElement < 0 || s.Length == 0)
			{
				throw new ArgumentOutOfRangeException("startingTextElement");
			}
			int num = 0;
			for (int i = 0; i < startingTextElement; i++)
			{
				if (num >= s.Length)
				{
					throw new ArgumentOutOfRangeException("startingTextElement");
				}
				num += GetNextTextElementLength(s, num);
			}
			return s.Substring(num);
		}

		public string SubstringByTextElements(int startingTextElement, int lengthInTextElements)
		{
			if (startingTextElement < 0 || s.Length == 0)
			{
				throw new ArgumentOutOfRangeException("startingTextElement");
			}
			if (lengthInTextElements < 0)
			{
				throw new ArgumentOutOfRangeException("lengthInTextElements");
			}
			int num = 0;
			for (int i = 0; i < startingTextElement; i++)
			{
				if (num >= s.Length)
				{
					throw new ArgumentOutOfRangeException("startingTextElement");
				}
				num += GetNextTextElementLength(s, num);
			}
			int num2 = num;
			for (int j = 0; j < lengthInTextElements; j++)
			{
				if (num >= s.Length)
				{
					throw new ArgumentOutOfRangeException("lengthInTextElements");
				}
				num += GetNextTextElementLength(s, num);
			}
			return s.Substring(num2, num - num2);
		}

		public static string GetNextTextElement(string str)
		{
			if (str == null || str.Length == 0)
			{
				throw new ArgumentNullException("string is null");
			}
			return GetNextTextElement(str, 0);
		}

		public static string GetNextTextElement(string str, int index)
		{
			int nextTextElementLength = GetNextTextElementLength(str, index);
			return (nextTextElementLength == 1) ? new string(str[index], 1) : str.Substring(index, nextTextElementLength);
		}

		private static int GetNextTextElementLength(string str, int index)
		{
			if (str == null)
			{
				throw new ArgumentNullException("string is null");
			}
			if (index >= str.Length)
			{
				return 0;
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("Index is not valid");
			}
			char c = str[index];
			switch (char.GetUnicodeCategory(c))
			{
			case UnicodeCategory.Surrogate:
				if (c >= '\ud800' && c <= '\udbff')
				{
					if (index + 1 < str.Length && str[index + 1] >= '\udc00' && str[index + 1] <= '\udfff')
					{
						return 2;
					}
					return 1;
				}
				return 1;
			case UnicodeCategory.NonSpacingMark:
			case UnicodeCategory.SpacingCombiningMark:
			case UnicodeCategory.EnclosingMark:
				return 1;
			default:
			{
				int i;
				for (i = 1; index + i < str.Length; i++)
				{
					UnicodeCategory unicodeCategory = char.GetUnicodeCategory(str[index + i]);
					if (unicodeCategory != UnicodeCategory.NonSpacingMark && unicodeCategory != UnicodeCategory.SpacingCombiningMark && unicodeCategory != UnicodeCategory.EnclosingMark)
					{
						break;
					}
				}
				return i;
			}
			}
		}

		public static TextElementEnumerator GetTextElementEnumerator(string str)
		{
			if (str == null || str.Length == 0)
			{
				throw new ArgumentNullException("string is null");
			}
			return new TextElementEnumerator(str, 0);
		}

		public static TextElementEnumerator GetTextElementEnumerator(string str, int index)
		{
			if (str == null)
			{
				throw new ArgumentNullException("string is null");
			}
			if (index < 0 || index >= str.Length)
			{
				throw new ArgumentOutOfRangeException("Index is not valid");
			}
			return new TextElementEnumerator(str, index);
		}

		public static int[] ParseCombiningCharacters(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("string is null");
			}
			ArrayList arrayList = new ArrayList(str.Length);
			TextElementEnumerator textElementEnumerator = GetTextElementEnumerator(str);
			textElementEnumerator.Reset();
			while (textElementEnumerator.MoveNext())
			{
				arrayList.Add(textElementEnumerator.ElementIndex);
			}
			return (int[])arrayList.ToArray(typeof(int));
		}
	}
}
