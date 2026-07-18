using UnityEngine;

public class ItemsMixerRewardItemCell : ScrollableCell
{
	[SerializeField]
	private PriceLabelController priceLabel;

	[SerializeField]
	private tk2dTextMesh cellText;

	[SerializeField]
	private tk2dBaseSprite backgroundSprite;

	[SerializeField]
	private tk2dBaseSprite chipSprite;

	[SerializeField]
	private GameObject contentGameObject;

	public ItemCollectionDataModel.Item Item { get; set; }

	public GachaPlinkoPrizesDataModel ItemPrizeDataModel { get; set; }

	public override void ConfigureCellData()
	{
		base.ConfigureCellData();
		ItemPrizeDataModel = (GachaPlinkoPrizesDataModel)dataObject;
		if (ItemPrizeDataModel == null)
		{
			return;
		}
		ItemCollectionDataModel.Item item = new ItemCollectionDataModel.Item((UserInventory.ItemType)ItemPrizeDataModel.itemType, ItemPrizeDataModel.itemId, 1);
		if (item != null)
		{
			Item = item;
			if ((bool)priceLabel)
			{
				priceLabel.ConfigurePriceLabel(new ItemCollectionDataModel(item));
			}
			if ((bool)cellText && item.Part != null)
			{
				cellText.text = item.Part.name.Localize();
			}
		}
	}

	public void SetDraggingState(bool state, Vector3 dragPosition, bool isLocalPosition = false)
	{
		SetContentState(!state);
		if (isLocalPosition)
		{
			dragPosition = base.transform.TransformPoint(dragPosition);
		}
		priceLabel.transform.position = dragPosition;
	}

	public void SetContentState(bool state)
	{
		if ((bool)contentGameObject)
		{
			contentGameObject.SetActive(state);
		}
	}

	public void SetBackgroundColor(Color color)
	{
		if ((bool)backgroundSprite)
		{
			backgroundSprite.color = color;
		}
	}

	public void SetChipState(bool state)
	{
		if ((bool)chipSprite)
		{
			chipSprite.gameObject.SetActive(state);
		}
	}

	public void SetChipColor(Color color)
	{
		if ((bool)chipSprite)
		{
			chipSprite.color = color;
		}
	}

	public void RestoreBackgroundColor()
	{
		SetBackgroundColor(Color.white);
	}
}
