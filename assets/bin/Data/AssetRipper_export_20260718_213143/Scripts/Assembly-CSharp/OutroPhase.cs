using System.Collections;
using UnityEngine;

public class OutroPhase : AbstractBattlePhase
{
	private int playerUnits;

	private string finalText;

	private Color finalTextColor = Color.white;

	public override void OnEnterPhase()
	{
		if (battleController.battleHooks.StopMusicOnBattleComplete())
		{
			Singleton<AudioManager>.instance.StopMusic();
		}
		if (battleController.playerTeam.IsBattleWinner)
		{
			AudioTrigger.CrowdCheering.Play();
			AudioTrigger.WinBattle.Play();
		}
		else
		{
			AudioTrigger.CrowdDisappointed.Play();
			AudioTrigger.LoseBattle.Play();
		}
		ReportingBattleResult reportingBattleResult = ReportingBattleResult.win;
		if (battleController.playerTeam.forfeited)
		{
			finalText = "ui_battle_result_forfeit".Localize("Forfeit!");
			finalTextColor = Color.white;
			reportingBattleResult = ReportingBattleResult.forfeit;
		}
		else if (battleController.enemyTeam.IsBattleWinner)
		{
			finalText = "ui_battle_result_defeat".Localize("Defeat");
			reportingBattleResult = ReportingBattleResult.lose;
			finalTextColor = Color.red;
		}
		else if (battleController.playerTeam.IsBattleWinner)
		{
			finalText = "ui_battle_result_victory".Localize("Victory!");
			reportingBattleResult = ReportingBattleResult.win;
			finalTextColor = Color.yellow;
		}
		else
		{
			reportingBattleResult = ReportingBattleResult.tie;
			finalText = "ui_battle_result_tie".Localize("Tie!");
		}
		string matchId = battleController.MatchData.matchId;
		Reporting.BattleEndEvent(matchId, reportingBattleResult, battleController);
		battleController.StartCoroutine(OutroText());
	}

	public IEnumerator OutroText()
	{
		if (battleController.playerTeam.IsBattleWinner)
		{
			foreach (UnitView unit in battleController.PlayerUnits)
			{
				unit.ShowCashEffect(unit.state.metadata.SurviveCash);
			}
		}
		yield return StartCoroutine(battleController.hud.ShowMessageSequence(finalText, finalTextColor));
		yield return StartCoroutine(battleController.battleHooks.OutroAnimation());
		if (battleController.battleHooks.ShouldRestartBattle())
		{
			yield return StartCoroutine(battleController.battleHooks.OnRestartBattle());
			yield break;
		}
		yield return StartCoroutine(MatchComplete());
		yield return StartCoroutine(battleController.PlayViewportWinAnimation());
		battleController.MatchHandler.GotoPostBattleScene();
	}

	private IEnumerator MatchComplete()
	{
		bool complete = false;
		battleController.MatchHandler.MatchComplete(battleController.playerTeam.stats, delegate
		{
			complete = true;
		});
		while (!complete)
		{
			yield return 0;
		}
	}
}
