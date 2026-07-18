using System;
using System.Collections.Generic;
using LitJson0;
using UnityEngine;

public class BountyBoardDataEntry
{
	public enum RaidBossState
	{
		ALIVE = 0,
		DEAD = 1,
		EXPIRED = 2
	}

	public string id;

	public string raidBossId;

	public string userId;

	public int bossHealth;

	public int armyId;

	public int damage;

	public bool fightCell;

	public string owner_club;

	public string owner_club_name;

	public string owner_club_badge;

	public string owner_user;

	public string owner_user_name;

	public string mvp_user;

	public string mvp_user_name;

	public string mvp_club;

	public string mvp_club_name;

	public string mvp_club_badge;

	public int mvp_damage;

	public long timeToLive;

	public Action<BattleSceneModel> battleBoss;

	public string state;

	public bool complete;

	public int raidBossLevel;

	public RaidBossState State
	{
		get
		{
			switch (state)
			{
			case "alive":
				return RaidBossState.ALIVE;
			case "dead":
				return RaidBossState.DEAD;
			case "expired":
				return RaidBossState.EXPIRED;
			default:
				return RaidBossState.ALIVE;
			}
		}
	}

	public static List<BountyBoardDataEntry> CreateEntryFromJson(JsonObject json)
	{
		List<BountyBoardDataEntry> list = new List<BountyBoardDataEntry>();
		if (!json.Contains("raidbosses"))
		{
			return list;
		}
		List<JsonObject> objectList = json.GetObjectList("raidbosses");
		foreach (JsonObject item in objectList)
		{
			BountyBoardDataEntry bountyBoardDataEntry = new BountyBoardDataEntry();
			bountyBoardDataEntry.id = json.GetString("_id");
			bountyBoardDataEntry.userId = json.GetString("user_id");
			bountyBoardDataEntry.bossHealth = Mathf.Max(0, item.GetIntOrDefault("boss_health", 0));
			bountyBoardDataEntry.damage = item.GetIntOrDefault("user_damage", 0);
			bountyBoardDataEntry.raidBossLevel = item.GetIntOrDefault("level", 1);
			bountyBoardDataEntry.owner_club = item.GetString("owner_club");
			bountyBoardDataEntry.owner_club_name = item.GetString("owner_club_name");
			bountyBoardDataEntry.owner_club_badge = item.GetString("owner_club_badge");
			bountyBoardDataEntry.owner_user = item.GetString("owner_user");
			bountyBoardDataEntry.owner_user_name = item.GetString("owner_user_name");
			bountyBoardDataEntry.timeToLive = item.GetLongOrDefault("time_to_live", 0L);
			bountyBoardDataEntry.state = item.GetString("state");
			bountyBoardDataEntry.complete = item.GetInt("complete") == 1;
			bountyBoardDataEntry.raidBossId = item.GetString("raidboss_id");
			bountyBoardDataEntry.armyId = int.Parse(item.GetString("army_id"));
			if (item.Contains("mvp_user"))
			{
				bountyBoardDataEntry.mvp_user = item.GetString("mvp_user");
				bountyBoardDataEntry.mvp_user_name = item.GetString("mvp_user_name");
				bountyBoardDataEntry.mvp_club = item.GetString("mvp_club");
				bountyBoardDataEntry.mvp_club_name = item.GetString("mvp_club_name");
				bountyBoardDataEntry.mvp_club_badge = item.GetString("mvp_club_badge");
				bountyBoardDataEntry.mvp_damage = item.GetInt("mvp_damage");
			}
			if (!bountyBoardDataEntry.complete)
			{
				list.Add(bountyBoardDataEntry);
			}
		}
		return list;
	}
}
