using System.Collections.Generic;

public class RewardLabelTypeCollection
{
	public List<ItemCollectionDataModel> itemCollections;

	public List<string> labelType;

	public int bonusEventPoints;

	public RewardLabelTypeCollection()
	{
		itemCollections = new List<ItemCollectionDataModel>();
		labelType = new List<string>();
		bonusEventPoints = 0;
	}
}
