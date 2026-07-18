using System.Collections.Generic;

public class GachaPlinkoDetailsDataModel : BaseLocalizationDataModel
{
	public static GachaPlinkoDetailsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPlinkoDetailsDataModel>(id.ToString());
	}

	public static GachaPlinkoDetailsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPlinkoDetailsDataModel>(id);
	}

	public static List<GachaPlinkoDetailsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<GachaPlinkoDetailsDataModel>();
	}
}
