using System.Collections;
using System.Collections.Generic;

public class TutorialHooksFourthBattle : TutorialHooks
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
			AbilityDataModel.GetAbilityByType("target"),
			AbilityDataModel.GetAbilityByType("small_ion_strike")
		};
		return opponentData;
	}

	public override OpponentData GetOpponentTeam()
	{
		OpponentData opponentData = OpponentDataFactory.CreateTutorialOpponent();
		opponentData.units = new List<IUnitMetadata>
		{
			new UserUnit(null, TutorialConstants.TUTORIAL_NME_4_1, 0, string.Empty, string.Empty)
		};
		opponentData.abilities = new List<IAbilityMetadata>();
		return opponentData;
	}

	public override void ModifyBeginBattleStep()
	{
		int boostValue = AbilityDataModel.GetAbilityByType("small_ion_strike").BoostValue;
		int num = battleController.playerTeam.units[0].metadata.RollValues[4];
		int num2 = battleController.playerTeam.units[1].metadata.RollValues[4];
		int num3 = battleController.playerTeam.units[2].metadata.RollValues[3];
		int num4 = boostValue + num + num2 + num3;
		battleController.enemyTeam.units[0].hp = num4;
		battleController.enemyTeam.units[0].unitView.LocalHealth = num4;
		battleController.enemyTeam.units[0].unitView.RefreshHealthHUD();
		battleController.enemyTeam.units[0].ForceCurrentRoll(4);
	}

	public override void ModifyRoundStep()
	{
		TeamState playerTeam = battleController.playerTeam;
		if (battleController.battleState.currentRound == 1)
		{
			playerTeam.units[0].ForceCurrentRoll(4);
			playerTeam.units[1].ForceCurrentRoll(4);
			playerTeam.units[2].ForceCurrentRoll(3);
			playerTeam.energy = 2;
		}
	}

	public override IEnumerator OnEnterDecisionPhase()
	{
		battleController.CubeBar.SetAbilityEnabled(0, false);
		battleController.CubeBar.SetAbilityEnabled(1, false);
		battleController.CubeBar.BattleButtonEnabled = false;
		yield return DialogTrigger("TutorialFourEnterDecisionPhase");
		ShowAbilityButtonArrow(2);
	}

	public override IEnumerator OnAbilityInvested(AbilityState abilityState)
	{
		if (abilityState.energy == 1)
		{
			yield return DialogTrigger("TutorialFourPostInvest");
			ShowAbilityButtonArrow(2);
		}
	}

	public override IEnumerator OnAbilityActivated(AbilityState abilityState, UnitState target)
	{
		if (abilityState.metadata.Type == "small_ion_strike")
		{
			if (battleController.playerTeam.energy > 0)
			{
			}
			battleController.hud.bouncingArrowAction.Hide();
			battleController.CubeBar.BattleButtonEnabled = true;
			ShowBattleButtonArrow();
		}
		yield break;
	}

	public override IEnumerator OutroAnimation()
	{
		yield return DialogTrigger("TutorialFourVictory");
	}

	public override bool StopMusicOnBattleComplete()
	{
		return true;
	}
}
