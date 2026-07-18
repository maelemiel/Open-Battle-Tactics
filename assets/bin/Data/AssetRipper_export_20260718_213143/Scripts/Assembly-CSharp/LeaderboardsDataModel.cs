using System;
using System.Collections.Generic;

public class LeaderboardsDataModel : BaseDataModel
{
	public int localIndex = -1;

	private static string[] leaderboardTitles = new string[3]
	{
		"ui_leaderboardTitles_contender".Localize("CONTENDER"),
		"ui_leaderboardTitles_semiPro".Localize("SEMI-PRO"),
		"ui_leaderboardTitles_elite".Localize("ELITE")
	};

	private static string[] leaderboardTitleImages = new string[3] { "Leaderboard_Contender", "Leaderboard_SemiPro", "Leaderboard_Elite" };

	public string dateEnd;

	public string dateStart;

	public int groupId;

	public int rewardsId;

	public int tierEnd;

	public int tierStart;

	public string Title
	{
		get
		{
			int num = Math.Min(GetLeaderboardsLocalIndex(), leaderboardTitles.Length - 1);
			return leaderboardTitles[num];
		}
	}

	public string TitleImage
	{
		get
		{
			int num = Math.Min(GetLeaderboardsLocalIndex(), leaderboardTitles.Length - 1);
			return leaderboardTitleImages[num];
		}
	}

	public static LeaderboardsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LeaderboardsDataModel>(id.ToString());
	}

	public static LeaderboardsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LeaderboardsDataModel>(id);
	}

	public static List<LeaderboardsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<LeaderboardsDataModel>();
	}

	public List<LeaderboardRewardsDataModel> GetRewards()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<LeaderboardRewardsDataModel>("WHERE rewards_id = " + rewardsId);
	}

	public int GetLeaderboardsLocalIndex()
	{
		if (localIndex >= 0)
		{
			return localIndex;
		}
		List<LeaderboardsDataModel> multiByQuery = NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<LeaderboardsDataModel>("WHERE group_id = " + groupId);
		int num = int.MaxValue;
		for (int i = 0; i < multiByQuery.Count; i++)
		{
			num = Math.Min(num, int.Parse(multiByQuery[i].id));
		}
		localIndex = int.Parse(id) - num;
		return localIndex;
	}
}
