public class EnergyItemView : PriceLabelItemView
{
	protected readonly string PRICE_LABEL_SEPARATOR = "/";

	public tk2dTextMesh priceLabel;

	public tk2dTextMesh energyNameLabel;

	public string prefix = string.Empty;

	public string Label
	{
		get
		{
			return priceLabel.text;
		}
		set
		{
			priceLabel.text = value;
		}
	}

	protected override void SetupPriceIcon(ItemCollectionDataModel.Item itemData)
	{
		if (itemData.Unit == null)
		{
			Log.Warning(string.Concat("Unit with unit of: ", itemData.Unit, " is null"));
		}
		else if ((bool)energyNameLabel)
		{
			energyNameLabel.text = "metadata_item_name_2".Localize("Ticket");
		}
	}

	protected override void SetupPriceLabel(ItemCollectionDataModel.Item itemData, bool showAvailable = false)
	{
		if ((bool)priceLabel)
		{
			if (!showAvailable)
			{
				priceLabel.text = prefix + itemData.amount;
				return;
			}
			UserProfile player = UserProfile.player;
			int num = player.inventory.GetItem(itemData.itemType, itemData.itemId);
			priceLabel.text = string.Format("{0}{1}{2}", num, PRICE_LABEL_SEPARATOR, itemData.amount);
		}
	}
}
