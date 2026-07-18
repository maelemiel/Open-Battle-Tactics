using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class DBNull : IConvertible, ISerializable
	{
		public static readonly DBNull Value = new DBNull();

		private DBNull()
		{
		}

		private DBNull(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException();
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == typeof(string))
			{
				return string.Empty;
			}
			if (targetType == typeof(DBNull))
			{
				return this;
			}
			throw new InvalidCastException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			UnitySerializationHolder.GetDBNullData(this, info, context);
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.DBNull;
		}

		public override string ToString()
		{
			return string.Empty;
		}

		public string ToString(IFormatProvider provider)
		{
			return string.Empty;
		}
	}
}
