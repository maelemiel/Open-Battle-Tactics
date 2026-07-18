using System.Collections.Generic;

public class GachaAbTestingGroupEnableDataModel : BaseDataModel
{
	public int abTestingId;

	public int groupNumber;

	public static GachaAbTestingGroupEnableDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaAbTestingGroupEnableDataModel>(id.ToString());
	}

	public static GachaAbTestingGroupEnableDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaAbTestingGroupEnableDataModel>(id);
	}

	public static List<GachaAbTestingGroupEnableDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<GachaAbTestingGroupEnableDataModel>();
	}
}
