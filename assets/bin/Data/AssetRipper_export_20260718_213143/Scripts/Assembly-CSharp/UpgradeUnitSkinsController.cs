using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUnitSkinsController : MonoBehaviour, IUpgradeUnitPartsContoller
{
	[SerializeField]
	private Transform unitAnchor;

	[SerializeField]
	private ScrollableAreaController scrollableArea;

	[SerializeField]
	private Transform content;

	[SerializeField]
	private Transform centeredArea;

	[SerializeField]
	private PurchaseSkinController skinController;

	private List<Action<bool>> skinEquipButtons;

	private UserUnit localUserUnit;

	private Action<UnitDataModel, int, int> unitViewChange;

	private Action<bool> scrapButtonUpdate;

	private Action closeScreen;

	private Action upgradeStart;

	public void SetupScreen(UserUnit localUserUnit, Action upgradeStart, Action closeScreen, Action<bool> scrapButtonUpdate, Action<UnitDataModel, int, int> unitViewChange)
	{
		this.closeScreen = closeScreen;
		this.upgradeStart = upgradeStart;
		this.localUserUnit = localUserUnit;
		this.unitViewChange = unitViewChange;
		this.scrapButtonUpdate = scrapButtonUpdate;
	}

	public void ToggleScreen(bool toggle)
	{
		if (toggle)
		{
			base.gameObject.SetActive(true);
			UpdatePromotePopUpState();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void UpdatePromotePopUpState()
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
	}

	private void SetSkinsScreen(List<UnitLevelProgressionDataModel> skinLevels)
	{
		scrollableArea.gameObject.SetActive(true);
		scrollableArea.DataSource = skinLevels;
		skinEquipButtons = new List<Action<bool>>();
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
			ConfigureUnitView(localUserUnit.UnitDataModel, localUserUnit.level);
			return;
		}
		skinController.PurchaseSkin(skin, delegate
		{
			Singleton<SessionManager>.instance.SetUnitSkin(localUserUnit, skin, null);
			localUserUnit.SetLevel(skin.level, 0, localUserUnit.boostId);
			UpdatePromotePopUpState();
			ConfigureUnitView(localUserUnit.UnitDataModel, localUserUnit.level);
		}, delegate
		{
		});
	}

	private void ConfigureUnitView(UnitDataModel unit, int level = -1, int partialLevelBitFlag = 0)
	{
		if (unitViewChange != null)
		{
			unitViewChange(unit, level, partialLevelBitFlag);
		}
	}

	public void SetPromoteButtonState(bool toggle)
	{
		for (int i = 0; i < skinEquipButtons.Count; i++)
		{
			skinEquipButtons[i](toggle);
		}
	}

	public bool IsPromoting()
	{
		return false;
	}

	public Transform GetUnitAnchor()
	{
		return unitAnchor;
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
