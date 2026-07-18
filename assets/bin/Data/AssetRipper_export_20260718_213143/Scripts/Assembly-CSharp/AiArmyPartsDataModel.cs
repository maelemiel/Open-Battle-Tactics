using System.Collections.Generic;

public class AiArmyPartsDataModel : BaseDataModel
{
	public int aiArmyId;

	public int partId;

	public static AiArmyPartsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AiArmyPartsDataModel>(id.ToString());
	}

	public static AiArmyPartsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AiArmyPartsDataModel>(id);
	}

	public static List<AiArmyPartsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AiArmyPartsDataModel>();
	}
}
