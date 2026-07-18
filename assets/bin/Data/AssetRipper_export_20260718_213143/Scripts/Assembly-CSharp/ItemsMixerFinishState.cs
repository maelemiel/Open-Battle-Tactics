using System.Collections;

public class ItemsMixerFinishState : GachaPlinkoBaseState
{
	public override IEnumerator StartStateSequence()
	{
		ItemCollectionDataModel.Item awardedItem = base.ItemsMixer.ItemsMixerItemResult;
		if (awardedItem == null)
		{
			Log.Error("[ItemsMixerFinishState] Trying to finish Items Mixer logic with no rewards", base.gameObject);
			PopupManager.ShowPopup(PopupDataModel.Ok("Error", "Trying to finish Items Mixer logic with no rewards", LoadScene));
		}
		GachaRewardsSceneModel gachaRewardsModel = new GachaRewardsSceneModel(GachaTypes.ITEMS_MIXER, new ItemCollectionDataModel(awardedItem), null);
		PopupManager.ShowPopup(PopupDataModel.GachaResult(gachaRewardsModel, "ui_drop_n_swap_results", LoadScene));
		yield break;
	}

	private void LoadScene()
	{
		Singleton<SessionManager>.instance.GetNextItemsMixerSet(base.ItemsMixer.gachaResultsPayload.prizeGachaPoolsDataModel.eventId, delegate(ItemCollectionDataModel result)
		{
			base.ItemsMixer.gachaResultsPayload.SetRewards(result, base.ItemsMixer.gachaResultsPayload.prizeGachaPoolsDataModel);
		});
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ItemsMixer, new ItemMixerSceneModel(base.ItemsMixer.gachaResultsPayload), false, true);
	}
}
