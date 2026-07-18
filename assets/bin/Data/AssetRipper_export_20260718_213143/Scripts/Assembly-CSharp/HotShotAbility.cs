using System;

public class HotShotAbility : ServerAbilityHandler
{
	public override bool UnitFiringEvent(ServerUnitState unit)
	{
		if (abilityState.isActive && unit == abilityState.target && unit.team == abilityState.team && !abilityState.target.IsDead)
		{
			target = BattleLogic.GetTargetForTeam(abilityState.team, "HotShot-TeamBeginAttackEvent-");
			int damage = (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			ApplyDamage(target, damage, abilityState.team, DamageType.Standard);
			target.damagePerRound += abilityState.SecondaryBoostValue;
			return true;
		}
		return false;
	}
}
