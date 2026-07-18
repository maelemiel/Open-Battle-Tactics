using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GachaInfoPopUpController : PopupController
{
	[SerializeField]
	private List<GachaInfoView> gachaInfoViews;

	[SerializeField]
	private UnitPositionSet[] unitPositionSets;

	[SerializeField]
	protected ScrollableAreaController scrollableAreaController;

	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < gachaInfoViews.Count; i++)
		{
			gachaInfoViews[i].gameObject.SetActive(false);
		}
		GachaPoolsDataModel prizeGachaPool = (GachaPoolsDataModel)model.payload;
		List<GachaInfoItemsDataModel> all = GachaInfoItemsDataModel.GetAll();
		all = all.FindAll((GachaInfoItemsDataModel x) => x.gachaPoolId == int.Parse(prizeGachaPool.id));
		if (all.Count > gachaInfoViews.Count)
		{
			Log.Error("DataModel count and views count should be consistent");
		}
		if (unitPositionSets.Length > gachaInfoViews.Count)
		{
			Log.Error("Position Sets Length and views count should be consistent");
		}
		GachaInfoItemsDataModel gachaInfoItemsDataModel = null;
		for (int num = 0; num < all.Count; num++)
		{
			gachaInfoItemsDataModel = all[num];
			if (gachaInfoItemsDataModel != null)
			{
				gachaInfoViews[num].gameObject.SetActive(true);
				if (prizeGachaPool.gachaType == 4)
				{
					EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
					if (activeEvent != null && int.Parse(activeEvent.id) == prizeGachaPool.eventId)
					{
						gachaInfoViews[num].ConfigureView(gachaInfoItemsDataModel.Name, gachaInfoItemsDataModel.Description, activeEvent.GetGachaInfoAssetIds[gachaInfoItemsDataModel.eventAssetLinkageIndex]);
					}
				}
				else
				{
					gachaInfoViews[num].ConfigureView(gachaInfoItemsDataModel.Name, gachaInfoItemsDataModel.Description, gachaInfoItemsDataModel.AssetLinkage);
				}
			}
			if (unitPositionSets.Length > num)
			{
				gachaInfoViews[num].transform.position = unitPositionSets[all.Count - 1].transformList[num].position;
			}
		}
		InitializeInfoDetailsViews(prizeGachaPool.id);
	}

	private void InitializeInfoDetailsViews(string gatchaId)
	{
		List<GachaInfoDetailsDataModel> list = (from x in GachaInfoDetailsDataModel.GetAll()
			where x.gachaId.ToString() == gatchaId
			select x).ToList();
		list.Sort((GachaInfoDetailsDataModel x, GachaInfoDetailsDataModel y) => x.displayOrder - y.displayOrder);
		scrollableAreaController.InitializeWithData(list);
		scrollableAreaController.FixScrollLimits();
	}
}
