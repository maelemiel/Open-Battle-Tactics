using System.Collections;

public class BattleHooks
{
	protected BattleController battleController;

	public virtual void Init(BattleController battleController)
	{
		this.battleController = battleController;
	}

	public virtual bool CanRevive()
	{
		return false;
	}

	public virtual IEnumerator Revive()
	{
		yield break;
	}

	public virtual IEnumerator PreIntroPlayerRollIn()
	{
		yield break;
	}

	public virtual IEnumerator PostIntroPlayerRollIn()
	{
		yield break;
	}

	public virtual IEnumerator PreIntroEnemyRollIn()
	{
		yield break;
	}

	public virtual IEnumerator PostIntroEnemyRollIn()
	{
		yield break;
	}

	public virtual IEnumerator OnExitIntroPhase()
	{
		yield break;
	}

	public virtual IEnumerator OnEnterDecisionPhase()
	{
		yield break;
	}

	public virtual IEnumerator OnEnterResolutionPhase()
	{
		yield break;
	}

	public virtual IEnumerator OutroAnimation()
	{
		yield break;
	}

	public virtual bool OverrideUnitDeath(UnitState unit)
	{
		return false;
	}

	public virtual IEnumerator OnUnitDeath(UnitView unitView)
	{
		yield break;
	}

	public virtual void OnPostReroll()
	{
	}

	public virtual IEnumerator OnAbilityActivated(AbilityState abilityState, UnitState target)
	{
		yield break;
	}

	public virtual IEnumerator OnAbilityInvested(AbilityState abilityState)
	{
		yield break;
	}

	public virtual IEnumerator OnAbilityDeactivated(AbilityState abilityState)
	{
		yield break;
	}

	public virtual IEnumerator OnRestartBattle()
	{
		yield break;
	}

	public virtual bool PlayerSpinAllDice()
	{
		return true;
	}

	public virtual void ModifyTeamStep(TeamState team)
	{
	}

	public virtual void ModifyPreBeginBattleStep()
	{
	}

	public virtual void ModifyBeginBattleStep()
	{
	}

	public virtual void ModifyRoundStep()
	{
	}

	public virtual bool StopMusicOnBattleComplete()
	{
		return true;
	}

	public virtual bool ShouldRestartBattle()
	{
		return false;
	}

	public virtual bool OnClickBattle()
	{
		return true;
	}

	public virtual bool OnClickCancel()
	{
		return true;
	}

	public virtual bool OnTapUnit(UnitView unit)
	{
		return true;
	}

	public virtual bool OnInvalidTapUnit(UnitView unit)
	{
		return true;
	}

	public virtual bool OnTargetUnit(UnitView unit)
	{
		return true;
	}

	public virtual bool OnTapAbility(AbilityState abilityState)
	{
		return true;
	}
}
