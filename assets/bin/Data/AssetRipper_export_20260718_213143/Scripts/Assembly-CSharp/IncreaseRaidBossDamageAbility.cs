public class IncreaseRaidBossDamageAbility : ServerAbilityHandler
{
	public override int GetRollValueForUnit(ServerUnitState unit, int defaultValue)
	{
		if (unit.team.otherTeam.type == TeamType.RaidBoss && unit == abilityState.target && (unit.CurrentRollType == DieFaceType.DirectDamage || unit.CurrentRollType == DieFaceType.Initiative || unit.CurrentRollType == DieFaceType.ArmourPiercing || unit.CurrentRollType == DieFaceType.AcidStrike) && unit.team == abilityState.team)
		{
			defaultValue += abilityState.BoostValue;
		}
		return defaultValue;
	}

	public override bool UnitPreFiringEvent(ServerUnitState unit, ServerUnitState target)
	{
		if (unit.team == abilityState.team && unit == abilityState.target)
		{
			if (unit.team.otherTeam.type == TeamType.RaidBoss)
			{
				unit.team.stats.raidBossBuffDamageTotal += abilityState.BoostValue;
			}
			return true;
		}
		return false;
	}

	public override void MatchBegin()
	{
		if (abilityState.target.team.otherTeam.type != TeamType.RaidBoss)
		{
			return;
		}
		foreach (ServerAbilityState ability in abilityState.target.abilities)
		{
			if (ability.metadata.ID != abilityState.metadata.ID)
			{
				ability.addedBoostA += abilityState.BoostValue;
			}
		}
	}
}
