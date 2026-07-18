using System;
using UnityEngine;

public class TeamCooldownViewController : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh teamIndexLabel;

	[SerializeField]
	private tk2dTextMesh teamIndexLabelDisabled;

	[SerializeField]
	private tk2dUIProgressBar cooldownProgressBar;

	private TeamCooldownState currentCooldownState;

	private UserTeam currentTeam;

	public UserTeam CurrentTeam
	{
		get
		{
			return currentTeam;
		}
	}

	public event Action<UserTeam> OnCooldownFinished;

	private void Update()
	{
		if (currentCooldownState == TeamCooldownState.ON_COOLDOWN)
		{
			UpdateViewOnCooldownState();
		}
	}

	private void SetState(TeamCooldownState teamCooldownState)
	{
		currentCooldownState = teamCooldownState;
		switch (teamCooldownState)
		{
		case TeamCooldownState.ON_COOLDOWN:
			SetOnCooldownState();
			break;
		case TeamCooldownState.NOT_ON_COOLDOWN:
			SetNotOnCooldownState();
			break;
		}
	}

	private void UpdateViewOnCooldownState()
	{
		if (currentTeam == null)
		{
			return;
		}
		if (!currentTeam.IsOnCooldown)
		{
			if (this.OnCooldownFinished != null)
			{
				this.OnCooldownFinished(currentTeam);
			}
			SetState(TeamCooldownState.NOT_ON_COOLDOWN);
			return;
		}
		if ((bool)teamIndexLabel && (bool)teamIndexLabelDisabled)
		{
			float num = 4f;
			teamIndexLabel.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time * 2f, num));
			teamIndexLabelDisabled.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time * 2f, num));
			if ((float)(int)Time.time % (num * 2f) < num)
			{
				teamIndexLabel.text = "ui_editteam_repairing".Localize("Repairing");
				teamIndexLabelDisabled.text = teamIndexLabel.text;
			}
			else
			{
				teamIndexLabel.text = NonUnitySingleton<TimeManager>.instance.GetCountdownString(currentTeam.cooldownFinishTime, true);
				teamIndexLabelDisabled.text = NonUnitySingleton<TimeManager>.instance.GetCountdownString(currentTeam.cooldownFinishTime, true);
			}
		}
		if ((bool)cooldownProgressBar)
		{
			cooldownProgressBar.Value = currentTeam.CooldownProgress;
		}
	}

	private void SetOnCooldownState()
	{
		if ((bool)cooldownProgressBar)
		{
			cooldownProgressBar.gameObject.SetActive(true);
		}
	}

	private void SetNotOnCooldownState()
	{
		if ((bool)cooldownProgressBar)
		{
			cooldownProgressBar.gameObject.SetActive(false);
			cooldownProgressBar.Value = 1f;
		}
		if ((bool)teamIndexLabel && (bool)teamIndexLabelDisabled)
		{
			teamIndexLabel.Alpha = 1f;
			teamIndexLabel.text = string.Format("ui_editteam_team_number".Localize("Team {0}"), currentTeam.index + 1);
			teamIndexLabelDisabled.Alpha = 1f;
			teamIndexLabelDisabled.text = teamIndexLabel.text;
		}
	}

	public void ConfigureView(UserTeam userTeam)
	{
		currentTeam = userTeam;
		SetState((!userTeam.IsOnCooldown) ? TeamCooldownState.NOT_ON_COOLDOWN : TeamCooldownState.ON_COOLDOWN);
	}
}
