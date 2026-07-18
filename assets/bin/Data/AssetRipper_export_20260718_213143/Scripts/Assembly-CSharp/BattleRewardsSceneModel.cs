public class BattleRewardsSceneModel : SceneModel
{
	public MatchData.Type matchType;

	public bool isPlayerWinner;

	public ServerTeamStatsState playerStats;

	public ServerTeamStatsState enemyStats;

	public OpponentData playerData;

	public OpponentData enemyData;

	public int deltaPlayerPVP;

	public BattleRewardsSceneModel(MatchData.Type matchType, bool isPlayerWinner, ServerTeamStatsState playerStats, ServerTeamStatsState enemyStats, OpponentData playerData, OpponentData enemyData, int deltaPlayerPVP)
	{
		this.matchType = matchType;
		this.isPlayerWinner = isPlayerWinner;
		this.playerStats = playerStats;
		this.enemyStats = enemyStats;
		this.playerData = playerData;
		this.enemyData = enemyData;
		this.deltaPlayerPVP = deltaPlayerPVP;
	}
}
