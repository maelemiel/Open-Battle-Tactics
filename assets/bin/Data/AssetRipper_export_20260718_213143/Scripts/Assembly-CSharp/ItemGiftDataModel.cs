using System.Collections.Generic;

public class ItemGiftDataModel : BaseDataModel
{
	public int amount;

	public int giftId;

	public int itemId;

	public int itemType;

	public static ItemGiftDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ItemGiftDataModel>(id.ToString());
	}

	public static ItemGiftDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ItemGiftDataModel>(id);
	}

	public static List<ItemGiftDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<ItemGiftDataModel>();
	}

	public static ItemCollectionDataModel GetGiftPackage(int giftId)
	{
		List<ItemGiftDataModel> multiByQuery = NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<ItemGiftDataModel>(" WHERE gift_id = " + giftId);
		if (multiByQuery == null)
		{
			return null;
		}
		ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
		for (int i = 0; i < multiByQuery.Count; i++)
		{
			ItemGiftDataModel itemGiftDataModel = multiByQuery[i];
			itemCollectionDataModel.AddItem((UserInventory.ItemType)itemGiftDataModel.itemType, itemGiftDataModel.itemId, itemGiftDataModel.amount);
		}
		return itemCollectionDataModel;
	}
}
