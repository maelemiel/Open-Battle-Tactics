using System;

public class ShortCircuitAbility : ServerAbilityHandler
{
	public override bool UnitFiringEvent(ServerUnitState unit)
	{
		if (abilityState.isActive && unit == abilityState.target && unit.team == abilityState.team && !abilityState.target.IsDead)
		{
			int damage = (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			target = BattleLogic.GetTargetForTeam(abilityState.team, "ShortCircuit-TeamBeginAttackEvent-");
			ApplyDamage(target, damage, abilityState.team, DamageType.Standard);
			target.roundsUntilRerollEnabled++;
			target.preventReroll = true;
			return true;
		}
		return false;
	}
}
