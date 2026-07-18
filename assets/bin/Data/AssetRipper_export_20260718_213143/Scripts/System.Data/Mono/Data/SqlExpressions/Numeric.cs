using System;

namespace Mono.Data.SqlExpressions
{
	internal class Numeric
	{
		internal static bool IsNumeric(object o)
		{
			if (o is IConvertible)
			{
				TypeCode typeCode = ((IConvertible)o).GetTypeCode();
				if (TypeCode.Char < typeCode && typeCode <= TypeCode.Decimal)
				{
					return true;
				}
			}
			return false;
		}

		internal static IConvertible Unify(IConvertible o)
		{
			switch (o.GetTypeCode())
			{
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				return (IConvertible)Convert.ChangeType(o, TypeCode.Int32);
			case TypeCode.UInt32:
				return (IConvertible)Convert.ChangeType(o, TypeCode.Int64);
			case TypeCode.UInt64:
				return (IConvertible)Convert.ChangeType(o, TypeCode.Decimal);
			case TypeCode.Single:
				return (IConvertible)Convert.ChangeType(o, TypeCode.Double);
			default:
				return o;
			}
		}

		internal static TypeCode ToSameType(ref IConvertible o1, ref IConvertible o2)
		{
			TypeCode typeCode = o1.GetTypeCode();
			TypeCode typeCode2 = o2.GetTypeCode();
			if (typeCode == typeCode2)
			{
				return typeCode;
			}
			if (typeCode == TypeCode.DBNull || typeCode2 == TypeCode.DBNull)
			{
				return TypeCode.DBNull;
			}
			if (typeCode < typeCode2)
			{
				o1 = (IConvertible)Convert.ChangeType(o1, typeCode2);
				return typeCode2;
			}
			o2 = (IConvertible)Convert.ChangeType(o2, typeCode);
			return typeCode;
		}

		internal static IConvertible Add(IConvertible o1, IConvertible o2)
		{
			switch (ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (long)((int)(object)o1 + (int)(object)o2);
			case TypeCode.Int64:
				return (long)(object)o1 + (long)(object)o2;
			case TypeCode.Double:
				return (double)(object)o1 + (double)(object)o2;
			case TypeCode.Decimal:
				return (decimal)(object)o1 + (decimal)(object)o2;
			default:
				return DBNull.Value;
			}
		}

		internal static IConvertible Subtract(IConvertible o1, IConvertible o2)
		{
			switch (ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (int)(object)o1 - (int)(object)o2;
			case TypeCode.Int64:
				return (long)(object)o1 - (long)(object)o2;
			case TypeCode.Double:
				return (double)(object)o1 - (double)(object)o2;
			case TypeCode.Decimal:
				return (decimal)(object)o1 - (decimal)(object)o2;
			default:
				return DBNull.Value;
			}
		}

		internal static IConvertible Multiply(IConvertible o1, IConvertible o2)
		{
			switch (ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (int)(object)o1 * (int)(object)o2;
			case TypeCode.Int64:
				return (long)(object)o1 * (long)(object)o2;
			case TypeCode.Double:
				return (double)(object)o1 * (double)(object)o2;
			case TypeCode.Decimal:
				return (decimal)(object)o1 * (decimal)(object)o2;
			default:
				return DBNull.Value;
			}
		}

		internal static IConvertible Divide(IConvertible o1, IConvertible o2)
		{
			switch (ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (int)(object)o1 / (int)(object)o2;
			case TypeCode.Int64:
				return (long)(object)o1 / (long)(object)o2;
			case TypeCode.Double:
				return (double)(object)o1 / (double)(object)o2;
			case TypeCode.Decimal:
				return (decimal)(object)o1 / (decimal)(object)o2;
			default:
				return DBNull.Value;
			}
		}

		internal static IConvertible Modulo(IConvertible o1, IConvertible o2)
		{
			switch (ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return (int)(object)o1 % (int)(object)o2;
			case TypeCode.Int64:
				return (long)(object)o1 % (long)(object)o2;
			case TypeCode.Double:
				return (double)(object)o1 % (double)(object)o2;
			case TypeCode.Decimal:
				return (decimal)(object)o1 % (decimal)(object)o2;
			default:
				return DBNull.Value;
			}
		}

		internal static IConvertible Negative(IConvertible o)
		{
			switch (o.GetTypeCode())
			{
			case TypeCode.Int32:
				return -(int)(object)o;
			case TypeCode.Int64:
				return -(long)(object)o;
			case TypeCode.Double:
				return 0.0 - (double)(object)o;
			case TypeCode.Decimal:
				return -(decimal)(object)o;
			default:
				return DBNull.Value;
			}
		}

		internal static IConvertible Min(IConvertible o1, IConvertible o2)
		{
			switch (ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return System.Math.Min((int)(object)o1, (int)(object)o2);
			case TypeCode.Int64:
				return System.Math.Min((long)(object)o1, (long)(object)o2);
			case TypeCode.Double:
				return System.Math.Min((double)(object)o1, (double)(object)o2);
			case TypeCode.Decimal:
				return System.Math.Min((decimal)(object)o1, (decimal)(object)o2);
			case TypeCode.String:
			{
				int num = string.Compare((string)o1, (string)o2);
				if (num <= 0)
				{
					return o1;
				}
				return o2;
			}
			default:
				return DBNull.Value;
			}
		}

		internal static IConvertible Max(IConvertible o1, IConvertible o2)
		{
			switch (ToSameType(ref o1, ref o2))
			{
			case TypeCode.Int32:
				return System.Math.Max((int)(object)o1, (int)(object)o2);
			case TypeCode.Int64:
				return System.Math.Max((long)(object)o1, (long)(object)o2);
			case TypeCode.Double:
				return System.Math.Max((double)(object)o1, (double)(object)o2);
			case TypeCode.Decimal:
				return System.Math.Max((decimal)(object)o1, (decimal)(object)o2);
			case TypeCode.String:
			{
				int num = string.Compare((string)o1, (string)o2);
				if (num >= 0)
				{
					return o1;
				}
				return o2;
			}
			default:
				return DBNull.Value;
			}
		}
	}
}
