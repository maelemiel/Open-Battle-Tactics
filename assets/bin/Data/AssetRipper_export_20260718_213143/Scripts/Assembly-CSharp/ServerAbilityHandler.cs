public class ServerAbilityHandler
{
	public ServerAbilityState abilityState;

	public ServerUnitState target;

	public virtual void Init(ServerAbilityState abilityState)
	{
		this.abilityState = abilityState;
	}

	protected int ApplyDamageInRange(ServerUnitState target, int damageMin, int damageMax, ServerTeamState damageSource, DamageType dmgType)
	{
		int num = BattleLogic.ApplyDamageInRange(target, damageMin, damageMax, damageSource, dmgType);
		if (abilityState.animHandler != null)
		{
			abilityState.animHandler.PushDamageValue(num);
		}
		return num;
	}

	protected void ApplyAOEDamageToTeam(ServerTeamState team, int damage, ServerTeamState damageSource)
	{
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState in aliveUnits)
		{
			ApplyDamage(serverUnitState, damage, damageSource, DamageType.AOE);
		}
	}

	protected void ApplyDamagePerRoundToTeam(ServerTeamState team, int damagePerRound, ServerTeamState damageSource)
	{
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState in aliveUnits)
		{
			serverUnitState.damagePerRound += damagePerRound;
		}
	}

	protected void ApplyAOEDamageAndDPRToTeam(ServerTeamState team, int damage, int damagePerRound, ServerTeamState damageSource)
	{
		ServerUnitState[] aliveUnits = team.aliveUnits;
		foreach (ServerUnitState serverUnitState in aliveUnits)
		{
			int num = ApplyDamage(serverUnitState, damage, damageSource, DamageType.AOE);
			if (num > 0)
			{
				serverUnitState.damagePerRound += damagePerRound;
			}
		}
	}

	protected int ApplyDamage(ServerUnitState target, int damage, ServerTeamState damageSource, DamageType dmgType)
	{
		int num = BattleLogic.ApplyDamage(target, damage, damageSource, dmgType);
		if (abilityState.animHandler != null)
		{
			abilityState.animHandler.PushDamageValue(num);
		}
		return num;
	}

	public virtual string GetName()
	{
		return string.Format(abilityState.metadata.Name, abilityState.BoostValue, abilityState.SecondaryBoostValue);
	}

	public virtual void MatchBegin()
	{
	}

	public virtual void Activate(ServerUnitState target)
	{
	}

	public virtual void Deactivate()
	{
	}

	public virtual void Destroy()
	{
	}

	public virtual bool ActivateOnDeath()
	{
		return false;
	}

	public virtual bool PreInitiativeEvent()
	{
		return false;
	}

	public virtual bool PostInitiativeEvent()
	{
		return false;
	}

	public virtual bool TeamBeginAttackEvent(ServerTeamState team)
	{
		return false;
	}

	public virtual bool UnitFiringEvent(ServerUnitState unit)
	{
		return false;
	}

	public virtual bool UnitPreFiringEvent(ServerUnitState unit, ServerUnitState target)
	{
		return false;
	}

	public virtual bool UnitAttackEvent(ServerUnitState unit, ServerUnitState target, int damage)
	{
		return false;
	}

	public virtual bool UnitReceivedDamageEvent(ServerUnitState unit, int damage, DamageType dmgType)
	{
		return false;
	}

	public virtual bool EndOfRoundWithAbilities()
	{
		return false;
	}

	public virtual bool UnitDiedEvent(ServerUnitState deadUnit, ServerTeamState damageSource)
	{
		return false;
	}

	public virtual bool JammedEvent(ServerAbilityState jammerAbility, JammerPhase jammerPhase)
	{
		return false;
	}

	public virtual ServerUnitState GetTargetForTeam(ServerTeamState team, ServerUnitState defaultTarget)
	{
		return defaultTarget;
	}

	public virtual int GetRollValueForUnit(ServerUnitState state, int defaultRollValue)
	{
		return defaultRollValue;
	}

	public virtual int GetDamageForUnitTarget(ServerUnitState unit, int defaultValue)
	{
		return defaultValue;
	}
}
