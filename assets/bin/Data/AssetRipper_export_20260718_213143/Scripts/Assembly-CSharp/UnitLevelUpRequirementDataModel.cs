using System.Collections.Generic;

public class UnitLevelUpRequirementDataModel : BaseDataModel
{
	public int currentLevel;

	public int gemToCashRate;

	public int killedCash;

	public int killedEventPoints;

	public int killedPoints;

	public int nextLevel;

	public int priceId;

	public int rarity;

	public int survivedCash;

	public int survivedPoints;

	public static UnitLevelUpRequirementDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitLevelUpRequirementDataModel>(id.ToString());
	}

	public static UnitLevelUpRequirementDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitLevelUpRequirementDataModel>(id);
	}

	public static List<UnitLevelUpRequirementDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitLevelUpRequirementDataModel>();
	}

	public static List<UnitLevelUpRequirementDataModel> GetLevelUpRequirementsForUnit(int unitRarity)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitLevelUpRequirementDataModel>(" WHERE rarity = " + unitRarity);
	}
}
