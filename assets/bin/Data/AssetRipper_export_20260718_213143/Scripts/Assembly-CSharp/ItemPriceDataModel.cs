using System.Collections.Generic;

public class ItemPriceDataModel : BaseDataModel
{
	public int amount;

	public int itemId;

	public int itemType;

	public int priceId;

	public static ItemPriceDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ItemPriceDataModel>(id.ToString());
	}

	public static ItemPriceDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ItemPriceDataModel>(id);
	}

	public static List<ItemPriceDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<ItemPriceDataModel>();
	}

	public static UserPriceDataModel GetPriceForID(int priceID)
	{
		UserPriceDataModel userPriceDataModel = new UserPriceDataModel();
		List<ItemPriceDataModel> all = GetAll();
		if (all != null && all.Count > 0)
		{
			IEnumerable<ItemPriceDataModel> enumerable = all.FindAll((ItemPriceDataModel x) => x.priceId == priceID);
			foreach (ItemPriceDataModel item in enumerable)
			{
				userPriceDataModel.AddItem((UserInventory.ItemType)item.itemType, item.itemId, item.amount);
			}
		}
		return userPriceDataModel;
	}

	public static List<ItemPriceDataModel> GetPartPrices(string partId)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<ItemPriceDataModel>(" WHERE item_type = 6 AND item_id = " + partId);
	}
}
