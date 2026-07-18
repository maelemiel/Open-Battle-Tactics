using System.Collections;
using UnityEngine;

public class ResolutionPhase : AbstractBattlePhase
{
	public override void OnEnterPhase()
	{
		StartCoroutine(BattleSequence());
	}

	private IEnumerator BattleSequence()
	{
		MatchData matchData = battleController.matchManager.MatchData;
		ServerBattleState battleState = battleController.battleState;
		if (battleController.battleHooks != null)
		{
			StartCoroutine(battleController.battleHooks.OnEnterResolutionPhase());
		}
		if (battleState.teamOne.forfeited || battleState.teamTwo.forfeited)
		{
			yield return StartCoroutine(ForfeitBattleSequence());
		}
		else
		{
			yield return StartCoroutine(NormalBattleSequence());
		}
		BattleLogic.SimulateRound(battleController.battleState);
		Reporting.RoundCompleteEvent(battleController.matchManager, battleState.currentRound, battleController.playerTeam, battleState, BattleLogic.GetInitiativeForTeam(battleController.playerTeam));
		if (matchData.opponentTeam.type != TeamType.Player)
		{
			Reporting.RoundCompleteAIEvent(matchData.matchId, battleState.currentRound, battleController.matchManager.enemyActions, battleState.initiativeWinner == battleState.teamTwo, BattleLogic.GetInitiativeForTeam(battleController.enemyTeam), battleState.teamTwo);
		}
		yield return StartCoroutine(battleController.animationHandler.WaitForSequenceComplete());
		yield return StartCoroutine(PostRoundSequence());
		FinishRound();
	}

	private IEnumerator NormalBattleSequence()
	{
		battleController.CubeBar.GoToTextState("ui_battle_roundstart".Localize("Battle!"));
		foreach (UnitView unit in battleController.AllUnits)
		{
			unit.PossibleRollsSimple.CloseDieBox(0.2f, true);
		}
		yield return new WaitForSeconds(0.2f);
		yield return StartCoroutine(RevealEnemyDice());
	}

	private IEnumerator PostRoundSequence()
	{
		foreach (UnitView unit in battleController.AllUnits)
		{
			unit.HasAttacked = false;
			unit.BuffEffects.ResetBuffEffects();
		}
		yield break;
	}

	private IEnumerator ForfeitBattleSequence()
	{
		battleController.CubeBar.GoToTextState(string.Empty);
		yield return new WaitForSeconds(1.5f);
	}

	public IEnumerator RevealEnemyDice()
	{
		foreach (UnitView enemy in battleController.EnemyUnits)
		{
			enemy.PossibleRollsSimple.CreateDiceEffectOnActiveFace();
			enemy.PossibleRollsSimple.ActivateActiveDie();
			enemy.BuffEffects.UpdateBuffEffects();
			yield return new WaitForSeconds(0.3f);
		}
		yield return new WaitForSeconds(0.7f);
	}

	public IEnumerator TeamBeginAttack(TeamState team)
	{
		if (!team.IsInitiativeWinner)
		{
			yield return StartCoroutine(battleController.hud.ShowMessageSequence("ui_battle_counterattack".Localize("Counter Attack!"), InBattleMessageType.COUNTER_ATTACK));
		}
	}

	public IEnumerator TeamBeginAttackAbility(TeamState team, AbilityState ability)
	{
		if (ability.metadata.IsUnitAbility)
		{
			ability.target.unitView.PresentSpecial();
			yield return new WaitForSeconds(battleController.tunables.attackTimeDelay);
			ability.target.unitView.PossibleRollsSimple.DeactivateActiveDie();
			AudioTrigger.CrowdCheering.Play();
		}
		yield return battleController.StartCoroutine(ability.animationHandler.TeamBeginAttackAnimation(team));
		yield return new WaitForSeconds(0.4f);
	}

	public IEnumerator UnitAttack(UnitState unit, UnitState target, int damage, DamageType damageType, int currentRoll)
	{
		UnitView unitView = unit.unitView;
		unitView.BuffEffects.RemoveBuffEffectOnActiveFace();
		unit.unitView.PossibleRollsSimple.ActivateActiveDie(currentRoll);
		yield return battleController.StartCoroutine(unit.unitView.PlayWeaponFiringAnimation());
		yield return new WaitForSeconds(0.2f);
		yield return battleController.StartCoroutine(unit.unitView.PlayWeaponHitAnimation(target.unitView));
		target.unitView.TakeDamage(damage, damageType);
		unitView.PossibleRollsSimple.ActiveFace.ResetPlusText();
		unitView.PossibleRollsSimple.ActiveFace.CloseFace(0.2f);
		unit.unitView.HasAttacked = true;
		yield return new WaitForSeconds(0.75f);
	}

	private void FinishRound()
	{
		foreach (UnitView enemyUnit in battleController.EnemyUnits)
		{
			enemyUnit.PossibleRollsSimple.ResetDieFaces();
			enemyUnit.RolledDice = false;
		}
		if (battleController.battleState.IsComplete)
		{
			if (battleController.battleHooks != null && battleController.battleHooks.CanRevive())
			{
				StartCoroutine(battleController.battleHooks.Revive());
			}
			else
			{
				battleController.phaseManager.SwitchPhase(Phase.OUTRO);
			}
		}
		else
		{
			battleController.phaseManager.SwitchPhase(Phase.ROUNDSTART);
		}
	}

	public IEnumerator TeamRevive(ReviveAction action)
	{
		BattleLogic.ApplyBattleAction(battleController.battleState.teamOne, action);
		battleController.ReAssignUnitViews(battleController.GetAllPlayerUnitViews(), battleController.battleState.teamOne, battleController.PlayerUnits);
		UnitState[] units = battleController.playerTeam.units;
		foreach (UnitState unit in units)
		{
			unit.unitView.DriveOffScreen(0f);
		}
		yield return StartCoroutine(battleController.introPhase.IntroPlayerTeam());
		battleController.phaseManager.SwitchPhase(Phase.ROUNDSTART);
		battleController.matchManager.playerActions.Add(action);
	}

	public IEnumerator TeamForfeit(TeamState team)
	{
		UnitView[] array = battleController.GetUnitsByTeam(team).ToArray();
		foreach (UnitView unit in array)
		{
			unit.TakeDamage(999999);
			yield return new WaitForSeconds(0.25f);
		}
		if (battleController.playerTeam != team)
		{
			yield return StartCoroutine(battleController.hud.ShowMessageSequence("ui_battle_enemysurrenders".Localize("Enemy Surrenders!")));
		}
	}

	public IEnumerator TeamWorstRoll(TeamState team, int[] rollValues)
	{
		if (battleController.playerTeam == team)
		{
			yield return StartCoroutine(battleController.hud.ShowMessageSequence("ui_battle_badluck".Localize("Bad Luck!")));
			yield return StartCoroutine(battleController.roundStartPhase.SpinAllDiceStaggered(rollValues, false));
		}
	}

	public IEnumerator TeamBestRoll(TeamState team)
	{
		if (battleController.playerTeam != team)
		{
			yield break;
		}
		int bestRollAmount = team.battle.GetConstantInt("battle_bestroll_reward", 50);
		foreach (UnitView unit in battleController.PlayerUnits)
		{
			unit.ShowCashEffect(bestRollAmount);
		}
		AudioTrigger.CrowdCheering.Play();
		yield return StartCoroutine(battleController.hud.ShowMessageSequence("ui_battle_bestroll".Localize("Best Roll!")));
	}

	public IEnumerator UnitAcidDamage(UnitState unit, int damageIncrease)
	{
		unit.unitView.LocalAcidPerRound += damageIncrease;
		yield break;
	}

	public IEnumerator UnitWorstRolls(UnitState unit)
	{
		if (battleController.playerTeam == unit.team)
		{
			yield return StartCoroutine(battleController.hud.ShowMessageSequence("ui_battle_badluck".Localize("Bad Luck!")));
			yield return StartCoroutine(unit.unitView.RollDice(unit.currentRoll, true, 1));
		}
	}

	public IEnumerator ApplyDamagePerRound(TeamState team, float delay = 0f)
	{
		foreach (UnitView unitView in battleController.GetUnitsByTeam(team))
		{
			if (unitView.state.damagePerRound > 0)
			{
				unitView.TakeDamage(unitView.state.damagePerRound, DamageType.PerRound, true);
			}
			unitView.PlayBurnFlareUp();
			if (unitView.state.acidPerRound > 0)
			{
				unitView.TakeDamage(unitView.state.acidPerRound, DamageType.Acid, true);
			}
		}
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
	}
}
