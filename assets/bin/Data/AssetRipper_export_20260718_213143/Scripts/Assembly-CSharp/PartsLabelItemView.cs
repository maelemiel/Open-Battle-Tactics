public class PartsLabelItemView : PriceLabelItemView
{
	protected readonly string PRICE_LABEL_SEPARATOR = "/";

	public tk2dTextMesh priceLabel;

	public PrefabProxy prefabProxy;

	public tk2dTextMesh partNameLabel;

	public string prefix = string.Empty;

	private bool waitForEnable;

	private ItemCollectionDataModel.Item priceData;

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

	private void OnEnable()
	{
		if (waitForEnable)
		{
			waitForEnable = false;
			StartCoroutine(prefabProxy.ChangeAssetCoroutine(priceData.Part.AssetLinkage));
		}
	}

	protected override void SetupPriceIcon(ItemCollectionDataModel.Item priceData)
	{
		if (!prefabProxy)
		{
			return;
		}
		if (priceData.Part == null)
		{
			Log.Warning("Part with ID: " + priceData.itemId + " is null");
			return;
		}
		this.priceData = priceData;
		if ((bool)partNameLabel)
		{
			partNameLabel.text = priceData.Part.Name;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			waitForEnable = true;
		}
		else
		{
			StartCoroutine(prefabProxy.ChangeAssetCoroutine(priceData.Part.AssetLinkage));
		}
	}

	protected override void SetupPriceLabel(ItemCollectionDataModel.Item priceData, bool showAvailable = false)
	{
		if ((bool)priceLabel)
		{
			if (!showAvailable)
			{
				priceLabel.text = prefix + priceData.amount;
				return;
			}
			UserProfile player = UserProfile.player;
			int num = player.inventory.GetItem(priceData.itemType, priceData.itemId);
			priceLabel.text = string.Format("{0}{1}{2}", num, PRICE_LABEL_SEPARATOR, priceData.amount);
		}
	}
}
