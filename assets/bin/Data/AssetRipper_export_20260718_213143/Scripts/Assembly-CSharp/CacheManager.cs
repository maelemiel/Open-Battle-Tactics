using System;
using System.Collections.Generic;
using System.Linq;

public static class CacheManager
{
	public static Dictionary<string, string> constantsCache = new Dictionary<string, string>();

	private static Dictionary<Type, Dictionary<string, object>> cachedDM = new Dictionary<Type, Dictionary<string, object>>();

	public static Dictionary<Type, object> cachedLists = new Dictionary<Type, object>();

	public static Dictionary<int, Dictionary<int, UnitLevelProgressionDataModel>> unitLevelCache;

	public static Dictionary<string, HelpTopicDataModel> helpTopicsCache;

	public static Dictionary<string, List<HelpRegistersDataModel>> helpRegistersCache;

	public static Dictionary<int, EffectsDataModel> effectsCache;

	public static Dictionary<int, AudioTriggersDataModel> audioTriggersCache;

	public static Dictionary<string, NewsDataModel> newsCache;

	public static Dictionary<string, Dictionary<string, float>> boostAbilityMultipliers;

	public static Dictionary<string, object> GetSingleValueCache<T>()
	{
		Type typeFromHandle = typeof(T);
		Dictionary<string, object> value = null;
		if (!cachedDM.TryGetValue(typeFromHandle, out value))
		{
			value = new Dictionary<string, object>();
			cachedDM.Add(typeFromHandle, value);
		}
		return value;
	}

	public static bool ConstantExists(string name)
	{
		return constantsCache.ContainsKey(name);
	}

	public static int GetConstantInt(string constantName, int defaultValue)
	{
		if (!ConstantExists(constantName))
		{
			return defaultValue;
		}
		return int.Parse(constantsCache[constantName]);
	}

	public static string GetConstantString(string constantName, string defaultValue)
	{
		if (!ConstantExists(constantName))
		{
			return defaultValue;
		}
		return constantsCache[constantName];
	}

	public static void SetConstantString(string constantName, string value)
	{
		if (!ConstantExists(constantName))
		{
			constantsCache[constantName] = value;
		}
		else
		{
			constantsCache.Add(constantName, value);
		}
	}

	public static float GetConstantFloat(string constantName, float defaultValue)
	{
		if (!ConstantExists(constantName))
		{
			return defaultValue;
		}
		return float.Parse(constantsCache[constantName]);
	}

	public static void CacheUnitLevelTable()
	{
		unitLevelCache = new Dictionary<int, Dictionary<int, UnitLevelProgressionDataModel>>();
		foreach (UnitLevelProgressionDataModel item in UnitLevelProgressionDataModel.GetAll())
		{
			if (!unitLevelCache.ContainsKey(item.unitId))
			{
				unitLevelCache[item.unitId] = new Dictionary<int, UnitLevelProgressionDataModel>();
			}
			unitLevelCache[item.unitId][item.level] = item;
		}
	}

	public static UnitLevelProgressionDataModel GetCachedUnitLevel(int unitId, int level)
	{
		if (unitLevelCache == null || !unitLevelCache.ContainsKey(unitId))
		{
			return null;
		}
		Dictionary<int, UnitLevelProgressionDataModel> dictionary = unitLevelCache[unitId];
		if (!dictionary.ContainsKey(level))
		{
			return null;
		}
		return dictionary[level];
	}

	private static void CacheHelpInformation()
	{
		helpTopicsCache = new Dictionary<string, HelpTopicDataModel>();
		foreach (HelpTopicDataModel item in HelpTopicDataModel.GetAll())
		{
			if (!helpTopicsCache.ContainsKey(item.id))
			{
				helpTopicsCache.Add(item.id, item);
			}
		}
		helpRegistersCache = new Dictionary<string, List<HelpRegistersDataModel>>();
		foreach (HelpRegistersDataModel item2 in HelpRegistersDataModel.GetAll())
		{
			if (!helpRegistersCache.ContainsKey(item2.topicId.ToString()))
			{
				helpRegistersCache.Add(item2.topicId.ToString(), new List<HelpRegistersDataModel>());
			}
			helpRegistersCache[item2.topicId.ToString()].Add(item2);
		}
	}

	private static void CacheNews()
	{
		newsCache = new Dictionary<string, NewsDataModel>();
		List<NewsDataModel> list = (from x in NewsDataModel.GetAll()
			orderby x.orderNumber
			select x).ToList();
		foreach (NewsDataModel item in list)
		{
			newsCache.Add(item.id, item);
		}
	}

	public static void CacheBoostAbilityMultipliers()
	{
		boostAbilityMultipliers = new Dictionary<string, Dictionary<string, float>>();
		foreach (BoostAbilityMultiplierDataModel item in BoostAbilityMultiplierDataModel.GetAll())
		{
			string key = item.boostId.ToString();
			string key2 = item.abilityId.ToString();
			float value = (float)item.multiplier / 100f;
			if (boostAbilityMultipliers.ContainsKey(key))
			{
				if (boostAbilityMultipliers[key].ContainsKey(key2))
				{
					boostAbilityMultipliers[key][key2] = value;
				}
				else
				{
					boostAbilityMultipliers[key].Add(key2, value);
				}
			}
			else
			{
				boostAbilityMultipliers.Add(key, new Dictionary<string, float>());
				boostAbilityMultipliers[key].Add(key2, value);
			}
		}
	}

	public static void PurgeCache()
	{
		Log.Info("CacheManager::PurgeCache");
		cachedDM = new Dictionary<Type, Dictionary<string, object>>();
		cachedLists = new Dictionary<Type, object>();
		constantsCache = new Dictionary<string, string>();
		CacheHelpInformation();
		CacheNews();
		CacheBoostAbilityMultipliers();
		effectsCache = null;
		audioTriggersCache = null;
	}
}
