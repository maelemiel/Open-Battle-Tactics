using System;

public class EventPointBoostPassiveAbility : ServerAbilityHandler
{
	public override bool UnitDiedEvent(ServerUnitState deadUnit, ServerTeamState damageSource)
	{
		int num = 0;
		if (!abilityState.target.IsDead && abilityState.team != deadUnit.team)
		{
			float num2 = (float)abilityState.BoostValue / 100f;
			num = (int)Math.Floor((float)deadUnit.metadata.DestroyEventPoints * num2);
			deadUnit.extraDestroyEventPoints += num;
			return true;
		}
		return false;
	}
}
