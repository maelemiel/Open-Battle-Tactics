using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitDataModel : BaseDataModel, IResearchableDataModel
{
	private List<UnitLevelProgressionDataModel> _cachedLevels;

	private UserPriceDataModel _cachedBuildPrice;

	private List<UnitPartialLevelDataModel> _cachedPartials;

	public int blueprintLinkageId;

	public int canBuyDirect;

	public int foundInGacha;

	public string keyName;

	public int rarity;

	public int researchTime;

	public int rewardAmount;

	public int rewardTypeId;

	public int type;

	public int unitIndex;

	public int unlockTier;

	public int weaponAnim;

	private IEnumerable<UnitPartsDataModel> _cachedParts;

	private IEnumerable<UnitPartsDataModel> _cachedDroppableParts;

	private IEnumerable<EventPartsDataModel> _cachedEventParts;

	private IEnumerable<EventPartsDataModel> _cachedDroppableEventParts;

	public string name
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(keyName);
		}
	}

	public List<UnitLevelProgressionDataModel> Levels
	{
		get
		{
			if (_cachedLevels == null)
			{
				_cachedLevels = NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitLevelProgressionDataModel>(" WHERE unit_id = " + id);
			}
			return _cachedLevels;
		}
	}

	public IEnumerable<UnitPartsDataModel> BuildableParts
	{
		get
		{
			return PartsList.Where((UnitPartsDataModel x) => x.amount > 0);
		}
	}

	public int MaxLevel
	{
		get
		{
			int num = 0;
			foreach (UnitLevelProgressionDataModel level in Levels)
			{
				if (!level.IsSkin)
				{
					num = Mathf.Max(num, level.level);
				}
			}
			return num;
		}
	}

	public long ResearchDuration
	{
		get
		{
			return researchTime * 1000;
		}
	}

	public bool CanBuyDirect
	{
		get
		{
			return canBuyDirect == 1;
		}
	}

	public AssetLinkageDataModel BlueprintLinkage
	{
		get
		{
			return AssetLinkageDataModel.GetSingle(blueprintLinkageId);
		}
	}

	public UnitRarityDataModel Rarity
	{
		get
		{
			return UnitRarityDataModel.GetSingle(rarity);
		}
	}

	public UnitType UnitType
	{
		get
		{
			return (UnitType)type;
		}
	}

	public bool IsAir
	{
		get
		{
			return UnitType == UnitType.AIR || UnitType == UnitType.EXCLUSIVE_AIR || UnitType == UnitType.EVENT_AIR;
		}
	}

	public bool IsAssault
	{
		get
		{
			return UnitType == UnitType.ASSAULT || UnitType == UnitType.EXCLUSIVE_ASSAULT || UnitType == UnitType.EVENT_ASSAULT;
		}
	}

	public bool IsCommand
	{
		get
		{
			return UnitType == UnitType.COMMAND || UnitType == UnitType.EXCLUSIVE_COMMAND || UnitType == UnitType.EVENT_COMMAND;
		}
	}

	public bool IsOperative
	{
		get
		{
			return UnitType == UnitType.OPERATIVE || UnitType == UnitType.EXCLUSIVE_OPERATIVE || UnitType == UnitType.EVENT_OPERATIVE;
		}
	}

	public IEnumerable<UnitPartsDataModel> PartsList
	{
		get
		{
			if (_cachedParts == null)
			{
				int intID = int.Parse(id);
				_cachedParts = from x in UnitPartsDataModel.GetAll()
					where x.unitId == intID
					select x;
			}
			return _cachedParts;
		}
	}

	public IEnumerable<UnitPartsDataModel> DroppableParts
	{
		get
		{
			if (_cachedDroppableParts == null)
			{
				_cachedDroppableParts = PartsList.Where((UnitPartsDataModel x) => x.dropRate > 0);
			}
			return _cachedDroppableParts;
		}
	}

	public IEnumerable<EventPartsDataModel> EventPartsList
	{
		get
		{
			if (_cachedEventParts == null)
			{
				int intID = int.Parse(id);
				_cachedEventParts = from x in EventPartsDataModel.GetAll()
					where x.unitId == intID
					select x;
			}
			return _cachedEventParts;
		}
	}

	public IEnumerable<EventPartsDataModel> DroppableEventParts
	{
		get
		{
			if (_cachedDroppableEventParts == null)
			{
				_cachedDroppableEventParts = EventPartsList.Where((EventPartsDataModel x) => x.dropRate > 0);
			}
			return _cachedDroppableEventParts;
		}
	}

	public static UnitDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitDataModel>(id.ToString());
	}

	public static UnitDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitDataModel>(id);
	}

	public static List<UnitDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitDataModel>();
	}

	public UserPriceDataModel GetBuildPrice(int timesBuilt = 0)
	{
		if (timesBuilt < 1 && _cachedBuildPrice != null)
		{
			return _cachedBuildPrice;
		}
		if (_cachedBuildPrice == null)
		{
			_cachedBuildPrice = new UserPriceDataModel();
			foreach (UnitPartsDataModel buildablePart in BuildableParts)
			{
				_cachedBuildPrice.AddItem(UserInventory.ItemType.Parts, buildablePart.partType, buildablePart.amount);
			}
		}
		return _cachedBuildPrice.Multiply((int)Math.Pow(Constants.RepeatTankBuildExponentialBase, timesBuilt));
	}

	public bool IsMaxLevel(int currentLevel)
	{
		foreach (UnitLevelProgressionDataModel level in Levels)
		{
			if (!level.IsSkin && level.level > currentLevel)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsLevelSkin(int currentLevel)
	{
		foreach (UnitLevelProgressionDataModel level in Levels)
		{
			if (level.level == currentLevel)
			{
				return level.IsSkin;
			}
		}
		return false;
	}

	public UnitLevelProgressionDataModel GetLevel(int currentLevel)
	{
		UnitLevelProgressionDataModel unitLevelProgressionDataModel = null;
		foreach (UnitLevelProgressionDataModel level in Levels)
		{
			if (unitLevelProgressionDataModel == null || level.level > unitLevelProgressionDataModel.level)
			{
				unitLevelProgressionDataModel = level;
			}
			if (level.level == currentLevel + 1)
			{
				return level;
			}
		}
		Log.ErrorTag("Could not find Level " + currentLevel + ", " + name + ". Returning level " + ((unitLevelProgressionDataModel == null) ? "Null" : unitLevelProgressionDataModel.level.ToString()), null, "DataModels");
		return unitLevelProgressionDataModel;
	}

	public static List<UnitDataModel> GetUnitsUnlockedAtTier(int tier)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitDataModel>(" WHERE unlock_tier = " + tier);
	}

	public static List<AbilityDataModel> GetAbilitiesUnlockedAtTier(int tier)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<AbilityDataModel>(" WHERE unlock_tier = " + tier);
	}

	public UserPriceDataModel GetResearchCost(UserProfile userProfile)
	{
		return GetBuildPrice(userProfile.TimesBuiltUnit(id));
	}

	public List<UnitPartialLevelDataModel> GetPartialsForUnit()
	{
		if (_cachedPartials == null)
		{
			_cachedPartials = NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitPartialLevelDataModel>(" WHERE unit_id = " + id);
		}
		return _cachedPartials;
	}
}
