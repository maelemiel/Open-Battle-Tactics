using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectScreenController : SceneController
{
	private const string POPUP_TITLE_NOT_ENOUGH_UNITS = "Warning!";

	private const string POPUP_TEXT_NOT_ENOUGH_UNITS = "You must have a total of 4 units on your first team";

	private const string POPUP_TITLE_TOO_MANY_UNITS = "Warning!";

	private const string POPUP_TEXT_TOO_MANY_UNITS = "You cannot select more than 4 units";

	private const int MAX_UNITS = 4;

	[SerializeField]
	private UnitSelectHUDController hudController;

	private Dictionary<string, UnitItemController> unitControllers;

	private bool initialized;

	public override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		base.SectionTitle = "Team Select";
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		hudController.Init(this);
		UpdateUserUnits();
		initialized = true;
	}

	public void InitializeUserUnits(UserTeam team)
	{
		UserProfile player = UserProfile.player;
		for (int i = 0; i < team.units.Count; i++)
		{
			ActivateUnitOnHUD(player, unitControllers[team.units[i].ID]);
		}
	}

	public void UpdateUserUnits()
	{
		ClearList();
		List<UserUnit> list = new List<UserUnit>();
		list.AddRange(UserProfile.player.unitInventory.Values);
		list.Sort(delegate(UserUnit unit1, UserUnit unit2)
		{
			int num = 0;
			if (unit1.Rarity == unit2.Rarity)
			{
				if (unit1.level > unit2.level)
				{
					num = 1;
				}
				else if (unit1.level < unit2.level)
				{
					num = -1;
				}
			}
			else
			{
				num = ((unit1.Rarity > unit2.Rarity) ? 1 : (-1));
			}
			return -num;
		});
		unitControllers = hudController.PopulateUnitList(list);
		InitializeUserUnits(UserProfile.player.CurrentTeam);
	}

	public void RemoveUnitItem(UserUnit unitItem)
	{
		if (unitItem != null)
		{
			hudController.RemoveUnitItem(unitItem, unitControllers);
		}
	}

	public void ClearList()
	{
		if (unitControllers == null)
		{
			return;
		}
		foreach (UnitItemController value in unitControllers.Values)
		{
			UnityEngine.Object.Destroy(value.gameObject);
		}
	}

	private void DeactivateAllUnits()
	{
		if (unitControllers == null)
		{
			return;
		}
		foreach (KeyValuePair<string, UnitItemController> unitController in unitControllers)
		{
			hudController.SetUnitState(false, unitController.Value, null);
		}
		hudController.ClearAllBottomBarUnits();
	}

	public void UnitSelected(UnitItemController unitItemController)
	{
		if (unitItemController.State)
		{
			DeactivateUnit(unitItemController);
		}
		else
		{
			ActivateUnit(unitItemController);
		}
	}

	public void UnitPromoted(UnitItemController unitItemController, Action<UnitItemController> unitPromoted)
	{
		Singleton<SessionManager>.instance.UpgradeUnit(unitItemController.unit, UserPriceDataModel.PaymentType.Normal, delegate(bool wasTheUnitUpgraded)
		{
			if (wasTheUnitUpgraded && unitPromoted != null)
			{
				unitPromoted(unitItemController);
			}
		});
	}

	private void ActivateUnitOnHUD(UserProfile userProfile, UnitItemController unitController)
	{
		hudController.SetUnitState(true, unitController, userProfile.CurrentTeam);
	}

	private void DeactivateUnitOnHUD(UserProfile userProfile, UnitItemController unitController)
	{
		hudController.SetUnitState(false, unitController, userProfile.CurrentTeam);
	}

	private void ActivateUnit(UnitItemController unitController)
	{
		UserProfile player = UserProfile.player;
		if (player.CurrentTeam.units.Count >= 4)
		{
			ShowMaxUnitsPopUp();
			return;
		}
		if (!player.CurrentTeam.Contains(unitController.unit.ID))
		{
			player.CurrentTeam.units.Add(unitController.unit);
			ActivateUnitOnHUD(player, unitController);
		}
		AudioTrigger.PlayerEquipItem.Play();
	}

	private void DeactivateUnit(UnitItemController unitController)
	{
		UserProfile player = UserProfile.player;
		if (player.CurrentTeam.units.Count <= 0)
		{
			Log.Error("User has no units on his team and is trying to deselect one", base.gameObject);
			return;
		}
		if (player.CurrentTeam.Contains(unitController.unit.ID))
		{
			player.CurrentTeam.units.Remove(unitController.unit);
			DeactivateUnitOnHUD(player, unitController);
		}
		AudioTrigger.PlayerUnEquipItem.Play();
	}

	public void Confirm()
	{
		SaveUnitsAndGoBack();
	}

	public void SaveUnitsAndGoBack()
	{
		if (UserProfile.player != null)
		{
			UserProfile.player.currentTeamIndex = 0;
			Singleton<SessionManager>.instance.UpdateTeam(delegate
			{
				Log.Debug("Team saved!");
			});
		}
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaScene);
	}

	public void OnToggleTeamButtonsChanged(tk2dUIToggleButtonGroup buttonGroup)
	{
		if (initialized)
		{
			UserProfile player = UserProfile.player;
			player.currentTeamIndex = buttonGroup.SelectedIndex;
			if (unitControllers != null && buttonGroup.SelectedIndex < player.teams.Count)
			{
				DeactivateAllUnits();
				InitializeUserUnits(player.CurrentTeam);
			}
		}
	}

	public void ShowMaxUnitsPopUp()
	{
		PopupManager.ShowPopup(PopupDataModel.One("Warning!", "You cannot select more than 4 units", "ok", null));
	}

	public bool EmptyUnitsAvailable()
	{
		UserProfile player = UserProfile.player;
		return player.CurrentTeam.units.Count < 4;
	}
}
