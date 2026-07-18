using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUnitPopUp : PopupController
{
	private const string MAX_LEVEL_BUTTON_TEXT = "Max Level";

	private const string POPUP_TITLE_NOT_ENOUGH_UNITS = "Warning!";

	private const string POPUP_TEXT_NOT_ENOUGH_UNITS = "You can't sell this unit. You need at least 4 units to battle";

	private const string PROMOTE_BUTTON_TEXT = "Promote to level {0}!";

	[SerializeField]
	private UnitInfoView unitInfoViewCurrentLevel;

	[SerializeField]
	private UnitInfoView unitInfoViewMaxLevel;

	[SerializeField]
	private PriceLabelController promotePriceLabelController;

	[SerializeField]
	private tk2dUIItem promoteButton;

	[SerializeField]
	private tk2dUIItem scrapButton;

	[SerializeField]
	private tk2dBaseSprite promoteButtonSprite;

	[SerializeField]
	private tk2dBaseSprite disablePromoteButtonSprite;

	[SerializeField]
	private tk2dTextMesh promoteButtonText;

	[SerializeField]
	private tk2dTextMesh promoteText;

	[SerializeField]
	private tk2dTextMesh currentLevelLabel;

	[SerializeField]
	private tk2dTextMesh maxLevelLabel;

	[SerializeField]
	private GameObject arrowsEffect;

	[SerializeField]
	private GameObject currentLevelUnitProxy;

	[SerializeField]
	private Transform sellEffectMarker;

	[SerializeField]
	private tk2dBaseSprite disableScrapButtonSprite;

	[SerializeField]
	private ScrollableAreaController scrollableArea;

	[SerializeField]
	private Transform content;

	[SerializeField]
	private Transform centeredArea;

	private PromoteUnitController promotionController;

	private PurchaseSkinController skinController;

	private UserUnit localUserUnit;

	public GameObject maxLevelUnitInfoPosition;

	public GameObject maxLevelSKinnedUnitInfoPosition;

	private bool unitScrapped;

	private bool scrappingUnit;

	private bool promotingUnit;

	private bool isMaxLevel;

	private List<Action<bool>> skinEquipButtons;

	protected override void Start()
	{
		base.Start();
		localUserUnit = (UserUnit)model.payload;
		skinEquipButtons = new List<Action<bool>>();
		if (localUserUnit != null)
		{
			promotionController = GetComponent<PromoteUnitController>();
			skinController = GetComponent<PurchaseSkinController>();
			if ((bool)unitInfoViewCurrentLevel)
			{
				unitInfoViewCurrentLevel.ConfigureUnitView(localUserUnit.UnitDataModel, localUserUnit.level);
			}
			if ((bool)unitInfoViewMaxLevel)
			{
				unitInfoViewMaxLevel.ConfigureUnitView(localUserUnit.UnitDataModel);
			}
			UpdatePriceLabel();
			UpdatePromotePopUpState();
			if (localUserUnit.Team != null)
			{
				SetScrapButtonState(!localUserUnit.Team.IsOnCooldown);
			}
			else
			{
				SetScrapButtonState(true);
			}
			Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.REWARD, 1);
		}
	}

	private void UpdatePriceLabel()
	{
		if ((bool)promotePriceLabelController)
		{
			UserPriceDataModel upgradePrice = localUserUnit.GetUpgradePrice();
			if (upgradePrice != null)
			{
				promotePriceLabelController.ConfigurePriceLabel(localUserUnit.GetUpgradePrice());
			}
		}
	}

	private void UpdatePromotePopUpState()
	{
		if (localUserUnit.IsMaxLevel)
		{
			List<UnitLevelProgressionDataModel> levels = localUserUnit.UnitDataModel.Levels;
			List<UnitLevelProgressionDataModel> list = new List<UnitLevelProgressionDataModel>();
			bool flag = false;
			for (int i = 0; i < levels.Count; i++)
			{
				if (levels[i].IsSkin)
				{
					flag = true;
					if (!list.Contains(levels[i]) && localUserUnit.level != levels[i].level)
					{
						list.Add(levels[i]);
					}
				}
				if (localUserUnit.MaxLevel == levels[i].level && !list.Contains(levels[i]) && localUserUnit.level != levels[i].level)
				{
					list.Add(levels[i]);
				}
			}
			if (flag)
			{
				SetSkinsScreen(list);
			}
			else
			{
				SetMaxLevelState();
			}
		}
		else
		{
			unitInfoViewCurrentLevel.ConfigureUnitView(localUserUnit.UnitDataModel, localUserUnit.level);
			SetPromoteButtonState(true);
			if ((bool)promoteText)
			{
				promoteText.color = Color.white;
				promoteText.text = string.Format("ui_upgradeunitpopup_promotetolevel".Localize("Promote to level {0}!"), localUserUnit.level + 1);
			}
		}
		promotingUnit = false;
	}

	private void SetSkinsScreen(List<UnitLevelProgressionDataModel> skinLevels)
	{
		TurnOffNonMaxItems();
		DisablePromoteButton();
		scrollableArea.gameObject.SetActive(true);
		scrollableArea.DataSource = skinLevels;
		skinEquipButtons = new List<Action<bool>>();
		if ((bool)maxLevelSKinnedUnitInfoPosition)
		{
			unitInfoViewCurrentLevel.transform.position = maxLevelSKinnedUnitInfoPosition.transform.position;
		}
		foreach (GameObject item in scrollableArea.CellsInUse)
		{
			MaxSkinUnitCell component = item.GetComponent<MaxSkinUnitCell>();
			if ((bool)component)
			{
				component.PurchaseAction = PurchaseSkin;
				skinEquipButtons.Add(component.SetState);
			}
		}
		if (skinLevels.Count == 1)
		{
			content.localPosition = centeredArea.localPosition;
		}
	}

	private void SetMaxLevelState()
	{
		isMaxLevel = true;
		TurnOffNonMaxItems();
		if ((bool)maxLevelUnitInfoPosition)
		{
			unitInfoViewCurrentLevel.transform.position = maxLevelUnitInfoPosition.transform.position;
		}
		SetPromoteButtonMaxLevel();
	}

	private void TurnOffNonMaxItems()
	{
		if ((bool)promoteText)
		{
			promoteText.gameObject.SetActive(false);
		}
		if ((bool)arrowsEffect)
		{
			arrowsEffect.SetActive(false);
		}
		if ((bool)promotePriceLabelController)
		{
			promotePriceLabelController.gameObject.SetActive(false);
		}
		unitInfoViewMaxLevel.gameObject.SetActive(false);
	}

	private void PurchaseSkin(UnitLevelProgressionDataModel skin)
	{
		if (localUserUnit.level == skin.level)
		{
			return;
		}
		if (UserProfile.player.HasUnlockedSkin(skin.id) || skin.level == localUserUnit.MaxLevel)
		{
			Singleton<SessionManager>.instance.SetUnitSkin(localUserUnit, skin, null);
			localUserUnit.SetLevel(skin.level, 0, localUserUnit.boostId);
			UpdatePromotePopUpState();
			unitInfoViewCurrentLevel.ConfigureUnitView(localUserUnit.UnitDataModel, localUserUnit.level);
			return;
		}
		skinController.PurchaseSkin(skin, delegate
		{
			Singleton<SessionManager>.instance.SetUnitSkin(localUserUnit, skin, null);
			localUserUnit.SetLevel(skin.level, 0, localUserUnit.boostId);
			UpdatePromotePopUpState();
			unitInfoViewCurrentLevel.ConfigureUnitView(localUserUnit.UnitDataModel, localUserUnit.level);
		}, delegate
		{
		});
	}

	private void PromoteUnitButton()
	{
		promotingUnit = true;
		SetPromoteButtonState(false);
		SetScrapButtonState(false);
		promotionController.PromoteUnit(localUserUnit, UnitPromoted, UnitPromotedCancelled);
	}

	private void ScrapUnitButton()
	{
		if (UserProfile.player.unitInventory.Count <= Constants.MinUnitsPerTeam)
		{
			PopupManager.ShowPopup(PopupDataModel.One("ui_upgradeunitpopup_sell_warning".Localize("Warning!"), "ui_upgradeunitpopup_sell_notenoughunits".Localize("You can't sell this unit. You need at least 4 units to battle"), "ui_popup_OK".Localize("ok"), null));
			return;
		}
		scrappingUnit = true;
		SetScrapButtonState(false);
		SetPromoteButtonState(false);
		SetSkinButtonsState(false);
		if (isMaxLevel)
		{
			SetPromoteButtonMaxLevel();
		}
		PopupManager.ShowPopup(PopupDataModel.PriceConfirmationPopUp(localUserUnit.GetScrap(), "ui_upgradeunitpopup_sell_confirmation".Localize("Confirmation"), "ui_upgradeunitpopup_sell_areyousure".Localize("Are you sure you want to sell this unit? You'll receive:"), InternalScrapUnit, null, CancelScrapUnit));
	}

	private void CancelScrapUnit()
	{
		scrappingUnit = false;
		Reporting.ScrapUnitEvent(localUserUnit.UnitDataModel.id, localUserUnit.id, "cancel");
		if (!unitScrapped)
		{
			SetSkinButtonsState(true);
			SetScrapButtonState(true);
			SetPromoteButtonState(true);
			UpdatePromotePopUpState();
		}
	}

	private void InternalScrapUnit()
	{
		SetPromoteButtonState(false);
		SetSkinButtonsState(false);
		unitScrapped = Singleton<SessionManager>.instance.ScrapUnit(localUserUnit, delegate(bool result)
		{
			Reporting.ScrapUnitEvent(localUserUnit.UnitDataModel.id, localUserUnit.id, (!result) ? "cancel" : "confirm", localUserUnit.GetScrap());
		});
		if (unitScrapped)
		{
			AudioTrigger.ScrapEarned.Play();
			StartCoroutine(UnitScrapped());
		}
	}

	private void UnitPromoted()
	{
		if (localUserUnit.Team != null)
		{
			SetScrapButtonState(!localUserUnit.Team.IsOnCooldown);
		}
		else
		{
			SetScrapButtonState(true);
		}
		UpdatePromotePopUpState();
	}

	private void UnitPromotedCancelled(bool forceClose)
	{
		if (forceClose)
		{
			OnCloseButton();
			return;
		}
		SetScrapButtonState(true);
		UpdatePromotePopUpState();
	}

	private IEnumerator UnitScrapped()
	{
		if ((bool)unitInfoViewCurrentLevel)
		{
			unitInfoViewCurrentLevel.gameObject.SetActive(false);
		}
		ItemCollectionDataModel scrap = localUserUnit.GetScrap();
		yield return StartCoroutine(CreateEffect(scrap.items, base.gameObject.transform, unitInfoViewCurrentLevel.transform.position, base.gameObject.layer));
		scrappingUnit = false;
		OnCloseButton();
	}

	private IEnumerator CreateEffect(List<ItemCollectionDataModel.Item> items, Transform parent, Vector3 location, int layer)
	{
		PartFoundEffect partsEffect = null;
		CurrencyEffect currencyEffect = null;
		for (int i = 0; i < items.Count; i++)
		{
			switch (items[i].itemType)
			{
			case UserInventory.ItemType.Parts:
			{
				partsEffect = GlobalEffectsManager.Create(EffectType.PART_DROP, location, parent).SetLayer(layer).GetComponent<PartFoundEffect>();
				partsEffect.rowWidth = 6;
				partsEffect.SortingOrder = 53;
				List<UnitPartTypesDataModel> partsResult = new List<UnitPartTypesDataModel>();
				UnitPartTypesDataModel partDataModel = UnitPartTypesDataModel.GetSingle(items[i].itemId);
				if (partDataModel != null)
				{
					for (int c = 0; c < items[i].amount; c++)
					{
						partsResult.Add(partDataModel);
					}
					if ((bool)partsEffect)
					{
						partsEffect.PlayAnimation(partsResult, null);
					}
					yield return new WaitForEndOfFrame();
				}
				break;
			}
			case UserInventory.ItemType.Energy:
			case UserInventory.ItemType.Coins:
			case UserInventory.ItemType.PremiumCurrency:
				if (items[i].itemType == UserInventory.ItemType.Coins)
				{
					EffectInstance rewardAnimEffect = GlobalEffectsManager.Create(EffectType.REWARD, sellEffectMarker.position).AutoDestroy();
					rewardAnimEffect.gameObject.SetLayerRecursively(base.gameObject.layer);
					rewardAnimEffect.SpineAnimation.Skeleton.SortOrder = 7;
				}
				currencyEffect = CurrencyEffect.Create(items[i].itemType, items[i].amount);
				currencyEffect.gameObject.SetLayerRecursively(layer);
				currencyEffect.transform.SetParent(parent);
				currencyEffect.transform.position = location;
				currencyEffect.SortingOrder = 53;
				yield return new WaitForEndOfFrame();
				break;
			}
		}
		while ((partsEffect != null && partsEffect.animating) || (currencyEffect != null && currencyEffect.animating))
		{
			yield return new WaitForEndOfFrame();
		}
	}

	public void SetSkinButtonsState(bool state)
	{
		if (skinEquipButtons != null && skinEquipButtons.Count > 0)
		{
			int i = 0;
			for (int count = skinEquipButtons.Count; i < count; i++)
			{
				skinEquipButtons[i](state);
			}
		}
	}

	public void SetScrapButtonState(bool state)
	{
		if ((bool)scrapButton)
		{
			scrapButton.enabled = state;
			disableScrapButtonSprite.gameObject.SetActive(!state);
		}
	}

	public void SetPromoteButtonState(bool state)
	{
		if ((bool)promoteButton)
		{
			promoteButton.enabled = state;
			SetPromoteButtonStateView(state);
		}
	}

	private void SetPromoteButtonMaxLevel()
	{
		if ((bool)promoteButton)
		{
			SetPromoteButtonState(false);
			promoteButtonText.text = "ui_upgradeunitpopup_maxlevel".Localize("Max Level");
		}
	}

	private void DisablePromoteButton()
	{
		if ((bool)promoteButton)
		{
			promoteButton.gameObject.SetActive(false);
		}
	}

	private void SetPromoteButtonStateView(bool state)
	{
		if ((bool)promoteButtonSprite)
		{
			promoteButtonText.text = string.Empty;
			disablePromoteButtonSprite.gameObject.SetActive(!state);
		}
	}

	public override void OnBackButtonPressed()
	{
		if (allowBackButton && !scrappingUnit && !promotingUnit)
		{
			OnCloseButton();
		}
	}
}
