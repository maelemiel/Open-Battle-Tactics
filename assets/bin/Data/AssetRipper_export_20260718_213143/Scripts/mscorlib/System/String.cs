using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Globalization.Unicode;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class String : IConvertible, IComparable, IEnumerable, ICloneable, IComparable<string>, IEquatable<string>, IEnumerable<char>
	{
		[NonSerialized]
		private int length;

		[NonSerialized]
		private char start_char;

		public static readonly string Empty = "";

		private static readonly char[] WhiteChars = new char[27]
		{
			'\t', '\n', '\v', '\f', '\r', '\u0085', '\u1680', '\u2028', '\u2029', ' ',
			'\u00a0', '\u2000', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005', '\u2006', '\u2007', '\u2008',
			'\u2009', '\u200a', '\u200b', '\u3000', '\ufeff', '\u202f', '\u205f'
		};

		[IndexerName("Chars")]
		public unsafe char this[int index]
		{
			get
			{
				if (index < 0 || index >= length)
				{
					throw new IndexOutOfRangeException();
				}
				fixed (char* ptr = &start_char)
				{
					return *(char*)((byte*)ptr + index * 2);
				}
			}
		}

		public int Length
		{
			get
			{
				return length;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public unsafe extern String(char* value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public unsafe extern String(char* value, int startIndex, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public unsafe extern String(sbyte* value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public unsafe extern String(sbyte* value, int startIndex, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public unsafe extern String(sbyte* value, int startIndex, int length, Encoding enc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(char[] value, int startIndex, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(char[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern String(char c, int count);

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this, provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this, provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this, provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(this, provider);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this, provider);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this, provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this, provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this, provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this, provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this, provider);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this, provider);
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("type");
			}
			return Convert.ToType(this, targetType, provider, false);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this, provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this, provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this, provider);
		}

		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{
			return new CharEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new CharEnumerator(this);
		}

		public unsafe static bool Equals(string a, string b)
		{
			if ((object)a == b)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			int num = a.length;
			if (num != b.length)
			{
				return false;
			}
			fixed (char* ptr = &a.start_char)
			{
				fixed (char* ptr2 = &b.start_char)
				{
					char* ptr3 = ptr;
					char* ptr4 = ptr2;
					while (num >= 8)
					{
						if (*(int*)ptr3 != *(int*)ptr4 || ((int*)ptr3)[1] != ((int*)ptr4)[1] || ((int*)ptr3)[2] != ((int*)ptr4)[2] || ((int*)ptr3)[3] != ((int*)ptr4)[3])
						{
							return false;
						}
						ptr3 += 8;
						ptr4 += 8;
						num -= 8;
					}
					if (num >= 4)
					{
						if (*(int*)ptr3 != *(int*)ptr4 || ((int*)ptr3)[1] != ((int*)ptr4)[1])
						{
							return false;
						}
						ptr3 += 4;
						ptr4 += 4;
						num -= 4;
					}
					if (num > 1)
					{
						if (*(int*)ptr3 != *(int*)ptr4)
						{
							return false;
						}
						ptr3 += 2;
						ptr4 += 2;
						num -= 2;
					}
					return num == 0 || *ptr3 == *ptr4;
				}
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public override bool Equals(object obj)
		{
			return Equals(this, obj as string);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public bool Equals(string value)
		{
			return Equals(this, value);
		}

		public object Clone()
		{
			return this;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.String;
		}

		public unsafe void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			//IL_00a7->IL00ae: Incompatible stack types: I vs Ref
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (sourceIndex < 0)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", "Cannot be negative");
			}
			if (destinationIndex < 0)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", "Cannot be negative.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative.");
			}
			if (sourceIndex > Length - count)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", "sourceIndex + count > Length");
			}
			if (destinationIndex > destination.Length - count)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", "destinationIndex + count > destination.Length");
			}
			fixed (char* ptr = &(destination != null && destination.Length != 0 ? ref destination[0] : ref *(char*)null))
			{
				fixed (char* ptr2 = this)
				{
					CharCopy((char*)((byte*)ptr + destinationIndex * 2), (char*)((byte*)ptr2 + sourceIndex * 2), count);
				}
			}
		}

		public char[] ToCharArray()
		{
			return ToCharArray(0, length);
		}

		public unsafe char[] ToCharArray(int startIndex, int length)
		{
			//IL_0068->IL006f: Incompatible stack types: I vs Ref
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "< 0");
			}
			if (startIndex > this.length - length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Must be greater than the length of the string.");
			}
			char[] array = new char[length];
			fixed (char* dest = &(array != null && array.Length != 0 ? ref array[0] : ref *(char*)null))
			{
				fixed (char* ptr = this)
				{
					CharCopy(dest, (char*)((byte*)ptr + startIndex * 2), length);
				}
			}
			return array;
		}

		public string[] Split(params char[] separator)
		{
			return Split(separator, int.MaxValue);
		}

		public string[] Split(char[] separator, int count)
		{
			if (separator == null || separator.Length == 0)
			{
				separator = WhiteChars;
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			switch (count)
			{
			case 0:
				return new string[0];
			case 1:
				return new string[1] { this };
			default:
				return InternalSplit(separator, count, 0);
			}
		}

		[ComVisible(false)]
		[MonoDocumentationNote("code should be moved to managed")]
		public string[] Split(char[] separator, int count, StringSplitOptions options)
		{
			if (separator == null || separator.Length == 0)
			{
				return Split(WhiteChars, count, options);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
			}
			if (options != StringSplitOptions.None && options != StringSplitOptions.RemoveEmptyEntries)
			{
				throw new ArgumentException(Concat("Illegal enum value: ", options, "."));
			}
			if (count == 0)
			{
				return new string[0];
			}
			return InternalSplit(separator, count, (int)options);
		}

		[ComVisible(false)]
		public string[] Split(string[] separator, int count, StringSplitOptions options)
		{
			if (separator == null || separator.Length == 0)
			{
				return Split(WhiteChars, count, options);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be less than zero.");
			}
			if (options != StringSplitOptions.None && options != StringSplitOptions.RemoveEmptyEntries)
			{
				throw new ArgumentException(Concat("Illegal enum value: ", options, "."));
			}
			if (count == 1)
			{
				return new string[1] { this };
			}
			bool flag = (options & StringSplitOptions.RemoveEmptyEntries) == StringSplitOptions.RemoveEmptyEntries;
			if (count == 0 || (this == Empty && flag))
			{
				return new string[0];
			}
			List<string> list = new List<string>();
			int num = 0;
			int num2 = 0;
			while (num < Length)
			{
				int num3 = -1;
				int num4 = int.MaxValue;
				for (int i = 0; i < separator.Length; i++)
				{
					string text = separator[i];
					if ((object)text != null && !(text == Empty))
					{
						int num5 = IndexOf(text, num);
						if (num5 > -1 && num5 < num4)
						{
							num3 = i;
							num4 = num5;
						}
					}
				}
				if (num3 == -1)
				{
					break;
				}
				if (num4 != num || !flag)
				{
					if (list.Count == count - 1)
					{
						break;
					}
					list.Add(Substring(num, num4 - num));
				}
				num = num4 + separator[num3].Length;
				num2++;
			}
			if (num2 == 0)
			{
				return new string[1] { this };
			}
			if (flag && num2 != 0 && num == Length && list.Count == 0)
			{
				return new string[0];
			}
			if (!flag || num != Length)
			{
				list.Add(Substring(num));
			}
			return list.ToArray();
		}

		[ComVisible(false)]
		public string[] Split(char[] separator, StringSplitOptions options)
		{
			return Split(separator, int.MaxValue, options);
		}

		[ComVisible(false)]
		public string[] Split(string[] separator, StringSplitOptions options)
		{
			return Split(separator, int.MaxValue, options);
		}

		public string Substring(int startIndex)
		{
			if (startIndex == 0)
			{
				return this;
			}
			if (startIndex < 0 || startIndex > length)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			return SubstringUnchecked(startIndex, length - startIndex);
		}

		public string Substring(int startIndex, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Cannot be negative.");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative.");
			}
			if (startIndex > this.length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot exceed length of string.");
			}
			if (startIndex > this.length - length)
			{
				throw new ArgumentOutOfRangeException("length", "startIndex + length > this.length");
			}
			if (startIndex == 0 && length == this.length)
			{
				return this;
			}
			return SubstringUnchecked(startIndex, length);
		}

		internal unsafe string SubstringUnchecked(int startIndex, int length)
		{
			if (length == 0)
			{
				return Empty;
			}
			string text = InternalAllocateStr(length);
			fixed (char* dest = text)
			{
				fixed (char* ptr = this)
				{
					CharCopy(dest, (char*)((byte*)ptr + startIndex * 2), length);
				}
			}
			return text;
		}

		public string Trim()
		{
			if (length == 0)
			{
				return Empty;
			}
			int num = FindNotWhiteSpace(0, length, 1);
			if (num == length)
			{
				return Empty;
			}
			int num2 = FindNotWhiteSpace(length - 1, num, -1);
			int num3 = num2 - num + 1;
			if (num3 == length)
			{
				return this;
			}
			return SubstringUnchecked(num, num3);
		}

		public string Trim(params char[] trimChars)
		{
			if (trimChars == null || trimChars.Length == 0)
			{
				return Trim();
			}
			if (length == 0)
			{
				return Empty;
			}
			int num = FindNotInTable(0, length, 1, trimChars);
			if (num == length)
			{
				return Empty;
			}
			int num2 = FindNotInTable(length - 1, num, -1, trimChars);
			int num3 = num2 - num + 1;
			if (num3 == length)
			{
				return this;
			}
			return SubstringUnchecked(num, num3);
		}

		public string TrimStart(params char[] trimChars)
		{
			if (length == 0)
			{
				return Empty;
			}
			int num = ((trimChars != null && trimChars.Length != 0) ? FindNotInTable(0, length, 1, trimChars) : FindNotWhiteSpace(0, length, 1));
			if (num == 0)
			{
				return this;
			}
			return SubstringUnchecked(num, length - num);
		}

		public string TrimEnd(params char[] trimChars)
		{
			if (length == 0)
			{
				return Empty;
			}
			int num = ((trimChars != null && trimChars.Length != 0) ? FindNotInTable(length - 1, -1, -1, trimChars) : FindNotWhiteSpace(length - 1, -1, -1));
			num++;
			if (num == length)
			{
				return this;
			}
			return SubstringUnchecked(0, num);
		}

		private int FindNotWhiteSpace(int pos, int target, int change)
		{
			while (pos != target)
			{
				char c = this[pos];
				if (c < '\u0085')
				{
					switch (c)
					{
					default:
						return pos;
					case '\t':
					case '\n':
					case '\v':
					case '\f':
					case '\r':
					case ' ':
						break;
					}
				}
				else
				{
					switch (c)
					{
					default:
						return pos;
					case '\u0085':
					case '\u00a0':
					case '\u1680':
					case '\u2000':
					case '\u2001':
					case '\u2002':
					case '\u2003':
					case '\u2004':
					case '\u2005':
					case '\u2006':
					case '\u2007':
					case '\u2008':
					case '\u2009':
					case '\u200a':
					case '\u200b':
					case '\u2028':
					case '\u2029':
					case '\u202f':
					case '\u205f':
					case '\u3000':
					case '\ufeff':
						break;
					}
				}
				pos += change;
			}
			return pos;
		}

		private unsafe int FindNotInTable(int pos, int target, int change, char[] table)
		{
			//IL_0017->IL001f: Incompatible stack types: I vs Ref
			fixed (char* ptr = &(table != null && table.Length != 0 ? ref table[0] : ref *(char*)null))
			{
				fixed (char* ptr2 = this)
				{
					while (pos != target)
					{
						char c = *(char*)((byte*)ptr2 + pos * 2);
						int i;
						for (i = 0; i < table.Length && c != *(ushort*)((byte*)ptr + i * 2); i++)
						{
						}
						if (i == table.Length)
						{
							return pos;
						}
						pos += change;
					}
				}
			}
			return pos;
		}

		public static int Compare(string strA, string strB)
		{
			return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.None);
		}

		public static int Compare(string strA, string strB, bool ignoreCase)
		{
			return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		public static int Compare(string strA, string strB, bool ignoreCase, CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return culture.CompareInfo.Compare(strA, strB, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length)
		{
			return Compare(strA, indexA, strB, indexB, length, false, CultureInfo.CurrentCulture);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase)
		{
			return Compare(strA, indexA, strB, indexB, length, ignoreCase, CultureInfo.CurrentCulture);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, bool ignoreCase, CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (indexA > strA.Length || indexB > strB.Length || indexA < 0 || indexB < 0 || length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (length == 0)
			{
				return 0;
			}
			if ((object)strA == null)
			{
				if ((object)strB == null)
				{
					return 0;
				}
				return -1;
			}
			if ((object)strB == null)
			{
				return 1;
			}
			CompareOptions options = (ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
			int length2 = length;
			int length3 = length;
			if (length > strA.Length - indexA)
			{
				length2 = strA.Length - indexA;
			}
			if (length > strB.Length - indexB)
			{
				length3 = strB.Length - indexB;
			}
			return culture.CompareInfo.Compare(strA, indexA, length2, strB, indexB, length3, options);
		}

		public static int Compare(string strA, string strB, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return Compare(strA, strB, false, CultureInfo.CurrentCulture);
			case StringComparison.CurrentCultureIgnoreCase:
				return Compare(strA, strB, true, CultureInfo.CurrentCulture);
			case StringComparison.InvariantCulture:
				return Compare(strA, strB, false, CultureInfo.InvariantCulture);
			case StringComparison.InvariantCultureIgnoreCase:
				return Compare(strA, strB, true, CultureInfo.InvariantCulture);
			case StringComparison.Ordinal:
				return CompareOrdinalUnchecked(strA, 0, int.MaxValue, strB, 0, int.MaxValue);
			case StringComparison.OrdinalIgnoreCase:
				return CompareOrdinalCaseInsensitiveUnchecked(strA, 0, int.MaxValue, strB, 0, int.MaxValue);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", comparisonType);
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return Compare(strA, indexA, strB, indexB, length, false, CultureInfo.CurrentCulture);
			case StringComparison.CurrentCultureIgnoreCase:
				return Compare(strA, indexA, strB, indexB, length, true, CultureInfo.CurrentCulture);
			case StringComparison.InvariantCulture:
				return Compare(strA, indexA, strB, indexB, length, false, CultureInfo.InvariantCulture);
			case StringComparison.InvariantCultureIgnoreCase:
				return Compare(strA, indexA, strB, indexB, length, true, CultureInfo.InvariantCulture);
			case StringComparison.Ordinal:
				return CompareOrdinal(strA, indexA, strB, indexB, length);
			case StringComparison.OrdinalIgnoreCase:
				return CompareOrdinalCaseInsensitive(strA, indexA, strB, indexB, length);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", comparisonType);
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		public static bool Equals(string a, string b, StringComparison comparisonType)
		{
			return Compare(a, b, comparisonType) == 0;
		}

		public bool Equals(string value, StringComparison comparisonType)
		{
			return Compare(value, this, comparisonType) == 0;
		}

		public static int Compare(string strA, string strB, CultureInfo culture, CompareOptions options)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return culture.CompareInfo.Compare(strA, strB, options);
		}

		public static int Compare(string strA, int indexA, string strB, int indexB, int length, CultureInfo culture, CompareOptions options)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			int length2 = length;
			int length3 = length;
			if (length > strA.Length - indexA)
			{
				length2 = strA.Length - indexA;
			}
			if (length > strB.Length - indexB)
			{
				length3 = strB.Length - indexB;
			}
			return culture.CompareInfo.Compare(strA, indexA, length2, strB, indexB, length3, options);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is string))
			{
				throw new ArgumentException();
			}
			return Compare(this, (string)value);
		}

		public int CompareTo(string strB)
		{
			if ((object)strB == null)
			{
				return 1;
			}
			return Compare(this, strB);
		}

		public static int CompareOrdinal(string strA, string strB)
		{
			return CompareOrdinalUnchecked(strA, 0, int.MaxValue, strB, 0, int.MaxValue);
		}

		public static int CompareOrdinal(string strA, int indexA, string strB, int indexB, int length)
		{
			if (indexA > strA.Length || indexB > strB.Length || indexA < 0 || indexB < 0 || length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return CompareOrdinalUnchecked(strA, indexA, length, strB, indexB, length);
		}

		internal static int CompareOrdinalCaseInsensitive(string strA, int indexA, string strB, int indexB, int length)
		{
			if (indexA > strA.Length || indexB > strB.Length || indexA < 0 || indexB < 0 || length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return CompareOrdinalCaseInsensitiveUnchecked(strA, indexA, length, strB, indexB, length);
		}

		internal unsafe static int CompareOrdinalUnchecked(string strA, int indexA, int lenA, string strB, int indexB, int lenB)
		{
			if ((object)strA == null)
			{
				if ((object)strB == null)
				{
					return 0;
				}
				return -1;
			}
			if ((object)strB == null)
			{
				return 1;
			}
			int num = Math.Min(lenA, strA.Length - indexA);
			int num2 = Math.Min(lenB, strB.Length - indexB);
			if (num == num2 && object.ReferenceEquals(strA, strB))
			{
				return 0;
			}
			fixed (char* ptr = strA)
			{
				fixed (char* ptr2 = strB)
				{
					char* ptr3 = (char*)((byte*)ptr + indexA * 2);
					char* ptr4 = (char*)((byte*)ptr3 + Math.Min(num, num2) * 2);
					char* ptr5 = (char*)((byte*)ptr2 + indexB * 2);
					while (ptr3 < ptr4)
					{
						if (*ptr3 != *ptr5)
						{
							return *ptr3 - *ptr5;
						}
						ptr3++;
						ptr5++;
					}
					return num - num2;
				}
			}
		}

		internal unsafe static int CompareOrdinalCaseInsensitiveUnchecked(string strA, int indexA, int lenA, string strB, int indexB, int lenB)
		{
			if ((object)strA == null)
			{
				if ((object)strB == null)
				{
					return 0;
				}
				return -1;
			}
			if ((object)strB == null)
			{
				return 1;
			}
			int num = Math.Min(lenA, strA.Length - indexA);
			int num2 = Math.Min(lenB, strB.Length - indexB);
			if (num == num2 && object.ReferenceEquals(strA, strB))
			{
				return 0;
			}
			fixed (char* ptr = strA)
			{
				fixed (char* ptr2 = strB)
				{
					char* ptr3 = (char*)((byte*)ptr + indexA * 2);
					char* ptr4 = (char*)((byte*)ptr3 + Math.Min(num, num2) * 2);
					char* ptr5 = (char*)((byte*)ptr2 + indexB * 2);
					while (ptr3 < ptr4)
					{
						if (*ptr3 != *ptr5)
						{
							char c = char.ToUpperInvariant(*ptr3);
							char c2 = char.ToUpperInvariant(*ptr5);
							if (c != c2)
							{
								return c - c2;
							}
						}
						ptr3++;
						ptr5++;
					}
					return num - num2;
				}
			}
		}

		public bool EndsWith(string value)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.None);
		}

		public bool EndsWith(string value, bool ignoreCase, CultureInfo culture)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			return culture.CompareInfo.IsSuffix(this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		public int IndexOfAny(char[] anyOf)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (length == 0)
			{
				return -1;
			}
			return IndexOfAnyUnchecked(anyOf, 0, length);
		}

		public int IndexOfAny(char[] anyOf, int startIndex)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (startIndex < 0 || startIndex > length)
			{
				throw new ArgumentOutOfRangeException();
			}
			return IndexOfAnyUnchecked(anyOf, startIndex, length - startIndex);
		}

		public int IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (startIndex < 0 || startIndex > length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (count < 0 || startIndex > length - count)
			{
				throw new ArgumentOutOfRangeException("count", "Count cannot be negative, and startIndex + count must be less than length of the string.");
			}
			return IndexOfAnyUnchecked(anyOf, startIndex, count);
		}

		private unsafe int IndexOfAnyUnchecked(char[] anyOf, int startIndex, int count)
		{
			//IL_0034->IL003b: Incompatible stack types: I vs Ref
			if (anyOf.Length == 0)
			{
				return -1;
			}
			if (anyOf.Length == 1)
			{
				return IndexOfUnchecked(anyOf[0], startIndex, count);
			}
			fixed (char* ptr = &(anyOf != null && anyOf.Length != 0 ? ref anyOf[0] : ref *(char*)null))
			{
				int num = *ptr;
				int num2 = *ptr;
				char* ptr2 = (char*)((byte*)ptr + anyOf.Length * 2);
				char* ptr3 = ptr;
				while (++ptr3 != ptr2)
				{
					if (*ptr3 > num)
					{
						num = *ptr3;
					}
					else if (*ptr3 < num2)
					{
						num2 = *ptr3;
					}
				}
				fixed (char* ptr4 = &start_char)
				{
					char* ptr5 = (char*)((byte*)ptr4 + startIndex * 2);
					char* ptr6 = (char*)((byte*)ptr5 + count * 2);
					while (ptr5 != ptr6)
					{
						if (*ptr5 > num || *ptr5 < num2)
						{
							ptr5++;
							continue;
						}
						if (*ptr5 == *ptr)
						{
							return (int)(ptr5 - ptr4);
						}
						ptr3 = ptr;
						while (++ptr3 != ptr2)
						{
							if (*ptr5 == *ptr3)
							{
								return (int)(ptr5 - ptr4);
							}
						}
						ptr5++;
					}
				}
			}
			return -1;
		}

		public int IndexOf(string value, StringComparison comparisonType)
		{
			return IndexOf(value, 0, Length, comparisonType);
		}

		public int IndexOf(string value, int startIndex, StringComparison comparisonType)
		{
			return IndexOf(value, startIndex, Length - startIndex, comparisonType);
		}

		public int IndexOf(string value, int startIndex, int count, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CultureInfo.InvariantCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CultureInfo.InvariantCulture.CompareInfo.IndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return IndexOfOrdinal(value, startIndex, count, CompareOptions.Ordinal);
			case StringComparison.OrdinalIgnoreCase:
				return IndexOfOrdinal(value, startIndex, count, CompareOptions.OrdinalIgnoreCase);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", comparisonType);
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		internal int IndexOfOrdinal(string value, int startIndex, int count, CompareOptions options)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || length - startIndex < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (options == CompareOptions.Ordinal)
			{
				return IndexOfOrdinalUnchecked(value, startIndex, count);
			}
			return IndexOfOrdinalIgnoreCaseUnchecked(value, startIndex, count);
		}

		internal unsafe int IndexOfOrdinalUnchecked(string value, int startIndex, int count)
		{
			int num = value.Length;
			if (count < num)
			{
				return -1;
			}
			if (num <= 1)
			{
				if (num == 1)
				{
					return IndexOfUnchecked(value[0], startIndex, count);
				}
				return startIndex;
			}
			fixed (char* ptr = this)
			{
				fixed (char* ptr2 = value)
				{
					char* ptr3 = (char*)((byte*)ptr + startIndex * 2);
					for (char* ptr4 = (char*)((byte*)ptr3 + count * 2 - num * 2) + 1; ptr3 != ptr4; ptr3++)
					{
						if (*ptr3 != *ptr2)
						{
							continue;
						}
						int num2 = 1;
						while (true)
						{
							if (num2 < num)
							{
								if (*(ushort*)((byte*)ptr3 + num2 * 2) != *(ushort*)((byte*)ptr2 + num2 * 2))
								{
									break;
								}
								num2++;
								continue;
							}
							return (int)(ptr3 - ptr);
						}
					}
				}
			}
			return -1;
		}

		internal unsafe int IndexOfOrdinalIgnoreCaseUnchecked(string value, int startIndex, int count)
		{
			int num = value.Length;
			if (count < num)
			{
				return -1;
			}
			if (num == 0)
			{
				return startIndex;
			}
			fixed (char* ptr = this)
			{
				fixed (char* ptr2 = value)
				{
					char* ptr3 = (char*)((byte*)ptr + startIndex * 2);
					for (char* ptr4 = (char*)((byte*)ptr3 + count * 2 - num * 2) + 1; ptr3 != ptr4; ptr3++)
					{
						int num2 = 0;
						while (true)
						{
							if (num2 < num)
							{
								if (char.ToUpperInvariant(*(char*)((byte*)ptr3 + num2 * 2)) != char.ToUpperInvariant(*(char*)((byte*)ptr2 + num2 * 2)))
								{
									break;
								}
								num2++;
								continue;
							}
							return (int)(ptr3 - ptr);
						}
					}
				}
			}
			return -1;
		}

		public int LastIndexOf(string value, StringComparison comparisonType)
		{
			if (Length == 0)
			{
				return (!(value == Empty)) ? (-1) : 0;
			}
			return LastIndexOf(value, Length - 1, Length, comparisonType);
		}

		public int LastIndexOf(string value, int startIndex, StringComparison comparisonType)
		{
			return LastIndexOf(value, startIndex, startIndex + 1, comparisonType);
		}

		public int LastIndexOf(string value, int startIndex, int count, StringComparison comparisonType)
		{
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(this, value, startIndex, count, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return LastIndexOfOrdinal(value, startIndex, count, CompareOptions.Ordinal);
			case StringComparison.OrdinalIgnoreCase:
				return LastIndexOfOrdinal(value, startIndex, count, CompareOptions.OrdinalIgnoreCase);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", comparisonType);
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		internal int LastIndexOfOrdinal(string value, int startIndex, int count, CompareOptions options)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > length)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < 0 || startIndex < count - 1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (options == CompareOptions.Ordinal)
			{
				return LastIndexOfOrdinalUnchecked(value, startIndex, count);
			}
			return LastIndexOfOrdinalIgnoreCaseUnchecked(value, startIndex, count);
		}

		internal unsafe int LastIndexOfOrdinalUnchecked(string value, int startIndex, int count)
		{
			int num = value.Length;
			if (count < num)
			{
				return -1;
			}
			if (num <= 1)
			{
				if (num == 1)
				{
					return LastIndexOfUnchecked(value[0], startIndex, count);
				}
				return startIndex;
			}
			fixed (char* ptr = this)
			{
				fixed (char* ptr2 = value)
				{
					char* ptr3 = (char*)((byte*)ptr + startIndex * 2 - num * 2) + 1;
					char* ptr4 = (char*)((byte*)ptr3 - count * 2 + num * 2) - 1;
					while (ptr3 != ptr4)
					{
						if (*ptr3 == *ptr2)
						{
							int num2 = 1;
							while (true)
							{
								if (num2 < num)
								{
									if (*(ushort*)((byte*)ptr3 + num2 * 2) != *(ushort*)((byte*)ptr2 + num2 * 2))
									{
										break;
									}
									num2++;
									continue;
								}
								return (int)(ptr3 - ptr);
							}
						}
						ptr3--;
					}
				}
			}
			return -1;
		}

		internal unsafe int LastIndexOfOrdinalIgnoreCaseUnchecked(string value, int startIndex, int count)
		{
			int num = value.Length;
			if (count < num)
			{
				return -1;
			}
			if (num == 0)
			{
				return startIndex;
			}
			fixed (char* ptr = this)
			{
				fixed (char* ptr2 = value)
				{
					char* ptr3 = (char*)((byte*)ptr + startIndex * 2 - num * 2) + 1;
					char* ptr4 = (char*)((byte*)ptr3 - count * 2 + num * 2) - 1;
					while (ptr3 != ptr4)
					{
						int num2 = 0;
						while (true)
						{
							if (num2 < num)
							{
								if (char.ToUpperInvariant(*(char*)((byte*)ptr3 + num2 * 2)) != char.ToUpperInvariant(*(char*)((byte*)ptr2 + num2 * 2)))
								{
									break;
								}
								num2++;
								continue;
							}
							return (int)(ptr3 - ptr);
						}
						ptr3--;
					}
				}
			}
			return -1;
		}

		public int IndexOf(char value)
		{
			if (length == 0)
			{
				return -1;
			}
			return IndexOfUnchecked(value, 0, length);
		}

		public int IndexOf(char value, int startIndex)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0");
			}
			if (startIndex > length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "startIndex > this.length");
			}
			if ((startIndex == 0 && length == 0) || startIndex == length)
			{
				return -1;
			}
			return IndexOfUnchecked(value, startIndex, length - startIndex);
		}

		public int IndexOf(char value, int startIndex, int count)
		{
			if (startIndex < 0 || startIndex > length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative and must be< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (startIndex > length - count)
			{
				throw new ArgumentOutOfRangeException("count", "startIndex + count > this.length");
			}
			if ((startIndex == 0 && length == 0) || startIndex == length || count == 0)
			{
				return -1;
			}
			return IndexOfUnchecked(value, startIndex, count);
		}

		internal unsafe int IndexOfUnchecked(char value, int startIndex, int count)
		{
			fixed (char* ptr = &start_char)
			{
				char* ptr2 = (char*)((byte*)ptr + startIndex * 2);
				char* ptr3;
				for (ptr3 = (char*)((byte*)ptr2 + (count >> 3 << 3) * 2); ptr2 != ptr3; ptr2 += 8)
				{
					if (*ptr2 == value)
					{
						return (int)(ptr2 - ptr);
					}
					if (ptr2[1] == value)
					{
						return (int)(ptr2 - ptr + 1);
					}
					if (ptr2[2] == value)
					{
						return (int)(ptr2 - ptr + 2);
					}
					if (ptr2[3] == value)
					{
						return (int)(ptr2 - ptr + 3);
					}
					if (ptr2[4] == value)
					{
						return (int)(ptr2 - ptr + 4);
					}
					if (ptr2[5] == value)
					{
						return (int)(ptr2 - ptr + 5);
					}
					if (ptr2[6] == value)
					{
						return (int)(ptr2 - ptr + 6);
					}
					if (ptr2[7] == value)
					{
						return (int)(ptr2 - ptr + 7);
					}
				}
				for (ptr3 = (char*)((byte*)ptr3 + (count & 7) * 2); ptr2 != ptr3; ptr2++)
				{
					if (*ptr2 == value)
					{
						return (int)(ptr2 - ptr);
					}
				}
				return -1;
			}
		}

		internal unsafe int IndexOfOrdinalIgnoreCase(char value, int startIndex, int count)
		{
			if (length == 0)
			{
				return -1;
			}
			int num = startIndex + count;
			char c = char.ToUpperInvariant(value);
			fixed (char* ptr = &start_char)
			{
				for (int i = startIndex; i < num; i++)
				{
					if (char.ToUpperInvariant(*(char*)((byte*)ptr + i * 2)) == c)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int IndexOf(string value)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.length == 0)
			{
				return 0;
			}
			if (length == 0)
			{
				return -1;
			}
			return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, 0, length, CompareOptions.Ordinal);
		}

		public int IndexOf(string value, int startIndex)
		{
			return IndexOf(value, startIndex, length - startIndex);
		}

		public int IndexOf(string value, int startIndex, int count)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative, and should not exceed length of string.");
			}
			if (count < 0 || startIndex > length - count)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative, and should point to location in string.");
			}
			if (value.length == 0)
			{
				return startIndex;
			}
			if (startIndex == 0 && length == 0)
			{
				return -1;
			}
			if (count == 0)
			{
				return -1;
			}
			return CultureInfo.CurrentCulture.CompareInfo.IndexOf(this, value, startIndex, count);
		}

		public int LastIndexOfAny(char[] anyOf)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			return LastIndexOfAnyUnchecked(anyOf, length - 1, length);
		}

		public int LastIndexOfAny(char[] anyOf, int startIndex)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (startIndex < 0 || startIndex >= length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative, and should be less than length of string.");
			}
			if (length == 0)
			{
				return -1;
			}
			return LastIndexOfAnyUnchecked(anyOf, startIndex, startIndex + 1);
		}

		public int LastIndexOfAny(char[] anyOf, int startIndex, int count)
		{
			if (anyOf == null)
			{
				throw new ArgumentNullException();
			}
			if (startIndex < 0 || startIndex >= Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0 || > this.Length");
			}
			if (count < 0 || count > Length)
			{
				throw new ArgumentOutOfRangeException("count", "< 0 || > this.Length");
			}
			if (startIndex - count + 1 < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex - count + 1 < 0");
			}
			if (length == 0)
			{
				return -1;
			}
			return LastIndexOfAnyUnchecked(anyOf, startIndex, count);
		}

		private unsafe int LastIndexOfAnyUnchecked(char[] anyOf, int startIndex, int count)
		{
			//IL_0037->IL003e: Incompatible stack types: I vs Ref
			if (anyOf.Length == 1)
			{
				return LastIndexOfUnchecked(anyOf[0], startIndex, count);
			}
			fixed (char* ptr = this)
			{
				fixed (char* ptr2 = &(anyOf != null && anyOf.Length != 0 ? ref anyOf[0] : ref *(char*)null))
				{
					char* ptr3 = (char*)((byte*)ptr + startIndex * 2);
					char* ptr4 = (char*)((byte*)ptr3 - count * 2);
					char* ptr5 = (char*)((byte*)ptr2 + anyOf.Length * 2);
					while (ptr3 != ptr4)
					{
						for (char* ptr6 = ptr2; ptr6 != ptr5; ptr6++)
						{
							if (*ptr6 == *ptr3)
							{
								return (int)(ptr3 - ptr);
							}
						}
						ptr3--;
					}
					return -1;
				}
			}
		}

		public int LastIndexOf(char value)
		{
			if (length == 0)
			{
				return -1;
			}
			return LastIndexOfUnchecked(value, length - 1, length);
		}

		public int LastIndexOf(char value, int startIndex)
		{
			return LastIndexOf(value, startIndex, startIndex + 1);
		}

		public int LastIndexOf(char value, int startIndex, int count)
		{
			if (startIndex == 0 && length == 0)
			{
				return -1;
			}
			if (startIndex < 0 || startIndex >= Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0 || >= this.Length");
			}
			if (count < 0 || count > Length)
			{
				throw new ArgumentOutOfRangeException("count", "< 0 || > this.Length");
			}
			if (startIndex - count + 1 < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex - count + 1 < 0");
			}
			return LastIndexOfUnchecked(value, startIndex, count);
		}

		internal unsafe int LastIndexOfUnchecked(char value, int startIndex, int count)
		{
			fixed (char* ptr = &start_char)
			{
				char* ptr2 = (char*)((byte*)ptr + startIndex * 2);
				char* ptr3 = (char*)((byte*)ptr2 - (count >> 3 << 3) * 2);
				while (ptr2 != ptr3)
				{
					if (*ptr2 == value)
					{
						return (int)(ptr2 - ptr);
					}
					if (*(ushort*)((byte*)ptr2 + -2) == value)
					{
						return (int)(ptr2 - ptr) - 1;
					}
					if (*(ushort*)((byte*)ptr2 + -4) == value)
					{
						return (int)(ptr2 - ptr) - 2;
					}
					if (*(ushort*)((byte*)ptr2 + -6) == value)
					{
						return (int)(ptr2 - ptr) - 3;
					}
					if (*(ushort*)((byte*)ptr2 + -8) == value)
					{
						return (int)(ptr2 - ptr) - 4;
					}
					if (*(ushort*)((byte*)ptr2 + -10) == value)
					{
						return (int)(ptr2 - ptr) - 5;
					}
					if (*(ushort*)((byte*)ptr2 + -12) == value)
					{
						return (int)(ptr2 - ptr) - 6;
					}
					if (*(ushort*)((byte*)ptr2 + -14) == value)
					{
						return (int)(ptr2 - ptr) - 7;
					}
					ptr2 -= 8;
				}
				ptr3 = (char*)((byte*)ptr3 - (count & 7) * 2);
				while (ptr2 != ptr3)
				{
					if (*ptr2 == value)
					{
						return (int)(ptr2 - ptr);
					}
					ptr2--;
				}
				return -1;
			}
		}

		internal unsafe int LastIndexOfOrdinalIgnoreCase(char value, int startIndex, int count)
		{
			if (length == 0)
			{
				return -1;
			}
			int num = startIndex - count;
			char c = char.ToUpperInvariant(value);
			fixed (char* ptr = &start_char)
			{
				for (int num2 = startIndex; num2 > num; num2--)
				{
					if (char.ToUpperInvariant(*(char*)((byte*)ptr + num2 * 2)) == c)
					{
						return num2;
					}
				}
			}
			return -1;
		}

		public int LastIndexOf(string value)
		{
			if (length == 0)
			{
				return LastIndexOf(value, 0, 0);
			}
			return LastIndexOf(value, length - 1, length);
		}

		public int LastIndexOf(string value, int startIndex)
		{
			int num = startIndex;
			if (num < Length)
			{
				num++;
			}
			return LastIndexOf(value, startIndex, num);
		}

		public int LastIndexOf(string value, int startIndex, int count)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < -1 || startIndex > Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0 || > this.Length");
			}
			if (count < 0 || count > Length)
			{
				throw new ArgumentOutOfRangeException("count", "< 0 || > this.Length");
			}
			if (startIndex - count + 1 < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex - count + 1 < 0");
			}
			if (value.Length == 0)
			{
				return startIndex;
			}
			if (startIndex == 0 && length == 0)
			{
				return -1;
			}
			if (length == 0 && value.length > 0)
			{
				return -1;
			}
			if (count == 0)
			{
				return -1;
			}
			if (startIndex == Length)
			{
				startIndex--;
			}
			return CultureInfo.CurrentCulture.CompareInfo.LastIndexOf(this, value, startIndex, count);
		}

		public bool Contains(string value)
		{
			return IndexOf(value) != -1;
		}

		public static bool IsNullOrEmpty(string value)
		{
			return (object)value == null || value.Length == 0;
		}

		public string Normalize()
		{
			return Normalization.Normalize(this, 0);
		}

		public string Normalize(NormalizationForm normalizationForm)
		{
			switch (normalizationForm)
			{
			default:
				return Normalization.Normalize(this, 0);
			case NormalizationForm.FormD:
				return Normalization.Normalize(this, 1);
			case NormalizationForm.FormKC:
				return Normalization.Normalize(this, 2);
			case NormalizationForm.FormKD:
				return Normalization.Normalize(this, 3);
			}
		}

		public bool IsNormalized()
		{
			return Normalization.IsNormalized(this, 0);
		}

		public bool IsNormalized(NormalizationForm normalizationForm)
		{
			switch (normalizationForm)
			{
			default:
				return Normalization.IsNormalized(this, 0);
			case NormalizationForm.FormD:
				return Normalization.IsNormalized(this, 1);
			case NormalizationForm.FormKC:
				return Normalization.IsNormalized(this, 2);
			case NormalizationForm.FormKD:
				return Normalization.IsNormalized(this, 3);
			}
		}

		public string Remove(int startIndex)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex can not be less than zero");
			}
			if (startIndex >= length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "StartIndex must be less than the length of the string");
			}
			return Remove(startIndex, length - startIndex);
		}

		public string PadLeft(int totalWidth)
		{
			return PadLeft(totalWidth, ' ');
		}

		public unsafe string PadLeft(int totalWidth, char paddingChar)
		{
			if (totalWidth < 0)
			{
				throw new ArgumentOutOfRangeException("totalWidth", "< 0");
			}
			if (totalWidth < length)
			{
				return this;
			}
			string text = InternalAllocateStr(totalWidth);
			fixed (char* ptr = text)
			{
				fixed (char* src = this)
				{
					char* ptr2 = ptr;
					char* ptr3 = (char*)((byte*)ptr + (totalWidth - length) * 2);
					while (ptr2 != ptr3)
					{
						*(ptr2++) = paddingChar;
					}
					CharCopy(ptr3, src, length);
				}
			}
			return text;
		}

		public string PadRight(int totalWidth)
		{
			return PadRight(totalWidth, ' ');
		}

		public unsafe string PadRight(int totalWidth, char paddingChar)
		{
			if (totalWidth < 0)
			{
				throw new ArgumentOutOfRangeException("totalWidth", "< 0");
			}
			if (totalWidth < length)
			{
				return this;
			}
			if (totalWidth == 0)
			{
				return Empty;
			}
			string text = InternalAllocateStr(totalWidth);
			fixed (char* ptr = text)
			{
				fixed (char* src = this)
				{
					CharCopy(ptr, src, length);
					char* ptr2 = (char*)((byte*)ptr + length * 2);
					char* ptr3 = (char*)((byte*)ptr + totalWidth * 2);
					while (ptr2 != ptr3)
					{
						*(ptr2++) = paddingChar;
					}
				}
			}
			return text;
		}

		public bool StartsWith(string value)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.None);
		}

		[ComVisible(false)]
		public bool StartsWith(string value, StringComparison comparisonType)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CultureInfo.InvariantCulture.CompareInfo.IsPrefix(this, value, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CultureInfo.InvariantCulture.CompareInfo.IsPrefix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.Ordinal);
			case StringComparison.OrdinalIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(this, value, CompareOptions.OrdinalIgnoreCase);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", comparisonType);
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		[ComVisible(false)]
		public bool EndsWith(string value, StringComparison comparisonType)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			switch (comparisonType)
			{
			case StringComparison.CurrentCulture:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.None);
			case StringComparison.CurrentCultureIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.InvariantCulture:
				return CultureInfo.InvariantCulture.CompareInfo.IsSuffix(this, value, CompareOptions.None);
			case StringComparison.InvariantCultureIgnoreCase:
				return CultureInfo.InvariantCulture.CompareInfo.IsSuffix(this, value, CompareOptions.IgnoreCase);
			case StringComparison.Ordinal:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.Ordinal);
			case StringComparison.OrdinalIgnoreCase:
				return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(this, value, CompareOptions.OrdinalIgnoreCase);
			default:
			{
				string text = Locale.GetText("Invalid value '{0}' for StringComparison", comparisonType);
				throw new ArgumentException(text, "comparisonType");
			}
			}
		}

		public bool StartsWith(string value, bool ignoreCase, CultureInfo culture)
		{
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			return culture.CompareInfo.IsPrefix(this, value, ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		public unsafe string Replace(char oldChar, char newChar)
		{
			if (length == 0 || oldChar == newChar)
			{
				return this;
			}
			int num = IndexOfUnchecked(oldChar, 0, length);
			if (num == -1)
			{
				return this;
			}
			if (num < 4)
			{
				num = 0;
			}
			string text = InternalAllocateStr(length);
			fixed (char* ptr = text)
			{
				fixed (char* ptr2 = &start_char)
				{
					if (num != 0)
					{
						CharCopy(ptr, ptr2, num);
					}
					char* ptr3 = (char*)((byte*)ptr + length * 2);
					char* ptr4 = (char*)((byte*)ptr + num * 2);
					char* ptr5 = (char*)((byte*)ptr2 + num * 2);
					for (; ptr4 != ptr3; ptr4++)
					{
						if (*ptr5 == oldChar)
						{
							*ptr4 = newChar;
						}
						else
						{
							*ptr4 = *ptr5;
						}
						ptr5++;
					}
				}
			}
			ptr2 = null;
			return text;
		}

		public string Replace(string oldValue, string newValue)
		{
			if ((object)oldValue == null)
			{
				throw new ArgumentNullException("oldValue");
			}
			if (oldValue.Length == 0)
			{
				throw new ArgumentException("oldValue is the empty string.");
			}
			if (Length == 0)
			{
				return this;
			}
			if ((object)newValue == null)
			{
				newValue = Empty;
			}
			return ReplaceUnchecked(oldValue, newValue);
		}

		private unsafe string ReplaceUnchecked(string oldValue, string newValue)
		{
			if (oldValue.length > length)
			{
				return this;
			}
			if (oldValue.length == 1 && newValue.length == 1)
			{
				return Replace(oldValue[0], newValue[0]);
			}
			int* ptr = stackalloc int[200];
			fixed (char* ptr2 = this)
			{
				fixed (char* src = newValue)
				{
					int num = 0;
					int num2 = 0;
					while (num < length)
					{
						int num3 = IndexOfOrdinalUnchecked(oldValue, num, length - num);
						if (num3 < 0)
						{
							break;
						}
						if (num2 < 200)
						{
							*(int*)((byte*)ptr + num2++ * 4) = num3;
							num = num3 + oldValue.length;
							continue;
						}
						return ReplaceFallback(oldValue, newValue, 200);
					}
					if (num2 == 0)
					{
						return this;
					}
					int num4 = length + (newValue.length - oldValue.length) * num2;
					string text = InternalAllocateStr(num4);
					int num5 = 0;
					int num6 = 0;
					fixed (char* ptr3 = text)
					{
						for (int i = 0; i < num2; i++)
						{
							int num7 = *(int*)((byte*)ptr + i * 4) - num6;
							CharCopy((char*)((byte*)ptr3 + num5 * 2), (char*)((byte*)ptr2 + num6 * 2), num7);
							num5 += num7;
							num6 = *(int*)((byte*)ptr + i * 4) + oldValue.length;
							CharCopy((char*)((byte*)ptr3 + num5 * 2), src, newValue.length);
							num5 += newValue.length;
						}
						CharCopy((char*)((byte*)ptr3 + num5 * 2), (char*)((byte*)ptr2 + num6 * 2), length - num6);
					}
					return text;
				}
			}
		}

		private string ReplaceFallback(string oldValue, string newValue, int testedCount)
		{
			int capacity = length + (newValue.length - oldValue.length) * testedCount;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			int num = 0;
			while (num < length)
			{
				int num2 = IndexOfOrdinalUnchecked(oldValue, num, length - num);
				if (num2 < 0)
				{
					stringBuilder.Append(SubstringUnchecked(num, length - num));
					break;
				}
				stringBuilder.Append(SubstringUnchecked(num, num2 - num));
				stringBuilder.Append(newValue);
				num = num2 + oldValue.Length;
			}
			return stringBuilder.ToString();
		}

		public unsafe string Remove(int startIndex, int count)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative.");
			}
			if (startIndex > length - count)
			{
				throw new ArgumentOutOfRangeException("count", "startIndex + count > this.length");
			}
			string text = InternalAllocateStr(length - count);
			fixed (char* ptr = text)
			{
				fixed (char* ptr2 = this)
				{
					char* ptr3 = ptr;
					CharCopy(ptr3, ptr2, startIndex);
					int num = startIndex + count;
					ptr3 = (char*)((byte*)ptr3 + startIndex * 2);
					CharCopy(ptr3, (char*)((byte*)ptr2 + num * 2), length - num);
				}
			}
			return text;
		}

		public string ToLower()
		{
			return ToLower(CultureInfo.CurrentCulture);
		}

		public string ToLower(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (culture.LCID == 127)
			{
				return ToLowerInvariant();
			}
			return culture.TextInfo.ToLower(this);
		}

		public unsafe string ToLowerInvariant()
		{
			if (length == 0)
			{
				return Empty;
			}
			string text = InternalAllocateStr(length);
			fixed (char* ptr = &start_char)
			{
				fixed (char* ptr2 = text)
				{
					char* ptr3 = ptr2;
					char* ptr4 = ptr;
					for (int i = 0; i < length; i++)
					{
						*ptr3 = char.ToLowerInvariant(*ptr4);
						ptr4++;
						ptr3++;
					}
				}
			}
			return text;
		}

		public string ToUpper()
		{
			return ToUpper(CultureInfo.CurrentCulture);
		}

		public string ToUpper(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			if (culture.LCID == 127)
			{
				return ToUpperInvariant();
			}
			return culture.TextInfo.ToUpper(this);
		}

		public unsafe string ToUpperInvariant()
		{
			if (length == 0)
			{
				return Empty;
			}
			string text = InternalAllocateStr(length);
			fixed (char* ptr = &start_char)
			{
				fixed (char* ptr2 = text)
				{
					char* ptr3 = ptr2;
					char* ptr4 = ptr;
					for (int i = 0; i < length; i++)
					{
						*ptr3 = char.ToUpperInvariant(*ptr4);
						ptr4++;
						ptr3++;
					}
				}
			}
			return text;
		}

		public override string ToString()
		{
			return this;
		}

		public string ToString(IFormatProvider provider)
		{
			return this;
		}

		public static string Format(string format, object arg0)
		{
			return Format(null, format, arg0);
		}

		public static string Format(string format, object arg0, object arg1)
		{
			return Format(null, format, arg0, arg1);
		}

		public static string Format(string format, object arg0, object arg1, object arg2)
		{
			return Format(null, format, arg0, arg1, arg2);
		}

		public static string Format(string format, params object[] args)
		{
			return Format(null, format, args);
		}

		public static string Format(IFormatProvider provider, string format, params object[] args)
		{
			StringBuilder stringBuilder = FormatHelper(null, provider, format, args);
			return stringBuilder.ToString();
		}

		internal static StringBuilder FormatHelper(StringBuilder result, IFormatProvider provider, string format, params object[] args)
		{
			if ((object)format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			if (result == null)
			{
				int num = 0;
				int i;
				for (i = 0; i < args.Length; i++)
				{
					string text = args[i] as string;
					if ((object)text != null)
					{
						num += text.length;
						continue;
					}
					break;
				}
				result = ((i != args.Length) ? new StringBuilder() : new StringBuilder(num + format.length));
			}
			int ptr = 0;
			int num2 = ptr;
			while (ptr < format.length)
			{
				char c = format[ptr++];
				switch (c)
				{
				case '{':
				{
					result.Append(format, num2, ptr - num2 - 1);
					if (format[ptr] == '{')
					{
						num2 = ptr++;
						continue;
					}
					int n;
					int width;
					bool left_align;
					string format2;
					ParseFormatSpecifier(format, ref ptr, out n, out width, out left_align, out format2);
					if (n >= args.Length)
					{
						throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
					}
					object obj = args[n];
					ICustomFormatter customFormatter = null;
					if (provider != null)
					{
						customFormatter = provider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
					}
					string text2 = ((obj == null) ? Empty : ((customFormatter != null) ? customFormatter.Format(format2, obj, provider) : ((!(obj is IFormattable)) ? obj.ToString() : ((IFormattable)obj).ToString(format2, provider))));
					if (width > text2.length)
					{
						int repeatCount = width - text2.length;
						if (left_align)
						{
							result.Append(text2);
							result.Append(' ', repeatCount);
						}
						else
						{
							result.Append(' ', repeatCount);
							result.Append(text2);
						}
					}
					else
					{
						result.Append(text2);
					}
					num2 = ptr;
					continue;
				}
				case '}':
					if (ptr < format.length && format[ptr] == '}')
					{
						result.Append(format, num2, ptr - num2 - 1);
						num2 = ptr++;
						continue;
					}
					break;
				}
				if (c == '}')
				{
					throw new FormatException("Input string was not in a correct format.");
				}
			}
			if (num2 < format.length)
			{
				result.Append(format, num2, format.Length - num2);
			}
			return result;
		}

		public unsafe static string Copy(string str)
		{
			if ((object)str == null)
			{
				throw new ArgumentNullException("str");
			}
			int num = str.length;
			string text = InternalAllocateStr(num);
			if (num != 0)
			{
				fixed (char* dest = text)
				{
					fixed (char* src = str)
					{
						CharCopy(dest, src, num);
					}
				}
			}
			return text;
		}

		public static string Concat(object arg0)
		{
			if (arg0 == null)
			{
				return Empty;
			}
			return arg0.ToString();
		}

		public static string Concat(object arg0, object arg1)
		{
			return ((arg0 == null) ? null : arg0.ToString()) + ((arg1 == null) ? null : arg1.ToString());
		}

		public static string Concat(object arg0, object arg1, object arg2)
		{
			string text = ((arg0 != null) ? arg0.ToString() : Empty);
			string text2 = ((arg1 != null) ? arg1.ToString() : Empty);
			string text3 = ((arg2 != null) ? arg2.ToString() : Empty);
			return text + text2 + text3;
		}

		[CLSCompliant(false)]
		public static string Concat(object arg0, object arg1, object arg2, object arg3, __arglist)
		{
			string text = ((arg0 != null) ? arg0.ToString() : Empty);
			string text2 = ((arg1 != null) ? arg1.ToString() : Empty);
			string text3 = ((arg2 != null) ? arg2.ToString() : Empty);
			ArgIterator argIterator = new ArgIterator(__arglist);
			int remainingCount = argIterator.GetRemainingCount();
			StringBuilder stringBuilder = new StringBuilder();
			if (arg3 != null)
			{
				stringBuilder.Append(arg3.ToString());
			}
			for (int i = 0; i < remainingCount; i++)
			{
				TypedReference nextArg = argIterator.GetNextArg();
				stringBuilder.Append(TypedReference.ToObject(nextArg));
			}
			string text4 = stringBuilder.ToString();
			return text + text2 + text3 + text4;
		}

		public unsafe static string Concat(string str0, string str1)
		{
			if ((object)str0 == null || str0.Length == 0)
			{
				if ((object)str1 == null || str1.Length == 0)
				{
					return Empty;
				}
				return str1;
			}
			if ((object)str1 == null || str1.Length == 0)
			{
				return str0;
			}
			string text = InternalAllocateStr(str0.length + str1.length);
			fixed (char* dest = text)
			{
				fixed (char* src = str0)
				{
					CharCopy(dest, src, str0.length);
				}
			}
			fixed (char* ptr = text)
			{
				fixed (char* src2 = str1)
				{
					CharCopy((char*)((byte*)ptr + str0.Length * 2), src2, str1.length);
				}
			}
			return text;
		}

		public unsafe static string Concat(string str0, string str1, string str2)
		{
			if ((object)str0 == null || str0.Length == 0)
			{
				if ((object)str1 == null || str1.Length == 0)
				{
					if ((object)str2 == null || str2.Length == 0)
					{
						return Empty;
					}
					return str2;
				}
				if ((object)str2 == null || str2.Length == 0)
				{
					return str1;
				}
				str0 = Empty;
			}
			else if ((object)str1 == null || str1.Length == 0)
			{
				if ((object)str2 == null || str2.Length == 0)
				{
					return str0;
				}
				str1 = Empty;
			}
			else if ((object)str2 == null || str2.Length == 0)
			{
				str2 = Empty;
			}
			string text = InternalAllocateStr(str0.length + str1.length + str2.length);
			if (str0.Length != 0)
			{
				fixed (char* dest = text)
				{
					fixed (char* src = str0)
					{
						CharCopy(dest, src, str0.length);
					}
				}
			}
			if (str1.Length != 0)
			{
				fixed (char* ptr = text)
				{
					fixed (char* src2 = str1)
					{
						CharCopy((char*)((byte*)ptr + str0.Length * 2), src2, str1.length);
					}
				}
			}
			if (str2.Length != 0)
			{
				fixed (char* ptr2 = text)
				{
					fixed (char* src3 = str2)
					{
						CharCopy((char*)((byte*)ptr2 + str0.Length * 2 + str1.Length * 2), src3, str2.length);
					}
				}
			}
			return text;
		}

		public unsafe static string Concat(string str0, string str1, string str2, string str3)
		{
			if ((object)str0 == null && (object)str1 == null && (object)str2 == null && (object)str3 == null)
			{
				return Empty;
			}
			if ((object)str0 == null)
			{
				str0 = Empty;
			}
			if ((object)str1 == null)
			{
				str1 = Empty;
			}
			if ((object)str2 == null)
			{
				str2 = Empty;
			}
			if ((object)str3 == null)
			{
				str3 = Empty;
			}
			string text = InternalAllocateStr(str0.length + str1.length + str2.length + str3.length);
			if (str0.Length != 0)
			{
				fixed (char* dest = text)
				{
					fixed (char* src = str0)
					{
						CharCopy(dest, src, str0.length);
					}
				}
			}
			if (str1.Length != 0)
			{
				fixed (char* ptr = text)
				{
					fixed (char* src2 = str1)
					{
						CharCopy((char*)((byte*)ptr + str0.Length * 2), src2, str1.length);
					}
				}
			}
			if (str2.Length != 0)
			{
				fixed (char* ptr2 = text)
				{
					fixed (char* src3 = str2)
					{
						CharCopy((char*)((byte*)ptr2 + str0.Length * 2 + str1.Length * 2), src3, str2.length);
					}
				}
			}
			if (str3.Length != 0)
			{
				fixed (char* ptr3 = text)
				{
					fixed (char* src4 = str3)
					{
						CharCopy((char*)((byte*)ptr3 + str0.Length * 2 + str1.Length * 2 + str2.Length * 2), src4, str3.length);
					}
				}
			}
			return text;
		}

		public static string Concat(params object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			int num = args.Length;
			if (num == 0)
			{
				return Empty;
			}
			string[] array = new string[num];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (args[i] != null)
				{
					array[i] = args[i].ToString();
					num2 += array[i].length;
				}
			}
			return ConcatInternal(array, num2);
		}

		public static string Concat(params string[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = 0;
			foreach (string text in values)
			{
				if ((object)text != null)
				{
					num += text.length;
				}
			}
			return ConcatInternal(values, num);
		}

		private unsafe static string ConcatInternal(string[] values, int length)
		{
			if (length == 0)
			{
				return Empty;
			}
			string text = InternalAllocateStr(length);
			fixed (char* ptr = text)
			{
				int num = 0;
				foreach (string text2 in values)
				{
					if ((object)text2 != null)
					{
						fixed (char* src = text2)
						{
							CharCopy((char*)((byte*)ptr + num * 2), src, text2.length);
						}
						num += text2.Length;
					}
				}
			}
			return text;
		}

		public unsafe string Insert(int startIndex, string value)
		{
			if ((object)value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative and must be less than or equal to length of string.");
			}
			if (value.Length == 0)
			{
				return this;
			}
			if (Length == 0)
			{
				return value;
			}
			string text = InternalAllocateStr(length + value.length);
			fixed (char* ptr = text)
			{
				fixed (char* ptr2 = this)
				{
					fixed (char* src = value)
					{
						char* ptr3 = ptr;
						CharCopy(ptr3, ptr2, startIndex);
						ptr3 = (char*)((byte*)ptr3 + startIndex * 2);
						CharCopy(ptr3, src, value.length);
						ptr3 = (char*)((byte*)ptr3 + value.length * 2);
						CharCopy(ptr3, (char*)((byte*)ptr2 + startIndex * 2), length - startIndex);
					}
				}
			}
			return text;
		}

		public static string Intern(string str)
		{
			if ((object)str == null)
			{
				throw new ArgumentNullException("str");
			}
			return InternalIntern(str);
		}

		public static string IsInterned(string str)
		{
			if ((object)str == null)
			{
				throw new ArgumentNullException("str");
			}
			return InternalIsInterned(str);
		}

		public static string Join(string separator, string[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if ((object)separator == null)
			{
				separator = Empty;
			}
			return JoinUnchecked(separator, value, 0, value.Length);
		}

		public static string Join(string separator, string[] value, int startIndex, int count)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (startIndex > value.Length - count)
			{
				throw new ArgumentOutOfRangeException("startIndex", "startIndex + count > value.length");
			}
			if (startIndex == value.Length)
			{
				return Empty;
			}
			if ((object)separator == null)
			{
				separator = Empty;
			}
			return JoinUnchecked(separator, value, startIndex, count);
		}

		private unsafe static string JoinUnchecked(string separator, string[] value, int startIndex, int count)
		{
			int num = 0;
			int num2 = startIndex + count;
			for (int i = startIndex; i < num2; i++)
			{
				string text = value[i];
				if ((object)text != null)
				{
					num += text.length;
				}
			}
			num += separator.length * (count - 1);
			if (num <= 0)
			{
				return Empty;
			}
			string text2 = InternalAllocateStr(num);
			num2--;
			fixed (char* ptr = text2)
			{
				fixed (char* src = separator)
				{
					int num3 = 0;
					for (int j = startIndex; j < num2; j++)
					{
						string text3 = value[j];
						if ((object)text3 != null && text3.Length > 0)
						{
							fixed (char* src2 = text3)
							{
								CharCopy((char*)((byte*)ptr + num3 * 2), src2, text3.Length);
							}
							num3 += text3.Length;
						}
						if (separator.Length > 0)
						{
							CharCopy((char*)((byte*)ptr + num3 * 2), src, separator.Length);
							num3 += separator.Length;
						}
					}
					string text4 = value[num2];
					if ((object)text4 != null && text4.Length > 0)
					{
						fixed (char* src3 = text4)
						{
							CharCopy((char*)((byte*)ptr + num3 * 2), src3, text4.Length);
						}
					}
				}
			}
			return text2;
		}

		public CharEnumerator GetEnumerator()
		{
			return new CharEnumerator(this);
		}

		private static void ParseFormatSpecifier(string str, ref int ptr, out int n, out int width, out bool left_align, out string format)
		{
			try
			{
				n = ParseDecimal(str, ref ptr);
				if (n < 0)
				{
					throw new FormatException("Input string was not in a correct format.");
				}
				if (str[ptr] == ',')
				{
					ptr++;
					while (char.IsWhiteSpace(str[ptr]))
					{
						ptr++;
					}
					int num = ptr;
					format = str.Substring(num, ptr - num);
					left_align = str[ptr] == '-';
					if (left_align)
					{
						ptr++;
					}
					width = ParseDecimal(str, ref ptr);
					if (width < 0)
					{
						throw new FormatException("Input string was not in a correct format.");
					}
				}
				else
				{
					width = 0;
					left_align = false;
					format = Empty;
				}
				if (str[ptr] == ':')
				{
					int num2 = ++ptr;
					while (str[ptr] != '}')
					{
						ptr++;
					}
					format += str.Substring(num2, ptr - num2);
				}
				else
				{
					format = null;
				}
				if (str[ptr++] != '}')
				{
					throw new FormatException("Input string was not in a correct format.");
				}
			}
			catch (IndexOutOfRangeException)
			{
				throw new FormatException("Input string was not in a correct format.");
			}
		}

		private static int ParseDecimal(string str, ref int ptr)
		{
			int num = ptr;
			int num2 = 0;
			while (true)
			{
				char c = str[num];
				if (c < '0' || '9' < c)
				{
					break;
				}
				num2 = num2 * 10 + c - 48;
				num++;
			}
			if (num == ptr)
			{
				return -1;
			}
			ptr = num;
			return num2;
		}

		internal unsafe void InternalSetChar(int idx, char val)
		{
			if ((uint)idx >= (uint)Length)
			{
				throw new ArgumentOutOfRangeException("idx");
			}
			fixed (char* ptr = &start_char)
			{
				*(char*)((byte*)ptr + idx * 2) = val;
			}
		}

		internal unsafe void InternalSetLength(int newLength)
		{
			if (newLength > length)
			{
				throw new ArgumentOutOfRangeException("newLength", "newLength as to be <= length");
			}
			fixed (char* ptr = &start_char)
			{
				char* ptr2 = (char*)((byte*)ptr + newLength * 2);
				for (char* ptr3 = (char*)((byte*)ptr + length * 2); ptr2 < ptr3; ptr2++)
				{
					*ptr2 = '\0';
				}
			}
			length = newLength;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public unsafe override int GetHashCode()
		{
			fixed (char* ptr = this)
			{
				char* ptr2 = ptr;
				char* ptr3 = (char*)((byte*)ptr2 + length * 2) - 1;
				int num = 0;
				for (; ptr2 < ptr3; ptr2 += 2)
				{
					num = (num << 5) - num + *ptr2;
					num = (num << 5) - num + ptr2[1];
				}
				ptr3++;
				if (ptr2 < ptr3)
				{
					num = (num << 5) - num + *ptr2;
				}
				return num;
			}
		}

		internal unsafe int GetCaseInsensitiveHashCode()
		{
			fixed (char* ptr = this)
			{
				char* ptr2 = ptr;
				char* ptr3 = (char*)((byte*)ptr2 + length * 2) - 1;
				int num = 0;
				for (; ptr2 < ptr3; ptr2 += 2)
				{
					num = (num << 5) - num + char.ToUpperInvariant(*ptr2);
					num = (num << 5) - num + char.ToUpperInvariant(ptr2[1]);
				}
				ptr3++;
				if (ptr2 < ptr3)
				{
					num = (num << 5) - num + char.ToUpperInvariant(*ptr2);
				}
				return num;
			}
		}

		private unsafe string CreateString(sbyte* value)
		{
			if (value == null)
			{
				return Empty;
			}
			byte* ptr = (byte*)value;
			int num = 0;
			try
			{
				while (*(ptr++) != 0)
				{
					num++;
				}
			}
			catch (NullReferenceException)
			{
				throw new ArgumentOutOfRangeException("ptr", "Value does not refer to a valid string.");
			}
			catch (AccessViolationException)
			{
				throw new ArgumentOutOfRangeException("ptr", "Value does not refer to a valid string.");
			}
			return CreateString(value, 0, num, null);
		}

		private unsafe string CreateString(sbyte* value, int startIndex, int length)
		{
			return CreateString(value, startIndex, length, null);
		}

		private unsafe string CreateString(sbyte* value, int startIndex, int length, Encoding enc)
		{
			//IL_0099->IL00a0: Incompatible stack types: I vs Ref
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Non-negative number required.");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Non-negative number required.");
			}
			if (value + startIndex < value)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Value, startIndex and length do not refer to a valid string.");
			}
			bool flag;
			if (flag = enc == null)
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (length == 0)
				{
					return Empty;
				}
				enc = Encoding.Default;
			}
			byte[] array = new byte[length];
			if (length != 0)
			{
				fixed (byte* dest = &(array != null && array.Length != 0 ? ref array[0] : ref *(byte*)null))
				{
					try
					{
						memcpy(dest, (byte*)(value + startIndex), length);
					}
					catch (NullReferenceException)
					{
						throw new ArgumentOutOfRangeException("ptr", "Value, startIndex and length do not refer to a valid string.");
					}
					catch (AccessViolationException)
					{
						if (!flag)
						{
							throw;
						}
						throw new ArgumentOutOfRangeException("value", "Value, startIndex and length do not refer to a valid string.");
					}
				}
				dest = null;
			}
			return enc.GetString(array);
		}

		private unsafe string CreateString(char* value)
		{
			if (value == null)
			{
				return Empty;
			}
			char* ptr = value;
			int num = 0;
			for (; *ptr != 0; ptr++)
			{
				num++;
			}
			string text = InternalAllocateStr(num);
			if (num != 0)
			{
				fixed (char* dest = text)
				{
					CharCopy(dest, value, num);
				}
			}
			return text;
		}

		private unsafe string CreateString(char* value, int startIndex, int length)
		{
			if (length == 0)
			{
				return Empty;
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			string text = InternalAllocateStr(length);
			fixed (char* dest = text)
			{
				CharCopy(dest, (char*)((byte*)value + startIndex * 2), length);
			}
			return text;
		}

		private unsafe string CreateString(char[] val, int startIndex, int length)
		{
			//IL_008d->IL0094: Incompatible stack types: I vs Ref
			if (val == null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Cannot be negative.");
			}
			if (startIndex > val.Length - length)
			{
				throw new ArgumentOutOfRangeException("startIndex", "Cannot be negative, and should be less than length of string.");
			}
			if (length == 0)
			{
				return Empty;
			}
			string text = InternalAllocateStr(length);
			fixed (char* dest = text)
			{
				fixed (char* ptr = &(val != null && val.Length != 0 ? ref val[0] : ref *(char*)null))
				{
					CharCopy(dest, (char*)((byte*)ptr + startIndex * 2), length);
				}
			}
			ptr = null;
			return text;
		}

		private unsafe string CreateString(char[] val)
		{
			//IL_0043->IL004a: Incompatible stack types: I vs Ref
			if (val == null)
			{
				return Empty;
			}
			if (val.Length == 0)
			{
				return Empty;
			}
			string text = InternalAllocateStr(val.Length);
			fixed (char* dest = text)
			{
				fixed (char* src = &(val != null && val.Length != 0 ? ref val[0] : ref *(char*)null))
				{
					CharCopy(dest, src, val.Length);
				}
			}
			src = null;
			return text;
		}

		private unsafe string CreateString(char c, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count == 0)
			{
				return Empty;
			}
			string text = InternalAllocateStr(count);
			fixed (char* ptr = text)
			{
				char* ptr2 = ptr;
				for (char* ptr3 = (char*)((byte*)ptr2 + count * 2); ptr2 < ptr3; ptr2++)
				{
					*ptr2 = c;
				}
			}
			return text;
		}

		internal unsafe static void memset(byte* dest, int val, int len)
		{
			if (len < 8)
			{
				while (len != 0)
				{
					*dest = (byte)val;
					dest++;
					len--;
				}
				return;
			}
			if (val != 0)
			{
				val |= val << 8;
				val |= val << 16;
			}
			int num = (int)dest & 3;
			if (num != 0)
			{
				num = 4 - num;
				len -= num;
				do
				{
					*dest = (byte)val;
					dest++;
					num--;
				}
				while (num != 0);
			}
			while (len >= 16)
			{
				*(int*)dest = val;
				((int*)dest)[1] = val;
				((int*)dest)[2] = val;
				((int*)dest)[3] = val;
				dest += 16;
				len -= 16;
			}
			while (len >= 4)
			{
				*(int*)dest = val;
				dest += 4;
				len -= 4;
			}
			while (len > 0)
			{
				*dest = (byte)val;
				dest++;
				len--;
			}
		}

		private unsafe static void memcpy4(byte* dest, byte* src, int size)
		{
			while (size >= 16)
			{
				*(int*)dest = *(int*)src;
				((int*)dest)[1] = ((int*)src)[1];
				((int*)dest)[2] = ((int*)src)[2];
				((int*)dest)[3] = ((int*)src)[3];
				dest += 16;
				src += 16;
				size -= 16;
			}
			while (size >= 4)
			{
				*(int*)dest = *(int*)src;
				dest += 4;
				src += 4;
				size -= 4;
			}
			while (size > 0)
			{
				*dest = *src;
				dest++;
				src++;
				size--;
			}
		}

		private unsafe static void memcpy2(byte* dest, byte* src, int size)
		{
			while (size >= 8)
			{
				*(short*)dest = *(short*)src;
				((short*)dest)[1] = ((short*)src)[1];
				((short*)dest)[2] = ((short*)src)[2];
				((short*)dest)[3] = ((short*)src)[3];
				dest += 8;
				src += 8;
				size -= 8;
			}
			while (size >= 2)
			{
				*(short*)dest = *(short*)src;
				dest += 2;
				src += 2;
				size -= 2;
			}
			if (size > 0)
			{
				*dest = *src;
			}
		}

		private unsafe static void memcpy1(byte* dest, byte* src, int size)
		{
			while (size >= 8)
			{
				*dest = *src;
				dest[1] = src[1];
				dest[2] = src[2];
				dest[3] = src[3];
				dest[4] = src[4];
				dest[5] = src[5];
				dest[6] = src[6];
				dest[7] = src[7];
				dest += 8;
				src += 8;
				size -= 8;
			}
			while (size >= 2)
			{
				*dest = *src;
				dest[1] = src[1];
				dest += 2;
				src += 2;
				size -= 2;
			}
			if (size > 0)
			{
				*dest = *src;
			}
		}

		internal unsafe static void memcpy(byte* dest, byte* src, int size)
		{
			if ((((int)dest | (int)src) & 3) != 0)
			{
				if (((int)dest & 1) != 0 && ((int)src & 1) != 0 && size >= 1)
				{
					*dest = *src;
					dest++;
					src++;
					size--;
				}
				if (((int)dest & 2) != 0 && ((int)src & 2) != 0 && size >= 2)
				{
					*(short*)dest = *(short*)src;
					dest += 2;
					src += 2;
					size -= 2;
				}
				if ((((int)dest | (int)src) & 1) != 0)
				{
					memcpy1(dest, src, size);
					return;
				}
				if ((((int)dest | (int)src) & 2) != 0)
				{
					memcpy2(dest, src, size);
					return;
				}
			}
			memcpy4(dest, src, size);
		}

		internal unsafe static void CharCopy(char* dest, char* src, int count)
		{
			if ((((int)dest | (int)src) & 3) != 0)
			{
				if (((int)dest & 2) != 0 && ((int)src & 2) != 0 && count > 0)
				{
					*dest = *src;
					dest++;
					src++;
					count--;
				}
				if ((((int)dest | (int)src) & 2) != 0)
				{
					memcpy2((byte*)dest, (byte*)src, count * 2);
					return;
				}
			}
			memcpy4((byte*)dest, (byte*)src, count * 2);
		}

		internal unsafe static void CharCopyReverse(char* dest, char* src, int count)
		{
			dest = (char*)((byte*)dest + count * 2);
			src = (char*)((byte*)src + count * 2);
			for (int num = count; num > 0; num--)
			{
				dest--;
				src--;
				*dest = *src;
			}
		}

		internal unsafe static void CharCopy(string target, int targetIndex, string source, int sourceIndex, int count)
		{
			fixed (char* ptr = target)
			{
				fixed (char* ptr2 = source)
				{
					CharCopy((char*)((byte*)ptr + targetIndex * 2), (char*)((byte*)ptr2 + sourceIndex * 2), count);
				}
			}
		}

		internal unsafe static void CharCopy(string target, int targetIndex, char[] source, int sourceIndex, int count)
		{
			//IL_0020->IL0027: Incompatible stack types: I vs Ref
			fixed (char* ptr = target)
			{
				fixed (char* ptr2 = &(source != null && source.Length != 0 ? ref source[0] : ref *(char*)null))
				{
					CharCopy((char*)((byte*)ptr + targetIndex * 2), (char*)((byte*)ptr2 + sourceIndex * 2), count);
				}
			}
			ptr2 = null;
		}

		internal unsafe static void CharCopyReverse(string target, int targetIndex, string source, int sourceIndex, int count)
		{
			fixed (char* ptr = target)
			{
				fixed (char* ptr2 = source)
				{
					CharCopyReverse((char*)((byte*)ptr + targetIndex * 2), (char*)((byte*)ptr2 + sourceIndex * 2), count);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string[] InternalSplit(char[] separator, int count, int options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string InternalAllocateStr(int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string InternalIntern(string str);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string InternalIsInterned(string str);

		public static bool operator ==(string a, string b)
		{
			return Equals(a, b);
		}

		public static bool operator !=(string a, string b)
		{
			return !Equals(a, b);
		}
	}
}
