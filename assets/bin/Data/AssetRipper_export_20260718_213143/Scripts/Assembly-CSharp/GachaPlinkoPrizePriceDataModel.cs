using System.Collections.Generic;

public class GachaPlinkoPrizePriceDataModel : BaseDataModel
{
	public int amount;

	public int itemId;

	public int itemType;

	public int orderNumber;

	public int priceId;

	public static GachaPlinkoPrizePriceDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPlinkoPrizePriceDataModel>(id.ToString());
	}

	public static GachaPlinkoPrizePriceDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPlinkoPrizePriceDataModel>(id);
	}

	public static List<GachaPlinkoPrizePriceDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<GachaPlinkoPrizePriceDataModel>();
	}
}
