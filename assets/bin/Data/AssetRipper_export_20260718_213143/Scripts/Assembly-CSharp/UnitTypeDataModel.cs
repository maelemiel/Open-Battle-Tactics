using System.Collections.Generic;

public class UnitTypeDataModel : BaseDataModel
{
	public string keyName;

	public static UnitTypeDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitTypeDataModel>(id.ToString());
	}

	public static UnitTypeDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitTypeDataModel>(id);
	}

	public static List<UnitTypeDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitTypeDataModel>();
	}
}
