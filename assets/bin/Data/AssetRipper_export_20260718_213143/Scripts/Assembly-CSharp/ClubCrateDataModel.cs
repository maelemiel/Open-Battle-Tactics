public class ClubCrateDataModel
{
	private ItemCollectionDataModel itemCollection;

	private string fromUser;

	public ItemCollectionDataModel ItemCollection
	{
		get
		{
			return itemCollection;
		}
	}

	public string FromUser
	{
		get
		{
			return fromUser;
		}
	}

	public ClubCrateDataModel(string fromUser, ItemCollectionDataModel itemCollection)
	{
		this.fromUser = fromUser;
		this.itemCollection = itemCollection;
	}
}
