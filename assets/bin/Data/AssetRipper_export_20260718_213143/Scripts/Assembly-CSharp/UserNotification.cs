using System;
using System.Collections.Generic;

public class UserNotification
{
	public class AbilityUnlocked : UserNotification
	{
		public AbilityDataModel ability;

		private GiftType giftType;

		public override object DataModel
		{
			get
			{
				return ability;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_ABILITY;
			}
		}

		public AbilityUnlocked(AbilityDataModel ability, GiftType giftType)
		{
			this.ability = ability;
			this.giftType = giftType;
		}

		public override void Execute()
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.OpenGiftNotification, new OpenGiftSceneModel(giftType, ability));
		}

		public override int OrderBy()
		{
			return 8;
		}
	}

	public class DivisionPromotion : UserNotification
	{
		public ProgressionDivisionDataModel division;

		public override object DataModel
		{
			get
			{
				return division;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_PROMOTION_EARNED;
			}
		}

		public DivisionPromotion(ProgressionDivisionDataModel division)
		{
			this.division = division;
		}

		public override void Execute()
		{
		}

		public override int OrderBy()
		{
			return 6;
		}
	}

	public class DivisionProgress : UserNotification
	{
		public DivisionProgressSceneModel sceneModel;

		public override object DataModel
		{
			get
			{
				return sceneModel;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_DIVISION_PROGRESS;
			}
		}

		public DivisionProgress(DivisionProgressSceneModel sceneModel)
		{
			this.sceneModel = sceneModel;
		}

		public override void Execute()
		{
		}

		public override int OrderBy()
		{
			return 3;
		}
	}

	public class PromotionSeriesEarned : UserNotification
	{
		public ProgressionPromotionSeriesDataModel promoSeries;

		public override object DataModel
		{
			get
			{
				return promoSeries;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_DIVISION_EARNED;
			}
		}

		public PromotionSeriesEarned(ProgressionPromotionSeriesDataModel promoSeries)
		{
			this.promoSeries = promoSeries;
		}

		public override void Execute()
		{
		}

		public override int OrderBy()
		{
			return 5;
		}
	}

	public class PromotionSeriesFailure : UserNotification
	{
		public ProgressionPromotionSeriesDataModel promoSeries;

		public DivisionProgressSceneModel sceneModel;

		public override object DataModel
		{
			get
			{
				return sceneModel;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_DIVISION_PROGRESS;
			}
		}

		public PromotionSeriesFailure(ProgressionPromotionSeriesDataModel promoSeries)
		{
			this.promoSeries = promoSeries;
		}

		public override void Execute()
		{
			sceneModel = new DivisionProgressSceneModel();
			sceneModel.previousPoints = promoSeries.CurrentDivision.totalPointToPromotionSeries;
			sceneModel.currentPoints = promoSeries.CurrentDivision.ResetPoints;
			sceneModel.divisionDataModel = promoSeries.CurrentDivision;
		}
	}

	public class PromoSeriesProgress : UserNotification
	{
		public PromotionSeriesProgressSceneModel sceneModel;

		public override object DataModel
		{
			get
			{
				return sceneModel;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_PROMOTION_PROGRESS;
			}
		}

		public PromoSeriesProgress(PromotionSeriesProgressSceneModel sceneModel)
		{
			this.sceneModel = sceneModel;
		}

		public override void Execute()
		{
		}

		public override int OrderBy()
		{
			return 4;
		}
	}

	public class PartsReceived : UserNotification
	{
		public BattleRewardsSceneModel rewards;

		public override object DataModel
		{
			get
			{
				return rewards;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_PARTS;
			}
		}

		public PartsReceived(BattleRewardsSceneModel rewards)
		{
			this.rewards = rewards;
		}

		public override void Execute()
		{
		}

		public override int OrderBy()
		{
			return 1;
		}
	}

	public class EventPointsReceived : UserNotification
	{
		public BattleRewardsSceneModel rewards;

		public EventDataModel.EventTypes type;

		public int boostPoints;

		public int killPoints;

		public List<string> toDisplay = new List<string>();

		public List<string> toDisplayTypes = new List<string>();

		public override object DataModel
		{
			get
			{
				return this;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_EVENT;
			}
		}

		public EventPointsReceived(BattleRewardsSceneModel battleRewards, int boostPoints, int killPoints, BattleController battleController, int extraEventPointsBoost)
		{
			string iconName = UserInventory.ItemType.EventPoint.GetIconName();
			string iconName2 = UserInventory.ItemType.Energy.GetIconName();
			string iconName3 = UserInventory.ItemType.RaidBossEventPoint.GetIconName();
			string iconName4 = UserInventory.ItemType.VictoryPoint.GetIconName();
			rewards = battleRewards;
			if (battleController == null)
			{
				type = EventDataModel.EventTypes.POINTS_EVENT;
			}
			else
			{
				type = battleController.SceneModel.activeEvent.EventType;
			}
			this.boostPoints = boostPoints;
			this.killPoints = killPoints;
			switch (battleController.SceneModel.activeEvent.EventType)
			{
			case EventDataModel.EventTypes.POINTS_EVENT:
			{
				int eventPointPVPBonus = UserProfile.player.CurrentDivision.EventPointPVPBonus;
				int winEventPoint = UserProfile.player.CurrentDivision.WinEventPoint;
				int eventPointsEarned = battleRewards.playerStats.eventPointsEarned;
				bool flag = battleRewards.enemyData.type == TeamType.Player;
				toDisplay.Add(string.Format("ui_postbattle_units_destroyed".Localize("Units Destroyed {0}"), killPoints));
				toDisplayTypes.Add(iconName);
				if (battleRewards.isPlayerWinner)
				{
					toDisplay.Add(string.Format("ui_postbattle_win_bonus".Localize("Win Bonus {0}"), winEventPoint));
					toDisplayTypes.Add(iconName);
				}
				if (flag)
				{
					toDisplay.Add(string.Format("ui_postbattle_pvp_bonus".Localize("PvP Bonus {0}"), eventPointPVPBonus));
					toDisplayTypes.Add(iconName);
				}
				toDisplay.Add(string.Format("ui_postbattle_event_unit_bonus".Localize("Event Unit Bonus {0}"), boostPoints));
				toDisplayTypes.Add(iconName);
				if (extraEventPointsBoost != 0)
				{
					toDisplay.Add(string.Format("ui_postbattle_multi_ticket_bonus".Localize("Multi-ticket Bonus {0}"), extraEventPointsBoost));
					toDisplayTypes.Add(iconName2);
				}
				if (UserProfile.player.userClub != null)
				{
					Reporting.LeaderboardContentUpdate(eventPointsEarned, eventPointsEarned + UserProfile.player.userClub.GetTotalEventClubPoints(), UserProfile.player.GetActiveEvent().id);
				}
				break;
			}
			case EventDataModel.EventTypes.WINS_EVENT:
			{
				int winEventPoint2 = UserProfile.player.CurrentDivision.WinEventPoint;
				if (battleRewards.isPlayerWinner)
				{
					toDisplay.Add(string.Format("ui_postbattle_win_bonus".Localize("Win Bonus {0}"), winEventPoint2));
					toDisplayTypes.Add(iconName);
					toDisplay.Add(string.Format("ui_postbattle_event_unit_bonus".Localize("Event Unit Bonus {0}"), boostPoints));
					toDisplayTypes.Add(iconName);
				}
				if (UserProfile.player.userClub != null)
				{
					Reporting.LeaderboardContentUpdate(winEventPoint2 + boostPoints, winEventPoint2 + boostPoints + UserProfile.player.userClub.GetTotalEventClubPoints(), UserProfile.player.GetActiveEvent().id);
				}
				break;
			}
			case EventDataModel.EventTypes.RAIDBOSS_EVENT:
			{
				int raidBossBuffDamageTotal = battleRewards.playerStats.raidBossBuffDamageTotal;
				int raidBossTicketBuffDamageTotal = battleRewards.playerStats.raidBossTicketBuffDamageTotal;
				int num5 = battleRewards.playerStats.raidBossDamageDealt - raidBossBuffDamageTotal - killPoints - raidBossTicketBuffDamageTotal;
				toDisplay.Add(string.Format("ui_postbattle_event_raid_boss_damage".Localize("Damage Dealt {0}"), num5));
				toDisplayTypes.Add(iconName3);
				toDisplay.Add(string.Format("ui_postbattle_event_raid_boss_unit_bonus".Localize("Event Unit Bonus {0}"), raidBossBuffDamageTotal));
				toDisplayTypes.Add(iconName3);
				toDisplay.Add(string.Format("ui_postbattle_event_raid_boss_ticket_bonus".Localize("Ticket Damage Bonus {0}"), raidBossTicketBuffDamageTotal));
				toDisplayTypes.Add(iconName3);
				toDisplay.Add(string.Format("ui_postbattle_event_raid_boss_destruction_bonus".Localize("Raid Boss Destruction Bonus {0}"), killPoints));
				toDisplayTypes.Add(iconName3);
				int num6 = 0;
				for (int i = 0; i < battleController.enemyTeam.units.Length; i++)
				{
					if (battleController.enemyTeam.units[i].metadata.UnitType == UnitType.RAID_BOSS)
					{
						num6 = i;
						break;
					}
				}
				if (UserProfile.player.userClub != null)
				{
					Reporting.LeaderboardContentUpdate(num5, num5 + UserProfile.player.userClub.GetTotalEventClubPoints(), UserProfile.player.GetActiveEvent().id);
					Reporting.RaidBossFight(int.Parse(battleController.MatchData.opponentTeam.id), battleController.MatchData.opponentTeam.raidbossId, battleController.playerTeam.stats.giftDrops, battleController.MatchData.opponentTeam.startingBossHealth, battleController.enemyTeam.units[num6].hp, battleController.playerTeam.units, UserProfile.player.currentTeamIndex, Kamcord.IsEnabled(), battleController.playerTeam.stats.revivesUsed, battleController.MatchData.matchId, battleRewards.playerStats.raidBossDamageDealt);
				}
				break;
			}
			case EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT:
			{
				int num = battleRewards.enemyData.pvpRating;
				if (num < 1000 && battleRewards.enemyData.type == TeamType.Bot)
				{
					num = 1000;
				}
				float num2 = (float)Math.Max(1, Constants.BattleMulitplierEnemyElo) / 100f;
				num = (int)Math.Floor((float)num * num2);
				int unitsDestroyed = battleRewards.playerStats.unitsDestroyed;
				int num3 = unitsDestroyed * num;
				int bonusVictoryPointsEarned = battleRewards.playerStats.bonusVictoryPointsEarned;
				num2 = (float)Math.Max(1, Constants.BattleMulitplierPlayerElo) / 100f;
				int num4 = (int)Math.Floor((float)battleRewards.playerData.pvpRating * num2);
				int victoryPointsEarned = battleRewards.playerStats.victoryPointsEarned;
				toDisplay.Add(string.Format("ui_postbattle_event_pvp_tournament_tanks_killed".Localize("Tanks Killed: {0}"), unitsDestroyed));
				toDisplayTypes.Add(iconName4);
				toDisplay.Add(string.Format("ui_postbattle_event_pvp_tournament_enemy_rating".Localize("Enemy Rating: {0}"), num));
				toDisplayTypes.Add(iconName4);
				toDisplay.Add(string.Format("ui_postbattle_event_pvp_tournament_victory_points".Localize("Victory Points: {0}"), num3));
				toDisplayTypes.Add(iconName4);
				if (battleRewards.isPlayerWinner)
				{
					toDisplay.Add(string.Format("ui_postbattle_event_pvp_tournament_win_bonus".Localize("Win Bonus: {0}"), num4));
					toDisplayTypes.Add(iconName4);
				}
				if (extraEventPointsBoost != 0)
				{
					toDisplay.Add(string.Format("ui_postbattle_multi_ticket_bonus".Localize("Multi-ticket Bonus {0}"), extraEventPointsBoost));
					toDisplayTypes.Add(iconName2);
				}
				if (UserProfile.player.userClub != null)
				{
					Reporting.LeaderboardContentUpdate(victoryPointsEarned, victoryPointsEarned + UserProfile.player.userClub.GetTotalEventClubPoints(), UserProfile.player.GetActiveEvent().id);
				}
				break;
			}
			}
		}

		public override void Execute()
		{
		}

		public override int OrderBy()
		{
			return 2;
		}
	}

	public class AbilityEarned : UserNotification
	{
		public AbilityDataModel reward;

		public override object DataModel
		{
			get
			{
				return reward;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_ABILITY;
			}
		}

		public AbilityEarned(AbilityDataModel reward)
		{
			this.reward = reward;
		}

		public override void Execute()
		{
		}

		public override int OrderBy()
		{
			return 9;
		}
	}

	public class ClubCrateSent : UserNotification
	{
		public ClubCrateSceneModel sceneModel;

		public override object DataModel
		{
			get
			{
				return sceneModel;
			}
		}

		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.NONE;
			}
		}

		public ClubCrateSent(ClubCrateSceneModel sceneModel)
		{
			this.sceneModel = sceneModel;
		}

		public override void Execute()
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.FoundClubCrate, sceneModel);
		}

		public override int OrderBy()
		{
			return 200;
		}
	}

	public class RewardsEnd : UserNotification
	{
		public override PostBattleRewardsStates PostBattleRewardsState
		{
			get
			{
				return PostBattleRewardsStates.REWARDS_END;
			}
		}

		public override int OrderBy()
		{
			return 100;
		}
	}

	private static Action notificationsCompleteCallback;

	public virtual object DataModel
	{
		get
		{
			return null;
		}
	}

	public virtual PostBattleRewardsStates PostBattleRewardsState
	{
		get
		{
			return PostBattleRewardsStates.NONE;
		}
	}

	public virtual void Execute()
	{
	}

	public static UserNotification ExecuteNotifications(Action completeCallback = null)
	{
		if (completeCallback != null)
		{
			notificationsCompleteCallback = completeCallback;
		}
		UserProfile player = UserProfile.player;
		UserNotification userNotification = null;
		player.notifications.Sort((UserNotification notifA, UserNotification notifB) => notifA.OrderBy().CompareTo(notifB.OrderBy()));
		if (player.notifications.Count > 0)
		{
			userNotification = player.notifications[0];
			player.notifications.RemoveAt(0);
			userNotification.Execute();
		}
		else if (notificationsCompleteCallback != null)
		{
			notificationsCompleteCallback();
			notificationsCompleteCallback = null;
		}
		else
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.Default);
		}
		return userNotification;
	}

	public virtual int OrderBy()
	{
		return 0;
	}
}
