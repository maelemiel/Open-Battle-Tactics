using System;

public class MinigunUnitAbility : ServerUnitAbilityHandler
{
	public int extraShots;

	public override bool UnitFiringEvent(ServerUnitState unit)
	{
		if (unit == abilityState.target && unit.team == abilityState.team && abilityState.isActive && base.IsOwnerUnitAlive)
		{
			target = BattleLogic.GetTargetForTeam(unit.team, "Minigun-TeamBeginAttackEvent-");
			int damage = (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			ApplyDamage(target, damage, abilityState.team, DamageType.Standard);
			int secondaryBoostValue = abilityState.SecondaryBoostValue;
			for (int i = 0; i < extraShots; i++)
			{
				ApplyDamage(target, secondaryBoostValue, abilityState.team, DamageType.Standard);
			}
			extraShots++;
			return true;
		}
		return false;
	}
}
