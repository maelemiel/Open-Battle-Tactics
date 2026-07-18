using System.Collections.Generic;

public class AbilityTypeDataModel : BaseLocalizationDataModel
{
	public static AbilityTypeDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AbilityTypeDataModel>(id.ToString());
	}

	public static AbilityTypeDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AbilityTypeDataModel>(id);
	}

	public static List<AbilityTypeDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AbilityTypeDataModel>();
	}
}
