namespace System.Data.Common
{
	internal sealed class UInt16DataContainer : DataContainer
	{
		private ushort[] _values;

		protected override object GetValue(int index)
		{
			return _values[index];
		}

		protected override void ZeroOut(int index)
		{
			_values[index] = 0;
		}

		protected override void SetValue(int index, object value)
		{
			_values[index] = (ushort)GetContainerData(value);
		}

		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			_values[index] = (ushort)record.GetInt16Safe(field);
		}

		protected override void DoCopyValue(DataContainer from, int from_index, int to_index)
		{
			_values[to_index] = ((UInt16DataContainer)from)._values[from_index];
		}

		protected override int DoCompareValues(int index1, int index2)
		{
			int num = _values[index1];
			int num2 = _values[index2];
			return num - num2;
		}

		protected override void Resize(int size)
		{
			if (_values == null)
			{
				_values = new ushort[size];
				return;
			}
			ushort[] array = new ushort[size];
			Array.Copy(_values, 0, array, 0, _values.Length);
			_values = array;
		}

		internal override long GetInt64(int index)
		{
			return (int)_values[index];
		}
	}
}
