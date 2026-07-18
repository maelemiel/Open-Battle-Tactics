using System.Collections.Generic;

public class UnitActionDataModel : BaseDataModel
{
	public static UnitActionDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitActionDataModel>(id.ToString());
	}

	public static UnitActionDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitActionDataModel>(id);
	}

	public static List<UnitActionDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitActionDataModel>();
	}
}
