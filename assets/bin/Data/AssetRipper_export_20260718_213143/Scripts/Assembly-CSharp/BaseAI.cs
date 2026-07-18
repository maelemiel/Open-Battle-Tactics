using System.Collections.Generic;

public class BaseAI
{
	public void ThinkAndApply(ServerTeamState team)
	{
		List<BattleAction> list = Think(team);
		foreach (BattleAction item in list)
		{
			BattleLogic.ApplyBattleAction(team, item);
		}
	}

	public virtual List<BattleAction> Think(ServerTeamState team)
	{
		return null;
	}

	public virtual bool ShouldUnitTriggerSpecial(ServerUnitState unit)
	{
		return false;
	}

	protected ServerTeamState CloneState(ServerTeamState team)
	{
		ServerBattleState battle = team.battle;
		ServerBattleState serverBattleState = battle.Clone();
		return (team != battle.teamOne) ? serverBattleState.teamTwo : serverBattleState.teamOne;
	}

	public void InvestEnergyToActivateAbility(ServerAbilityState ability, ServerUnitState target, List<BattleAction> actionList)
	{
		while (ability.team.energy > 0)
		{
			if (ability.metadata.Cost - ability.energy <= 1)
			{
				if (actionList != null)
				{
					actionList.Add(UseAbilityAction.Create(ability, target));
				}
				BattleLogic.ActivateAbility(ability, target);
				break;
			}
			if (actionList != null)
			{
				actionList.Add(InvestEnergyAction.Create(ability));
			}
			BattleLogic.InvestAbilityEnergy(ability);
		}
	}

	protected static ServerUnitState GetWeakestOpponentUnit(ServerTeamState team)
	{
		ServerUnitState serverUnitState = null;
		ServerUnitState[] aliveUnits = team.otherTeam.aliveUnits;
		foreach (ServerUnitState serverUnitState2 in aliveUnits)
		{
			if (serverUnitState == null || serverUnitState2.hp < serverUnitState.hp)
			{
				serverUnitState = serverUnitState2;
			}
		}
		return serverUnitState;
	}

	protected static ServerUnitState GetRandomOpponentUnit(ServerTeamState team)
	{
		return BattleLogic.GetRandomUnitOnTeam(team.otherTeam, team, "GetRandomOpponentUnit");
	}

	protected static ServerUnitState GetRandomPlayerUnit(ServerTeamState team)
	{
		return BattleLogic.GetRandomUnitOnTeam(team, team, "GetRandomPlayerUnit");
	}

	protected static ServerUnitState GetStrongestUnit(ServerTeamState team)
	{
		ServerUnitState serverUnitState = null;
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState2 in aliveUnits)
		{
			if (serverUnitState == null || serverUnitState2.hp > serverUnitState.hp)
			{
				serverUnitState = serverUnitState2;
			}
		}
		return serverUnitState;
	}

	protected static ServerUnitState GetStrongestOpponentUnit(ServerTeamState team)
	{
		ServerUnitState serverUnitState = null;
		ServerUnitState[] aliveUnits = team.otherTeam.aliveUnits;
		foreach (ServerUnitState serverUnitState2 in aliveUnits)
		{
			if (serverUnitState == null || serverUnitState2.hp > serverUnitState.hp)
			{
				serverUnitState = serverUnitState2;
			}
		}
		return serverUnitState;
	}

	protected static ServerUnitState GetWeakestUnit(ServerTeamState team)
	{
		ServerUnitState serverUnitState = null;
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState2 in aliveUnits)
		{
			if (serverUnitState == null || serverUnitState2.hp < serverUnitState.hp)
			{
				serverUnitState = serverUnitState2;
			}
		}
		return serverUnitState;
	}

	protected static ServerUnitState GetRandomTargetForAbility(ServerAbilityState ability)
	{
		switch (ability.metadata.TargetSelectType)
		{
		case TargetType.EnemyUnit:
			return GetRandomOpponentUnit(ability.team);
		case TargetType.PlayerUnit:
			return GetRandomPlayerUnit(ability.team);
		default:
			return null;
		}
	}

	protected static bool RandomChance(ServerTeamState team, float percent)
	{
		return (float)BattleLogic.GetNextTeamRandom(team, 0, 100, "BaseAI-RandomChance") < percent * 100f;
	}

	protected static int GetTotalHP(ServerTeamState team)
	{
		int num = 0;
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState in aliveUnits)
		{
			num += serverUnitState.hp;
		}
		return num;
	}

	protected static bool HasTypeFace(ServerUnitState unit, DieFaceType typeToCheck)
	{
		for (int i = 0; i < unit.rollTypes.Length; i++)
		{
			if (unit.rollTypes[i] == typeToCheck)
			{
				return true;
			}
		}
		return false;
	}

	protected static List<ServerUnitState> GetUnitsWithDieFaceType(ServerTeamState team, DieFaceType typeToCheck)
	{
		List<ServerUnitState> list = new List<ServerUnitState>();
		for (int i = 0; i < team.aliveUnits.Length; i++)
		{
			if (HasTypeFace(team.aliveUnits[i], typeToCheck))
			{
				list.Add(team.aliveUnits[i]);
			}
		}
		return list;
	}

	protected static bool HasSpecialFace(ServerUnitState unit)
	{
		return HasTypeFace(unit, DieFaceType.Special);
	}

	protected static List<ServerUnitState> GetUnitsWithSpecialFaces(ServerTeamState team)
	{
		return GetUnitsWithDieFaceType(team, DieFaceType.Special);
	}

	protected static bool HasFirstStrikeFace(ServerUnitState unit)
	{
		return HasTypeFace(unit, DieFaceType.Initiative);
	}

	protected static List<ServerUnitState> GetUnitsWithFirstStrikelFaces(ServerTeamState team)
	{
		return GetUnitsWithDieFaceType(team, DieFaceType.Initiative);
	}

	protected static int GetCurrentTotalDamage(ServerTeamState team)
	{
		int num = 0;
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState in aliveUnits)
		{
			num += serverUnitState.CurrentRollValue;
		}
		return num;
	}

	protected static int GetCurrentTotalFirstStrike(ServerTeamState team)
	{
		int num = 0;
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState in aliveUnits)
		{
			if (serverUnitState.CurrentRollType == DieFaceType.Initiative)
			{
				num += serverUnitState.CurrentRollValue;
			}
		}
		return num;
	}

	protected static int GetBestDieFaceTypeValue(ServerUnitState unit, DieFaceType dieFaceType)
	{
		int num = 0;
		for (int i = 0; i < unit.rollTypes.Length; i++)
		{
			if (unit.rollTypes[i] == dieFaceType && num < unit.rollValues[i])
			{
				num = unit.rollValues[i];
			}
		}
		return num;
	}

	protected static int GetBestDamageValue(ServerUnitState unit)
	{
		return GetBestDieFaceTypeValue(unit, DieFaceType.DirectDamage);
	}

	protected static int GetBestFirstStrikeValue(ServerUnitState unit)
	{
		return GetBestDieFaceTypeValue(unit, DieFaceType.Initiative);
	}

	protected static int GetBestPotentialTotalForDieFaceType(ServerTeamState team, DieFaceType dieFaceType)
	{
		int num = 0;
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState unit in aliveUnits)
		{
			num += GetBestDieFaceTypeValue(unit, dieFaceType);
		}
		return num;
	}

	protected static int GetBestPotentialTotalDamage(ServerTeamState team)
	{
		return GetBestPotentialTotalForDieFaceType(team, DieFaceType.DirectDamage);
	}

	protected static int GetBestPotentialTotalFirstStrike(ServerTeamState team)
	{
		return GetBestPotentialTotalForDieFaceType(team, DieFaceType.Initiative);
	}

	public static void DebugTest(ServerTeamState team)
	{
	}
}
