using System;
using System.Collections;
using Holoville.HOTween;

public class RoundStartPhase : AbstractBattlePhase
{
	private const float UNIT_RELOCATE_TRANSITION_TIME = 1.75f;

	public override void OnEnterPhase()
	{
		battleController.matchManager.playerActions.Clear();
		BattleSyncChecker.SyncCheck(battleController);
		battleController.animationHandler.AddSequence(BeginRoundSequence());
		BattleLogic.BeginRound(battleController.battleState);
		if (battleController.battleHooks != null)
		{
			battleController.battleHooks.ModifyRoundStep();
		}
		ResetUnits();
		battleController.animationHandler.AddSequence(CompleteRoundStartPhase());
	}

	public IEnumerator BeginRoundSequence()
	{
		foreach (UnitView unit in battleController.EnemyUnits)
		{
			unit.PossibleRollsSimple.CloseDieBox();
		}
		ResetUnitPositions();
		yield return StartCoroutine(battleController.hud.ShowMessageSequence(string.Format("ui_battle_round".Localize("Round {0}"), battleController.battleState.currentRound)));
		yield return battleController.StartCoroutine(RollAllDiceAnim());
	}

	private void ResetUnits()
	{
		foreach (UnitView enemyUnit in battleController.EnemyUnits)
		{
			enemyUnit.SetLocalArmor(0, false);
			enemyUnit.PossibleRollsSimple.DeactivateActiveDie();
			enemyUnit.PossibleRollsSimple.ResetDieFaces();
			enemyUnit.RefreshHealthHUD();
			enemyUnit.PossibleRollsSimple.CloseDieBox();
			enemyUnit.LocalPreventReroll = enemyUnit.LocalRoundsUntilRerollEnabled > 0;
			if (enemyUnit.LocalPreventReroll)
			{
				enemyUnit.LocalRoundsUntilRerollEnabled--;
			}
		}
		foreach (UnitView playerUnit in battleController.PlayerUnits)
		{
			playerUnit.SetLocalArmor(0, false);
			playerUnit.RefreshHealthHUD();
			playerUnit.LocalPreventReroll = playerUnit.LocalRoundsUntilRerollEnabled > 0;
			if (playerUnit.LocalPreventReroll)
			{
				playerUnit.LocalRoundsUntilRerollEnabled--;
			}
		}
	}

	private void OnTankReposition(TweenEvent callbackData)
	{
		((UnitView)callbackData.parms[0]).BeginAmbientMovement();
	}

	private void ResetUnitPositions()
	{
		battleController.RepositionTeam(battleController.playerTeam);
		foreach (UnitView playerUnit in battleController.PlayerUnits)
		{
			HOTween.To(playerUnit.transform, 1.75f, new TweenParms().Ease(EaseType.EaseOutSine).Prop("localPosition", playerUnit.originalLocalPosition).OnComplete(OnTankReposition, playerUnit));
		}
		battleController.RepositionTeam(battleController.enemyTeam);
		foreach (UnitView enemyUnit in battleController.EnemyUnits)
		{
			HOTween.To(enemyUnit.transform, 1.75f, new TweenParms().Ease(EaseType.EaseOutSine).Prop("localPosition", enemyUnit.originalLocalPosition).OnComplete(OnTankReposition, enemyUnit));
		}
	}

	private IEnumerator RollAllDiceAnim()
	{
		foreach (UnitView unit in battleController.PlayerUnits)
		{
			unit.PossibleRollsSimple.SetVisible(true);
			unit.PossibleRollsSimple.CloseDieBox();
			if ((bool)unit.RarityStars)
			{
				unit.RarityStars.SetBarLevel(unit.Rarity - 1);
			}
			unit.HealthUI.SetVisible(true);
		}
		yield return StartCoroutine(SpinAllDiceStaggered(GetTeamRollValues(true), true));
	}

	public int[] GetTeamRollValues(bool isRoundStart)
	{
		int[] array = new int[battleController.PlayerUnits.Count];
		for (int i = 0; i < array.Length; i++)
		{
			UnitState state = battleController.PlayerUnits[i].state;
			array[i] = ((!isRoundStart) ? state.currentRoll : state.roundStartRoll);
		}
		return array;
	}

	public IEnumerator SpinAllDiceStaggered(int[] rollValues, bool isRoundStartRoll)
	{
		if (!battleController.battleHooks.PlayerSpinAllDice())
		{
			yield break;
		}
		int unitsRolling = 0;
		Action<UnitView> finishRollHandler = delegate(UnitView unitView)
		{
			unitsRolling--;
			unitView.OnDiceFinishRollEvent -= finishRollHandler;
		};
		int i = 0;
		foreach (UnitView unit in battleController.PlayerUnits)
		{
			unitsRolling++;
			unit.OnDiceFinishRollEvent += finishRollHandler;
			StartCoroutine(unit.RollDice(rollValues[i], true, 3 + i * 2, isRoundStartRoll));
			i++;
		}
		while (unitsRolling > 0)
		{
			yield return 0;
		}
	}

	private IEnumerator CompleteRoundStartPhase()
	{
		battleController.CubeBar.GoToMainState();
		battleController.phaseManager.SwitchPhase(Phase.DECISION);
		yield break;
	}

	public override void OnUpdate()
	{
		UpdateEnergy();
	}

	public override void OnExitPhase()
	{
	}

	private void UpdateEnergy()
	{
		if ((bool)battleController.CubeBar.EnergyWidget)
		{
			battleController.CubeBar.EnergyWidget.UpdateEnergy(battleController.playerTeam.energy);
		}
	}
}
