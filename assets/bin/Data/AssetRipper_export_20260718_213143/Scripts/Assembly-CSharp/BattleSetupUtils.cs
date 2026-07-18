using System;
using System.Collections.Generic;

public class BattleSetupUtils
{
	public static TBattleType CreateBattleState<TBattleType>(int randomSeed, ServerTeamState teamOne, ServerTeamState teamTwo, bool teamOneIsHost = true, bool autoBeginBattle = true) where TBattleType : ServerBattleState, new()
	{
		TBattleType val = new TBattleType();
		InitBattleRandom(val, randomSeed);
		val.teamOne = teamOne;
		val.teamTwo = teamTwo;
		val.hostTeam = ((!teamOneIsHost) ? teamTwo : teamOne);
		val.constantsProvider = new BattleConstantsProvider();
		if (autoBeginBattle)
		{
			BattleLogic.BeginBattle(val);
		}
		return val;
	}

	public static TTeamType CreateTeamState<TTeamType, TUnitType, TAbilityType>(OpponentData opponent) where TTeamType : ServerTeamState, new() where TUnitType : ServerUnitState, new() where TAbilityType : ServerAbilityState, new()
	{
		TTeamType val = new TTeamType();
		InitTeamRandom(val, opponent.randomSeed);
		val.type = opponent.type;
		val.divisionMetadata = opponent.division;
		List<TUnitType> list = new List<TUnitType>();
		foreach (IUnitMetadata unit in opponent.units)
		{
			if (unit != null)
			{
				TUnitType val2 = CreateUnitState<TUnitType, TAbilityType>(unit);
				if (unit.UnitType == UnitType.RAID_BOSS)
				{
					val2.hp = opponent.startingBossHealth;
				}
				list.Add(val2);
			}
		}
		val.units = list.ToArray();
		List<TAbilityType> list2 = new List<TAbilityType>();
		foreach (IAbilityMetadata ability in opponent.abilities)
		{
			if (ability != null)
			{
				list2.Add(CreateAbilityState<TAbilityType>(ability));
			}
		}
		val.pvpRating = opponent.pvpRating;
		val.abilities = list2.ToArray();
		val.type = opponent.type;
		val.rewardDropRate = opponent.rewardDropRate;
		List<BoostDataModel> list3 = new List<BoostDataModel>(opponent.boosts.Count);
		foreach (int boost in opponent.boosts)
		{
			BoostDataModel single = BoostDataModel.GetSingle(boost);
			if (single != null)
			{
				list3.Add(single);
			}
		}
		val.boosts = list3.ToArray();
		return val;
	}

	public static TUnitType CreateUnitState<TUnitType, TAbilityType>(IUnitMetadata metadata) where TUnitType : ServerUnitState, new() where TAbilityType : ServerAbilityState, new()
	{
		TUnitType val = new TUnitType
		{
			metadata = metadata,
			hp = metadata.StartingHealth
		};
		Console.WriteLine(string.Concat("Metadata information: ", metadata, " ", (metadata.GetAbilitiesCount() <= 0) ? string.Empty : metadata.GetAbilityMetaData(0).ToString()));
		val.abilities = new List<ServerAbilityState>();
		int i = 0;
		for (int abilitiesCount = metadata.GetAbilitiesCount(); i < abilitiesCount; i++)
		{
			if (metadata.GetAbilityMetaData(i) != null && metadata.GetAbilityMetaData(i).Type != null)
			{
				val.abilities.Add(CreateAbilityState<TAbilityType>(metadata.GetAbilityMetaData(i)));
				val.abilities[i].abilityIndex = i;
				val.abilities[i].target = val;
			}
		}
		return val;
	}

	public static TAbilityType CreateAbilityState<TAbilityType>(IAbilityMetadata metadata) where TAbilityType : ServerAbilityState, new()
	{
		return new TAbilityType
		{
			metadata = metadata
		};
	}

	public static void InitTeamRandom(ServerTeamState team, int randomSeed)
	{
		team.randomSeed = randomSeed;
		team.randomProvider = new MicrosoftRNG((uint)randomSeed);
	}

	public static void InitBattleRandom(ServerBattleState battle, int randomSeed)
	{
		battle.randomSeed = randomSeed;
		battle.randomProvider = new MicrosoftRNG((uint)randomSeed);
	}
}
