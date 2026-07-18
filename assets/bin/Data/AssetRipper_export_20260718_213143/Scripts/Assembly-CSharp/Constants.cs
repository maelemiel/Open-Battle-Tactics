public static class Constants
{
	public static bool USE_DEFAULT_CONSTANTS;

	public static bool ShowUpgradeCost
	{
		get
		{
			return CacheManager.GetConstantInt("unit_upgrade_parts_cost_show", 1) == 1;
		}
	}

	public static string APAnchor1
	{
		get
		{
			return CacheManager.GetConstantString("ap_anchor_priority_1", "custom_1");
		}
	}

	public static string APAnchor2
	{
		get
		{
			return CacheManager.GetConstantString("ap_anchor_priority_2", "cannon");
		}
	}

	public static string ASAnchor1
	{
		get
		{
			return CacheManager.GetConstantString("as_anchor_priority_1", "custom_2");
		}
	}

	public static string ASAnchor2
	{
		get
		{
			return CacheManager.GetConstantString("as_anchor_priority_2", "cannon");
		}
	}

	public static int StartReplacingCratesCount
	{
		get
		{
			return CacheManager.GetConstantInt("gacha_box_replace_count", 6);
		}
	}

	public static bool UseOtherLevelsInterstitials
	{
		get
		{
			return CacheManager.GetConstantInt("enable_other_levels_interstitials", 1) == 1;
		}
	}

	public static bool UseOtherLevelsLogin
	{
		get
		{
			return CacheManager.GetConstantInt("enable_other_levels_login", 1) == 1;
		}
	}

	public static int BattleMulitplierEnemyElo
	{
		get
		{
			return CacheManager.GetConstantInt("battle_victory_enemy_elo", 100);
		}
	}

	public static int BattleMulitplierPlayerElo
	{
		get
		{
			return CacheManager.GetConstantInt("battle_victory_player_elo", 100);
		}
	}

	public static bool EnableFireWorks
	{
		get
		{
			return CacheManager.GetConstantInt("settings_fireworks?", 1) == 1;
		}
	}

	public static int PendingClubCratesCountRefresh
	{
		get
		{
			return CacheManager.GetConstantInt("topbar_pending_club_crates_count_refresh", 120);
		}
	}

	public static bool AddDamageBoostToArmourPiercing
	{
		get
		{
			return CacheManager.GetConstantInt("battle_add_damage_boost_to_ap", 1) == 1;
		}
	}

	public static bool DisableExtraAnalytics
	{
		get
		{
			return CacheManager.GetConstantInt("battle_disable_extra_analytics", 0) == 1;
		}
	}

	public static bool EloPointBoostToggle
	{
		get
		{
			return CacheManager.GetConstantInt("elo_points_boost_toggle", 0) == 1;
		}
	}

	public static double TierBonusBaseElo
	{
		get
		{
			return CacheManager.GetConstantInt("tier_bonus_base_elo", 1450);
		}
	}

	public static double TierBonusExp
	{
		get
		{
			return (double)CacheManager.GetConstantInt("tier_bonus_exp", 1250) / 1000.0;
		}
	}

	public static double TierBonusScale
	{
		get
		{
			return (double)CacheManager.GetConstantInt("tier_bonus_scale", 75) / 1000.0;
		}
	}

	public static int TierEventNewsLockOut
	{
		get
		{
			return CacheManager.GetConstantInt("tier_event_news_lockout", 1);
		}
	}

	public static int TopBarClubChatRefresh
	{
		get
		{
			return CacheManager.GetConstantInt("topbar_club_chat_refresh", 60);
		}
	}

	public static int PopupClubChatRefresh
	{
		get
		{
			return CacheManager.GetConstantInt("popup_club_chat_refresh", 20);
		}
	}

	public static int EnergyRechargeSeconds
	{
		get
		{
			return CacheManager.GetConstantInt("energy_recharge_seconds", 60);
		}
	}

	public static int MaxEnergy
	{
		get
		{
			return CacheManager.GetConstantInt("max_energy", 4);
		}
	}

	public static int PvPRoundTimer
	{
		get
		{
			return CacheManager.GetConstantInt("pvp_round_timer", 31);
		}
	}

	public static int BattleEnergyCost
	{
		get
		{
			return CacheManager.GetConstantInt("battle_energy_cost", 1);
		}
	}

	public static int EnergyRestoreCost
	{
		get
		{
			return CacheManager.GetConstantInt("energy_restore_cost", 30);
		}
	}

	public static int RaidBossRevivePriceA
	{
		get
		{
			return CacheManager.GetConstantInt("rb_revive_price_a", 300);
		}
	}

	public static int RaidBossRevivePriceB
	{
		get
		{
			return CacheManager.GetConstantInt("rb_revive_price_b", 301);
		}
	}

	public static int RaidBossReviveTimer
	{
		get
		{
			return CacheManager.GetConstantInt("rb_revive_popup_timer", 15);
		}
	}

	public static int MaxUnitInventorySize
	{
		get
		{
			return CacheManager.GetConstantInt("max_unit_num", 50);
		}
	}

	public static int MinUnitInventorySize
	{
		get
		{
			return CacheManager.GetConstantInt("max_unit_num", 4);
		}
	}

	public static int PlayerStartingCoins
	{
		get
		{
			return CacheManager.GetConstantInt("player_starting_coins", 250);
		}
	}

	public static int PlayerStartingScrap
	{
		get
		{
			return CacheManager.GetConstantInt("player_starting_scrap", 0);
		}
	}

	public static int PlayerStartingGift
	{
		get
		{
			return CacheManager.GetConstantInt("player_starting_gift", 4);
		}
	}

	public static int PromoSeriesSetback
	{
		get
		{
			return CacheManager.GetConstantInt("promo_series_setback", 75);
		}
	}

	public static UserInventory.ItemType RepairSkipItem
	{
		get
		{
			return (UserInventory.ItemType)CacheManager.GetConstantInt("repair_skip_item", 4);
		}
	}

	public static int RepairSkipCost
	{
		get
		{
			return CacheManager.GetConstantInt("repair_skip_cost", 1);
		}
	}

	public static int RepairSkipTimeUnit
	{
		get
		{
			return CacheManager.GetConstantInt("repair_skip_time_unit", 60);
		}
	}

	public static UserInventory.ItemType ResearchSkipItem
	{
		get
		{
			return (UserInventory.ItemType)CacheManager.GetConstantInt("research_skip_item", 4);
		}
	}

	public static int ResearchSkipCost
	{
		get
		{
			return CacheManager.GetConstantInt("research_skip_cost", 1);
		}
	}

	public static int ResearchSkipTimeUnit
	{
		get
		{
			return CacheManager.GetConstantInt("research_skip_time_unit", 60);
		}
	}

	public static int BattleHeartbeatInterval
	{
		get
		{
			return CacheManager.GetConstantInt("battle_heartbeat_interval", 10);
		}
	}

	public static int BattleOverkillDamage
	{
		get
		{
			return CacheManager.GetConstantInt("battle_overkill_threshold", 7);
		}
	}

	public static int UnitLoadPriorityTierMultiplier
	{
		get
		{
			return CacheManager.GetConstantInt("loading_unit_priority_tier_multiplier", 10);
		}
	}

	public static int UnitLoadPriorityEvoMultiplier
	{
		get
		{
			return CacheManager.GetConstantInt("loading_unit_priority_evo_multiplier", 500);
		}
	}

	public static int DialogTimeoutSeconds
	{
		get
		{
			return CacheManager.GetConstantInt("dialog_timeout_seconds", 10000);
		}
	}

	public static int MinUnitsPerTeam
	{
		get
		{
			return 4;
		}
	}

	public static int BattleReceiveActionsInterval
	{
		get
		{
			return CacheManager.GetConstantInt("battle_receiveactions_interval", 2000);
		}
	}

	public static int BattleCreateMatchInterval
	{
		get
		{
			return CacheManager.GetConstantInt("battle_creatematch_interval", 1000);
		}
	}

	public static int BattleCreateMatchBotFallbackTime
	{
		get
		{
			return CacheManager.GetConstantInt("battle_creatematch_bot_fallback_time", 30000);
		}
	}

	public static int BattleCreateRetryLimit
	{
		get
		{
			return CacheManager.GetConstantInt("pvp_matchmaking_retries", 10);
		}
	}

	public static int ResearchStartReward
	{
		get
		{
			return CacheManager.GetConstantInt("research_start_reward", 0);
		}
	}

	public static int ResearchClaimReward
	{
		get
		{
			return CacheManager.GetConstantInt("research_claim_reward", 0);
		}
	}

	public static int PromoSeriesStartReward
	{
		get
		{
			return CacheManager.GetConstantInt("promo_series_start_reward", 0);
		}
	}

	public static int PromoSeriesCompleteReward
	{
		get
		{
			return CacheManager.GetConstantInt("promo_series_complete_reward", 0);
		}
	}

	public static int ContractsPoolCount
	{
		get
		{
			return CacheManager.GetConstantInt("contracts_pool_count", 3);
		}
	}

	public static int CooldownGemToCashExchangeRate
	{
		get
		{
			return CacheManager.GetConstantInt("cooldown_gem_to_cash_exchange_rate", 360);
		}
	}

	public static int FirstTierToAskForRating
	{
		get
		{
			return CacheManager.GetConstantInt("first_tier_to_ask_for_rating", 6);
		}
	}

	public static int TierAskForRatingInterval
	{
		get
		{
			return CacheManager.GetConstantInt("tier_ask_for_rating_interval", 5);
		}
	}

	public static int RepeatTankBuildExponentialBase
	{
		get
		{
			return CacheManager.GetConstantInt("repeat_tank_build_exponential_base", 2);
		}
	}

	public static int ExclusiveTanksUnlockTier
	{
		get
		{
			return CacheManager.GetConstantInt("exclusive_tanks_unlock_tier", 6);
		}
	}

	public static int TellFriendDivisionId
	{
		get
		{
			return CacheManager.GetConstantInt("tell_friend_division_id", 2);
		}
	}

	public static int ClubsTotalMembers
	{
		get
		{
			return CacheManager.GetConstantInt("club_total_members", 4);
		}
	}

	public static int EventPointsKeptOnKick
	{
		get
		{
			return CacheManager.GetConstantInt("user_points_kept_kicked", 100);
		}
	}

	public static int EventPointsKeptOnLeave
	{
		get
		{
			return CacheManager.GetConstantInt("user_points_kept_left", 50);
		}
	}

	public static int EventPointsKeptOnLeaveEmptyClub
	{
		get
		{
			return CacheManager.GetConstantInt("user_points_kept_left_empty_club", 100);
		}
	}

	public static int SkinGemToCashConversion
	{
		get
		{
			return CacheManager.GetConstantInt("purchase_skin_gem_to_cash_exchange_rate", 200);
		}
	}

	public static int BigExplosionThreshold
	{
		get
		{
			return CacheManager.GetConstantInt("big_explosion_threshold", 12);
		}
	}

	public static int ChangeNamePriceId
	{
		get
		{
			return CacheManager.GetConstantInt("name_change_price_id", 102);
		}
	}

	public static int ChangeNameGemConversion
	{
		get
		{
			return CacheManager.GetConstantInt("name_change_gem_to_cash_exchange_rate", 200);
		}
	}

	public static float ReceiveActionsFatalTimeout
	{
		get
		{
			return CacheManager.GetConstantInt("receive_actions_fatal_timeout", 20);
		}
	}

	public static string ChatGodUsername
	{
		get
		{
			return CacheManager.GetConstantString("chat_god_username", "[Frank]");
		}
	}

	public static int FacebookConnectIncentive
	{
		get
		{
			return CacheManager.GetConstantInt("facebook_connect_incentivization", 2000);
		}
	}

	public static string FacebookFanPageId
	{
		get
		{
			return CacheManager.GetConstantString("facebook_fan_page_id", "315218065302094");
		}
	}

	public static int FacebookLikeReappearTime
	{
		get
		{
			return CacheManager.GetConstantInt("facebook_like_reappear_time", 48);
		}
	}

	public static AssetLinkageDataModel NoEventLogoAssetLinkage
	{
		get
		{
			return AssetLinkageDataModel.GetSingle(CacheManager.GetConstantString("no_event_logo_asset_id", "0"));
		}
	}

	public static int MaxAdsVideoPerDay
	{
		get
		{
			return CacheManager.GetConstantInt("max_ads_videos_per_day", 4);
		}
	}

	public static long VideoAdsDecreaseGachaTime
	{
		get
		{
			return TimeUtility.ConvertHoursToMiliseconds(CacheManager.GetConstantInt("video_ads_decrease_gacha_time", 4));
		}
	}

	public static AssetLinkageDataModel GachaBannerAssetLinkage
	{
		get
		{
			return AssetLinkageDataModel.GetSingle(CacheManager.GetConstantString("gacha_banner_asset_linkage_id", "23000"));
		}
	}

	public static int EventJoinClubPopUpTierRequirement
	{
		get
		{
			return CacheManager.GetConstantInt("event_join_club_popup_tier_requirement", 2);
		}
	}

	public static int LikeUsOnFBPopUpTierRequirement
	{
		get
		{
			return CacheManager.GetConstantInt("like_us_on_fb_popup_tier_requirement", 3);
		}
	}

	public static bool PvpRankingRewardsEnable
	{
		get
		{
			return CacheManager.GetConstantInt("pvp_ranking_rewards_enable", 1) == 1;
		}
	}

	public static float ShowNextJoinClubPeriod
	{
		get
		{
			return CacheManager.GetConstantFloat("show_next_join_club_period", 4f);
		}
	}

	public static int MinTierEventContent
	{
		get
		{
			return CacheManager.GetConstantInt("min_tier_event_content", 2);
		}
	}

	public static int MinTierEventTicketBoostPopUp
	{
		get
		{
			return CacheManager.GetConstantInt("min_tier_event_ticket_boost_popup", 2);
		}
	}

	public static int RaidBossMaxLevel
	{
		get
		{
			return CacheManager.GetConstantInt("raid_boss_max_level", 5);
		}
	}

	public static int MultiStrikeMaxMissileCount
	{
		get
		{
			return CacheManager.GetConstantInt("multi_strike_max_missile_count", 15);
		}
	}

	public static bool ShowSpecialHomeContent
	{
		get
		{
			return CacheManager.GetConstantString("show_special_home_content", "FALSE") == "TRUE";
		}
	}

	public static int MultiTeamReportBattleCount
	{
		get
		{
			return CacheManager.GetConstantInt("event_report_battle_count", 10);
		}
	}

	public static bool MultiTeamReportsEnabled
	{
		get
		{
			return CacheManager.GetConstantString("multi_team_reports_enabled", "FALSE") == "TRUE";
		}
	}

	public static int NewsRotationTime
	{
		get
		{
			return CacheManager.GetConstantInt("news_rotation_time", 5);
		}
	}

	public static int MultiTeamReportMinimumTier
	{
		get
		{
			return CacheManager.GetConstantInt("multi_team_reports_min_tier", 2);
		}
	}

	public static int EventReportLengthInSeconds
	{
		get
		{
			return CacheManager.GetConstantInt("event_report_time", 14400);
		}
	}

	public static string GetEventIntroMovieURL
	{
		get
		{
			return CacheManager.GetConstantString("event_intro_video_url", "http://d3dhq3qiq7lleq.cloudfront.net/EventIntroVideo.mp4");
		}
	}

	public static bool ShowBiggerBoostInNormalBattle
	{
		get
		{
			return CacheManager.GetConstantString("show_bigger_boost_in_normal_battle", "TRUE") == "TRUE";
		}
	}

	public static bool ShowBiggerBoostInPvpTournament
	{
		get
		{
			return CacheManager.GetConstantString("show_bigger_boost_in_pvp_tournament", "TRUE") == "TRUE";
		}
	}

	public static string TransformerURL
	{
		get
		{
			return CacheManager.GetConstantString("transformer_url", "http://www.transformersbattletactics.com/play?xpsbt");
		}
	}

	public static float OtherLevelsRefreshUserProfileSeconds
	{
		get
		{
			return CacheManager.GetConstantFloat("other_levels_refresh_userprofile_seconds", 5.5f);
		}
	}

	public static bool BattlePVPRefreshUIEnabled
	{
		get
		{
			return CacheManager.GetConstantString("battle_pvp_refreshui_enabled", "FALSE") == "TRUE";
		}
	}

	public static int MinTierForShowBoostSelection
	{
		get
		{
			return CacheManager.GetConstantInt("min_tier_for_show_boost_selection", 2);
		}
	}

	public static int MaxBostBattlesPerDay
	{
		get
		{
			return CacheManager.GetConstantInt("max_bost_battles_per_day", 10);
		}
	}

	public static int CashToGemRateMaxUpgrade
	{
		get
		{
			return CacheManager.GetConstantInt("cash_to_gem_tate_upgrade", 200);
		}
	}

	public static int BotBattleDailyCapRestoreCost
	{
		get
		{
			return CacheManager.GetConstantInt("bot_battle_daily_cap_restore_cost", 100);
		}
	}

	public static int MultiTeamPopUpUnit(int index)
	{
		return CacheManager.GetConstantInt("multi_team_popup_unit_" + index, 4301013);
	}

	public static int GetIntConstantWithID(string constantId, int defaultValue = 0)
	{
		return CacheManager.GetConstantInt(constantId, defaultValue);
	}
}
