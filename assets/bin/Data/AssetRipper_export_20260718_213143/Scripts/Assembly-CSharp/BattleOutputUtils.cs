using System.Linq;
using LitJson0;

public class BattleOutputUtils
{
	public static JsonObject CreateJsonBattleResult(ServerTeamState teamState)
	{
		ServerTeamStatsState stats = teamState.stats;
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetBoolean("is_win", stats.isWinner);
		jsonObject.SetInt("coins_earned", stats.coinsEarned);
		jsonObject.SetInt("gems_earned", stats.gemsEarned);
		jsonObject.SetInt("points_earned", stats.pointsEarned);
		jsonObject.SetInt("event_points_earned", stats.eventPointsEarned);
		jsonObject.SetInt("victory_points_earned", stats.victoryPointsEarned);
		jsonObject.SetInt("units_destroyed", stats.unitsDestroyed);
		jsonObject.SetList("parts_earned", stats.partsEarned.Select((IPartMetadata x) => x.ID).ToList());
		jsonObject.SetList("damage_drop_reward", stats.giftDrops);
		jsonObject.SetInt("raid_boss_damage_dealt", stats.raidBossDamageDealt);
		jsonObject.SetInt("initiative_wins", stats.initiativeWins);
		jsonObject.SetInt("units_survived", stats.unitsSurvived);
		jsonObject.SetInt("revives_used", stats.revivesUsed);
		jsonObject.SetInt("_sim_version", BattleSimulatorVersion.Version);
		int randomSeed = teamState.randomSeed;
		int value = (int)teamState.randomProvider.Clone().Next(1000000u);
		jsonObject.SetInt("_start_rand", randomSeed);
		jsonObject.SetInt("_end_rand", value);
		jsonObject.SetInt("_generated_random_numbers", teamState.randomProvider.GeneratedNumbers);
		jsonObject.SetList("_generated_random_numbers_data", teamState.stats.generatedRandomNumbersData);
		return jsonObject;
	}
}
