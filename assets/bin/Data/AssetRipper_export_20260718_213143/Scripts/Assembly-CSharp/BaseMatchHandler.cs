using System;

public abstract class BaseMatchHandler
{
	protected BattleController battleController;

	protected MatchManager matchManager;

	public int roundId;

	protected MatchData matchData
	{
		get
		{
			return matchManager.MatchData;
		}
		set
		{
			matchManager.MatchData = value;
		}
	}

	public virtual int RoundTimeLimit
	{
		get
		{
			return 0;
		}
	}

	public virtual bool AllowEmoticons
	{
		get
		{
			return true;
		}
	}

	public virtual bool IsEventMatch
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsEventPointsMatch
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsRaidBossEventActive
	{
		get
		{
			return false;
		}
	}

	public MatchData MatchData
	{
		get
		{
			return matchData;
		}
	}

	public virtual void Init(BattleController battleController, MatchManager matchManager)
	{
		this.battleController = battleController;
		this.matchManager = matchManager;
	}

	public abstract void CreateMatch(Action successCallback);

	public abstract void SendPlayerActions(Action successCallback);

	public abstract void ReceiveOpponentActions(Action successCallback);

	public abstract void MatchComplete(ServerTeamStatsState playerStats, Action successCallback);

	public virtual OpponentData GetPlayerTeam()
	{
		return matchData.playerTeam;
	}

	public virtual OpponentData GetOpponentTeam()
	{
		return matchData.opponentTeam;
	}

	public abstract void GotoPostBattleScene();

	public virtual void GiveUpPVPSearch()
	{
	}
}
