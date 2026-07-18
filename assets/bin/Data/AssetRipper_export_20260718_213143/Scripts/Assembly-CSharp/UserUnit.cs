using System.Collections.Generic;
using System.Linq;
using LitJson0;

public class UserUnit : IUnitMetadata
{
	private UnitMetadataAdapter unitMetadata;

	public string id;

	public string metadataId;

	public string boostId;

	public string eventParts;

	public int level;

	public int partialLevel;

	public UserTeam Team
	{
		get
		{
			foreach (UserTeam team in UserProfile.player.teams)
			{
				if (team.Contains(id))
				{
					return team;
				}
			}
			return null;
		}
	}

	public EventUnitBoostDataModel UnitBoost
	{
		get
		{
			return unitMetadata.UnitBoost;
		}
	}

	public int MaxLevel
	{
		get
		{
			return unitMetadata.unitDataModel.MaxLevel;
		}
	}

	public bool IsMaxLevel
	{
		get
		{
			return level >= MaxLevel;
		}
	}

	public int CooldownCost
	{
		get
		{
			UnitCooldownDataModel unitCooldownDataModel = UnitCooldownDataModel.GetAll().Find((UnitCooldownDataModel model) => model.rarity == unitMetadata.Rarity && model.unitLevel == level);
			return (unitCooldownDataModel != null) ? unitCooldownDataModel.seconds : 0;
		}
	}

	public bool IsOnCooldown
	{
		get
		{
			UserTeam team = Team;
			if (team != null)
			{
				return team.IsOnCooldown;
			}
			return false;
		}
	}

	public string Name
	{
		get
		{
			return unitMetadata.unitDataModel.name;
		}
	}

	public string AbilityName
	{
		get
		{
			if (unitMetadata.GetAbilitiesCount() > 0)
			{
				return string.Format(unitMetadata.GetAbilityMetaData(0).Name, GetAbilityBoostValueA(0), GetAbilityBoostValueB(0));
			}
			return string.Empty;
		}
	}

	public UnitWeaponType WeaponType
	{
		get
		{
			return (UnitWeaponType)unitMetadata.unitDataModel.weaponAnim;
		}
	}

	public bool HasSpecial
	{
		get
		{
			return unitMetadata.GetAbilitiesCount() > 0;
		}
	}

	public UnitDataModel UnitDataModel
	{
		get
		{
			return unitMetadata.unitDataModel;
		}
	}

	public int Rarity
	{
		get
		{
			return unitMetadata.Rarity;
		}
	}

	public string ID
	{
		get
		{
			return unitMetadata.ID;
		}
	}

	public int StartingHealth
	{
		get
		{
			return unitMetadata.StartingHealth;
		}
	}

	public DieFaceType[] RollTypes
	{
		get
		{
			return unitMetadata.RollTypes;
		}
	}

	public int[] RollValues
	{
		get
		{
			return unitMetadata.RollValues;
		}
	}

	public int AssetBundleID
	{
		get
		{
			return unitMetadata.AssetBundleID;
		}
	}

	public int GemDropMin
	{
		get
		{
			return unitMetadata.GemDropMin;
		}
	}

	public int GemDropMax
	{
		get
		{
			return unitMetadata.GemDropMax;
		}
	}

	public int GemDropChance
	{
		get
		{
			return unitMetadata.GemDropChance;
		}
	}

	public int DestroyCash
	{
		get
		{
			return unitMetadata.DestroyCash;
		}
	}

	public int SurviveCash
	{
		get
		{
			return unitMetadata.SurviveCash;
		}
	}

	public int DestroyPoints
	{
		get
		{
			return unitMetadata.DestroyPoints;
		}
	}

	public int DestroyEventPoints
	{
		get
		{
			return unitMetadata.DestroyEventPoints;
		}
	}

	public int SurvivePoints
	{
		get
		{
			return unitMetadata.SurvivePoints;
		}
	}

	public IPartMetadata[] PartDrops
	{
		get
		{
			return unitMetadata.PartDrops;
		}
	}

	public UnitType UnitType
	{
		get
		{
			return unitMetadata.UnitType;
		}
	}

	public UnitLevelProgressionDataModel CurrentLevelDataModel
	{
		get
		{
			return UnitDataModel.GetLevel(level - 1);
		}
	}

	public int AlternativeWeapon
	{
		get
		{
			return CurrentLevelDataModel.alternativeWeapon;
		}
	}

	public UserUnit(string id, string unitMetadataId, int level, int partialLevel, string boostId, string eventParts)
	{
		this.id = id;
		metadataId = unitMetadataId;
		this.boostId = boostId;
		this.eventParts = eventParts;
		SetLevel(level, partialLevel, boostId);
	}

	public UserUnit(string id, string unitLevelMetadataId, int partialLevel, string boostId, string partsId)
	{
		this.id = id;
		this.boostId = boostId;
		eventParts = partsId;
		UnitLevelProgressionDataModel single = UnitLevelProgressionDataModel.GetSingle(unitLevelMetadataId);
		metadataId = single.unitId.ToString();
		SetLevel(single.level, partialLevel, this.boostId);
	}

	public UserUnit(string id, int unitLevelMetadataId, int partialLevel, string boostId, string partsId)
		: this(id, unitLevelMetadataId.ToString(), partialLevel, boostId, partsId)
	{
	}

	public static UserUnit FromLevelID(string id, string unitLevelID)
	{
		return new UserUnit(id, unitLevelID, 0, string.Empty, string.Empty);
	}

	public void IncreaseMaxCashLevel()
	{
		SetLevel(GetMaxCashLevel(), 0, boostId);
	}

	public void IncreaseLevel()
	{
		SetLevel(level + 1, 0, boostId);
	}

	public void SetLevel(int level, int partialLevel, string boostId)
	{
		this.level = level;
		this.partialLevel = partialLevel;
		unitMetadata = new UnitMetadataAdapter(id, metadataId, level, partialLevel, boostId, eventParts);
	}

	public void UpdateBoost(EventUnitBoostDataModel boostModel)
	{
		if (boostModel != null)
		{
			boostId = boostModel.id;
			unitMetadata.boostID = boostModel.id;
			unitMetadata.unitBoostData = boostModel;
		}
		else
		{
			boostId = string.Empty;
			unitMetadata.boostID = string.Empty;
			unitMetadata.unitBoostData = null;
		}
	}

	public void UpdateEventParts(string partsID)
	{
		eventParts = partsID;
		unitMetadata.eventParts = partsID;
	}

	public int GetAbilitiesCount()
	{
		return unitMetadata.GetAbilitiesCount();
	}

	public IAbilityMetadata GetAbilityMetaData(int index)
	{
		return unitMetadata.GetAbilityMetaData(index);
	}

	public int GetAbilityBoostValueA(int index)
	{
		return unitMetadata.GetAbilityBoostValueA(index);
	}

	public int GetAbilityBoostValueB(int index)
	{
		return unitMetadata.GetAbilityBoostValueB(index);
	}

	public static UserUnit FromJSON(JsonObject json)
	{
		string text = json.GetString("_id");
		string unitMetadataId = json.GetString("meta_id");
		string text2 = json.GetString("boost_id");
		string text3 = json.GetString("event_parts");
		int num = json.GetInt("partial_level");
		int num2 = json.GetInt("lvl");
		UserUnit userUnit = null;
		return new UserUnit(text, unitMetadataId, num2, num, text2, text3);
	}

	public UserPriceDataModel GetUpgradePrice()
	{
		UnitLevelUpRequirementDataModel unitLevelRequirementModel = unitMetadata.unitLevelRequirementModel;
		if (unitLevelRequirementModel == null)
		{
			return null;
		}
		return ItemPriceDataModel.GetPriceForID(unitLevelRequirementModel.priceId);
	}

	public UserPriceDataModel GetMaxUpgradePrice()
	{
		List<UnitLevelProgressionDataModel> allNonSkinLevels = UnitLevelProgressionDataModel.GetAllNonSkinLevels(metadataId, level);
		UserPriceDataModel userPriceDataModel = new UserPriceDataModel(UserInventory.ItemType.Coins, 0);
		for (int i = 0; i < allNonSkinLevels.Count; i++)
		{
			UnitLevelUpRequirementDataModel single = UnitLevelUpRequirementDataModel.GetSingle(allNonSkinLevels[i].levelUpRequirementId);
			UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(single.priceId);
			if (priceForID.HasItemOfType(UserInventory.ItemType.Coins) && priceForID.items.Count == 1)
			{
				userPriceDataModel.items[0].amount += priceForID.items[0].amount;
			}
		}
		return userPriceDataModel;
	}

	public int GetMaxCashLevel()
	{
		List<UnitLevelProgressionDataModel> allNonSkinLevels = UnitLevelProgressionDataModel.GetAllNonSkinLevels(metadataId, 1);
		List<UnitPartialLevelDataModel> partialsForUnit = UnitDataModel.GetPartialsForUnit();
		int num = int.MaxValue;
		for (int i = 0; i < partialsForUnit.Count; i++)
		{
			if (partialsForUnit[i].level < num)
			{
				num = partialsForUnit[i].level;
			}
		}
		int num2 = 1;
		Log.DebugTag("Levels Count for CASH: " + allNonSkinLevels.Count + " " + num, null, "UserUnit");
		for (int j = 0; j < allNonSkinLevels.Count; j++)
		{
			UnitLevelUpRequirementDataModel single = UnitLevelUpRequirementDataModel.GetSingle(allNonSkinLevels[j].levelUpRequirementId);
			UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(single.priceId);
			Log.DebugTag("Bools!: " + priceForID.HasItemOfType(UserInventory.ItemType.Coins) + " " + (single.currentLevel > num2) + " " + single.currentLevel, null, "UserUnit");
			if (priceForID.HasItemOfType(UserInventory.ItemType.Coins) && priceForID.items.Count == 1 && single.nextLevel > num2 && single.nextLevel <= num)
			{
				num2 = single.nextLevel;
			}
		}
		Log.DebugTag("Max Level for CASH: " + num2, null, "UserUnit");
		return num2;
	}

	public ItemCollectionDataModel GetScrap()
	{
		UnitScrapValueDataModel unitScrapValueDataModel = (from x in UnitScrapValueDataModel.GetAll()
			where x.rarity == Rarity && x.unitLevel == level
			select x).First();
		if (unitScrapValueDataModel == null)
		{
			return null;
		}
		return ItemGiftDataModel.GetGiftPackage(unitScrapValueDataModel.giftId);
	}

	public int GetUpgradeGemExchangeRate()
	{
		UnitLevelUpRequirementDataModel unitLevelRequirementModel = unitMetadata.unitLevelRequirementModel;
		if (unitLevelRequirementModel == null)
		{
			return 0;
		}
		return unitLevelRequirementModel.gemToCashRate;
	}

	public bool UpgradeCostsParts()
	{
		List<UnitPartialLevelDataModel> partialsForUnit = UnitDataModel.GetPartialsForUnit();
		return partialsForUnit.Count > 0;
	}

	public List<UnitPartialLevelDataModel> GetPartialLevelsForCurrentLevel()
	{
		List<UnitPartialLevelDataModel> partialsForUnit = UnitDataModel.GetPartialsForUnit();
		List<UnitPartialLevelDataModel> list = new List<UnitPartialLevelDataModel>();
		for (int i = 0; i < partialsForUnit.Count; i++)
		{
			UnitPartialLevelDataModel unitPartialLevelDataModel = partialsForUnit[i];
			if (unitPartialLevelDataModel.level == level)
			{
				list.Add(unitPartialLevelDataModel);
			}
		}
		return list;
	}
}
