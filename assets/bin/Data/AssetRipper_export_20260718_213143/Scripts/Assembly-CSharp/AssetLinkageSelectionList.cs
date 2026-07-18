using UnityEngine;

public class AssetLinkageSelectionList : SelectionListController<AssetLinkageDataModel>
{
	[SerializeField]
	protected PrefabProxy prefabProxy;

	public override void SetSelectedItem(int index)
	{
		base.SetSelectedItem(index);
		if (dataObjectList == null)
		{
			Log.Warning("Trying to select an item from an empty list", base.gameObject);
		}
		else if ((bool)prefabProxy)
		{
			StopAllCoroutines();
			StartCoroutine(prefabProxy.ChangeAssetCoroutine(GetSelectedItem()));
		}
	}
}
