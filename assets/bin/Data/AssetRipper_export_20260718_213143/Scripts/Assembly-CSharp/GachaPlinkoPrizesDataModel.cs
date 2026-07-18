using System.Collections.Generic;

public class GachaPlinkoPrizesDataModel : BaseDataModel
{
	public int itemId;

	public int itemType;

	public int itemsMixerId;

	public int orderNumber;

	public int priceId;

	public static GachaPlinkoPrizesDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPlinkoPrizesDataModel>(id.ToString());
	}

	public static GachaPlinkoPrizesDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPlinkoPrizesDataModel>(id);
	}

	public static List<GachaPlinkoPrizesDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<GachaPlinkoPrizesDataModel>();
	}

	public static List<GachaPlinkoPrizesDataModel> GetAllPrizes(int mixerId, string partFilterID = "")
	{
		int partFilterIDInt = int.Parse((partFilterID == null) ? "0" : partFilterID);
		return GetAll().FindAll((GachaPlinkoPrizesDataModel x) => x.itemsMixerId == mixerId && (partFilterID == null || x.itemId == partFilterIDInt));
	}
}
