public class GachaInfoCellController : ScrollableCell
{
	private GachaInfoLabelItemView gachaLabel;

	private PrefabProxy partsProxy;

	private void Awake()
	{
		gachaLabel = GetComponent<GachaInfoLabelItemView>();
	}

	public override void ConfigureCellData()
	{
		GachaInfoDetailsDataModel gachaInfoDetailsDataModel = (GachaInfoDetailsDataModel)dataObject;
		if (gachaInfoDetailsDataModel != null)
		{
			gachaLabel.SetupGachaInfoCell(gachaInfoDetailsDataModel);
		}
	}
}
