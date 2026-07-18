using System;
using System.Collections.Generic;
using System.Linq;

public class UnitMetadataAdapter : IUnitMetadata
{
	public string unitID;

	public string unitMetaID;

	public string boostID;

	public string eventParts;

	public int unitLevel;

	private UnitPartialLevelDataModel.PartialLevel partialLevel;

	public UnitDataModel unitDataModel;

	public EventUnitBoostDataModel unitBoostData;

	public UnitLevelProgressionDataModel unitLevelModel;

	public UnitLevelUpRequirementDataModel unitLevelRequirementModel;

	public UnitDestroyGemDropDataModel unitFusionPayoutModel;

	public List<UnitSpecialDataModel> specialDataModels = new List<UnitSpecialDataModel>();

	public List<int> specialBoostValuesA = new List<int>();

	public List<int> specialBoostValuesB = new List<int>();

	private int[] cachedRolls;

	private DieFaceType[] cachedRollTypes;

	private IPartMetadata[] _cachedPartDropsArray;

	public bool HasEventParts
	{
		get
		{
			return !string.IsNullOrEmpty(eventParts) && eventParts != "0";
		}
	}

	public string ID
	{
		get
		{
			return unitID;
		}
	}

	public int Rarity
	{
		get
		{
			return unitLevelModel.Rarity;
		}
	}

	public int StartingHealth
	{
		get
		{
			if (unitBoostData != null)
			{
				return unitBoostData.hpBoost + unitLevelModel.hp + partialLevel.health;
			}
			return unitLevelModel.hp + partialLevel.health;
		}
	}

	public EventUnitBoostDataModel UnitBoost
	{
		get
		{
			return unitBoostData;
		}
	}

	public DieFaceType[] RollTypes
	{
		get
		{
			if (cachedRollTypes != null)
			{
				return cachedRollTypes;
			}
			cachedRollTypes = new DieFaceType[unitLevelModel.RollTypes.Length];
			for (int i = 0; i < unitLevelModel.RollTypes.Length; i++)
			{
				cachedRollTypes[i] = unitLevelModel.RollTypes[i];
				if (partialLevel.diceTypes[i] != DieFaceType.None)
				{
					cachedRollTypes[i] = partialLevel.diceTypes[i];
				}
			}
			return cachedRollTypes;
		}
	}

	public int[] RollValues
	{
		get
		{
			if (cachedRolls != null)
			{
				return cachedRolls;
			}
			cachedRolls = new int[RollTypes.Length];
			for (int i = 0; i < unitLevelModel.RollTypes.Length; i++)
			{
				cachedRolls[i] = unitLevelModel.RollValues[i] + partialLevel.diceValues[i];
				if (unitBoostData != null)
				{
					if (RollTypes[i] == DieFaceType.Initiative)
					{
						cachedRolls[i] += unitBoostData.dieBoostInitiative;
					}
					if (RollTypes[i] == DieFaceType.DirectDamage)
					{
						cachedRolls[i] += unitBoostData.dieBoostDamage;
					}
					if (RollTypes[i] == DieFaceType.ArmourPiercing)
					{
						cachedRolls[i] += unitBoostData.dieBoostArmourPiercing;
					}
					if (RollTypes[i] == DieFaceType.AcidStrike)
					{
						cachedRolls[i] += unitBoostData.dieBoostAcidStrike;
					}
				}
			}
			return cachedRolls;
		}
	}

	public int AssetBundleID
	{
		get
		{
			return unitLevelModel.assetBundleId;
		}
	}

	public int GemDropMin
	{
		get
		{
			if (unitFusionPayoutModel != null)
			{
				return unitFusionPayoutModel.minGemNum;
			}
			return 0;
		}
	}

	public int GemDropMax
	{
		get
		{
			if (unitFusionPayoutModel != null)
			{
				return unitFusionPayoutModel.maxGemNum;
			}
			return 0;
		}
	}

	public int GemDropChance
	{
		get
		{
			if (unitFusionPayoutModel != null)
			{
				return unitFusionPayoutModel.gemProcRate;
			}
			return 0;
		}
	}

	public int DestroyCash
	{
		get
		{
			return DefaultIfZero(unitLevelModel.killedCash, unitLevelRequirementModel.killedCash);
		}
	}

	public int SurviveCash
	{
		get
		{
			return DefaultIfZero(unitLevelModel.survivedCash, unitLevelRequirementModel.survivedCash);
		}
	}

	public int DestroyPoints
	{
		get
		{
			return DefaultIfZero(unitLevelModel.killedPoint, unitLevelRequirementModel.killedPoints);
		}
	}

	public int DestroyEventPoints
	{
		get
		{
			return DefaultIfZero(unitLevelModel.killedEventPoint, unitLevelRequirementModel.killedEventPoints);
		}
	}

	public int SurvivePoints
	{
		get
		{
			return DefaultIfZero(unitLevelModel.survivedPoint, unitLevelRequirementModel.survivedPoints);
		}
	}

	public IPartMetadata[] PartDrops
	{
		get
		{
			if (_cachedPartDropsArray == null)
			{
				_cachedPartDropsArray = unitDataModel.DroppableParts.ToArray();
				if (HasEventParts)
				{
					IPartMetadata[] second = unitDataModel.DroppableEventParts.ToArray();
					_cachedPartDropsArray = _cachedPartDropsArray.Union(second).ToArray();
				}
			}
			return _cachedPartDropsArray;
		}
	}

	public UnitType UnitType
	{
		get
		{
			return (UnitType)unitDataModel.type;
		}
	}

	public UnitMetadataAdapter(string unitID, string unitMetaID, int unitLevel, int partiallevel, string boostID, string eventParts)
	{
		this.unitID = unitID;
		this.eventParts = eventParts;
		SetData(unitMetaID, unitLevel, partiallevel, boostID);
	}

	public void SetData(string unitMetaID, int unitLevel, int partiallevelBitFlag, string boostID)
	{
		this.unitMetaID = unitMetaID;
		this.unitLevel = unitLevel;
		this.boostID = boostID;
		if (!string.IsNullOrEmpty(boostID) && boostID != "0")
		{
			unitBoostData = EventUnitBoostDataModel.GetSingle(boostID);
		}
		unitDataModel = UnitDataModel.GetSingle(unitMetaID);
		partialLevel = UnitPartialLevelDataModel.SetupPartialLevel(unitMetaID, unitLevel, partiallevelBitFlag);
		if (!string.IsNullOrEmpty(boostID) && boostID != "0" && unitBoostData == null)
		{
			Log.Error("Unit Boost Data Model not found. UnitID: " + this.unitMetaID + ", BoostId: " + boostID);
		}
		if (unitDataModel == null)
		{
			Log.Error("Unit Data Model not found. UnitID: " + this.unitMetaID + ".");
		}
		int intUnitMetaID = int.Parse(this.unitMetaID);
		unitLevelModel = CacheManager.GetCachedUnitLevel(intUnitMetaID, this.unitLevel);
		if (unitLevelModel == null)
		{
			unitLevelModel = UnitLevelProgressionDataModel.GetAll().Find((UnitLevelProgressionDataModel levelModel) => levelModel.unitId == intUnitMetaID && levelModel.level == this.unitLevel);
		}
		if (unitLevelModel == null)
		{
			Log.Error("Unit Level Progression Data Model not found. UnitID: " + this.unitMetaID + ". Level: " + this.unitLevel);
		}
		UnitSpecialDataModel unitSpecial = UnitSpecialDataModel.GetSingle((partialLevel.special1 != 0) ? partialLevel.special1 : unitLevelModel.specialId);
		if (unitSpecial != null && unitSpecial.Type != null)
		{
			specialDataModels.Add(UnitSpecialDataModel.GetSingle(unitLevelModel.specialId));
			specialBoostValuesA.Add(unitLevelModel.specialBoostValueA + partialLevel.special1BoostA);
			specialBoostValuesB.Add(unitLevelModel.specialBoostValueB + partialLevel.special1BoostB);
		}
		UnitSpecialDataModel unitSpecial2 = UnitSpecialDataModel.GetSingle((partialLevel.special1 != 0) ? partialLevel.special2 : unitLevelModel.passiveId);
		if (unitSpecial2 != null && unitSpecial2.Type != null)
		{
			specialDataModels.Add(UnitSpecialDataModel.GetSingle(unitLevelModel.passiveId));
			specialBoostValuesA.Add(unitLevelModel.passiveBoostValueA + partialLevel.special2BoostA);
			specialBoostValuesB.Add(unitLevelModel.passiveBoostValueB + partialLevel.special2BoostB);
		}
		SetupUnitBoostData(ref unitSpecial, ref unitSpecial2);
		unitFusionPayoutModel = UnitDestroyGemDropDataModel.GetAll().Find((UnitDestroyGemDropDataModel model) => model.unitId == intUnitMetaID);
		unitLevelRequirementModel = UnitLevelUpRequirementDataModel.GetSingle(unitLevelModel.levelUpRequirementId);
		if (unitLevelRequirementModel == null)
		{
			Log.Error("Unit LevelUp Requirement Data Model not found. UnitID: " + this.unitMetaID + ". Level: " + this.unitLevel);
		}
	}

	private void SetupUnitBoostData(ref UnitSpecialDataModel unitSpecial1, ref UnitSpecialDataModel unitSpecial2)
	{
		if (unitBoostData == null)
		{
			return;
		}
		if (unitBoostData.ability1Override != 0)
		{
			unitSpecial1 = UnitSpecialDataModel.GetSingle(unitBoostData.ability1Override);
			if (unitSpecial1 != null && unitSpecial1.Type != null)
			{
				if (specialDataModels.Count > 0)
				{
					specialDataModels[0] = unitSpecial1;
					List<int> list2;
					List<int> list = (list2 = specialBoostValuesA);
					int index2;
					int index = (index2 = 0);
					index2 = list2[index2];
					list[index] = index2 + unitBoostData.ability1BoostA;
					List<int> list4;
					List<int> list3 = (list4 = specialBoostValuesB);
					int index3 = (index2 = 0);
					index2 = list4[index2];
					list3[index3] = index2 + unitBoostData.ability1BoostB;
				}
				else
				{
					specialDataModels.Add(unitSpecial1);
					specialBoostValuesA.Add(unitBoostData.ability1BoostA);
					specialBoostValuesB.Add(unitBoostData.ability1BoostB);
				}
			}
		}
		if (unitBoostData.ability2Override != 0)
		{
			unitSpecial2 = UnitSpecialDataModel.GetSingle(unitBoostData.ability2Override);
			if (unitSpecial2 != null && unitSpecial2.Type != null)
			{
				if (specialDataModels.Count > 1)
				{
					specialDataModels[1] = unitSpecial2;
					List<int> list6;
					List<int> list5 = (list6 = specialBoostValuesA);
					int index2;
					int index4 = (index2 = 1);
					index2 = list6[index2];
					list5[index4] = index2 + unitBoostData.ability2BoostA;
					List<int> list8;
					List<int> list7 = (list8 = specialBoostValuesB);
					int index5 = (index2 = 1);
					index2 = list8[index2];
					list7[index5] = index2 + unitBoostData.ability2BoostB;
				}
				else
				{
					specialDataModels.Add(unitSpecial2);
					specialBoostValuesA.Add(unitBoostData.ability2BoostA);
					specialBoostValuesB.Add(unitBoostData.ability2BoostB);
				}
			}
		}
		bool flag = Array.Exists(specialDataModels.ToArray(), (UnitSpecialDataModel item) => item.ID == "60901");
		if (unitBoostData.bonusPointsBoost > 0 && !flag)
		{
			specialDataModels.Add(UnitSpecialDataModel.GetSingle("60901"));
			specialBoostValuesA.Add(unitBoostData.bonusPointsBoost);
			specialBoostValuesB.Add(0);
		}
		specialDataModels.Add(UnitSpecialDataModel.GetSingle("60908"));
		specialBoostValuesA.Add(0);
		specialBoostValuesB.Add(0);
	}

	public int GetAbilitiesCount()
	{
		return specialDataModels.Count;
	}

	public IAbilityMetadata GetAbilityMetaData(int index)
	{
		return specialDataModels[index];
	}

	public int GetAbilityBoostValueA(int index)
	{
		return specialBoostValuesA[index];
	}

	public int GetAbilityBoostValueB(int index)
	{
		return specialBoostValuesB[index];
	}

	private int DefaultIfZero(int firstValue, int defaultValue)
	{
		if (firstValue == 0)
		{
			return defaultValue;
		}
		return firstValue;
	}
}
