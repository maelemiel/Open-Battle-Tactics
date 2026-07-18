using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUnitPartsPopUp : PopupController
{
	private enum PopUpStates
	{
		None = 0,
		CashUpgrade = 1,
		PartsUpgrade = 2,
		MaxLevel = 3,
		Skins = 4
	}

	private const string POPUP_TITLE_NOT_ENOUGH_UNITS = "Warning!";

	private const string POPUP_TEXT_NOT_ENOUGH_UNITS = "You can't sell this unit. You need at least 4 units to battle";

	private const string PROMOTE_BUTTON_TEXT = "Promote to level {0}!";

	[SerializeField]
	private UpgradeUnitCashController unitCashController;

	[SerializeField]
	private UpgradeUnitPartsController unitPartsController;

	[SerializeField]
	private UpgradeUnitMaxLevelController unitMaxLevelController;

	[SerializeField]
	private UpgradeUnitSkinsController unitSkinsController;

	[SerializeField]
	private UnitInfoView unitInfoViewCurrentLevel;

	[SerializeField]
	private tk2dUIItem scrapButton;

	[SerializeField]
	private tk2dUIItem toggleNextLevelButton;

	[SerializeField]
	private Transform sellEffectMarker;

	[SerializeField]
	private tk2dBaseSprite disableScrapButtonSprite;

	[SerializeField]
	private tk2dBaseSprite disableviewMaxButtonSprite;

	private UserUnit localUserUnit;

	private bool unitScrapped;

	private bool scrappingUnit;

	private bool viewingNext;

	private PopUpStates currentState;

	private IUpgradeUnitPartsContoller CurrentScreen
	{
		get
		{
			switch (currentState)
			{
			case PopUpStates.CashUpgrade:
				return unitCashController;
			case PopUpStates.PartsUpgrade:
				return unitPartsController;
			case PopUpStates.MaxLevel:
				return unitMaxLevelController;
			case PopUpStates.Skins:
				return unitSkinsController;
			default:
				return null;
			}
		}
	}

	protected override void Start()
	{
		base.Start();
		localUserUnit = (UserUnit)model.payload;
		if (localUserUnit == null)
		{
			return;
		}
		if ((bool)unitInfoViewCurrentLevel)
		{
			unitInfoViewCurrentLevel.ConfigureUnitView(localUserUnit.UnitDataModel, localUserUnit.level, localUserUnit.partialLevel);
			tk2dTextMesh[] componentsInChildren = toggleNextLevelButton.GetComponentsInChildren<tk2dTextMesh>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].text = string.Format((!viewingNext) ? "ui_upgrade_next_level".Localize("NEXT LEVEL") : "ui_upgrade_current_level".Localize("CURRENT LEVEL"));
			}
		}
		unitPartsController.SetupScreen(localUserUnit, UpgradeStart, ClosePopUpButton, SetScrapButtonState, UnitViewChange);
		unitCashController.SetupScreen(localUserUnit, UpgradeStart, ClosePopUpButton, SetScrapButtonState, UnitViewChange);
		unitMaxLevelController.SetupScreen(localUserUnit, UpgradeStart, ClosePopUpButton, SetScrapButtonState, UnitViewChange);
		unitSkinsController.SetupScreen(localUserUnit, UpgradeStart, ClosePopUpButton, SetScrapButtonState, UnitViewChange);
		UpdateCurrentScreen();
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

	private void UpgradeStart()
	{
		ToggleNext(true);
	}

	private void UpdateCurrentScreen()
	{
		List<UnitPartialLevelDataModel> partialLevelsForUnit = UnitPartialLevelDataModel.GetPartialLevelsForUnit(localUserUnit.metadataId, localUserUnit.level);
		bool flag = localUserUnit.IsMaxLevel && (partialLevelsForUnit == null || partialLevelsForUnit.Count == 0);
		if (flag && UnitLevelProgressionDataModel.UnitHasSkins(localUserUnit.metadataId))
		{
			SetScreen(PopUpStates.Skins);
		}
		else if (flag)
		{
			SetScreen(PopUpStates.MaxLevel);
		}
		else if (partialLevelsForUnit != null && partialLevelsForUnit.Count > 0)
		{
			SetScreen(PopUpStates.PartsUpgrade);
		}
		else
		{
			SetScreen(PopUpStates.CashUpgrade);
		}
		if (localUserUnit.IsMaxLevel)
		{
			toggleNextLevelButton.gameObject.SetActive(false);
		}
	}

	[ContextMenu("Everybody get naked!!")]
	public void TestAnimation()
	{
		currentState = PopUpStates.CashUpgrade;
		unitCashController.ToggleScreen(false);
		unitPartsController.ToggleScreen(false);
		unitMaxLevelController.ToggleScreen(false);
		unitSkinsController.ToggleScreen(false);
		CurrentScreen.ToggleScreen(true);
		unitInfoViewCurrentLevel.transform.position = CurrentScreen.GetUnitAnchor().position;
		SetScreen(PopUpStates.PartsUpgrade);
	}

	private void SetScreen(PopUpStates state)
	{
		if (state != currentState && currentState != PopUpStates.None && state != PopUpStates.MaxLevel && state != PopUpStates.Skins)
		{
			StartCoroutine(AnimateTransition(state));
			return;
		}
		currentState = state;
		unitCashController.ToggleScreen(false);
		unitPartsController.ToggleScreen(false);
		unitMaxLevelController.ToggleScreen(false);
		unitSkinsController.ToggleScreen(false);
		CurrentScreen.ToggleScreen(true);
		unitInfoViewCurrentLevel.transform.position = CurrentScreen.GetUnitAnchor().position;
		SetScrapButtonState(true);
		CurrentScreen.SetPromoteButtonState(true);
	}

	private IEnumerator AnimateTransition(PopUpStates state)
	{
		CurrentScreen.SetPromoteButtonState(false);
		yield return new WaitForEndOfFrame();
		CurrentScreen.SetPromoteButtonState(false);
		yield return StartCoroutine(CurrentScreen.AnimateControllerOut());
		currentState = state;
		unitCashController.ToggleScreen(false);
		unitPartsController.ToggleScreen(false);
		unitMaxLevelController.ToggleScreen(false);
		unitSkinsController.ToggleScreen(false);
		unitInfoViewCurrentLevel.transform.position = CurrentScreen.GetUnitAnchor().position;
		yield return StartCoroutine(CurrentScreen.AnimateControllerIn());
		SetScrapButtonState(true);
		CurrentScreen.SetPromoteButtonState(true);
	}

	private void OnPartsCommitedComplete(object input)
	{
		CurrentScreen.OnPartsCommittedComplete(input);
	}

	private void UnitViewChange(UnitDataModel unitData, int level, int bitFlagPartial)
	{
		localUserUnit.SetLevel(level, bitFlagPartial, localUserUnit.boostId);
		unitInfoViewCurrentLevel.ConfigureUnitView(unitData, level, bitFlagPartial);
		UpdateCurrentScreen();
	}

	public void ToggleNext()
	{
		ToggleNext(false);
	}

	public void ToggleNext(bool forceCurrent)
	{
		tk2dTextMesh[] componentsInChildren = toggleNextLevelButton.GetComponentsInChildren<tk2dTextMesh>(true);
		if (viewingNext || forceCurrent)
		{
			unitInfoViewCurrentLevel.ConfigureUnitView(localUserUnit.UnitDataModel, localUserUnit.level, localUserUnit.partialLevel);
		}
		else
		{
			unitInfoViewCurrentLevel.ConfigureUnitView(localUserUnit.UnitDataModel, localUserUnit.level + 1);
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].text = string.Format((!viewingNext) ? "ui_upgrade_current_level".Localize("CURRENT LEVEL") : "ui_upgrade_next_level".Localize("NEXT LEVEL"));
		}
		viewingNext = !viewingNext;
	}

	private IEnumerator UnitScrapped()
	{
		if ((bool)unitInfoViewCurrentLevel)
		{
			unitInfoViewCurrentLevel.gameObject.SetActive(false);
		}
		float delay = 0f;
		EffectInstance rewardAnimEffect = GlobalEffectsManager.Create(EffectType.REWARD, sellEffectMarker.position).AutoDestroy();
		rewardAnimEffect.gameObject.SetLayerRecursively(base.gameObject.layer);
		rewardAnimEffect.SpineAnimation.Skeleton.SortOrder = 7;
		delay = rewardAnimEffect.SpineAnimation.state.Animation.Duration;
		yield return new WaitForSeconds(delay);
		scrappingUnit = false;
		ClosePopUpButton();
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
		if (localUserUnit.IsMaxLevel)
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
			SetScrapButtonState(true);
			SetPromoteButtonState(true);
		}
	}

	private void InternalScrapUnit()
	{
		SetPromoteButtonState(false);
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

	private void ClosePopUpButton()
	{
		if (SceneTransitionManager.CurrentSceneDM._scene == SceneTransitionManager.Scene.EditTeamUnitsScene)
		{
			((EditTeamUnitsSceneController)SceneTransitionManager.CurrentSceneDM.controller).RestoreSavedState(localUserUnit);
		}
		OnCloseButton();
		if (SceneTransitionManager.CurrentSceneDM._scene == SceneTransitionManager.Scene.EditTeamUnitsScene && model.afterRemoveAction != new Action(((EditTeamUnitsSceneController)SceneTransitionManager.CurrentSceneDM.controller).OnUpgradeUnitPopUpClosed))
		{
			((EditTeamUnitsSceneController)SceneTransitionManager.CurrentSceneDM.controller).OnUpgradeUnitPopUpClosed();
		}
	}

	public void SetScrapButtonState(bool state)
	{
		if (!scrapButton)
		{
			return;
		}
		if (localUserUnit.Team != null)
		{
			state = state && !localUserUnit.Team.IsOnCooldown;
		}
		scrapButton.enabled = state;
		toggleNextLevelButton.enabled = state;
		if (state)
		{
			viewingNext = false;
			tk2dTextMesh[] componentsInChildren = toggleNextLevelButton.GetComponentsInChildren<tk2dTextMesh>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].text = string.Format((!viewingNext) ? "ui_upgrade_next_level".Localize("NEXT LEVEL") : "ui_upgrade_current_level".Localize("CURRENT LEVEL"));
			}
		}
		disableScrapButtonSprite.gameObject.SetActive(!state);
		disableviewMaxButtonSprite.gameObject.SetActive(!state);
	}

	public void SetPromoteButtonState(bool state)
	{
		CurrentScreen.SetPromoteButtonState(state);
	}

	private void SetPromoteButtonMaxLevel()
	{
		CurrentScreen.SetPromoteButtonState(false);
	}

	public override void OnBackButtonPressed()
	{
		if (allowBackButton && !scrappingUnit && !CurrentScreen.IsPromoting())
		{
			ClosePopUpButton();
		}
	}

	public void OnPartsCommittedComplete(object input)
	{
		unitPartsController.OnPartsCommittedComplete(input);
	}
}
