using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAnimationHandler : IBattleAnimationHandler
{
	private class AnimSequence
	{
		public IEnumerator coroutine;

		public bool blocking;
	}

	private static bool VERBOSE_LOG;

	private BattleController battleController;

	private List<AnimSequence> animSequences;

	private bool isRunning;

	public bool InDecisionPhase
	{
		get
		{
			return battleController.phaseManager.currentPhase == Phase.DECISION;
		}
	}

	public BattleAnimationHandler(BattleController battleController)
	{
		this.battleController = battleController;
		animSequences = new List<AnimSequence>();
	}

	public void AddSequence(IEnumerator coroutine, bool blocking = true)
	{
		if (coroutine != null)
		{
			if (VERBOSE_LOG)
			{
				Log.Info(string.Concat("AddSequence ", coroutine, "  blocking=", blocking));
			}
			animSequences.Add(new AnimSequence
			{
				coroutine = coroutine,
				blocking = blocking
			});
			isRunning = true;
		}
	}

	public void AddImmediateSequence(IEnumerator coroutine, bool blocking = true)
	{
		if (coroutine != null)
		{
			if (VERBOSE_LOG)
			{
				Log.Info(string.Concat("AddImmediateSequence ", coroutine, "  blocking=", blocking));
			}
			animSequences.Insert(0, new AnimSequence
			{
				coroutine = coroutine,
				blocking = blocking
			});
			isRunning = true;
		}
	}

	public IEnumerator RunSequences()
	{
		while (true)
		{
			if (animSequences.Count > 0)
			{
				isRunning = true;
				AnimSequence animSequence = animSequences[0];
				animSequences.RemoveAt(0);
				Coroutine coroutine = battleController.StartCoroutine(animSequence.coroutine);
				if (animSequence.blocking)
				{
					yield return coroutine;
				}
			}
			else
			{
				isRunning = false;
				yield return 0;
			}
		}
	}

	public IEnumerator WaitForSequenceComplete()
	{
		while (isRunning)
		{
			yield return 0;
		}
	}

	public void RoundStart(int roundIndex)
	{
	}

	public void RoundComplete(int roundIndex)
	{
	}

	public void PreInitiativeAbility(ServerAbilityState _ability)
	{
		AbilityState abilityState = _ability as AbilityState;
		bool blocking = true;
		if (abilityState.handler.GetType() == typeof(IntelAbility))
		{
			blocking = false;
		}
		AddSequence(abilityState.animationHandler.PreInitiativeAnimation(), blocking);
	}

	public void PreInitiativeResultsAbility(ServerAbilityState _ability, int intelBoostTeam, int intelBoostOtherTeam)
	{
		AbilityState abilityState = _ability as AbilityState;
		bool blocking = true;
		if (abilityState.handler.GetType() == typeof(IntelAbility))
		{
			blocking = false;
		}
		AddSequence(abilityState.animationHandler.PreInitiativeResultsAbility(intelBoostTeam, intelBoostOtherTeam), blocking);
	}

	public void InitiativeResults(ServerTeamState winningTeam, int hostTeamInitiative, int guestTeamInitiative)
	{
		AddSequence(battleController.initiativePhase.InitiativeSequence(winningTeam, hostTeamInitiative, guestTeamInitiative));
	}

	public void PostInitiativeAbility(ServerAbilityState _ability)
	{
		AbilityState abilityState = _ability as AbilityState;
		AddSequence(abilityState.animationHandler.PostInitiativeAnimation());
	}

	public void TeamBeginAttack(ServerTeamState team)
	{
		AddSequence(battleController.resolutionPhase.TeamBeginAttack(team as TeamState));
	}

	public void TeamBeginAttackAbility(ServerTeamState team, ServerAbilityState _ability)
	{
		AddSequence(battleController.resolutionPhase.TeamBeginAttackAbility(team as TeamState, _ability as AbilityState));
	}

	public void PassiveFiringAnimation(ServerUnitState _unit, ServerAbilityState _ability)
	{
		AbilityState abilityState = _ability as AbilityState;
		AddSequence(abilityState.animationHandler.PassiveFiringAnimation(_unit as UnitState));
	}

	public void UnitFiringAbility(ServerUnitState _unit, ServerAbilityState _ability, ServerUnitState _target)
	{
		AbilityState abilityState = _ability as AbilityState;
		AddSequence(abilityState.animationHandler.UnitFiringAnimation(_unit as UnitState, _target as UnitState));
	}

	public void UnitAttack(ServerUnitState _unit, ServerUnitState _target, int damage, DamageType damageType)
	{
		AddSequence(battleController.resolutionPhase.UnitAttack(_unit as UnitState, _target as UnitState, damage, damageType, _unit.currentRoll));
	}

	public void UnitReceivedDamageAbility(ServerUnitState _unit, int damage, DamageType dmgType, ServerAbilityState _ability)
	{
		AbilityState abilityState = _ability as AbilityState;
		AddSequence(abilityState.animationHandler.UnitReceivedDamageAnimation(_unit as UnitState));
	}

	public void UnitDiedAbility(ServerUnitState _unit, ServerTeamState _team, ServerAbilityState _ability)
	{
		AbilityState abilityState = _ability as AbilityState;
		AddSequence(abilityState.animationHandler.UnitDiedAnimation(_unit as UnitState));
	}

	public void EndOfRoundWithAbilities(ServerTeamState _team, ServerAbilityState _ability)
	{
		AbilityState abilityState = _ability as AbilityState;
		AddSequence(abilityState.animationHandler.EndOfRoundWithAbilities(_team as TeamState));
	}

	public void TeamEndAttack(ServerTeamState team)
	{
	}

	public void PreFireAbility(ServerAbilityState _ability, ServerUnitState unit, ServerUnitState target)
	{
		AbilityState abilityState = _ability as AbilityState;
		AddSequence(abilityState.animationHandler.PreFiringAnimation(unit, target));
	}

	public void PreActivateAbility(ServerAbilityState _ability, ServerUnitState _target)
	{
		AbilityState abilityState = _ability as AbilityState;
		UnitState target = _target as UnitState;
		AddSequence(abilityState.animationHandler.PreActivationAnimation(target));
	}

	public void ActivateAbility(ServerAbilityState _ability, ServerUnitState _target)
	{
		AbilityState abilityState = _ability as AbilityState;
		UnitState target = _target as UnitState;
		AddSequence(abilityState.animationHandler.ActivationAnimation(target));
		if (InDecisionPhase)
		{
			AddSequence(abilityState.animationHandler.PreviewActivationAnimation(target));
		}
		AddSequence(battleController.battleHooks.OnAbilityActivated(abilityState, target));
	}

	public void DeactivateAbility(ServerAbilityState _ability)
	{
		AbilityState abilityState = _ability as AbilityState;
		AddSequence(abilityState.animationHandler.DeactivationAnimation(), !InDecisionPhase);
		if (InDecisionPhase)
		{
			AddSequence(abilityState.animationHandler.PreviewDeactivationAnimation());
		}
		AddSequence(battleController.battleHooks.OnAbilityDeactivated(abilityState));
	}

	public void ReRoll(ServerAbilityState ability, int index)
	{
		UnitState unitState = (UnitState)ability.target;
		AddSequence(unitState.unitView.RollDice(index, true));
	}

	public void QuickDraw(ServerAbilityState ability)
	{
		QuickDrawAnimationHandler quickDrawAnimationHandler = ability.animHandler as QuickDrawAnimationHandler;
		AddSequence(quickDrawAnimationHandler.DisplayQuickDrawEffect());
	}

	public void JamAbility(ServerAbilityState jammedAbility, ServerAbilityState jammerAbility)
	{
		AbilityState abilityState = jammedAbility as AbilityState;
		AddSequence(abilityState.animationHandler.JammedAnimation(jammerAbility), false);
	}

	public void InvestAbilityEnergy(ServerAbilityState _ability)
	{
		AbilityState abilityState = _ability as AbilityState;
		AddSequence(battleController.battleHooks.OnAbilityInvested(abilityState));
	}

	public void FinishBattle(ServerTeamState winningTeam)
	{
	}

	public void TeamForfeit(ServerTeamState team)
	{
		AddSequence(battleController.resolutionPhase.TeamForfeit(team as TeamState));
	}

	public void TeamWorstRoll(ServerTeamState team)
	{
		int[] array = new int[team.aliveUnits.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = team.aliveUnits[i].currentRoll;
		}
		AddSequence(battleController.resolutionPhase.TeamWorstRoll(team as TeamState, array));
	}

	public void TeamBestRoll(ServerTeamState team)
	{
		AddSequence(battleController.resolutionPhase.TeamBestRoll(team as TeamState));
	}

	public void UnitWorstRolls(ServerUnitState unit)
	{
		AddSequence(battleController.resolutionPhase.UnitWorstRolls(unit as UnitState));
	}

	public void UnitAcidDamage(ServerUnitState unit, int acidIncrease)
	{
		Log.DebugTag("INCREASING ACID!", null, "ACIDSTRIKE");
		AddSequence(battleController.resolutionPhase.UnitAcidDamage(unit as UnitState, acidIncrease));
	}

	public void ApplyDamagePerRound()
	{
		AddSequence(battleController.resolutionPhase.ApplyDamagePerRound(battleController.battleState.initiativeWinner.otherTeam, 1f));
		AddSequence(battleController.resolutionPhase.ApplyDamagePerRound(battleController.battleState.initiativeWinner, 0f));
	}
}
