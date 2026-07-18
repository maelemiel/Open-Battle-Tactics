using System;

public class IncreaseBaseArmourAbility : ServerAbilityHandler
{
	public override bool TeamBeginAttackEvent(ServerTeamState team)
	{
		bool flag = false;
		if (abilityState.target != null && abilityState.target.IsDead)
		{
			flag = true;
		}
		if (team == abilityState.team.otherTeam && abilityState.isActive && !flag)
		{
			ServerUnitState[] aliveUnits = abilityState.team.aliveUnits;
			foreach (ServerUnitState serverUnitState in aliveUnits)
			{
				serverUnitState.armor += (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			}
			return true;
		}
		return false;
	}

	public override void Deactivate()
	{
		ServerUnitState[] aliveUnits = abilityState.team.aliveUnits;
		foreach (ServerUnitState serverUnitState in aliveUnits)
		{
			serverUnitState.armor -= (int)Math.Floor(abilityState.boostMultiplier * (float)abilityState.BoostValue);
			if (serverUnitState.armor < 0)
			{
				serverUnitState.armor = 0;
			}
		}
	}
}
