using System;
using System.Collections.Generic;

public class GachaRewardsSceneModel
{
	public ItemCollectionDataModel gachaRewards;

	public GachaTypes gachaType;

	public GachaPoolsDataModel prizeGachaPoolsDataModel;

	public List<string> itemBonusLabels = new List<string>();

	public event Action OnRewardsAvailable;

	public GachaRewardsSceneModel(GachaTypes gachaType)
	{
		this.gachaType = gachaType;
	}

	public GachaRewardsSceneModel(GachaTypes gachaType, ItemCollectionDataModel gachaRewards, GachaPoolsDataModel prizeGachaPoolsDataModel)
	{
		this.gachaRewards = gachaRewards;
		this.gachaType = gachaType;
		this.prizeGachaPoolsDataModel = prizeGachaPoolsDataModel;
	}

	public void SetRewards(ItemCollectionDataModel gachaRewards, GachaPoolsDataModel prizeGachaModel = null)
	{
		this.gachaRewards = gachaRewards;
		prizeGachaPoolsDataModel = prizeGachaModel;
		if (this.OnRewardsAvailable != null)
		{
			this.OnRewardsAvailable();
		}
	}
}
