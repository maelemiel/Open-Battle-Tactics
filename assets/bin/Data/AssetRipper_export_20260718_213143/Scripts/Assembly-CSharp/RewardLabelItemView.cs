using UnityEngine;

public class RewardLabelItemView : MonoBehaviour
{
	public ItemCollectionDataModel.Item item;

	public virtual void SetupAllItems(ItemCollectionDataModel itemModels, ItemCollectionDataModel.Item itemData, bool showAvailable = false)
	{
		item = itemData;
		SetupItem(itemModels, itemData);
		SetupPriceLabel(itemData, showAvailable);
	}

	protected virtual void SetupItem(ItemCollectionDataModel allItems, ItemCollectionDataModel.Item itemData)
	{
	}

	protected virtual void SetupPriceLabel(ItemCollectionDataModel.Item itemData, bool showAvailable = false)
	{
	}
}
