namespace System.Data.Common
{
	internal sealed class UInt64DataContainer : DataContainer
	{
		private ulong[] _values;

		protected override object GetValue(int index)
		{
			return _values[index];
		}

		protected override void ZeroOut(int index)
		{
			_values[index] = 0uL;
		}

		protected override void SetValue(int index, object value)
		{
			_values[index] = (ulong)GetContainerData(value);
		}

		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			_values[index] = (ulong)record.GetInt64Safe(field);
		}

		protected override void DoCopyValue(DataContainer from, int from_index, int to_index)
		{
			_values[to_index] = ((UInt64DataContainer)from)._values[from_index];
		}

		protected override int DoCompareValues(int index1, int index2)
		{
			ulong num = _values[index1];
			ulong num2 = _values[index2];
			return (num != num2) ? ((num >= num2) ? 1 : (-1)) : 0;
		}

		protected override void Resize(int size)
		{
			if (_values == null)
			{
				_values = new ulong[size];
				return;
			}
			ulong[] array = new ulong[size];
			Array.Copy(_values, 0, array, 0, _values.Length);
			_values = array;
		}

		internal override long GetInt64(int index)
		{
			return Convert.ToInt64(_values[index]);
		}
	}
}
