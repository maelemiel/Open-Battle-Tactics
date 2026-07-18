using System.Linq;

public class BattleDebugUtils
{
	public static void PrintTeamStats(ServerTeamState team)
	{
		Log.Debug(BattleOutputHandler.GetTeamName(team) + " Stats:");
		Log.Debug("simulatorVersion: " + BattleSimulatorVersion.Version);
		Log.Debug("isWinner:" + team.stats.isWinner);
		Log.Debug("coinsEarned:" + team.stats.coinsEarned);
		Log.Debug("gemsEarned:" + team.stats.gemsEarned);
		Log.Debug("pointsEarned:" + team.stats.pointsEarned);
		Log.Debug("eventPointsEarned:" + team.stats.eventPointsEarned);
		Log.Debug("unitsDestroyed:" + team.stats.unitsDestroyed);
		Log.Debug("totalDamageDealt:" + team.stats.totalDamageDealt);
		Log.Debug("partsEarned: " + string.Join(", ", team.stats.partsEarned.Select((IPartMetadata x) => x.ID).ToArray()));
		Log.Debug("initiativeWins: " + team.stats.initiativeWins);
		Log.Debug("unitsSurvived:" + team.stats.unitsSurvived);
		Log.Debug("raidBossDamage:" + team.stats.raidBossDamageDealt);
		Log.Debug("revivesUsed:" + team.stats.revivesUsed);
		int num = (int)team.randomProvider.Clone().Next(1000000u);
		Log.Debug("startRandom: " + team.randomSeed);
		Log.Debug("endRandom:" + num);
		Log.Debug("generatedRandomNumbers:" + team.randomProvider.GeneratedNumbers);
		Log.Debug(string.Join("\n", team.stats.generatedRandomNumbersData.ToArray()));
		Log.Debug("\n");
	}

	public static void PrintTeamState(ServerTeamState team)
	{
		Log.Debug(BattleOutputHandler.GetTeamName(team) + " State:");
		Log.Debug("Units:");
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState in aliveUnits)
		{
			Log.Debug("\tHP:" + serverUnitState.hp + "\tArmor:" + serverUnitState.armor + "\tRollValue[" + serverUnitState.currentRoll + "]:" + serverUnitState.CurrentRollValue + "\tRollType:" + serverUnitState.CurrentRollType.ToString());
		}
		Log.Debug("Abilities:");
		ServerAbilityState[] abilities = team.abilities;
		foreach (ServerAbilityState serverAbilityState in abilities)
		{
			Log.Debug("\tName:" + serverAbilityState.metadata.Name + "\tEnergy:" + serverAbilityState.energy + "\tTarget:" + serverAbilityState.target);
		}
		Log.Debug("\n");
	}
}
