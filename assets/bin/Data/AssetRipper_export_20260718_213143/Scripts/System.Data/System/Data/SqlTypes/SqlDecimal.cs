using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlDecimal : IComparable, INullable, IXmlSerializable
	{
		private const int SCALE_SHIFT = 16;

		private const int SIGN_SHIFT = 31;

		private const int RESERVED_SS32_BITS = 2130771967;

		private const ulong LIT_GUINT64_HIGHBIT = 9223372036854775808uL;

		private const ulong LIT_GUINT32_HIGHBIT = 2147483648uL;

		private const byte DECIMAL_MAX_INTFACTORS = 9;

		private int[] value;

		private byte precision;

		private byte scale;

		private bool positive;

		private bool notNull;

		private static uint[] constantsDecadeInt32Factors = new uint[10] { 1u, 10u, 100u, 1000u, 10000u, 100000u, 1000000u, 10000000u, 100000000u, 1000000000u };

		public static readonly byte MaxPrecision = 38;

		public static readonly byte MaxScale = 38;

		public static readonly SqlDecimal MaxValue = new SqlDecimal(MaxPrecision, 0, true, -1, 160047679, 1518781562, 1262177448);

		public static readonly SqlDecimal MinValue = new SqlDecimal(MaxPrecision, 0, false, -1, 160047679, 1518781562, 1262177448);

		public static readonly SqlDecimal Null;

		public byte[] BinData
		{
			get
			{
				byte[] array = new byte[value.Length * 4];
				int num = 0;
				for (int i = 0; i < value.Length; i++)
				{
					array[num++] = (byte)(0xFF & value[i]);
					array[num++] = (byte)(0xFF & (value[i] >> 8));
					array[num++] = (byte)(0xFF & (value[i] >> 16));
					array[num++] = (byte)(0xFF & (value[i] >> 24));
				}
				return array;
			}
		}

		public int[] Data
		{
			get
			{
				if (IsNull)
				{
					throw new SqlNullValueException();
				}
				return new int[4]
				{
					value[0],
					value[1],
					value[2],
					value[3]
				};
			}
		}

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public bool IsPositive
		{
			get
			{
				return positive;
			}
		}

		public byte Precision
		{
			get
			{
				return precision;
			}
		}

		public byte Scale
		{
			get
			{
				return scale;
			}
		}

		public decimal Value
		{
			get
			{
				if (IsNull)
				{
					throw new SqlNullValueException();
				}
				if (value[3] > 0)
				{
					throw new OverflowException();
				}
				return new decimal(value[0], value[1], value[2], !positive, scale);
			}
		}

		public SqlDecimal(decimal value)
		{
			int[] bits = decimal.GetBits(value);
			precision = MaxPrecision;
			scale = (byte)((uint)bits[3] >> 16);
			if (scale > MaxScale || (bits[3] & 0x7F00FFFF) != 0)
			{
				throw new ArgumentException(global::Locale.GetText("Invalid scale"));
			}
			this.value = new int[4];
			this.value[0] = bits[0];
			this.value[1] = bits[1];
			this.value[2] = bits[2];
			this.value[3] = 0;
			positive = value >= 0m;
			notNull = true;
			precision = GetPrecision(value);
		}

		public SqlDecimal(double dVal)
			: this((decimal)dVal)
		{
			SqlDecimal sqlDecimal = this;
			int num = 17 - precision;
			sqlDecimal = ((num <= 0) ? Round(this, 17) : AdjustScale(this, num, false));
			notNull = sqlDecimal.notNull;
			positive = sqlDecimal.positive;
			precision = sqlDecimal.precision;
			scale = sqlDecimal.scale;
			value = sqlDecimal.value;
		}

		public SqlDecimal(int value)
			: this((decimal)value)
		{
		}

		public SqlDecimal(long value)
			: this((decimal)value)
		{
		}

		public SqlDecimal(byte bPrecision, byte bScale, bool fPositive, int[] bits)
			: this(bPrecision, bScale, fPositive, bits[0], bits[1], bits[2], bits[3])
		{
		}

		public SqlDecimal(byte bPrecision, byte bScale, bool fPositive, int data1, int data2, int data3, int data4)
		{
			precision = bPrecision;
			scale = bScale;
			positive = fPositive;
			value = new int[4];
			value[0] = data1;
			value[1] = data2;
			value[2] = data3;
			value[3] = data4;
			notNull = true;
			if (precision < scale)
			{
				throw new SqlTypeException(global::Locale.GetText("Invalid presicion/scale combination."));
			}
			if (precision > 38)
			{
				throw new SqlTypeException(global::Locale.GetText("Invalid precision/scale combination."));
			}
			if (ToDouble() > Math.Pow(10.0, 38.0) - 1.0 || ToDouble() < 0.0 - Math.Pow(10.0, 38.0))
			{
				throw new OverflowException("Can't convert to SqlDecimal, Out of range ");
			}
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				return;
			}
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return;
			}
			reader.MoveToContent();
			if (reader.EOF)
			{
				return;
			}
			reader.Read();
			if (reader.NodeType != XmlNodeType.EndElement && reader.Value.Length > 0)
			{
				if (string.Compare("Null", reader.Value) == 0)
				{
					notNull = false;
					return;
				}
				SqlDecimal sqlDecimal = new SqlDecimal(decimal.Parse(reader.Value));
				value = sqlDecimal.Data;
				notNull = true;
				scale = sqlDecimal.Scale;
				precision = sqlDecimal.Precision;
				positive = sqlDecimal.IsPositive;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(Value.ToString());
		}

		public static SqlDecimal Abs(SqlDecimal n)
		{
			if (!n.notNull)
			{
				return n;
			}
			return new SqlDecimal(n.Precision, n.Scale, true, n.Data);
		}

		public static SqlDecimal Add(SqlDecimal x, SqlDecimal y)
		{
			return x + y;
		}

		public static SqlDecimal AdjustScale(SqlDecimal n, int digits, bool fRound)
		{
			byte b = n.Precision;
			if (n.IsNull)
			{
				throw new SqlNullValueException();
			}
			if (digits == 0)
			{
				return n;
			}
			byte bScale;
			if (digits > 0)
			{
				b = (byte)(b + digits);
				bScale = (byte)(n.scale + digits);
				for (int i = 0; i < digits; i++)
				{
					n *= (SqlDecimal)10L;
				}
			}
			else
			{
				if (n.Scale < Math.Abs(digits))
				{
					throw new SqlTruncateException();
				}
				n = ((!fRound) ? Round(Truncate(n, digits + n.scale), digits + n.scale) : Round(n, digits + n.scale));
				bScale = n.scale;
			}
			return new SqlDecimal(b, bScale, n.positive, n.Data);
		}

		public static SqlDecimal Ceiling(SqlDecimal n)
		{
			if (!n.notNull)
			{
				return n;
			}
			return AdjustScale(n, -n.Scale, true);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlDecimal))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlDecimal"));
			}
			return CompareTo((SqlDecimal)value);
		}

		public int CompareTo(SqlDecimal value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return Value.CompareTo(value.Value);
		}

		public static SqlDecimal ConvertToPrecScale(SqlDecimal n, int precision, int scale)
		{
			int num = n.Precision;
			int num2 = n.Scale;
			n = AdjustScale(n, scale - n.scale, true);
			if (n.Scale >= num2 && precision < n.Precision)
			{
				throw new SqlTruncateException();
			}
			num = precision;
			return new SqlDecimal((byte)num, n.scale, n.IsPositive, n.Data);
		}

		public static SqlDecimal Divide(SqlDecimal x, SqlDecimal y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlDecimal))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlDecimal)value).IsNull;
			}
			if (((SqlDecimal)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlDecimal)value);
		}

		public static SqlBoolean Equals(SqlDecimal x, SqlDecimal y)
		{
			return x == y;
		}

		public static SqlDecimal Floor(SqlDecimal n)
		{
			return AdjustScale(n, -n.Scale, false);
		}

		internal static SqlDecimal FromTdsBigDecimal(TdsBigDecimal x)
		{
			if (x == null)
			{
				return Null;
			}
			return new SqlDecimal(x.Precision, x.Scale, !x.IsNegative, x.Data);
		}

		public override int GetHashCode()
		{
			int num = 10;
			num = 91 * num + Data[0];
			num = 91 * num + Data[1];
			num = 91 * num + Data[2];
			num = 91 * num + Data[3];
			num = 91 * num + Scale;
			return 91 * num + Precision;
		}

		public static SqlBoolean GreaterThan(SqlDecimal x, SqlDecimal y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlDecimal x, SqlDecimal y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlDecimal x, SqlDecimal y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlDecimal x, SqlDecimal y)
		{
			return x <= y;
		}

		public static SqlDecimal Multiply(SqlDecimal x, SqlDecimal y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlDecimal x, SqlDecimal y)
		{
			return x != y;
		}

		public static SqlDecimal Parse(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException(global::Locale.GetText("string s"));
			}
			return new SqlDecimal(decimal.Parse(s));
		}

		public static SqlDecimal Power(SqlDecimal n, double exp)
		{
			if (n.IsNull)
			{
				return Null;
			}
			return new SqlDecimal(Math.Pow(n.ToDouble(), exp));
		}

		public static SqlDecimal Round(SqlDecimal n, int position)
		{
			if (n.IsNull)
			{
				throw new SqlNullValueException();
			}
			decimal d = n.Value;
			d = decimal.Round(d, position);
			return new SqlDecimal(d);
		}

		public static SqlInt32 Sign(SqlDecimal n)
		{
			if (n.IsNull)
			{
				return SqlInt32.Null;
			}
			return n.IsPositive ? 1 : (-1);
		}

		public static SqlDecimal Subtract(SqlDecimal x, SqlDecimal y)
		{
			return x - y;
		}

		private byte GetPrecision(decimal value)
		{
			string text = value.ToString();
			byte b = 0;
			string text2 = text;
			foreach (char c in text2)
			{
				if (c >= '0' && c <= '9')
				{
					b++;
				}
			}
			return b;
		}

		public double ToDouble()
		{
			double num = (uint)Data[0];
			num += (double)(uint)Data[1] * Math.Pow(2.0, 32.0);
			num += (double)(uint)Data[2] * Math.Pow(2.0, 64.0);
			num += (double)(uint)Data[3] * Math.Pow(2.0, 96.0);
			return num / Math.Pow(10.0, (int)scale);
		}

		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		public SqlByte ToSqlByte()
		{
			return (SqlByte)this;
		}

		public SqlDouble ToSqlDouble()
		{
			return this;
		}

		public SqlInt16 ToSqlInt16()
		{
			return (SqlInt16)this;
		}

		public SqlInt32 ToSqlInt32()
		{
			return (SqlInt32)this;
		}

		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
		}

		public SqlMoney ToSqlMoney()
		{
			return (SqlMoney)this;
		}

		public SqlSingle ToSqlSingle()
		{
			return this;
		}

		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		public override string ToString()
		{
			if (IsNull)
			{
				return "Null";
			}
			ulong num = (uint)Data[0];
			num += (ulong)((long)Data[1] << 32);
			ulong num2 = (uint)Data[2];
			num2 += (ulong)((long)Data[3] << 32);
			uint rest = 0u;
			StringBuilder stringBuilder = new StringBuilder();
			int num3 = 0;
			while (num != 0L || num2 != 0L)
			{
				Div128By32(ref num2, ref num, 10u, ref rest);
				stringBuilder.Insert(0, rest.ToString());
				num3++;
			}
			while (stringBuilder.Length > Precision)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			if (Scale > 0)
			{
				stringBuilder.Insert(stringBuilder.Length - Scale, ".");
			}
			if (!positive)
			{
				stringBuilder.Insert(0, '-');
			}
			return stringBuilder.ToString();
		}

		private static int Div128By32(ref ulong hi, ref ulong lo, uint divider)
		{
			uint rest = 0u;
			return Div128By32(ref hi, ref lo, divider, ref rest);
		}

		private static int Div128By32(ref ulong hi, ref ulong lo, uint divider, ref uint rest)
		{
			ulong num = 0uL;
			ulong num2 = 0uL;
			ulong num3 = 0uL;
			num = (uint)(hi >> 32);
			num2 = num / divider;
			num -= num2 * divider;
			num <<= 32;
			num |= (uint)hi;
			num3 = num / divider;
			num -= num3 * divider;
			num <<= 32;
			hi = (num2 << 32) | (uint)num3;
			num |= (uint)(lo >> 32);
			num2 = num / divider;
			num -= num2 * divider;
			num <<= 32;
			num |= (uint)lo;
			num3 = num / divider;
			num -= num3 * divider;
			lo = (num2 << 32) | (uint)num3;
			rest = (uint)num;
			num <<= 1;
			return (num > divider || (num == divider && (num3 & 1) == 1)) ? 1 : 0;
		}

		[System.MonoTODO("Find out what is the right way to set scale and precision")]
		private static SqlDecimal DecimalDiv(SqlDecimal x, SqlDecimal y)
		{
			ulong clo = 0uL;
			ulong chi = 0uL;
			int num = 0;
			int exp = 0;
			byte b = 0;
			bool fPositive = !(x.positive ^ y.positive);
			b = ((x.Precision < y.Precision) ? y.Precision : x.Precision);
			DecimalDivSub(ref x, ref y, ref clo, ref chi, ref exp);
			num = x.Scale - y.Scale;
			Rescale128(ref clo, ref chi, ref num, exp, 0, 38, 1);
			uint rest = 0u;
			while (b < num)
			{
				Div128By32(ref chi, ref clo, 10u, ref rest);
				num--;
			}
			if (rest >= 5)
			{
				clo++;
			}
			while ((double)chi * Math.Pow(2.0, 64.0) + (double)clo - Math.Pow(10.0, (int)b) > 0.0)
			{
				b++;
			}
			while (b + num > MaxScale)
			{
				Div128By32(ref chi, ref clo, 10u, ref rest);
				num--;
				if (rest >= 5)
				{
					clo++;
				}
			}
			int data = (int)clo;
			int data2 = (int)(clo >> 32);
			int data3 = (int)chi;
			int data4 = (int)(chi >> 32);
			return new SqlDecimal(b, (byte)num, fPositive, data, data2, data3, data4);
		}

		private static void Rescale128(ref ulong clo, ref ulong chi, ref int scale, int texp, int minScale, int maxScale, int roundFlag)
		{
			uint num = 0u;
			uint num2 = 0u;
			int num3 = 0;
			int num4 = 0;
			int roundBit = 0;
			num3 = scale;
			if (texp > 0)
			{
				while (texp > 0 && num3 <= maxScale)
				{
					num2 = (uint)chi;
					while (texp > 0 && ((clo & 1) == 0L || num2 != 0))
					{
						if (--texp == 0)
						{
							roundBit = (int)(clo & 1);
						}
						RShift128(ref clo, ref chi);
						num2 = (uint)(chi >> 32);
					}
					num4 = ((texp <= 9) ? texp : 9);
					if (num3 + num4 > maxScale)
					{
						num4 = maxScale - num3;
					}
					if (num4 == 0)
					{
						break;
					}
					texp -= num4;
					num3 += num4;
					num = constantsDecadeInt32Factors[num4] >> num4;
					Mult128By32(ref clo, ref chi, num, 0);
				}
				while (texp > 0)
				{
					if (--texp == 0)
					{
						roundBit = (int)(clo & 1);
					}
					RShift128(ref clo, ref chi);
				}
			}
			while (num3 > maxScale)
			{
				num4 = scale - maxScale;
				if (num4 > 9)
				{
					num4 = 9;
				}
				num3 -= num4;
				roundBit = Div128By32(ref clo, ref chi, constantsDecadeInt32Factors[num4]);
			}
			while (num3 < minScale)
			{
				if (roundFlag == 0)
				{
					roundBit = 0;
				}
				num4 = minScale - num3;
				if (num4 > 9)
				{
					num4 = 9;
				}
				num3 += num4;
				Mult128By32(ref clo, ref chi, constantsDecadeInt32Factors[num4], roundBit);
				roundBit = 0;
			}
			scale = num3;
			Normalize128(ref clo, ref chi, ref num3, roundFlag, roundBit);
		}

		private static void Normalize128(ref ulong clo, ref ulong chi, ref int scale, int roundFlag, int roundBit)
		{
			int num = scale;
			scale = num;
			if (roundFlag != 0 && roundBit != 0)
			{
				RoundUp128(ref clo, ref chi);
			}
		}

		private static void RoundUp128(ref ulong lo, ref ulong hi)
		{
			if (++lo == 0L)
			{
				hi++;
			}
		}

		private static void DecimalDivSub(ref SqlDecimal x, ref SqlDecimal y, ref ulong clo, ref ulong chi, ref int exp)
		{
			ulong num = 0uL;
			ulong num2 = 0uL;
			ulong num3 = 0uL;
			uint num4 = 0u;
			uint num5 = 0u;
			uint num6 = 0u;
			uint num7 = 0u;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			ulong hi = (ulong)(((long)x.Data[3] << 32) | x.Data[2]);
			ulong lo = (ulong)(((long)x.Data[1] << 32) | x.Data[0]);
			ulong lo2 = 0uL;
			num4 = (uint)y.Data[0];
			num5 = (uint)y.Data[1];
			num6 = (uint)y.Data[2];
			num7 = (uint)y.Data[3];
			if (num4 == 0 && num5 == 0 && num6 == 0 && num7 == 0)
			{
				throw new DivideByZeroException();
			}
			if (lo == 0L && hi == 0L)
			{
				clo = (chi = 0uL);
				return;
			}
			num8 = 0;
			while ((hi & 0x8000000000000000uL) == 0L)
			{
				LShift128(ref lo, ref hi);
				num8++;
			}
			num9 = 0;
			while (((ulong)num7 & 0x80000000uL) == 0L)
			{
				LShift128(ref num4, ref num5, ref num6, ref num7);
				num9++;
			}
			num3 = ((ulong)num7 << 32) | num6;
			num2 = ((ulong)num5 << 32) | num4;
			num = 0uL;
			if (hi > num3 || (hi == num3 && lo >= num2))
			{
				Sub192(lo2, lo, hi, num, num2, num3, ref lo2, ref lo, ref hi);
				num10 = 1;
			}
			else
			{
				num10 = 0;
			}
			Div192By128To128(lo2, lo, hi, num4, num5, num6, num7, ref clo, ref chi);
			exp = 128 + num8 - num9;
			if (num10 != 0)
			{
				RShift128(ref clo, ref chi);
				chi += 9223372036854775808uL;
				exp--;
			}
			while (exp > 0 && (clo & 1) == 0L)
			{
				RShift128(ref clo, ref chi);
				exp--;
			}
		}

		private static void RShift128(ref ulong lo, ref ulong hi)
		{
			lo >>= 1;
			if ((hi & 1) != 0L)
			{
				lo |= 9223372036854775808uL;
			}
			hi >>= 1;
		}

		private static void LShift128(ref ulong lo, ref ulong hi)
		{
			hi <<= 1;
			if ((lo & 0x8000000000000000uL) != 0L)
			{
				hi++;
			}
			lo <<= 1;
		}

		private static void LShift128(ref uint lo, ref uint mi, ref uint mi2, ref uint hi)
		{
			hi <<= 1;
			if (((ulong)mi2 & 0x80000000uL) != 0L)
			{
				hi++;
			}
			mi2 <<= 1;
			if (((ulong)mi & 0x80000000uL) != 0L)
			{
				mi2++;
			}
			mi <<= 1;
			if (((ulong)lo & 0x80000000uL) != 0L)
			{
				mi++;
			}
			lo <<= 1;
		}

		private static void Div192By128To128(ulong xlo, ulong xmi, ulong xhi, uint ylo, uint ymi, uint ymi2, uint yhi, ref ulong clo, ref ulong chi)
		{
			ulong xlo2 = xlo;
			ulong xmi2 = xmi;
			ulong xhi2 = xhi;
			uint num = Div192By128To32WithRest(ref xlo2, ref xmi2, ref xhi2, ylo, ymi, ymi2, yhi);
			xhi2 = (xhi2 << 32) | (xmi2 >> 32);
			xmi2 = (xmi2 << 32) | (xlo2 >> 32);
			xlo2 <<= 32;
			chi = ((ulong)num << 32) | Div192By128To32WithRest(ref xlo2, ref xmi2, ref xhi2, ylo, ymi, ymi2, yhi);
			xhi2 = (xhi2 << 32) | (xmi2 >> 32);
			xmi2 = (xmi2 << 32) | (xlo2 >> 32);
			xlo2 <<= 32;
			num = Div192By128To32WithRest(ref xlo2, ref xmi2, ref xhi2, ylo, ymi, ymi2, yhi);
			uint num2;
			if (xhi2 >= yhi)
			{
				num2 = uint.MaxValue;
			}
			else
			{
				xhi2 <<= 32;
				num2 = (uint)(xhi2 / yhi);
			}
			clo = ((ulong)num << 32) | num2;
		}

		private static uint Div192By128To32WithRest(ref ulong xlo, ref ulong xmi, ref ulong xhi, uint ylo, uint ymi, uint ymi2, uint yhi)
		{
			ulong clo = 0uL;
			ulong chi = 0uL;
			ulong lo = xlo;
			ulong mi = xmi;
			ulong hi = xhi;
			uint num = (uint)((hi < (ulong)yhi << 32) ? (hi / yhi) : uint.MaxValue);
			Mult128By32To128(ylo, ymi, ymi2, yhi, num, ref clo, ref chi);
			Sub192(lo, mi, hi, 0uL, clo, chi, ref lo, ref mi, ref hi);
			while ((long)hi < 0L)
			{
				num--;
				Add192(lo, mi, hi, 0uL, ((ulong)ymi << 32) | ylo, yhi | ymi2, ref lo, ref mi, ref hi);
			}
			xlo = lo;
			xmi = mi;
			xhi = hi;
			return num;
		}

		private static void Mult128By32(ref ulong clo, ref ulong chi, uint factor, int roundBit)
		{
			ulong num = 0uL;
			uint num2 = 0u;
			uint num3 = 0u;
			num = (ulong)(uint)clo * (ulong)factor;
			if (roundBit != 0)
			{
				num += factor / 2;
			}
			num2 = (uint)num;
			num >>= 32;
			num += (clo >> 32) * factor;
			num3 = (uint)num;
			clo = ((ulong)num3 << 32) | num2;
			num >>= 32;
			num += (ulong)((long)(uint)chi * (long)factor);
			num2 = (uint)num;
			num >>= 32;
			num += (chi >> 32) * factor;
			num3 = (uint)num;
			chi = ((ulong)num3 << 32) | num2;
		}

		private static void Mult128By32To128(uint xlo, uint xmi, uint xmi2, uint xhi, uint factor, ref ulong clo, ref ulong chi)
		{
			ulong num = (ulong)xlo * (ulong)factor;
			uint num2 = (uint)num;
			num >>= 32;
			num += (ulong)((long)xmi * (long)factor);
			uint num3 = (uint)num;
			num >>= 32;
			num += (ulong)((long)xmi2 * (long)factor);
			uint num4 = (uint)num;
			num >>= 32;
			num += (ulong)((long)xhi * (long)factor);
			clo = ((ulong)num3 << 32) | num2;
			chi = num | num4;
		}

		private static void Add192(ulong xlo, ulong xmi, ulong xhi, ulong ylo, ulong ymi, ulong yhi, ref ulong clo, ref ulong cmi, ref ulong chi)
		{
			xlo += ylo;
			if (xlo < ylo)
			{
				xmi++;
				if (xmi == 0L)
				{
					xhi++;
				}
			}
			xmi += ymi;
			if (xmi < ymi)
			{
				xmi++;
			}
			xhi += yhi;
			clo = xlo;
			cmi = xmi;
			chi = xhi;
		}

		private static void Sub192(ulong xlo, ulong xmi, ulong xhi, ulong ylo, ulong ymi, ulong yhi, ref ulong lo, ref ulong mi, ref ulong hi)
		{
			ulong num = 0uL;
			ulong num2 = 0uL;
			ulong num3 = 0uL;
			num = xlo - ylo;
			num2 = xmi - ymi;
			num3 = xhi - yhi;
			if (xlo < ylo)
			{
				if (num2 == 0L)
				{
					num3--;
				}
				num2--;
			}
			if (xmi < ymi)
			{
				num3--;
			}
			lo = num;
			mi = num2;
			hi = num3;
		}

		public static SqlDecimal Truncate(SqlDecimal n, int position)
		{
			int num = n.scale - position;
			if (num == 0)
			{
				return n;
			}
			int[] data = n.Data;
			decimal num2 = new decimal(data[0], data[1], data[2], !n.positive, 0);
			decimal num3 = 10m;
			int num4 = 0;
			while (num4 < num)
			{
				num2 -= num2 % num3;
				num4++;
				num3 *= 10m;
			}
			data = decimal.GetBits(num2);
			data[3] = 0;
			return new SqlDecimal(n.precision, n.scale, n.positive, data);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			if (schemaSet != null && schemaSet.Count == 0)
			{
				XmlSchema xmlSchema = new XmlSchema();
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = "decimal";
				xmlSchema.Items.Add(xmlSchemaComplexType);
				schemaSet.Add(xmlSchema);
			}
			return new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlDecimal operator +(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			if (x.IsPositive && !y.IsPositive)
			{
				y = new SqlDecimal(y.Precision, y.Scale, !y.IsPositive, y.Data);
				return x - y;
			}
			if (!x.IsPositive && y.IsPositive)
			{
				x = new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
				return y - x;
			}
			if (!x.IsPositive && !y.IsPositive)
			{
				x = new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
				y = new SqlDecimal(y.Precision, y.Scale, !y.IsPositive, y.Data);
				x += y;
				return new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
			}
			if (x.scale > y.scale)
			{
				y = AdjustScale(y, x.scale - y.scale, false);
			}
			else if (y.scale > x.scale)
			{
				x = AdjustScale(x, y.scale - x.scale, false);
			}
			byte b = (byte)(Math.Max(x.Scale, y.Scale) + Math.Max(x.Precision - x.Scale, y.Precision - y.Scale) + 1);
			if (b > MaxPrecision)
			{
				b = MaxPrecision;
			}
			int[] data = x.Data;
			int[] data2 = y.Data;
			int[] array = new int[4];
			ulong num = 0uL;
			ulong num2 = 0uL;
			for (int i = 0; i < 4; i++)
			{
				num2 = (ulong)((long)(uint)data[i] + (long)(uint)data2[i]) + num;
				array[i] = (int)(num2 & 0xFFFFFFFFu);
				num = num2 >> 32;
			}
			if (num != 0)
			{
				throw new OverflowException();
			}
			return new SqlDecimal(b, x.Scale, x.IsPositive, array);
		}

		public static SqlDecimal operator /(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			return DecimalDiv(x, y);
		}

		public static SqlBoolean operator ==(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return SqlBoolean.False;
			}
			if (x.Scale > y.Scale)
			{
				y = AdjustScale(y, x.Scale - y.Scale, false);
			}
			else if (y.Scale > x.Scale)
			{
				x = AdjustScale(y, y.Scale - x.Scale, false);
			}
			for (int i = 0; i < 4; i++)
			{
				if (x.Data[i] != y.Data[i])
				{
					return SqlBoolean.False;
				}
			}
			return SqlBoolean.True;
		}

		public static SqlBoolean operator >(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return new SqlBoolean(x.IsPositive);
			}
			if (x.Scale > y.Scale)
			{
				y = AdjustScale(y, x.Scale - y.Scale, false);
			}
			else if (y.Scale > x.Scale)
			{
				x = AdjustScale(x, y.Scale - x.Scale, false);
			}
			for (int num = 3; num >= 0; num--)
			{
				if (x.Data[num] != 0 || y.Data[num] != 0)
				{
					return new SqlBoolean(x.Data[num] > y.Data[num]);
				}
			}
			return new SqlBoolean(false);
		}

		public static SqlBoolean operator >=(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return new SqlBoolean(x.IsPositive);
			}
			if (x.Scale > y.Scale)
			{
				y = AdjustScale(y, x.Scale - y.Scale, true);
			}
			else if (y.Scale > x.Scale)
			{
				x = AdjustScale(x, y.Scale - x.Scale, true);
			}
			for (int num = 3; num >= 0; num--)
			{
				if (x.Data[num] != 0 || y.Data[num] != 0)
				{
					return new SqlBoolean(x.Data[num] >= y.Data[num]);
				}
			}
			return new SqlBoolean(true);
		}

		public static SqlBoolean operator !=(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return SqlBoolean.True;
			}
			if (x.Scale > y.Scale)
			{
				x = AdjustScale(x, y.Scale - x.Scale, true);
			}
			else if (y.Scale > x.Scale)
			{
				y = AdjustScale(y, x.Scale - y.Scale, true);
			}
			for (int i = 0; i < 4; i++)
			{
				if (x.Data[i] != y.Data[i])
				{
					return SqlBoolean.True;
				}
			}
			return SqlBoolean.False;
		}

		public static SqlBoolean operator <(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return new SqlBoolean(y.IsPositive);
			}
			if (x.Scale > y.Scale)
			{
				y = AdjustScale(y, x.Scale - y.Scale, true);
			}
			else if (y.Scale > x.Scale)
			{
				x = AdjustScale(x, y.Scale - x.Scale, true);
			}
			for (int num = 3; num >= 0; num--)
			{
				if (x.Data[num] != 0 || y.Data[num] != 0)
				{
					return new SqlBoolean(x.Data[num] < y.Data[num]);
				}
			}
			return new SqlBoolean(false);
		}

		public static SqlBoolean operator <=(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			if (x.IsPositive != y.IsPositive)
			{
				return new SqlBoolean(y.IsPositive);
			}
			if (x.Scale > y.Scale)
			{
				y = AdjustScale(y, x.Scale - y.Scale, true);
			}
			else if (y.Scale > x.Scale)
			{
				x = AdjustScale(x, y.Scale - x.Scale, true);
			}
			for (int num = 3; num >= 0; num--)
			{
				if (x.Data[num] != 0 || y.Data[num] != 0)
				{
					return new SqlBoolean(x.Data[num] <= y.Data[num]);
				}
			}
			return new SqlBoolean(true);
		}

		public static SqlDecimal operator *(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			byte b = (byte)(x.Precision + y.Precision + 1);
			byte bScale = (byte)(x.Scale + y.Scale);
			if (b > MaxPrecision)
			{
				b = MaxPrecision;
			}
			int[] data = x.Data;
			int[] data2 = y.Data;
			int[] array = new int[4];
			ulong num = 0uL;
			for (int i = 0; i < 4; i++)
			{
				ulong num2 = 0uL;
				for (int j = i; j <= i; j++)
				{
					num2 += (ulong)((long)(uint)data[j] * (long)(uint)data2[i - j]);
				}
				array[i] = (int)((num2 + num) & 0xFFFFFFFFu);
				num = num2 >> 32;
			}
			if (num != 0)
			{
				throw new OverflowException();
			}
			return new SqlDecimal(b, bScale, x.IsPositive == y.IsPositive, array);
		}

		public static SqlDecimal operator -(SqlDecimal x, SqlDecimal y)
		{
			if (x.IsNull || y.IsNull)
			{
				return Null;
			}
			if (x.IsPositive && !y.IsPositive)
			{
				y = new SqlDecimal(y.Precision, y.Scale, !y.IsPositive, y.Data);
				return x + y;
			}
			if (!x.IsPositive && y.IsPositive)
			{
				x = new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
				x += y;
				return new SqlDecimal(x.Precision, x.Scale, false, x.Data);
			}
			if (!x.IsPositive && !y.IsPositive)
			{
				y = new SqlDecimal(y.Precision, y.Scale, !y.IsPositive, y.Data);
				x = new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
				return y - x;
			}
			if (x.scale > y.scale)
			{
				y = AdjustScale(y, x.scale - y.scale, false);
			}
			else if (y.scale > x.scale)
			{
				x = AdjustScale(x, y.scale - x.scale, false);
			}
			byte bPrecision = (byte)(Math.Max(x.Scale, y.Scale) + Math.Max(x.Precision - x.Scale, y.Precision - y.Scale));
			int[] data;
			int[] data2;
			if (x >= y)
			{
				data = x.Data;
				data2 = y.Data;
			}
			else
			{
				data = y.Data;
				data2 = x.Data;
			}
			ulong num = 0uL;
			int num2 = 0;
			int[] array = new int[4];
			for (int i = 0; i < 4; i++)
			{
				num = (ulong)((long)(uint)data[i] - (long)(uint)data2[i] + num2);
				num2 = 0;
				if ((uint)data2[i] > (uint)data[i])
				{
					num2 = -1;
				}
				array[i] = (int)num;
			}
			if (num2 > 0)
			{
				throw new OverflowException();
			}
			return new SqlDecimal(bPrecision, x.Scale, (x >= y).Value, array);
		}

		public static SqlDecimal operator -(SqlDecimal x)
		{
			return new SqlDecimal(x.Precision, x.Scale, !x.IsPositive, x.Data);
		}

		public static explicit operator SqlDecimal(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDecimal((decimal)x.ByteValue);
		}

		public static explicit operator decimal(SqlDecimal x)
		{
			return x.Value;
		}

		public static explicit operator SqlDecimal(SqlDouble x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDecimal(x.Value);
		}

		public static explicit operator SqlDecimal(SqlSingle x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDecimal(x.Value);
		}

		public static explicit operator SqlDecimal(SqlString x)
		{
			return Parse(x.Value);
		}

		public static explicit operator SqlDecimal(double x)
		{
			return new SqlDecimal(x);
		}

		public static implicit operator SqlDecimal(long x)
		{
			return new SqlDecimal(x);
		}

		public static implicit operator SqlDecimal(decimal x)
		{
			return new SqlDecimal(x);
		}

		public static implicit operator SqlDecimal(SqlByte x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDecimal((decimal)x.Value);
		}

		public static implicit operator SqlDecimal(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDecimal((decimal)x.Value);
		}

		public static implicit operator SqlDecimal(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDecimal((decimal)x.Value);
		}

		public static implicit operator SqlDecimal(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDecimal((decimal)x.Value);
		}

		public static implicit operator SqlDecimal(SqlMoney x)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDecimal(x.Value);
		}
	}
}
