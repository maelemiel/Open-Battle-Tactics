using System.Collections.Generic;

public class SimpleAI : BaseAI
{
	private const int THRESHOLD_REROLL = 1;

	private const int THRESHOLD_SACRIFICE_UNITS = 3;

	private const int THRESHOLD_SACRIFICE_HP = 2;

	private const int THRESHOLD_JAMMER = 3;

	private const float PROBABILITY_LOW = 0.1f;

	private const float PROBABILITY_MED = 0.25f;

	private const float PROBABILITY_HIGH = 0.5f;

	private const float PROBABILITY_ALWAYS = 1f;

	protected List<BattleAction> actions;

	private int unitsRemaining;

	protected AIState currentState;

	public override List<BattleAction> Think(ServerTeamState team)
	{
		team = CloneState(team);
		actions = new List<BattleAction>();
		DecideAIState(team);
		if (unitsRemaining >= 3)
		{
			TrySelfDestruct(team, 1f);
		}
		if (unitsRemaining == 1)
		{
			TryDrawfire(team, 1f);
		}
		TryReroll(team, 1f);
		TryJammer(team, (unitsRemaining < 3) ? 0.25f : 0.5f);
		TryJammer2(team, (unitsRemaining < 3) ? 0.25f : 0.5f);
		TryTarget(team, 0.5f);
		TryTarget2(team, 0.5f);
		if (unitsRemaining > 1)
		{
			TryDrawfire(team, 0.5f);
		}
		TryMiniStrike(team, 0.5f);
		TryIonStrike(team, 0.5f);
		TryArmorAll(team, 0.25f);
		TryEMPulse(team, 0.5f);
		TryIntel(team, 0.5f);
		TryFirebomb(team, 1f);
		TryBarrage(team, 1f);
		TrySendEmoticon(team, 0.25f);
		return actions;
	}

	protected void DecideAIState(ServerTeamState team)
	{
		unitsRemaining = team.aliveUnits.Length;
		if (unitsRemaining < team.otherTeam.aliveUnits.Length)
		{
			currentState = AIState.LOSING;
		}
		else if (unitsRemaining == team.otherTeam.aliveUnits.Length)
		{
			currentState = AIState.TIED;
		}
		else if (unitsRemaining > team.otherTeam.aliveUnits.Length)
		{
			currentState = AIState.WINNING;
		}
		else
		{
			currentState = AIState.IDLE;
		}
	}

	public override bool ShouldUnitTriggerSpecial(ServerUnitState unit)
	{
		bool result = false;
		switch (unit.abilities[0].metadata.Type)
		{
		default:
			return result;
		}
	}

	protected void TryReroll(ServerTeamState team, float chance)
	{
		TryRerollThreshold(team, chance, 1);
	}

	protected void TryRerollThreshold(ServerTeamState team, float chance, int threshold)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("reroll");
		if (abilityByType == null || !BaseAI.RandomChance(team, chance))
		{
			return;
		}
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState in aliveUnits)
		{
			if (!serverUnitState.preventReroll && serverUnitState.currentRoll <= threshold && abilityByType.CanActivate)
			{
				InvestEnergyToActivateAbility(abilityByType, serverUnitState, actions);
			}
		}
	}

	protected void TryTarget(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("target");
		if (abilityByType == null)
		{
			return;
		}
		bool flag = team.otherTeam.aliveUnits.Length > 1;
		if (BaseAI.RandomChance(team, chance) && abilityByType.CanActivate && flag)
		{
			ServerUnitState randomOpponentUnit = BaseAI.GetRandomOpponentUnit(team);
			switch (currentState)
			{
			case AIState.LOSING:
				randomOpponentUnit = BaseAI.GetWeakestOpponentUnit(team);
				break;
			case AIState.WINNING:
				randomOpponentUnit = ((!BaseAI.RandomChance(team, 0.25f)) ? BaseAI.GetStrongestOpponentUnit(team) : BaseAI.GetWeakestOpponentUnit(team));
				break;
			default:
				randomOpponentUnit = ((!BaseAI.RandomChance(team, 0.5f)) ? BaseAI.GetRandomOpponentUnit(team) : BaseAI.GetWeakestOpponentUnit(team));
				break;
			}
			if (randomOpponentUnit != null)
			{
				InvestEnergyToActivateAbility(abilityByType, randomOpponentUnit, actions);
			}
		}
	}

	protected void TryTarget2(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("target_advance");
		if (abilityByType == null)
		{
			return;
		}
		bool flag = team.otherTeam.aliveUnits.Length > 1;
		if (BaseAI.RandomChance(team, chance) && abilityByType.CanActivate && flag)
		{
			ServerUnitState targetingTarget = GetTargetingTarget(team);
			if (targetingTarget != null)
			{
				InvestEnergyToActivateAbility(abilityByType, targetingTarget, actions);
			}
		}
	}

	protected ServerUnitState GetTargetingTarget(ServerTeamState team)
	{
		switch (currentState)
		{
		case AIState.LOSING:
			return BaseAI.GetWeakestOpponentUnit(team);
		case AIState.WINNING:
			if (BaseAI.RandomChance(team, 0.25f))
			{
				return BaseAI.GetWeakestOpponentUnit(team);
			}
			return BaseAI.GetStrongestOpponentUnit(team);
		default:
			if (BaseAI.RandomChance(team, 0.5f))
			{
				return BaseAI.GetWeakestOpponentUnit(team);
			}
			return BaseAI.GetRandomOpponentUnit(team);
		}
	}

	protected void TryAdvTarget(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("target_advance");
		if (abilityByType == null)
		{
			return;
		}
		bool flag = team.otherTeam.aliveUnits.Length > 1;
		if (!BaseAI.RandomChance(team, chance) || !abilityByType.CanActivate || !flag)
		{
			return;
		}
		ServerUnitState weakestOpponentUnit;
		switch (currentState)
		{
		case AIState.LOSING:
			weakestOpponentUnit = BaseAI.GetWeakestOpponentUnit(team);
			break;
		case AIState.WINNING:
			if (BaseAI.RandomChance(team, 0.5f))
			{
				weakestOpponentUnit = BaseAI.GetStrongestOpponentUnit(team);
			}
			else
			{
				weakestOpponentUnit = BaseAI.GetWeakestOpponentUnit(team);
			}
			break;
		default:
			if (BaseAI.RandomChance(team, 0.25f))
			{
				weakestOpponentUnit = BaseAI.GetWeakestOpponentUnit(team);
			}
			else
			{
				weakestOpponentUnit = BaseAI.GetStrongestOpponentUnit(team);
			}
			break;
		}
		weakestOpponentUnit = BaseAI.GetRandomOpponentUnit(team);
		if (weakestOpponentUnit != null)
		{
			InvestEnergyToActivateAbility(abilityByType, weakestOpponentUnit, actions);
		}
	}

	protected void TryEMPulse(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("em_pulse");
		if (abilityByType == null)
		{
			return;
		}
		bool flag = team.otherTeam.aliveUnits.Length > 1;
		if (BaseAI.RandomChance(team, chance) && abilityByType.CanActivate && flag)
		{
			ServerUnitState serverUnitState = ((!BaseAI.RandomChance(team, 0.5f)) ? BaseAI.GetRandomOpponentUnit(team) : BaseAI.GetWeakestOpponentUnit(team));
			if (serverUnitState != null)
			{
				InvestEnergyToActivateAbility(abilityByType, serverUnitState, actions);
			}
		}
	}

	protected void TryDrawfire(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("drawfire");
		if (abilityByType != null)
		{
			ServerUnitState strongestUnit = BaseAI.GetStrongestUnit(team);
			if (abilityByType.CanActivate && BaseAI.RandomChance(team, chance) && strongestUnit != null)
			{
				InvestEnergyToActivateAbility(abilityByType, strongestUnit, actions);
			}
		}
	}

	protected void TryArmorAll(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("armour_buff");
		if (abilityByType != null && BaseAI.RandomChance(team, chance))
		{
			InvestEnergyToActivateAbility(abilityByType, null, actions);
		}
	}

	protected void TryBarrage(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("barrage");
		if (abilityByType != null && BaseAI.RandomChance(team, chance))
		{
			InvestEnergyToActivateAbility(abilityByType, null, actions);
		}
	}

	protected void TryFirebomb(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("firebomb");
		if (abilityByType != null && BaseAI.RandomChance(team, chance))
		{
			InvestEnergyToActivateAbility(abilityByType, null, actions);
		}
	}

	protected void TryIntel(ServerTeamState team, float chance)
	{
		Log.Debug("Calling Intel with a chance of: " + chance);
		ServerAbilityState abilityByType = team.GetAbilityByType("intel");
		if (abilityByType != null && BaseAI.RandomChance(team, chance))
		{
			InvestEnergyToActivateAbility(abilityByType, null, actions);
		}
	}

	protected void TryIonStrike(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("ion_strike");
		if (abilityByType != null && BaseAI.RandomChance(team, chance))
		{
			ServerUnitState serverUnitState = null;
			serverUnitState = ((!BaseAI.RandomChance(team, 0.5f)) ? BaseAI.GetRandomOpponentUnit(team) : BaseAI.GetWeakestOpponentUnit(team));
			InvestEnergyToActivateAbility(abilityByType, serverUnitState, actions);
		}
	}

	protected void TryMiniStrike(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("small_ion_strike");
		if (abilityByType != null && BaseAI.RandomChance(team, chance))
		{
			ServerUnitState serverUnitState = null;
			serverUnitState = ((!BaseAI.RandomChance(team, 0.5f)) ? BaseAI.GetRandomOpponentUnit(team) : BaseAI.GetWeakestOpponentUnit(team));
			InvestEnergyToActivateAbility(abilityByType, serverUnitState, actions);
		}
	}

	protected void TryJammer(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("jammer");
		if (abilityByType == null)
		{
			return;
		}
		bool flag = team.aliveUnits.Length > 1;
		if (BaseAI.RandomChance(team, chance) && abilityByType.CanActivate && flag)
		{
			ServerUnitState randomPlayerUnit = BaseAI.GetRandomPlayerUnit(team);
			if (randomPlayerUnit != null && BaseAI.RandomChance(team, chance))
			{
				InvestEnergyToActivateAbility(abilityByType, randomPlayerUnit, actions);
			}
		}
	}

	protected void TryJammer2(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("jammer2");
		if (abilityByType == null)
		{
			return;
		}
		bool flag = team.aliveUnits.Length > 1;
		if (BaseAI.RandomChance(team, chance) && abilityByType.CanActivate && flag)
		{
			ServerUnitState randomPlayerUnit = BaseAI.GetRandomPlayerUnit(team);
			if (randomPlayerUnit != null && BaseAI.RandomChance(team, chance))
			{
				InvestEnergyToActivateAbility(abilityByType, randomPlayerUnit, actions);
			}
		}
	}

	protected void TrySelfDestruct(ServerTeamState team, float chance)
	{
		ServerAbilityState abilityByType = team.GetAbilityByType("self_destruct");
		if (abilityByType != null)
		{
			ServerUnitState weakestUnit = BaseAI.GetWeakestUnit(team);
			if (abilityByType.CanActivate && weakestUnit != null && weakestUnit.hp <= 2 && BaseAI.RandomChance(team, chance))
			{
				InvestEnergyToActivateAbility(abilityByType, weakestUnit, actions);
			}
		}
	}

	protected void TrySendEmoticon(ServerTeamState team, float chance)
	{
		if (BaseAI.RandomChance(team, chance))
		{
			EmoticonTypes emoticonTypes = EmoticonTypes.NONE;
			switch (currentState)
			{
			case AIState.WINNING:
				emoticonTypes = EmoticonTypes.HAPPY;
				break;
			case AIState.LOSING:
				emoticonTypes = EmoticonTypes.SAD;
				break;
			default:
				emoticonTypes = EmoticonTypes.ANGRY;
				break;
			}
			EmoticonAction item = EmoticonAction.Create(emoticonTypes.ToString());
			actions.Add(item);
		}
	}
}
