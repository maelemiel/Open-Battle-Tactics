using System;
using UnityEngine;

[RequireComponent(typeof(tk2dUIToggleButtonGroup))]
public class CooldownsController : MonoBehaviour
{
	private tk2dUIToggleButtonGroup cooldownTogglesGroup;

	private TeamCooldownViewController[] teamCooldownViewControllers;

	public int SelectedIndex
	{
		get
		{
			return cooldownTogglesGroup.SelectedIndex;
		}
		set
		{
			cooldownTogglesGroup.SelectedIndex = value;
		}
	}

	public event Action<UserTeam> OnControllerCooldownFinished;

	public void Init()
	{
		UserProfile player = UserProfile.player;
		cooldownTogglesGroup = GetComponent<tk2dUIToggleButtonGroup>();
		teamCooldownViewControllers = new TeamCooldownViewController[cooldownTogglesGroup.ToggleBtns.Length];
		for (int i = 0; i < cooldownTogglesGroup.ToggleBtns.Length; i++)
		{
			teamCooldownViewControllers[i] = cooldownTogglesGroup.ToggleBtns[i].GetComponent<TeamCooldownViewController>();
			teamCooldownViewControllers[i].ConfigureView(UserProfile.player.teams[i]);
			teamCooldownViewControllers[i].OnCooldownFinished += OnCooldownFinished;
		}
	}

	public void SelectFirstAvailableTeam()
	{
		UserProfile player = UserProfile.player;
		bool flag = false;
		for (int i = 0; i < player.teams.Count; i++)
		{
			if (!player.teams[i].IsOnCooldown && !flag)
			{
				cooldownTogglesGroup.SelectedIndex = i;
				flag = true;
			}
		}
		if (!flag)
		{
			cooldownTogglesGroup.SelectedIndex = 0;
		}
	}

	private void OnCooldownFinished(UserTeam team)
	{
		if (this.OnControllerCooldownFinished != null)
		{
			this.OnControllerCooldownFinished(team);
		}
	}
}
