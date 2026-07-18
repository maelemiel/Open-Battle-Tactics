using System;
using System.Collections;
using System.Collections.Generic;
using LitJson0;

public static class MatchParser
{
	public class Input
	{
		public JsonObject json;

		public Func<JsonObject, IUnitMetadata> UnitParser;

		public Func<string, IAbilityMetadata> AbilityParser;

		public Func<string, IDivisionMetadata> DivisionParser;

		public string localPlayerID;
	}

	public static MatchData ParseMatchJSON(Input input)
	{
		JsonObject json = input.json;
		MatchData matchData = new MatchData();
		matchData.matchId = input.json.GetString("_id");
		List<JsonObject> objectList = json.GetObjectList("players");
		if (objectList.Count < 2)
		{
			throw new Exception("Match must have 2 players!");
		}
		matchData.battleSeed = json.GetInt("match_random_seed");
		int num = 0;
		foreach (JsonObject item in (IEnumerable)objectList)
		{
			JsonObject jsonObject2 = item.GetObject("player");
			List<JsonObject> objectList2 = item.GetObjectList("units");
			if (jsonObject2.GetString("_id") == input.localPlayerID)
			{
				matchData.playerTeam = ParseMatchPlayerJSON(item, jsonObject2, input, objectList2);
				matchData.playerTeam.isTeamOne = true;
				matchData.playerIsHost = num == 0;
			}
			else
			{
				matchData.opponentTeam = ParseMatchPlayerJSON(item, jsonObject2, input, objectList2);
			}
			num++;
		}
		if (json.Contains("rounds"))
		{
			matchData.actions = new List<MatchData.RoundActions>();
			List<JsonObject> objectList3 = json.GetObjectList("rounds");
			foreach (JsonObject item2 in objectList3)
			{
				MatchData.RoundActions roundActions = new MatchData.RoundActions();
				roundActions.round = item2.GetInt("round_id");
				roundActions.userID = item2.GetString("user_id");
				roundActions.actions = new List<BattleAction>();
				foreach (JsonObject @object in item2.GetObjectList("actions"))
				{
					roundActions.actions.Add(BattleAction.DeserializeAction(@object));
				}
				matchData.actions.Add(roundActions);
			}
		}
		switch (input.json.GetString("type"))
		{
		case "pvp":
			matchData.type = MatchData.Type.PVP;
			break;
		case "bot":
			matchData.type = MatchData.Type.AI;
			break;
		case "auto_bot_battle":
			matchData.type = MatchData.Type.AUTO_BOT_BATTLE;
			break;
		}
		return matchData;
	}

	public static OpponentData ParseMatchPlayerJSON(JsonObject containerJson, JsonObject playerData, Input input, List<JsonObject> unitData)
	{
		OpponentData opponentData = null;
		switch (playerData.GetString("type"))
		{
		default:
		{
			int num;
			if (num == 1)
			{
				opponentData = ParseMatchBotJson(playerData, input, unitData);
			}
			else
			{
				Log.Error("Could not find Player type: " + playerData.GetString("type"));
			}
			break;
		}
		case "user":
			opponentData = ParseMatchUserJSON(playerData, input, unitData);
			break;
		}
		opponentData.randomSeed = containerJson.GetInt("random_seed");
		return opponentData;
	}

	public static OpponentData ParseMatchBotJson(JsonObject json, Input matchInput, List<JsonObject> userUnits)
	{
		OpponentData opponentData = ParseMatchUserJSON(json, matchInput, userUnits);
		opponentData.type = TeamType.Bot;
		AiArmyDataModel single = AiArmyDataModel.GetSingle(opponentData.id);
		if (single != null)
		{
			AiHandlerDataModel single2 = AiHandlerDataModel.GetSingle(single.aiStrategyId);
			if (single2 != null)
			{
				opponentData.ai = AIHandlerFactory.Create(single2.handler);
			}
			else
			{
				Log.Error("AI Handler ID " + single.aiStrategyId + " does not exist!");
			}
			opponentData.name = single.Name;
			opponentData.type = single.TeamType;
			opponentData.rewardDropRate = single.RewardDropRate;
		}
		else
		{
			Log.Error("Could not find AI Army for bot ID " + opponentData.id);
		}
		if (opponentData.ai == null)
		{
			opponentData.ai = new SimpleAI();
			Log.Warning("Bot ID " + opponentData.id + " is using default AI.");
		}
		return opponentData;
	}

	public static OpponentData ParseMatchUserJSON(JsonObject json, Input matchInput, List<JsonObject> userUnits)
	{
		OpponentData opponentData = new OpponentData();
		opponentData.id = json.GetString("_id");
		opponentData.type = TeamType.Player;
		JsonObject jsonObject = json.GetObject("mobage");
		if (jsonObject != null)
		{
			opponentData.name = jsonObject.GetString("nickname");
			opponentData.thumbnailURL = jsonObject.GetString("thumbnail");
		}
		JsonObject jsonObject2 = json.GetObject("stats");
		if (jsonObject2 != null)
		{
			JsonObject jsonObject3 = jsonObject2.GetObject("division");
			if (jsonObject3 != null)
			{
				opponentData.division = matchInput.DivisionParser(jsonObject3.GetString("division_id"));
			}
			opponentData.winStreak = jsonObject2.GetInt("win_streak");
		}
		JsonObject jsonObject4 = json.GetObject("pvp_stats");
		if (jsonObject4 != null)
		{
			opponentData.winStreak = jsonObject4.GetInt("win_streak");
			opponentData.division = matchInput.DivisionParser(jsonObject4.GetString("division_id"));
		}
		opponentData.pvpRating = json.GetInt("pvp_rating");
		List<string> list = new List<string>();
		if (json.Contains("abilities"))
		{
			list = new List<string>();
			foreach (object item3 in json.GetList("abilities"))
			{
				list.Add((string)item3);
			}
		}
		if (json.Contains("boosts"))
		{
			foreach (JsonObject @object in json.GetObjectList("boosts"))
			{
				int item = @object.GetInt("meta_id");
				opponentData.boosts.Add(item);
			}
		}
		List<IAbilityMetadata> list2 = new List<IAbilityMetadata>();
		foreach (string item4 in list)
		{
			if (!string.IsNullOrEmpty(item4) && !(item4 == "0"))
			{
				IAbilityMetadata abilityMetadata = matchInput.AbilityParser(item4);
				if (abilityMetadata != null)
				{
					list2.Add(abilityMetadata);
				}
			}
		}
		opponentData.abilities = list2;
		List<IUnitMetadata> list3 = new List<IUnitMetadata>();
		foreach (JsonObject userUnit in userUnits)
		{
			if (userUnit != null)
			{
				IUnitMetadata item2 = matchInput.UnitParser(userUnit);
				list3.Add(item2);
			}
		}
		opponentData.units = list3;
		if (opponentData.division == null)
		{
			opponentData.division = matchInput.DivisionParser("1");
		}
		JsonObject jsonObject5 = json.GetObject("raidbosscontainer");
		if (jsonObject5 != null)
		{
			opponentData.startingBossHealth = jsonObject5.GetInt("bosshealth");
			opponentData.raidbossId = jsonObject5.GetString("bossId");
			opponentData.raidbossStatus = jsonObject5.GetString("bossState");
		}
		return opponentData;
	}
}
