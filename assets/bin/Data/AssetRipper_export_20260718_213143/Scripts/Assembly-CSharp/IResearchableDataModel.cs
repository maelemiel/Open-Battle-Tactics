public interface IResearchableDataModel
{
	long ResearchDuration { get; }

	UserPriceDataModel GetResearchCost(UserProfile userProfile);
}
