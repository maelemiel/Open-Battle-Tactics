using System.Collections.Generic;

public class UnitCooldownDataModel : BaseDataModel
{
	public int rarity;

	public int seconds;

	public int unitLevel;

	public static UnitCooldownDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitCooldownDataModel>(id.ToString());
	}

	public static UnitCooldownDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitCooldownDataModel>(id);
	}

	public static List<UnitCooldownDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitCooldownDataModel>();
	}
}
