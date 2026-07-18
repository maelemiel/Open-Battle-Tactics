using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHooksSecondBattle : TutorialHooks
{
	public override OpponentData GetPlayerTeam()
	{
		OpponentData opponentData = OpponentDataFactory.CreateTutorialOpponent();
		opponentData.units = new List<IUnitMetadata>
		{
			UserUnit.FromLevelID(null, UserProfile.player.tutorial.FirstUnitID.ToString()),
			UserUnit.FromLevelID(null, TutorialConstants.SECOND_UNIT.ToString())
		};
		opponentData.abilities = new List<IAbilityMetadata> { AbilityDataModel.GetAbilityByType("reroll") };
		return opponentData;
	}

	public override OpponentData GetOpponentTeam()
	{
		OpponentData opponentData = OpponentDataFactory.CreateTutorialOpponent();
		opponentData.units = new List<IUnitMetadata>
		{
			new UserUnit(null, TutorialConstants.TUTORIAL_NME_2_1, 0, string.Empty, string.Empty),
			new UserUnit(null, TutorialConstants.TUTORIAL_NME_2_2, 0, string.Empty, string.Empty)
		};
		opponentData.abilities = new List<IAbilityMetadata>();
		return opponentData;
	}

	public override void ModifyBeginBattleStep()
	{
		battleController.enemyTeam.units[0].hp -= 11;
		battleController.enemyTeam.units[0].unitView.LocalHealth = battleController.enemyTeam.units[0].hp;
		battleController.enemyTeam.units[0].unitView.RefreshHealthHUD();
		battleController.enemyTeam.units[1].hp -= 7;
		battleController.enemyTeam.units[1].unitView.LocalHealth = battleController.enemyTeam.units[1].hp;
		battleController.enemyTeam.units[1].unitView.RefreshHealthHUD();
	}

	public override void ModifyRoundStep()
	{
		TeamState playerTeam = battleController.playerTeam;
		TeamState enemyTeam = battleController.enemyTeam;
		playerTeam.randomProvider = new MersenneTwister(686u);
		if (battleController.battleState.currentRound == 1)
		{
			playerTeam.units[0].preventReroll = true;
			playerTeam.units[0].ForceCurrentRoll(4);
			playerTeam.units[1].ForceCurrentRoll(1);
			playerTeam.energy = 1;
			enemyTeam.units[0].ForceCurrentRoll(4);
			enemyTeam.units[1].ForceCurrentRoll(1);
			battleController.CubeBar.SetAbilityEnabled(0, false);
			battleController.CubeBar.BattleButtonEnabled = false;
		}
		else if (battleController.battleState.currentRound == 2)
		{
			playerTeam.units[0].ForceCurrentRoll(0);
			playerTeam.units[1].ForceCurrentRoll(2);
		}
		else if (battleController.battleState.currentRound == 3)
		{
			playerTeam.units[0].ForceCurrentRoll(1);
			playerTeam.units[1].ForceCurrentRoll(3);
		}
	}

	public override IEnumerator OnEnterDecisionPhase()
	{
		if (battleController.battleState.currentRound == 1)
		{
			yield return new WaitForSeconds(1.5f);
			yield return DialogTrigger("TutorialTwoEnterDecisionPhase");
			battleController.CubeBar.SetAbilityEnabled(0, true);
			ShowAbilityButtonArrow(0);
		}
	}

	public override IEnumerator OnEnterResolutionPhase()
	{
		battleController.playerTeam.units[0].preventReroll = false;
		yield break;
	}

	public override void OnPostReroll()
	{
		if (battleController.playerTeam.energy == 0 && !battleController.CubeBar.BattleButtonEnabled && battleController.battleState.currentRound == 1)
		{
			StartCoroutine(PostRollAnimation());
		}
	}

	private IEnumerator PostRollAnimation()
	{
		yield return new WaitForSeconds(1.5f);
		yield return DialogTrigger("TutorialTwoPostReroll");
		battleController.CubeBar.BattleButtonEnabled = true;
		ShowBattleButtonArrow();
	}

	public override bool ShouldRestartBattle()
	{
		if (battleController.playerTeam.IsBattleWinner)
		{
			UserProfile.player.tutorial.CurrentStep = TutorialStep.ThirdBattle;
			battleController.BattleHooks = new TutorialHooksThirdBattle();
		}
		return true;
	}

	public override IEnumerator OutroAnimation()
	{
		if (battleController.playerTeam.IsBattleWinner)
		{
			yield return DialogTrigger("TutorialTwoVictory");
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
