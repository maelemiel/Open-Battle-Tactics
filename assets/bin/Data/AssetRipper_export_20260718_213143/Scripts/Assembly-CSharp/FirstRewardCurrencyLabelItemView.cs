using UnityEngine;

public class FirstRewardCurrencyLabelItemView : CurrencyLabelItemView
{
	[SerializeField]
	private tk2dSprite[] _priceIcons;

	protected override void SetupPriceIcon(ItemCollectionDataModel.Item priceData)
	{
		for (int i = 0; i < _priceIcons.Length; i++)
		{
			_priceIcons[i].SetSprite(priceData.itemType.GetIconName());
		}
	}
}
