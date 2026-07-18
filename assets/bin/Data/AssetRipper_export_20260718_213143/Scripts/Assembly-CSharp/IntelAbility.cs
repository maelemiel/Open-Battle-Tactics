public class IntelAbility : ServerAbilityHandler
{
	public override void Activate(ServerUnitState target)
	{
		base.Activate(target);
		int nextTeamRandom = BattleLogic.GetNextTeamRandom(abilityState.team, abilityState.BoostValue, abilityState.SecondaryBoostValue, "IntelAbility-Activate");
		abilityState.team.intelBoost = nextTeamRandom;
	}

	public override bool PreInitiativeEvent()
	{
		if (abilityState.isActive)
		{
			return true;
		}
		return false;
	}

	public override void Deactivate()
	{
		abilityState.team.intelBoost = 0;
	}
}
