using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityAnimationHandler : IAbilityAnimationHandler
{
	protected AbilityState abilityState;

	protected ServerAbilityHandler handler;

	protected BattleController battleController;

	private List<int> lastDamageValues;

	public int DamageValuesCount
	{
		get
		{
			return lastDamageValues.Count;
		}
	}

	public virtual string InBattleName
	{
		get
		{
			return string.Empty;
		}
	}

	public virtual void Init(BattleController battleController, AbilityState abilityState)
	{
		this.battleController = battleController;
		this.abilityState = abilityState;
		handler = abilityState.handler;
		this.abilityState.animHandler = this;
		lastDamageValues = new List<int>();
		LoadDependencies();
	}

	public void PushDamageValue(int damage)
	{
		lastDamageValues.Add(damage);
	}

	public int GetNextDamage()
	{
		if (lastDamageValues.Count == 0)
		{
			Debug.LogError("AbilityAnimation ran out of saved damage values! Desync!");
			return 0;
		}
		int result = lastDamageValues[0];
		lastDamageValues.RemoveAt(0);
		return result;
	}

	protected virtual void LoadDependencies()
	{
	}

	public virtual IEnumerator OntoFieldAnimation()
	{
		return null;
	}

	public virtual IEnumerator PreActivationAnimation(UnitState target)
	{
		return null;
	}

	public virtual IEnumerator ActivationAnimation(UnitState target)
	{
		return null;
	}

	public virtual IEnumerator PreviewActivationAnimation(UnitState target)
	{
		return null;
	}

	public virtual IEnumerator DeactivationAnimation()
	{
		return null;
	}

	public virtual IEnumerator PreviewDeactivationAnimation()
	{
		return null;
	}

	public virtual IEnumerator DestroyAnimation()
	{
		yield break;
	}

	public virtual IEnumerator PreFiringAnimation(ServerUnitState unit, ServerUnitState target)
	{
		return null;
	}

	public virtual IEnumerator PreInitiativeAnimation()
	{
		return null;
	}

	public virtual IEnumerator PreInitiativeResultsAbility(int intelBoostTeam, int intelBoostOtherTeam)
	{
		return null;
	}

	public virtual IEnumerator PostInitiativeAnimation()
	{
		return null;
	}

	public virtual IEnumerator TeamBeginAttackAnimation(TeamState team)
	{
		return null;
	}

	public virtual IEnumerator PassiveFiringAnimation(UnitState unit)
	{
		return null;
	}

	public virtual IEnumerator UnitFiringAnimation(UnitState unit, UnitState target)
	{
		return null;
	}

	public virtual IEnumerator UnitReceivedDamageAnimation(UnitState unit)
	{
		return null;
	}

	public virtual IEnumerator UnitDiedAnimation(UnitState unit)
	{
		return null;
	}

	public virtual IEnumerator JammedAnimation(ServerAbilityState jammerAnimation)
	{
		return null;
	}

	public virtual IEnumerator OnRollFinished()
	{
		return null;
	}

	public virtual IEnumerator EndOfRoundWithAbilities(TeamState team)
	{
		return null;
	}

	protected Coroutine StartCoroutine(IEnumerator enumerator)
	{
		return battleController.StartCoroutine(enumerator);
	}
}
