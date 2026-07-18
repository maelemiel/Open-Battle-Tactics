using UnityEngine;

public class SecondRewardPartsLabelItemView : PartsLabelItemView
{
	[SerializeField]
	private PrefabProxy[] _prefabProxys;

	protected override void SetupPriceIcon(ItemCollectionDataModel.Item priceData)
	{
		base.SetupPriceIcon(priceData);
		for (int i = 0; i < _prefabProxys.Length; i++)
		{
			StartCoroutine(_prefabProxys[i].ChangeAssetCoroutine(priceData.Part.AssetLinkage));
		}
	}
}
