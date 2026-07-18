using System.Collections.Generic;

public class LeaderboardRewardsDataModel : BaseDataModel
{
	public int giftPackageId;

	public int rankEnd;

	public int rankStart;

	public int rewardsId;

	public static LeaderboardRewardsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LeaderboardRewardsDataModel>(id.ToString());
	}

	public static LeaderboardRewardsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LeaderboardRewardsDataModel>(id);
	}

	public static List<LeaderboardRewardsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<LeaderboardRewardsDataModel>();
	}
}
