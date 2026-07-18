using System;

public class ItemMixerSceneModel : SceneModel
{
	public GachaRewardsSceneModel gachaRewardsSceneModel;

	public string forcePartId;

	public Action onExit;

	public ItemMixerSceneModel()
	{
	}

	public ItemMixerSceneModel(GachaRewardsSceneModel gachaRewardsSceneModel)
	{
		this.gachaRewardsSceneModel = gachaRewardsSceneModel;
	}

	public ItemMixerSceneModel(GachaRewardsSceneModel gachaRewardsSceneModel, string forcePartId, Action onExit)
	{
		this.gachaRewardsSceneModel = gachaRewardsSceneModel;
		this.forcePartId = forcePartId;
		this.onExit = onExit;
	}
}
