using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHooksFirstBattle : TutorialHooks
{
	public override OpponentData GetPlayerTeam()
	{
		OpponentData opponentData = OpponentDataFactory.CreateTutorialOpponent();
		opponentData.units = new List<IUnitMetadata> { UserUnit.FromLevelID(null, UserProfile.player.tutorial.FirstUnitID.ToString()) };
		opponentData.abilities = new List<IAbilityMetadata> { AbilityDataModel.GetAbilityByType("reroll") };
		return opponentData;
	}

	public override OpponentData GetOpponentTeam()
	{
		OpponentData opponentData = OpponentDataFactory.CreateTutorialOpponent();
		opponentData.units = new List<IUnitMetadata>
		{
			new UserUnit(null, TutorialConstants.TUTORIAL_NME_1_1, 0, string.Empty, string.Empty)
		};
		opponentData.abilities = new List<IAbilityMetadata>();
		return opponentData;
	}

	public override void ModifyBeginBattleStep()
	{
		battleController.enemyTeam.units[0].hp = 14;
	}

	public override IEnumerator PreIntroPlayerRollIn()
	{
		yield return DialogTrigger("TutorialOnePrePlayerRollIn");
	}

	public override IEnumerator PreIntroEnemyRollIn()
	{
		yield return DialogTrigger("TutorialOnePreEnemyRollIn");
	}

	public override IEnumerator PostIntroEnemyRollIn()
	{
		yield return DialogTrigger("TutorialOnePostEnemyRollIn");
	}

	public override IEnumerator OnEnterDecisionPhase()
	{
		battleController.PlayerUnits[0].PossibleRollsSimple.ActiveFace.HideLabel();
		battleController.CubeBar.SetAbilityEnabled(0, false);
		yield return DialogTrigger("TutorialOneEnterDecisionPhase");
		battleController.CubeBar.SetAbilityEnabled(0, true);
		ShowAbilityButtonArrow(0);
	}

	public override IEnumerator OnExitIntroPhase()
	{
		battleController.CubeBar.BattleButtonEnabled = false;
		yield break;
	}

	public override bool PlayerSpinAllDice()
	{
		if (battleController.battleState.currentRound == 1)
		{
			return false;
		}
		return true;
	}

	public override void OnPostReroll()
	{
		battleController.hud.bouncingArrowAction.StopAllCoroutines();
		battleController.hud.bouncingArrowAction.Hide();
		if (battleController.playerTeam.energy == 0 && !battleController.CubeBar.BattleButtonEnabled)
		{
			StartCoroutine(PostRollAnimation());
		}
	}

	private IEnumerator PostRollAnimation()
	{
		yield return new WaitForSeconds(1.5f);
		battleController.CubeBar.BattleButtonEnabled = true;
		yield return DialogTrigger("TutorialOnePostReroll");
		ShowBattleButtonArrow();
	}

	public override void ModifyRoundStep()
	{
		if (battleController.battleState.currentRound == 1)
		{
			TeamState playerTeam = battleController.playerTeam;
			playerTeam.randomProvider = new MersenneTwister(14u);
			playerTeam.units[0].ForceCurrentRoll(1);
			playerTeam.energy = 1;
			battleController.enemyTeam.units[0].ForceCurrentRoll(3);
		}
	}

	public override IEnumerator OnEnterResolutionPhase()
	{
		battleController.hud.bouncingArrowCommit.StopAllCoroutines();
		battleController.hud.bouncingArrowCommit.Hide();
		battleController.CubeBar.BattleButtonAlpha = 1f;
		yield break;
	}

	public override bool ShouldRestartBattle()
	{
		if (battleController.playerTeam.IsBattleWinner)
		{
			UserProfile.player.tutorial.CurrentStep = TutorialStep.SecondBattle;
			battleController.BattleHooks = new TutorialHooksSecondBattle();
		}
		return true;
	}

	public override IEnumerator OutroAnimation()
	{
		if (battleController.playerTeam.IsBattleWinner)
		{
			yield return DialogTrigger("TutorialOneVictory");
		}
		else
		{
			yield return DialogTrigger("TutorialLoseRestart");
		}
	}

	public override bool OnClickCancel()
	{
		ShowAbilityButtonArrow(0);
		return true;
	}
}
