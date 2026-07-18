using System.Collections.Generic;
using UnityEngine;

public class AiArmyDataModel : BaseDataModel
{
	private List<string> idList;

	public int abilityId0;

	public int abilityId1;

	public int abilityId2;

	public int abilityId3;

	public int aiStrategyId;

	public int difficultyId;

	public int divisionMaxId;

	public int divisionMinId;

	public int eventId;

	public string eventTimeActiveEnd;

	public string eventTimeActiveStart;

	public string keyName;

	public int promoUse;

	public int streak;

	public string teamType;

	public int unitLevelId1;

	public int unitLevelId2;

	public int unitLevelId3;

	public int unitLevelId4;

	private EventRaidbossDamageDropRateDataModel[] _cacheRewardDropRate;

	public string Name
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(keyName);
		}
	}

	public TeamType TeamType
	{
		get
		{
			switch (teamType)
			{
			case "bot":
			case "ai":
			case "reg":
				return TeamType.Bot;
			case "boss":
				return TeamType.Boss;
			case "raidboss":
				return TeamType.RaidBoss;
			default:
				return TeamType.Bot;
			}
		}
	}

	public EventRaidbossDamageDropRateDataModel[] RewardDropRate
	{
		get
		{
			if (_cacheRewardDropRate == null)
			{
				_cacheRewardDropRate = EventRaidbossDamageDropRateDataModel.GetTeamDamageDropRates(int.Parse(id)).ToArray();
			}
			return _cacheRewardDropRate;
		}
	}

	public static AiArmyDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AiArmyDataModel>(id.ToString());
	}

	public static AiArmyDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AiArmyDataModel>(id);
	}

	public static List<AiArmyDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AiArmyDataModel>();
	}

	public List<UserUnit> GetUnitList()
	{
		List<UserUnit> list = new List<UserUnit>();
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		EventUnitBoostDataModel eventUnitBoostDataModel = null;
		UserUnit userUnit = null;
		if (unitLevelId1 != 0)
		{
			list.Add(CreateUserUnit(unitLevelId1, activeEvent));
		}
		if (unitLevelId2 != 0)
		{
			list.Add(CreateUserUnit(unitLevelId2, activeEvent));
		}
		if (unitLevelId3 != 0)
		{
			list.Add(CreateUserUnit(unitLevelId3, activeEvent));
		}
		if (unitLevelId4 != 0)
		{
			list.Add(CreateUserUnit(unitLevelId4, activeEvent));
		}
		return list;
	}

	private UserUnit CreateUserUnit(int levelId, EventDataModel currentEvent)
	{
		UnitLevelProgressionDataModel single = UnitLevelProgressionDataModel.GetSingle(levelId);
		EventUnitBoostDataModel eventUnitBoostDataModel = null;
		EventPartsDataModel eventPartsDataModel = null;
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (currentEvent != null)
		{
			eventUnitBoostDataModel = EventUnitBoostDataModel.FindUnitBoost(single.unitId.ToString(), single.level, currentEvent.id);
			if (eventUnitBoostDataModel != null)
			{
				empty = eventUnitBoostDataModel.id;
			}
			eventPartsDataModel = EventPartsDataModel.FindUnitEventPart(single.unitId.ToString(), currentEvent.id);
			if (eventPartsDataModel != null)
			{
				empty2 = eventPartsDataModel.id;
			}
		}
		return new UserUnit(null, levelId, 0, empty, empty2);
	}

	public ProgressionDivisionDataModel GetRandomDivision()
	{
		int num = Random.Range(divisionMinId, divisionMaxId + 1);
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(num.ToString());
	}

	public List<string> GetAbilityIDs()
	{
		if (idList == null)
		{
			idList = new List<string>
			{
				abilityId0.ToString(),
				abilityId1.ToString(),
				abilityId2.ToString(),
				abilityId3.ToString()
			};
		}
		return idList;
	}
}
