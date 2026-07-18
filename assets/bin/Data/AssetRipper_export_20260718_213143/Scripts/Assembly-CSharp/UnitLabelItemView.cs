public class UnitLabelItemView : PriceLabelItemView
{
	protected readonly string PRICE_LABEL_SEPARATOR = "/";

	public tk2dTextMesh priceLabel;

	public UnitProxy prefabProxy;

	public tk2dTextMesh unitNameLabel;

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
		if (!prefabProxy)
		{
			return;
		}
		if (itemData.Unit == null)
		{
			Log.Warning(string.Concat("Unit with unit of: ", itemData.Unit, " is null"));
			return;
		}
		if ((bool)unitNameLabel)
		{
			unitNameLabel.text = itemData.Unit.UnitDataModel.name;
		}
		StartCoroutine(prefabProxy.ChangeAssetCoroutine(itemData.Unit.assetBundleId));
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
