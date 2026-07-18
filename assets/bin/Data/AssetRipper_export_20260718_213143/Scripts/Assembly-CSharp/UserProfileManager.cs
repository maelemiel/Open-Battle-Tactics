using System;
using System.Collections;
using System.Collections.Generic;
using LitJson0;
using UnityEngine;

public class UserProfileManager : Singleton<UserProfileManager>
{
	private class CacheEntry
	{
		public UserProfile userProfile;
	}

	public delegate void UpdatedCallback(UserProfile newUserProfile);

	private const string KEY_LAST_SHOW_JOIN_CLUB = "lastShowJoinClub";

	private UserProfile _userProfile;

	private static KeyValueStorage kvs;

	private YieldInstruction energyUpdateInterval = new WaitForSeconds(0.1f);

	public static KeyValueStorage Kvs
	{
		get
		{
			if (kvs == null)
			{
				kvs = KeyValueStorage.Instance(KeyValueStorage.Storage.USER_PROFILE);
			}
			return kvs;
		}
	}

	public UserProfile UserProfile
	{
		get
		{
			return _userProfile;
		}
		set
		{
			_userProfile = value;
		}
	}

	public event UpdatedCallback Updated;

	private void OnUpdated()
	{
		if (this.Updated != null)
		{
			this.Updated(UserProfile);
		}
	}

	public IEnumerator WaitForUserProfile()
	{
		Log.Debug("UserProfileManager.WaitForUserProfile");
		while (UserProfile == null)
		{
			yield return 0;
		}
	}

	private void Awake()
	{
		Log.Debug("UserProfileManager.Awake");
		StartCoroutine(EnergyRechargerCoroutine());
	}

	public void SyncCheck(UserProfile serverProfile)
	{
		if (serverProfile.coins != UserProfile.coins)
		{
			Log.Error(string.Format("Coins out of sync. Local:{0} Server:{1}", UserProfile.coins, serverProfile.coins));
		}
	}

	private IEnumerator EnergyRechargerCoroutine()
	{
		while (true)
		{
			yield return energyUpdateInterval;
			if (UserProfile != null)
			{
				UserProfile.energy = UserProfile.energy;
			}
		}
	}

	public static void ParseFacebookData(JsonObject json, UserProfile profile)
	{
		JsonObject jsonObject = json.GetObject("facebook");
		if (jsonObject != null)
		{
			profile.FacebookData.Token = jsonObject.GetString("token");
			profile.FacebookData.Name = jsonObject.GetString("name");
		}
	}

	public static UserProfile ParseUserProfile(JsonObject json, UserProfile profile = null)
	{
		Log.DebugTag("ParseUserProfile" + json.ToJson(), null, "UserProfileManager");
		if (profile == null)
		{
			profile = new UserProfile();
			if (kvs == null)
			{
				kvs = KeyValueStorage.Instance(KeyValueStorage.Storage.USER_PROFILE);
			}
			if (kvs.ContainsKey("lastShowJoinClub"))
			{
				profile.joinClubLastShow = Convert.ToInt64(kvs.GetValue<string>("lastShowJoinClub"));
			}
		}
		profile.jsonSource = json;
		profile.id = json.GetString("_id");
		profile.username = json.GetString("username");
		if (json.Contains("last_login"))
		{
			profile.lastLogin = json.GetLong("last_login");
		}
		if (json.Contains("version"))
		{
			profile.version = json.GetLong("version");
		}
		if (json.Contains("pvp_rating"))
		{
			profile.pvpRating = json.GetInt("pvp_rating");
		}
		if (json.Contains("locale"))
		{
			profile.locale = json.GetString("locale");
		}
		if (json.Contains("last_club_msg"))
		{
			profile.LastClubMsg = json.GetInt("last_club_msg");
		}
		profile.activeLeaderboards = new List<string>();
		if (json.Contains("active_leaderboards"))
		{
			foreach (object item2 in json.GetList("active_leaderboards"))
			{
				profile.activeLeaderboards.Add((string)item2);
			}
		}
		JsonObject jsonObject = json.GetObject("mobage");
		if (jsonObject != null)
		{
			profile.nickname = jsonObject.GetString("nickname");
			long num = jsonObject.GetLong("id");
			Log.DebugTag("MobageId = " + num, null, "Default");
			profile.mobageId = num;
			profile.thumbnail = jsonObject.GetString("thumbnail");
		}
		ParseFacebookData(json, profile);
		if (json.Contains("ability_sets"))
		{
			List<JsonObject> objectList = json.GetObjectList("ability_sets");
			profile.userAbilitySets = new List<UserAbilitySet>();
			int num2 = 0;
			foreach (JsonObject item3 in objectList)
			{
				if (!item3.Contains("abilities"))
				{
					continue;
				}
				List<string> list = new List<string>();
				foreach (object item4 in item3.GetList("abilities"))
				{
					list.Add((string)item4);
				}
				profile.userAbilitySets.Add(new UserAbilitySet(list, num2));
				num2++;
			}
		}
		if (json.Contains("next_ai_armies"))
		{
			profile.nextAIArmies = new List<string>();
			foreach (object item5 in json.GetList("next_ai_armies"))
			{
				profile.nextAIArmies.Add((string)item5);
			}
		}
		JsonObject jsonObject2 = json.GetObject("team_details");
		if (jsonObject2 != null)
		{
			profile.currentTeamIndex = jsonObject2.GetInt("current_team_index");
			if (profile.unitInventory != null)
			{
				profile.teams = new List<UserTeam>();
				int num3 = 0;
				foreach (JsonObject @object in jsonObject2.GetObjectList("teams"))
				{
					UserTeam userTeam = new UserTeam();
					userTeam.index = num3;
					userTeam.units = new List<UserUnit>();
					num3++;
					userTeam.cooldownFinishTime = @object.GetLongOrDefault("cooldown_finish_time", 0L);
					foreach (string item6 in @object.GetList("units"))
					{
						if (item6 == null)
						{
							userTeam.units.Add(null);
						}
						else if (profile.unitInventory.ContainsKey(item6))
						{
							userTeam.units.Add(profile.unitInventory[item6]);
						}
						else
						{
							Debug.LogWarning("UserProfile specified a unit in their team that does not exist. unitID=" + item6);
						}
					}
					profile.teams.Add(userTeam);
				}
				UserTeam userTeam2 = ((profile.teams.Count <= 0) ? new UserTeam() : profile.teams[0]);
				if (profile.teams.Count < 3)
				{
					UserTeam userTeam3 = null;
					for (int i = profile.teams.Count; i < 3; i++)
					{
						userTeam3 = new UserTeam();
						userTeam3.units = new List<UserUnit>(userTeam2.units);
						profile.teams.Add(userTeam3);
					}
				}
			}
		}
		JsonObject jsonObject3 = json.GetObject("inventory");
		if (jsonObject3 != null)
		{
			profile.coins = jsonObject3.GetInt("coins");
			if (jsonObject3.Contains("parts"))
			{
				profile.parts = new Dictionary<string, int>();
				foreach (KeyValuePair<string, object> item7 in jsonObject3.GetObject("parts").Dictionary)
				{
					profile.inventory.SetParts(item7.Key, Convert.ToInt32(item7.Value));
				}
			}
			if (jsonObject3.Contains("abilities"))
			{
				profile.abilitiesInventory = new List<string>();
				foreach (object item8 in jsonObject3.GetList("abilities"))
				{
					profile.abilitiesInventory.Add((string)item8);
				}
			}
			IList list2 = jsonObject3.GetList("skins");
			if (list2 != null)
			{
				profile.purchasedSkinIds = new List<string>();
				foreach (object item9 in list2)
				{
					profile.purchasedSkinIds.Add(item9.ToString());
				}
			}
		}
		JsonObject jsonObject4 = json.GetObject("stats");
		if (jsonObject4 != null)
		{
			JsonObject jsonObject5 = jsonObject4.GetObject("division");
			JsonObject jsonObject6 = jsonObject4.GetObject("promo");
			profile.points = jsonObject5.GetInt("points");
			profile.divisionId = jsonObject5.GetString("division_id");
			profile.promoSeriesId = jsonObject6.GetString("series_id");
			profile.promoSeriesWins = jsonObject6.GetInt("series_wins");
			profile.promoSeriesLosses = jsonObject6.GetInt("series_losses");
			profile.promoSeriesLastId = jsonObject6.GetString("series_last_id");
			if (jsonObject5.Contains("divisions_with_rewards_claimed"))
			{
				profile.divisionsWithRewardsClaimed = new List<int>();
				foreach (object item10 in jsonObject5.GetList("divisions_with_rewards_claimed"))
				{
					profile.divisionsWithRewardsClaimed.Add(Convert.ToInt32(item10));
				}
			}
			if (jsonObject4.Contains("video_ads"))
			{
				JsonObject jsonObject7 = jsonObject4.GetObject("video_ads");
				profile.videoAdsCount = jsonObject7.GetInt("daily_count");
				profile.videoAdsLastShow = jsonObject7.GetLong("last_show");
			}
			if (jsonObject4.Contains("bot_battle"))
			{
				JsonObject jsonObject8 = jsonObject4.GetObject("bot_battle");
				profile.botBattleCount = jsonObject8.GetInt("daily_count");
				profile.botBattleLast = jsonObject8.GetLong("last");
				profile.botBattleRestoreCount = jsonObject8.GetInt("daily_restore_count");
			}
			profile.unitsKilled = jsonObject4.GetInt("destroyed_units");
			profile.totalBattles = jsonObject4.GetIntOrDefault("total_battles", 0);
			profile.wins = jsonObject4.GetInt("wins");
			profile.losses = jsonObject4.GetInt("losses");
			profile.winStreak = jsonObject4.GetInt("win_streak");
			profile.stadiumIndex = jsonObject4.GetIntOrDefault("stadium_index", 0);
			List<JsonObject> objectList2 = jsonObject4.GetObjectList("unit_stats");
			if (objectList2 != null)
			{
				int num4 = 0;
				foreach (JsonObject item11 in objectList2)
				{
					profile.unitStats.Add(UserUnitStats.FromJSON(item11));
					num4++;
				}
			}
		}
		if (json.Contains("dialogs_triggered"))
		{
			foreach (object item12 in json.GetList("dialogs_triggered"))
			{
				profile.dialogTriggers.SetDialogTriggered(Convert.ToInt32(item12), false);
			}
		}
		JsonObject jsonObject9 = json.GetObject("pvp_stats");
		if (jsonObject9 != null)
		{
			profile.winStreak = jsonObject9.GetInt("win_streak");
			profile.pvpRating = jsonObject9.GetInt("pvp_rating");
			profile.divisionId = jsonObject9.GetString("division_id");
		}
		JsonObject jsonObject10 = json.GetObject("flags");
		if (jsonObject10 != null)
		{
			profile.claimedFirstUnit = jsonObject10.GetBooleanOrDefault("starter_unit_claimed", false);
		}
		JsonObject jsonObject11 = json.GetObject("energy");
		if (jsonObject11 != null)
		{
			if (jsonObject11.Contains("energy_recovery_time") && jsonObject11.GetLong("energy_recovery_time") >= 0)
			{
				profile.energyRecoveryTime = jsonObject11.GetLong("energy_recovery_time");
			}
			if (jsonObject11.Contains("energy_overcharge") && jsonObject11.GetInt("energy_overcharge") >= 0)
			{
				profile.energyOvercharge = jsonObject11.GetInt("energy_overcharge");
			}
		}
		List<JsonObject> objectList3 = json.GetObjectList("researchers");
		if (objectList3 != null)
		{
			int num5 = 0;
			foreach (JsonObject item13 in objectList3)
			{
				UserResearcher userResearcher = new UserResearcher();
				userResearcher.itemID = item13.GetString("item_id");
				userResearcher.researchType = (UserResearcher.ResearchType)(int)Enum.Parse(typeof(UserResearcher.ResearchType), item13.GetString("research_type"));
				userResearcher.startTime = item13.GetLong("start_time");
				userResearcher.finishTime = item13.GetLong("finish_time");
				profile.researchers[num5] = userResearcher;
				num5++;
				if (num5 >= profile.researchers.Count)
				{
					break;
				}
			}
		}
		JsonObject jsonObject12 = json.GetObject("contract");
		if (jsonObject12 != null)
		{
			UserContract userContract = new UserContract();
			userContract.contractID = jsonObject12.GetInt("contract_id");
			userContract.startTime = jsonObject12.GetLong("start_time");
			userContract.finishTime = jsonObject12.GetLong("finish_time");
			if (json.Contains("random_seed_contracts") && json.GetLong("random_seed_contracts") >= 0)
			{
				profile.random_seed_contracts = json.GetLong("random_seed_contracts");
			}
			profile.contract = userContract;
		}
		if (json.Contains("next_contracts"))
		{
			profile.nextContracts = new List<int>();
			foreach (object item14 in json.GetList("next_contracts"))
			{
				profile.nextContracts.Add(int.Parse(item14.ToString()));
			}
		}
		JsonObject jsonObject13 = json.GetObject("tutorial");
		if (jsonObject13 != null)
		{
			profile.tutorial.IsComplete = jsonObject13.GetBoolean("is_complete");
			profile.tutorial.CurrentStep = (TutorialStep)jsonObject13.GetInt("current_step");
		}
		if (json.Contains("prize_gacha"))
		{
			profile.userGachaPrizes = new List<UserGachaPrize>();
			List<JsonObject> objectList4 = json.GetObjectList("prize_gacha");
			string text2 = "-1";
			int num6 = -1;
			long num7 = -1L;
			int num8 = -1;
			long num9 = -1L;
			foreach (JsonObject item15 in objectList4)
			{
				text2 = item15.GetString("_id");
				num6 = item15.GetInt("prize_gacha_id");
				num7 = item15.GetLong("last_claimed_time");
				num8 = item15.GetInt("gacha_type");
				num9 = item15.GetLong("achieved_division_at");
				profile.userGachaPrizes.Add(new UserGachaPrize(text2, num6, num7, num9, num8));
			}
		}
		if (json.Contains("boosts"))
		{
			profile.boosts = new List<UserBoost>();
			foreach (JsonObject object2 in json.GetObjectList("boosts"))
			{
				int metaID = object2.GetInt("meta_id");
				long expireTime = object2.GetLong("expire_time");
				UserBoost item = new UserBoost(metaID, expireTime);
				profile.boosts.Add(item);
			}
		}
		if (json.Contains("club_id"))
		{
			string text3 = json.GetString("club_id");
			if (!string.IsNullOrEmpty(text3))
			{
				profile.clubID = text3;
			}
		}
		if (json.Contains("last_claimed_report"))
		{
			JsonObject jsonMultiTeamReport = json.GetObject("last_claimed_report");
			profile.lastClaimedReport = ClaimedReportData.FromJSON(jsonMultiTeamReport);
		}
		if (json.Contains("is_admin"))
		{
			bool boolean = json.GetBoolean("is_admin");
			profile.isAdmin = boolean;
		}
		return profile;
	}

	public void UpdateUserProfile(JsonObject newUserProfileJson, Action cb)
	{
		Log.DebugTag("UpdateUserProfile", null, "UserProfileManager");
		CacheUserProfile(newUserProfileJson);
		UserProfile newUserProfile = ParseUserProfile(newUserProfileJson, _userProfile);
		UpdateUserProfile(newUserProfile);
		cb();
	}

	public void UpdateUserProfileServerData(JsonObject newUserProfileJson, UserProfile userProfileToUpdate)
	{
		Log.DebugTag("UpdateUserProfileServerData" + newUserProfileJson.ToJson(), null, "UserProfileManager");
		if (newUserProfileJson.Contains("next_ai_armies"))
		{
			userProfileToUpdate.nextAIArmies = new List<string>();
			foreach (object item2 in newUserProfileJson.GetList("next_ai_armies"))
			{
				userProfileToUpdate.nextAIArmies.Add((string)item2);
			}
		}
		if (newUserProfileJson.Contains("username"))
		{
			userProfileToUpdate.username = newUserProfileJson.GetString("username");
		}
		if (newUserProfileJson.Contains("pvp_rating"))
		{
			userProfileToUpdate.pvpRating = newUserProfileJson.GetInt("pvp_rating");
		}
		userProfileToUpdate.activeLeaderboards = new List<string>();
		if (newUserProfileJson.Contains("active_leaderboards"))
		{
			foreach (object item3 in newUserProfileJson.GetList("active_leaderboards"))
			{
				userProfileToUpdate.activeLeaderboards.Add((string)item3);
			}
		}
		if (newUserProfileJson.Contains("prize_gacha"))
		{
			userProfileToUpdate.userGachaPrizes = new List<UserGachaPrize>();
			string text = "-1";
			int num = -1;
			long num2 = -1L;
			int num3 = -1;
			long num4 = -1L;
			foreach (JsonObject @object in newUserProfileJson.GetObjectList("prize_gacha"))
			{
				text = @object.GetString("_id");
				num = @object.GetInt("prize_gacha_id");
				num2 = @object.GetLong("last_claimed_time");
				num3 = @object.GetInt("gacha_type");
				num4 = @object.GetLong("achieved_division_at");
				userProfileToUpdate.userGachaPrizes.Add(new UserGachaPrize(text, num, num2, num4, num3));
			}
		}
		if (newUserProfileJson.Contains("boosts"))
		{
			userProfileToUpdate.boosts = new List<UserBoost>();
			foreach (JsonObject object2 in newUserProfileJson.GetObjectList("boosts"))
			{
				int metaID = object2.GetInt("meta_id");
				long expireTime = object2.GetLong("expire_time");
				UserBoost item = new UserBoost(metaID, expireTime);
				userProfileToUpdate.boosts.Add(item);
			}
		}
		JsonObject jsonObject = newUserProfileJson.GetObject("inventory");
		if (jsonObject != null)
		{
			userProfileToUpdate.coins = jsonObject.GetInt("coins");
			if (jsonObject.Contains("parts"))
			{
				userProfileToUpdate.parts = new Dictionary<string, int>();
				foreach (KeyValuePair<string, object> item4 in jsonObject.GetObject("parts").Dictionary)
				{
					userProfileToUpdate.inventory.SetParts(item4.Key, Convert.ToInt32(item4.Value));
				}
			}
			if (jsonObject.Contains("abilities"))
			{
				userProfileToUpdate.abilitiesInventory = new List<string>();
				foreach (object item5 in jsonObject.GetList("abilities"))
				{
					userProfileToUpdate.abilitiesInventory.Add((string)item5);
				}
			}
			IList list = jsonObject.GetList("skins");
			if (list != null)
			{
				userProfileToUpdate.purchasedSkinIds = new List<string>();
				foreach (object item6 in list)
				{
					userProfileToUpdate.purchasedSkinIds.Add(item6.ToString());
				}
			}
		}
		JsonObject jsonObject2 = newUserProfileJson.GetObject("energy");
		if (jsonObject2 != null)
		{
			if (jsonObject2.Contains("energy_recovery_time") && jsonObject2.GetLong("energy_recovery_time") >= 0)
			{
				userProfileToUpdate.energyRecoveryTime = jsonObject2.GetLong("energy_recovery_time");
			}
			if (jsonObject2.Contains("energy_overcharge") && jsonObject2.GetInt("energy_overcharge") >= 0)
			{
				userProfileToUpdate.energyOvercharge = jsonObject2.GetInt("energy_overcharge");
			}
		}
		JsonObject jsonObject3 = newUserProfileJson.GetObject("stats");
		if (jsonObject3 != null && jsonObject3.Contains("bot_battle"))
		{
			JsonObject jsonObject4 = jsonObject3.GetObject("bot_battle");
			userProfileToUpdate.botBattleCount = jsonObject4.GetInt("daily_count");
			userProfileToUpdate.botBattleLast = jsonObject4.GetLong("last");
			userProfileToUpdate.botBattleRestoreCount = jsonObject4.GetInt("daily_restore_count");
		}
		ParseFacebookData(newUserProfileJson, userProfileToUpdate);
	}

	private void UpdateUserProfile(UserProfile newUserProfile)
	{
		_userProfile = newUserProfile;
		OnUpdated();
	}

	private CacheEntry FromCachedJson(JsonObject common, JsonObject stats)
	{
		if (common == null)
		{
			return null;
		}
		CacheEntry cacheEntry = new CacheEntry();
		UserProfile userProfile = new UserProfile();
		userProfile.id = common.GetString("_id");
		userProfile.nickname = common.GetString("nickname");
		userProfile.version = common.GetLong("version");
		cacheEntry.userProfile = userProfile;
		return cacheEntry;
	}

	public void CacheUserProfile()
	{
		CacheUserProfile(_userProfile.jsonSource);
	}

	private void CacheUserProfile(JsonObject json)
	{
	}

	public bool CanShowJoinClub()
	{
		if (TimeUtility.ServerTs > UserProfile.joinClubLastShow + TimeUtility.ConvertHoursToMiliseconds(Constants.ShowNextJoinClubPeriod))
		{
			return true;
		}
		return false;
	}

	public void SaveShowJoinClub()
	{
		UserProfile.joinClubLastShow = TimeUtility.ServerTs;
		kvs.SetValue("lastShowJoinClub", TimeUtility.ServerTs.ToString());
	}

	public void ShowJoinClubNotification()
	{
		if (Singleton<UserProfileManager>.instance.CanShowJoinClub() && !UserProfile.player.IsClubMember)
		{
			SaveShowJoinClub();
			PopupManager.ShowPopup(PopupDataModel.Full("ui_leaderboards_joinaclubpopup_title".Localize("Join a club?"), "ui_leaderboards_joinaclubpopup_desc".Localize("You aren't in a club yet, go join one?"), "ui_popup_later".Localize("Later"), null, "ui_popup_OK".Localize("OK"), delegate
			{
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ClubScene);
			}));
		}
	}

	public bool IsFirstTimeInScene(SceneTransitionManager.Scene scene)
	{
		int value = kvs.GetValue<int>("SCENE_SHOW_COUNT_" + scene);
		bool result = false;
		if (value == 0)
		{
			result = true;
		}
		kvs.SetValue("SCENE_SHOW_COUNT_" + scene, ++value);
		return result;
	}
}
