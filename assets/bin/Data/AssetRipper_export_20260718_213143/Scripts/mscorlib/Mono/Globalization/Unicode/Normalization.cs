using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mono.Globalization.Unicode
{
	internal class Normalization
	{
		public const int NoNfd = 1;

		public const int NoNfkd = 2;

		public const int NoNfc = 4;

		public const int MaybeNfc = 8;

		public const int NoNfkc = 16;

		public const int MaybeNfkc = 32;

		public const int FullCompositionExclusion = 64;

		public const int IsUnsafe = 128;

		private const int HangulSBase = 44032;

		private const int HangulLBase = 4352;

		private const int HangulVBase = 4449;

		private const int HangulTBase = 4519;

		private const int HangulLCount = 19;

		private const int HangulVCount = 21;

		private const int HangulTCount = 28;

		private const int HangulNCount = 588;

		private const int HangulSCount = 11172;

		private unsafe static byte* props;

		private unsafe static int* mappedChars;

		private unsafe static short* charMapIndex;

		private unsafe static short* helperIndex;

		private unsafe static ushort* mapIdxToComposite;

		private unsafe static byte* combiningClass;

		private static object forLock;

		public static readonly bool isReady;

		public static bool IsReady
		{
			get
			{
				return isReady;
			}
		}

		unsafe static Normalization()
		{
			forLock = new object();
			lock (forLock)
			{
				IntPtr intPtr;
				IntPtr intPtr2;
				IntPtr intPtr3;
				IntPtr intPtr4;
				IntPtr intPtr5;
				IntPtr intPtr6;
				load_normalization_resource(out intPtr, out intPtr2, out intPtr3, out intPtr4, out intPtr5, out intPtr6);
				props = (byte*)(void*)intPtr;
				mappedChars = (int*)(void*)intPtr2;
				charMapIndex = (short*)(void*)intPtr3;
				helperIndex = (short*)(void*)intPtr4;
				mapIdxToComposite = (ushort*)(void*)intPtr5;
				combiningClass = (byte*)(void*)intPtr6;
			}
			isReady = true;
		}

		private unsafe static uint PropValue(int cp)
		{
			return props[NormalizationTableUtil.PropIdx(cp)];
		}

		private unsafe static int CharMapIdx(int cp)
		{
			return *(short*)((byte*)charMapIndex + NormalizationTableUtil.MapIdx(cp) * 2);
		}

		private unsafe static int GetNormalizedStringLength(int ch)
		{
			int num = *(short*)((byte*)charMapIndex + NormalizationTableUtil.MapIdx(ch) * 2);
			int i;
			for (i = num; *(int*)((byte*)mappedChars + i * 4) != 0; i++)
			{
			}
			return i - num;
		}

		private unsafe static byte GetCombiningClass(int c)
		{
			return combiningClass[NormalizationTableUtil.Combining.ToIndex(c)];
		}

		private unsafe static int GetPrimaryCompositeFromMapIndex(int src)
		{
			return *(ushort*)((byte*)mapIdxToComposite + NormalizationTableUtil.Composite.ToIndex(src) * 2);
		}

		private unsafe static int GetPrimaryCompositeHelperIndex(int cp)
		{
			return *(short*)((byte*)helperIndex + NormalizationTableUtil.Helper.ToIndex(cp) * 2);
		}

		private unsafe static int GetPrimaryCompositeCharIndex(object chars, int start)
		{
			string text = chars as string;
			StringBuilder stringBuilder = chars as StringBuilder;
			char c = ((text == null) ? stringBuilder[start] : text[start]);
			int num = ((stringBuilder == null) ? text.Length : stringBuilder.Length);
			int i = GetPrimaryCompositeHelperIndex(c);
			if (i == 0)
			{
				return 0;
			}
			int j;
			for (; *(int*)((byte*)mappedChars + i * 4) == c; i += j + 1)
			{
				int num2 = 0;
				int num3 = 0;
				j = 1;
				int num4 = 1;
				while (true)
				{
					num2 = num3;
					if (*(int*)((byte*)mappedChars + (i + j) * 4) == 0)
					{
						return i;
					}
					if (start + j >= num)
					{
						return 0;
					}
					bool flag = false;
					char c2;
					do
					{
						c2 = ((text == null) ? stringBuilder[start + num4] : text[start + num4]);
						num3 = GetCombiningClass(c2);
						if (*(int*)((byte*)mappedChars + (i + j) * 4) == c2)
						{
							flag = true;
							break;
						}
					}
					while (num3 >= num2 && ++num4 + start < num && num3 != 0);
					if (!flag)
					{
						if (num2 >= num3)
						{
							break;
						}
						num4--;
						if (*(int*)((byte*)mappedChars + (i + j) * 4) != c2)
						{
							break;
						}
					}
					j++;
					num4++;
				}
				for (; *(int*)((byte*)mappedChars + j * 4) != 0; j++)
				{
				}
			}
			return 0;
		}

		private static string Compose(string source, int checkType)
		{
			StringBuilder sb = null;
			Decompose(source, ref sb, checkType);
			if (sb == null)
			{
				sb = Combine(source, 0, checkType);
			}
			else
			{
				Combine(sb, 0, checkType);
			}
			return (sb == null) ? source : sb.ToString();
		}

		private static StringBuilder Combine(string source, int start, int checkType)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (QuickCheck(source[i], checkType) != NormalizationCheck.Yes)
				{
					StringBuilder stringBuilder = new StringBuilder(source.Length + source.Length / 10);
					stringBuilder.Append(source);
					Combine(stringBuilder, i, checkType);
					return stringBuilder;
				}
			}
			return null;
		}

		private static bool CanBePrimaryComposite(int i)
		{
			if (i >= 13312 && i <= 40891)
			{
				return GetPrimaryCompositeHelperIndex(i) != 0;
			}
			return (PropValue(i) & 0x80) != 0;
		}

		private unsafe static void Combine(StringBuilder sb, int start, int checkType)
		{
			for (int i = start; i < sb.Length; i++)
			{
				if (QuickCheck(sb[i], checkType) == NormalizationCheck.Yes)
				{
					continue;
				}
				int num = i;
				while (i > 0 && GetCombiningClass(sb[i]) != 0)
				{
					i--;
				}
				int num2 = 0;
				for (; i < num; i++)
				{
					num2 = GetPrimaryCompositeMapIndex(sb, sb[i], i);
					if (num2 > 0)
					{
						break;
					}
				}
				if (num2 == 0)
				{
					i = num;
					continue;
				}
				int primaryCompositeFromMapIndex = GetPrimaryCompositeFromMapIndex(num2);
				int normalizedStringLength = GetNormalizedStringLength(primaryCompositeFromMapIndex);
				if (primaryCompositeFromMapIndex == 0 || normalizedStringLength == 0)
				{
					throw new SystemException("Internal error: should not happen. Input: " + sb);
				}
				int num3 = 0;
				sb.Insert(i++, (char)primaryCompositeFromMapIndex);
				while (num3 < normalizedStringLength)
				{
					if (sb[i] == *(int*)((byte*)mappedChars + (num2 + num3) * 4))
					{
						sb.Remove(i, 1);
						num3++;
					}
					else
					{
						i++;
					}
				}
				i = num - 1;
			}
		}

		private static int GetPrimaryCompositeMapIndex(object o, int cur, int bufferPos)
		{
			if ((PropValue(cur) & 0x40) != 0)
			{
				return 0;
			}
			if (GetCombiningClass(cur) != 0)
			{
				return 0;
			}
			return GetPrimaryCompositeCharIndex(o, bufferPos);
		}

		private static string Decompose(string source, int checkType)
		{
			StringBuilder sb = null;
			Decompose(source, ref sb, checkType);
			return (sb == null) ? source : sb.ToString();
		}

		private static void Decompose(string source, ref StringBuilder sb, int checkType)
		{
			int[] buf = null;
			int start = 0;
			for (int i = 0; i < source.Length; i++)
			{
				if (QuickCheck(source[i], checkType) == NormalizationCheck.No)
				{
					DecomposeChar(ref sb, ref buf, source, i, ref start);
				}
			}
			if (sb != null)
			{
				sb.Append(source, start, source.Length - start);
			}
			ReorderCanonical(source, ref sb, 1);
		}

		private static void ReorderCanonical(string src, ref StringBuilder sb, int start)
		{
			if (sb == null)
			{
				for (int i = 1; i < src.Length; i++)
				{
					int num = GetCombiningClass(src[i]);
					if (num != 0 && GetCombiningClass(src[i - 1]) > num)
					{
						sb = new StringBuilder(src.Length);
						sb.Append(src, 0, src.Length);
						ReorderCanonical(src, ref sb, i);
						break;
					}
				}
				return;
			}
			for (int j = start; j < sb.Length; j++)
			{
				int num2 = GetCombiningClass(sb[j]);
				if (num2 != 0 && GetCombiningClass(sb[j - 1]) > num2)
				{
					char value = sb[j - 1];
					sb[j - 1] = sb[j];
					sb[j] = value;
					j--;
				}
			}
		}

		private static void DecomposeChar(ref StringBuilder sb, ref int[] buf, string s, int i, ref int start)
		{
			if (sb == null)
			{
				sb = new StringBuilder(s.Length + 100);
			}
			sb.Append(s, start, i - start);
			if (buf == null)
			{
				buf = new int[19];
			}
			GetCanonical(s[i], buf, 0);
			for (int j = 0; buf[j] != 0; j++)
			{
				if (buf[j] < 65535)
				{
					sb.Append((char)buf[j]);
					continue;
				}
				sb.Append((char)(buf[j] >> 10));
				sb.Append((char)((buf[j] & 0xFFF) + 56320));
			}
			start = i + 1;
		}

		public static NormalizationCheck QuickCheck(char c, int type)
		{
			switch (type)
			{
			default:
			{
				uint num = PropValue(c);
				return ((num & 4) != 0) ? NormalizationCheck.No : (((num & 8) != 0) ? NormalizationCheck.Maybe : NormalizationCheck.Yes);
			}
			case 1:
				if ('가' <= c && c <= '힣')
				{
					return NormalizationCheck.No;
				}
				return ((PropValue(c) & 1) != 0) ? NormalizationCheck.No : NormalizationCheck.Yes;
			case 2:
			{
				uint num = PropValue(c);
				return ((num & 0x10) != 0) ? NormalizationCheck.No : (((num & 0x20) != 0) ? NormalizationCheck.Maybe : NormalizationCheck.Yes);
			}
			case 3:
				if ('가' <= c && c <= '힣')
				{
					return NormalizationCheck.No;
				}
				return ((PropValue(c) & 2) != 0) ? NormalizationCheck.No : NormalizationCheck.Yes;
			}
		}

		private static bool GetCanonicalHangul(int s, int[] buf, int bufIdx)
		{
			int num = s - 44032;
			if (num < 0 || num >= 11172)
			{
				return false;
			}
			int num2 = 4352 + num / 588;
			int num3 = 4449 + num % 588 / 28;
			int num4 = 4519 + num % 28;
			buf[bufIdx++] = num2;
			buf[bufIdx++] = num3;
			if (num4 != 4519)
			{
				buf[bufIdx++] = num4;
			}
			buf[bufIdx] = 0;
			return true;
		}

		public unsafe static void GetCanonical(int c, int[] buf, int bufIdx)
		{
			if (!GetCanonicalHangul(c, buf, bufIdx))
			{
				for (int i = CharMapIdx(c); *(int*)((byte*)mappedChars + i * 4) != 0; i++)
				{
					buf[bufIdx++] = *(int*)((byte*)mappedChars + i * 4);
				}
				buf[bufIdx] = 0;
			}
		}

		public static bool IsNormalized(string source, int type)
		{
			int num = -1;
			for (int i = 0; i < source.Length; i++)
			{
				int num2 = GetCombiningClass(source[i]);
				if (num2 != 0 && num2 < num)
				{
					return false;
				}
				num = num2;
				switch (QuickCheck(source[i], type))
				{
				case NormalizationCheck.No:
					return false;
				case NormalizationCheck.Maybe:
					switch (type)
					{
					case 0:
					case 2:
						return source == Normalize(source, type);
					default:
					{
						int num3 = i;
						while (i > 0 && GetCombiningClass(source[i]) != 0)
						{
							i--;
						}
						for (; i < num3; i++)
						{
							if (GetPrimaryCompositeCharIndex(source, i) != 0)
							{
								return false;
							}
						}
						break;
					}
					}
					break;
				}
			}
			return true;
		}

		public static string Normalize(string source, int type)
		{
			switch (type)
			{
			default:
				return Compose(source, type);
			case 1:
			case 3:
				return Decompose(source, type);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void load_normalization_resource(out IntPtr props, out IntPtr mappedChars, out IntPtr charMapIndex, out IntPtr helperIndex, out IntPtr mapIdxToComposite, out IntPtr combiningClass);
	}
}
