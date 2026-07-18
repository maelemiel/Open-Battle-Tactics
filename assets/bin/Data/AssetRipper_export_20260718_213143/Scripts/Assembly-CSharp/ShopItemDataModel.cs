public class ShopItemDataModel : IPurchasableDataModel
{
	private UserPriceDataModel _cachedPrice;

	public ShopItemDataModel(UserInventory.ItemType itemType, int amount)
	{
		_cachedPrice = new UserPriceDataModel(itemType, amount);
	}

	public UserPriceDataModel GetPrice()
	{
		return _cachedPrice;
	}
}
