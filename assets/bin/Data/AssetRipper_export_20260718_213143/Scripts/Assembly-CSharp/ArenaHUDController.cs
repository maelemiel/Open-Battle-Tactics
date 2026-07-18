using System.Collections.Generic;
using UnityEngine;

public class ArenaHUDController : MonoBehaviour
{
	private ArenaController arenaController;

	public UnitProxy[] unitProxies;

	public UnitInfoController[] unitInfoViews;

	[SerializeField]
	private AbilityItemBottomBarController abilitiesController;

	[SerializeField]
	private AbilityItemBottomBarController abilitiesControllerDown;

	[SerializeField]
	public CooldownsController cooldownsController;

	[SerializeField]
	private GameObject emptyTeamButton;

	private bool initialized;

	private void Start()
	{
	}

	public void Init(ArenaController arenaController)
	{
		this.arenaController = arenaController;
		initialized = true;
		cooldownsController.Init();
		cooldownsController.SelectedIndex = UserProfile.player.currentTeamIndex;
		cooldownsController.OnControllerCooldownFinished += OnControllerCooldownFinished;
	}

	public void UpdateUnitsTeam(UserTeam team)
	{
		ResetAllViews();
		int num = 0;
		for (int i = 0; i < team.units.Count; i++)
		{
			UserUnit userUnit = team.units[i];
			if (userUnit != null)
			{
				unitInfoViews[i].SetState(true);
				unitInfoViews[i].ConfigureUnitInfo(userUnit, UpdateCurrentTeam);
			}
			else
			{
				unitInfoViews[i].SetState(false);
				num++;
			}
		}
		if ((bool)emptyTeamButton)
		{
			emptyTeamButton.SetActive(team.units.Count <= 0 || num == Constants.MinUnitsPerTeam);
		}
	}

	public void UpdateCurrentTeam()
	{
		UpdateUnitsTeam(UserProfile.player.CurrentTeam);
	}

	private void ResetAllViews()
	{
		for (int i = 0; i < unitInfoViews.Length; i++)
		{
			unitInfoViews[i].SetState(false);
		}
	}

	public void UpdateAbilityIcons(List<string> abilityIDs)
	{
		abilitiesController.UpdateAbilityIcons(abilityIDs);
		abilitiesControllerDown.UpdateAbilityIcons(abilityIDs);
	}

	public void OnControllerCooldownFinished(UserTeam team)
	{
		if (team != null)
		{
			UserTeam userTeam = UserProfile.player.teams[cooldownsController.SelectedIndex];
			if (userTeam != null && userTeam == team)
			{
				UpdateUnitsTeam(team);
			}
		}
	}

	public void OnToggleButtonGroupChanged(tk2dUIToggleButtonGroup toggleButtonGroup)
	{
		if (initialized)
		{
			arenaController.SelectTeam(toggleButtonGroup.SelectedIndex);
		}
	}

	public void OnUnitPressed(tk2dUIItem unitItem)
	{
		if (initialized)
		{
			UnitInfoController component = unitItem.GetComponent<UnitInfoController>();
			if ((bool)component)
			{
				PopupManager.ShowPopup(PopupDataModel.UpgradeUnitPopUp(component.UserUnitData, UpdateCurrentTeam));
			}
		}
	}
}
