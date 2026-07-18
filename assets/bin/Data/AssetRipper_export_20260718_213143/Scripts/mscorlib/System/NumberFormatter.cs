using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace System
{
	internal sealed class NumberFormatter
	{
		private class CustomInfo
		{
			public bool UseGroup;

			public int DecimalDigits;

			public int DecimalPointPos = -1;

			public int DecimalTailSharpDigits;

			public int IntegerDigits;

			public int IntegerHeadSharpDigits;

			public int IntegerHeadPos;

			public bool UseExponent;

			public int ExponentDigits;

			public int ExponentTailSharpDigits;

			public bool ExponentNegativeSignOnly = true;

			public int DividePlaces;

			public int Percents;

			public int Permilles;

			public static void GetActiveSection(string format, ref bool positive, bool zero, ref int offset, ref int length)
			{
				int[] array = new int[3];
				int num = 0;
				int num2 = 0;
				char c = '\0';
				for (int i = 0; i < format.Length; i++)
				{
					char c2 = format[i];
					if (c2 == c || (c == '\0' && (c2 == '"' || c2 == '\'')))
					{
						c = ((c == '\0') ? c2 : '\0');
					}
					else if (c == '\0' && format[i] == ';' && (i == 0 || format[i - 1] != '\\'))
					{
						array[num++] = i - num2;
						num2 = i + 1;
						if (num == 3)
						{
							break;
						}
					}
				}
				switch (num)
				{
				case 0:
					offset = 0;
					length = format.Length;
					break;
				case 1:
					if (positive || zero)
					{
						offset = 0;
						length = array[0];
					}
					else if (array[0] + 1 < format.Length)
					{
						positive = true;
						offset = array[0] + 1;
						length = format.Length - offset;
					}
					else
					{
						offset = 0;
						length = array[0];
					}
					break;
				case 2:
					if (zero)
					{
						offset = array[0] + array[1] + 2;
						length = format.Length - offset;
					}
					else if (positive)
					{
						offset = 0;
						length = array[0];
					}
					else if (array[1] > 0)
					{
						positive = true;
						offset = array[0] + 1;
						length = array[1];
					}
					else
					{
						offset = 0;
						length = array[0];
					}
					break;
				case 3:
					if (zero)
					{
						offset = array[0] + array[1] + 2;
						length = array[2];
					}
					else if (positive)
					{
						offset = 0;
						length = array[0];
					}
					else if (array[1] > 0)
					{
						positive = true;
						offset = array[0] + 1;
						length = array[1];
					}
					else
					{
						offset = 0;
						length = array[0];
					}
					break;
				default:
					throw new ArgumentException();
				}
			}

			public static CustomInfo Parse(string format, int offset, int length, NumberFormatInfo nfi)
			{
				char c = '\0';
				bool flag = true;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = true;
				CustomInfo customInfo = new CustomInfo();
				int num = 0;
				for (int i = offset; i - offset < length; i++)
				{
					char c2 = format[i];
					if (c2 == c && c2 != 0)
					{
						c = '\0';
					}
					else
					{
						if (c != 0)
						{
							continue;
						}
						if (flag3 && c2 != 0 && c2 != '0' && c2 != '#')
						{
							flag3 = false;
							flag = customInfo.DecimalPointPos < 0;
							flag2 = !flag;
							i--;
							continue;
						}
						switch (c2)
						{
						case '\\':
							i++;
							break;
						case '"':
						case '\'':
							if (c2 == '"' || c2 == '\'')
							{
								c = c2;
							}
							break;
						case '#':
							if (flag4 && flag)
							{
								customInfo.IntegerHeadSharpDigits++;
							}
							else if (flag2)
							{
								customInfo.DecimalTailSharpDigits++;
							}
							else if (flag3)
							{
								customInfo.ExponentTailSharpDigits++;
							}
							goto case '0';
						case '0':
							if (c2 != '#')
							{
								flag4 = false;
								if (flag2)
								{
									customInfo.DecimalTailSharpDigits = 0;
								}
								else if (flag3)
								{
									customInfo.ExponentTailSharpDigits = 0;
								}
							}
							if (customInfo.IntegerHeadPos == -1)
							{
								customInfo.IntegerHeadPos = i;
							}
							if (flag)
							{
								customInfo.IntegerDigits++;
								if (num > 0)
								{
									customInfo.UseGroup = true;
								}
								num = 0;
							}
							else if (flag2)
							{
								customInfo.DecimalDigits++;
							}
							else if (flag3)
							{
								customInfo.ExponentDigits++;
							}
							break;
						case 'E':
						case 'e':
						{
							if (customInfo.UseExponent)
							{
								break;
							}
							customInfo.UseExponent = true;
							flag = false;
							flag2 = false;
							flag3 = true;
							if (i + 1 - offset >= length)
							{
								break;
							}
							char c3 = format[i + 1];
							if (c3 == '+')
							{
								customInfo.ExponentNegativeSignOnly = false;
							}
							switch (c3)
							{
							case '+':
							case '-':
								i++;
								break;
							default:
								customInfo.UseExponent = false;
								if (customInfo.DecimalPointPos < 0)
								{
									flag = true;
								}
								break;
							case '#':
							case '0':
								break;
							}
							break;
						}
						case '.':
							flag = false;
							flag2 = true;
							flag3 = false;
							if (customInfo.DecimalPointPos == -1)
							{
								customInfo.DecimalPointPos = i;
							}
							break;
						case '%':
							customInfo.Percents++;
							break;
						case '‰':
							customInfo.Permilles++;
							break;
						case ',':
							if (flag && customInfo.IntegerDigits > 0)
							{
								num++;
							}
							break;
						}
					}
				}
				if (customInfo.ExponentDigits == 0)
				{
					customInfo.UseExponent = false;
				}
				else
				{
					customInfo.IntegerHeadSharpDigits = 0;
				}
				if (customInfo.DecimalDigits == 0)
				{
					customInfo.DecimalPointPos = -1;
				}
				customInfo.DividePlaces += num * 3;
				return customInfo;
			}

			public string Format(string format, int offset, int length, NumberFormatInfo nfi, bool positive, StringBuilder sb_int, StringBuilder sb_dec, StringBuilder sb_exp)
			{
				StringBuilder stringBuilder = new StringBuilder();
				char c = '\0';
				bool flag = true;
				bool flag2 = false;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int[] rawNumberGroupSizes = nfi.RawNumberGroupSizes;
				string numberGroupSeparator = nfi.NumberGroupSeparator;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				if (UseGroup && rawNumberGroupSizes.Length > 0)
				{
					num4 = sb_int.Length;
					for (int i = 0; i < rawNumberGroupSizes.Length; i++)
					{
						num5 += rawNumberGroupSizes[i];
						if (num5 <= num4)
						{
							num6 = i;
						}
					}
					num8 = rawNumberGroupSizes[num6];
					int num9 = ((num4 > num5) ? (num4 - num5) : 0);
					if (num8 == 0)
					{
						while (num6 >= 0 && rawNumberGroupSizes[num6] == 0)
						{
							num6--;
						}
						num8 = ((num9 <= 0) ? rawNumberGroupSizes[num6] : num9);
					}
					if (num9 == 0)
					{
						num7 = num8;
					}
					else
					{
						num6 += num9 / num8;
						num7 = num9 % num8;
						if (num7 == 0)
						{
							num7 = num8;
						}
						else
						{
							num6++;
						}
					}
				}
				else
				{
					UseGroup = false;
				}
				for (int j = offset; j - offset < length; j++)
				{
					char c2 = format[j];
					if (c2 == c && c2 != 0)
					{
						c = '\0';
						continue;
					}
					if (c != 0)
					{
						stringBuilder.Append(c2);
						continue;
					}
					switch (c2)
					{
					case '\\':
						j++;
						if (j - offset < length)
						{
							stringBuilder.Append(format[j]);
						}
						break;
					case '"':
					case '\'':
						if (c2 == '"' || c2 == '\'')
						{
							c = c2;
						}
						break;
					case '#':
					case '0':
						if (flag)
						{
							num++;
							if (IntegerDigits - num >= sb_int.Length + num2 && c2 != '0')
							{
								break;
							}
							while (IntegerDigits - num + num2 < sb_int.Length)
							{
								stringBuilder.Append(sb_int[num2++]);
								if (UseGroup && --num4 > 0 && --num7 == 0)
								{
									stringBuilder.Append(numberGroupSeparator);
									if (--num6 < rawNumberGroupSizes.Length && num6 >= 0)
									{
										num8 = rawNumberGroupSizes[num6];
									}
									num7 = num8;
								}
							}
						}
						else if (flag2)
						{
							if (num3 < sb_dec.Length)
							{
								stringBuilder.Append(sb_dec[num3++]);
							}
						}
						else
						{
							stringBuilder.Append(c2);
						}
						break;
					case 'E':
					case 'e':
					{
						if (sb_exp == null || !UseExponent)
						{
							stringBuilder.Append(c2);
							break;
						}
						bool flag3 = true;
						bool flag4 = false;
						int k;
						for (k = j + 1; k - offset < length; k++)
						{
							if (format[k] == '0')
							{
								flag4 = true;
							}
							else if (k != j + 1 || (format[k] != '+' && format[k] != '-'))
							{
								if (!flag4)
								{
									flag3 = false;
								}
								break;
							}
						}
						if (flag3)
						{
							j = k - 1;
							flag = DecimalPointPos < 0;
							flag2 = !flag;
							stringBuilder.Append(c2);
							stringBuilder.Append(sb_exp);
							sb_exp = null;
						}
						else
						{
							stringBuilder.Append(c2);
						}
						break;
					}
					case '.':
						if (DecimalPointPos == j)
						{
							if (DecimalDigits > 0)
							{
								while (num2 < sb_int.Length)
								{
									stringBuilder.Append(sb_int[num2++]);
								}
							}
							if (sb_dec.Length > 0)
							{
								stringBuilder.Append(nfi.NumberDecimalSeparator);
							}
						}
						flag = false;
						flag2 = true;
						break;
					case '%':
						stringBuilder.Append(nfi.PercentSymbol);
						break;
					case '‰':
						stringBuilder.Append(nfi.PerMilleSymbol);
						break;
					default:
						stringBuilder.Append(c2);
						break;
					case ',':
						break;
					}
				}
				if (!positive)
				{
					stringBuilder.Insert(0, nfi.NegativeSign);
				}
				return stringBuilder.ToString();
			}
		}

		private const int DefaultExpPrecision = 6;

		private const int HundredMillion = 100000000;

		private const long SeventeenDigitsThreshold = 10000000000000000L;

		private const ulong ULongDivHundredMillion = 184467440737uL;

		private const ulong ULongModHundredMillion = 9551616uL;

		private const int DoubleBitsExponentShift = 52;

		private const int DoubleBitsExponentMask = 2047;

		private const long DoubleBitsMantissaMask = 4503599627370495L;

		private const int DecimalBitsScaleMask = 2031616;

		private const int SingleDefPrecision = 7;

		private const int DoubleDefPrecision = 15;

		private const int Int8DefPrecision = 3;

		private const int UInt8DefPrecision = 3;

		private const int Int16DefPrecision = 5;

		private const int UInt16DefPrecision = 5;

		private const int Int32DefPrecision = 10;

		private const int UInt32DefPrecision = 10;

		private const int Int64DefPrecision = 19;

		private const int UInt64DefPrecision = 20;

		private const int DecimalDefPrecision = 100;

		private const int TenPowersListLength = 19;

		private const double MinRoundtripVal = -1.79769313486231E+308;

		private const double MaxRoundtripVal = 1.79769313486231E+308;

		private unsafe static readonly ulong* MantissaBitsTable;

		private unsafe static readonly int* TensExponentTable;

		private unsafe static readonly char* DigitLowerTable;

		private unsafe static readonly char* DigitUpperTable;

		private unsafe static readonly long* TenPowersList;

		private unsafe static readonly int* DecHexDigits;

		private Thread _thread;

		private NumberFormatInfo _nfi;

		private bool _NaN;

		private bool _infinity;

		private bool _isCustomFormat;

		private bool _specifierIsUpper;

		private bool _positive;

		private char _specifier;

		private int _precision;

		private int _defPrecision;

		private int _digitsLen;

		private int _offset;

		private int _decPointPos;

		private uint _val1;

		private uint _val2;

		private uint _val3;

		private uint _val4;

		private char[] _cbuf;

		private int _ind;

		[ThreadStatic]
		private static NumberFormatter threadNumberFormatter;

		public CultureInfo CurrentCulture
		{
			set
			{
				if (value != null && value.IsReadOnly)
				{
					_nfi = value.NumberFormat;
				}
				else
				{
					_nfi = null;
				}
			}
		}

		private int IntegerDigits
		{
			get
			{
				return (_decPointPos <= 0) ? 1 : _decPointPos;
			}
		}

		private int DecimalDigits
		{
			get
			{
				return (_digitsLen > _decPointPos) ? (_digitsLen - _decPointPos) : 0;
			}
		}

		private bool IsFloatingSource
		{
			get
			{
				return _defPrecision == 15 || _defPrecision == 7;
			}
		}

		private bool IsZero
		{
			get
			{
				return _digitsLen == 0;
			}
		}

		private bool IsZeroInteger
		{
			get
			{
				return _digitsLen == 0 || _decPointPos <= 0;
			}
		}

		public NumberFormatter(Thread current)
		{
			_cbuf = new char[0];
			if (current != null)
			{
				_thread = current;
				CurrentCulture = _thread.CurrentCulture;
			}
		}

		unsafe static NumberFormatter()
		{
			GetFormatterTables(out MantissaBitsTable, out TensExponentTable, out DigitLowerTable, out DigitUpperTable, out TenPowersList, out DecHexDigits);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GetFormatterTables(out ulong* MantissaBitsTable, out int* TensExponentTable, out char* DigitLowerTable, out char* DigitUpperTable, out long* TenPowersList, out int* DecHexDigits);

		private unsafe static long GetTenPowerOf(int i)
		{
			return *(long*)((byte*)TenPowersList + i * 8);
		}

		private void InitDecHexDigits(uint value)
		{
			if (value >= 100000000)
			{
				int num = (int)(value / 100000000);
				value -= (uint)(100000000 * num);
				_val2 = FastToDecHex(num);
			}
			_val1 = ToDecHex((int)value);
		}

		private void InitDecHexDigits(ulong value)
		{
			if (value >= 100000000)
			{
				long num = (long)(value / 100000000);
				value -= (ulong)(100000000 * num);
				if (num >= 100000000)
				{
					int num2 = (int)(num / 100000000);
					num -= (long)num2 * 100000000L;
					_val3 = ToDecHex(num2);
				}
				if (num != 0L)
				{
					_val2 = ToDecHex((int)num);
				}
			}
			if (value != 0L)
			{
				_val1 = ToDecHex((int)value);
			}
		}

		private void InitDecHexDigits(uint hi, ulong lo)
		{
			if (hi == 0)
			{
				InitDecHexDigits(lo);
				return;
			}
			uint num = hi / 100000000;
			ulong num2 = hi - num * 100000000;
			ulong num3 = lo / 100000000;
			ulong num4 = lo - num3 * 100000000 + num2 * 9551616;
			hi = num;
			lo = num3 + num2 * 184467440737L;
			num3 = num4 / 100000000;
			num4 -= num3 * 100000000;
			lo += num3;
			_val1 = ToDecHex((int)num4);
			num3 = lo / 100000000;
			num4 = lo - num3 * 100000000;
			lo = num3;
			if (hi != 0)
			{
				lo += (ulong)((long)hi * 184467440737L);
				num4 += (ulong)((long)hi * 9551616L);
				num3 = num4 / 100000000;
				lo += num3;
				num4 -= num3 * 100000000;
			}
			_val2 = ToDecHex((int)num4);
			if (lo >= 100000000)
			{
				num3 = lo / 100000000;
				lo -= num3 * 100000000;
				_val4 = ToDecHex((int)num3);
			}
			_val3 = ToDecHex((int)lo);
		}

		private unsafe static uint FastToDecHex(int val)
		{
			if (val < 100)
			{
				return *(uint*)((byte*)DecHexDigits + val * 4);
			}
			int num = val * 5243 >> 19;
			return (uint)((*(int*)((byte*)DecHexDigits + num * 4) << 8) | *(int*)((byte*)DecHexDigits + (val - num * 100) * 4));
		}

		private static uint ToDecHex(int val)
		{
			uint num = 0u;
			if (val >= 10000)
			{
				int num2 = val / 10000;
				val -= num2 * 10000;
				num = FastToDecHex(num2) << 16;
			}
			return num | FastToDecHex(val);
		}

		private static int FastDecHexLen(int val)
		{
			if (val < 256)
			{
				if (val < 16)
				{
					return 1;
				}
				return 2;
			}
			if (val < 4096)
			{
				return 3;
			}
			return 4;
		}

		private static int DecHexLen(uint val)
		{
			if (val < 65536)
			{
				return FastDecHexLen((int)val);
			}
			return 4 + FastDecHexLen((int)(val >> 16));
		}

		private int DecHexLen()
		{
			if (_val4 != 0)
			{
				return DecHexLen(_val4) + 24;
			}
			if (_val3 != 0)
			{
				return DecHexLen(_val3) + 16;
			}
			if (_val2 != 0)
			{
				return DecHexLen(_val2) + 8;
			}
			if (_val1 != 0)
			{
				return DecHexLen(_val1);
			}
			return 0;
		}

		private static int ScaleOrder(long hi)
		{
			for (int num = 18; num >= 0; num--)
			{
				if (hi >= GetTenPowerOf(num))
				{
					return num + 1;
				}
			}
			return 1;
		}

		private int InitialFloatingPrecision()
		{
			if (_specifier == 'R')
			{
				return _defPrecision + 2;
			}
			if (_precision < _defPrecision)
			{
				return _defPrecision;
			}
			if (_specifier == 'G')
			{
				return Math.Min(_defPrecision + 2, _precision);
			}
			if (_specifier == 'E')
			{
				return Math.Min(_defPrecision + 2, _precision + 1);
			}
			return _defPrecision;
		}

		private static int ParsePrecision(string format)
		{
			int num = 0;
			for (int i = 1; i < format.Length; i++)
			{
				int num2 = format[i] - 48;
				num = num * 10 + num2;
				if (num2 < 0 || num2 > 9 || num > 99)
				{
					return -2;
				}
			}
			return num;
		}

		private void Init(string format)
		{
			_val1 = (_val2 = (_val3 = (_val4 = 0u)));
			_offset = 0;
			_NaN = (_infinity = false);
			_isCustomFormat = false;
			_specifierIsUpper = true;
			_precision = -1;
			if (format == null || format.Length == 0)
			{
				_specifier = 'G';
				return;
			}
			char c = format[0];
			if (c >= 'a' && c <= 'z')
			{
				c = (char)(c - 97 + 65);
				_specifierIsUpper = false;
			}
			else if (c < 'A' || c > 'Z')
			{
				_isCustomFormat = true;
				_specifier = '0';
				return;
			}
			_specifier = c;
			if (format.Length > 1)
			{
				_precision = ParsePrecision(format);
				if (_precision == -2)
				{
					_isCustomFormat = true;
					_specifier = '0';
					_precision = -1;
				}
			}
		}

		private void InitHex(ulong value)
		{
			switch (_defPrecision)
			{
			case 3:
				value = (byte)value;
				break;
			case 5:
				value = (ushort)value;
				break;
			case 10:
				value = (uint)value;
				break;
			}
			_val1 = (uint)value;
			_val2 = (uint)(value >> 32);
			_decPointPos = (_digitsLen = DecHexLen());
			if (value == 0L)
			{
				_decPointPos = 1;
			}
		}

		private void Init(string format, int value, int defPrecision)
		{
			Init(format);
			_defPrecision = defPrecision;
			_positive = value >= 0;
			if (value == 0 || _specifier == 'X')
			{
				InitHex((ulong)value);
				return;
			}
			if (value < 0)
			{
				value = -value;
			}
			InitDecHexDigits((uint)value);
			_decPointPos = (_digitsLen = DecHexLen());
		}

		private void Init(string format, uint value, int defPrecision)
		{
			Init(format);
			_defPrecision = defPrecision;
			_positive = true;
			if (value == 0 || _specifier == 'X')
			{
				InitHex(value);
				return;
			}
			InitDecHexDigits(value);
			_decPointPos = (_digitsLen = DecHexLen());
		}

		private void Init(string format, long value)
		{
			Init(format);
			_defPrecision = 19;
			_positive = value >= 0;
			if (value == 0L || _specifier == 'X')
			{
				InitHex((ulong)value);
				return;
			}
			if (value < 0)
			{
				value = -value;
			}
			InitDecHexDigits((ulong)value);
			_decPointPos = (_digitsLen = DecHexLen());
		}

		private void Init(string format, ulong value)
		{
			Init(format);
			_defPrecision = 20;
			_positive = true;
			if (value == 0L || _specifier == 'X')
			{
				InitHex(value);
				return;
			}
			InitDecHexDigits(value);
			_decPointPos = (_digitsLen = DecHexLen());
		}

		private unsafe void Init(string format, double value, int defPrecision)
		{
			Init(format);
			_defPrecision = defPrecision;
			long num = BitConverter.DoubleToInt64Bits(value);
			_positive = num >= 0;
			num &= 0x7FFFFFFFFFFFFFFFL;
			if (num == 0L)
			{
				_decPointPos = 1;
				_digitsLen = 0;
				_positive = true;
				return;
			}
			int num2 = (int)(num >> 52);
			long num3 = num & 0xFFFFFFFFFFFFFL;
			if (num2 == 2047)
			{
				_NaN = num3 != 0;
				_infinity = num3 == 0;
				return;
			}
			int num4 = 0;
			if (num2 == 0)
			{
				num2 = 1;
				int num5 = ScaleOrder(num3);
				if (num5 < 15)
				{
					num4 = num5 - 15;
					num3 *= GetTenPowerOf(-num4);
				}
			}
			else
			{
				num3 = (num3 + 4503599627370495L + 1) * 10;
				num4 = -1;
			}
			ulong num6 = (uint)num3;
			ulong num7 = (ulong)num3 >> 32;
			ulong num8 = *(ulong*)((byte*)MantissaBitsTable + num2 * 8);
			ulong num9 = num8 >> 32;
			num8 = (uint)num8;
			ulong num10 = num7 * num8 + num6 * num9 + (num6 * num8 >> 32);
			long num11 = (long)(num7 * num9 + (num10 >> 32));
			while (num11 < 10000000000000000L)
			{
				num10 = (num10 & 0xFFFFFFFFu) * 10;
				num11 = num11 * 10 + (long)(num10 >> 32);
				num4--;
			}
			if ((num10 & 0x80000000u) != 0L)
			{
				num11++;
			}
			int num12 = 17;
			_decPointPos = *(int*)((byte*)TensExponentTable + num2 * 4) + num4 + num12;
			int num13 = InitialFloatingPrecision();
			if (num12 > num13)
			{
				long tenPowerOf = GetTenPowerOf(num12 - num13);
				num11 = (num11 + (tenPowerOf >> 1)) / tenPowerOf;
				num12 = num13;
			}
			if (num11 >= GetTenPowerOf(num12))
			{
				num12++;
				_decPointPos++;
			}
			InitDecHexDigits((ulong)num11);
			_offset = CountTrailingZeros();
			_digitsLen = num12 - _offset;
		}

		private void Init(string format, decimal value)
		{
			Init(format);
			_defPrecision = 100;
			int[] bits = decimal.GetBits(value);
			int num = (bits[3] & 0x1F0000) >> 16;
			_positive = bits[3] >= 0;
			if (bits[0] == 0 && bits[1] == 0 && bits[2] == 0)
			{
				_decPointPos = -num;
				_positive = true;
				_digitsLen = 0;
				return;
			}
			InitDecHexDigits((uint)bits[2], (ulong)(((long)bits[1] << 32) | (uint)bits[0]));
			_digitsLen = DecHexLen();
			_decPointPos = _digitsLen - num;
			if (_precision != -1 || _specifier != 'G')
			{
				_offset = CountTrailingZeros();
				_digitsLen -= _offset;
			}
		}

		private void ResetCharBuf(int size)
		{
			_ind = 0;
			if (_cbuf.Length < size)
			{
				_cbuf = new char[size];
			}
		}

		private void Resize(int len)
		{
			char[] array = new char[len];
			Array.Copy(_cbuf, array, _ind);
			_cbuf = array;
		}

		private void Append(char c)
		{
			if (_ind == _cbuf.Length)
			{
				Resize(_ind + 10);
			}
			_cbuf[_ind++] = c;
		}

		private void Append(char c, int cnt)
		{
			if (_ind + cnt > _cbuf.Length)
			{
				Resize(_ind + cnt + 10);
			}
			while (cnt-- > 0)
			{
				_cbuf[_ind++] = c;
			}
		}

		private void Append(string s)
		{
			int length = s.Length;
			if (_ind + length > _cbuf.Length)
			{
				Resize(_ind + length + 10);
			}
			for (int i = 0; i < length; i++)
			{
				_cbuf[_ind++] = s[i];
			}
		}

		private NumberFormatInfo GetNumberFormatInstance(IFormatProvider fp)
		{
			if (_nfi != null && fp == null)
			{
				return _nfi;
			}
			return NumberFormatInfo.GetInstance(fp);
		}

		private void RoundPos(int pos)
		{
			RoundBits(_digitsLen - pos);
		}

		private bool RoundDecimal(int decimals)
		{
			return RoundBits(_digitsLen - _decPointPos - decimals);
		}

		private bool RoundBits(int shift)
		{
			if (shift <= 0)
			{
				return false;
			}
			if (shift > _digitsLen)
			{
				_digitsLen = 0;
				_decPointPos = 1;
				_val1 = (_val2 = (_val3 = (_val4 = 0u)));
				_positive = true;
				return false;
			}
			shift += _offset;
			_digitsLen += _offset;
			while (shift > 8)
			{
				_val1 = _val2;
				_val2 = _val3;
				_val3 = _val4;
				_val4 = 0u;
				_digitsLen -= 8;
				shift -= 8;
			}
			shift = shift - 1 << 2;
			uint num = _val1 >> shift;
			uint num2 = num & 0xF;
			_val1 = (num ^ num2) << shift;
			bool result = false;
			if (num2 >= 5)
			{
				_val1 |= 2576980377u >> 28 - shift;
				AddOneToDecHex();
				int num3 = DecHexLen();
				result = num3 != _digitsLen;
				_decPointPos = _decPointPos + num3 - _digitsLen;
				_digitsLen = num3;
			}
			RemoveTrailingZeros();
			return result;
		}

		private void RemoveTrailingZeros()
		{
			_offset = CountTrailingZeros();
			_digitsLen -= _offset;
			if (_digitsLen == 0)
			{
				_offset = 0;
				_decPointPos = 1;
				_positive = true;
			}
		}

		private void AddOneToDecHex()
		{
			if (_val1 == 2576980377u)
			{
				_val1 = 0u;
				if (_val2 == 2576980377u)
				{
					_val2 = 0u;
					if (_val3 == 2576980377u)
					{
						_val3 = 0u;
						_val4 = AddOneToDecHex(_val4);
					}
					else
					{
						_val3 = AddOneToDecHex(_val3);
					}
				}
				else
				{
					_val2 = AddOneToDecHex(_val2);
				}
			}
			else
			{
				_val1 = AddOneToDecHex(_val1);
			}
		}

		private static uint AddOneToDecHex(uint val)
		{
			if ((val & 0xFFFF) == 39321)
			{
				if ((val & 0xFFFFFF) == 10066329)
				{
					if ((val & 0xFFFFFFF) == 161061273)
					{
						return val + 107374183;
					}
					return val + 6710887;
				}
				if ((val & 0xFFFFF) == 629145)
				{
					return val + 419431;
				}
				return val + 26215;
			}
			if ((val & 0xFF) == 153)
			{
				if ((val & 0xFFF) == 2457)
				{
					return val + 1639;
				}
				return val + 103;
			}
			if ((val & 0xF) == 9)
			{
				return val + 7;
			}
			return val + 1;
		}

		private int CountTrailingZeros()
		{
			if (_val1 != 0)
			{
				return CountTrailingZeros(_val1);
			}
			if (_val2 != 0)
			{
				return CountTrailingZeros(_val2) + 8;
			}
			if (_val3 != 0)
			{
				return CountTrailingZeros(_val3) + 16;
			}
			if (_val4 != 0)
			{
				return CountTrailingZeros(_val4) + 24;
			}
			return _digitsLen;
		}

		private static int CountTrailingZeros(uint val)
		{
			if ((val & 0xFFFF) == 0)
			{
				if ((val & 0xFFFFFF) == 0)
				{
					if ((val & 0xFFFFFFF) == 0)
					{
						return 7;
					}
					return 6;
				}
				if ((val & 0xFFFFF) == 0)
				{
					return 5;
				}
				return 4;
			}
			if ((val & 0xFF) == 0)
			{
				if ((val & 0xFFF) == 0)
				{
					return 3;
				}
				return 2;
			}
			if ((val & 0xF) == 0)
			{
				return 1;
			}
			return 0;
		}

		private static NumberFormatter GetInstance()
		{
			NumberFormatter numberFormatter = threadNumberFormatter;
			threadNumberFormatter = null;
			if (numberFormatter == null)
			{
				return new NumberFormatter(Thread.CurrentThread);
			}
			return numberFormatter;
		}

		private void Release()
		{
			threadNumberFormatter = this;
		}

		internal static void SetThreadCurrentCulture(CultureInfo culture)
		{
			if (threadNumberFormatter != null)
			{
				threadNumberFormatter.CurrentCulture = culture;
			}
		}

		public static string NumberToString(string format, sbyte value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value, 3);
			string result = instance.IntegerToString(format, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, byte value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value, 3);
			string result = instance.IntegerToString(format, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, ushort value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value, 5);
			string result = instance.IntegerToString(format, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, short value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value, 5);
			string result = instance.IntegerToString(format, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, uint value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value, 10);
			string result = instance.IntegerToString(format, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, int value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value, 10);
			string result = instance.IntegerToString(format, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, ulong value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value);
			string result = instance.IntegerToString(format, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, long value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value);
			string result = instance.IntegerToString(format, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, float value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value, 7);
			NumberFormatInfo numberFormatInstance = instance.GetNumberFormatInstance(fp);
			string result = (instance._NaN ? numberFormatInstance.NaNSymbol : (instance._infinity ? ((!instance._positive) ? numberFormatInstance.NegativeInfinitySymbol : numberFormatInstance.PositiveInfinitySymbol) : ((instance._specifier != 'R') ? instance.NumberToString(format, numberFormatInstance) : instance.FormatRoundtrip(value, numberFormatInstance))));
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, double value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value, 15);
			NumberFormatInfo numberFormatInstance = instance.GetNumberFormatInstance(fp);
			string result = (instance._NaN ? numberFormatInstance.NaNSymbol : (instance._infinity ? ((!instance._positive) ? numberFormatInstance.NegativeInfinitySymbol : numberFormatInstance.PositiveInfinitySymbol) : ((instance._specifier != 'R') ? instance.NumberToString(format, numberFormatInstance) : instance.FormatRoundtrip(value, numberFormatInstance))));
			instance.Release();
			return result;
		}

		public static string NumberToString(string format, decimal value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(format, value);
			string result = instance.NumberToString(format, instance.GetNumberFormatInstance(fp));
			instance.Release();
			return result;
		}

		public static string NumberToString(uint value, IFormatProvider fp)
		{
			if (value >= 100000000)
			{
				return NumberToString(null, value, fp);
			}
			NumberFormatter instance = GetInstance();
			string result = instance.FastIntegerToString((int)value, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(int value, IFormatProvider fp)
		{
			if (value >= 100000000 || value <= -100000000)
			{
				return NumberToString(null, value, fp);
			}
			NumberFormatter instance = GetInstance();
			string result = instance.FastIntegerToString(value, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(ulong value, IFormatProvider fp)
		{
			if (value >= 100000000)
			{
				return NumberToString(null, value, fp);
			}
			NumberFormatter instance = GetInstance();
			string result = instance.FastIntegerToString((int)value, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(long value, IFormatProvider fp)
		{
			if (value >= 100000000 || value <= -100000000)
			{
				return NumberToString(null, value, fp);
			}
			NumberFormatter instance = GetInstance();
			string result = instance.FastIntegerToString((int)value, fp);
			instance.Release();
			return result;
		}

		public static string NumberToString(float value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			instance.Init(null, value, 7);
			NumberFormatInfo numberFormatInstance = instance.GetNumberFormatInstance(fp);
			string result = (instance._NaN ? numberFormatInstance.NaNSymbol : ((!instance._infinity) ? instance.FormatGeneral(-1, numberFormatInstance) : ((!instance._positive) ? numberFormatInstance.NegativeInfinitySymbol : numberFormatInstance.PositiveInfinitySymbol)));
			instance.Release();
			return result;
		}

		public static string NumberToString(double value, IFormatProvider fp)
		{
			NumberFormatter instance = GetInstance();
			NumberFormatInfo numberFormatInstance = instance.GetNumberFormatInstance(fp);
			instance.Init(null, value, 15);
			string result = (instance._NaN ? numberFormatInstance.NaNSymbol : ((!instance._infinity) ? instance.FormatGeneral(-1, numberFormatInstance) : ((!instance._positive) ? numberFormatInstance.NegativeInfinitySymbol : numberFormatInstance.PositiveInfinitySymbol)));
			instance.Release();
			return result;
		}

		private string FastIntegerToString(int value, IFormatProvider fp)
		{
			if (value < 0)
			{
				string negativeSign = GetNumberFormatInstance(fp).NegativeSign;
				ResetCharBuf(8 + negativeSign.Length);
				value = -value;
				Append(negativeSign);
			}
			else
			{
				ResetCharBuf(8);
			}
			if (value >= 10000)
			{
				int num = value / 10000;
				FastAppendDigits(num, false);
				FastAppendDigits(value - num * 10000, true);
			}
			else
			{
				FastAppendDigits(value, false);
			}
			return new string(_cbuf, 0, _ind);
		}

		private string IntegerToString(string format, IFormatProvider fp)
		{
			NumberFormatInfo numberFormatInstance = GetNumberFormatInstance(fp);
			switch (_specifier)
			{
			case 'C':
				return FormatCurrency(_precision, numberFormatInstance);
			case 'D':
				return FormatDecimal(_precision, numberFormatInstance);
			case 'E':
				return FormatExponential(_precision, numberFormatInstance);
			case 'F':
				return FormatFixedPoint(_precision, numberFormatInstance);
			case 'G':
				if (_precision <= 0)
				{
					return FormatDecimal(-1, numberFormatInstance);
				}
				return FormatGeneral(_precision, numberFormatInstance);
			case 'N':
				return FormatNumber(_precision, numberFormatInstance);
			case 'P':
				return FormatPercent(_precision, numberFormatInstance);
			case 'X':
				return FormatHexadecimal(_precision);
			default:
				if (_isCustomFormat)
				{
					return FormatCustom(format, numberFormatInstance);
				}
				throw new FormatException("The specified format '" + format + "' is invalid");
			}
		}

		private string NumberToString(string format, NumberFormatInfo nfi)
		{
			switch (_specifier)
			{
			case 'C':
				return FormatCurrency(_precision, nfi);
			case 'E':
				return FormatExponential(_precision, nfi);
			case 'F':
				return FormatFixedPoint(_precision, nfi);
			case 'G':
				return FormatGeneral(_precision, nfi);
			case 'N':
				return FormatNumber(_precision, nfi);
			case 'P':
				return FormatPercent(_precision, nfi);
			default:
				if (_isCustomFormat)
				{
					return FormatCustom(format, nfi);
				}
				throw new FormatException("The specified format '" + format + "' is invalid");
			}
		}

		public string FormatCurrency(int precision, NumberFormatInfo nfi)
		{
			precision = ((precision < 0) ? nfi.CurrencyDecimalDigits : precision);
			RoundDecimal(precision);
			ResetCharBuf(IntegerDigits * 2 + precision * 2 + 16);
			if (_positive)
			{
				switch (nfi.CurrencyPositivePattern)
				{
				case 0:
					Append(nfi.CurrencySymbol);
					break;
				case 2:
					Append(nfi.CurrencySymbol);
					Append(' ');
					break;
				}
			}
			else
			{
				switch (nfi.CurrencyNegativePattern)
				{
				case 0:
					Append('(');
					Append(nfi.CurrencySymbol);
					break;
				case 1:
					Append(nfi.NegativeSign);
					Append(nfi.CurrencySymbol);
					break;
				case 2:
					Append(nfi.CurrencySymbol);
					Append(nfi.NegativeSign);
					break;
				case 3:
					Append(nfi.CurrencySymbol);
					break;
				case 4:
					Append('(');
					break;
				case 5:
					Append(nfi.NegativeSign);
					break;
				case 8:
					Append(nfi.NegativeSign);
					break;
				case 9:
					Append(nfi.NegativeSign);
					Append(nfi.CurrencySymbol);
					Append(' ');
					break;
				case 11:
					Append(nfi.CurrencySymbol);
					Append(' ');
					break;
				case 12:
					Append(nfi.CurrencySymbol);
					Append(' ');
					Append(nfi.NegativeSign);
					break;
				case 14:
					Append('(');
					Append(nfi.CurrencySymbol);
					Append(' ');
					break;
				case 15:
					Append('(');
					break;
				}
			}
			AppendIntegerStringWithGroupSeparator(nfi.RawCurrencyGroupSizes, nfi.CurrencyGroupSeparator);
			if (precision > 0)
			{
				Append(nfi.CurrencyDecimalSeparator);
				AppendDecimalString(precision);
			}
			if (_positive)
			{
				switch (nfi.CurrencyPositivePattern)
				{
				case 1:
					Append(nfi.CurrencySymbol);
					break;
				case 3:
					Append(' ');
					Append(nfi.CurrencySymbol);
					break;
				}
			}
			else
			{
				switch (nfi.CurrencyNegativePattern)
				{
				case 0:
					Append(')');
					break;
				case 3:
					Append(nfi.NegativeSign);
					break;
				case 4:
					Append(nfi.CurrencySymbol);
					Append(')');
					break;
				case 5:
					Append(nfi.CurrencySymbol);
					break;
				case 6:
					Append(nfi.NegativeSign);
					Append(nfi.CurrencySymbol);
					break;
				case 7:
					Append(nfi.CurrencySymbol);
					Append(nfi.NegativeSign);
					break;
				case 8:
					Append(' ');
					Append(nfi.CurrencySymbol);
					break;
				case 10:
					Append(' ');
					Append(nfi.CurrencySymbol);
					Append(nfi.NegativeSign);
					break;
				case 11:
					Append(nfi.NegativeSign);
					break;
				case 13:
					Append(nfi.NegativeSign);
					Append(' ');
					Append(nfi.CurrencySymbol);
					break;
				case 14:
					Append(')');
					break;
				case 15:
					Append(' ');
					Append(nfi.CurrencySymbol);
					Append(')');
					break;
				}
			}
			return new string(_cbuf, 0, _ind);
		}

		private string FormatDecimal(int precision, NumberFormatInfo nfi)
		{
			if (precision < _digitsLen)
			{
				precision = _digitsLen;
			}
			if (precision == 0)
			{
				return "0";
			}
			ResetCharBuf(precision + 1);
			if (!_positive)
			{
				Append(nfi.NegativeSign);
			}
			AppendDigits(0, precision);
			return new string(_cbuf, 0, _ind);
		}

		private unsafe string FormatHexadecimal(int precision)
		{
			int num = Math.Max(precision, _decPointPos);
			char* ptr = ((!_specifierIsUpper) ? DigitLowerTable : DigitUpperTable);
			ResetCharBuf(num);
			_ind = num;
			ulong num2 = _val1 | ((ulong)_val2 << 32);
			while (num > 0)
			{
				_cbuf[--num] = ptr[num2 & 0xF];
				num2 >>= 4;
			}
			return new string(_cbuf, 0, _ind);
		}

		public string FormatFixedPoint(int precision, NumberFormatInfo nfi)
		{
			if (precision == -1)
			{
				precision = nfi.NumberDecimalDigits;
			}
			RoundDecimal(precision);
			ResetCharBuf(IntegerDigits + precision + 2);
			if (!_positive)
			{
				Append(nfi.NegativeSign);
			}
			AppendIntegerString(IntegerDigits);
			if (precision > 0)
			{
				Append(nfi.NumberDecimalSeparator);
				AppendDecimalString(precision);
			}
			return new string(_cbuf, 0, _ind);
		}

		private string FormatRoundtrip(double origval, NumberFormatInfo nfi)
		{
			NumberFormatter clone = GetClone();
			if (origval >= -1.79769313486231E+308 && origval <= 1.79769313486231E+308)
			{
				string text = FormatGeneral(_defPrecision, nfi);
				if (origval == double.Parse(text, nfi))
				{
					return text;
				}
			}
			return clone.FormatGeneral(_defPrecision + 2, nfi);
		}

		private string FormatRoundtrip(float origval, NumberFormatInfo nfi)
		{
			NumberFormatter clone = GetClone();
			string text = FormatGeneral(_defPrecision, nfi);
			if (origval == float.Parse(text, nfi))
			{
				return text;
			}
			return clone.FormatGeneral(_defPrecision + 2, nfi);
		}

		private string FormatGeneral(int precision, NumberFormatInfo nfi)
		{
			bool flag;
			if (precision == -1)
			{
				flag = IsFloatingSource;
				precision = _defPrecision;
			}
			else
			{
				flag = true;
				if (precision == 0)
				{
					precision = _defPrecision;
				}
				RoundPos(precision);
			}
			int num = _decPointPos;
			int digitsLen = _digitsLen;
			int num2 = digitsLen - num;
			if ((num > precision || num <= -4) && flag)
			{
				return FormatExponential(digitsLen - 1, nfi, 2);
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num < 0)
			{
				num = 0;
			}
			ResetCharBuf(num2 + num + 3);
			if (!_positive)
			{
				Append(nfi.NegativeSign);
			}
			if (num == 0)
			{
				Append('0');
			}
			else
			{
				AppendDigits(digitsLen - num, digitsLen);
			}
			if (num2 > 0)
			{
				Append(nfi.NumberDecimalSeparator);
				AppendDigits(0, num2);
			}
			return new string(_cbuf, 0, _ind);
		}

		public string FormatNumber(int precision, NumberFormatInfo nfi)
		{
			precision = ((precision < 0) ? nfi.NumberDecimalDigits : precision);
			ResetCharBuf(IntegerDigits * 3 + precision);
			RoundDecimal(precision);
			if (!_positive)
			{
				switch (nfi.NumberNegativePattern)
				{
				case 0:
					Append('(');
					break;
				case 1:
					Append(nfi.NegativeSign);
					break;
				case 2:
					Append(nfi.NegativeSign);
					Append(' ');
					break;
				}
			}
			AppendIntegerStringWithGroupSeparator(nfi.RawNumberGroupSizes, nfi.NumberGroupSeparator);
			if (precision > 0)
			{
				Append(nfi.NumberDecimalSeparator);
				AppendDecimalString(precision);
			}
			if (!_positive)
			{
				switch (nfi.NumberNegativePattern)
				{
				case 0:
					Append(')');
					break;
				case 3:
					Append(nfi.NegativeSign);
					break;
				case 4:
					Append(' ');
					Append(nfi.NegativeSign);
					break;
				}
			}
			return new string(_cbuf, 0, _ind);
		}

		public string FormatPercent(int precision, NumberFormatInfo nfi)
		{
			precision = ((precision < 0) ? nfi.PercentDecimalDigits : precision);
			Multiply10(2);
			RoundDecimal(precision);
			ResetCharBuf(IntegerDigits * 2 + precision + 16);
			if (_positive)
			{
				if (nfi.PercentPositivePattern == 2)
				{
					Append(nfi.PercentSymbol);
				}
			}
			else
			{
				switch (nfi.PercentNegativePattern)
				{
				case 0:
					Append(nfi.NegativeSign);
					break;
				case 1:
					Append(nfi.NegativeSign);
					break;
				case 2:
					Append(nfi.NegativeSign);
					Append(nfi.PercentSymbol);
					break;
				}
			}
			AppendIntegerStringWithGroupSeparator(nfi.RawPercentGroupSizes, nfi.PercentGroupSeparator);
			if (precision > 0)
			{
				Append(nfi.PercentDecimalSeparator);
				AppendDecimalString(precision);
			}
			if (_positive)
			{
				switch (nfi.PercentPositivePattern)
				{
				case 0:
					Append(' ');
					Append(nfi.PercentSymbol);
					break;
				case 1:
					Append(nfi.PercentSymbol);
					break;
				}
			}
			else
			{
				switch (nfi.PercentNegativePattern)
				{
				case 0:
					Append(' ');
					Append(nfi.PercentSymbol);
					break;
				case 1:
					Append(nfi.PercentSymbol);
					break;
				}
			}
			return new string(_cbuf, 0, _ind);
		}

		public string FormatExponential(int precision, NumberFormatInfo nfi)
		{
			if (precision == -1)
			{
				precision = 6;
			}
			RoundPos(precision + 1);
			return FormatExponential(precision, nfi, 3);
		}

		private string FormatExponential(int precision, NumberFormatInfo nfi, int expDigits)
		{
			int decPointPos = _decPointPos;
			int digitsLen = _digitsLen;
			int exponent = decPointPos - 1;
			decPointPos = (_decPointPos = 1);
			ResetCharBuf(precision + 8);
			if (!_positive)
			{
				Append(nfi.NegativeSign);
			}
			AppendOneDigit(digitsLen - 1);
			if (precision > 0)
			{
				Append(nfi.NumberDecimalSeparator);
				AppendDigits(digitsLen - precision - 1, digitsLen - _decPointPos);
			}
			AppendExponent(nfi, exponent, expDigits);
			return new string(_cbuf, 0, _ind);
		}

		public string FormatCustom(string format, NumberFormatInfo nfi)
		{
			bool positive = _positive;
			int offset = 0;
			int length = 0;
			CustomInfo.GetActiveSection(format, ref positive, IsZero, ref offset, ref length);
			if (length == 0)
			{
				return (!_positive) ? nfi.NegativeSign : string.Empty;
			}
			_positive = positive;
			CustomInfo customInfo = CustomInfo.Parse(format, offset, length, nfi);
			StringBuilder stringBuilder = new StringBuilder(customInfo.IntegerDigits * 2);
			StringBuilder stringBuilder2 = new StringBuilder(customInfo.DecimalDigits * 2);
			StringBuilder stringBuilder3 = ((!customInfo.UseExponent) ? null : new StringBuilder(customInfo.ExponentDigits * 2));
			int num = 0;
			if (customInfo.Percents > 0)
			{
				Multiply10(2 * customInfo.Percents);
			}
			if (customInfo.Permilles > 0)
			{
				Multiply10(3 * customInfo.Permilles);
			}
			if (customInfo.DividePlaces > 0)
			{
				Divide10(customInfo.DividePlaces);
			}
			bool flag = true;
			if (customInfo.UseExponent && (customInfo.DecimalDigits > 0 || customInfo.IntegerDigits > 0))
			{
				if (!IsZero)
				{
					RoundPos(customInfo.DecimalDigits + customInfo.IntegerDigits);
					num -= _decPointPos - customInfo.IntegerDigits;
					_decPointPos = customInfo.IntegerDigits;
				}
				flag = num <= 0;
				AppendNonNegativeNumber(stringBuilder3, (num >= 0) ? num : (-num));
			}
			else
			{
				RoundDecimal(customInfo.DecimalDigits);
			}
			if (customInfo.IntegerDigits != 0 || !IsZeroInteger)
			{
				AppendIntegerString(IntegerDigits, stringBuilder);
			}
			AppendDecimalString(DecimalDigits, stringBuilder2);
			if (customInfo.UseExponent)
			{
				if (customInfo.DecimalDigits <= 0 && customInfo.IntegerDigits <= 0)
				{
					_positive = true;
				}
				if (stringBuilder.Length < customInfo.IntegerDigits)
				{
					stringBuilder.Insert(0, "0", customInfo.IntegerDigits - stringBuilder.Length);
				}
				while (stringBuilder3.Length < customInfo.ExponentDigits - customInfo.ExponentTailSharpDigits)
				{
					stringBuilder3.Insert(0, '0');
				}
				if (flag && !customInfo.ExponentNegativeSignOnly)
				{
					stringBuilder3.Insert(0, nfi.PositiveSign);
				}
				else if (!flag)
				{
					stringBuilder3.Insert(0, nfi.NegativeSign);
				}
			}
			else
			{
				if (stringBuilder.Length < customInfo.IntegerDigits - customInfo.IntegerHeadSharpDigits)
				{
					stringBuilder.Insert(0, "0", customInfo.IntegerDigits - customInfo.IntegerHeadSharpDigits - stringBuilder.Length);
				}
				if (customInfo.IntegerDigits == customInfo.IntegerHeadSharpDigits && IsZeroOnly(stringBuilder))
				{
					stringBuilder.Remove(0, stringBuilder.Length);
				}
			}
			ZeroTrimEnd(stringBuilder2, true);
			while (stringBuilder2.Length < customInfo.DecimalDigits - customInfo.DecimalTailSharpDigits)
			{
				stringBuilder2.Append('0');
			}
			if (stringBuilder2.Length > customInfo.DecimalDigits)
			{
				stringBuilder2.Remove(customInfo.DecimalDigits, stringBuilder2.Length - customInfo.DecimalDigits);
			}
			return customInfo.Format(format, offset, length, nfi, _positive, stringBuilder, stringBuilder2, stringBuilder3);
		}

		private static void ZeroTrimEnd(StringBuilder sb, bool canEmpty)
		{
			int num = 0;
			int num2 = sb.Length - 1;
			while (((!canEmpty) ? (num2 > 0) : (num2 >= 0)) && sb[num2] == '0')
			{
				num++;
				num2--;
			}
			if (num > 0)
			{
				sb.Remove(sb.Length - num, num);
			}
		}

		private static bool IsZeroOnly(StringBuilder sb)
		{
			for (int i = 0; i < sb.Length; i++)
			{
				if (char.IsDigit(sb[i]) && sb[i] != '0')
				{
					return false;
				}
			}
			return true;
		}

		private static void AppendNonNegativeNumber(StringBuilder sb, int v)
		{
			if (v < 0)
			{
				throw new ArgumentException();
			}
			int num = ScaleOrder(v) - 1;
			do
			{
				int num2 = v / (int)GetTenPowerOf(num);
				sb.Append((char)(0x30 | num2));
				v -= (int)GetTenPowerOf(num--) * num2;
			}
			while (num >= 0);
		}

		private void AppendIntegerString(int minLength, StringBuilder sb)
		{
			if (_decPointPos <= 0)
			{
				sb.Append('0', minLength);
				return;
			}
			if (_decPointPos < minLength)
			{
				sb.Append('0', minLength - _decPointPos);
			}
			AppendDigits(_digitsLen - _decPointPos, _digitsLen, sb);
		}

		private void AppendIntegerString(int minLength)
		{
			if (_decPointPos <= 0)
			{
				Append('0', minLength);
				return;
			}
			if (_decPointPos < minLength)
			{
				Append('0', minLength - _decPointPos);
			}
			AppendDigits(_digitsLen - _decPointPos, _digitsLen);
		}

		private void AppendDecimalString(int precision, StringBuilder sb)
		{
			AppendDigits(_digitsLen - precision - _decPointPos, _digitsLen - _decPointPos, sb);
		}

		private void AppendDecimalString(int precision)
		{
			AppendDigits(_digitsLen - precision - _decPointPos, _digitsLen - _decPointPos);
		}

		private void AppendIntegerStringWithGroupSeparator(int[] groups, string groupSeparator)
		{
			if (IsZeroInteger)
			{
				Append('0');
				return;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < groups.Length; i++)
			{
				num += groups[i];
				if (num <= _decPointPos)
				{
					num2 = i;
					continue;
				}
				break;
			}
			if (groups.Length > 0 && num > 0)
			{
				int num3 = groups[num2];
				int num4 = ((_decPointPos > num) ? (_decPointPos - num) : 0);
				if (num3 == 0)
				{
					while (num2 >= 0 && groups[num2] == 0)
					{
						num2--;
					}
					num3 = ((num4 <= 0) ? groups[num2] : num4);
				}
				int num5;
				if (num4 == 0)
				{
					num5 = num3;
				}
				else
				{
					num2 += num4 / num3;
					num5 = num4 % num3;
					if (num5 == 0)
					{
						num5 = num3;
					}
					else
					{
						num2++;
					}
				}
				int num6 = 0;
				while (_decPointPos - num6 > num5 && num5 != 0)
				{
					AppendDigits(_digitsLen - num6 - num5, _digitsLen - num6);
					num6 += num5;
					Append(groupSeparator);
					if (--num2 < groups.Length && num2 >= 0)
					{
						num3 = groups[num2];
					}
					num5 = num3;
				}
				AppendDigits(_digitsLen - _decPointPos, _digitsLen - num6);
			}
			else
			{
				AppendDigits(_digitsLen - _decPointPos, _digitsLen);
			}
		}

		private void AppendExponent(NumberFormatInfo nfi, int exponent, int minDigits)
		{
			if (_specifierIsUpper || _specifier == 'R')
			{
				Append('E');
			}
			else
			{
				Append('e');
			}
			if (exponent >= 0)
			{
				Append(nfi.PositiveSign);
			}
			else
			{
				Append(nfi.NegativeSign);
				exponent = -exponent;
			}
			if (exponent == 0)
			{
				Append('0', minDigits);
				return;
			}
			if (exponent < 10)
			{
				Append('0', minDigits - 1);
				Append((char)(0x30 | exponent));
				return;
			}
			uint num = FastToDecHex(exponent);
			if (exponent >= 100 || minDigits == 3)
			{
				Append((char)(0x30 | (num >> 8)));
			}
			Append((char)(0x30 | ((num >> 4) & 0xF)));
			Append((char)(0x30 | (num & 0xF)));
		}

		private void AppendOneDigit(int start)
		{
			if (_ind == _cbuf.Length)
			{
				Resize(_ind + 10);
			}
			start += _offset;
			uint num = ((start >= 0) ? ((start < 8) ? _val1 : ((start < 16) ? _val2 : ((start < 24) ? _val3 : ((start < 32) ? _val4 : 0u)))) : 0u);
			num >>= (start & 7) << 2;
			_cbuf[_ind++] = (char)(0x30 | (num & 0xF));
		}

		private unsafe void FastAppendDigits(int val, bool force)
		{
			int ind = _ind;
			int num2;
			if (force || val >= 100)
			{
				int num = val * 5243 >> 19;
				num2 = *(int*)((byte*)DecHexDigits + num * 4);
				if (force || val >= 1000)
				{
					_cbuf[ind++] = (char)(0x30 | (num2 >> 4));
				}
				_cbuf[ind++] = (char)(0x30 | (num2 & 0xF));
				num2 = *(int*)((byte*)DecHexDigits + (val - num * 100) * 4);
			}
			else
			{
				num2 = *(int*)((byte*)DecHexDigits + val * 4);
			}
			if (force || val >= 10)
			{
				_cbuf[ind++] = (char)(0x30 | (num2 >> 4));
			}
			_cbuf[ind++] = (char)(0x30 | (num2 & 0xF));
			_ind = ind;
		}

		private void AppendDigits(int start, int end)
		{
			if (start >= end)
			{
				return;
			}
			int num = _ind + (end - start);
			if (num > _cbuf.Length)
			{
				Resize(num + 10);
			}
			_ind = num;
			end += _offset;
			start += _offset;
			int num2 = start + 8 - (start & 7);
			while (true)
			{
				uint num3;
				switch (num2)
				{
				case 8:
					num3 = _val1;
					break;
				case 16:
					num3 = _val2;
					break;
				case 24:
					num3 = _val3;
					break;
				case 32:
					num3 = _val4;
					break;
				default:
					num3 = 0u;
					break;
				}
				num3 >>= (start & 7) << 2;
				if (num2 > end)
				{
					num2 = end;
				}
				_cbuf[--num] = (char)(0x30 | (num3 & 0xF));
				switch (num2 - start)
				{
				case 8:
					_cbuf[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 7;
				case 7:
					_cbuf[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 6;
				case 6:
					_cbuf[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 5;
				case 5:
					_cbuf[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 4;
				case 4:
					_cbuf[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 3;
				case 3:
					_cbuf[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 2;
				case 2:
					_cbuf[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					break;
				case 1:
					break;
				default:
					goto IL_01d5;
				}
				if (num2 == end)
				{
					break;
				}
				goto IL_01d5;
				IL_01d5:
				start = num2;
				num2 += 8;
			}
		}

		private void AppendDigits(int start, int end, StringBuilder sb)
		{
			if (start >= end)
			{
				return;
			}
			int num = (sb.Length += end - start);
			end += _offset;
			start += _offset;
			int num2 = start + 8 - (start & 7);
			while (true)
			{
				uint num3;
				switch (num2)
				{
				case 8:
					num3 = _val1;
					break;
				case 16:
					num3 = _val2;
					break;
				case 24:
					num3 = _val3;
					break;
				case 32:
					num3 = _val4;
					break;
				default:
					num3 = 0u;
					break;
				}
				num3 >>= (start & 7) << 2;
				if (num2 > end)
				{
					num2 = end;
				}
				sb[--num] = (char)(0x30 | (num3 & 0xF));
				switch (num2 - start)
				{
				case 8:
					sb[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 7;
				case 7:
					sb[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 6;
				case 6:
					sb[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 5;
				case 5:
					sb[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 4;
				case 4:
					sb[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 3;
				case 3:
					sb[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					goto case 2;
				case 2:
					sb[--num] = (char)(0x30 | ((num3 >>= 4) & 0xF));
					break;
				case 1:
					break;
				default:
					goto IL_01b5;
				}
				if (num2 == end)
				{
					break;
				}
				goto IL_01b5;
				IL_01b5:
				start = num2;
				num2 += 8;
			}
		}

		private void Multiply10(int count)
		{
			if (count > 0 && _digitsLen != 0)
			{
				_decPointPos += count;
			}
		}

		private void Divide10(int count)
		{
			if (count > 0 && _digitsLen != 0)
			{
				_decPointPos -= count;
			}
		}

		private NumberFormatter GetClone()
		{
			return (NumberFormatter)MemberwiseClone();
		}
	}
}
