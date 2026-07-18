using System;

public class AoeDamageAbility : ServerAbilityHandler
{
	public override bool UnitFiringEvent(ServerUnitState unit)
	{
		if (abilityState.isActive && unit == abilityState.target && unit.team == abilityState.team && !abilityState.target.IsDead)
		{
			int damage = (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			ApplyAOEDamageToTeam(unit.team.otherTeam, damage, abilityState.team);
			return true;
		}
		return false;
	}
}
