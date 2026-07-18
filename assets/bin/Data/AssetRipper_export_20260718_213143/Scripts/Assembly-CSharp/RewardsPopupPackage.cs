public class RewardsPopupPackage
{
	public enum Type
	{
		PvpAllTimeLeaderboard = 1,
		EventSoloLeaderboard = 2,
		EventClubLeaderboard = 3
	}

	public LeaderboardRewardsSceneModel rewardsModel;

	public Type type;

	public RewardsPopupPackage(LeaderboardRewardsSceneModel rewardsModel, Type typeReward)
	{
		this.rewardsModel = rewardsModel;
		type = typeReward;
	}
}
