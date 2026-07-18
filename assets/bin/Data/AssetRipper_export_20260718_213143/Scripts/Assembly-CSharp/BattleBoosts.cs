using System;
using System.Collections.Generic;
using System.Linq;

public class BattleBoosts
{
	public static void ApplyPlayerBoosts(List<int> boosts, ServerTeamStatsState stats)
	{
		List<int> list = boosts.Distinct().ToList();
		if (list.Count <= 0)
		{
			return;
		}
		foreach (int item in list)
		{
			BoostDataModel single = BoostDataModel.GetSingle(item);
			stats.pointsEarned = (int)Math.Floor((float)stats.pointsEarned * single.TierMultiplier);
			stats.coinsFromBoost = (int)Math.Floor((float)stats.coinsEarned * (single.CashMultiplier - 1f));
			if (stats.eventPointsEarned != 0)
			{
				stats.eventPointsEarned = (int)Math.Floor((float)stats.eventPointsEarned * single.Multiplier1);
			}
			if (stats.victoryPointsEarned != 0)
			{
				stats.victoryPointsEarned = (int)Math.Floor((float)stats.victoryPointsEarned * single.Multiplier1);
			}
		}
	}
}
