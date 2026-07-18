using System;
using System.Text;
using Mono.Data.Tds.Protocol;

namespace Mono.Data.Tds
{
	public class TdsMetaParameter
	{
		private TdsParameterDirection direction;

		private byte precision;

		private byte scale;

		private int size;

		private string typeName;

		private string name;

		private bool isSizeSet;

		private bool isNullable;

		private object value;

		private bool isVariableSizeType;

		private FrameworkValueGetter frameworkValueGetter;

		private object rawValue;

		private bool isUpdated;

		public TdsParameterDirection Direction
		{
			get
			{
				return direction;
			}
			set
			{
				direction = value;
			}
		}

		public string TypeName
		{
			get
			{
				return typeName;
			}
			set
			{
				typeName = value;
			}
		}

		public string ParameterName
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public bool IsNullable
		{
			get
			{
				return isNullable;
			}
			set
			{
				isNullable = value;
			}
		}

		public object Value
		{
			get
			{
				if (frameworkValueGetter != null)
				{
					object obj = frameworkValueGetter(rawValue, ref isUpdated);
					if (isUpdated)
					{
						value = obj;
					}
				}
				if (isUpdated)
				{
					value = ResizeValue(value);
					isUpdated = false;
				}
				return value;
			}
			set
			{
				rawValue = (this.value = value);
				isUpdated = true;
			}
		}

		public object RawValue
		{
			get
			{
				return rawValue;
			}
			set
			{
				Value = value;
			}
		}

		public byte Precision
		{
			get
			{
				return precision;
			}
			set
			{
				precision = value;
			}
		}

		public byte Scale
		{
			get
			{
				if ((TypeName == "decimal" || TypeName == "numeric") && scale == 0 && !Convert.IsDBNull(Value))
				{
					int[] bits = decimal.GetBits(Convert.ToDecimal(Value));
					scale = (byte)((bits[3] >> 16) & 0xFF);
				}
				return scale;
			}
			set
			{
				scale = value;
			}
		}

		public int Size
		{
			get
			{
				return GetSize();
			}
			set
			{
				size = value;
				isUpdated = true;
				isSizeSet = true;
			}
		}

		public bool IsVariableSizeType
		{
			get
			{
				return isVariableSizeType;
			}
			set
			{
				isVariableSizeType = value;
			}
		}

		public TdsMetaParameter(string name, object value)
			: this(name, string.Empty, value)
		{
		}

		public TdsMetaParameter(string name, FrameworkValueGetter valueGetter)
			: this(name, string.Empty, null)
		{
			frameworkValueGetter = valueGetter;
		}

		public TdsMetaParameter(string name, string typeName, object value)
		{
			ParameterName = name;
			Value = value;
			TypeName = typeName;
			IsNullable = false;
		}

		public TdsMetaParameter(string name, int size, bool isNullable, byte precision, byte scale, object value)
		{
			ParameterName = name;
			Size = size;
			IsNullable = isNullable;
			Precision = precision;
			Scale = scale;
			Value = value;
		}

		public TdsMetaParameter(string name, int size, bool isNullable, byte precision, byte scale, FrameworkValueGetter valueGetter)
		{
			ParameterName = name;
			Size = size;
			IsNullable = isNullable;
			Precision = precision;
			Scale = scale;
			frameworkValueGetter = valueGetter;
		}

		private object ResizeValue(object newValue)
		{
			if (newValue == DBNull.Value || newValue == null)
			{
				return newValue;
			}
			if (!isSizeSet || size <= 0)
			{
				return newValue;
			}
			string text = newValue as string;
			if (text != null)
			{
				if ((TypeName == "nvarchar" || TypeName == "nchar" || TypeName == "xml") && text.Length > size)
				{
					return text.Substring(0, size);
				}
			}
			else if (newValue.GetType() == typeof(byte[]))
			{
				byte[] array = (byte[])newValue;
				if (array.Length > size)
				{
					byte[] array2 = new byte[size];
					Array.Copy(array, array2, size);
					return array2;
				}
			}
			return newValue;
		}

		internal string Prepare()
		{
			string text = TypeName;
			if (text == "varbinary")
			{
				int actualSize = Size;
				if (actualSize <= 0)
				{
					actualSize = GetActualSize();
				}
				if (actualSize > 8000)
				{
					text = "image";
				}
			}
			string arg = "@";
			if (ParameterName[0] == '@')
			{
				arg = string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(string.Format("{0}{1} {2}", arg, ParameterName, text));
			switch (text)
			{
			case "decimal":
			case "numeric":
				stringBuilder.Append(string.Format("({0},{1})", (byte)((Precision != 0) ? Precision : 29), Scale));
				break;
			case "varchar":
			case "varbinary":
			{
				int num = Size;
				if (num <= 0)
				{
					num = GetActualSize();
					if (num <= 0)
					{
						num = 1;
					}
				}
				stringBuilder.Append((num <= 8000) ? string.Format("({0})", num) : "(max)");
				break;
			}
			case "nvarchar":
			case "xml":
				stringBuilder.Append((Size <= 0) ? "(4000)" : ((Size <= 8000) ? string.Format("({0})", Size) : "(max)"));
				break;
			case "char":
			case "nchar":
			case "binary":
				if (isSizeSet && Size > 0)
				{
					stringBuilder.Append(string.Format("({0})", Size));
				}
				break;
			}
			return stringBuilder.ToString();
		}

		internal int GetActualSize()
		{
			if (Value == DBNull.Value || Value == null)
			{
				return 0;
			}
			switch (Value.GetType().ToString())
			{
			case "System.String":
			{
				int num = ((string)value).Length;
				if (TypeName == "nvarchar" || TypeName == "nchar" || TypeName == "ntext" || TypeName == "xml")
				{
					num *= 2;
				}
				return num;
			}
			case "System.Byte[]":
				return ((byte[])value).Length;
			default:
				return GetSize();
			}
		}

		private int GetSize()
		{
			switch (TypeName)
			{
			case "decimal":
				return 17;
			case "uniqueidentifier":
				return 16;
			case "bigint":
			case "datetime":
			case "float":
			case "money":
				return 8;
			case "int":
			case "real":
			case "smalldatetime":
			case "smallmoney":
				return 4;
			case "smallint":
				return 2;
			case "tinyint":
			case "bit":
				return 1;
			case "nchar":
			case "ntext":
				return size * 2;
			default:
				return size;
			}
		}

		internal byte[] GetBytes()
		{
			byte[] result = new byte[0];
			if (Value == DBNull.Value || Value == null)
			{
				return result;
			}
			switch (TypeName)
			{
			case "nvarchar":
			case "nchar":
			case "ntext":
			case "xml":
				return Encoding.Unicode.GetBytes((string)Value);
			case "varchar":
			case "char":
			case "text":
				return Encoding.Default.GetBytes((string)Value);
			default:
				return (byte[])Value;
			}
		}

		internal TdsColumnType GetMetaType()
		{
			switch (TypeName)
			{
			case "binary":
				return TdsColumnType.BigBinary;
			case "bit":
				if (IsNullable)
				{
					return TdsColumnType.BitN;
				}
				return TdsColumnType.Bit;
			case "bigint":
				if (IsNullable)
				{
					return TdsColumnType.IntN;
				}
				return TdsColumnType.BigInt;
			case "char":
				return TdsColumnType.Char;
			case "money":
				if (IsNullable)
				{
					return TdsColumnType.MoneyN;
				}
				return TdsColumnType.Money;
			case "smallmoney":
				if (IsNullable)
				{
					return TdsColumnType.MoneyN;
				}
				return TdsColumnType.Money4;
			case "decimal":
				return TdsColumnType.Decimal;
			case "datetime":
				if (IsNullable)
				{
					return TdsColumnType.DateTimeN;
				}
				return TdsColumnType.DateTime;
			case "smalldatetime":
				if (IsNullable)
				{
					return TdsColumnType.DateTimeN;
				}
				return TdsColumnType.DateTime4;
			case "float":
				if (IsNullable)
				{
					return TdsColumnType.FloatN;
				}
				return TdsColumnType.Float8;
			case "image":
				return TdsColumnType.Image;
			case "int":
				if (IsNullable)
				{
					return TdsColumnType.IntN;
				}
				return TdsColumnType.Int4;
			case "numeric":
				return TdsColumnType.Numeric;
			case "nchar":
				return TdsColumnType.NChar;
			case "ntext":
				return TdsColumnType.NText;
			case "xml":
			case "nvarchar":
				return TdsColumnType.BigNVarChar;
			case "real":
				if (IsNullable)
				{
					return TdsColumnType.FloatN;
				}
				return TdsColumnType.Real;
			case "smallint":
				if (IsNullable)
				{
					return TdsColumnType.IntN;
				}
				return TdsColumnType.Int2;
			case "text":
				return TdsColumnType.Text;
			case "tinyint":
				if (IsNullable)
				{
					return TdsColumnType.IntN;
				}
				return TdsColumnType.Int1;
			case "uniqueidentifier":
				return TdsColumnType.UniqueIdentifier;
			case "varbinary":
				return TdsColumnType.BigVarBinary;
			case "varchar":
				return TdsColumnType.BigVarChar;
			default:
				throw new NotSupportedException("Unknown Type : " + TypeName);
			}
		}

		public void Validate(int index)
		{
			if ((direction == TdsParameterDirection.InputOutput || direction == TdsParameterDirection.Output) && isVariableSizeType && (Value == DBNull.Value || Value == null) && Size == 0)
			{
				throw new InvalidOperationException(string.Format("{0}[{1}]: the Size property should not be of size 0", typeName, index));
			}
		}
	}
}
