public class SortTypeString : SortType
{
	public override int CompareData(object data1, object data2)
	{
		return ((string)data2).CompareTo((string)data1);
	}
}
