using UnityEngine;

public class PartsLabelWithColorItemView : PartsLabelItemView
{
	public tk2dSprite checkSprite;

	public bool clampValuesWhenAfford;

	private void Awake()
	{
		if ((bool)checkSprite)
		{
			checkSprite.gameObject.SetActive(false);
		}
	}

	protected override void SetupPriceLabel(ItemCollectionDataModel.Item priceData, bool showAvailable = false)
	{
		UserProfile player = UserProfile.player;
		if (player.CanAffordItem(priceData))
		{
			int num = ((!clampValuesWhenAfford) ? player.inventory.GetItem(priceData.itemType, priceData.itemId) : priceData.amount);
			priceLabel.color = Color.green;
			priceLabel.text = num + PRICE_LABEL_SEPARATOR + priceData.amount;
			checkSprite.gameObject.SetActive(true);
		}
		else
		{
			checkSprite.gameObject.SetActive(false);
			base.SetupPriceLabel(priceData, showAvailable);
			priceLabel.color = Color.red;
		}
	}
}
