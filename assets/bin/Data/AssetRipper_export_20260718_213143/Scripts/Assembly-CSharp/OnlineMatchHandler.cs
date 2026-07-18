using System;
using System.Collections;
using System.Collections.Generic;
using LitJson0;
using UnityEngine;

public class OnlineMatchHandler : BaseMatchHandler
{
	protected bool _isEventMatch;

	protected bool giveup;

	public override int RoundTimeLimit
	{
		get
		{
			return (battleController.MatchType == MatchData.Type.PVP && base.matchData.opponentTeam.type == TeamType.Player) ? Constants.PvPRoundTimer : 0;
		}
	}

	public override bool IsEventMatch
	{
		get
		{
			return _isEventMatch;
		}
	}

	public override bool IsEventPointsMatch
	{
		get
		{
			return _isEventMatch && battleController.SceneModel.activeEvent.EventType == EventDataModel.EventTypes.POINTS_EVENT;
		}
	}

	public override bool IsRaidBossEventActive
	{
		get
		{
			return _isEventMatch && battleController.SceneModel.activeEvent.EventType == EventDataModel.EventTypes.RAIDBOSS_EVENT;
		}
	}

	public override void Init(BattleController battleController, MatchManager matchManager)
	{
		base.Init(battleController, matchManager);
		base.matchData.playerTeam = OpponentDataFactory.FromUserProfile(UserProfile.player);
		_isEventMatch = battleController.SceneModel.activeEvent != null;
	}

	public override void CreateMatch(Action successCallback)
	{
		battleController.StartCoroutine(CreateMatchSequence(successCallback));
	}

	private IEnumerator CreateMatchSequence(Action successCallback)
	{
		string matchID = null;
		giveup = false;
		long currentTime = TimeManager.ServerTime;
		int numTries = 0;
		bool searchingUIVisible = false;
		Action successCallback2 = default(Action);
		while (true)
		{
			bool received = false;
			bool success = false;
			int numAborts = 0;
			string firstMatchID = null;
			int difficulty = battleController.SceneModel.difficulty;
			string botPartId = battleController.SceneModel.botPartId;
			MatchData.Type matchType = battleController.SceneModel.matchType;
			string raidBossId = battleController.SceneModel.raidbossId;
			Singleton<SessionManager>.instance.CreateMatch(matchType, giveup, matchID, difficulty, botPartId, raidBossId, delegate(CreateMatchResponse response)
			{
				received = true;
				matchID = response.matchID;
				if (matchID != null && firstMatchID == null)
				{
					firstMatchID = matchID;
				}
				if (response.abortSequence)
				{
					string matchID2 = response.matchID;
					matchID = null;
					giveup = true;
					numAborts++;
					if (numAborts >= 3)
					{
						Reporting.MatchmakingFailureEvent(firstMatchID, matchID2);
						PopupManager.ShowPopup(PopupDataModel.NetworkError(QuitUtility.Restart));
					}
					else
					{
						Reporting.MatchmakingAbortEvent(firstMatchID, matchID2);
					}
				}
				else if (response.success)
				{
					success = true;
					HandleCreateMatchSuccess(response);
					if (battleController.hud != null)
					{
						battleController.hud.HideSearchingUI();
					}
					successCallback2();
				}
			});
			if (numAborts >= 3)
			{
				yield return new WaitForSeconds(1000000f);
			}
			while (!received)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (success)
			{
				break;
			}
			yield return new WaitForSeconds((float)Constants.BattleCreateMatchInterval / 1000f);
			numTries++;
			bool battlePVPRefreshEnabled = Constants.BattlePVPRefreshUIEnabled && (battleController.SceneModel.activeEvent == null || battleController.SceneModel.activeEvent.EventType != EventDataModel.EventTypes.RAIDBOSS_EVENT);
			if (battlePVPRefreshEnabled && numTries > Constants.BattleCreateRetryLimit && !giveup && !searchingUIVisible)
			{
				currentTime = TimeManager.ServerTime - Constants.BattleCreateMatchInterval * Constants.BattleCreateRetryLimit;
				battleController.hud.ShowSearchingUI();
				searchingUIVisible = true;
			}
			if (numTries > Constants.BattleCreateRetryLimit && TimeManager.ServerTime - currentTime > Constants.BattleCreateMatchBotFallbackTime)
			{
				if (!giveup && battlePVPRefreshEnabled)
				{
					battleController.hud.HideSearchingUI();
					searchingUIVisible = false;
					battleController.hud.StartCoroutine(battleController.hud.battleText.ShowMessageSequence("ui_battle_notfoundliveopponent".Localize("LIVE PLAYER NOT AVAILABLE"), 0.6f));
				}
				giveup = true;
			}
		}
	}

	private void HandleCreateMatchSuccess(CreateMatchResponse response)
	{
		matchManager.MatchData = response.matchData;
		battleController.StartCoroutine(HeartbeatSequence());
		CrittercismUtil.LeaveBreadcrumb("MatchStart ID=" + base.matchData.matchId);
		CrittercismUtil.LeaveBreadcrumb("OpponentType " + base.matchData.opponentTeam.type);
		CrittercismUtil.LeaveBreadcrumb("OpponentID " + base.matchData.opponentTeam.id);
	}

	public override void SendPlayerActions(Action successCallback)
	{
		Log.DebugTag("SubmitActions Count: " + battleController.matchManager.playerActions.Count, null, "OnlineMatchHandler");
		Singleton<SessionManager>.instance.SendActions(base.matchData.matchId, roundId, battleController.matchManager.playerActions, delegate(TypedRestResponse<JsonObject> response)
		{
			JsonObject jsonObject = response.Resource();
			if (jsonObject.Contains("error"))
			{
				PopupManager.ShowPopup(PopupDataModel.TimeOutForfeit(delegate
				{
					QuitUtility.Restart();
				}));
			}
			else
			{
				successCallback();
			}
		});
	}

	public override void ReceiveOpponentActions(Action successCallback)
	{
		if (base.matchData.opponentTeam.ai != null)
		{
			matchManager.enemyActions = base.matchData.opponentTeam.ai.Think(battleController.enemyTeam);
			roundId++;
			successCallback();
		}
		else
		{
			battleController.StartCoroutine(ReceiveActionsSequence(successCallback));
		}
	}

	private IEnumerator ReceiveActionsSequence(Action successCallback)
	{
		Action successCallback2 = default(Action);
		while (true)
		{
			bool received = false;
			bool success = false;
			Singleton<SessionManager>.instance.ReceiveActions(base.matchData.matchId, roundId, battleController.enemyTeam, delegate(ServerUtilities.ReceiveActionsResponse response)
			{
				received = true;
				if (response.success)
				{
					success = true;
					matchManager.enemyActions = response.actions;
					roundId++;
					successCallback2();
				}
			});
			while (!received)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (success)
			{
				break;
			}
			yield return new WaitForSeconds((float)Constants.BattleReceiveActionsInterval / 1000f);
		}
	}

	public override void MatchComplete(ServerTeamStatsState playerStats, Action successCallback)
	{
		CrittercismUtil.LeaveBreadcrumb("MatchComplete Win=" + playerStats.isWinner + " Promo=" + UserProfile.player.IsInPromoSeries);
		BattleBoosts.ApplyPlayerBoosts(base.matchData.playerTeam.boosts, playerStats);
		int num = 0;
		BoostDataModel activeBoost = null;
		if (base.matchData.playerTeam.boosts.Count > 0)
		{
			foreach (int boost in base.matchData.playerTeam.boosts)
			{
				BoostDataModel single = BoostDataModel.GetSingle(boost);
				num = playerStats.pointsEarned - (int)Math.Ceiling((double)playerStats.pointsEarned / (1.0 + (double)single.TierMultiplier));
				if (num > 0)
				{
					activeBoost = single;
				}
			}
		}
		int extraEventPointsBoost = 0;
		if (base.matchData.playerTeam.boosts.Count > 0)
		{
			foreach (int boost2 in base.matchData.playerTeam.boosts)
			{
				BoostDataModel single2 = BoostDataModel.GetSingle(boost2);
				if (battleController.SceneModel.activeEvent != null)
				{
					switch (battleController.SceneModel.activeEvent.EventType)
					{
					case EventDataModel.EventTypes.POINTS_EVENT:
						extraEventPointsBoost = playerStats.eventPointsEarned - (int)Math.Ceiling((double)playerStats.eventPointsEarned / (1.0 + (double)single2.Multiplier1));
						break;
					case EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT:
						extraEventPointsBoost = playerStats.victoryPointsEarned - (int)Math.Ceiling((double)playerStats.victoryPointsEarned / (1.0 + (double)single2.Multiplier1));
						break;
					}
				}
			}
		}
		if (Constants.EloPointBoostToggle)
		{
			playerStats.pointsEarned += (int)CalculateEloBoost(UserProfile.player.pvpRating, Constants.TierBonusBaseElo, Constants.TierBonusExp, Constants.TierBonusScale);
		}
		UserProfile.player.ApplyStreakLogic(playerStats.isWinner);
		if (battleController.SceneModel.activeEvent != null && UserProfile.player.divisionInt >= Constants.MinTierEventContent)
		{
			TeamState playerTeam = battleController.playerTeam;
			TeamState enemyTeam = battleController.enemyTeam;
			int num2 = 0;
			int num3 = 0;
			UnitState[] units = enemyTeam.units;
			foreach (UnitState unitState in units)
			{
				if (unitState.IsDead)
				{
					num2 += unitState.extraDestroyEventPoints;
					if (battleController.SceneModel.activeEvent.EventType != EventDataModel.EventTypes.RAIDBOSS_EVENT || unitState.metadata.UnitType == UnitType.RAID_BOSS)
					{
						num3 += unitState.metadata.DestroyEventPoints;
					}
				}
			}
			BattleRewardsSceneModel battleRewards = new BattleRewardsSceneModel(battleController.MatchType, playerTeam.IsBattleWinner, playerTeam.stats, enemyTeam.stats, battleController.MatchData.playerTeam, battleController.MatchData.opponentTeam, 0);
			if (battleController.SceneModel.activeEvent.EventType != EventDataModel.EventTypes.RAIDBOSS_EVENT)
			{
				UserProfile.player.notifications.Add(new UserNotification.EventPointsReceived(battleRewards, num2, num3, battleController, extraEventPointsBoost));
			}
		}
		if (UserProfile.player.IsInPromoSeries)
		{
			if (battleController.MatchType == MatchData.Type.PVP)
			{
				PromotionSeriesProgressSceneModel promotionSeriesProgressSceneModel = new PromotionSeriesProgressSceneModel();
				promotionSeriesProgressSceneModel.didWin = playerStats.isWinner;
				promotionSeriesProgressSceneModel.previousWins = UserProfile.player.promoSeriesWins;
				promotionSeriesProgressSceneModel.previousLosses = UserProfile.player.promoSeriesLosses;
				promotionSeriesProgressSceneModel.promotionSeriesDataModel = UserProfile.player.PromoSeries;
				UserProfile.player.notifications.Add(new UserNotification.PromoSeriesProgress(promotionSeriesProgressSceneModel));
			}
		}
		else
		{
			DivisionProgressSceneModel divisionProgressSceneModel = new DivisionProgressSceneModel();
			divisionProgressSceneModel.previousPoints = UserProfile.player.points;
			divisionProgressSceneModel.currentPoints = UserProfile.player.points + playerStats.pointsEarned;
			divisionProgressSceneModel.pointsBoost = num;
			divisionProgressSceneModel.activeBoost = activeBoost;
			divisionProgressSceneModel.divisionDataModel = UserProfile.player.CurrentDivision;
			divisionProgressSceneModel.didPlayerWinBattle = playerStats.isWinner;
			string id = UserProfile.player.CurrentDivision.promotionSeriesId.ToString();
			ProgressionPromotionSeriesDataModel single3 = NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionPromotionSeriesDataModel>(id);
			divisionProgressSceneModel.didEarnEnergy = single3 != null && UserProfile.player.promoSeriesLastId != single3.id;
			UserProfile.player.notifications.Add(new UserNotification.DivisionProgress(divisionProgressSceneModel));
		}
		UserProfile.player.notifications.Add(new UserNotification.RewardsEnd());
		Singleton<SessionManager>.instance.RecordBattleResult(base.matchData, battleController.playerTeam, delegate(TypedRestResponse<JsonObject> response)
		{
			JsonObject jsonObject = response.Resource().GetObject("result");
			if (jsonObject.Contains("crateRecipient"))
			{
				ClubCrateSceneModel sceneModel = new ClubCrateSceneModel
				{
					recepientUsername = jsonObject.GetString("crateRecipient")
				};
				UserProfile.player.notifications.Add(new UserNotification.ClubCrateSent(sceneModel));
			}
			successCallback();
		});
		SocialLeaderboard.ReportTotalWins(UserProfile.player.wins);
		SocialLeaderboard.ReportTotalKills(UserProfile.player.unitsKilled);
	}

	public static double CalculateEloBoost(double playerElo, double baseElo, double exponenent, double scale)
	{
		return Math.Max(Math.Floor(Math.Pow(Math.Max(0.0, playerElo - baseElo), exponenent)), 0.0) * scale;
	}

	public override void GotoPostBattleScene()
	{
		TeamState playerTeam = battleController.playerTeam;
		TeamState enemyTeam = battleController.enemyTeam;
		UserProfile player = UserProfile.player;
		int num = ((!playerTeam.IsBattleWinner) ? 1 : (-1));
		int num2 = player.pvpRating - base.matchData.playerTeam.pvpRating;
		battleController.MatchData.opponentTeam.pvpRating -= num2;
		battleController.MatchData.opponentTeam.winStreak += num;
		OpponentData opponentData = new OpponentData();
		opponentData.name = player.nickname;
		opponentData.division = player.CurrentDivision;
		opponentData.winStreak = player.winStreak;
		opponentData.pvpRating = player.pvpRating;
		opponentData.boosts = new List<int>();
		for (int i = 0; i < playerTeam.boosts.Length; i++)
		{
			opponentData.boosts.Add(Convert.ToInt32(playerTeam.boosts[i].id));
		}
		BattleRewardsSceneModel sceneDM = new BattleRewardsSceneModel(battleController.MatchType, playerTeam.IsBattleWinner, playerTeam.stats, enemyTeam.stats, opponentData, battleController.MatchData.opponentTeam, num2);
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.PostBattleRewardsScene, sceneDM, false);
	}

	private IEnumerator HeartbeatSequence()
	{
		while (true)
		{
			if (base.matchData.opponentTeam.type == TeamType.Player)
			{
				Singleton<SessionManager>.instance.BattleHeartbeat(base.matchData.matchId);
			}
			yield return new WaitForSeconds(Constants.BattleHeartbeatInterval);
		}
	}

	public override void GiveUpPVPSearch()
	{
		giveup = true;
	}
}
