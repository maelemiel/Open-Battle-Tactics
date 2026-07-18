using System.Collections.Generic;

public class MatchData
{
	public enum Type
	{
		AI = 0,
		PVP = 1,
		TUTORIAL = 2,
		RAIDBOSS = 3,
		TEST = 4,
		AUTO_BOT_BATTLE = 5
	}

	public class RoundActions
	{
		public List<BattleAction> actions;

		public int round;

		public string userID;
	}

	public Type type;

	public OpponentData opponentTeam;

	public OpponentData playerTeam;

	public bool playerIsHost;

	public string matchId;

	public int battleSeed;

	public List<RoundActions> actions;

	public List<BattleAction> GetRoundActions(int round, string playerID)
	{
		if (actions == null)
		{
			return null;
		}
		foreach (RoundActions action in actions)
		{
			if (action.round == round && action.userID == playerID)
			{
				return action.actions;
			}
		}
		return null;
	}
}
