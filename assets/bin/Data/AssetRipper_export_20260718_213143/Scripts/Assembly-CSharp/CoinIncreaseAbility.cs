using System;

public class CoinIncreaseAbility : ServerAbilityHandler
{
	public override bool UnitDiedEvent(ServerUnitState deadUnit, ServerTeamState damageSource)
	{
		int num = 0;
		if (!abilityState.target.IsDead && abilityState.team != deadUnit.team)
		{
			num += (int)Math.Floor((float)deadUnit.destroyCoins * (float)abilityState.BoostValue / 100f);
		}
		deadUnit.extraDestroyCoins += num;
		return num > 0;
	}
}
