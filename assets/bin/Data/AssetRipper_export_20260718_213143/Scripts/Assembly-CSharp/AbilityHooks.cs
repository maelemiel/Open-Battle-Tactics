public static class AbilityHooks
{
	public static ServerUnitState GetTargetForTeam(ServerTeamState team, ServerUnitState defaultTarget)
	{
		ServerAbilityHandler[] allBattleAbilities = team.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			defaultTarget = serverAbilityHandler.GetTargetForTeam(team, defaultTarget);
		}
		return defaultTarget;
	}

	public static int GetRollValueForUnit(ServerUnitState unit, int defaultValue)
	{
		ServerAbilityHandler[] allBattleAbilities = unit.team.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			defaultValue = serverAbilityHandler.GetRollValueForUnit(unit, defaultValue);
		}
		return defaultValue;
	}

	public static int GetDamageForUnitTarget(ServerUnitState unit, int defaultValue)
	{
		ServerAbilityHandler[] allBattleAbilities = unit.team.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			defaultValue = serverAbilityHandler.GetDamageForUnitTarget(unit, defaultValue);
		}
		return defaultValue;
	}

	public static void PreInitiativeEvent(ServerBattleState battle)
	{
		ServerAbilityHandler[] allBattleAbilities = battle.teamOne.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			if (serverAbilityHandler.PreInitiativeEvent())
			{
				if (battle.animationHandler != null)
				{
					battle.animationHandler.PreInitiativeAbility(serverAbilityHandler.abilityState);
				}
				if (battle.animationHandler != null)
				{
					battle.animationHandler.PreInitiativeResultsAbility(serverAbilityHandler.abilityState, serverAbilityHandler.abilityState.team.intelBoost, serverAbilityHandler.abilityState.team.otherTeam.intelBoost);
				}
				BattleLogic.DeactivateDeadUnitAbilities(battle);
			}
			if (battle.IsComplete)
			{
				break;
			}
		}
	}

	public static void PostInitiativeEvent(ServerBattleState battle, ServerTeamState initiativeWinner)
	{
		ServerAbilityHandler[] allBattleAbilities = initiativeWinner.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			if (serverAbilityHandler.PostInitiativeEvent())
			{
				if (battle.animationHandler != null)
				{
					battle.animationHandler.PostInitiativeAbility(serverAbilityHandler.abilityState);
				}
				BattleLogic.DeactivateDeadUnitAbilities(battle);
			}
			if (battle.IsComplete)
			{
				break;
			}
		}
	}

	public static void EndOfRoundWithAbilitiesEvent(ServerBattleState battle, ServerTeamState team)
	{
		ServerAbilityHandler[] allBattleAbilities = team.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			if (serverAbilityHandler.EndOfRoundWithAbilities() && battle.animationHandler != null)
			{
				battle.animationHandler.EndOfRoundWithAbilities(serverAbilityHandler.abilityState.team, serverAbilityHandler.abilityState);
			}
			if (battle.IsComplete)
			{
				break;
			}
		}
	}

	public static void PreFireEvent(ServerUnitState unit, ServerUnitState target)
	{
		ServerAbilityHandler[] allBattleAbilities = unit.team.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			if (serverAbilityHandler.UnitPreFiringEvent(unit, target) && unit.team.battle.animationHandler != null)
			{
				unit.team.battle.animationHandler.PreFireAbility(serverAbilityHandler.abilityState, unit, target);
			}
			if (unit.team.battle.IsComplete)
			{
				break;
			}
		}
	}

	public static void TeamBeginAttackEvent(ServerTeamState team)
	{
		ServerAbilityHandler[] allBattleAbilities = team.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			if (serverAbilityHandler.TeamBeginAttackEvent(team))
			{
				if (team.battle.animationHandler != null)
				{
					team.battle.animationHandler.TeamBeginAttackAbility(team, serverAbilityHandler.abilityState);
				}
				BattleLogic.DeactivateDeadUnitAbilities(team.battle);
			}
			if (team.battle.IsComplete)
			{
				break;
			}
		}
	}

	public static bool UnitFiringEvent(ServerUnitState unit)
	{
		bool result = false;
		ServerAbilityHandler[] allBattleAbilities = unit.team.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			if (serverAbilityHandler.UnitFiringEvent(unit))
			{
				result = true;
				if (unit.battle.animationHandler != null)
				{
					unit.battle.animationHandler.UnitFiringAbility(unit, serverAbilityHandler.abilityState, serverAbilityHandler.target);
				}
				BattleLogic.DeactivateDeadUnitAbilities(unit.battle);
			}
			if (unit.battle.animationHandler != null)
			{
				unit.battle.animationHandler.PassiveFiringAnimation(unit, serverAbilityHandler.abilityState);
			}
			if (unit.team.battle.IsComplete)
			{
				break;
			}
		}
		return result;
	}

	public static void UnitReceivedDamage(ServerUnitState unit, int damage, DamageType dmgType)
	{
		ServerAbilityHandler[] allBattleAbilities = unit.team.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			if (serverAbilityHandler.UnitReceivedDamageEvent(unit, damage, dmgType) && unit.battle.animationHandler != null)
			{
				unit.battle.animationHandler.UnitReceivedDamageAbility(unit, damage, dmgType, serverAbilityHandler.abilityState);
			}
			if (unit.team.battle.IsComplete)
			{
				break;
			}
		}
	}

	public static void UnitDiedEvent(ServerUnitState deadUnit, ServerTeamState damageSource)
	{
		ServerAbilityHandler[] allBattleAbilities = damageSource.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			if (serverAbilityHandler.UnitDiedEvent(deadUnit, damageSource) && deadUnit.battle.animationHandler != null)
			{
				deadUnit.battle.animationHandler.UnitDiedAbility(deadUnit, damageSource, serverAbilityHandler.abilityState);
			}
		}
	}
}
