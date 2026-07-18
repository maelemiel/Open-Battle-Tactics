using System;

public class ExtinguishUnitAbility : ServerAbilityHandler
{
	public override bool PreInitiativeEvent()
	{
		return !abilityState.target.IsDead;
	}

	public override bool EndOfRoundWithAbilities()
	{
		if (abilityState.isActive)
		{
			ServerUnitState[] aliveUnits = abilityState.team.aliveUnits;
			foreach (ServerUnitState serverUnitState in aliveUnits)
			{
				int num = (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
				if (serverUnitState.damagePerRound <= num)
				{
					serverUnitState.damagePerRound = 0;
				}
				else
				{
					serverUnitState.damagePerRound -= num;
				}
			}
			return true;
		}
		return false;
	}

	public override bool ActivateOnDeath()
	{
		return true;
	}
}
