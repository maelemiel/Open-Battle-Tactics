using NUnit.Framework;

public class CurrencyLabelItemView : PriceLabelItemView
{
	protected readonly string PRICE_LABEL_SEPARATOR = "/";

	public tk2dSprite priceIcon;

	public tk2dTextMesh priceLabel;

	public tk2dTextMesh currencyNameLabel;

	public string prePend = string.Empty;

	public override void SetupPriceItem(ItemCollectionDataModel.Item priceData, bool showAvailable = false)
	{
		base.SetupPriceItem(priceData);
		SetupPriceIcon(priceData);
		SetupPriceLabel(priceData, showAvailable);
	}

	protected override void SetupPriceIcon(ItemCollectionDataModel.Item priceData)
	{
		if ((bool)priceIcon)
		{
			priceIcon.SetSprite(priceData.itemType.GetIconName());
		}
	}

	protected override void SetupPriceLabel(ItemCollectionDataModel.Item priceData, bool showAvailable = false)
	{
		if ((bool)priceLabel)
		{
			if (!showAvailable)
			{
				priceLabel.text = prePend + priceData.amount;
			}
			else
			{
				UserProfile player = UserProfile.player;
				Assert.True(player != null, "A userProfile is needed in order to configure this priceLabel");
				int num = player.inventory.GetItem(priceData.itemType, priceData.itemId);
				priceLabel.text = string.Format("{0}{1}{2}", num, PRICE_LABEL_SEPARATOR, priceData.amount);
			}
		}
		if ((bool)currencyNameLabel)
		{
			currencyNameLabel.text = priceData.itemType.GetLocalizedName();
		}
	}
}
