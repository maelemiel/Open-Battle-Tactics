public interface IPrizeGachaMetadata
{
	string ID { get; }

	string Name { get; }

	string Description { get; }

	float FreeCooldownTime { get; }

	AssetLinkageDataModel AssetLinkage { get; }
}
