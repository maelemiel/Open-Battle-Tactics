using System.Collections.Generic;

public class TierBadgeSelectionList : AssetLinkageSelectionList
{
	private List<ProgressionDivisionDataModel> divisionDataModels;

	protected void Awake()
	{
		divisionDataModels = ProgressionDivisionDataModel.GetAll();
		divisionDataModels.RemoveAll((ProgressionDivisionDataModel x) => x.isHidden == 1);
		List<AssetLinkageDataModel> list = new List<AssetLinkageDataModel>();
		foreach (ProgressionDivisionDataModel divisionDataModel in divisionDataModels)
		{
			list.Add(divisionDataModel.BadgeLinkage);
		}
		Init(list);
	}

	public ProgressionDivisionDataModel GetSelectedDivision()
	{
		if (divisionDataModels != null && divisionDataModels.Count > currentIndex)
		{
			return divisionDataModels[currentIndex];
		}
		return null;
	}
}
