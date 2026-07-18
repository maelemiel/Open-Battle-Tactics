using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mono.Globalization.Unicode
{
	internal class MSCompatUnicodeTable
	{
		private const int ResourceVersionSize = 1;

		public static int MaxExpansionLength;

		private unsafe static readonly byte* ignorableFlags;

		private unsafe static readonly byte* categories;

		private unsafe static readonly byte* level1;

		private unsafe static readonly byte* level2;

		private unsafe static readonly byte* level3;

		private unsafe static byte* cjkCHScategory;

		private unsafe static byte* cjkCHTcategory;

		private unsafe static byte* cjkJAcategory;

		private unsafe static byte* cjkKOcategory;

		private unsafe static byte* cjkCHSlv1;

		private unsafe static byte* cjkCHTlv1;

		private unsafe static byte* cjkJAlv1;

		private unsafe static byte* cjkKOlv1;

		private unsafe static byte* cjkKOlv2;

		private static readonly char[] tailoringArr;

		private static readonly TailoringInfo[] tailoringInfos;

		private static object forLock;

		public static readonly bool isReady;

		public static bool IsReady
		{
			get
			{
				return isReady;
			}
		}

		unsafe static MSCompatUnicodeTable()
		{
			MaxExpansionLength = 3;
			forLock = new object();
			uint num = 0u;
			IntPtr resource = GetResource("collation.core.bin");
			if (resource == IntPtr.Zero)
			{
				return;
			}
			byte* ptr = (byte*)(void*)resource;
			resource = GetResource("collation.tailoring.bin");
			if (resource == IntPtr.Zero)
			{
				return;
			}
			byte* ptr2 = (byte*)(void*)resource;
			if (ptr != null && ptr2 != null && *ptr == 3 && *ptr2 == 3)
			{
				num = 1u;
				uint num2 = UInt32FromBytePtr(ptr, num);
				num += 4;
				ignorableFlags = ptr + num;
				num += num2;
				num2 = UInt32FromBytePtr(ptr, num);
				num += 4;
				categories = ptr + num;
				num += num2;
				num2 = UInt32FromBytePtr(ptr, num);
				num += 4;
				level1 = ptr + num;
				num += num2;
				num2 = UInt32FromBytePtr(ptr, num);
				num += 4;
				level2 = ptr + num;
				num += num2;
				num2 = UInt32FromBytePtr(ptr, num);
				num += 4;
				level3 = ptr + num;
				num += num2;
				num = 1u;
				uint num3 = UInt32FromBytePtr(ptr2, num);
				num += 4;
				tailoringInfos = new TailoringInfo[num3];
				for (int i = 0; i < num3; i++)
				{
					int lcid = (int)UInt32FromBytePtr(ptr2, num);
					num += 4;
					int tailoringIndex = (int)UInt32FromBytePtr(ptr2, num);
					num += 4;
					int tailoringCount = (int)UInt32FromBytePtr(ptr2, num);
					num += 4;
					TailoringInfo tailoringInfo = new TailoringInfo(lcid, tailoringIndex, tailoringCount, ptr2[num++] != 0);
					tailoringInfos[i] = tailoringInfo;
				}
				num += 2;
				num3 = UInt32FromBytePtr(ptr2, num);
				num += 4;
				tailoringArr = new char[num3];
				int num4 = 0;
				while (num4 < num3)
				{
					tailoringArr[num4] = (char)(ptr2[num] + (ptr2[num + 1] << 8));
					num4++;
					num += 2;
				}
				isReady = true;
			}
		}

		public static TailoringInfo GetTailoringInfo(int lcid)
		{
			for (int i = 0; i < tailoringInfos.Length; i++)
			{
				if (tailoringInfos[i].LCID == lcid)
				{
					return tailoringInfos[i];
				}
			}
			return null;
		}

		public unsafe static void BuildTailoringTables(CultureInfo culture, TailoringInfo t, ref Contraction[] contractions, ref Level2Map[] diacriticals)
		{
			//IL_0029->IL0034: Incompatible stack types: I vs Ref
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			fixed (char* ptr = &(tailoringArr != null && tailoringArr.Length != 0 ? ref tailoringArr[0] : ref *(char*)null))
			{
				int num = t.TailoringIndex;
				int num2 = num + t.TailoringCount;
				while (num < num2)
				{
					int i = num + 1;
					char[] array = null;
					switch ((char)(*(ushort*)((byte*)ptr + num * 2)))
					{
					case '\u0001':
					{
						num++;
						for (; *(ushort*)((byte*)ptr + i * 2) != 0; i++)
						{
						}
						array = new char[i - num];
						Marshal.Copy((IntPtr)((byte*)ptr + num * 2), array, 0, i - num);
						byte[] array2 = new byte[4];
						for (int k = 0; k < 4; k++)
						{
							array2[k] = (byte)(*(ushort*)((byte*)ptr + (i + 1 + k) * 2));
						}
						arrayList.Add(new Contraction(array, null, array2));
						num = i + 6;
						break;
					}
					case '\u0002':
						arrayList2.Add(new Level2Map((byte)(*(ushort*)((byte*)ptr + (num + 1) * 2)), (byte)(*(ushort*)((byte*)ptr + (num + 2) * 2))));
						num += 3;
						break;
					case '\u0003':
					{
						num++;
						for (; *(ushort*)((byte*)ptr + i * 2) != 0; i++)
						{
						}
						array = new char[i - num];
						Marshal.Copy((IntPtr)((byte*)ptr + num * 2), array, 0, i - num);
						i++;
						int j;
						for (j = i; *(ushort*)((byte*)ptr + j * 2) != 0; j++)
						{
						}
						string replacement = new string(ptr, i, j - i);
						arrayList.Add(new Contraction(array, replacement, null));
						num = j + 1;
						break;
					}
					default:
						throw new NotImplementedException(string.Format("Mono INTERNAL ERROR (Should not happen): Collation tailoring table is broken for culture {0} ({1}) at 0x{2:X}", culture.LCID, culture.Name, num));
					}
				}
			}
			arrayList.Sort(ContractionComparer.Instance);
			arrayList2.Sort(Level2MapComparer.Instance);
			contractions = arrayList.ToArray(typeof(Contraction)) as Contraction[];
			diacriticals = arrayList2.ToArray(typeof(Level2Map)) as Level2Map[];
		}

		private unsafe static void SetCJKReferences(string name, ref CodePointIndexer cjkIndexer, ref byte* catTable, ref byte* lv1Table, ref CodePointIndexer lv2Indexer, ref byte* lv2Table)
		{
			//IL_0083: Expected O, but got I
			//IL_008a: Expected O, but got I
			//IL_009d: Expected O, but got I
			//IL_00a4: Expected O, but got I
			//IL_00b7: Expected O, but got I
			//IL_00be: Expected O, but got I
			//IL_00d1: Expected O, but got I
			//IL_00d8: Expected O, but got I
			//IL_00e0: Expected O, but got I
			switch (name)
			{
			case "zh-CHS":
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref catTable) = (object)cjkCHScategory;
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv1Table) = (object)cjkCHSlv1;
				cjkIndexer = MSCompatUnicodeTableUtil.CjkCHS;
				break;
			case "zh-CHT":
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref catTable) = (object)cjkCHTcategory;
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv1Table) = (object)cjkCHTlv1;
				cjkIndexer = MSCompatUnicodeTableUtil.Cjk;
				break;
			case "ja":
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref catTable) = (object)cjkJAcategory;
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv1Table) = (object)cjkJAlv1;
				cjkIndexer = MSCompatUnicodeTableUtil.Cjk;
				break;
			case "ko":
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref catTable) = (object)cjkKOcategory;
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv1Table) = (object)cjkKOlv1;
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv2Table) = (object)cjkKOlv2;
				cjkIndexer = MSCompatUnicodeTableUtil.Cjk;
				lv2Indexer = MSCompatUnicodeTableUtil.Cjk;
				break;
			}
		}

		public unsafe static byte Category(int cp)
		{
			return categories[MSCompatUnicodeTableUtil.Category.ToIndex(cp)];
		}

		public unsafe static byte Level1(int cp)
		{
			return level1[MSCompatUnicodeTableUtil.Level1.ToIndex(cp)];
		}

		public unsafe static byte Level2(int cp)
		{
			return level2[MSCompatUnicodeTableUtil.Level2.ToIndex(cp)];
		}

		public unsafe static byte Level3(int cp)
		{
			return level3[MSCompatUnicodeTableUtil.Level3.ToIndex(cp)];
		}

		public static bool IsSortable(string s)
		{
			foreach (char cp in s)
			{
				if (!IsSortable(cp))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsSortable(int cp)
		{
			if (!IsIgnorable(cp))
			{
				return true;
			}
			if (cp == 0 || cp == 1600 || cp == 65279)
			{
				return true;
			}
			return (6155 <= cp && cp <= 6158) || (8204 <= cp && cp <= 8207) || (8234 <= cp && cp <= 8238) || (8298 <= cp && cp <= 8303) || (8204 <= cp && cp <= 8207) || (65529 <= cp && cp <= 65533);
		}

		public static bool IsIgnorable(int cp)
		{
			return IsIgnorable(cp, 1);
		}

		public unsafe static bool IsIgnorable(int cp, byte flag)
		{
			if (cp == 0)
			{
				return false;
			}
			if ((flag & 1) != 0)
			{
				UnicodeCategory unicodeCategory = char.GetUnicodeCategory((char)cp);
				if (unicodeCategory == UnicodeCategory.OtherNotAssigned)
				{
					return true;
				}
				if (55424 <= cp && cp < 56192)
				{
					return true;
				}
			}
			int num = MSCompatUnicodeTableUtil.Ignorable.ToIndex(cp);
			return num >= 0 && (ignorableFlags[num] & flag) != 0;
		}

		public static bool IsIgnorableSymbol(int cp)
		{
			return IsIgnorable(cp, 2);
		}

		public static bool IsIgnorableNonSpacing(int cp)
		{
			return IsIgnorable(cp, 4);
		}

		public static int ToKanaTypeInsensitive(int i)
		{
			return (12353 > i || i > 12436) ? i : (i + 96);
		}

		public static int ToWidthCompat(int i)
		{
			if (i < 8592)
			{
				return i;
			}
			switch (i)
			{
			case 65281:
			case 65282:
			case 65283:
			case 65284:
			case 65285:
			case 65286:
			case 65287:
			case 65288:
			case 65289:
			case 65290:
			case 65291:
			case 65292:
			case 65293:
			case 65294:
			case 65295:
			case 65296:
			case 65297:
			case 65298:
			case 65299:
			case 65300:
			case 65301:
			case 65302:
			case 65303:
			case 65304:
			case 65305:
			case 65306:
			case 65307:
			case 65308:
			case 65309:
			case 65310:
			case 65311:
			case 65312:
			case 65313:
			case 65314:
			case 65315:
			case 65316:
			case 65317:
			case 65318:
			case 65319:
			case 65320:
			case 65321:
			case 65322:
			case 65323:
			case 65324:
			case 65325:
			case 65326:
			case 65327:
			case 65328:
			case 65329:
			case 65330:
			case 65331:
			case 65332:
			case 65333:
			case 65334:
			case 65335:
			case 65336:
			case 65337:
			case 65338:
			case 65339:
			case 65340:
			case 65341:
			case 65342:
			case 65343:
			case 65344:
			case 65345:
			case 65346:
			case 65347:
			case 65348:
			case 65349:
			case 65350:
			case 65351:
			case 65352:
			case 65353:
			case 65354:
			case 65355:
			case 65356:
			case 65357:
			case 65358:
			case 65359:
			case 65360:
			case 65361:
			case 65362:
			case 65363:
			case 65364:
			case 65365:
			case 65366:
			case 65367:
			case 65368:
			case 65369:
			case 65370:
			case 65371:
			case 65372:
			case 65373:
			case 65374:
				return i - 65280 + 32;
			case 65504:
				return 162;
			case 65505:
				return 163;
			case 65506:
				return 172;
			case 65507:
				return 175;
			case 65508:
				return 166;
			case 65509:
				return 165;
			case 65510:
				return 8361;
			default:
				if (i > 13054)
				{
					return i;
				}
				if (i <= 8595)
				{
					return 56921 + i;
				}
				if (i < 9474)
				{
					return i;
				}
				if (i <= 9675)
				{
					switch (i)
					{
					case 9474:
						return 65512;
					case 9632:
						return 65517;
					case 9675:
						return 65518;
					default:
						return i;
					}
				}
				if (i < 12288)
				{
					return i;
				}
				if (i < 12593)
				{
					switch (i)
					{
					case 12288:
						return 32;
					case 12289:
						return 65380;
					case 12290:
						return 65377;
					case 12300:
						return 65378;
					case 12301:
						return 65379;
					case 12539:
						return 65381;
					default:
						return i;
					}
				}
				if (i < 12644)
				{
					return i - 12592 + 65440;
				}
				if (i == 12644)
				{
					return 65440;
				}
				return i;
			}
		}

		public static bool HasSpecialWeight(char c)
		{
			if (c < 'ぁ')
			{
				return false;
			}
			if ('ｦ' <= c && c < 'ﾞ')
			{
				return true;
			}
			if ('㌀' <= c)
			{
				return false;
			}
			if (c < 'ゝ')
			{
				return c < '\u3099';
			}
			if (c < '\u3100')
			{
				return c != '・';
			}
			if (c < '㋐')
			{
				return false;
			}
			if (c < '㋿')
			{
				return true;
			}
			return false;
		}

		public static byte GetJapaneseDashType(char c)
		{
			switch (c)
			{
			case 'ゝ':
			case 'ゞ':
			case 'ヽ':
			case 'ヾ':
			case 'ｰ':
				return 4;
			case 'ー':
				return 5;
			default:
				return 3;
			}
		}

		public static bool IsHalfWidthKana(char c)
		{
			return 'ｦ' <= c && c <= 'ﾝ';
		}

		public static bool IsHiragana(char c)
		{
			return 'ぁ' <= c && c <= 'ゔ';
		}

		public static bool IsJapaneseSmallLetter(char c)
		{
			if ('ｧ' <= c && c <= 'ｯ')
			{
				return true;
			}
			if ('\u3040' < c && c < 'ヺ')
			{
				switch (c)
				{
				case 'ぁ':
				case 'ぃ':
				case 'ぅ':
				case 'ぇ':
				case 'ぉ':
				case 'っ':
				case 'ゃ':
				case 'ゅ':
				case 'ょ':
				case 'ゎ':
				case 'ァ':
				case 'ィ':
				case 'ゥ':
				case 'ェ':
				case 'ォ':
				case 'ッ':
				case 'ャ':
				case 'ュ':
				case 'ョ':
				case 'ヮ':
				case 'ヵ':
				case 'ヶ':
					return true;
				}
			}
			return false;
		}

		private static IntPtr GetResource(string name)
		{
			int size;
			Module module;
			return Assembly.GetExecutingAssembly().GetManifestResourceInternal(name, out size, out module);
		}

		private unsafe static uint UInt32FromBytePtr(byte* raw, uint idx)
		{
			return (uint)(raw[idx] + (raw[idx + 1] << 8) + (raw[idx + 2] << 16) + (raw[idx + 3] << 24));
		}

		public unsafe static void FillCJK(string culture, ref CodePointIndexer cjkIndexer, ref byte* catTable, ref byte* lv1Table, ref CodePointIndexer lv2Indexer, ref byte* lv2Table)
		{
			lock (forLock)
			{
				FillCJKCore(culture, ref cjkIndexer, ref catTable, ref lv1Table, ref lv2Indexer, ref lv2Table);
				SetCJKReferences(culture, ref cjkIndexer, ref catTable, ref lv1Table, ref lv2Indexer, ref lv2Table);
			}
		}

		private unsafe static void FillCJKCore(string culture, ref CodePointIndexer cjkIndexer, ref byte* catTable, ref byte* lv1Table, ref CodePointIndexer cjkLv2Indexer, ref byte* lv2Table)
		{
			//IL_014c: Expected O, but got I
			//IL_0156: Expected O, but got I
			//IL_00a0: Expected O, but got I
			//IL_00a7: Expected O, but got I
			//IL_00b9: Expected O, but got I
			//IL_00c0: Expected O, but got I
			//IL_00d2: Expected O, but got I
			//IL_00d9: Expected O, but got I
			//IL_00eb: Expected O, but got I
			//IL_00f2: Expected O, but got I
			//IL_0272: Expected O, but got I
			if (!IsReady)
			{
				return;
			}
			string text = null;
			switch (culture)
			{
			case "zh-CHS":
				text = "cjkCHS";
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref catTable) = (object)cjkCHScategory;
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv1Table) = (object)cjkCHSlv1;
				break;
			case "zh-CHT":
				text = "cjkCHT";
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref catTable) = (object)cjkCHTcategory;
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv1Table) = (object)cjkCHTlv1;
				break;
			case "ja":
				text = "cjkJA";
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref catTable) = (object)cjkJAcategory;
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv1Table) = (object)cjkJAlv1;
				break;
			case "ko":
				text = "cjkKO";
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref catTable) = (object)cjkKOcategory;
				System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv1Table) = (object)cjkKOlv1;
				break;
			}
			if (text == null || lv1Table != null)
			{
				return;
			}
			uint num = 0u;
			string name = string.Format("collation.{0}.bin", text);
			IntPtr resource = GetResource(name);
			if (resource == IntPtr.Zero)
			{
				return;
			}
			byte* ptr = (byte*)(void*)resource;
			num++;
			uint num2 = UInt32FromBytePtr(ptr, num);
			num += 4;
			System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref catTable) = (object)(ptr + num);
			System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv1Table) = (object)(ptr + num + num2);
			switch (culture)
			{
			case "zh-CHS":
				cjkCHScategory = catTable;
				cjkCHSlv1 = lv1Table;
				break;
			case "zh-CHT":
				cjkCHTcategory = catTable;
				cjkCHTlv1 = lv1Table;
				break;
			case "ja":
				cjkJAcategory = catTable;
				cjkJAlv1 = lv1Table;
				break;
			case "ko":
				cjkKOcategory = catTable;
				cjkKOlv1 = lv1Table;
				break;
			}
			if (!(text != "cjkKO"))
			{
				resource = GetResource("collation.cjkKOlv2.bin");
				if (!(resource == IntPtr.Zero))
				{
					ptr = (byte*)(void*)resource;
					num = 5u;
					cjkKOlv2 = ptr + num;
					System.Runtime.CompilerServices.Unsafe.As<byte*, object>(ref lv2Table) = (object)cjkKOlv2;
				}
			}
		}
	}
}
