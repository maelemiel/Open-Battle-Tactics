using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson0;
using UnityEngine;

public class Reporting
{
	private const float REPORT_SEND_FREQUENCY = 5f;

	private static long _gameStartTimestamp = 0L;

	private static long _sessionSerialID = 0L;

	public static bool enableStartupFunnel = true;

	public static bool enableBattleAnalytics = true;

	private static Queue<JsonObject> _reportsToSend = new Queue<JsonObject>();

	private static int _manageSquadTeamsPreModCurrentTeam;

	private static string _manageSquadTeamsPreMod;

	private static string _initialAbilityIDs;

	private static string UserID()
	{
		if (UserProfile.player != null)
		{
			return UserProfile.player.id;
		}
		return string.Empty;
	}

	private static string UserPriceDataModelToString(UserPriceDataModel price)
	{
		List<string> list = new List<string>();
		foreach (ItemCollectionDataModel.Item item in price.items)
		{
			list.Add((int)item.itemType + "," + item.amount.ToString());
		}
		return string.Join("|", list.ToArray());
	}

	private string UnitRollsAsDelimitedString(ServerUnitState[] units)
	{
		return string.Join("|", units.Select((ServerUnitState x) => x.currentRoll.ToString()).ToArray());
	}

	private static int PlayerSquadID()
	{
		return UserProfile.player.CurrentTeam.index;
	}

	private static string AbilitiesUsedAsDelimitedString(List<BattleAction> battleActions)
	{
		List<string> list = new List<string>();
		foreach (BattleAction battleAction in battleActions)
		{
			UseAbilityAction useAbilityAction = battleAction as UseAbilityAction;
			if (useAbilityAction != null)
			{
				list.Add(useAbilityAction.abilityID.ToString());
			}
		}
		return string.Join("|", list.ToArray());
	}

	private static JsonObject GetPlayerState()
	{
		JsonObject jsonObject = new JsonObject();
		UserProfile player = UserProfile.player;
		jsonObject["coins"] = player.coins;
		jsonObject["diamonds"] = player.gems;
		jsonObject["energy"] = player.energy;
		jsonObject["tier"] = player.divisionId;
		return jsonObject;
	}

	private static bool UsePlayerState()
	{
		UserProfile player = UserProfile.player;
		if (player != null && player.energy >= 0)
		{
			return true;
		}
		return false;
	}

	private static string UnitIDsAsDelimitedString(ServerUnitState[] units)
	{
		return string.Join("|", units.Select((ServerUnitState x) => x.metadata.ID.ToString()).ToArray());
	}

	private static string AbilityIDsAsDelimitedString(List<string> abilities)
	{
		return string.Join("|", abilities.ToArray());
	}

	private static long GetPerSessionSerialNumber()
	{
		return _sessionSerialID++;
	}

	private static void Report(string evcl, string evid, JsonObject payload)
	{
		if (!payload.Contains("timestamp"))
		{
			payload["timestamp"] = TimeManager.ServerTime;
		}
		if (!payload.Contains("user_id"))
		{
			payload["user_id"] = UserID();
		}
		payload["session_serial"] = GetPerSessionSerialNumber();
		JsonObject jsonObject = new JsonObject();
		jsonObject["evcl"] = evcl;
		jsonObject["evid"] = evid;
		jsonObject["evpl"] = payload;
		if (UsePlayerState())
		{
			jsonObject["plst"] = GetPlayerState();
		}
		string stackTrace = jsonObject.ToJson();
		PostOrCacheAnalyticsReports(jsonObject);
		if (Singleton<TestConsole>.IsInstantiated())
		{
			Singleton<TestConsole>.instance.HandleLogWithType("AnalyticsEvent\n", stackTrace, LogTypeExtended.Analytics);
		}
	}

	private static void PostOrCacheAnalyticsReports(JsonObject json)
	{
		_reportsToSend.Enqueue(json);
	}

	public static IEnumerator SendReportsLoop()
	{
		while (true)
		{
			if (Singleton<SessionManager>.instance.connectedState == SessionManager.ConnectedState.Connected && _reportsToSend.Count > 0)
			{
				List<JsonObject> messages = new List<JsonObject>();
				while (0 < _reportsToSend.Count)
				{
					JsonObject message = _reportsToSend.Dequeue();
					messages.Add(message);
				}
				Singleton<SessionManager>.instance.PostAnalytics(messages, null);
			}
			yield return new WaitForSeconds(5f);
		}
	}

	public static void StartupFunnelEvent(InitializationManager.StartupStep step)
	{
		if (enableStartupFunnel)
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject["step"] = string.Format("stp{0:D2}_{1}", (int)step, step.ToString());
			Report("GAME", "STARTUPPROGRESS", jsonObject);
		}
	}

	public static void TutorialFunnelEvent(TutorialStep step)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["step"] = string.Format("tut{0:D2}_{1}", (int)step, step.ToString());
		Report("tutorial", "TUTORIALPROGRESS", jsonObject);
	}

	public static void TutorialAction(string actionName)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["action"] = actionName;
		Report("tutorial", "TUTORIALACTION", jsonObject);
	}

	public static void GameStartEvent()
	{
		JsonObject jsonObject = new JsonObject();
		_gameStartTimestamp = NonUnitySingleton<TimeManager>.instance.TimeProvider.GetCurrentDevicetime();
		jsonObject.SetLong("timestamp", _gameStartTimestamp);
		Report("game load", "GAMESTART", jsonObject);
	}

	public static void GameLoadedEvent()
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetLong("timestamp_game_load_start", _gameStartTimestamp);
		jsonObject.SetLong("timestamp_game_load_end", NonUnitySingleton<TimeManager>.instance.TimeProvider.GetCurrentDevicetime());
		jsonObject.SetLong("duration", jsonObject.GetLong("timestamp_game_load_end") - jsonObject.GetLong("timestamp_game_load_start"));
		jsonObject.SetString("deviceModel", SystemInfo.deviceModel);
		jsonObject.SetString("deviceName", SystemInfo.deviceName);
		jsonObject.SetString("deviceType", SystemInfo.deviceType.ToString());
		jsonObject.SetString("deviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier);
		jsonObject.SetInt("graphicsDeviceID", SystemInfo.graphicsDeviceID);
		jsonObject.SetString("graphicsDeviceName", SystemInfo.graphicsDeviceName);
		jsonObject.SetString("graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
		jsonObject.SetInt("graphicsDeviceVendorID", SystemInfo.graphicsDeviceVendorID);
		jsonObject.SetString("graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
		jsonObject.SetInt("graphicsMemorySize", SystemInfo.graphicsMemorySize);
		jsonObject.SetInt("graphicsPixelFillrate", SystemInfo.graphicsPixelFillrate);
		jsonObject.SetInt("graphicsShaderLevel", SystemInfo.graphicsShaderLevel);
		jsonObject.SetString("npotSupport", SystemInfo.npotSupport.ToString());
		jsonObject.SetString("operatingSystem", SystemInfo.operatingSystem);
		jsonObject.SetInt("processorCount", SystemInfo.processorCount);
		jsonObject.SetString("processorType", SystemInfo.processorType);
		jsonObject.SetInt("supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount);
		jsonObject.SetBoolean("supports3DTextures", SystemInfo.supports3DTextures);
		jsonObject.SetBoolean("supportsAccelerometer", SystemInfo.supportsAccelerometer);
		jsonObject.SetBoolean("supportsComputeShaders", SystemInfo.supportsComputeShaders);
		jsonObject.SetBoolean("supportsGyroscope", SystemInfo.supportsGyroscope);
		jsonObject.SetBoolean("supportsImageEffects", SystemInfo.supportsImageEffects);
		jsonObject.SetBoolean("supportsInstancing", SystemInfo.supportsInstancing);
		jsonObject.SetBoolean("supportsLocationService", SystemInfo.supportsLocationService);
		jsonObject.SetBoolean("supportsRenderTextures", SystemInfo.supportsRenderTextures);
		jsonObject.SetBoolean("supportsRenderToCubemap", SystemInfo.supportsRenderToCubemap);
		jsonObject.SetBoolean("supportsShadows", SystemInfo.supportsShadows);
		jsonObject.SetBoolean("supportsSparseTextures", SystemInfo.supportsSparseTextures);
		jsonObject.SetInt("supportsStencil", SystemInfo.supportsStencil);
		jsonObject.SetBoolean("supportsVibration", SystemInfo.supportsVibration);
		jsonObject.SetInt("systemMemorySize", SystemInfo.systemMemorySize);
		jsonObject.SetString("gacha_abtest", UserProfile.player.GetGachaABTestingAnalitics());
		Report("game load", "GAMELOADED", jsonObject);
	}

	public static void BattleStartEvent(string battleID, UnitState[] units, MatchData.Type matchType)
	{
		if (enableBattleAnalytics)
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject["battle_id"] = ((battleID == null) ? string.Empty : battleID);
			jsonObject["timestamp"] = TimeManager.ServerTime;
			jsonObject["battle_type"] = matchType.ToString();
			jsonObject["promo_series_flag"] = Singleton<UserProfileManager>.instance.UserProfile.IsInPromoSeries;
			jsonObject["tank_ids"] = string.Join("|", units.Select((UnitState x) => x.UserUnitMetadata.UnitDataModel.id.ToString() + "," + x.UserUnitMetadata.level).ToArray());
			jsonObject["squad_id"] = PlayerSquadID();
			jsonObject["kamcord_enabled"] = UserProfile.player.preferences.KamcordOn;
			jsonObject["kamcord_available"] = Kamcord.IsEnabled();
			Report("gameplay", "BATTLESTART", jsonObject);
		}
	}

	public static void BattleEndEvent(string battleID, ReportingBattleResult battleResult, BattleController controller)
	{
		if (!enableBattleAnalytics)
		{
			return;
		}
		JsonObject jsonObject = new JsonObject();
		jsonObject["battle_id"] = ((battleID == null) ? string.Empty : battleID);
		jsonObject["timestamp"] = TimeManager.ServerTime;
		jsonObject["result"] = (int)battleResult;
		jsonObject["coins"] = controller.playerTeam.stats.coinsEarned;
		jsonObject["gems"] = controller.playerTeam.stats.gemsEarned;
		jsonObject["parts"] = string.Join("|", controller.playerTeam.stats.partsEarned.Select((IPartMetadata x) => x.ID).ToArray());
		jsonObject["scmd"] = "pvp_rating";
		jsonObject["battle_type"] = controller.MatchType;
		jsonObject["total_revives"] = controller.playerTeam.stats.revivesUsed;
		jsonObject["squad_id"] = PlayerSquadID();
		jsonObject["start_elo"] = controller.MatchData.playerTeam.pvpRating;
		jsonObject["elo_change"] = UserProfile.player.pvpRating - controller.MatchData.playerTeam.pvpRating;
		jsonObject["new_elo"] = UserProfile.player.pvpRating;
		jsonObject["promo_series"] = ((!UserProfile.player.IsInPromoSeries) ? "0" : "1");
		jsonObject["total_rounds"] = controller.matchManager.MatchHandler.roundId;
		jsonObject["pvp_forcebot"] = ((!controller.hud.playerForcedBot) ? "0" : "1");
		if (Constants.DisableExtraAnalytics)
		{
			if (controller.MatchData.opponentTeam.type == TeamType.Player)
			{
				jsonObject["opponent_id"] = controller.MatchData.opponentTeam.id;
				jsonObject["opponent_tier"] = controller.MatchData.playerTeam.division.ID;
				jsonObject["bot_id"] = string.Empty;
			}
			else
			{
				jsonObject["opponent_id"] = string.Empty;
				jsonObject["opponent_tier"] = string.Empty;
				jsonObject["bot_id"] = controller.MatchData.opponentTeam.id;
			}
			EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
			jsonObject["event_id"] = ((activeEvent == null) ? "0" : activeEvent.id);
			int num = 1;
			foreach (int boost in controller.MatchData.playerTeam.boosts)
			{
				BoostDataModel single = BoostDataModel.GetSingle(boost);
				num += single.Price.GetItemCountOfType(UserInventory.ItemType.Energy);
			}
			jsonObject["tickets_count"] = num;
			List<string> list = controller.MatchData.playerTeam.abilities.ConvertAll((IAbilityMetadata data) => data.ID);
			List<string> list2 = controller.MatchData.playerTeam.units.ConvertAll((IUnitMetadata data) => data.ID);
			jsonObject["abilitie_ids"] = JoinArray(list);
			jsonObject["tank_ids"] = JoinArray(list2);
		}
		Report("gameplay", "BATTLEEND", jsonObject);
	}

	private static string JoinArray(List<string> list)
	{
		string text = string.Empty;
		for (int i = 0; i < list.Count; i++)
		{
			text = text + list[i] + "|";
		}
		return text;
	}

	public static void RoundCompleteEvent(MatchManager matchManager, int currentRound, ServerTeamState playerTeam, ServerBattleState serverBattleState, int firstStrikeValue)
	{
		if (enableBattleAnalytics)
		{
			MatchData matchData = matchManager.MatchData;
			JsonObject jsonObject = new JsonObject();
			jsonObject["battle_id"] = ((matchData.matchId == null) ? string.Empty : matchData.matchId);
			jsonObject["round_number"] = currentRound;
			jsonObject["abilities_used"] = AbilitiesUsedAsDelimitedString(matchManager.playerActions);
			jsonObject["first_strike"] = serverBattleState.initiativeWinner == playerTeam;
			jsonObject["first_strike_number"] = firstStrikeValue;
			jsonObject["unit_info"] = string.Join("|", playerTeam.aliveUnits.Select((ServerUnitState x) => x.CurrentRollValue + "," + x.CurrentRollType).ToArray());
			Report("gameplay", "ROUNDCOMPLETE", jsonObject);
		}
	}

	public static void RoundCompleteAIEvent(string battleId, int currentRound, List<BattleAction> actions, bool firstStrike, int firstStrikeValue, ServerTeamState team)
	{
		if (enableBattleAnalytics)
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject["battle_id"] = ((battleId == null) ? string.Empty : battleId);
			jsonObject["round_number"] = currentRound;
			jsonObject["abilities_used"] = AbilitiesUsedAsDelimitedString(actions);
			jsonObject["first_strike"] = firstStrike;
			jsonObject["first_strike_number"] = firstStrikeValue;
			jsonObject["unit_info"] = string.Join("|", team.aliveUnits.Select((ServerUnitState x) => x.CurrentRollValue + "," + x.CurrentRollType).ToArray());
			jsonObject["isBot"] = true;
			Report("gameplay", "ROUNDCOMPLETE", jsonObject);
		}
	}

	private static string ItemCollectionDataModelToDelimitedString(ItemCollectionDataModel unitValue)
	{
		List<string> list = new List<string>();
		if (unitValue != null)
		{
			foreach (ItemCollectionDataModel.Item item in unitValue.items)
			{
				string text = ((item.itemId == 0) ? string.Empty : (item.itemId + ":"));
				list.Add((int)item.itemType + ":" + text + item.amount);
			}
		}
		return string.Join("|", list.ToArray());
	}

	public static void ScrapUnitEvent(string unit_id, string object_id, string outcome, ItemCollectionDataModel unitValue = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["unit_id"] = unit_id;
		jsonObject["object_id"] = object_id;
		jsonObject["outcome"] = outcome;
		jsonObject["prizes"] = ItemCollectionDataModelToDelimitedString(unitValue);
		Report("gameplay", "SCRAPUNIT", jsonObject);
	}

	public static void SetManageSquadPreTeams(List<UserTeam> teams, int current_team)
	{
		_manageSquadTeamsPreModCurrentTeam = current_team;
		_manageSquadTeamsPreMod = ManageSquadEventTeamsToString(teams);
	}

	private static List<string> ManageSquadEventTeamsToStringsList(List<UserTeam> teams)
	{
		List<string> list = new List<string>();
		foreach (UserTeam team in teams)
		{
			list.Add(string.Join(";", team.units.Select((UserUnit x) => (x == null) ? string.Empty : (x.metadataId + "," + x.level)).ToArray()));
		}
		return list;
	}

	private static string ManageSquadEventTeamsToString(List<UserTeam> teams)
	{
		List<string> list = new List<string>();
		foreach (UserTeam team in teams)
		{
			list.Add(string.Join(";", team.units.Select((UserUnit x) => (x == null) ? string.Empty : (x.metadataId + "," + x.level)).ToArray()));
		}
		return string.Join("|", list.ToArray());
	}

	public static void ManageSquadEvent(List<UserTeam> teams, int current_team)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["pre_tank_ids"] = _manageSquadTeamsPreMod;
		jsonObject["post_tank_ids"] = ManageSquadEventTeamsToString(teams);
		jsonObject["pre_squad_id"] = _manageSquadTeamsPreModCurrentTeam;
		jsonObject["squad_id"] = current_team;
		Report("gameplay", "MANAGESQUAD", jsonObject);
	}

	public static void SetInitialAbilities(List<string> abilities)
	{
		_initialAbilityIDs = string.Join(",", abilities.ToArray());
	}

	public static bool AbilitiesDidChange(List<string> abilities)
	{
		return _initialAbilityIDs != string.Join(",", abilities.ToArray());
	}

	public static void ManageAbilitiesEvent(List<string> abilities)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["ability_ids"] = AbilityIDsAsDelimitedString(abilities);
		jsonObject["ability_changes"] = AbilitiesDidChange(abilities);
		Report("gameplay", "MANAGEABILITIES", jsonObject);
	}

	public static void ResearchStartEvent(UserResearcher researcher)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["entity_id"] = researcher.startTime;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		string evid = "NONE";
		UserResearcher.ResearchType researchType = researcher.researchType;
		if (researchType == UserResearcher.ResearchType.BuildTank)
		{
			jsonObject["tank_id"] = researcher.itemID;
			UserPriceDataModel researchCost = researcher.ResearchItem.GetResearchCost(UserProfile.player);
			jsonObject["cost"] = string.Empty;
			foreach (ItemCollectionDataModel.Item item in researchCost.items)
			{
				JsonObject jsonObject3;
				JsonObject jsonObject2 = (jsonObject3 = jsonObject);
				string index2;
				string index = (index2 = "cost");
				object obj = jsonObject3[index2];
				object obj2 = obj;
				jsonObject2[index] = string.Concat(obj2, item.itemId, ",", item.amount, "|");
			}
			evid = "BUILDTANKSTART";
		}
		else
		{
			Log.Warning("Unsupported research type: " + researcher.researchType);
		}
		Report("gameplay", evid, jsonObject);
	}

	public static void ResearchSkippedEvent(UserResearcher researcher)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["start_time"] = researcher.startTime;
		UserPriceDataModel hurryCost = researcher.GetHurryCost();
		jsonObject["skip_cost_item"] = (int)hurryCost.items[0].itemType;
		jsonObject["skip_cost"] = hurryCost.items[0].amount;
		jsonObject["skip_remaining_seconds"] = researcher.GetRemainingTime();
		string evid = "NONE";
		UserResearcher.ResearchType researchType = researcher.researchType;
		if (researchType == UserResearcher.ResearchType.BuildTank)
		{
			jsonObject["tank_id"] = researcher.itemID;
			evid = "BUILDTANKSKIP";
		}
		else
		{
			Log.Warning("Unsupported research type: " + researcher.researchType);
		}
		Report("gameplay", evid, jsonObject);
	}

	public static void ResearchCompleteEvent(UserResearcher researcher, UserUnit unit)
	{
		JsonObject jsonObject = new JsonObject();
		string evid = "BUILDTANKCOMPLETE";
		jsonObject["start_time"] = researcher.startTime;
		jsonObject["object_id"] = unit.id;
		jsonObject["tank_id"] = unit.metadataId;
		Report("gameplay", evid, jsonObject);
	}

	public static void TankUpgradePopup(UserUnit unit)
	{
		JsonObject jsonObject = new JsonObject();
		string evid = "ACCESSTANKUPGRADE";
		jsonObject["tank_id"] = unit.metadataId;
		jsonObject["entity_id"] = unit.id;
		jsonObject["level_type"] = ((UnitPartialLevelDataModel.GetPartialLevelsForUnit(unit.metadataId, unit.level).Count <= 0) ? "cash" : "parts");
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", evid, jsonObject);
	}

	public static void TankLevelUpEvent(string tankID, int level, string cost, string gifts)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["tank_id"] = tankID;
		jsonObject["level"] = level;
		jsonObject["cost"] = cost;
		jsonObject["prizes"] = gifts;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "TANKLEVELUP", jsonObject);
	}

	public static void TierUpEvent(int tier)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["tier"] = tier;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "TIERUP", jsonObject);
	}

	public static void RepairSquadEvent(string squadID, List<UserUnit> units, bool timeAccelerated, UserPriceDataModel price)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["squad_id"] = squadID;
		jsonObject["tank_ids"] = string.Join("|", units.Select((UserUnit x) => x.metadataId).ToArray());
		jsonObject["time_accelerated"] = timeAccelerated;
		jsonObject["skip_cost"] = price.items[0].amount + " " + price.items[0].itemType;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "REPAIRSQUAD", jsonObject);
	}

	public static void BuyTicketsEvent(int quantity, UserPriceDataModel price)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["qty"] = quantity;
		jsonObject["price"] = UserPriceDataModelToString(price);
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "BUYTICKETS", jsonObject);
	}

	public static void ClientBattleSyncError(string errorMessage, string matchID)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["msg"] = errorMessage;
		jsonObject["matchID"] = matchID;
		Report("error", "CLIENTBATTLESYNCERROR", jsonObject);
	}

	public static void UnitSeriesCompleteEvent(int tier, ItemCollectionDataModel unitValue = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["tier"] = tier;
		jsonObject["prizes"] = ItemCollectionDataModelToDelimitedString(unitValue);
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "UNITSERIESCOMPLETE", jsonObject);
	}

	public static void GemsToCashPopupEvent(PurchaseSource source, PurchaseAction action, UserPriceDataModel cost = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["source"] = source.ToString();
		jsonObject["action"] = action.ToString();
		jsonObject["cost"] = ((cost != null) ? UserPriceDataModelToString(cost) : string.Empty);
		Report("gameplay", "GEMSTOCASHPOPUP", jsonObject);
	}

	public static void TicketsDepletedEvent(string action, UserPriceDataModel cost = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["action"] = action;
		jsonObject["cost"] = ((cost != null) ? UserPriceDataModelToString(cost) : string.Empty);
		Report("gameplay", "TICKETSDEPLETED", jsonObject);
	}

	public static void CurrencyTransactionEvent(PurchaseSource source, UserPriceDataModel cost = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["source"] = source.ToString();
		jsonObject["cost"] = ((cost != null) ? UserPriceDataModelToString(cost) : string.Empty);
		Report("gameplay", "CURRENCYTRANSACTION", jsonObject);
	}

	public static void CollectContractEvent(string entityId)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["entity_id"] = entityId;
		Report("gameplay", "COLLECTCONTRACT", jsonObject);
	}

	public static void BeginContractEvent(string entityId, string contractId, long duration, ItemCollectionDataModel unitValue = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["entity_id"] = entityId;
		jsonObject["contract_id"] = contractId;
		jsonObject["duration"] = duration;
		jsonObject["prizes"] = ItemCollectionDataModelToDelimitedString(unitValue);
		Report("gameplay", "BEGINCONTRACT", jsonObject);
	}

	public static void GachaEvent(int gacha_id, ItemCollectionDataModel prizes, UserPriceDataModel cost = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["gacha_id"] = gacha_id;
		jsonObject["prizes"] = ItemCollectionDataModelToDelimitedString(prizes);
		jsonObject["cost"] = ((cost != null) ? UserPriceDataModelToString(cost) : string.Empty);
		jsonObject["gacha_abtest"] = UserProfile.player.GetGachaABTestingAnalitics(gacha_id);
		Report("gacha", "GACHA", jsonObject);
	}

	public static void BotTeamQuotaHit(string outcome, int cost, int reset_qty)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["outcome"] = outcome;
		jsonObject["cost"] = cost;
		jsonObject["reset_qty"] = reset_qty;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "TARGETEDBOTTEAMQUOTA", jsonObject);
	}

	public static void TargetBotTeam(string outcome, string boost)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["outcome"] = outcome;
		jsonObject["boost"] = boost;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "TARGETEDBOTTEAM", jsonObject);
	}

	public static void TankDirectPurchase(string outcome, string cost, string tankid)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["outcome"] = outcome;
		jsonObject["cost"] = cost;
		jsonObject["tank_id"] = tankid;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "TANKDIRECTPURCHASE", jsonObject);
	}

	public static void MissingPartUpgrade(string outcome, string cost, string tankid)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["outcome"] = outcome;
		jsonObject["cost"] = cost;
		jsonObject["tank_id"] = tankid;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "PARTUPGRADE", jsonObject);
	}

	public static void PartsMixerEvent(string gacha_id, string partWanted, string partTraded)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["gacha_id"] = gacha_id;
		jsonObject["part_wanted"] = partWanted;
		jsonObject["part_traded"] = partTraded;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gacha", "PLINKO", jsonObject);
	}

	public static void MatchmakingFailureEvent(string firstMatchID, string previousMatchID)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["first_match_id"] = previousMatchID;
		jsonObject["prev_match_id"] = previousMatchID;
		Report("error", "MATCHMAKINGFAILURE", jsonObject);
	}

	public static void MatchmakingAbortEvent(string firstMatchID, string previousMatchID)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["first_match_id"] = previousMatchID;
		jsonObject["last_match_id"] = previousMatchID;
		Report("error", "MATCHMAKINGABORTED", jsonObject);
	}

	public static void TellAFriendEvent(string action, string channel)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["action"] = action;
		jsonObject["channel"] = channel;
		Report("gameplay", "TELLFRIEND", jsonObject);
	}

	public static void ViewedLeaderboard(long rank)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["leaderboard_rank"] = rank;
		Report("gameplay", "LEADERBOARDVIEW", jsonObject);
	}

	public static void ViewedClubLeaderboard(long rank)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["leaderboard_rank"] = rank;
		Report("gameplay", "CLUBLEADERBOARDVIEW", jsonObject);
	}

	public static void FacebookUpgrade(string action)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["timestamp"] = TimeManager.ServerTime;
		jsonObject["action"] = action;
		Report("ui", "FACEBOOKUPGRADE", jsonObject);
	}

	public static void VideoAdDisplay(string adId)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["ad_id"] = adId;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("ui", "VIDEOADDISPLAY", jsonObject);
	}

	public static void VideoAdComplete(string adId, SponsorPayManager.BrandEngageResult videoResult)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["ad_id"] = adId;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		string text = string.Empty;
		switch (videoResult)
		{
		case SponsorPayManager.BrandEngageResult.Success:
			text = "VIDEOADDISPLAYCOMPLETE";
			break;
		case SponsorPayManager.BrandEngageResult.Error:
			text = "VIDEOADDISPLAYERROR";
			break;
		case SponsorPayManager.BrandEngageResult.NoVideoAvailable:
			text = "VIDEOADDISPLAYNOVIDEOAVAILABLE";
			break;
		case SponsorPayManager.BrandEngageResult.Aborted:
			text = "VIDEOADDISPLAYVIDEOABORTED";
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			Report("ui", text, jsonObject);
		}
	}

	public static void EventIntroDisplay(string eventID)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["event_id"] = eventID;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("ui", "VIDEOINTRODISPLAY", jsonObject);
	}

	public static void NotificationClicked(string notificationType)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["notificationtype"] = notificationType;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("ui", "NOTIFICATIONCLICK", jsonObject);
	}

	public static void NotificationShown(string notificationType)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["notificationtype"] = notificationType;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("ui", "NOTIFICATIONSHOWN", jsonObject);
	}

	public static void AccessDetails(string gachaId)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["gacha_id"] = gachaId;
		Report("ui", "ACCESSDETAILS", jsonObject);
	}

	public static void MenuNavigation(string menuItem)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["menu_item"] = menuItem;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("ui", "MENUNAVIGATION", jsonObject);
	}

	public static void KamcordShareIntent()
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("ui", "KAMKORDSHAREINTENT", jsonObject);
	}

	public static void KamcordShare(string channel, bool success)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["timestamp"] = TimeManager.ServerTime;
		jsonObject["channel"] = channel;
		jsonObject["success"] = success;
		Report("ui", "KAMKORDSHARE", jsonObject);
	}

	public static void KamcordShareEnable()
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("ui", "KAMKORDENABLE", jsonObject);
	}

	public static void KamcordShareDisable()
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("ui", "KAMKORDDISABLE", jsonObject);
	}

	public static void JoinClubEvent(string id, string clubID, string clubName)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["user_id"] = id;
		jsonObject["club_id"] = clubID;
		jsonObject["club_name"] = clubName;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "CLUBJOIN", jsonObject);
	}

	public static void LeaveClubEvent(string id, string clubID)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["user_id"] = id;
		jsonObject["club_id"] = clubID;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "CLUBLEAVE", jsonObject);
	}

	public static void CreateClubEvent(string clubID, string name, ClubTypes teamType)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["club_id"] = clubID;
		jsonObject["name"] = name;
		jsonObject["club_type"] = (int)teamType;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "CLUBCREATE", jsonObject);
	}

	public static void PvpLeaderBoard(string leaderboard_league, string prizes)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["leaderboard_league"] = leaderboard_league;
		jsonObject["prizes"] = prizes;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "PVPLEADERBOARD", jsonObject);
	}

	public static void LeaderboardContentUpdate(int scoreDelta, int totalScore, string leaderboardName)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["leaderboard_game"] = leaderboardName;
		jsonObject["score_delta"] = scoreDelta;
		jsonObject["total_score"] = totalScore;
		jsonObject["timestamp"] = TimeManager.ServerTime;
		Report("gameplay", "LEADERBOARDUPDATE", jsonObject);
	}

	public static void RaidBossDismissed(int raidbossId, string raidbossInstanceId, int BaseHealth, int HealthRemaining, string dismissal_type)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetInt("boss_id", raidbossId);
		jsonObject.SetString("boss_entity_id", raidbossInstanceId);
		jsonObject.SetInt("base_hp", BaseHealth);
		jsonObject.SetInt("hp_remaining", HealthRemaining);
		jsonObject.SetString("dismissal_type", dismissal_type);
		jsonObject.SetLong("timestamp", TimeManager.ServerTime);
		Report("gameplay", "RAIDBOSSDISMISS", jsonObject);
	}

	public static void RaidBossRewardClaimed(int raidbossId, string raidbossInstanceId, int prizes, string mvp, int mvpBonus, int socialBonus, int socialAmount, int discoveryBonus, int totalDamage, int bonusDamage)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetInt("boss_id", raidbossId);
		jsonObject.SetString("boss_entity_id", raidbossInstanceId);
		jsonObject.SetInt("prizes", prizes);
		jsonObject.SetString("mvp", mvp);
		jsonObject.SetInt("mvp_bonus", mvpBonus);
		jsonObject.SetInt("social_bonus", socialBonus);
		jsonObject.SetInt("social_amount", socialAmount);
		jsonObject.SetInt("discovery_bonus", discoveryBonus);
		jsonObject.SetInt("total_damage_dealt", totalDamage);
		jsonObject.SetInt("total_damage_dealt_bonus", bonusDamage);
		jsonObject.SetLong("timestamp", TimeManager.ServerTime);
		Report("gameplay", "RAIDBOSSCLAIMREWARD", jsonObject);
	}

	public static void RaidBossAccessBoard(List<string> raidbossIds)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("boss_id_list", string.Join("|", raidbossIds.ToArray()));
		jsonObject.SetLong("timestamp", TimeManager.ServerTime);
		Report("gameplay", "RAIDBOSSACCESSBOARD", jsonObject);
	}

	public static void RaidBossFight(int raidbossId, string raidbossInstanceId, List<int> prizes, int baseHp, int remainingHp, UnitState[] units, int squadId, bool kamkord, int revive_qty, string battleId, int attackDamage)
	{
		string text = string.Empty;
		foreach (int prize in prizes)
		{
			text = text + prize + "|";
		}
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetInt("boss_id", raidbossId);
		jsonObject.SetString("boss_entity_id", raidbossInstanceId);
		jsonObject.SetInt("base_hp", baseHp);
		jsonObject.SetInt("hp_remaining", remainingHp);
		jsonObject.SetInt("squad_id", squadId);
		jsonObject.SetBoolean("kamkord", kamkord);
		jsonObject.SetInt("revive_qty", revive_qty);
		jsonObject.SetString("prizes", text);
		jsonObject.SetString("battle_id", battleId);
		jsonObject.SetInt("attack_damage", attackDamage);
		jsonObject.SetLong("timestamp", TimeManager.ServerTime);
		jsonObject["tank_ids"] = string.Join("|", units.Select((UnitState x) => x.UserUnitMetadata.UnitDataModel.id.ToString() + "," + x.UserUnitMetadata.level).ToArray());
		Report("gameplay", "RAIDBOSSFIGHT", jsonObject);
	}

	public static void NewsTemplateClick(NewsTypes newType, string origin, string destination)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["timestamp"] = TimeManager.ServerTime;
		jsonObject["origin"] = origin;
		jsonObject["news_item_clicked"] = newType.ToString();
		jsonObject["destination"] = destination;
		Report("gameplay", "NEWSCLICK", jsonObject);
	}

	public static void MultiTeamReportClaimed(List<UserTeam> teams, int activeEventId, float totalPointsEarned)
	{
		List<string> list = ManageSquadEventTeamsToStringsList(teams);
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetLong("timestamp", TimeManager.ServerTime);
		jsonObject.SetInt("event_id", activeEventId);
		jsonObject.SetString("squad_1_detail", list[0]);
		jsonObject.SetString("squad_2_detail", list[1]);
		jsonObject.SetString("squad_3_detail", list[2]);
		jsonObject.SetFloat("prizes", totalPointsEarned);
		Report("gameplay", "REPORTCARD", jsonObject);
	}
}
