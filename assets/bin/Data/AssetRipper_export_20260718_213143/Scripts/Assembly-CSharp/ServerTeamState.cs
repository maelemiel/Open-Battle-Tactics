public class ServerTeamState
{
	public int randomSeed;

	public ServerTeamStatsState stats;

	public IDivisionMetadata divisionMetadata;

	public ServerUnitState[] units;

	public ServerUnitState[] aliveUnits;

	public ServerAbilityState[] abilities;

	public int energy;

	public int pvpRating;

	public bool forfeited;

	public int currentRoundKills;

	public IRandomProvider randomProvider;

	public TeamType type;

	public int intelBoost;

	public BoostDataModel[] boosts;

	public ServerBattleState battle;

	public ServerTeamState otherTeam;

	public ServerAbilityHandler[] abilityHandlers;

	public ServerAbilityHandler[] allBattleAbilities;

	public int index;

	public EventRaidbossDamageDropRateDataModel[] rewardDropRate;

	public bool IsDead
	{
		get
		{
			return aliveUnits.Length == 0;
		}
	}

	public bool IsInitiativeWinner
	{
		get
		{
			return this == battle.initiativeWinner;
		}
	}

	public bool IsBattleWinner
	{
		get
		{
			return this == battle.winningTeam;
		}
	}

	public bool IsActingTeam
	{
		get
		{
			return this == battle.actingTeam;
		}
	}

	public bool IsHostTeam
	{
		get
		{
			return this == battle.hostTeam;
		}
	}

	internal ServerTeamState Clone()
	{
		ServerTeamState serverTeamState = new ServerTeamState();
		serverTeamState.randomSeed = randomSeed;
		serverTeamState.stats = stats.Clone();
		serverTeamState.divisionMetadata = divisionMetadata;
		serverTeamState.units = new ServerUnitState[units.Length];
		for (int i = 0; i < units.Length; i++)
		{
			serverTeamState.units[i] = units[i].Clone();
		}
		serverTeamState.abilities = new ServerAbilityState[abilities.Length];
		for (int j = 0; j < abilities.Length; j++)
		{
			serverTeamState.abilities[j] = abilities[j].Clone();
		}
		serverTeamState.energy = energy;
		serverTeamState.forfeited = forfeited;
		serverTeamState.currentRoundKills = currentRoundKills;
		serverTeamState.randomProvider = randomProvider.Clone();
		serverTeamState.type = type;
		BattleLogic.UpdateAliveUnits(serverTeamState);
		serverTeamState.boosts = new BoostDataModel[boosts.Length];
		for (int k = 0; k < boosts.Length; k++)
		{
			serverTeamState.boosts[k] = boosts[k];
		}
		return serverTeamState;
	}

	public ServerAbilityState GetAbilityByType(string type)
	{
		ServerAbilityState[] array = abilities;
		foreach (ServerAbilityState serverAbilityState in array)
		{
			if (serverAbilityState.metadata.Type == type)
			{
				return serverAbilityState;
			}
		}
		return null;
	}

	public ServerAbilityState GetAbilityByID(string id)
	{
		ServerAbilityState[] array = abilities;
		foreach (ServerAbilityState serverAbilityState in array)
		{
			if (serverAbilityState.metadata.ID == id)
			{
				return serverAbilityState;
			}
		}
		return null;
	}
}
