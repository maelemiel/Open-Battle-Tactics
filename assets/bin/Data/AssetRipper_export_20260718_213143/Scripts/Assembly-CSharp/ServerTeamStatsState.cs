using System.Collections.Generic;

public class ServerTeamStatsState
{
	public bool isWinner;

	public int pointsEarned;

	public int eventPointsEarned;

	public int victoryPointsEarned;

	public int bonusVictoryPointsEarned;

	public int raidBossDamageDealt;

	public int raidBossBuffDamageTotal;

	public int raidBossTicketBuffDamageTotal;

	public int unitsDestroyed;

	public int unitsSurvived;

	public int gemsEarned;

	public int baseCoins;

	public int coinsFromUnitsDestroyed;

	public int coinsFromUnitsSurvived;

	public int coinsFromMultiKill;

	public int coinsFromBestRolls;

	public int coinsFromPerfectKills;

	public int coinsFromOverKills;

	public int coinsFromPrizePool;

	public int coinsFromBoost;

	public int totalDamageDealt;

	public int energySpent;

	public int initiativeWins;

	public int revivesUsed;

	public List<IPartMetadata> partsEarned;

	public List<int> giftDrops;

	public List<string> generatedRandomNumbersData = new List<string>();

	public int coinsEarned
	{
		get
		{
			return baseCoins + coinsFromUnitsDestroyed + coinsFromUnitsSurvived + coinsFromMultiKill + coinsFromBestRolls + coinsFromPerfectKills + coinsFromOverKills + coinsFromPrizePool + coinsFromBoost;
		}
	}

	internal ServerTeamStatsState Clone()
	{
		ServerTeamStatsState serverTeamStatsState = new ServerTeamStatsState();
		serverTeamStatsState.isWinner = isWinner;
		serverTeamStatsState.pointsEarned = pointsEarned;
		serverTeamStatsState.eventPointsEarned = eventPointsEarned;
		serverTeamStatsState.raidBossDamageDealt = raidBossDamageDealt;
		serverTeamStatsState.raidBossBuffDamageTotal = raidBossBuffDamageTotal;
		serverTeamStatsState.raidBossTicketBuffDamageTotal = raidBossTicketBuffDamageTotal;
		serverTeamStatsState.unitsDestroyed = unitsDestroyed;
		serverTeamStatsState.unitsSurvived = unitsSurvived;
		serverTeamStatsState.baseCoins = baseCoins;
		serverTeamStatsState.coinsFromUnitsDestroyed = coinsFromUnitsDestroyed;
		serverTeamStatsState.coinsFromUnitsSurvived = coinsFromUnitsSurvived;
		serverTeamStatsState.coinsFromMultiKill = coinsFromMultiKill;
		serverTeamStatsState.coinsFromBestRolls = coinsFromBestRolls;
		serverTeamStatsState.coinsFromPerfectKills = coinsFromPerfectKills;
		serverTeamStatsState.coinsFromOverKills = coinsFromOverKills;
		serverTeamStatsState.coinsFromPrizePool = coinsFromPrizePool;
		serverTeamStatsState.coinsFromBoost = coinsFromBoost;
		serverTeamStatsState.gemsEarned = gemsEarned;
		serverTeamStatsState.totalDamageDealt = totalDamageDealt;
		serverTeamStatsState.energySpent = energySpent;
		serverTeamStatsState.initiativeWins = initiativeWins;
		serverTeamStatsState.revivesUsed = revivesUsed;
		serverTeamStatsState.victoryPointsEarned = victoryPointsEarned;
		serverTeamStatsState.bonusVictoryPointsEarned = bonusVictoryPointsEarned;
		serverTeamStatsState.partsEarned = new List<IPartMetadata>();
		foreach (IPartMetadata item in partsEarned)
		{
			serverTeamStatsState.partsEarned.Add(item);
		}
		serverTeamStatsState.giftDrops = new List<int>();
		if (giftDrops != null)
		{
			foreach (int giftDrop in giftDrops)
			{
				serverTeamStatsState.giftDrops.Add(giftDrop);
			}
		}
		return serverTeamStatsState;
	}
}
