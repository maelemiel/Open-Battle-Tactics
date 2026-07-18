namespace System.Data.Common
{
	internal sealed class DecimalDataContainer : ObjectDataContainer
	{
		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			base.SetValue(index, (object)record.GetDecimalSafe(field));
		}

		protected override void SetValue(int index, object value)
		{
			base.SetValue(index, GetContainerData(value));
		}
	}
}
