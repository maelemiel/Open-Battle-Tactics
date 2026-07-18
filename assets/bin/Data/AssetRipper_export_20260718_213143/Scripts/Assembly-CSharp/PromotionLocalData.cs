public class PromotionLocalData
{
	public int assetBundleID;

	public int health;

	public int level;

	public int partialLevel;

	public int[] dieValues;

	public DieFaceType[] dieFaceTypes;

	public ItemCollectionDataModel promotionCost;

	public UnitLevelProgressionDataModel unitLevelProgressionDataModel;

	public PromotionLocalData(int assetBundleID, int health, int level, int partialLevel, int[] dieValues, DieFaceType[] dieFaceTypes, ItemCollectionDataModel promotionCost, UnitLevelProgressionDataModel abilityMetadata)
	{
		this.assetBundleID = assetBundleID;
		this.health = health;
		this.level = level;
		this.partialLevel = partialLevel;
		this.dieValues = new int[dieValues.Length];
		for (int i = 0; i < dieValues.Length; i++)
		{
			this.dieValues[i] = dieValues[i];
		}
		this.dieFaceTypes = new DieFaceType[dieFaceTypes.Length];
		for (int j = 0; j < dieValues.Length; j++)
		{
			this.dieFaceTypes[j] = dieFaceTypes[j];
		}
		this.promotionCost = promotionCost;
		unitLevelProgressionDataModel = abilityMetadata;
	}
}
