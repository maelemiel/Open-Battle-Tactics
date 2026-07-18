using System.Collections.Generic;

public class BattleSyncChecker
{
	public static void SyncCheck(BattleController battleController)
	{
		BattleState battleState = battleController.battleState;
		ServerTeamState[] bothTeams = battleState.bothTeams;
		for (int i = 0; i < bothTeams.Length; i++)
		{
			TeamState teamState = (TeamState)bothTeams[i];
			UnitState[] units = teamState.units;
			foreach (UnitState unitState in units)
			{
				UnitView unitView = unitState.unitView;
				if (unitView.CurrentRoll != unitState.currentRoll)
				{
					MakeSyncError("Battle Sync Error! Unit " + BattleAction.GetUnitID(unitState) + "'s CurrentRoll is " + unitView.CurrentRoll + ", should be " + unitState.currentRoll, battleController);
				}
				if (unitView.LocalHealth != unitState.hp)
				{
					MakeSyncError("Battle Sync Error! Unit " + BattleAction.GetUnitID(unitState) + "'s HP is " + unitView.LocalHealth + ", should be " + unitState.hp, battleController);
				}
				if (unitView.LocalArmor != unitState.armor)
				{
					MakeSyncError("Battle Sync Error! Unit " + BattleAction.GetUnitID(unitState) + "'s Armor is " + unitView.LocalArmor + ", should be " + unitState.armor, battleController);
				}
				if (unitView.LocalPreventReroll != unitState.preventReroll)
				{
					MakeSyncError("Battle Sync Error! Unit " + BattleAction.GetUnitID(unitState) + "'s PreventReroll flag is " + unitView.LocalPreventReroll + ", should be " + unitState.preventReroll, battleController);
				}
				if (unitView.LocalRoundsUntilRerollEnabled != unitState.roundsUntilRerollEnabled)
				{
					MakeSyncError("Battle Sync Error! Unit " + BattleAction.GetUnitID(unitState) + "'s RoundsUntilRerollEnabled is " + unitView.LocalRoundsUntilRerollEnabled + ", should be " + unitState.roundsUntilRerollEnabled, battleController);
				}
				if (unitView.LocalExtraDamage != unitState.extraDamage)
				{
					MakeSyncError("Battle Sync Error! Unit " + BattleAction.GetUnitID(unitState) + "'s ExtraDamage is " + unitView.LocalExtraDamage + ", should be " + unitState.extraDamage, battleController);
				}
				if (unitView.LocalDamagePerRound != unitState.damagePerRound)
				{
					MakeSyncError("Battle Sync Error! Unit " + BattleAction.GetUnitID(unitState) + "'s DamagePerRound is " + unitView.LocalDamagePerRound + ", should be " + unitState.damagePerRound, battleController);
				}
				if (unitView.LocalAcidPerRound != unitState.acidPerRound)
				{
					MakeSyncError("Battle Sync Error! Unit " + BattleAction.GetUnitID(unitState) + "'s AcidPerRound is " + unitView.LocalAcidPerRound + ", should be " + unitState.acidPerRound, battleController);
				}
				if (unitView.LocalTotalDamageReceived != unitState.totalDamageReceived)
				{
					MakeSyncError("Battle Sync Error! Unit " + BattleAction.GetUnitID(unitState) + "'s TotalDamageReceived is " + unitView.LocalTotalDamageReceived + ", should be " + unitState.totalDamageReceived, battleController);
				}
				foreach (AbilityState ability in unitState.abilities)
				{
					if (ability.animationHandler.DamageValuesCount > 0)
					{
						MakeSyncError(string.Concat("Battle Sync Error! Leftover damage values from unit '", BattleAction.GetUnitID(unitState), "' ability '", ability.animationHandler, "'"), battleController);
					}
				}
			}
			AbilityState[] abilities = teamState.abilities;
			foreach (AbilityState abilityState in abilities)
			{
				if (abilityState.animationHandler.DamageValuesCount > 0)
				{
					MakeSyncError(string.Concat("Battle Sync Error! Leftover damage values from player ability '", abilityState.animationHandler, "'"), battleController);
				}
			}
			List<UnitView> unitsByTeam = battleController.GetUnitsByTeam(teamState);
			if (unitsByTeam.Count != teamState.aliveUnits.Length)
			{
				MakeSyncError("Battle Sync Error! Alive unit lists have different lengths. UnitViews: " + unitsByTeam.Count + " - team.aliveUnits: " + teamState.aliveUnits.Length, battleController);
			}
			if (teamState.aliveUnits.Length != unitsByTeam.Count)
			{
				MakeSyncError("Battle Sync Error! Unit lists are not the same. UnitViews: " + unitsByTeam.Count + " - team.aliveUnits: " + teamState.aliveUnits.Length, battleController);
				continue;
			}
			for (int l = 0; l < teamState.aliveUnits.Length; l++)
			{
				if (unitsByTeam[l].state != teamState.aliveUnits[l])
				{
					MakeSyncError("Battle Sync Error! Alive units list is not in the correct order.", battleController);
				}
			}
		}
	}

	private static void MakeSyncError(string message, BattleController battleController)
	{
		Log.Error(message);
		string matchId = battleController.MatchData.matchId;
		Reporting.ClientBattleSyncError(message, matchId);
	}
}
