using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopItemsSceneController : SceneController
{
	public static ShopItemsSceneController instance;

	[SerializeField]
	private ScrollableAreaController shopItemScrollableAreaController;

	[SerializeField]
	private GameObject sponsorPayGameObject;

	private Dictionary<int, UserStepGacha> userStepGachas;

	public override void Awake()
	{
		instance = this;
		allowsBackButton = true;
		base.Awake();
	}

	private void Start()
	{
		Singleton<SponsorPayManager>.instance.Init();
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Singleton<SessionManager>.instance.GetStepGachas(delegate(Dictionary<int, UserStepGacha> userStepGachasList)
			{
				userStepGachas = userStepGachasList;
				if ((bool)base.gameObject)
				{
					Init();
				}
			});
		});
		AudioTrigger.MenuBackground_Music.PlayMusic();
	}

	private void Init()
	{
		List<GachaPoolsDataModel> gachaPrizesMetadata = (from x in GachaPoolsDataModel.GetAll()
			where UserProfile.player.GetGachaPrizeData(int.Parse(x.id)).ShowInList(userStepGachas)
			orderby x.orderPosition
			select x).ToList();
		gachaPrizesMetadata = SetupGachaABTesting(gachaPrizesMetadata);
		int usedCount = 0;
		int num = 1;
		List<float> adjustedSize = new List<float>();
		List<float> list = new List<float>();
		for (int num2 = 0; num2 < gachaPrizesMetadata.Count; num2++)
		{
			usedCount = AddToModifiedList(gachaPrizesMetadata[num2], adjustedSize, list, usedCount);
			if (gachaPrizesMetadata[num2].gachaType == 4)
			{
				EventDataModel activeOnCooldownEvent = UserProfile.player.GetActiveOnCooldownEvent();
				switch (num)
				{
				case 1:
					gachaPrizesMetadata[num2].assetLinkageId = activeOnCooldownEvent.Assets.gachaAssetBundle1;
					break;
				case 2:
					gachaPrizesMetadata[num2].assetLinkageId = activeOnCooldownEvent.Assets.gachaAssetBundle2;
					break;
				}
				num++;
			}
		}
		if ((bool)shopItemScrollableAreaController)
		{
			shopItemScrollableAreaController.adjustedSize = adjustedSize;
			shopItemScrollableAreaController.adjustedPlacement = list;
			shopItemScrollableAreaController.InitializeWithData(gachaPrizesMetadata);
		}
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
		}
		if (this != null)
		{
			StartCoroutine(AnnouncerController.DialogTrigger("GachaShop"));
		}
	}

	private List<GachaPoolsDataModel> SetupGachaABTesting(List<GachaPoolsDataModel> gachaPrizesMetadata)
	{
		List<GachaPoolsDataModel> list = new List<GachaPoolsDataModel>();
		foreach (GachaPoolsDataModel gachaPrizesMetadatum in gachaPrizesMetadata)
		{
			bool flag = true;
			if (gachaPrizesMetadatum.abTestingId != 0)
			{
				GachaAbTestingDataModel single = GachaAbTestingDataModel.GetSingle(gachaPrizesMetadatum.abTestingId);
				if (single != null && !single.UserIsInABTesting)
				{
					flag = false;
				}
			}
			if (flag)
			{
				list.Add(gachaPrizesMetadatum);
			}
		}
		return list;
	}

	private int AddToModifiedList(GachaPoolsDataModel gachaPrizesMetadata, List<float> adjustedSize, List<float> adjustedPosition, int usedCount)
	{
		adjustedSize.Add((float)gachaPrizesMetadata.size / 100f);
		if (usedCount == 0)
		{
			adjustedPosition.Add(0f);
			adjustedPosition.Add(adjustedSize[usedCount]);
		}
		else
		{
			adjustedPosition.Add(adjustedSize[usedCount] + adjustedPosition[usedCount]);
		}
		return usedCount + 1;
	}

	public void ChangeListButtonStatus(bool enable)
	{
		List<PrizeGachaCell> cellComponents = shopItemScrollableAreaController.GetCellComponents<PrizeGachaCell>();
		foreach (PrizeGachaCell item in cellComponents)
		{
			item.enabled = enable;
		}
	}

	public void Confirm()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
	}

	private void OnDestroy()
	{
		instance = null;
		Object.Destroy(sponsorPayGameObject);
	}
}
