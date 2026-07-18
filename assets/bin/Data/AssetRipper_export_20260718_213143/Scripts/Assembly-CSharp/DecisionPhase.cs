using System.Collections;
using UnityEngine;

public class DecisionPhase : AbstractBattlePhase
{
	public override void OnEnterPhase()
	{
		if (battleController.battleState.currentRound == 1)
		{
			Reporting.BattleStartEvent(battleController.matchManager.MatchData.matchId, battleController.playerTeam.units, battleController.MatchType);
		}
		battleController.hud.ActivateTimer();
		if (battleController.MatchHandler.AllowEmoticons)
		{
			battleController.hud.emoticonController.ResetToggleEmoticon();
			battleController.hud.emoticonController.ShowToggleButton();
		}
		foreach (UnitView enemyUnit in battleController.EnemyUnits)
		{
			enemyUnit.PossibleRollsSimple.CloseDieBox();
		}
		foreach (UnitView playerUnit in battleController.PlayerUnits)
		{
			playerUnit.PossibleRollsSimple.RestoreDecisionState();
		}
		if (battleController.battleHooks != null)
		{
			StartCoroutine(battleController.battleHooks.OnEnterDecisionPhase());
		}
	}

	public override void OnUpdate()
	{
		UpdateEnergy();
	}

	public override void OnExitPhase()
	{
		foreach (UnitView playerUnit in battleController.PlayerUnits)
		{
			playerUnit.PossibleRollsSimple.RememberDecisionState();
		}
		if (battleController.targetSelectionManager.isActive)
		{
			battleController.targetSelectionManager.ExitTargettingMode();
		}
		battleController.hud.bouncingArrowAction.Hide();
		battleController.hud.bouncingArrowCommit.Hide();
	}

	private void UpdateEnergy()
	{
		if ((bool)battleController.CubeBar.EnergyWidget)
		{
			battleController.CubeBar.EnergyWidget.UpdateEnergy(battleController.playerTeam.energy);
		}
	}

	private IEnumerator ShowBounceAction(float sec)
	{
		yield return new WaitForSeconds(sec);
		if (battleController.playerTeam.energy == 3 && battleController.phaseManager.currentPhase == Phase.DECISION)
		{
			battleController.hud.bouncingArrowAction.Bounce();
		}
	}

	private IEnumerator ShowBounceStart(float sec)
	{
		yield return new WaitForSeconds(sec);
		if (battleController.phaseManager.currentPhase == Phase.DECISION)
		{
			battleController.hud.bouncingArrowCommit.Bounce();
		}
	}
}
