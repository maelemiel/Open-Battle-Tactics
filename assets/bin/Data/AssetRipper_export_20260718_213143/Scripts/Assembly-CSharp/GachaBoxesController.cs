using System.Collections.Generic;
using UnityEngine;

public class GachaBoxesController : MonoBehaviour
{
	public GachaBoxResource[] gachaBoxResources;

	private Dictionary<GachaTypes, string> gachaBoxResourcesDictionary = new Dictionary<GachaTypes, string>();

	private void Awake()
	{
		for (int i = 0; i < gachaBoxResources.Length; i++)
		{
			gachaBoxResourcesDictionary.Add(gachaBoxResources[i].gachaType, gachaBoxResources[i].gachaBoxResourceName);
		}
	}

	public tk2dSpineSkeletonDataAsset GetGachaBoxSkeletonDataAsset(GachaTypes gachaType)
	{
		if (!gachaBoxResourcesDictionary.ContainsKey(gachaType))
		{
			Log.Error("The box resource with type: " + gachaType.ToString() + " doesn't exist", base.gameObject);
			return null;
		}
		string path = gachaBoxResourcesDictionary[gachaType];
		return Resources.Load(path) as tk2dSpineSkeletonDataAsset;
	}
}
