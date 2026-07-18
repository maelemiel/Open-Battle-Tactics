using System.Collections.Generic;

public class UnitSpecialHandlerDataModel : BaseDataModel
{
	public string handler;

	public static UnitSpecialHandlerDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitSpecialHandlerDataModel>(id.ToString());
	}

	public static UnitSpecialHandlerDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitSpecialHandlerDataModel>(id);
	}

	public static List<UnitSpecialHandlerDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitSpecialHandlerDataModel>();
	}
}
