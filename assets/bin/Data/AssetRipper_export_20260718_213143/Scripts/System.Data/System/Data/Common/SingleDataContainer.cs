namespace System.Data.Common
{
	internal sealed class SingleDataContainer : DataContainer
	{
		private float[] _values;

		protected override object GetValue(int index)
		{
			return _values[index];
		}

		protected override void ZeroOut(int index)
		{
			_values[index] = 0f;
		}

		protected override void SetValue(int index, object value)
		{
			_values[index] = (float)GetContainerData(value);
		}

		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			_values[index] = record.GetFloatSafe(field);
		}

		protected override void DoCopyValue(DataContainer from, int from_index, int to_index)
		{
			_values[to_index] = ((SingleDataContainer)from)._values[from_index];
		}

		protected override int DoCompareValues(int index1, int index2)
		{
			float num = _values[index1];
			float num2 = _values[index2];
			return (num != num2) ? ((!(num < num2)) ? 1 : (-1)) : 0;
		}

		protected override void Resize(int size)
		{
			if (_values == null)
			{
				_values = new float[size];
				return;
			}
			float[] array = new float[size];
			Array.Copy(_values, 0, array, 0, _values.Length);
			_values = array;
		}

		internal override long GetInt64(int index)
		{
			return Convert.ToInt64(_values[index]);
		}
	}
}
