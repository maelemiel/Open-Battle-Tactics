using System.Collections.Generic;

public class BoostAbilityMultiplierDataModel : BaseDataModel
{
	public int abilityId;

	public int boostId;

	public int multiplier;

	public static BoostAbilityMultiplierDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<BoostAbilityMultiplierDataModel>(id.ToString());
	}

	public static BoostAbilityMultiplierDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<BoostAbilityMultiplierDataModel>(id);
	}

	public static List<BoostAbilityMultiplierDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<BoostAbilityMultiplierDataModel>();
	}
}
