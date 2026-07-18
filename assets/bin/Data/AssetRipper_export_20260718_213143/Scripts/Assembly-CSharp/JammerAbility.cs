public class JammerAbility : ServerAbilityHandler
{
	public override bool TeamBeginAttackEvent(ServerTeamState team)
	{
		if (abilityState.isActive)
		{
			bool result = false;
			if (team == abilityState.team.otherTeam)
			{
				ServerAbilityState[] abilities = team.abilities;
				foreach (ServerAbilityState serverAbilityState in abilities)
				{
					if (serverAbilityState.handler.JammedEvent(abilityState, JammerPhase.JAM_OFFENSIVE_ABILITES))
					{
						result = true;
						if (team.battle.animationHandler != null)
						{
							team.battle.animationHandler.JamAbility(serverAbilityState, abilityState);
						}
					}
				}
			}
			else
			{
				ServerAbilityState[] abilities2 = team.otherTeam.abilities;
				foreach (ServerAbilityState serverAbilityState2 in abilities2)
				{
					if (serverAbilityState2.handler.JammedEvent(abilityState, JammerPhase.JAM_DEFENSIVE_ABILITES))
					{
						result = true;
						if (team.battle.animationHandler != null)
						{
							team.battle.animationHandler.JamAbility(serverAbilityState2, abilityState);
						}
					}
				}
			}
			return result;
		}
		return false;
	}
}
