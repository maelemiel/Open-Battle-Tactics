using System.Collections.Generic;

public class UnitPartialLevelDataModel : BaseDataModel
{
	public class PartialLevel
	{
		public int[] diceValues;

		public DieFaceType[] diceTypes;

		public int health;

		public int special1;

		public int special1BoostA;

		public int special1BoostB;

		public int special2;

		public int special2BoostA;

		public int special2BoostB;

		public PartialLevel()
		{
			diceValues = new int[5];
			diceTypes = new DieFaceType[5];
		}
	}

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

	public int level;

	public int partIndex;

	public int passiveBoostValueA;

	public int passiveBoostValueB;

	public int passiveId;

	public int requirementPriceId;

	public int requirementTier;

	public int specialBoostValueA;

	public int specialBoostValueB;

	public int specialId;

	public int unitId;

	public static UnitPartialLevelDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitPartialLevelDataModel>(id.ToString());
	}

	public static UnitPartialLevelDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitPartialLevelDataModel>(id);
	}

	public static List<UnitPartialLevelDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitPartialLevelDataModel>();
	}

	public static List<UnitPartialLevelDataModel> GetPartialLevelsPrice(int price)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitPartialLevelDataModel>(" WHERE requirement_price_id = " + price);
	}

	public static List<UnitPartialLevelDataModel> GetPartialLevelsForUnit(string unitid, int level)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitPartialLevelDataModel>(" WHERE unit_id = " + unitid + " AND level = " + level);
	}

	public static UnitPartialLevelDataModel GetUnitPartialLevel(string unitId, int level, int partIndex)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingleByQuery<UnitPartialLevelDataModel>(" WHERE unit_id = " + unitId + " AND level = " + level + " AND part_index = " + partIndex);
	}

	public static PartialLevel SetupPartialLevel(string unitMetaID, int unitLevel, int levelBitFlag)
	{
		PartialLevel partialLevel = new PartialLevel();
		for (int i = 1; i <= 6; i++)
		{
			if ((levelBitFlag >> i) % 2 != 1)
			{
				continue;
			}
			UnitPartialLevelDataModel unitPartialLevel = GetUnitPartialLevel(unitMetaID, unitLevel, i);
			if (unitPartialLevel == null)
			{
				Log.ErrorTag("Couldn't find at index: " + i + " Metadata: " + unitMetaID + " Unit Level: " + unitLevel, null, "PartialLevel");
				continue;
			}
			partialLevel.diceValues[0] += unitPartialLevel.face1Value;
			partialLevel.diceValues[1] += unitPartialLevel.face2Value;
			partialLevel.diceValues[2] += unitPartialLevel.face3Value;
			partialLevel.diceValues[3] += unitPartialLevel.face4Value;
			partialLevel.diceValues[4] += unitPartialLevel.face5Value;
			partialLevel.diceTypes[0] = ((unitPartialLevel.face1Type == 0) ? partialLevel.diceTypes[0] : ((DieFaceType)unitPartialLevel.face1Type));
			partialLevel.diceTypes[1] = ((unitPartialLevel.face2Type == 0) ? partialLevel.diceTypes[1] : ((DieFaceType)unitPartialLevel.face2Type));
			partialLevel.diceTypes[1] = ((unitPartialLevel.face3Type == 0) ? partialLevel.diceTypes[2] : ((DieFaceType)unitPartialLevel.face3Type));
			partialLevel.diceTypes[1] = ((unitPartialLevel.face4Type == 0) ? partialLevel.diceTypes[3] : ((DieFaceType)unitPartialLevel.face4Type));
			partialLevel.diceTypes[1] = ((unitPartialLevel.face5Type == 0) ? partialLevel.diceTypes[4] : ((DieFaceType)unitPartialLevel.face5Type));
			partialLevel.health += unitPartialLevel.hp;
			if (unitPartialLevel.specialId != 0)
			{
				partialLevel.special1 = unitPartialLevel.specialId;
			}
			partialLevel.special1BoostA += unitPartialLevel.specialBoostValueA;
			partialLevel.special1BoostB += unitPartialLevel.specialBoostValueB;
			if (unitPartialLevel.passiveId != 0)
			{
				partialLevel.special2 = unitPartialLevel.passiveId;
			}
			partialLevel.special2BoostA += unitPartialLevel.passiveBoostValueA;
			partialLevel.special2BoostB += unitPartialLevel.passiveBoostValueB;
		}
		return partialLevel;
	}
}
