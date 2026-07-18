using System.Collections.Generic;

public class GachaInfoDetailsDataModel : BaseDataModel
{
	public int assetId;

	public int displayOrder;

	public int gachaId;

	public string keyText;

	public int spacing;

	public static GachaInfoDetailsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaInfoDetailsDataModel>(id.ToString());
	}

	public static GachaInfoDetailsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaInfoDetailsDataModel>(id);
	}

	public static List<GachaInfoDetailsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<GachaInfoDetailsDataModel>();
	}
}
