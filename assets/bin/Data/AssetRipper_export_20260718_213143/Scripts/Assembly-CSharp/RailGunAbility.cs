using System;

public class RailGunAbility : ServerAbilityHandler
{
	public bool shouldFire;

	public bool shouldDeploy = true;

	public override bool UnitFiringEvent(ServerUnitState unit)
	{
		if (abilityState.isActive && unit == abilityState.target && unit.team == abilityState.team && !abilityState.target.IsDead)
		{
			if (shouldFire)
			{
				shouldDeploy = false;
				target = BattleLogic.GetTargetForTeam(abilityState.team, "Railgun-TeamBeginAttackEvent-");
				int damage = (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
				ApplyDamage(target, damage, abilityState.team, DamageType.Standard);
			}
			else
			{
				shouldDeploy = true;
				shouldFire = true;
			}
			return true;
		}
		return false;
	}
}
