public interface IUnitMetadata
{
	string ID { get; }

	int Rarity { get; }

	int StartingHealth { get; }

	DieFaceType[] RollTypes { get; }

	int[] RollValues { get; }

	int AssetBundleID { get; }

	UnitType UnitType { get; }

	EventUnitBoostDataModel UnitBoost { get; }

	int GemDropMin { get; }

	int GemDropMax { get; }

	int GemDropChance { get; }

	int DestroyPoints { get; }

	int DestroyCash { get; }

	int SurvivePoints { get; }

	int SurviveCash { get; }

	int DestroyEventPoints { get; }

	IPartMetadata[] PartDrops { get; }

	int GetAbilitiesCount();

	IAbilityMetadata GetAbilityMetaData(int index);

	int GetAbilityBoostValueA(int index);

	int GetAbilityBoostValueB(int index);
}
