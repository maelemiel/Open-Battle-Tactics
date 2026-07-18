using UnityEngine;

public class ItemsMixerPriceItemCell : ScrollableCell
{
	private const char SEPARATOR = '/';

	[SerializeField]
	private Color enoughPartsColor = Color.green;

	[SerializeField]
	private Color notEnoughPartsColor = Color.red;

	[SerializeField]
	private PriceLabelController priceLabel;

	[SerializeField]
	private tk2dTextMesh cellText;

	[SerializeField]
	private tk2dTextMesh priceText;

	[SerializeField]
	private tk2dBaseSprite backgroundSprite;

	[SerializeField]
	private tk2dBaseSprite chipSprite;

	[SerializeField]
	private Color notEnoughPartsBackgroundColor = Color.white;

	[SerializeField]
	private Color enoughPartsBackgroundColor = Color.white;

	[SerializeField]
	private GameObject contentGameObject;

	[SerializeField]
	private Collider dragItemCellCollider;

	[SerializeField]
	private GameObject chipCollider;

	private bool enoughParts;

	public ItemCollectionDataModel.Item Item { get; set; }

	public GachaPlinkoPrizePriceDataModel ItemPrizePriceDataModel { get; set; }

	public override void ConfigureCellData()
	{
		base.ConfigureCellData();
		SetChipColliderState(false);
		ItemPrizePriceDataModel = (GachaPlinkoPrizePriceDataModel)dataObject;
		if (ItemPrizePriceDataModel == null)
		{
			return;
		}
		ItemCollectionDataModel.Item item = new ItemCollectionDataModel.Item((UserInventory.ItemType)ItemPrizePriceDataModel.itemType, ItemPrizePriceDataModel.itemId, ItemPrizePriceDataModel.amount);
		if (item != null)
		{
			UserProfile player = UserProfile.player;
			Item = item;
			int item2 = player.inventory.GetItem(Item.itemType, Item.itemId);
			enoughParts = player.CanAffordItem(Item);
			if ((bool)dragItemCellCollider)
			{
				dragItemCellCollider.enabled = enoughParts;
			}
			SetNameLabel();
			SetPriceLabel(enoughParts, item2);
			SetBackgroundState(enoughParts);
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

	private void SetNameLabel()
	{
		if (Item != null && (bool)cellText && Item.Part != null)
		{
			cellText.text = Item.Part.name.Localize();
		}
	}

	private void SetPriceLabel(bool enoughParts, int ownedPartsCount)
	{
		if ((bool)priceLabel)
		{
			priceLabel.ConfigurePriceLabel(new ItemCollectionDataModel(Item));
		}
		if ((bool)priceText)
		{
			if (enoughParts)
			{
				priceText.text = string.Format("{0}" + '/' + "{1}", Item.amount, ownedPartsCount);
				priceText.color = enoughPartsColor;
			}
			else
			{
				priceText.text = string.Format("{0}" + '/' + "{1}", ownedPartsCount, Item.amount);
				priceText.color = notEnoughPartsColor;
			}
		}
	}

	private void SetBackgroundState(bool state)
	{
		Color backgroundColor = ((!state) ? notEnoughPartsBackgroundColor : enoughPartsBackgroundColor);
		SetBackgroundColor(backgroundColor);
	}

	public void SetChipColor(Color color)
	{
		if ((bool)chipSprite)
		{
			chipSprite.color = color;
		}
	}

	public void SetBackgroundColor(Color color)
	{
		if ((bool)backgroundSprite)
		{
			backgroundSprite.color = color;
		}
	}

	public void RestoreBackgroundColor()
	{
		SetBackgroundState(enoughParts);
	}

	public void SetContentState(bool state)
	{
		if ((bool)contentGameObject)
		{
			contentGameObject.SetActive(state);
		}
	}

	public void SetChipColliderState(bool state)
	{
		if ((bool)chipCollider)
		{
			chipCollider.SetActive(state);
		}
	}
}
