using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class GachaResultPopUpController : PopupController
{
	private const int MAX_ITEMS = 3;

	private const float TIME_PER_ITEM = 0.5f;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private GameObject closeButton;

	[SerializeField]
	private PartsVehicleDisplayController partsVehicle;

	public GachaUnitController[] gachaUnitControllers;

	public PriceLabelController[] priceLabelControllers;

	private GachaRewardsSceneModel gachaRewardsDataModel;

	[SerializeField]
	private ScrollableAreaController scrollableAreaResults;

	private Tweener scrollTween;

	[SerializeField]
	private GameObject regularBackground;

	[SerializeField]
	private GameObject raidBossBackground;

	protected override void Start()
	{
		base.Start();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	public void Init()
	{
		gachaRewardsDataModel = (GachaRewardsSceneModel)model.payload;
		if ((bool)_title && !string.IsNullOrEmpty(model.title))
		{
			_title.text = model.title.Localize("SUMMARY");
		}
		if (gachaRewardsDataModel == null)
		{
			gachaRewardsDataModel = new GachaRewardsSceneModel(GachaTypes.REGULAR);
			gachaRewardsDataModel.gachaRewards = new ItemCollectionDataModel();
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Unit, 42005, 1));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Unit, 42003, 1));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Unit, 42003, 1));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Unit, 42003, 1));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 901, 2));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Unit, 42003, 1));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 901, 2));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Unit, 42003, 1));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Unit, 42003, 1));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Unit, 42005, 1));
			gachaRewardsDataModel.gachaRewards.AddItem(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, 901, 2));
		}
		SetupBackground();
		SetupScrollableArea();
	}

	protected void SetupBackground()
	{
		if ((bool)regularBackground)
		{
			regularBackground.gameObject.SetActive(gachaRewardsDataModel.gachaType != GachaTypes.RAID_BOSS);
		}
		if ((bool)raidBossBackground)
		{
			raidBossBackground.gameObject.SetActive(gachaRewardsDataModel.gachaType == GachaTypes.RAID_BOSS);
		}
	}

	public void SetupScrollableArea()
	{
		if (gachaRewardsDataModel == null)
		{
			return;
		}
		if ((bool)scrollableAreaResults)
		{
			if (gachaRewardsDataModel.prizeGachaPoolsDataModel != null && gachaRewardsDataModel.prizeGachaPoolsDataModel.bundleGiftId != 0)
			{
				ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(gachaRewardsDataModel.prizeGachaPoolsDataModel.bundleGiftId);
				foreach (ItemCollectionDataModel.Item item in giftPackage.items)
				{
					gachaRewardsDataModel.gachaRewards.AddItem(item);
				}
			}
			int count = gachaRewardsDataModel.gachaRewards.items.Count;
			if (gachaRewardsDataModel.gachaRewards.items.Count < 4)
			{
				scrollableAreaResults.collider.enabled = false;
			}
			int num = 3 - count;
			if (num > 0)
			{
				float x = (float)num * 0.5f * scrollableAreaResults.cellWidth;
				scrollableAreaResults.padding = new Vector2(x, scrollableAreaResults.padding.y);
			}
			scrollableAreaResults.DataSource = GetGachaResults(gachaRewardsDataModel.gachaRewards);
			if (gachaRewardsDataModel.gachaRewards.items.Count < 4)
			{
				scrollableAreaResults.ScrollableArea.scrollBar.gameObject.SetActive(false);
			}
		}
		SetupPartsSequence();
	}

	public List<GachaResultItem> GetGachaResults(ItemCollectionDataModel rewardItemsCollection)
	{
		List<GachaResultItem> list = new List<GachaResultItem>();
		for (int i = 0; i < rewardItemsCollection.items.Count; i++)
		{
			GachaResultItem gachaResultItem = new GachaResultItem();
			gachaResultItem.item = rewardItemsCollection.items[i];
			if (gachaRewardsDataModel.itemBonusLabels.Count > i)
			{
				gachaResultItem.label = gachaRewardsDataModel.itemBonusLabels[i];
			}
			list.Add(gachaResultItem);
		}
		return list;
	}

	public void SetupPartsSequence()
	{
		foreach (ItemCollectionDataModel.Item item in gachaRewardsDataModel.gachaRewards.items)
		{
			if (item.itemType != UserInventory.ItemType.Parts)
			{
				continue;
			}
			UnitRarityDataModel rarity = item.Part.Rarity;
			if (rarity.Type == UnitRarityDataModel.RarityType.SUPERRARE)
			{
				if ((bool)closeButton)
				{
					closeButton.SetActive(false);
				}
				List<UnitPartsDataModel> multiByQuery = NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitPartsDataModel>(" WHERE part_type = " + item.itemId + " AND amount > 0");
				if (multiByQuery.Count > 0)
				{
					partsVehicle.AddItemToQueue(multiByQuery[0].unitId, item.Part, item.amount);
				}
			}
		}
		if (!partsVehicle.finishedAnimating)
		{
			if ((bool)container)
			{
				container.SetActive(false);
			}
			partsVehicle.gameObject.SetActive(true);
		}
		StartCoroutine(WaitForAnimationToFinished());
	}

	private IEnumerator WaitForAnimationToFinished()
	{
		while (!partsVehicle.finishedAnimating)
		{
			yield return new WaitForEndOfFrame();
		}
		if ((bool)closeButton)
		{
			closeButton.SetActive(true);
		}
		if ((bool)container)
		{
			container.SetActive(true);
		}
		int totalItems = gachaRewardsDataModel.gachaRewards.items.Count;
		if (totalItems <= 3)
		{
			yield break;
		}
		int deltaItems = totalItems - 3;
		Tweener scrollTween = SimpleTween.Start(1f, 0f, 0.5f * (float)deltaItems, EaseType.EaseOutExpo, delegate(float val)
		{
			if ((bool)scrollableAreaResults)
			{
				scrollableAreaResults.ContentToPosition(val * scrollableAreaResults.ScrollableArea.ContentLength);
			}
		});
	}

	private void OnDestroy()
	{
		if (scrollTween != null)
		{
			scrollTween.Kill();
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
