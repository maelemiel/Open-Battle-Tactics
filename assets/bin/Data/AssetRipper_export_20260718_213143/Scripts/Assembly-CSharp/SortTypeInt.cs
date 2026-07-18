public class SortTypeInt : SortType
{
	public override int CompareData(object data1, object data2)
	{
		return ((int)data1).CompareTo((int)data2);
	}
}
