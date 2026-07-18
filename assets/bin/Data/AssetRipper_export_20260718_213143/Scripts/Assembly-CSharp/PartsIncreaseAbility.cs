public class PartsIncreaseAbility : ServerAbilityHandler
{
	public override bool UnitDiedEvent(ServerUnitState deadUnit, ServerTeamState damageSource)
	{
		bool result = false;
		if (!abilityState.target.IsDead && abilityState.team != deadUnit.team && abilityState.BoostValue > 0)
		{
			result = true;
			deadUnit.extraDestroyPartDropChance += abilityState.BoostValue;
		}
		return result;
	}
}
