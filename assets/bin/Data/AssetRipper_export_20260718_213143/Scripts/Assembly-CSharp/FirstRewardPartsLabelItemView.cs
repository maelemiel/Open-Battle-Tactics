using UnityEngine;

public class FirstRewardPartsLabelItemView : PartsLabelItemView
{
	[SerializeField]
	private PrefabProxy[] _prefabProxys;

	[SerializeField]
	private bool _showOnlyOnePart = true;

	[SerializeField]
	private Vector3 _onePartScale = new Vector3(0f, 0f, 0f);

	[SerializeField]
	private Vector3 _onePartPosition = new Vector3(0f, 0f, 0f);

	protected override void SetupPriceIcon(ItemCollectionDataModel.Item priceData)
	{
		base.SetupPriceIcon(priceData);
		if (!_showOnlyOnePart)
		{
			if (priceData.amount >= 9)
			{
				for (int i = 0; i < _prefabProxys.Length; i++)
				{
					StartCoroutine(_prefabProxys[i].ChangeAssetCoroutine(priceData.Part.AssetLinkage));
				}
			}
			else
			{
				for (int j = 0; j < priceData.amount; j++)
				{
					StartCoroutine(_prefabProxys[j].ChangeAssetCoroutine(priceData.Part.AssetLinkage));
				}
			}
		}
		else
		{
			StartCoroutine(_prefabProxys[0].ChangeAssetCoroutine(priceData.Part.AssetLinkage));
			_prefabProxys[0].transform.localPosition = _onePartPosition;
			_prefabProxys[0].transform.localScale = _onePartScale;
		}
	}
}
