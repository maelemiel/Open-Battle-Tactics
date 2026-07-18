using UnityEngine;

public class PriceLabelItemView : MonoBehaviour
{
	public ItemCollectionDataModel.Item item;

	public virtual void SetupPriceItem(ItemCollectionDataModel.Item priceData, bool showAvailable = false)
	{
		item = priceData;
		SetupPriceIcon(priceData);
		SetupPriceLabel(priceData, showAvailable);
	}

	protected virtual void SetupPriceIcon(ItemCollectionDataModel.Item priceData)
	{
	}

	protected virtual void SetupPriceLabel(ItemCollectionDataModel.Item priceData, bool showAvailable = false)
	{
	}
}
