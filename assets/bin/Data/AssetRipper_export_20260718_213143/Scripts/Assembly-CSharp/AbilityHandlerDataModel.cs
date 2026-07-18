using System.Collections.Generic;

public class AbilityHandlerDataModel : BaseDataModel
{
	public string handler;

	public static AbilityHandlerDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AbilityHandlerDataModel>(id.ToString());
	}

	public static AbilityHandlerDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AbilityHandlerDataModel>(id);
	}

	public static List<AbilityHandlerDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AbilityHandlerDataModel>();
	}
}
