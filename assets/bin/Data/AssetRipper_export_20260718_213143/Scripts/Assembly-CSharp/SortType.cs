using UnityEngine;

public abstract class SortType
{
	public delegate object getComparator(object data);

	public delegate int SortValueDelegate(object data);

	public delegate string SortStringValueDelegate(object data);

	public string label;

	public getComparator sortHandler;

	public GameObject sortButton;

	public bool HasHandler
	{
		get
		{
			return sortHandler != null;
		}
	}

	public abstract int CompareData(object data1, object data2);
}
