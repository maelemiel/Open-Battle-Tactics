using System;
using System.Collections;
using UnityEngine;

public class PromoteUnitController : MonoBehaviour
{
	private PromotionLocalData previousPromotionData;

	private bool tokenPopupPlaying;

	[SerializeField]
	private UnitItemPromoteViewController unitItemPromoteView;

	[SerializeField]
	private UnitItemUpgradeViewController unitItemUpgradeView;

	private Action parentCallback;

	private Action<bool> parentCancelCallback;

	[SerializeField]
	private Transform[] rewardPositions;

	public void PromoteUnit(UserUnit unit, Action parentCallback, Action<bool> parentCancelCallback)
	{
		previousPromotionData = new PromotionLocalData(unit.AssetBundleID, unit.StartingHealth, unit.level, unit.partialLevel, unit.RollValues, unit.RollTypes, unit.GetUpgradePrice(), unit.UnitDataModel.GetLevel(unit.level - 1));
		this.parentCallback = parentCallback;
		this.parentCancelCallback = parentCancelCallback;
		PromoteUnitRequest(unit);
	}

	private void PromoteUnitRequest(UserUnit unit)
	{
		UserPriceDataModel upgradePrice = unit.GetUpgradePrice();
		if (UserProfile.player.CanAfford(upgradePrice))
		{
			PromoteUnitTransaction(unit, UserPriceDataModel.PaymentType.Normal);
		}
		else if (upgradePrice.HasItemOfType(UserInventory.ItemType.Parts))
		{
			PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_parts_title".Localize("Not enough parts"), "ui_not_enough_parts_desc".Localize("Would you like get more?"), delegate
			{
				parentCancelCallback(true);
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
			}, delegate
			{
				parentCancelCallback(false);
			}, delegate
			{
				parentCancelCallback(false);
			}));
		}
		else if (upgradePrice.HasItemOfType(UserInventory.ItemType.Coins))
		{
			PurchaseWithGems(unit, upgradePrice);
		}
		else
		{
			Log.ErrorTag("No Handler for upgrade cost of type: " + upgradePrice.PrintItems(), null, "PromoteUnitController");
			if (parentCancelCallback != null)
			{
				parentCancelCallback(false);
			}
		}
	}

	private void PurchaseWithGems(UserUnit unit, UserPriceDataModel upgradePrice)
	{
		bool refreshPromoteButton = true;
		UserPriceDataModel premiumPrice = UserPriceDataModel.GetPremiumPrice(upgradePrice, UserProfile.player, unit.GetUpgradeGemExchangeRate());
		PopupManager.ShowPopup(PopupDataModel.PriceConfirmationPopUp(premiumPrice, "ui_not_enough_cash_title".Localize("Not enough cash"), "ui_not_enough_cash_desc".Localize("Pay with gems?"), delegate
		{
			if (UserProfile.player.CanAfford(premiumPrice))
			{
				refreshPromoteButton = false;
				PromoteUnitTransaction(unit, UserPriceDataModel.PaymentType.UsePremiumForDifference);
				Reporting.GemsToCashPopupEvent(PurchaseSource.UnitPromotionPopup, PurchaseAction.purchase, premiumPrice);
			}
			else
			{
				refreshPromoteButton = false;
				PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_gems_title".Localize("Not enough gems"), "ui_not_enough_gems_desc".Localize("Do you want to go buy more gems?"), delegate
				{
					PopupManager.DestroyAllPopups();
					TopBarController.instance.LoadShop();
				}, CallParentCallBack, CallParentCallBack));
			}
		}, delegate
		{
			if (refreshPromoteButton && parentCancelCallback != null)
			{
				parentCancelCallback(false);
			}
		}, delegate
		{
			Reporting.GemsToCashPopupEvent(PurchaseSource.UnitPromotionPopup, PurchaseAction.cancelled, premiumPrice);
		}));
	}

	private void CallParentCallBack()
	{
		if (parentCancelCallback != null)
		{
			parentCancelCallback(false);
		}
	}

	private void PromoteUnitTransaction(UserUnit unit, UserPriceDataModel.PaymentType type)
	{
		Singleton<SessionManager>.instance.UpgradeUnit(unit, type, null);
		UnitPromotedSuccesfully(unit);
	}

	private void UnitPromotedSuccesfully(UserUnit unit)
	{
		int onLevelGiftId = unit.CurrentLevelDataModel.onLevelGiftId;
		if (onLevelGiftId != -1)
		{
			ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(onLevelGiftId);
			UnitPartTypesDataModel unitPartTypesDataModel = null;
			for (int i = 0; i < giftPackage.items.Count; i++)
			{
				if (giftPackage.items[i].itemType == UserInventory.ItemType.Parts && giftPackage.items[i].Part.IsToken)
				{
					unitPartTypesDataModel = giftPackage.items[i].Part;
				}
			}
			if (unitPartTypesDataModel != null)
			{
				tokenPopupPlaying = true;
				PopupManager.ShowPopup(PopupDataModel.ClaimUnitPopUp(null, unitPartTypesDataModel, delegate
				{
					tokenPopupPlaying = false;
				}));
			}
			else
			{
				GrantGiftEffect.Create(this, giftPackage, unitItemPromoteView.transform.position, new Vector3(0f, 500f, 0f), base.gameObject.layer, rewardPositions);
			}
		}
		UserPriceDataModel upgradePrice = unit.GetUpgradePrice();
		int num = ((upgradePrice.items.Count > 0) ? upgradePrice.items[0].amount : 0);
		PromotionLocalData currentPromotionData = new PromotionLocalData(unit.AssetBundleID, unit.StartingHealth, unit.level, unit.partialLevel, unit.RollValues, unit.RollTypes, unit.GetUpgradePrice(), unit.UnitDataModel.GetLevel(unit.level - 1));
		StartCoroutine(UnitPromoteSuccessfullyAnimation(previousPromotionData, currentPromotionData, unit));
	}

	private IEnumerator UnitPromoteSuccessfullyAnimation(PromotionLocalData previousPromotionData, PromotionLocalData currentPromotionData, UserUnit unit)
	{
		while (tokenPopupPlaying)
		{
			yield return new WaitForEndOfFrame();
		}
		if (unit != null)
		{
			if (previousPromotionData.partialLevel != 0)
			{
				yield return StartCoroutine(unitItemUpgradeView.PlayUpgradeEffect(previousPromotionData, currentPromotionData, unit));
			}
			else
			{
				yield return StartCoroutine(unitItemPromoteView.PlayPromoteEffect(previousPromotionData, currentPromotionData, unit));
			}
		}
		if (parentCallback != null)
		{
			parentCallback();
		}
	}

	public IEnumerator UnitPromotePartialLevel(UserUnit unit, int partialLevel, UnitPartialLevelDataModel.PartialLevel partialIncrease, Action cb)
	{
		PromotionLocalData currentPromotionData = new PromotionLocalData(unit.AssetBundleID, unit.StartingHealth, unit.level, unit.partialLevel, unit.RollValues, unit.RollTypes, unit.GetUpgradePrice(), unit.UnitDataModel.GetLevel(unit.level - 1));
		int[] values = unit.RollValues;
		DieFaceType[] types = unit.RollTypes;
		int health = unit.StartingHealth;
		if (partialIncrease != null)
		{
			health += partialIncrease.health;
			for (int i = 0; i < unit.RollTypes.Length; i++)
			{
				values[i] += partialIncrease.diceValues[i];
				types[i] = ((partialIncrease.diceTypes[i] != DieFaceType.None) ? partialIncrease.diceTypes[i] : types[i]);
			}
		}
		previousPromotionData = new PromotionLocalData(unit.AssetBundleID, health, unit.level, unit.partialLevel, values, types, unit.GetUpgradePrice(), unit.UnitDataModel.GetLevel(unit.level - 1));
		yield return StartCoroutine(UnitUpgradeSuccessfullyAnimation(currentPromotionData, previousPromotionData, unit));
		if (cb != null)
		{
			cb();
		}
	}

	private IEnumerator UnitUpgradeSuccessfullyAnimation(PromotionLocalData previousPromotionData, PromotionLocalData currentPromotionData, UserUnit unit)
	{
		while (tokenPopupPlaying)
		{
			yield return new WaitForEndOfFrame();
		}
		if (unit != null)
		{
			yield return StartCoroutine(unitItemUpgradeView.PlayPromoteEffect(previousPromotionData, currentPromotionData, unit));
		}
		if (parentCallback != null)
		{
			parentCallback();
		}
	}
}
