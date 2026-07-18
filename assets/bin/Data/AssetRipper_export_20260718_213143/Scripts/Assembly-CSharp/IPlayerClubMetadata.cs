public interface IPlayerClubMetadata
{
	UserClub Club { get; set; }

	string ID { get; }

	string Name { get; }

	int PVPRating { get; }

	string ThumbnailURL { get; }

	ProgressionDivisionDataModel Division { get; }

	AssetLinkageDataModel TierAssetLinkage { get; }
}
