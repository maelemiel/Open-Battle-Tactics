using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHooksThirdBattle : TutorialHooks
{
	public override void Init(BattleController battleController)
	{
		base.Init(battleController);
	}

	public override OpponentData GetPlayerTeam()
	{
		OpponentData opponentData = OpponentDataFactory.CreateTutorialOpponent();
		opponentData.units = new List<IUnitMetadata>
		{
			UserUnit.FromLevelID(null, UserProfile.player.tutorial.FirstUnitID.ToString()),
			UserUnit.FromLevelID(null, TutorialConstants.SECOND_UNIT.ToString()),
			UserUnit.FromLevelID(null, TutorialConstants.THIRD_UNIT.ToString())
		};
		opponentData.abilities = new List<IAbilityMetadata>
		{
			AbilityDataModel.GetAbilityByType("reroll"),
			AbilityDataModel.GetAbilityByType("target")
		};
		return opponentData;
	}

	public override OpponentData GetOpponentTeam()
	{
		OpponentData opponentData = OpponentDataFactory.CreateTutorialOpponent();
		opponentData.units = new List<IUnitMetadata>
		{
			new UserUnit(null, TutorialConstants.TUTORIAL_NME_3_1, 0, string.Empty, string.Empty),
			new UserUnit(null, TutorialConstants.TUTORIAL_NME_3_2, 0, string.Empty, string.Empty),
			new UserUnit(null, TutorialConstants.TUTORIAL_NME_3_3, 0, string.Empty, string.Empty)
		};
		opponentData.abilities = new List<IAbilityMetadata>();
		return opponentData;
	}

	public override void ModifyRoundStep()
	{
		TeamState playerTeam = battleController.playerTeam;
		playerTeam.randomProvider = new MersenneTwister(112878u);
		if (battleController.battleState.currentRound == 1)
		{
			playerTeam.units[0].ForceCurrentRoll(0);
			playerTeam.units[1].ForceCurrentRoll(4);
			playerTeam.units[2].ForceCurrentRoll(1);
		}
		else if (battleController.battleState.currentRound == 2)
		{
			playerTeam.units[0].ForceCurrentRoll(2);
			playerTeam.units[1].ForceCurrentRoll(0);
			playerTeam.units[2].ForceCurrentRoll(4);
		}
		else if (battleController.battleState.currentRound == 3)
		{
			playerTeam.units[0].ForceCurrentRoll(3);
			playerTeam.units[1].ForceCurrentRoll(2);
			playerTeam.units[2].ForceCurrentRoll(2);
		}
	}

	public override IEnumerator OnEnterDecisionPhase()
	{
		if (battleController.battleState.currentRound == 1)
		{
			battleController.CubeBar.SetAbilityEnabled(0, false);
			battleController.CubeBar.SetAbilityEnabled(1, false);
			battleController.CubeBar.BattleButtonEnabled = false;
			yield return new WaitForSeconds(1f);
			battleController.CubeBar.SetAbilityEnabled(1, true);
			yield return DialogTrigger("TutorialThreeEnterDecisionPhase");
			ShowAbilityButtonArrow(1);
		}
		else
		{
			battleController.CubeBar.SetAbilityEnabled(0, true);
		}
	}

	public override IEnumerator OnAbilityActivated(AbilityState abilityState, UnitState target)
	{
		if (abilityState.metadata.Type == "target" && battleController.battleState.currentRound == 1)
		{
			yield return new WaitForSeconds(1f);
			battleController.hud.bouncingArrowAction.Hide();
			yield return DialogTrigger("TutorialThreePostTarget");
			battleController.CubeBar.SetAbilityEnabled(0, true);
			battleController.CubeBar.BattleButtonEnabled = true;
		}
	}

	public override bool ShouldRestartBattle()
	{
		if (battleController.playerTeam.IsBattleWinner)
		{
			UserProfile.player.tutorial.CurrentStep = TutorialStep.FourthBattle;
			battleController.BattleHooks = new TutorialHooksFourthBattle();
		}
		return true;
	}

	public override IEnumerator OutroAnimation()
	{
		if (battleController.playerTeam.IsBattleWinner)
		{
			yield return DialogTrigger("TutorialThreeVictory");
		}
		else
		{
			yield return DialogTrigger("TutorialLoseRestart");
		}
	}
}
