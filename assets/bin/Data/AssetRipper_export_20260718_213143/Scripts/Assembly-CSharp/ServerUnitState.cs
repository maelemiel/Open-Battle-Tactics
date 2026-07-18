using System.Collections.Generic;

public class ServerUnitState
{
	public IUnitMetadata metadata;

	public int hp;

	public int rarity;

	public int armor;

	public int currentRoll;

	public int roundStartRoll;

	public List<ServerAbilityState> abilities;

	public List<IPartMetadata> droppedParts;

	public int destroyCoins;

	public int extraDestroyCoins;

	public int extraDestroyPartDropChance;

	public int extraDestroyEventPoints;

	public int killIndex;

	public int rollRepeats;

	public int damagePerRound;

	public int acidPerRound;

	public int extraDamage;

	public int gemsDropped;

	public bool preventReroll;

	public int roundsUntilRerollEnabled;

	public bool evadeNextAOE;

	public int totalDamageReceived;

	public ServerBattleState battle;

	public ServerTeamState team;

	public int index;

	public int startingHp
	{
		get
		{
			return metadata.StartingHealth;
		}
	}

	public DieFaceType[] rollTypes
	{
		get
		{
			return metadata.RollTypes;
		}
	}

	public int[] rollValues
	{
		get
		{
			return metadata.RollValues;
		}
	}

	public DieFaceType CurrentRollType
	{
		get
		{
			return metadata.RollTypes[currentRoll];
		}
	}

	public int CurrentRollValue
	{
		get
		{
			return metadata.RollValues[currentRoll];
		}
	}

	public bool IsDead
	{
		get
		{
			return hp <= 0;
		}
	}

	internal ServerUnitState Clone()
	{
		ServerUnitState serverUnitState = new ServerUnitState();
		serverUnitState.metadata = metadata;
		serverUnitState.hp = hp;
		serverUnitState.rarity = rarity;
		serverUnitState.armor = armor;
		serverUnitState.currentRoll = currentRoll;
		serverUnitState.roundStartRoll = roundStartRoll;
		if (droppedParts != null)
		{
			serverUnitState.droppedParts = new List<IPartMetadata>();
			foreach (IPartMetadata droppedPart in droppedParts)
			{
				serverUnitState.droppedParts.Add(droppedPart);
			}
		}
		serverUnitState.abilities = new List<ServerAbilityState>();
		foreach (ServerAbilityState ability in abilities)
		{
			serverUnitState.abilities.Add(ability.Clone());
		}
		serverUnitState.killIndex = killIndex;
		serverUnitState.rollRepeats = rollRepeats;
		serverUnitState.damagePerRound = damagePerRound;
		serverUnitState.acidPerRound = acidPerRound;
		serverUnitState.extraDamage = extraDamage;
		serverUnitState.preventReroll = preventReroll;
		serverUnitState.roundsUntilRerollEnabled = roundsUntilRerollEnabled;
		serverUnitState.destroyCoins = destroyCoins;
		serverUnitState.extraDestroyCoins = extraDestroyCoins;
		serverUnitState.extraDestroyPartDropChance = extraDestroyPartDropChance;
		serverUnitState.extraDestroyEventPoints = extraDestroyEventPoints;
		serverUnitState.evadeNextAOE = evadeNextAOE;
		serverUnitState.totalDamageReceived = totalDamageReceived;
		serverUnitState.gemsDropped = gemsDropped;
		return serverUnitState;
	}
}
