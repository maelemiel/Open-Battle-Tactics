public class UserGiftDataModel : ItemCollectionDataModel
{
	public UserGiftDataModel(UserInventory.ItemType itemType, int itemId, int amount)
	{
		AddItem(itemType, itemId, amount);
	}

	public UserGiftDataModel(UserInventory.ItemType itemType, int amount)
	{
		AddItem(itemType, 0, amount);
	}
}
