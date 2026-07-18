using System.Collections.Generic;

public class SimpleAI_mod02 : SimpleAI
{
	private const int THRESHOLD_SACRIFICE_UNITS = 3;

	private const int THRESHOLD_SACRIFICE_HP = 2;

	private const int THRESHOLD_JAMMER = 2;

	private const float PROBABILITY_LOW = 0.1f;

	private const float PROBABILITY_MED = 0.5f;

	private const float PROBABILITY_HIGH = 0.75f;

	private const float PROBABILITY_ALWAYS = 1f;

	public override List<BattleAction> Think(ServerTeamState team)
	{
		team = CloneState(team);
		actions = new List<BattleAction>();
		DecideAIState(team);
		int num = team.aliveUnits.Length;
		if (num >= 3)
		{
			TrySelfDestruct(team, 1f);
		}
		switch (currentState)
		{
		case AIState.WINNING:
			TryDrawfire(team, 0.75f);
			TryJammer(team, (num < 2) ? 0.5f : 1f);
			TryJammer2(team, (num < 2) ? 0.5f : 1f);
			TryArmorAll(team, 0.5f);
			break;
		case AIState.LOSING:
			if (num > 1)
			{
				TryJammer(team, 1f);
				TryJammer2(team, 1f);
				TryArmorAll(team, 0.75f);
			}
			else
			{
				TryJammer2(team, 0.5f);
			}
			break;
		default:
			if (num == 1)
			{
				TryDrawfire(team, 1f);
			}
			TryArmorAll(team, 0.5f);
			TryJammer(team, (num < 2) ? 0.5f : 0.75f);
			TryJammer2(team, (num < 2) ? 0.5f : 0.75f);
			break;
		}
		TryRerollThreshold(team, 1f, 0);
		TryRerollThreshold(team, 0.75f, 1);
		TryRerollThreshold(team, 0.5f, 2);
		TryTarget(team, 0.75f);
		TryAdvTarget(team, 0.75f);
		TryMiniStrike(team, 0.75f);
		TryIonStrike(team, 0.75f);
		TryEMPulse(team, 0.75f);
		TryFirebomb(team, 1f);
		TryBarrage(team, 1f);
		TrySendEmoticon(team, 0.5f);
		return actions;
	}
}
