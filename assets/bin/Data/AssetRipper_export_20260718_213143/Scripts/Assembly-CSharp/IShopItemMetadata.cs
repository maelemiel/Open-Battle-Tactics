public interface IShopItemMetadata
{
	string ID { get; }

	string Name { get; }

	string Description { get; }

	IPurchasableDataModel Cost { get; }

	int AssetBundleID { get; }

	string IconName { get; }
}
