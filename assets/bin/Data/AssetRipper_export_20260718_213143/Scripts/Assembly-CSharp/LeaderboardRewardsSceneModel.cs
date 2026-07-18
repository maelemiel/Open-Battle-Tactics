public class LeaderboardRewardsSceneModel
{
	public ItemCollectionDataModel leaderboardRewards;

	public int rank;

	public int points;

	public SessionManager.LeaderboardRewardResponse rewardResponse;

	public LeaderboardRewardsSceneModel(ItemCollectionDataModel leaderboardRewards, int rank, int points, SessionManager.LeaderboardRewardResponse rewardResponse)
	{
		this.leaderboardRewards = leaderboardRewards;
		this.rank = rank;
		this.rewardResponse = rewardResponse;
		this.points = points;
	}
}
