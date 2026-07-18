using System;
using System.Collections;
using System.Collections.Generic;

public class AutoBotBattleMatchHandler : OnlineMatchHandler
{
	public override void MatchComplete(ServerTeamStatsState playerStats, Action successCallback)
	{
		battleController.StartCoroutine(MatchCompleteCoroutine(playerStats, successCallback));
	}

	private IEnumerator MatchCompleteCoroutine(ServerTeamStatsState playerStats, Action successCallback)
	{
		long battleStart = DateTime.UtcNow.Ticks;
		while (!battleController.battleState.IsComplete)
		{
			BattleLogic.BeginRound(battleController.battleState);
			yield return battleController.StartCoroutine(ApplyPlayerRoundActions());
			if (!battleController.battleState.teamOne.forfeited)
			{
				yield return battleController.StartCoroutine(ApplyOpponentRoundActions());
			}
			BattleLogic.SimulateRound(battleController.battleState);
		}
		Log.Debug("Final Battle Random seed: " + battleController.battleState.randomSeed);
		Log.Debug("Battle took    : " + (double)(DateTime.UtcNow.Ticks - battleStart) / 10000.0 + "ms");
		Log.Debug("Total Rounds:" + battleController.battleState.currentRound);
		base.MatchComplete(battleController.playerTeam.stats, successCallback);
	}

	protected IEnumerator ApplyPlayerRoundActions()
	{
		bool finishPlayerActions = false;
		SendPlayerActions(delegate
		{
			MatchData.RoundActions roundActions = new MatchData.RoundActions
			{
				userID = base.matchData.playerTeam.id,
				round = battleController.battleState.currentRound,
				actions = matchManager.playerActions
			};
			base.matchData.actions.Add(roundActions);
			foreach (BattleAction action in roundActions.actions)
			{
				BattleLogic.ApplyBattleAction(battleController.playerTeam, action);
			}
			finishPlayerActions = true;
		});
		while (!finishPlayerActions)
		{
			yield return 0;
		}
		yield return 1;
	}

	protected IEnumerator ApplyOpponentRoundActions()
	{
		bool finishOpponentActions = false;
		ReceiveOpponentActions(delegate
		{
			foreach (BattleAction action in new MatchData.RoundActions
			{
				userID = base.matchData.opponentTeam.id,
				round = battleController.battleState.currentRound,
				actions = matchManager.enemyActions
			}.actions)
			{
				BattleLogic.ApplyBattleAction(battleController.enemyTeam, action);
			}
			finishOpponentActions = true;
		});
		while (!finishOpponentActions)
		{
			yield return 0;
		}
		yield return 1;
	}

	public override void SendPlayerActions(Action successCallback)
	{
		matchManager.playerActions.Clear();
		if (base.matchData.playerTeam.ai != null)
		{
			matchManager.playerActions = base.matchData.playerTeam.ai.Think(battleController.playerTeam);
			base.SendPlayerActions(successCallback);
		}
		else
		{
			Log.Error("AutoBotBattleMatchHandler. SendPlayerActions. The ai of player is not defined");
		}
	}

	public override void ReceiveOpponentActions(Action successCallback)
	{
		Log.Warning("ReceiveOpponentActions");
		matchManager.enemyActions.Clear();
		matchManager.enemyActions = new List<BattleAction>();
		ForfeitAction item = new ForfeitAction();
		matchManager.enemyActions.Add(item);
		successCallback();
	}
}
