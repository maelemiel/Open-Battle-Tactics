public class GachaInfoLabelItemView : PriceLabelItemView
{
	public PrefabProxy prefabProxy;

	public tk2dTextMesh partNameLabel;

	private bool waitForEnable;

	private ItemCollectionDataModel.Item priceData;

	private void OnEnable()
	{
		if (waitForEnable)
		{
			waitForEnable = false;
			StartCoroutine(prefabProxy.ChangeAssetCoroutine(priceData.Part.AssetLinkage));
		}
	}

	public void SetupGachaInfoCell(GachaInfoDetailsDataModel gachaData)
	{
		if ((bool)prefabProxy)
		{
			if ((bool)partNameLabel)
			{
				partNameLabel.text = LocalizationManager.GetString(gachaData.keyText, "PRIZE INFO");
			}
			if (!base.gameObject.activeInHierarchy)
			{
				waitForEnable = true;
			}
			else
			{
				StartCoroutine(prefabProxy.ChangeAssetCoroutine(AssetLinkageDataModel.GetSingle(gachaData.assetId)));
			}
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
}
