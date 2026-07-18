using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Mono.Security;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct Guid : IFormattable, IComparable, IComparable<Guid>, IEquatable<Guid>
	{
		internal class GuidParser
		{
			private string _src;

			private int _length;

			private int _cur;

			public GuidParser(string src)
			{
				_src = src;
				Reset();
			}

			private void Reset()
			{
				_cur = 0;
				_length = _src.Length;
			}

			private bool AtEnd()
			{
				return _cur >= _length;
			}

			private void ThrowFormatException()
			{
				throw new FormatException(Locale.GetText("Invalid format for Guid.Guid(string)."));
			}

			private ulong ParseHex(int length, bool strictLength)
			{
				ulong num = 0uL;
				bool flag = false;
				int num2 = 0;
				while (!flag && num2 < length)
				{
					if (AtEnd())
					{
						if (strictLength || num2 == 0)
						{
							ThrowFormatException();
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						char c = char.ToLowerInvariant(_src[_cur]);
						if (char.IsDigit(c))
						{
							num = num * 16 + c - 48;
							_cur++;
						}
						else if (c >= 'a' && c <= 'f')
						{
							num = num * 16 + c - 97 + 10;
							_cur++;
						}
						else if (strictLength || num2 == 0)
						{
							ThrowFormatException();
						}
						else
						{
							flag = true;
						}
					}
					num2++;
				}
				return num;
			}

			private bool ParseOptChar(char c)
			{
				if (!AtEnd() && _src[_cur] == c)
				{
					_cur++;
					return true;
				}
				return false;
			}

			private void ParseChar(char c)
			{
				if (!ParseOptChar(c))
				{
					ThrowFormatException();
				}
			}

			private Guid ParseGuid1()
			{
				bool flag = true;
				char c = '}';
				byte[] array = new byte[8];
				bool flag2 = ParseOptChar('{');
				if (!flag2)
				{
					flag2 = ParseOptChar('(');
					if (flag2)
					{
						c = ')';
					}
				}
				int a = (int)ParseHex(8, true);
				if (flag2)
				{
					ParseChar('-');
				}
				else
				{
					flag = ParseOptChar('-');
				}
				short b = (short)ParseHex(4, true);
				if (flag)
				{
					ParseChar('-');
				}
				short c2 = (short)ParseHex(4, true);
				if (flag)
				{
					ParseChar('-');
				}
				for (int i = 0; i < 8; i++)
				{
					array[i] = (byte)ParseHex(2, true);
					if (i == 1 && flag)
					{
						ParseChar('-');
					}
				}
				if (flag2 && !ParseOptChar(c))
				{
					ThrowFormatException();
				}
				return new Guid(a, b, c2, array);
			}

			private void ParseHexPrefix()
			{
				ParseChar('0');
				ParseChar('x');
			}

			private Guid ParseGuid2()
			{
				byte[] array = new byte[8];
				ParseChar('{');
				ParseHexPrefix();
				int a = (int)ParseHex(8, false);
				ParseChar(',');
				ParseHexPrefix();
				short b = (short)ParseHex(4, false);
				ParseChar(',');
				ParseHexPrefix();
				short c = (short)ParseHex(4, false);
				ParseChar(',');
				ParseChar('{');
				for (int i = 0; i < 8; i++)
				{
					ParseHexPrefix();
					array[i] = (byte)ParseHex(2, false);
					if (i != 7)
					{
						ParseChar(',');
					}
				}
				ParseChar('}');
				ParseChar('}');
				return new Guid(a, b, c, array);
			}

			public Guid Parse()
			{
				Guid result;
				try
				{
					result = ParseGuid1();
				}
				catch (FormatException)
				{
					Reset();
					result = ParseGuid2();
				}
				if (!AtEnd())
				{
					ThrowFormatException();
				}
				return result;
			}
		}

		private int _a;

		private short _b;

		private short _c;

		private byte _d;

		private byte _e;

		private byte _f;

		private byte _g;

		private byte _h;

		private byte _i;

		private byte _j;

		private byte _k;

		public static readonly Guid Empty;

		private static object _rngAccess;

		private static RandomNumberGenerator _rng;

		private static RandomNumberGenerator _fastRng;

		public Guid(byte[] b)
		{
			CheckArray(b, 16);
			_a = BitConverterLE.ToInt32(b, 0);
			_b = BitConverterLE.ToInt16(b, 4);
			_c = BitConverterLE.ToInt16(b, 6);
			_d = b[8];
			_e = b[9];
			_f = b[10];
			_g = b[11];
			_h = b[12];
			_i = b[13];
			_j = b[14];
			_k = b[15];
		}

		public Guid(string g)
		{
			CheckNull(g);
			g = g.Trim();
			GuidParser guidParser = new GuidParser(g);
			Guid guid = guidParser.Parse();
			this = guid;
		}

		public Guid(int a, short b, short c, byte[] d)
		{
			CheckArray(d, 8);
			_a = a;
			_b = b;
			_c = c;
			_d = d[0];
			_e = d[1];
			_f = d[2];
			_g = d[3];
			_h = d[4];
			_i = d[5];
			_j = d[6];
			_k = d[7];
		}

		public Guid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
		{
			_a = a;
			_b = b;
			_c = c;
			_d = d;
			_e = e;
			_f = f;
			_g = g;
			_h = h;
			_i = i;
			_j = j;
			_k = k;
		}

		[CLSCompliant(false)]
		public Guid(uint a, ushort b, ushort c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
			: this((int)a, (short)b, (short)c, d, e, f, g, h, i, j, k)
		{
		}

		static Guid()
		{
			Empty = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			_rngAccess = new object();
			if (MonoTouchAOTHelper.FalseFlag)
			{
				GenericComparer<Guid> genericComparer = new GenericComparer<Guid>();
				GenericEqualityComparer<Guid> genericEqualityComparer = new GenericEqualityComparer<Guid>();
			}
		}

		private static void CheckNull(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException(Locale.GetText("Value cannot be null."));
			}
		}

		private static void CheckLength(byte[] o, int l)
		{
			if (o.Length != l)
			{
				throw new ArgumentException(string.Format(Locale.GetText("Array should be exactly {0} bytes long."), l));
			}
		}

		private static void CheckArray(byte[] o, int l)
		{
			CheckNull(o);
			CheckLength(o, l);
		}

		private static int Compare(int x, int y)
		{
			if (x < y)
			{
				return -1;
			}
			return 1;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is Guid))
			{
				throw new ArgumentException("value", Locale.GetText("Argument of System.Guid.CompareTo should be a Guid."));
			}
			return CompareTo((Guid)value);
		}

		public override bool Equals(object o)
		{
			if (o is Guid)
			{
				return CompareTo((Guid)o) == 0;
			}
			return false;
		}

		public int CompareTo(Guid value)
		{
			if (_a != value._a)
			{
				return Compare(_a, value._a);
			}
			if (_b != value._b)
			{
				return Compare(_b, value._b);
			}
			if (_c != value._c)
			{
				return Compare(_c, value._c);
			}
			if (_d != value._d)
			{
				return Compare(_d, value._d);
			}
			if (_e != value._e)
			{
				return Compare(_e, value._e);
			}
			if (_f != value._f)
			{
				return Compare(_f, value._f);
			}
			if (_g != value._g)
			{
				return Compare(_g, value._g);
			}
			if (_h != value._h)
			{
				return Compare(_h, value._h);
			}
			if (_i != value._i)
			{
				return Compare(_i, value._i);
			}
			if (_j != value._j)
			{
				return Compare(_j, value._j);
			}
			if (_k != value._k)
			{
				return Compare(_k, value._k);
			}
			return 0;
		}

		public bool Equals(Guid g)
		{
			return CompareTo(g) == 0;
		}

		public override int GetHashCode()
		{
			int a = _a;
			a ^= (_b << 16) | _c;
			a ^= _d << 24;
			a ^= _e << 16;
			a ^= _f << 8;
			a ^= _g;
			a ^= _h << 24;
			a ^= _i << 16;
			a ^= _j << 8;
			return a ^ _k;
		}

		private static char ToHex(int b)
		{
			return (char)((b >= 10) ? (97 + b - 10) : (48 + b));
		}

		public static Guid NewGuid()
		{
			byte[] array = new byte[16];
			lock (_rngAccess)
			{
				if (_rng == null)
				{
					_rng = RandomNumberGenerator.Create();
				}
				_rng.GetBytes(array);
			}
			Guid result = new Guid(array);
			result._d = (byte)((result._d & 0x3F) | 0x80);
			result._c = (short)(((ulong)result._c & 0xFFFuL) | 0x4000);
			return result;
		}

		internal static byte[] FastNewGuidArray()
		{
			byte[] array = new byte[16];
			lock (_rngAccess)
			{
				if (_rng != null)
				{
					_fastRng = _rng;
				}
				if (_fastRng == null)
				{
					_fastRng = new RNGCryptoServiceProvider();
				}
				_fastRng.GetBytes(array);
			}
			array[8] = (byte)((array[8] & 0x3F) | 0x80);
			array[7] = (byte)((array[7] & 0xF) | 0x40);
			return array;
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[16];
			int num = 0;
			byte[] bytes = BitConverterLE.GetBytes(_a);
			for (int i = 0; i < 4; i++)
			{
				array[num++] = bytes[i];
			}
			bytes = BitConverterLE.GetBytes(_b);
			for (int i = 0; i < 2; i++)
			{
				array[num++] = bytes[i];
			}
			bytes = BitConverterLE.GetBytes(_c);
			for (int i = 0; i < 2; i++)
			{
				array[num++] = bytes[i];
			}
			array[8] = _d;
			array[9] = _e;
			array[10] = _f;
			array[11] = _g;
			array[12] = _h;
			array[13] = _i;
			array[14] = _j;
			array[15] = _k;
			return array;
		}

		private static void AppendInt(StringBuilder builder, int value)
		{
			builder.Append(ToHex((value >> 28) & 0xF));
			builder.Append(ToHex((value >> 24) & 0xF));
			builder.Append(ToHex((value >> 20) & 0xF));
			builder.Append(ToHex((value >> 16) & 0xF));
			builder.Append(ToHex((value >> 12) & 0xF));
			builder.Append(ToHex((value >> 8) & 0xF));
			builder.Append(ToHex((value >> 4) & 0xF));
			builder.Append(ToHex(value & 0xF));
		}

		private static void AppendShort(StringBuilder builder, short value)
		{
			builder.Append(ToHex((value >> 12) & 0xF));
			builder.Append(ToHex((value >> 8) & 0xF));
			builder.Append(ToHex((value >> 4) & 0xF));
			builder.Append(ToHex(value & 0xF));
		}

		private static void AppendByte(StringBuilder builder, byte value)
		{
			builder.Append(ToHex((value >> 4) & 0xF));
			builder.Append(ToHex(value & 0xF));
		}

		private string BaseToString(bool h, bool p, bool b)
		{
			StringBuilder stringBuilder = new StringBuilder(40);
			if (p)
			{
				stringBuilder.Append('(');
			}
			else if (b)
			{
				stringBuilder.Append('{');
			}
			AppendInt(stringBuilder, _a);
			if (h)
			{
				stringBuilder.Append('-');
			}
			AppendShort(stringBuilder, _b);
			if (h)
			{
				stringBuilder.Append('-');
			}
			AppendShort(stringBuilder, _c);
			if (h)
			{
				stringBuilder.Append('-');
			}
			AppendByte(stringBuilder, _d);
			AppendByte(stringBuilder, _e);
			if (h)
			{
				stringBuilder.Append('-');
			}
			AppendByte(stringBuilder, _f);
			AppendByte(stringBuilder, _g);
			AppendByte(stringBuilder, _h);
			AppendByte(stringBuilder, _i);
			AppendByte(stringBuilder, _j);
			AppendByte(stringBuilder, _k);
			if (p)
			{
				stringBuilder.Append(')');
			}
			else if (b)
			{
				stringBuilder.Append('}');
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return BaseToString(true, false, false);
		}

		public string ToString(string format)
		{
			bool h = true;
			bool p = false;
			bool b = false;
			if (format != null)
			{
				string text = format.ToLowerInvariant();
				switch (text)
				{
				case "b":
					b = true;
					break;
				case "p":
					p = true;
					break;
				case "n":
					h = false;
					break;
				default:
					if (text != "d" && text != string.Empty)
					{
						throw new FormatException(Locale.GetText("Argument to Guid.ToString(string format) should be \"b\", \"B\", \"d\", \"D\", \"n\", \"N\", \"p\" or \"P\""));
					}
					break;
				}
			}
			return BaseToString(h, p, b);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return ToString(format);
		}

		public static bool operator ==(Guid a, Guid b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Guid a, Guid b)
		{
			return !a.Equals(b);
		}
	}
}
