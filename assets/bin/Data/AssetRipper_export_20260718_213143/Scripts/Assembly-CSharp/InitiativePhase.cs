using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiativePhase : AbstractBattlePhase
{
	private const string COIN_TOSS_WIN_ANIMATION = "Coin Toss Win";

	private const string COIN_TOSS_LOSE_ANIMATION = "Coin Toss Lose";

	private static readonly Color FIRST_STRIKE_COLOR = new Color(0.11764706f, 32f / 51f, 0.9843137f, 1f);

	private float playerInitiative;

	private float enemyInitiative;

	private BattleField _startingBackground;

	public IEnumerator InitiativeSequence(ServerTeamState winningTeam, int hostTeamInitiative, int guestTeamInitiative)
	{
		playerInitiative = ((!battleController.playerTeam.IsHostTeam) ? guestTeamInitiative : hostTeamInitiative);
		enemyInitiative = ((!battleController.enemyTeam.IsHostTeam) ? guestTeamInitiative : hostTeamInitiative);
		battleController.hud.HideTimer();
		yield return StartCoroutine(battleController.hud.ShowMessageSequence("ui_battle_firststrike".Localize("First Strike"), FIRST_STRIKE_COLOR, InBattleMessageType.FIRST_STRIKE));
		yield return StartCoroutine(ShowFacesSequence());
		battleController.CubeBar.UpdateTextState(string.Empty);
	}

	private IEnumerator ShowFacesSequence()
	{
		HideAllUnitSpecialRollText(battleController.PlayerUnits);
		yield return new WaitForSeconds(0.5f);
		if (playerInitiative == enemyInitiative)
		{
			if (playerInitiative > 0f && enemyInitiative > 0f)
			{
				ExpandInitiativeDieFaces(battleController.EnemyUnits);
				ExpandInitiativeDieFaces(battleController.PlayerUnits);
				yield return new WaitForSeconds(1f);
				RestoreInitiativeDieFaces(battleController.EnemyUnits);
				RestoreInitiativeDieFaces(battleController.PlayerUnits);
			}
			else
			{
				yield return new WaitForSeconds(0.25f);
			}
			yield return StartCoroutine(TieBreakerSequence());
		}
		else
		{
			yield return StartCoroutine(ActivateWinnerSequence());
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator ActivateWinnerSequence()
	{
		TeamState winningTeam = battleController.battleState.initiativeWinner;
		ExpandInitiativeDieFaces(battleController.GetUnitsByTeam(winningTeam));
		yield return new WaitForSeconds(0.3f);
		ShowInitiativeDieEffects(battleController.GetUnitsByTeam(winningTeam));
		yield return new WaitForSeconds(0.5f);
		RestoreInitiativeDieFaces(battleController.GetUnitsByTeam(winningTeam));
		yield return new WaitForSeconds(0.3f);
	}

	private IEnumerator TieBreakerSequence()
	{
		EffectInstance coinToss = GlobalEffectsManager.Create(EffectType.COIN_TOSS_EFFECT, new Vector3(0f, 0f, 0f));
		AudioTrigger.CoinFlip.Play();
		coinToss.SpineAnimation.animationName = ((!battleController.playerTeam.IsInitiativeWinner) ? "Coin Toss Lose" : "Coin Toss Win");
		yield return new WaitForSeconds(0.55f);
		if (battleController.playerTeam.IsInitiativeWinner)
		{
			AudioTrigger.WinFirstStrike.Play();
			AudioTrigger.HighFirstStrikeResult.Play();
		}
		else
		{
			AudioTrigger.LoseFirstStrike.Play();
			AudioTrigger.SpecialResult.Play();
		}
		battleController.playerField.shaker.Shake(5f, 0.2f);
		battleController.enemyField.shaker.Shake(5f, 0.2f);
		yield return new WaitForSeconds(1.25f);
		Object.Destroy(coinToss.gameObject);
	}

	private void ShowInitiativeDieEffects(List<UnitView> units)
	{
		foreach (UnitView unit in units)
		{
			if (unit.CurrentRollAction == DieFaceType.Initiative)
			{
				EffectInstance effectInstance = unit.PossibleRollsSimple.CreateDiceEffectOnActiveFace(false, false);
				effectInstance.transform.localScale *= 2f;
			}
		}
	}

	private void ExpandInitiativeDieFaces(List<UnitView> units)
	{
		foreach (UnitView unit in units)
		{
			if (unit.CurrentRollAction == DieFaceType.Initiative)
			{
				unit.PossibleRollsSimple.ExpandActiveDieFace(3f, 0.3f);
			}
		}
	}

	private void RestoreInitiativeDieFaces(List<UnitView> units)
	{
		foreach (UnitView unit in units)
		{
			if (unit.CurrentRollAction == DieFaceType.Initiative)
			{
				unit.PossibleRollsSimple.RestoreActiveDieFace(0.25f);
			}
		}
	}

	private void HideAllUnitSpecialRollText(List<UnitView> units)
	{
		foreach (UnitView unit in units)
		{
			if (unit.CurrentRollAction == DieFaceType.Special)
			{
				unit.AbilityText.Hide();
			}
		}
	}
}
