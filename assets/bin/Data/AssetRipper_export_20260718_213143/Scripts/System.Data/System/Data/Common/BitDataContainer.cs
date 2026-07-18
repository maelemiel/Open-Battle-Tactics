using System.Collections;

namespace System.Data.Common
{
	internal sealed class BitDataContainer : DataContainer
	{
		private BitArray _values;

		protected override object GetValue(int index)
		{
			return _values[index];
		}

		protected override void ZeroOut(int index)
		{
			_values[index] = false;
		}

		protected override void SetValue(int index, object value)
		{
			_values[index] = (bool)GetContainerData(value);
		}

		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			_values[index] = record.GetBooleanSafe(field);
		}

		protected override void DoCopyValue(DataContainer from, int from_index, int to_index)
		{
			_values[to_index] = ((BitDataContainer)from)._values[from_index];
		}

		protected override int DoCompareValues(int index1, int index2)
		{
			bool flag = _values[index1];
			bool flag2 = _values[index2];
			return (flag != flag2) ? (flag ? 1 : (-1)) : 0;
		}

		protected override void Resize(int size)
		{
			if (_values == null)
			{
				_values = new BitArray(size);
			}
			else
			{
				_values.Length = size;
			}
		}

		internal override long GetInt64(int index)
		{
			return Convert.ToInt64(_values[index]);
		}
	}
}
