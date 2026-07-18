namespace System.Data.Common
{
	internal sealed class StringDataContainer : ObjectDataContainer
	{
		private void SetValue(int index, string value)
		{
			if (value != null && base.Column.MaxLength >= 0 && base.Column.MaxLength < value.Length)
			{
				throw new ArgumentException("Cannot set column '" + base.Column.ColumnName + "' to '" + value + "'. The value violates the MaxLength limit of this column.");
			}
			base.SetValue(index, (object)value);
		}

		protected override void SetValue(int index, object value)
		{
			SetValue(index, (string)GetContainerData(value));
		}

		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			SetValue(index, record.GetStringSafe(field));
		}

		protected override int DoCompareValues(int index1, int index2)
		{
			DataTable table = base.Column.Table;
			return string.Compare((string)base[index1], (string)base[index2], !table.CaseSensitive, table.Locale);
		}
	}
}
