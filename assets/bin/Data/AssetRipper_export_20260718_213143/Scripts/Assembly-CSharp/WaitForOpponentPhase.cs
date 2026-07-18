using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForOpponentPhase : AbstractBattlePhase
{
	public override void OnEnterPhase()
	{
		battleController.CubeBar.GoToTextState("ui_battle_waiting".Localize("Waiting for opponent..."));
		battleController.hud.emoticonController.HideToggleButton();
		StartCoroutine(WaitSequence());
	}

	private IEnumerator WaitSequence()
	{
		yield return StartCoroutine(SendActions());
		if (!battleController.playerTeam.forfeited)
		{
			yield return StartCoroutine(ReceiveActions());
			UnitView randomPlayerUnit = battleController.PlayerUnits[UnityEngine.Random.Range(0, battleController.PlayerUnits.Count)];
			battleController.hud.emoticonController.ShowSpeechBubble(battleController.hud.emoticonController.GetCurrentSelectedEmoticon(), randomPlayerUnit.transform);
		}
		PhaseComplete();
	}

	private IEnumerator SendActions()
	{
		bool complete = false;
		battleController.matchManager.playerActions.AddRange(CheckForExtraActions());
		battleController.MatchHandler.SendPlayerActions(delegate
		{
			complete = true;
		});
		while (!complete)
		{
			yield return 0;
		}
	}

	private IEnumerator ReceiveActions()
	{
		battleController.matchManager.enemyActions.Clear();
		bool complete = false;
		battleController.MatchHandler.ReceiveOpponentActions(delegate
		{
			complete = true;
			RollEnemyDice();
			ExecuteEnemyActions();
		});
		while (!complete)
		{
			if ((bool)battleController.hud.timer && battleController.hud.timer.GetTimeSinceClockStopped > Constants.ReceiveActionsFatalTimeout)
			{
				PopupManager.ShowPopup(PopupDataModel.Ok("popup_fatal_game_timeout_title".Localize("Connection Problem"), "popup_fatal_game_timeout_desc".Localize("We're sorry, something went wrong with the battle."), QuitUtility.Restart));
				break;
			}
			yield return 0;
		}
	}

	private void RollEnemyDice()
	{
		foreach (UnitView enemyUnit in battleController.EnemyUnits)
		{
			StartCoroutine(enemyUnit.RollDice(enemyUnit.state.currentRoll, false, 5, true));
		}
	}

	private void ExecuteEnemyActions()
	{
		List<BattleAction> enemyActions = battleController.matchManager.enemyActions;
		foreach (BattleAction item in enemyActions)
		{
			BattleLogic.ApplyBattleAction(battleController.enemyTeam, item);
			if (item is EmoticonAction)
			{
				EmoticonAction emoticonAction = item as EmoticonAction;
				EmoticonTypes emoticonType = (EmoticonTypes)(int)Enum.Parse(typeof(EmoticonTypes), emoticonAction.emoticonName);
				UnitView unitView = battleController.EnemyUnits[UnityEngine.Random.Range(0, battleController.EnemyUnits.Count)];
				battleController.hud.emoticonController.ShowSpeechBubble(emoticonType, unitView.transform);
			}
		}
	}

	public void PhaseComplete()
	{
		battleController.phaseManager.SwitchPhase(Phase.RESOLUTION);
	}

	private List<BattleAction> CheckForExtraActions()
	{
		List<BattleAction> list = new List<BattleAction>();
		if ((bool)battleController.hud.emoticonController)
		{
			EmoticonTypes currentSelectedEmoticon = battleController.hud.emoticonController.GetCurrentSelectedEmoticon();
			if (currentSelectedEmoticon != EmoticonTypes.NONE)
			{
				EmoticonAction item = EmoticonAction.Create(currentSelectedEmoticon.ToString());
				list.Add(item);
			}
		}
		return list;
	}
}
