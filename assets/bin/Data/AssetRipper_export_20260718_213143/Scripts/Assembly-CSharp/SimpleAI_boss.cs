using System.Collections.Generic;

public class SimpleAI_boss : SimpleAI
{
	private const int THRESHOLD_SACRIFICE_UNITS = 3;

	private const int THRESHOLD_SACRIFICE_HP = 2;

	private const int THRESHOLD_JAMMER = 2;

	private const float PROBABILITY_LOW = 0.1f;

	private const float PROBABILITY_MED = 0.25f;

	private const float PROBABILITY_HIGH = 0.5f;

	private const float PROBABILITY_ALWAYS = 1f;

	public override List<BattleAction> Think(ServerTeamState team)
	{
		team = CloneState(team);
		actions = new List<BattleAction>();
		int num = team.aliveUnits.Length;
		TryRerollThreshold(team, 1f, 0);
		TryRerollThreshold(team, 0.25f, 1);
		TryJammer(team, (num < 2) ? 0.25f : 0.5f);
		TryTarget(team, 0.5f);
		TryAdvTarget(team, 0.5f);
		TryDrawfire(team, 0.5f);
		TryMiniStrike(team, 0.5f);
		TryIonStrike(team, 0.5f);
		TryEMPulse(team, 0.5f);
		TryFirebomb(team, 1f);
		TryBarrage(team, 1f);
		TrySendEmoticon(team, 0.25f);
		return actions;
	}
}
