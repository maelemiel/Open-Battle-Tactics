using System;
using System.Collections.Generic;

public class UnitLevelProgressionDataModel : BaseDataModel
{
	public int alternativeWeapon;

	public int assetBundleId;

	public int evolutionStage;

	public int face1Type;

	public int face1Value;

	public int face2Type;

	public int face2Value;

	public int face3Type;

	public int face3Value;

	public int face4Type;

	public int face4Value;

	public int face5Type;

	public int face5Value;

	public int hp;

	public int isSkin;

	public int killedCash;

	public int killedEventPoint;

	public int killedPoint;

	public int level;

	public int levelUpRequirementId;

	public int onLevelGiftId;

	public int passiveBoostValueA;

	public int passiveBoostValueB;

	public int passiveId;

	public int priceId;

	public int rarity;

	public int specialBoostValueA;

	public int specialBoostValueB;

	public int specialId;

	public int survivedCash;

	public int survivedPoint;

	public int unitId;

	public static Dictionary<string, DieFaceType> faceTypeMap = new Dictionary<string, DieFaceType>
	{
		{
			"10000",
			DieFaceType.None
		},
		{
			"20000",
			DieFaceType.DirectDamage
		},
		{
			"30000",
			DieFaceType.Special
		},
		{
			"40000",
			DieFaceType.Initiative
		},
		{
			"50000",
			DieFaceType.Special
		},
		{
			"60000",
			DieFaceType.Special
		},
		{
			"70000",
			DieFaceType.ArmourPiercing
		},
		{
			"80000",
			DieFaceType.AcidStrike
		}
	};

	private int[] cachedRollValues;

	private DieFaceType[] cachedRollTypes;

	public bool IsSkin
	{
		get
		{
			return isSkin == 1;
		}
	}

	public UnitRarityDataModel RarityModel
	{
		get
		{
			return UnitRarityDataModel.GetSingle(Rarity);
		}
	}

	public int[] RollValues
	{
		get
		{
			if (cachedRollValues == null)
			{
				cachedRollValues = new int[5] { face1Value, face2Value, face3Value, face4Value, face5Value };
			}
			return cachedRollValues;
		}
	}

	public DieFaceType[] RollTypes
	{
		get
		{
			if (cachedRollTypes == null)
			{
				cachedRollTypes = new DieFaceType[5]
				{
					faceTypeMap[face1Type.ToString()],
					faceTypeMap[face2Type.ToString()],
					faceTypeMap[face3Type.ToString()],
					faceTypeMap[face4Type.ToString()],
					faceTypeMap[face5Type.ToString()]
				};
			}
			return cachedRollTypes;
		}
	}

	public UnitDataModel UnitDataModel
	{
		get
		{
			return UnitDataModel.GetSingle(unitId);
		}
	}

	public int Rarity
	{
		get
		{
			int num = 6;
			return Math.Min(num - 1, UnitDataModel.rarity + rarity);
		}
	}

	public static UnitLevelProgressionDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitLevelProgressionDataModel>(id.ToString());
	}

	public static UnitLevelProgressionDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitLevelProgressionDataModel>(id);
	}

	public static List<UnitLevelProgressionDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitLevelProgressionDataModel>();
	}

	public static List<UnitLevelProgressionDataModel> GetAllNonSkinLevels(string unitid, int startLevel)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitLevelProgressionDataModel>(" WHERE unit_id = " + unitid + " AND level >= " + startLevel + " AND is_skin = 0 ");
	}

	public static bool UnitHasSkins(string unitid)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitLevelProgressionDataModel>(" WHERE unit_id = " + unitid + " AND is_skin = 1 ").Count > 0;
	}
}
