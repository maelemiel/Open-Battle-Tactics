using System;

public class NapalmAbility : ServerAbilityHandler
{
	public override bool UnitFiringEvent(ServerUnitState unit)
	{
		if (abilityState.isActive && unit == abilityState.target && unit.team == abilityState.team && !abilityState.target.IsDead)
		{
			int damage = (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			ApplyAOEDamageAndDPRToTeam(unit.team.otherTeam, damage, abilityState.SecondaryBoostValue, abilityState.team);
			return true;
		}
		return false;
	}
}
