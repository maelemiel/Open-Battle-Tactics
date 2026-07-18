using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectPathData : ScriptableObject
{
	[Serializable]
	private class DataItem
	{
		public EffectType type;

		public string path;
	}

	[SerializeField]
	private List<DataItem> pathList = new List<DataItem>();

	private Dictionary<EffectType, string> pathLookup;

	public void AddPath(EffectType type, string path)
	{
		DataItem dataItem = new DataItem();
		dataItem.type = type;
		dataItem.path = path;
		pathList.Add(dataItem);
	}

	public string GetPath(EffectType type)
	{
		if (pathLookup == null)
		{
			pathLookup = new Dictionary<EffectType, string>();
			foreach (DataItem path in pathList)
			{
				pathLookup[path.type] = path.path;
			}
		}
		if (!pathLookup.ContainsKey(type))
		{
			Log.Error("The effect with key: " + type.ToString() + " is not in the Effects Dictionary. Is it an AssetBundle correctly configured in metadata?");
		}
		return pathLookup[type];
	}
}
