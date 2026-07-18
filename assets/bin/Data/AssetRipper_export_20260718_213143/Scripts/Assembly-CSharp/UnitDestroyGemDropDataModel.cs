using System.Collections.Generic;

public class UnitDestroyGemDropDataModel : BaseDataModel
{
	public int gemProcRate;

	public int maxGemNum;

	public int minGemNum;

	public int unitId;

	public static UnitDestroyGemDropDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitDestroyGemDropDataModel>(id.ToString());
	}

	public static UnitDestroyGemDropDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitDestroyGemDropDataModel>(id);
	}

	public static List<UnitDestroyGemDropDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitDestroyGemDropDataModel>();
	}
}
