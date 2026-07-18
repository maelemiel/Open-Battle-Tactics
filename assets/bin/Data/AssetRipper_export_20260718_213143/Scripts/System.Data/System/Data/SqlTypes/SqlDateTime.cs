using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public struct SqlDateTime : IComparable, INullable, IXmlSerializable
	{
		private DateTime value;

		private bool notNull;

		public static readonly SqlDateTime MaxValue;

		public static readonly SqlDateTime MinValue;

		public static readonly SqlDateTime Null;

		public static readonly int SQLTicksPerHour;

		public static readonly int SQLTicksPerMinute;

		public static readonly int SQLTicksPerSecond;

		private static readonly DateTime zero_day;

		public int DayTicks
		{
			get
			{
				return (Value - zero_day).Days;
			}
		}

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public int TimeTicks
		{
			get
			{
				return TimeSpanTicksToSQLTicks(Value.TimeOfDay.Ticks);
			}
		}

		public DateTime Value
		{
			get
			{
				if (IsNull)
				{
					throw new SqlNullValueException("The property contains Null.");
				}
				return value;
			}
		}

		public SqlDateTime(DateTime value)
		{
			this.value = value;
			notNull = true;
			CheckRange(this);
		}

		public SqlDateTime(int dayTicks, int timeTicks)
		{
			try
			{
				long num = SQLTicksToMilliseconds(timeTicks);
				value = zero_day.AddDays(dayTicks).AddMilliseconds(num);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			notNull = true;
			CheckRange(this);
		}

		public SqlDateTime(int year, int month, int day)
		{
			try
			{
				value = new DateTime(year, month, day);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			notNull = true;
			CheckRange(this);
		}

		public SqlDateTime(int year, int month, int day, int hour, int minute, int second)
		{
			try
			{
				value = new DateTime(year, month, day, hour, minute, second);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			notNull = true;
			CheckRange(this);
		}

		public SqlDateTime(int year, int month, int day, int hour, int minute, int second, double millisecond)
		{
			try
			{
				long ticks = (long)(millisecond * 10000.0);
				long num = SQLTicksToMilliseconds(TimeSpanTicksToSQLTicks(ticks));
				value = new DateTime(year, month, day, hour, minute, second).AddMilliseconds(num);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			notNull = true;
			CheckRange(this);
		}

		public SqlDateTime(int year, int month, int day, int hour, int minute, int second, int bilisecond)
		{
			try
			{
				long ticks = bilisecond * 10;
				long num = SQLTicksToMilliseconds(TimeSpanTicksToSQLTicks(ticks));
				value = new DateTime(year, month, day, hour, minute, second).AddMilliseconds(num);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			notNull = true;
			CheckRange(this);
		}

		static SqlDateTime()
		{
			SQLTicksPerHour = 1080000;
			SQLTicksPerMinute = 18000;
			SQLTicksPerSecond = 300;
			zero_day = new DateTime(1900, 1, 1);
			long ticks = new DateTime(9999, 12, 31, 23, 59, 59).Ticks + 9970000;
			MaxValue.value = new DateTime(ticks);
			MaxValue.notNull = true;
			MinValue.value = new DateTime(1753, 1, 1);
			MinValue.notNull = true;
		}

		[System.MonoTODO]
		XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		private static int TimeSpanTicksToSQLTicks(long ticks)
		{
			return (int)(ticks * SQLTicksPerSecond / 10000000);
		}

		private static long SQLTicksToMilliseconds(int timeTicks)
		{
			return (long)((double)timeTicks * 1000.0 / (double)SQLTicksPerSecond + 0.5);
		}

		private static void CheckRange(SqlDateTime target)
		{
			if (target.IsNull)
			{
				return;
			}
			DateTime dateTime = target.value;
			SqlDateTime maxValue = MaxValue;
			if (!(dateTime > maxValue.value))
			{
				DateTime dateTime2 = target.value;
				SqlDateTime minValue = MinValue;
				if (!(dateTime2 < minValue.value))
				{
					return;
				}
			}
			throw new SqlTypeException(string.Format("SqlDateTime overflow. Must be between {0} and {1}. Value was {2}", MinValue.Value, MaxValue.Value, target.value));
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlDateTime))
			{
				throw new ArgumentException(global::Locale.GetText("Value is not a System.Data.SqlTypes.SqlDateTime"));
			}
			return CompareTo((SqlDateTime)value);
		}

		public int CompareTo(SqlDateTime value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlDateTime))
			{
				return false;
			}
			if (IsNull)
			{
				return ((SqlDateTime)value).IsNull;
			}
			if (((SqlDateTime)value).IsNull)
			{
				return false;
			}
			return (bool)(this == (SqlDateTime)value);
		}

		public static SqlBoolean Equals(SqlDateTime x, SqlDateTime y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public static SqlDateTime Add(SqlDateTime x, TimeSpan t)
		{
			return x + t;
		}

		public static SqlDateTime Subtract(SqlDateTime x, TimeSpan t)
		{
			return x - t;
		}

		public static SqlBoolean GreaterThan(SqlDateTime x, SqlDateTime y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlDateTime x, SqlDateTime y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlDateTime x, SqlDateTime y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlDateTime x, SqlDateTime y)
		{
			return x <= y;
		}

		public static SqlBoolean NotEquals(SqlDateTime x, SqlDateTime y)
		{
			return x != y;
		}

		public static SqlDateTime Parse(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("Argument cannot be null");
			}
			DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
			try
			{
				return new SqlDateTime(DateTime.Parse(s, currentInfo));
			}
			catch (Exception)
			{
			}
			try
			{
				return new SqlDateTime(DateTime.Parse(s, CultureInfo.InvariantCulture));
			}
			catch (Exception)
			{
			}
			throw new FormatException(string.Format("String {0} is not recognized as valid DateTime.", s));
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
			return value.ToString(CultureInfo.InvariantCulture);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("dateTime", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlDateTime operator +(SqlDateTime x, TimeSpan t)
		{
			if (x.IsNull)
			{
				return Null;
			}
			return new SqlDateTime(x.Value + t);
		}

		public static SqlBoolean operator ==(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlBoolean operator >(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(!(x.Value == y.Value));
		}

		public static SqlBoolean operator <(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlDateTime operator -(SqlDateTime x, TimeSpan t)
		{
			if (x.IsNull)
			{
				return x;
			}
			return new SqlDateTime(x.Value - t);
		}

		public static explicit operator DateTime(SqlDateTime x)
		{
			return x.Value;
		}

		public static explicit operator SqlDateTime(SqlString x)
		{
			return Parse(x.Value);
		}

		public static implicit operator SqlDateTime(DateTime value)
		{
			return new SqlDateTime(value);
		}
	}
}
