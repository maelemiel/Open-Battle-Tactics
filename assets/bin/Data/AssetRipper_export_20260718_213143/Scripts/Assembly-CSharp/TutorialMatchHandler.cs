using System;

internal class TutorialMatchHandler : BaseMatchHandler
{
	private TutorialHooks tutorialHandler
	{
		get
		{
			return battleController.BattleHooks as TutorialHooks;
		}
	}

	public override bool AllowEmoticons
	{
		get
		{
			return false;
		}
	}

	public override void Init(BattleController battleController, MatchManager matchManager)
	{
		base.Init(battleController, matchManager);
		battleController.BattleHooks = UserProfile.player.tutorial.GetTutorialHandler();
		base.matchData.playerTeam = tutorialHandler.GetPlayerTeam();
		base.matchData.opponentTeam = tutorialHandler.GetOpponentTeam();
		base.matchData.battleSeed = 328372;
		base.matchData.playerTeam.randomSeed = 48792;
		base.matchData.opponentTeam.randomSeed = 85547;
	}

	public override OpponentData GetPlayerTeam()
	{
		return tutorialHandler.GetPlayerTeam();
	}

	public override OpponentData GetOpponentTeam()
	{
		base.matchData.opponentTeam = tutorialHandler.GetOpponentTeam();
		return base.matchData.opponentTeam;
	}

	public override void CreateMatch(Action successCallback)
	{
		base.matchData.opponentTeam = GetOpponentTeam();
		successCallback();
	}

	public override void SendPlayerActions(Action successCallback)
	{
		successCallback();
	}

	public override void ReceiveOpponentActions(Action successCallback)
	{
		if (base.matchData.opponentTeam.ai != null)
		{
			matchManager.enemyActions = base.matchData.opponentTeam.ai.Think(battleController.enemyTeam);
		}
		successCallback();
	}

	public override void MatchComplete(ServerTeamStatsState playerStats, Action successCallback)
	{
		successCallback();
	}

	public override void GotoPostBattleScene()
	{
		tutorialHandler.GotoPostBattleScene();
	}
}
