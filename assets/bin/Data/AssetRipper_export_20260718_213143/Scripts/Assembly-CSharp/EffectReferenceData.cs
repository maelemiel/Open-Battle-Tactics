using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectReferenceData : ScriptableObject
{
	[Serializable]
	public class DataItem
	{
		public EffectType type;

		public GameObject prefab;

		public bool isBundle;
	}

	public List<DataItem> referenceList = new List<DataItem>();

	public GameObject GetReference(EffectType type)
	{
		foreach (DataItem reference in referenceList)
		{
			if (reference.type == type)
			{
				return reference.prefab;
			}
		}
		return null;
	}

	public void SetReference(EffectType type, GameObject obj)
	{
		foreach (DataItem reference in referenceList)
		{
			if (reference.type == type)
			{
				reference.prefab = obj;
				return;
			}
		}
		referenceList.Add(new DataItem
		{
			type = type,
			prefab = obj
		});
	}

	public DataItem GetData(EffectType type)
	{
		foreach (DataItem reference in referenceList)
		{
			if (reference.type == type)
			{
				return reference;
			}
		}
		DataItem dataItem = new DataItem();
		dataItem.type = type;
		referenceList.Add(dataItem);
		return dataItem;
	}
}
