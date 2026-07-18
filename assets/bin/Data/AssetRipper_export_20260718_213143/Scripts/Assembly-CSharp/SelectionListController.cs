using System.Collections.Generic;
using UnityEngine;

public class SelectionListController<T> : MonoBehaviour
{
	protected int currentIndex;

	protected int itemCount;

	protected IList<T> dataObjectList;

	public int SelectedIndex
	{
		get
		{
			return currentIndex;
		}
	}

	protected void Init(IList<T> dataObjectList)
	{
		this.dataObjectList = dataObjectList;
		itemCount = dataObjectList.Count;
		currentIndex = 0;
	}

	public virtual void Start()
	{
		SetSelectedItem(currentIndex);
	}

	public int GetItemIndex(T item)
	{
		for (int i = 0; i < dataObjectList.Count; i++)
		{
			if (dataObjectList[i].Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	protected virtual void OnPrevious()
	{
		if (itemCount != 0)
		{
			currentIndex = (currentIndex + (itemCount - 1)) % itemCount;
			SetSelectedItem(currentIndex);
		}
	}

	protected virtual void OnNext()
	{
		if (itemCount != 0)
		{
			currentIndex++;
			currentIndex %= itemCount;
			SetSelectedItem(currentIndex);
		}
	}

	public virtual T GetSelectedItem()
	{
		if (dataObjectList != null && dataObjectList.Count > currentIndex)
		{
			return dataObjectList[currentIndex];
		}
		return default(T);
	}

	public virtual void SetSelectedItem(int index)
	{
		currentIndex = index;
	}
}
