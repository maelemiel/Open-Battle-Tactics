using System;
using System.Collections;
using UnityEngine;

public class UpgradeUnitMaxLevelController : MonoBehaviour, IUpgradeUnitPartsContoller
{
	[SerializeField]
	private Transform unitAnchor;

	[SerializeField]
	private Transform unitWithAbilityAnchor;

	[SerializeField]
	private tk2dUIItem promoteButton;

	private UserUnit localUserUnit;

	private Action<UnitDataModel, int, int> unitViewChange;

	private Action<bool> scrapButtonUpdate;

	private Action closeScreen;

	private Action upgradeStart;

	public void SetupScreen(UserUnit localUserUnit, Action upgradeStart, Action closeScreen, Action<bool> scrapButtonUpdate, Action<UnitDataModel, int, int> unitViewChange)
	{
		this.upgradeStart = upgradeStart;
		this.closeScreen = closeScreen;
		this.localUserUnit = localUserUnit;
		this.unitViewChange = unitViewChange;
		this.scrapButtonUpdate = scrapButtonUpdate;
	}

	public void ToggleScreen(bool toggle)
	{
		if (toggle)
		{
			base.gameObject.SetActive(true);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	public void SetPromoteButtonState(bool toggle)
	{
		promoteButton.enabled = toggle;
	}

	public bool IsPromoting()
	{
		return false;
	}

	public Transform GetUnitAnchor()
	{
		UnitLevelProgressionDataModel currentLevelDataModel = localUserUnit.CurrentLevelDataModel;
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		EventUnitBoostDataModel eventUnitBoostDataModel = null;
		if (activeEvent != null)
		{
			eventUnitBoostDataModel = EventUnitBoostDataModel.FindUnitBoost(currentLevelDataModel.unitId.ToString(), currentLevelDataModel.level, activeEvent.id);
		}
		if (localUserUnit.HasSpecial || eventUnitBoostDataModel != null)
		{
			return unitWithAbilityAnchor;
		}
		return unitAnchor;
	}

	public void MaxLeveledButton()
	{
		PopupManager.ShowPopup(PopupDataModel.Ok("ui_upgrade_nolevels_title".Localize("No More Levels"), "ui_upgrade_nolevels_desc".Localize("There are currently no more levels for this unit.")));
	}

	public void OnPartsCommittedComplete(object input)
	{
	}

	public IEnumerator AnimateControllerIn()
	{
		yield break;
	}

	public IEnumerator AnimateControllerOut()
	{
		yield break;
	}
}
