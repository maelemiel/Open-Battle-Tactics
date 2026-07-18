using System;
using System.Globalization;

namespace Mono.Globalization.Unicode
{
	internal class SimpleCollator
	{
		internal struct Context
		{
			public readonly CompareOptions Option;

			public unsafe readonly byte* NeverMatchFlags;

			public unsafe readonly byte* AlwaysMatchFlags;

			public unsafe byte* Buffer1;

			public unsafe byte* Buffer2;

			public int PrevCode;

			public unsafe byte* PrevSortKey;

			public readonly bool QuickCheckPossible;

			public unsafe Context(CompareOptions opt, byte* alwaysMatchFlags, byte* neverMatchFlags, byte* buffer1, byte* buffer2, byte* prev1, bool quickCheckPossible)
			{
				Option = opt;
				AlwaysMatchFlags = alwaysMatchFlags;
				NeverMatchFlags = neverMatchFlags;
				Buffer1 = buffer1;
				Buffer2 = buffer2;
				PrevSortKey = prev1;
				PrevCode = -1;
				QuickCheckPossible = quickCheckPossible;
			}

			public unsafe void ClearPrevInfo()
			{
				PrevCode = -1;
				PrevSortKey = null;
			}
		}

		private struct PreviousInfo
		{
			public int Code;

			public unsafe byte* SortKey;

			public unsafe PreviousInfo(bool dummy)
			{
				Code = -1;
				SortKey = null;
			}
		}

		private struct Escape
		{
			public string Source;

			public int Index;

			public int Start;

			public int End;

			public int Optional;
		}

		private enum ExtenderType
		{
			None = 0,
			Simple = 1,
			Voiced = 2,
			Conditional = 3,
			Buggy = 4
		}

		private const int UnsafeFlagLength = 96;

		private static bool QuickCheckDisabled = Environment.internalGetEnvironmentVariable("MONO_COLLATION_QUICK_CHECK_DISABLED") == "yes";

		private static SimpleCollator invariant = new SimpleCollator(CultureInfo.InvariantCulture);

		private readonly TextInfo textInfo;

		private readonly bool frenchSort;

		private unsafe readonly byte* cjkCatTable;

		private unsafe readonly byte* cjkLv1Table;

		private readonly CodePointIndexer cjkIndexer;

		private unsafe readonly byte* cjkLv2Table;

		private readonly CodePointIndexer cjkLv2Indexer;

		private readonly int lcid;

		private readonly Contraction[] contractions;

		private readonly Level2Map[] level2Maps;

		private readonly byte[] unsafeFlags;

		public unsafe SimpleCollator(CultureInfo culture)
		{
			lcid = culture.LCID;
			textInfo = culture.TextInfo;
			SetCJKTable(culture, ref cjkIndexer, ref cjkCatTable, ref cjkLv1Table, ref cjkLv2Indexer, ref cjkLv2Table);
			TailoringInfo tailoringInfo = null;
			CultureInfo cultureInfo = culture;
			while (cultureInfo.LCID != 127)
			{
				tailoringInfo = MSCompatUnicodeTable.GetTailoringInfo(cultureInfo.LCID);
				if (tailoringInfo != null)
				{
					break;
				}
				cultureInfo = cultureInfo.Parent;
			}
			if (tailoringInfo == null)
			{
				tailoringInfo = MSCompatUnicodeTable.GetTailoringInfo(127);
			}
			frenchSort = tailoringInfo.FrenchSort;
			MSCompatUnicodeTable.BuildTailoringTables(culture, tailoringInfo, ref contractions, ref level2Maps);
			unsafeFlags = new byte[96];
			Contraction[] array = contractions;
			foreach (Contraction contraction in array)
			{
				if (contraction.Source.Length > 1)
				{
					char[] source = contraction.Source;
					foreach (char c in source)
					{
						unsafeFlags[c / 8] |= (byte)(1 << (c & 7));
					}
				}
			}
			if (lcid == 127)
			{
				return;
			}
			Contraction[] array2 = invariant.contractions;
			foreach (Contraction contraction2 in array2)
			{
				if (contraction2.Source.Length > 1)
				{
					char[] source2 = contraction2.Source;
					foreach (char c2 in source2)
					{
						unsafeFlags[c2 / 8] |= (byte)(1 << (c2 & 7));
					}
				}
			}
		}

		private unsafe void SetCJKTable(CultureInfo culture, ref CodePointIndexer cjkIndexer, ref byte* catTable, ref byte* lv1Table, ref CodePointIndexer lv2Indexer, ref byte* lv2Table)
		{
			string name = GetNeutralCulture(culture).Name;
			MSCompatUnicodeTable.FillCJK(name, ref cjkIndexer, ref catTable, ref lv1Table, ref lv2Indexer, ref lv2Table);
		}

		private static CultureInfo GetNeutralCulture(CultureInfo info)
		{
			CultureInfo cultureInfo = info;
			while (cultureInfo.Parent != null && cultureInfo.Parent.LCID != 127)
			{
				cultureInfo = cultureInfo.Parent;
			}
			return cultureInfo;
		}

		private unsafe byte Category(int cp)
		{
			if (cp < 12288 || cjkCatTable == null)
			{
				return MSCompatUnicodeTable.Category(cp);
			}
			int num = cjkIndexer.ToIndex(cp);
			return (num >= 0) ? cjkCatTable[num] : MSCompatUnicodeTable.Category(cp);
		}

		private unsafe byte Level1(int cp)
		{
			if (cp < 12288 || cjkLv1Table == null)
			{
				return MSCompatUnicodeTable.Level1(cp);
			}
			int num = cjkIndexer.ToIndex(cp);
			return (num >= 0) ? cjkLv1Table[num] : MSCompatUnicodeTable.Level1(cp);
		}

		private unsafe byte Level2(int cp, ExtenderType ext)
		{
			switch (ext)
			{
			case ExtenderType.Buggy:
				return 5;
			case ExtenderType.Conditional:
				return 0;
			default:
			{
				if (cp < 12288 || cjkLv2Table == null)
				{
					return MSCompatUnicodeTable.Level2(cp);
				}
				int num = cjkLv2Indexer.ToIndex(cp);
				byte b = (byte)((num >= 0) ? cjkLv2Table[num] : 0);
				if (b != 0)
				{
					return b;
				}
				b = MSCompatUnicodeTable.Level2(cp);
				if (level2Maps.Length == 0)
				{
					return b;
				}
				for (int i = 0; i < level2Maps.Length; i++)
				{
					if (level2Maps[i].Source == b)
					{
						return level2Maps[i].Replace;
					}
					if (level2Maps[i].Source > b)
					{
						break;
					}
				}
				return b;
			}
			}
		}

		private static bool IsHalfKana(int cp, CompareOptions opt)
		{
			return (opt & CompareOptions.IgnoreWidth) != CompareOptions.None || MSCompatUnicodeTable.IsHalfWidthKana((char)cp);
		}

		private Contraction GetContraction(string s, int start, int end)
		{
			Contraction contraction = GetContraction(s, start, end, contractions);
			if (contraction != null || lcid == 127)
			{
				return contraction;
			}
			return GetContraction(s, start, end, invariant.contractions);
		}

		private Contraction GetContraction(string s, int start, int end, Contraction[] clist)
		{
			foreach (Contraction contraction in clist)
			{
				int num = contraction.Source[0] - s[start];
				if (num > 0)
				{
					return null;
				}
				if (num < 0)
				{
					continue;
				}
				char[] source = contraction.Source;
				if (end - start < source.Length)
				{
					continue;
				}
				bool flag = true;
				for (int j = 0; j < source.Length; j++)
				{
					if (s[start + j] != source[j])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					return contraction;
				}
			}
			return null;
		}

		private Contraction GetTailContraction(string s, int start, int end)
		{
			Contraction tailContraction = GetTailContraction(s, start, end, contractions);
			if (tailContraction != null || lcid == 127)
			{
				return tailContraction;
			}
			return GetTailContraction(s, start, end, invariant.contractions);
		}

		private Contraction GetTailContraction(string s, int start, int end, Contraction[] clist)
		{
			if (start == end || end < -1 || start >= s.Length || s.Length <= end + 1)
			{
				throw new SystemException(string.Format("MONO internal error. Failed to get TailContraction. start = {0} end = {1} string = '{2}'", start, end, s));
			}
			foreach (Contraction contraction in clist)
			{
				char[] source = contraction.Source;
				if (source.Length > start - end || source[source.Length - 1] != s[start])
				{
					continue;
				}
				bool flag = true;
				int num = 0;
				int num2 = start - source.Length + 1;
				while (num < source.Length)
				{
					if (s[num2] != source[num])
					{
						flag = false;
						break;
					}
					num++;
					num2++;
				}
				if (flag)
				{
					return contraction;
				}
			}
			return null;
		}

		private Contraction GetContraction(char c)
		{
			Contraction contraction = GetContraction(c, contractions);
			if (contraction != null || lcid == 127)
			{
				return contraction;
			}
			return GetContraction(c, invariant.contractions);
		}

		private Contraction GetContraction(char c, Contraction[] clist)
		{
			foreach (Contraction contraction in clist)
			{
				if (contraction.Source[0] > c)
				{
					return null;
				}
				if (contraction.Source[0] == c && contraction.Source.Length == 1)
				{
					return contraction;
				}
			}
			return null;
		}

		private int FilterOptions(int i, CompareOptions opt)
		{
			if ((opt & CompareOptions.IgnoreWidth) != CompareOptions.None)
			{
				int num = MSCompatUnicodeTable.ToWidthCompat(i);
				if (num != 0)
				{
					i = num;
				}
			}
			if ((opt & CompareOptions.OrdinalIgnoreCase) != CompareOptions.None)
			{
				i = textInfo.ToLower((char)i);
			}
			if ((opt & CompareOptions.IgnoreCase) != CompareOptions.None)
			{
				i = textInfo.ToLower((char)i);
			}
			if ((opt & CompareOptions.IgnoreKanaType) != CompareOptions.None)
			{
				i = MSCompatUnicodeTable.ToKanaTypeInsensitive(i);
			}
			return i;
		}

		private ExtenderType GetExtenderType(int i)
		{
			if (i == 8213)
			{
				return (lcid == 16) ? ExtenderType.Conditional : ExtenderType.None;
			}
			if (i < 12293 || i > 65392)
			{
				return ExtenderType.None;
			}
			if (i >= 65148)
			{
				switch (i)
				{
				case 65148:
				case 65149:
					return ExtenderType.Simple;
				case 65392:
					return ExtenderType.Conditional;
				case 65438:
				case 65439:
					return ExtenderType.Voiced;
				}
			}
			if (i > 12542)
			{
				return ExtenderType.None;
			}
			switch (i)
			{
			case 12293:
				return ExtenderType.Buggy;
			case 12337:
			case 12338:
			case 12445:
			case 12541:
				return ExtenderType.Simple;
			case 12446:
			case 12542:
				return ExtenderType.Voiced;
			case 12540:
				return ExtenderType.Conditional;
			default:
				return ExtenderType.None;
			}
		}

		private static byte ToDashTypeValue(ExtenderType ext, CompareOptions opt)
		{
			if ((opt & CompareOptions.IgnoreNonSpace) != CompareOptions.None)
			{
				return 3;
			}
			switch (ext)
			{
			case ExtenderType.None:
				return 3;
			case ExtenderType.Conditional:
				return 5;
			default:
				return 4;
			}
		}

		private int FilterExtender(int i, ExtenderType ext, CompareOptions opt)
		{
			if (ext == ExtenderType.Conditional && MSCompatUnicodeTable.HasSpecialWeight((char)i))
			{
				bool flag = IsHalfKana((ushort)i, opt);
				bool flag2 = !MSCompatUnicodeTable.IsHiragana((char)i);
				switch (Level1(i) & 7)
				{
				case 2:
					return flag ? 65393 : ((!flag2) ? 12354 : 12450);
				case 3:
					return flag ? 65394 : ((!flag2) ? 12356 : 12452);
				case 4:
					return flag ? 65395 : ((!flag2) ? 12358 : 12454);
				case 5:
					return flag ? 65396 : ((!flag2) ? 12360 : 12456);
				case 6:
					return flag ? 65397 : ((!flag2) ? 12362 : 12458);
				}
			}
			return i;
		}

		private static bool IsIgnorable(int i, CompareOptions opt)
		{
			return MSCompatUnicodeTable.IsIgnorable(i, (byte)(1 + (((opt & CompareOptions.IgnoreSymbols) != CompareOptions.None) ? 2 : 0) + (((opt & CompareOptions.IgnoreNonSpace) != CompareOptions.None) ? 4 : 0)));
		}

		private bool IsSafe(int i)
		{
			return i / 8 >= unsafeFlags.Length || (unsafeFlags[i / 8] & (1 << i % 8)) == 0;
		}

		public SortKey GetSortKey(string s)
		{
			return GetSortKey(s, CompareOptions.None);
		}

		public SortKey GetSortKey(string s, CompareOptions options)
		{
			return GetSortKey(s, 0, s.Length, options);
		}

		public SortKey GetSortKey(string s, int start, int length, CompareOptions options)
		{
			SortKeyBuffer sortKeyBuffer = new SortKeyBuffer(lcid);
			sortKeyBuffer.Initialize(options, lcid, s, frenchSort);
			int end = start + length;
			GetSortKey(s, start, end, sortKeyBuffer, options);
			return sortKeyBuffer.GetResultAndReset();
		}

		private unsafe void GetSortKey(string s, int start, int end, SortKeyBuffer buf, CompareOptions opt)
		{
			byte* ptr = stackalloc byte[4];
			ClearBuffer(ptr, 4);
			Context context = new Context(opt, null, null, null, null, ptr, false);
			for (int i = start; i < end; i++)
			{
				int i2 = s[i];
				ExtenderType extenderType = GetExtenderType(i2);
				if (extenderType != ExtenderType.None)
				{
					i2 = FilterExtender(context.PrevCode, extenderType, opt);
					if (i2 >= 0)
					{
						FillSortKeyRaw(i2, extenderType, buf, opt);
					}
					else if (context.PrevSortKey != null)
					{
						byte* prevSortKey = context.PrevSortKey;
						buf.AppendNormal(*prevSortKey, prevSortKey[1], (prevSortKey[2] == 1) ? Level2(i2, extenderType) : prevSortKey[2], (prevSortKey[3] == 1) ? MSCompatUnicodeTable.Level3(i2) : prevSortKey[3]);
					}
				}
				else
				{
					if (IsIgnorable(i2, opt))
					{
						continue;
					}
					i2 = FilterOptions(i2, opt);
					Contraction contraction = GetContraction(s, i, end);
					if (contraction != null)
					{
						if (contraction.Replacement != null)
						{
							GetSortKey(contraction.Replacement, 0, contraction.Replacement.Length, buf, opt);
						}
						else
						{
							byte* prevSortKey2 = context.PrevSortKey;
							for (int j = 0; j < contraction.SortKey.Length; j++)
							{
								prevSortKey2[j] = contraction.SortKey[j];
							}
							buf.AppendNormal(*prevSortKey2, prevSortKey2[1], (prevSortKey2[2] == 1) ? Level2(i2, extenderType) : prevSortKey2[2], (prevSortKey2[3] == 1) ? MSCompatUnicodeTable.Level3(i2) : prevSortKey2[3]);
							context.PrevCode = -1;
						}
						i += contraction.Source.Length - 1;
					}
					else
					{
						if (!MSCompatUnicodeTable.IsIgnorableNonSpacing(i2))
						{
							context.PrevCode = i2;
						}
						FillSortKeyRaw(i2, ExtenderType.None, buf, opt);
					}
				}
			}
		}

		private void FillSortKeyRaw(int i, ExtenderType ext, SortKeyBuffer buf, CompareOptions opt)
		{
			if (13312 <= i && i <= 19893)
			{
				int num = i - 13312;
				buf.AppendCJKExtension((byte)(16 + num / 254), (byte)(num % 254 + 2));
				return;
			}
			switch (char.GetUnicodeCategory((char)i))
			{
			case UnicodeCategory.PrivateUse:
			{
				int num2 = i - 57344;
				buf.AppendNormal((byte)(229 + num2 / 254), (byte)(num2 % 254 + 2), 0, 0);
				return;
			}
			case UnicodeCategory.Surrogate:
				FillSurrogateSortKeyRaw(i, buf);
				return;
			}
			byte lv = Level2(i, ext);
			if (MSCompatUnicodeTable.HasSpecialWeight((char)i))
			{
				byte lv2 = Level1(i);
				buf.AppendKana(Category(i), lv2, lv, MSCompatUnicodeTable.Level3(i), MSCompatUnicodeTable.IsJapaneseSmallLetter((char)i), ToDashTypeValue(ext, opt), !MSCompatUnicodeTable.IsHiragana((char)i), IsHalfKana((ushort)i, opt));
				if ((opt & CompareOptions.IgnoreNonSpace) == 0 && ext == ExtenderType.Voiced)
				{
					buf.AppendNormal(1, 1, 1, 0);
				}
			}
			else
			{
				buf.AppendNormal(Category(i), Level1(i), lv, MSCompatUnicodeTable.Level3(i));
			}
		}

		private void FillSurrogateSortKeyRaw(int i, SortKeyBuffer buf)
		{
			int num = 0;
			int num2 = 0;
			byte b = 0;
			if (i < 55360)
			{
				num = 55296;
				num2 = 65;
				b = (byte)((i != 55296) ? 63u : 62u);
			}
			else if (55360 <= i && i < 55424)
			{
				num = 55360;
				num2 = 242;
				b = 62;
			}
			else if (56192 <= i && i < 56320)
			{
				num = 56128;
				num2 = 254;
				b = 62;
			}
			else
			{
				num = 56074;
				num2 = 65;
				b = 63;
			}
			int num3 = i - num;
			buf.AppendNormal((byte)(num2 + num3 / 254), (byte)(num3 % 254 + 2), b, b);
		}

		public int Compare(string s1, string s2)
		{
			return Compare(s1, s2, CompareOptions.None);
		}

		public int Compare(string s1, string s2, CompareOptions options)
		{
			return Compare(s1, 0, s1.Length, s2, 0, s2.Length, options);
		}

		private int CompareOrdinal(string s1, int idx1, int len1, string s2, int idx2, int len2)
		{
			int num = ((len1 >= len2) ? len2 : len1);
			int num2 = idx1 + num;
			int num3 = idx2 + num;
			if (idx1 < 0 || idx2 < 0 || num2 > s1.Length || num3 > s2.Length)
			{
				throw new SystemException(string.Format("CompareInfo Internal Error: Should not happen. {0} {1} {2} {3} {4} {5}", idx1, idx2, len1, len2, s1.Length, s2.Length));
			}
			int num4 = idx1;
			int num5 = idx2;
			while (num4 < num2 && num5 < num3)
			{
				if (s1[num4] != s2[num5])
				{
					return s1[num4] - s2[num5];
				}
				num4++;
				num5++;
			}
			return (len1 != len2) ? ((len1 != num) ? 1 : (-1)) : 0;
		}

		private int CompareQuick(string s1, int idx1, int len1, string s2, int idx2, int len2, out bool sourceConsumed, out bool targetConsumed, bool immediateBreakup)
		{
			sourceConsumed = false;
			targetConsumed = false;
			int num = ((len1 >= len2) ? len2 : len1);
			int num2 = idx1 + num;
			int num3 = idx2 + num;
			if (idx1 < 0 || idx2 < 0 || num2 > s1.Length || num3 > s2.Length)
			{
				throw new SystemException(string.Format("CompareInfo Internal Error: Should not happen. {0} {1} {2} {3} {4} {5}", idx1, idx2, len1, len2, s1.Length, s2.Length));
			}
			int num4 = idx1;
			int num5 = idx2;
			while (num4 < num2 && num5 < num3)
			{
				if (s1[num4] != s2[num5])
				{
					if (immediateBreakup)
					{
						return -1;
					}
					int num6 = Category(s1[num4]) - Category(s2[num5]);
					if (num6 == 0)
					{
						num6 = Level1(s1[num4]) - Level1(s2[num5]);
					}
					if (num6 == 0)
					{
						num6 = MSCompatUnicodeTable.Level3(s1[num4]) - MSCompatUnicodeTable.Level3(s2[num5]);
					}
					if (num6 == 0)
					{
						throw new SystemException(string.Format("CompareInfo Internal Error: Should not happen. '{0}' {2} {3} '{1}' {4} {5}", s1, s2, idx1, num2, idx2, num3));
					}
					return num6;
				}
				num4++;
				num5++;
			}
			sourceConsumed = len1 <= len2;
			targetConsumed = len1 >= len2;
			return (len1 != len2) ? ((len1 != num) ? 1 : (-1)) : 0;
		}

		private int CompareOrdinalIgnoreCase(string s1, int idx1, int len1, string s2, int idx2, int len2)
		{
			int num = ((len1 >= len2) ? len2 : len1);
			int num2 = idx1 + num;
			int num3 = idx2 + num;
			if (idx1 < 0 || idx2 < 0 || num2 > s1.Length || num3 > s2.Length)
			{
				throw new SystemException(string.Format("CompareInfo Internal Error: Should not happen. {0} {1} {2} {3} {4} {5}", idx1, idx2, len1, len2, s1.Length, s2.Length));
			}
			TextInfo textInfo = invariant.textInfo;
			int num4 = idx1;
			int num5 = idx2;
			while (num4 < num2 && num5 < num3)
			{
				if (textInfo.ToLower(s1[num4]) != textInfo.ToLower(s2[num5]))
				{
					return textInfo.ToLower(s1[num4]) - textInfo.ToLower(s2[num5]);
				}
				num4++;
				num5++;
			}
			return (len1 != len2) ? ((len1 != num) ? 1 : (-1)) : 0;
		}

		public unsafe int Compare(string s1, int idx1, int len1, string s2, int idx2, int len2, CompareOptions options)
		{
			if (idx1 == idx2 && len1 == len2 && object.ReferenceEquals(s1, s2))
			{
				return 0;
			}
			switch (options)
			{
			case CompareOptions.Ordinal:
				return CompareOrdinal(s1, idx1, len1, s2, idx2, len2);
			case CompareOptions.OrdinalIgnoreCase:
				return CompareOrdinalIgnoreCase(s1, idx1, len1, s2, idx2, len2);
			default:
			{
				byte* ptr = stackalloc byte[4];
				byte* ptr2 = stackalloc byte[4];
				ClearBuffer(ptr, 4);
				ClearBuffer(ptr2, 4);
				Context ctx = new Context(options, null, null, ptr, ptr2, null, QuickCheckPossible(s1, idx1, idx1 + len1, s2, idx2, idx2 + len2));
				bool targetConsumed;
				bool sourceConsumed;
				int num = CompareInternal(s1, idx1, len1, s2, idx2, len2, out targetConsumed, out sourceConsumed, true, false, ref ctx);
				return (num != 0) ? ((num >= 0) ? 1 : (-1)) : 0;
			}
			}
		}

		private unsafe void ClearBuffer(byte* buffer, int size)
		{
			for (int i = 0; i < size; i++)
			{
				buffer[i] = 0;
			}
		}

		private bool QuickCheckPossible(string s1, int idx1, int end1, string s2, int idx2, int end2)
		{
			return false;
		}

		private unsafe int CompareInternal(string s1, int idx1, int len1, string s2, int idx2, int len2, out bool targetConsumed, out bool sourceConsumed, bool skipHeadingExtenders, bool immediateBreakup, ref Context ctx)
		{
			CompareOptions option = ctx.Option;
			int num = idx1;
			int num2 = idx2;
			int num3 = idx1 + len1;
			int num4 = idx2 + len2;
			targetConsumed = false;
			sourceConsumed = false;
			PreviousInfo previousInfo = new PreviousInfo(false);
			if (option == CompareOptions.None && ctx.QuickCheckPossible)
			{
				return CompareQuick(s1, idx1, len1, s2, idx2, len2, out sourceConsumed, out targetConsumed, immediateBreakup);
			}
			int num5 = 0;
			int num6 = 5;
			int num7 = -1;
			int num8 = -1;
			int num9 = 0;
			int num10 = 0;
			if (skipHeadingExtenders)
			{
				while (idx1 < num3 && GetExtenderType(s1[idx1]) != ExtenderType.None)
				{
					idx1++;
				}
				while (idx2 < num4 && GetExtenderType(s2[idx2]) != ExtenderType.None)
				{
					idx2++;
				}
			}
			ExtenderType extenderType = ExtenderType.None;
			ExtenderType extenderType2 = ExtenderType.None;
			int num11 = idx1;
			int num12 = idx2;
			bool flag = (option & CompareOptions.StringSort) != 0;
			bool flag2 = (option & CompareOptions.IgnoreNonSpace) != 0;
			Escape escape = default(Escape);
			Escape escape2 = default(Escape);
			while (true)
			{
				if (idx1 < num3 && IsIgnorable(s1[idx1], option))
				{
					idx1++;
					continue;
				}
				while (idx2 < num4 && IsIgnorable(s2[idx2], option))
				{
					idx2++;
				}
				if (idx1 >= num3)
				{
					if (escape.Source == null)
					{
						break;
					}
					s1 = escape.Source;
					num = escape.Start;
					idx1 = escape.Index;
					num3 = escape.End;
					num11 = escape.Optional;
					escape.Source = null;
					continue;
				}
				if (idx2 >= num4)
				{
					if (escape2.Source == null)
					{
						break;
					}
					s2 = escape2.Source;
					num2 = escape2.Start;
					idx2 = escape2.Index;
					num4 = escape2.End;
					num12 = escape2.Optional;
					escape2.Source = null;
					continue;
				}
				if (num11 < idx1 && num12 < idx2)
				{
					while (idx1 < num3 && idx2 < num4 && s1[idx1] == s2[idx2])
					{
						idx1++;
						idx2++;
					}
					if (idx1 == num3 || idx2 == num4)
					{
						continue;
					}
					int num13 = num11;
					int num14 = num12;
					num11 = idx1;
					num12 = idx2;
					idx1--;
					idx2--;
					while (idx1 > num13 && Category(s1[idx1]) == 1)
					{
						idx1--;
					}
					while (idx2 > num14 && Category(s2[idx2]) == 1)
					{
						idx2--;
					}
					while (idx1 > num13 && !IsSafe(s1[idx1]))
					{
						idx1--;
					}
					while (idx2 > num14 && !IsSafe(s2[idx2]))
					{
						idx2--;
					}
				}
				int num15 = idx1;
				int num16 = idx2;
				byte* ptr = null;
				byte* ptr2 = null;
				int num17 = FilterOptions(s1[idx1], option);
				int num18 = FilterOptions(s2[idx2], option);
				bool flag3 = false;
				bool flag4 = false;
				extenderType = GetExtenderType(num17);
				if (extenderType != ExtenderType.None)
				{
					if (ctx.PrevCode < 0)
					{
						if (ctx.PrevSortKey == null)
						{
							idx1++;
							continue;
						}
						ptr = ctx.PrevSortKey;
					}
					else
					{
						num17 = FilterExtender(ctx.PrevCode, extenderType, option);
					}
				}
				extenderType2 = GetExtenderType(num18);
				if (extenderType2 != ExtenderType.None)
				{
					if (previousInfo.Code < 0)
					{
						if (previousInfo.SortKey == null)
						{
							idx2++;
							continue;
						}
						ptr2 = previousInfo.SortKey;
					}
					else
					{
						num18 = FilterExtender(previousInfo.Code, extenderType2, option);
					}
				}
				byte b = Category(num17);
				byte b2 = Category(num18);
				if (b == 6)
				{
					if (!flag && num6 == 5)
					{
						num7 = ((escape.Source == null) ? (num15 - num) : (escape.Index - escape.Start));
						num9 = Level1(num17) << 8 + MSCompatUnicodeTable.Level3(num17);
					}
					ctx.PrevCode = num17;
					idx1++;
				}
				if (b2 == 6)
				{
					if (!flag && num6 == 5)
					{
						num8 = ((escape2.Source == null) ? (num16 - num2) : (escape2.Index - escape2.Start));
						num10 = Level1(num18) << 8 + MSCompatUnicodeTable.Level3(num18);
					}
					previousInfo.Code = num18;
					idx2++;
				}
				if (b == 6 || b2 == 6)
				{
					if (num6 == 5)
					{
						if (num9 == num10)
						{
							num7 = (num8 = -1);
							num9 = (num10 = 0);
						}
						else
						{
							num6 = 4;
						}
					}
					continue;
				}
				Contraction contraction = null;
				if (extenderType == ExtenderType.None)
				{
					contraction = GetContraction(s1, idx1, num3);
				}
				int num19 = 1;
				if (ptr != null)
				{
					num19 = 1;
				}
				else if (contraction != null)
				{
					num19 = contraction.Source.Length;
					if (contraction.SortKey != null)
					{
						ptr = ctx.Buffer1;
						for (int i = 0; i < contraction.SortKey.Length; i++)
						{
							ptr[i] = contraction.SortKey[i];
						}
						ctx.PrevCode = -1;
						ctx.PrevSortKey = ptr;
					}
					else if (escape.Source == null)
					{
						escape.Source = s1;
						escape.Start = num;
						escape.Index = num15 + contraction.Source.Length;
						escape.End = num3;
						escape.Optional = num11;
						s1 = contraction.Replacement;
						idx1 = 0;
						num = 0;
						num3 = s1.Length;
						num11 = 0;
						continue;
					}
				}
				else
				{
					ptr = ctx.Buffer1;
					*ptr = b;
					ptr[1] = Level1(num17);
					if (!flag2 && num6 > 1)
					{
						ptr[2] = Level2(num17, extenderType);
					}
					if (num6 > 2)
					{
						ptr[3] = MSCompatUnicodeTable.Level3(num17);
					}
					if (num6 > 3)
					{
						flag3 = MSCompatUnicodeTable.HasSpecialWeight((char)num17);
					}
					if (b > 1)
					{
						ctx.PrevCode = num17;
					}
				}
				Contraction contraction2 = null;
				if (extenderType2 == ExtenderType.None)
				{
					contraction2 = GetContraction(s2, idx2, num4);
				}
				if (ptr2 != null)
				{
					idx2++;
				}
				else if (contraction2 != null)
				{
					idx2 += contraction2.Source.Length;
					if (contraction2.SortKey != null)
					{
						ptr2 = ctx.Buffer2;
						for (int j = 0; j < contraction2.SortKey.Length; j++)
						{
							ptr2[j] = contraction2.SortKey[j];
						}
						previousInfo.Code = -1;
						previousInfo.SortKey = ptr2;
					}
					else if (escape2.Source == null)
					{
						escape2.Source = s2;
						escape2.Start = num2;
						escape2.Index = num16 + contraction2.Source.Length;
						escape2.End = num4;
						escape2.Optional = num12;
						s2 = contraction2.Replacement;
						idx2 = 0;
						num2 = 0;
						num4 = s2.Length;
						num12 = 0;
						continue;
					}
				}
				else
				{
					ptr2 = ctx.Buffer2;
					*ptr2 = b2;
					ptr2[1] = Level1(num18);
					if (!flag2 && num6 > 1)
					{
						ptr2[2] = Level2(num18, extenderType2);
					}
					if (num6 > 2)
					{
						ptr2[3] = MSCompatUnicodeTable.Level3(num18);
					}
					if (num6 > 3)
					{
						flag4 = MSCompatUnicodeTable.HasSpecialWeight((char)num18);
					}
					if (b2 > 1)
					{
						previousInfo.Code = num18;
					}
					idx2++;
				}
				idx1 += num19;
				if (!flag2)
				{
					while (idx1 < num3 && Category(s1[idx1]) == 1)
					{
						if (ptr[2] == 0)
						{
							ptr[2] = 2;
						}
						ptr[2] = (byte)(ptr[2] + Level2(s1[idx1], ExtenderType.None));
						idx1++;
					}
					while (idx2 < num4 && Category(s2[idx2]) == 1)
					{
						if (ptr2[2] == 0)
						{
							ptr2[2] = 2;
						}
						ptr2[2] = (byte)(ptr2[2] + Level2(s2[idx2], ExtenderType.None));
						idx2++;
					}
				}
				int num20 = *ptr - *ptr2;
				num20 = ((num20 == 0) ? (ptr[1] - ptr2[1]) : num20);
				if (num20 != 0)
				{
					return num20;
				}
				if (num6 == 1)
				{
					continue;
				}
				if (!flag2)
				{
					num20 = ptr[2] - ptr2[2];
					if (num20 != 0)
					{
						num5 = num20;
						if (immediateBreakup)
						{
							return -1;
						}
						num6 = ((!frenchSort) ? 1 : 2);
						continue;
					}
				}
				if (num6 == 2)
				{
					continue;
				}
				num20 = ptr[3] - ptr2[3];
				if (num20 != 0)
				{
					num5 = num20;
					if (immediateBreakup)
					{
						return -1;
					}
					num6 = 2;
				}
				else
				{
					if (num6 == 3)
					{
						continue;
					}
					if (flag3 != flag4)
					{
						if (immediateBreakup)
						{
							return -1;
						}
						num5 = (flag3 ? 1 : (-1));
						num6 = 3;
					}
					else
					{
						if (!flag3)
						{
							continue;
						}
						num20 = CompareFlagPair(!MSCompatUnicodeTable.IsJapaneseSmallLetter((char)num17), !MSCompatUnicodeTable.IsJapaneseSmallLetter((char)num18));
						num20 = ((num20 == 0) ? (ToDashTypeValue(extenderType, option) - ToDashTypeValue(extenderType2, option)) : num20);
						num20 = ((num20 == 0) ? CompareFlagPair(MSCompatUnicodeTable.IsHiragana((char)num17), MSCompatUnicodeTable.IsHiragana((char)num18)) : num20);
						num20 = ((num20 == 0) ? CompareFlagPair(!IsHalfKana((ushort)num17, option), !IsHalfKana((ushort)num18, option)) : num20);
						if (num20 != 0)
						{
							if (immediateBreakup)
							{
								return -1;
							}
							num5 = num20;
							num6 = 3;
						}
					}
				}
			}
			if (!flag2 && num5 != 0 && num6 > 2)
			{
				while (idx1 < num3 && idx2 < num4 && MSCompatUnicodeTable.IsIgnorableNonSpacing(s1[idx1]) && MSCompatUnicodeTable.IsIgnorableNonSpacing(s2[idx2]))
				{
					num5 = Level2(FilterOptions(s1[idx1], option), extenderType) - Level2(FilterOptions(s2[idx2], option), extenderType2);
					if (num5 != 0)
					{
						break;
					}
					idx1++;
					idx2++;
					extenderType = ExtenderType.None;
					extenderType2 = ExtenderType.None;
				}
			}
			if (num6 == 1 && num5 != 0)
			{
				while (idx1 < num3 && MSCompatUnicodeTable.IsIgnorableNonSpacing(s1[idx1]))
				{
					idx1++;
				}
				while (idx2 < num4 && MSCompatUnicodeTable.IsIgnorableNonSpacing(s2[idx2]))
				{
					idx2++;
				}
			}
			if (num5 == 0)
			{
				if (num7 < 0 && num8 >= 0)
				{
					num5 = -1;
				}
				else if (num8 < 0 && num7 >= 0)
				{
					num5 = 1;
				}
				else
				{
					num5 = num7 - num8;
					if (num5 == 0)
					{
						num5 = num9 - num10;
					}
				}
			}
			if (num5 == 0)
			{
				if (idx2 == num4)
				{
					targetConsumed = true;
				}
				if (idx1 == num3)
				{
					sourceConsumed = true;
				}
			}
			return (idx1 != num3) ? 1 : ((idx2 != num4) ? (-1) : num5);
		}

		private int CompareFlagPair(bool b1, bool b2)
		{
			return (b1 != b2) ? (b1 ? 1 : (-1)) : 0;
		}

		public bool IsPrefix(string src, string target, CompareOptions opt)
		{
			return IsPrefix(src, target, 0, src.Length, opt);
		}

		public unsafe bool IsPrefix(string s, string target, int start, int length, CompareOptions opt)
		{
			if (target.Length == 0)
			{
				return true;
			}
			byte* ptr = stackalloc byte[4];
			byte* ptr2 = stackalloc byte[4];
			ClearBuffer(ptr, 4);
			ClearBuffer(ptr2, 4);
			Context ctx = new Context(opt, null, null, ptr, ptr2, null, QuickCheckPossible(s, start, start + length, target, 0, target.Length));
			return IsPrefix(s, target, start, length, true, ref ctx);
		}

		private bool IsPrefix(string s, string target, int start, int length, bool skipHeadingExtenders, ref Context ctx)
		{
			bool targetConsumed;
			bool sourceConsumed;
			CompareInternal(s, start, length, target, 0, target.Length, out targetConsumed, out sourceConsumed, skipHeadingExtenders, true, ref ctx);
			return targetConsumed;
		}

		public bool IsSuffix(string src, string target, CompareOptions opt)
		{
			return IsSuffix(src, target, src.Length - 1, src.Length, opt);
		}

		public bool IsSuffix(string s, string target, int start, int length, CompareOptions opt)
		{
			if (target.Length == 0)
			{
				return true;
			}
			int num = LastIndexOf(s, target, start, length, opt);
			return num >= 0 && Compare(s, num, s.Length - num, target, 0, target.Length, opt) == 0;
		}

		public int IndexOf(string s, string target, CompareOptions opt)
		{
			return IndexOf(s, target, 0, s.Length, opt);
		}

		private int QuickIndexOf(string s, string target, int start, int length, out bool testWasUnable)
		{
			int num = -1;
			int num2 = -1;
			testWasUnable = true;
			if (target.Length == 0)
			{
				return 0;
			}
			if (target.Length > length)
			{
				return -1;
			}
			testWasUnable = false;
			int num3 = start + length - target.Length + 1;
			for (int i = start; i < num3; i++)
			{
				bool flag = false;
				for (int j = 0; j < target.Length; j++)
				{
					if (num2 < j)
					{
						if (target[j] >= '\u0080')
						{
							testWasUnable = true;
							return -1;
						}
						num2 = j;
					}
					if (num < i + j)
					{
						if (s[i + j] >= '\u0080')
						{
							testWasUnable = true;
							return -1;
						}
						num = i + j;
					}
					if (s[i + j] != target[j])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return i;
				}
			}
			return -1;
		}

		public unsafe int IndexOf(string s, string target, int start, int length, CompareOptions opt)
		{
			switch (opt)
			{
			case CompareOptions.Ordinal:
				return IndexOfOrdinal(s, target, start, length);
			case CompareOptions.OrdinalIgnoreCase:
				return IndexOfOrdinalIgnoreCase(s, target, start, length);
			case CompareOptions.None:
			{
				bool testWasUnable;
				int result = QuickIndexOf(s, target, start, length, out testWasUnable);
				if (!testWasUnable)
				{
					return result;
				}
				break;
			}
			}
			byte* ptr = stackalloc byte[16];
			byte* ptr2 = stackalloc byte[16];
			byte* ptr3 = stackalloc byte[4];
			byte* ptr4 = stackalloc byte[4];
			byte* ptr5 = stackalloc byte[4];
			ClearBuffer(ptr, 16);
			ClearBuffer(ptr2, 16);
			ClearBuffer(ptr3, 4);
			ClearBuffer(ptr4, 4);
			ClearBuffer(ptr5, 4);
			Context ctx = new Context(opt, ptr, ptr2, ptr4, ptr5, null, false);
			return IndexOf(s, target, start, length, ptr3, ref ctx);
		}

		private int IndexOfOrdinal(string s, string target, int start, int length)
		{
			if (target.Length == 0)
			{
				return 0;
			}
			if (target.Length > length)
			{
				return -1;
			}
			int num = start + length - target.Length + 1;
			for (int i = start; i < num; i++)
			{
				bool flag = false;
				for (int j = 0; j < target.Length; j++)
				{
					if (s[i + j] != target[j])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return i;
				}
			}
			return -1;
		}

		private int IndexOfOrdinalIgnoreCase(string s, string target, int start, int length)
		{
			if (target.Length == 0)
			{
				return 0;
			}
			if (target.Length > length)
			{
				return -1;
			}
			int num = start + length - target.Length + 1;
			for (int i = start; i < num; i++)
			{
				bool flag = false;
				for (int j = 0; j < target.Length; j++)
				{
					if (textInfo.ToLower(s[i + j]) != textInfo.ToLower(target[j]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(string s, char target, CompareOptions opt)
		{
			return IndexOf(s, target, 0, s.Length, opt);
		}

		public unsafe int IndexOf(string s, char target, int start, int length, CompareOptions opt)
		{
			switch (opt)
			{
			case CompareOptions.Ordinal:
				return IndexOfOrdinal(s, target, start, length);
			case CompareOptions.OrdinalIgnoreCase:
				return IndexOfOrdinalIgnoreCase(s, target, start, length);
			default:
			{
				byte* ptr = stackalloc byte[16];
				byte* ptr2 = stackalloc byte[16];
				byte* ptr3 = stackalloc byte[4];
				byte* ptr4 = stackalloc byte[4];
				byte* ptr5 = stackalloc byte[4];
				ClearBuffer(ptr, 16);
				ClearBuffer(ptr2, 16);
				ClearBuffer(ptr3, 4);
				ClearBuffer(ptr4, 4);
				ClearBuffer(ptr5, 4);
				Context ctx = new Context(opt, ptr, ptr2, ptr4, ptr5, null, false);
				Contraction contraction = GetContraction(target);
				if (contraction != null)
				{
					if (contraction.Replacement != null)
					{
						return IndexOf(s, contraction.Replacement, start, length, ptr3, ref ctx);
					}
					for (int i = 0; i < contraction.SortKey.Length; i++)
					{
						ptr5[i] = contraction.SortKey[i];
					}
					return IndexOfSortKey(s, start, length, ptr5, '\0', -1, true, ref ctx);
				}
				int num = FilterOptions(target, opt);
				*ptr3 = Category(num);
				ptr3[1] = Level1(num);
				if ((opt & CompareOptions.IgnoreNonSpace) == 0)
				{
					ptr3[2] = Level2(num, ExtenderType.None);
				}
				ptr3[3] = MSCompatUnicodeTable.Level3(num);
				return IndexOfSortKey(s, start, length, ptr3, target, num, !MSCompatUnicodeTable.HasSpecialWeight((char)num), ref ctx);
			}
			}
		}

		private int IndexOfOrdinal(string s, char target, int start, int length)
		{
			int num = start + length;
			for (int i = start; i < num; i++)
			{
				if (s[i] == target)
				{
					return i;
				}
			}
			return -1;
		}

		private int IndexOfOrdinalIgnoreCase(string s, char target, int start, int length)
		{
			int num = start + length;
			target = textInfo.ToLower(target);
			for (int i = start; i < num; i++)
			{
				if (textInfo.ToLower(s[i]) == target)
				{
					return i;
				}
			}
			return -1;
		}

		private unsafe int IndexOfSortKey(string s, int start, int length, byte* sortkey, char target, int ti, bool noLv4, ref Context ctx)
		{
			int num = start + length;
			int idx = start;
			while (idx < num)
			{
				int result = idx;
				if (MatchesForward(s, ref idx, num, ti, sortkey, noLv4, ref ctx))
				{
					return result;
				}
			}
			return -1;
		}

		private unsafe int IndexOf(string s, string target, int start, int length, byte* targetSortKey, ref Context ctx)
		{
			CompareOptions option = ctx.Option;
			int i;
			for (i = 0; i < target.Length && IsIgnorable(target[i], option); i++)
			{
			}
			if (i == target.Length)
			{
				return start;
			}
			Contraction contraction = GetContraction(target, i, target.Length - i);
			string text = ((contraction == null) ? null : contraction.Replacement);
			byte* ptr = ((text != null) ? null : targetSortKey);
			bool noLv = true;
			char target2 = '\0';
			int num = -1;
			if (contraction != null && ptr != null)
			{
				for (int j = 0; j < contraction.SortKey.Length; j++)
				{
					ptr[j] = contraction.SortKey[j];
				}
			}
			else if (ptr != null)
			{
				target2 = target[i];
				num = FilterOptions(target[i], option);
				*ptr = Category(num);
				ptr[1] = Level1(num);
				if ((option & CompareOptions.IgnoreNonSpace) == 0)
				{
					ptr[2] = Level2(num, ExtenderType.None);
				}
				ptr[3] = MSCompatUnicodeTable.Level3(num);
				noLv = !MSCompatUnicodeTable.HasSpecialWeight((char)num);
			}
			if (ptr != null)
			{
				for (i++; i < target.Length && Category(target[i]) == 1; i++)
				{
					if (ptr[2] == 0)
					{
						ptr[2] = 2;
					}
					ptr[2] = (byte)(ptr[2] + Level2(target[i], ExtenderType.None));
				}
			}
			do
			{
				int num2 = 0;
				num2 = ((text == null) ? IndexOfSortKey(s, start, length, ptr, target2, num, noLv, ref ctx) : IndexOf(s, text, start, length, targetSortKey, ref ctx));
				if (num2 < 0)
				{
					return -1;
				}
				length -= num2 - start;
				start = num2;
				if (IsPrefix(s, target, start, length, false, ref ctx))
				{
					return num2;
				}
				Contraction contraction2 = GetContraction(s, start, length);
				if (contraction2 != null)
				{
					start += contraction2.Source.Length;
					length -= contraction2.Source.Length;
				}
				else
				{
					start++;
					length--;
				}
			}
			while (length > 0);
			return -1;
		}

		public int LastIndexOf(string s, string target, CompareOptions opt)
		{
			return LastIndexOf(s, target, s.Length - 1, s.Length, opt);
		}

		public unsafe int LastIndexOf(string s, string target, int start, int length, CompareOptions opt)
		{
			switch (opt)
			{
			case CompareOptions.Ordinal:
				return LastIndexOfOrdinal(s, target, start, length);
			case CompareOptions.OrdinalIgnoreCase:
				return LastIndexOfOrdinalIgnoreCase(s, target, start, length);
			default:
			{
				byte* ptr = stackalloc byte[16];
				byte* ptr2 = stackalloc byte[16];
				byte* ptr3 = stackalloc byte[4];
				byte* ptr4 = stackalloc byte[4];
				byte* ptr5 = stackalloc byte[4];
				ClearBuffer(ptr, 16);
				ClearBuffer(ptr2, 16);
				ClearBuffer(ptr3, 4);
				ClearBuffer(ptr4, 4);
				ClearBuffer(ptr5, 4);
				Context ctx = new Context(opt, ptr, ptr2, ptr4, ptr5, null, false);
				return LastIndexOf(s, target, start, length, ptr3, ref ctx);
			}
			}
		}

		private int LastIndexOfOrdinal(string s, string target, int start, int length)
		{
			if (target.Length == 0)
			{
				return 0;
			}
			if (s.Length < target.Length || target.Length > length)
			{
				return -1;
			}
			int num = start - length + target.Length - 1;
			char c = target[target.Length - 1];
			int num2 = start;
			while (num2 > num)
			{
				if (s[num2] != c)
				{
					num2--;
					continue;
				}
				int num3 = num2 - target.Length + 1;
				num2--;
				bool flag = false;
				for (int num4 = target.Length - 2; num4 >= 0; num4--)
				{
					if (s[num3 + num4] != target[num4])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				return num3;
			}
			return -1;
		}

		private int LastIndexOfOrdinalIgnoreCase(string s, string target, int start, int length)
		{
			if (target.Length == 0)
			{
				return 0;
			}
			if (s.Length < length || target.Length > length)
			{
				return -1;
			}
			int num = start - length + target.Length - 1;
			char c = textInfo.ToLower(target[target.Length - 1]);
			int num2 = start;
			while (num2 > num)
			{
				if (textInfo.ToLower(s[num2]) != c)
				{
					num2--;
					continue;
				}
				int num3 = num2 - target.Length + 1;
				num2--;
				bool flag = false;
				for (int num4 = target.Length - 2; num4 >= 0; num4--)
				{
					if (textInfo.ToLower(s[num3 + num4]) != textInfo.ToLower(target[num4]))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				return num3;
			}
			return -1;
		}

		public int LastIndexOf(string s, char target, CompareOptions opt)
		{
			return LastIndexOf(s, target, s.Length - 1, s.Length, opt);
		}

		public unsafe int LastIndexOf(string s, char target, int start, int length, CompareOptions opt)
		{
			switch (opt)
			{
			case CompareOptions.Ordinal:
				return LastIndexOfOrdinal(s, target, start, length);
			case CompareOptions.OrdinalIgnoreCase:
				return LastIndexOfOrdinalIgnoreCase(s, target, start, length);
			default:
			{
				byte* ptr = stackalloc byte[16];
				byte* ptr2 = stackalloc byte[16];
				byte* ptr3 = stackalloc byte[4];
				byte* ptr4 = stackalloc byte[4];
				byte* ptr5 = stackalloc byte[4];
				ClearBuffer(ptr, 16);
				ClearBuffer(ptr2, 16);
				ClearBuffer(ptr3, 4);
				ClearBuffer(ptr4, 4);
				ClearBuffer(ptr5, 4);
				Context ctx = new Context(opt, ptr, ptr2, ptr4, ptr5, null, false);
				Contraction contraction = GetContraction(target);
				if (contraction != null)
				{
					if (contraction.Replacement != null)
					{
						return LastIndexOf(s, contraction.Replacement, start, length, ptr3, ref ctx);
					}
					for (int i = 0; i < contraction.SortKey.Length; i++)
					{
						ptr5[i] = contraction.SortKey[i];
					}
					return LastIndexOfSortKey(s, start, start, length, ptr5, -1, true, ref ctx);
				}
				int num = FilterOptions(target, opt);
				*ptr3 = Category(num);
				ptr3[1] = Level1(num);
				if ((opt & CompareOptions.IgnoreNonSpace) == 0)
				{
					ptr3[2] = Level2(num, ExtenderType.None);
				}
				ptr3[3] = MSCompatUnicodeTable.Level3(num);
				return LastIndexOfSortKey(s, start, start, length, ptr3, num, !MSCompatUnicodeTable.HasSpecialWeight((char)num), ref ctx);
			}
			}
		}

		private int LastIndexOfOrdinal(string s, char target, int start, int length)
		{
			if (s.Length == 0)
			{
				return -1;
			}
			int num = start - length;
			for (int num2 = start; num2 > num; num2--)
			{
				if (s[num2] == target)
				{
					return num2;
				}
			}
			return -1;
		}

		private int LastIndexOfOrdinalIgnoreCase(string s, char target, int start, int length)
		{
			if (s.Length == 0)
			{
				return -1;
			}
			int num = start - length;
			char c = textInfo.ToUpper(target);
			for (int num2 = start; num2 > num; num2--)
			{
				if (textInfo.ToUpper(s[num2]) == c)
				{
					return num2;
				}
			}
			return -1;
		}

		private unsafe int LastIndexOfSortKey(string s, int start, int orgStart, int length, byte* sortkey, int ti, bool noLv4, ref Context ctx)
		{
			int num = start - length;
			int idx = start;
			while (idx > num)
			{
				int result = idx;
				if (MatchesBackward(s, ref idx, num, orgStart, ti, sortkey, noLv4, ref ctx))
				{
					return result;
				}
			}
			return -1;
		}

		private unsafe int LastIndexOf(string s, string target, int start, int length, byte* targetSortKey, ref Context ctx)
		{
			CompareOptions option = ctx.Option;
			int num = start;
			int i;
			for (i = 0; i < target.Length && IsIgnorable(target[i], option); i++)
			{
			}
			if (i == target.Length)
			{
				return start;
			}
			Contraction contraction = GetContraction(target, i, target.Length - i);
			string text = ((contraction == null) ? null : contraction.Replacement);
			byte* ptr = ((text != null) ? null : targetSortKey);
			bool noLv = true;
			int num2 = -1;
			if (contraction != null && ptr != null)
			{
				for (int j = 0; j < contraction.SortKey.Length; j++)
				{
					ptr[j] = contraction.SortKey[j];
				}
			}
			else if (ptr != null)
			{
				num2 = FilterOptions(target[i], option);
				*ptr = Category(num2);
				ptr[1] = Level1(num2);
				if ((option & CompareOptions.IgnoreNonSpace) == 0)
				{
					ptr[2] = Level2(num2, ExtenderType.None);
				}
				ptr[3] = MSCompatUnicodeTable.Level3(num2);
				noLv = !MSCompatUnicodeTable.HasSpecialWeight((char)num2);
			}
			if (ptr != null)
			{
				for (i++; i < target.Length && Category(target[i]) == 1; i++)
				{
					if (ptr[2] == 0)
					{
						ptr[2] = 2;
					}
					ptr[2] = (byte)(ptr[2] + Level2(target[i], ExtenderType.None));
				}
			}
			do
			{
				int num3 = 0;
				num3 = ((text == null) ? LastIndexOfSortKey(s, start, num, length, ptr, num2, noLv, ref ctx) : LastIndexOf(s, text, start, length, targetSortKey, ref ctx));
				if (num3 < 0)
				{
					return -1;
				}
				length -= start - num3;
				start = num3;
				if (IsPrefix(s, target, num3, num - num3 + 1, false, ref ctx))
				{
					for (; num3 < num && IsIgnorable(s[num3], option); num3++)
					{
					}
					return num3;
				}
				Contraction contraction2 = GetContraction(s, num3, num - num3 + 1);
				if (contraction2 != null)
				{
					start -= contraction2.Source.Length;
					length -= contraction2.Source.Length;
				}
				else
				{
					start--;
					length--;
				}
			}
			while (length > 0);
			return -1;
		}

		private unsafe bool MatchesForward(string s, ref int idx, int end, int ti, byte* sortkey, bool noLv4, ref Context ctx)
		{
			int num = s[idx];
			if (ctx.AlwaysMatchFlags != null && num < 128 && (ctx.AlwaysMatchFlags[num / 8] & (1 << num % 8)) != 0)
			{
				return true;
			}
			if (ctx.NeverMatchFlags != null && num < 128 && (ctx.NeverMatchFlags[num / 8] & (1 << num % 8)) != 0)
			{
				idx++;
				return false;
			}
			ExtenderType extenderType = GetExtenderType(s[idx]);
			Contraction ct = null;
			if (MatchesForwardCore(s, ref idx, end, ti, sortkey, noLv4, extenderType, ref ct, ref ctx))
			{
				if (ctx.AlwaysMatchFlags != null && ct == null && extenderType == ExtenderType.None && num < 128)
				{
					byte* intPtr = ctx.AlwaysMatchFlags + num / 8;
					*intPtr |= (byte)(1 << num % 8);
				}
				return true;
			}
			if (ctx.NeverMatchFlags != null && ct == null && extenderType == ExtenderType.None && num < 128)
			{
				byte* intPtr2 = ctx.NeverMatchFlags + num / 8;
				*intPtr2 |= (byte)(1 << num % 8);
			}
			return false;
		}

		private unsafe bool MatchesForwardCore(string s, ref int idx, int end, int ti, byte* sortkey, bool noLv4, ExtenderType ext, ref Contraction ct, ref Context ctx)
		{
			CompareOptions option = ctx.Option;
			byte* ptr = ctx.Buffer1;
			bool flag = (option & CompareOptions.IgnoreNonSpace) != 0;
			int num = -1;
			if (ext == ExtenderType.None)
			{
				ct = GetContraction(s, idx, end);
			}
			else if (ctx.PrevCode < 0)
			{
				if (ctx.PrevSortKey == null)
				{
					idx++;
					return false;
				}
				ptr = ctx.PrevSortKey;
			}
			else
			{
				num = FilterExtender(ctx.PrevCode, ext, option);
			}
			if (ct != null)
			{
				idx += ct.Source.Length;
				if (!noLv4)
				{
					return false;
				}
				if (ct.SortKey == null)
				{
					int idx2 = 0;
					return MatchesForward(ct.Replacement, ref idx2, ct.Replacement.Length, ti, sortkey, noLv4, ref ctx);
				}
				for (int i = 0; i < 4; i++)
				{
					ptr[i] = sortkey[i];
				}
				ctx.PrevCode = -1;
				ctx.PrevSortKey = ptr;
			}
			else
			{
				if (num < 0)
				{
					num = FilterOptions(s[idx], option);
				}
				idx++;
				*ptr = Category(num);
				bool flag2 = false;
				if (*sortkey == *ptr)
				{
					ptr[1] = Level1(num);
				}
				else
				{
					flag2 = true;
				}
				if (!flag && sortkey[1] == ptr[1])
				{
					ptr[2] = Level2(num, ext);
				}
				else if (!flag)
				{
					flag2 = true;
				}
				if (flag2)
				{
					while (idx < end && Category(s[idx]) == 1)
					{
						idx++;
					}
					return false;
				}
				ptr[3] = MSCompatUnicodeTable.Level3(num);
				if (*ptr != 1)
				{
					ctx.PrevCode = num;
				}
			}
			while (idx < end && Category(s[idx]) == 1)
			{
				if (!flag)
				{
					if (ptr[2] == 0)
					{
						ptr[2] = 2;
					}
					ptr[2] = (byte)(ptr[2] + Level2(s[idx], ExtenderType.None));
				}
				idx++;
			}
			return MatchesPrimitive(option, ptr, num, ext, sortkey, ti, noLv4);
		}

		private unsafe bool MatchesPrimitive(CompareOptions opt, byte* source, int si, ExtenderType ext, byte* target, int ti, bool noLv4)
		{
			bool flag = (opt & CompareOptions.IgnoreNonSpace) != 0;
			if (*source != *target || source[1] != target[1] || (!flag && source[2] != target[2]) || source[3] != target[3])
			{
				return false;
			}
			if (noLv4 && (si < 0 || !MSCompatUnicodeTable.HasSpecialWeight((char)si)))
			{
				return true;
			}
			if (noLv4)
			{
				return false;
			}
			if (!flag && ext == ExtenderType.Conditional)
			{
				return false;
			}
			if (MSCompatUnicodeTable.IsJapaneseSmallLetter((char)si) != MSCompatUnicodeTable.IsJapaneseSmallLetter((char)ti) || ToDashTypeValue(ext, opt) != ToDashTypeValue(ExtenderType.None, opt) || !MSCompatUnicodeTable.IsHiragana((char)si) != !MSCompatUnicodeTable.IsHiragana((char)ti) || IsHalfKana((ushort)si, opt) != IsHalfKana((ushort)ti, opt))
			{
				return false;
			}
			return true;
		}

		private unsafe bool MatchesBackward(string s, ref int idx, int end, int orgStart, int ti, byte* sortkey, bool noLv4, ref Context ctx)
		{
			int num = s[idx];
			if (ctx.AlwaysMatchFlags != null && num < 128 && (ctx.AlwaysMatchFlags[num / 8] & (1 << num % 8)) != 0)
			{
				return true;
			}
			if (ctx.NeverMatchFlags != null && num < 128 && (ctx.NeverMatchFlags[num / 8] & (1 << num % 8)) != 0)
			{
				idx--;
				return false;
			}
			ExtenderType extenderType = GetExtenderType(s[idx]);
			Contraction ct = null;
			if (MatchesBackwardCore(s, ref idx, end, orgStart, ti, sortkey, noLv4, extenderType, ref ct, ref ctx))
			{
				if (ctx.AlwaysMatchFlags != null && ct == null && extenderType == ExtenderType.None && num < 128)
				{
					byte* intPtr = ctx.AlwaysMatchFlags + num / 8;
					*intPtr |= (byte)(1 << num % 8);
				}
				return true;
			}
			if (ctx.NeverMatchFlags != null && ct == null && extenderType == ExtenderType.None && num < 128)
			{
				byte* intPtr2 = ctx.NeverMatchFlags + num / 8;
				*intPtr2 |= (byte)(1 << num % 8);
			}
			return false;
		}

		private unsafe bool MatchesBackwardCore(string s, ref int idx, int end, int orgStart, int ti, byte* sortkey, bool noLv4, ExtenderType ext, ref Contraction ct, ref Context ctx)
		{
			CompareOptions option = ctx.Option;
			byte* buffer = ctx.Buffer1;
			bool flag = (option & CompareOptions.IgnoreNonSpace) != 0;
			int num = idx;
			int num2 = -1;
			if (ext != ExtenderType.None)
			{
				byte b = 0;
				int num3 = 0;
				int num4;
				byte b2;
				while (true)
				{
					if (num3 < 0)
					{
						return false;
					}
					if (!IsIgnorable(s[num3], option))
					{
						num4 = FilterOptions(s[num3], option);
						b2 = Category(num4);
						if (b2 != 1)
						{
							break;
						}
						b = Level2(num4, ExtenderType.None);
					}
					num3--;
				}
				num2 = FilterExtender(num4, ext, option);
				*buffer = b2;
				buffer[1] = Level1(num2);
				if (!flag)
				{
					buffer[2] = Level2(num2, ext);
				}
				buffer[3] = MSCompatUnicodeTable.Level3(num2);
				if (ext != ExtenderType.Conditional && b != 0)
				{
					buffer[2] = ((buffer[2] != 0) ? b : ((byte)(b + 2)));
				}
				idx--;
			}
			if (ext == ExtenderType.None)
			{
				ct = GetTailContraction(s, idx, end);
			}
			if (ct != null)
			{
				idx -= ct.Source.Length;
				if (!noLv4)
				{
					return false;
				}
				if (ct.SortKey == null)
				{
					int num5 = ct.Replacement.Length - 1;
					return 0 <= LastIndexOfSortKey(ct.Replacement, num5, num5, ct.Replacement.Length, sortkey, ti, noLv4, ref ctx);
				}
				for (int i = 0; i < 4; i++)
				{
					buffer[i] = sortkey[i];
				}
				ctx.PrevCode = -1;
				ctx.PrevSortKey = buffer;
			}
			else if (ext == ExtenderType.None)
			{
				if (num2 < 0)
				{
					num2 = FilterOptions(s[idx], option);
				}
				idx--;
				bool flag2 = false;
				*buffer = Category(num2);
				if (*buffer == *sortkey)
				{
					buffer[1] = Level1(num2);
				}
				else
				{
					flag2 = true;
				}
				if (!flag && buffer[1] == sortkey[1])
				{
					buffer[2] = Level2(num2, ext);
				}
				else if (!flag)
				{
					flag2 = true;
				}
				if (flag2)
				{
					return false;
				}
				buffer[3] = MSCompatUnicodeTable.Level3(num2);
				if (*buffer != 1)
				{
					ctx.PrevCode = num2;
				}
			}
			if (ext == ExtenderType.None)
			{
				for (int j = num + 1; j < orgStart && Category(s[j]) == 1; j++)
				{
					if (!flag)
					{
						if (buffer[2] == 0)
						{
							buffer[2] = 2;
						}
						buffer[2] = (byte)(buffer[2] + Level2(s[j], ExtenderType.None));
					}
				}
			}
			return MatchesPrimitive(option, buffer, num2, ext, sortkey, ti, noLv4);
		}
	}
}
