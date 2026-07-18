public class EvadeAOEAbility : ServerAbilityHandler
{
	public override bool UnitReceivedDamageEvent(ServerUnitState unit, int damage, DamageType dmgType)
	{
		if (!abilityState.target.IsDead && abilityState.target == unit && dmgType == DamageType.AOE)
		{
			int nextTeamRandom = BattleLogic.GetNextTeamRandom(abilityState.target.team, 0, 100, "EvadeAOE-UnitReceivedDamage");
			abilityState.target.evadeNextAOE = nextTeamRandom <= abilityState.BoostValue;
			return abilityState.target.evadeNextAOE;
		}
		return false;
	}
}
