using System.Collections;
using System.Reflection;

namespace System.Data.Common
{
	internal abstract class DataContainer
	{
		private BitArray null_values;

		private Type _type;

		private DataColumn _column;

		internal object this[int index]
		{
			get
			{
				return (!IsNull(index)) ? GetValue(index) : DBNull.Value;
			}
			set
			{
				if (value == null)
				{
					CopyValue(Column.Table.DefaultValuesRowIndex, index);
					return;
				}
				bool flag = value == DBNull.Value;
				if (flag)
				{
					ZeroOut(index);
				}
				else
				{
					SetValue(index, value);
				}
				null_values[index] = flag;
			}
		}

		internal int Capacity
		{
			get
			{
				return (null_values != null) ? null_values.Count : 0;
			}
			set
			{
				int capacity = Capacity;
				if (value != capacity)
				{
					if (null_values == null)
					{
						null_values = new BitArray(value);
					}
					else
					{
						null_values.Length = value;
					}
					Resize(value);
				}
			}
		}

		internal Type Type
		{
			get
			{
				return _type;
			}
		}

		protected DataColumn Column
		{
			get
			{
				return _column;
			}
		}

		protected abstract object GetValue(int index);

		internal abstract long GetInt64(int index);

		protected abstract void ZeroOut(int index);

		protected abstract void SetValue(int index, object value);

		protected abstract void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field);

		protected abstract void DoCopyValue(DataContainer from, int from_index, int to_index);

		protected abstract int DoCompareValues(int index1, int index2);

		protected abstract void Resize(int length);

		internal static DataContainer Create(Type type, DataColumn column)
		{
			DataContainer dataContainer;
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Int16:
				dataContainer = new Int16DataContainer();
				break;
			case TypeCode.Int32:
				dataContainer = new Int32DataContainer();
				break;
			case TypeCode.Int64:
				dataContainer = new Int64DataContainer();
				break;
			case TypeCode.String:
				dataContainer = new StringDataContainer();
				break;
			case TypeCode.Boolean:
				dataContainer = new BitDataContainer();
				break;
			case TypeCode.Byte:
				dataContainer = new ByteDataContainer();
				break;
			case TypeCode.Char:
				dataContainer = new CharDataContainer();
				break;
			case TypeCode.Double:
				dataContainer = new DoubleDataContainer();
				break;
			case TypeCode.SByte:
				dataContainer = new SByteDataContainer();
				break;
			case TypeCode.Single:
				dataContainer = new SingleDataContainer();
				break;
			case TypeCode.UInt16:
				dataContainer = new UInt16DataContainer();
				break;
			case TypeCode.UInt32:
				dataContainer = new UInt32DataContainer();
				break;
			case TypeCode.UInt64:
				dataContainer = new UInt64DataContainer();
				break;
			case TypeCode.DateTime:
				dataContainer = new DateTimeDataContainer();
				break;
			case TypeCode.Decimal:
				dataContainer = new DecimalDataContainer();
				break;
			default:
				dataContainer = new ObjectDataContainer();
				break;
			}
			dataContainer._type = type;
			dataContainer._column = column;
			return dataContainer;
		}

		internal static object GetExplicitValue(object value)
		{
			Type type = value.GetType();
			MethodInfo method = type.GetMethod("op_Explicit", new Type[1] { type });
			if (method != null)
			{
				return method.Invoke(value, new object[1] { value });
			}
			return null;
		}

		internal object GetContainerData(object value)
		{
			if (_type.IsInstanceOfType(value))
			{
				return value;
			}
			if (value is IConvertible)
			{
				switch (Type.GetTypeCode(_type))
				{
				case TypeCode.Int16:
					return Convert.ToInt16(value);
				case TypeCode.Int32:
					return Convert.ToInt32(value);
				case TypeCode.Int64:
					return Convert.ToInt64(value);
				case TypeCode.String:
					return Convert.ToString(value);
				case TypeCode.Boolean:
					return Convert.ToBoolean(value);
				case TypeCode.Byte:
					return Convert.ToByte(value);
				case TypeCode.Char:
					return Convert.ToChar(value);
				case TypeCode.Double:
					return Convert.ToDouble(value);
				case TypeCode.SByte:
					return Convert.ToSByte(value);
				case TypeCode.Single:
					return Convert.ToSingle(value);
				case TypeCode.UInt16:
					return Convert.ToUInt16(value);
				case TypeCode.UInt32:
					return Convert.ToUInt32(value);
				case TypeCode.UInt64:
					return Convert.ToUInt64(value);
				case TypeCode.DateTime:
					return Convert.ToDateTime(value);
				case TypeCode.Decimal:
					return Convert.ToDecimal(value);
				default:
					throw new InvalidCastException();
				}
			}
			object explicitValue;
			if ((explicitValue = GetExplicitValue(value)) != null)
			{
				return explicitValue;
			}
			throw new InvalidCastException();
		}

		internal bool IsNull(int index)
		{
			return null_values == null || null_values[index];
		}

		internal void FillValues(int fromIndex)
		{
			for (int i = 0; i < Capacity; i++)
			{
				CopyValue(fromIndex, i);
			}
		}

		internal void CopyValue(int from_index, int to_index)
		{
			CopyValue(this, from_index, to_index);
		}

		internal void CopyValue(DataContainer from, int from_index, int to_index)
		{
			DoCopyValue(from, from_index, to_index);
			null_values[to_index] = from.null_values[from_index];
		}

		internal void SetItemFromDataRecord(int index, IDataRecord record, int field)
		{
			if (record.IsDBNull(field))
			{
				this[index] = DBNull.Value;
			}
			else if (record is ISafeDataRecord)
			{
				SetValueFromSafeDataRecord(index, (ISafeDataRecord)record, field);
			}
			else
			{
				this[index] = record.GetValue(field);
			}
		}

		internal int CompareValues(int index1, int index2)
		{
			bool flag = IsNull(index1);
			bool flag2 = IsNull(index2);
			if (flag == flag2)
			{
				return (!flag) ? DoCompareValues(index1, index2) : 0;
			}
			return (!flag) ? 1 : (-1);
		}
	}
}
