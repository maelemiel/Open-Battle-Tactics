using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class IntroPhase : AbstractBattlePhase
{
	public float introTime = 3f;

	public bool showText = true;

	public override void OnEnterPhase()
	{
		battleController.hud.HideTimer();
		battleController.StartCoroutine(IntroAnimation());
	}

	public IEnumerator IntroAnimation()
	{
		while (!AreUnitsLoaded(battleController.PlayerUnits))
		{
			yield return 0;
		}
		yield return StartCoroutine(GlobalEffectsManager.WaitUntilEffectsLoaded());
		if (UserProfile.player.preferences.KamcordOn && battleController.MatchType != MatchData.Type.TUTORIAL)
		{
			Kamcord.StartRecording();
		}
		if (battleController.battleHooks != null)
		{
			yield return StartCoroutine(battleController.battleHooks.PreIntroPlayerRollIn());
		}
		battleController.CubeBar.GoToTextState("ui_battle_intro".Localize("let's roll"));
		yield return StartCoroutine(IntroPlayerTeam());
		yield return new WaitForSeconds(1f);
		battleController.CubeBar.GoToTextState(string.Empty);
		if (battleController.battleHooks != null)
		{
			yield return StartCoroutine(battleController.battleHooks.PostIntroPlayerRollIn());
		}
		if (showText)
		{
			battleController.CubeBar.UpdateTextState("ui_battle_findingopponent".Localize("Finding opponent..."));
		}
		while (!AreUnitsLoaded(battleController.EnemyUnits))
		{
			yield return 0;
		}
		yield return StartCoroutine(GlobalEffectsManager.WaitUntilEffectsLoaded());
		foreach (UnitView unit in battleController.AllUnits)
		{
			unit.LocalHealth = unit.state.hp;
			unit.RefreshHealthHUD();
		}
		if (showText)
		{
			AudioTrigger.BattleStart.Play();
			battleController.CubeBar.UpdateTextState(string.Format("ui_battle_foundopponentname".Localize("Opponent: {0}"), battleController.matchManager.MatchData.opponentTeam.name));
		}
		if (battleController.MatchData.opponentTeam.type == TeamType.Player)
		{
			yield return StartCoroutine(battleController.hud.ShowMessageSequence("ui_battle_foundliveopponent".Localize("Live Player Found!")));
			ShowLiveOpponentSequence();
		}
		if (battleController.battleHooks != null)
		{
			yield return StartCoroutine(battleController.battleHooks.PreIntroEnemyRollIn());
		}
		yield return StartCoroutine(IntroEnemyTeam());
		if (battleController.battleHooks != null)
		{
			yield return StartCoroutine(battleController.battleHooks.PostIntroEnemyRollIn());
		}
		battleController.hud.abilityReporter.Hide();
		battleController.CubeBar.UpdateTextState(string.Empty);
		if (battleController.battleHooks != null)
		{
			yield return StartCoroutine(battleController.battleHooks.OnExitIntroPhase());
		}
		if (battleController.MatchData.opponentTeam.type == TeamType.Player)
		{
			StartCoroutine(HideLiveOpponentSequence());
		}
		battleController.phaseManager.SwitchPhase(Phase.ROUNDSTART);
	}

	private bool AreUnitsLoaded(List<UnitView> unitList)
	{
		if (unitList == null || unitList.Count == 0)
		{
			return false;
		}
		foreach (UnitView unit in unitList)
		{
			if (!unit.IsLoaded)
			{
				return false;
			}
		}
		return true;
	}

	private void ShowLiveOpponentSequence()
	{
		UserProfile player = UserProfile.player;
		OpponentData opponentData = new OpponentData();
		opponentData.name = player.nickname;
		opponentData.division = player.CurrentDivision;
		opponentData.pvpRating = player.pvpRating;
		opponentData.winStreak = player.winStreak;
		opponentData.thumbnailURL = player.thumbnail;
		battleController.hud.ShowPVPMomentAnimation(opponentData, battleController.MatchData.opponentTeam);
	}

	private IEnumerator HideLiveOpponentSequence()
	{
		yield return StartCoroutine(battleController.hud.HidePVPMomentAnimation());
	}

	private void BringUnitOnToField(UnitView unit)
	{
		unit.HealthUI.SetVisible(false);
		unit.PossibleRollsSimple.SetVisible(false);
		if ((bool)unit.RarityStars)
		{
			unit.RarityStars.DeactivateBar();
		}
		if (!unit.isOnScreen)
		{
			unit.DriveOnScreen(1.75f, unit.OnToField);
		}
		else
		{
			unit.DriveOnScreen(1.2f);
		}
	}

	public IEnumerator IntroPlayerTeam()
	{
		foreach (UnitView unit in battleController.PlayerUnits)
		{
			BringUnitOnToField(unit);
			yield return new WaitForSeconds(0.3f);
		}
		yield return new WaitForSeconds(1f);
	}

	private void PresentUnitHUD(UnitView unit)
	{
		unit.HealthUI.SetVisible(true);
		unit.PossibleRollsSimple.ResetAllPlusNumber();
		if ((bool)unit.RarityStars)
		{
			unit.RarityStars.SetBarLevel(unit.Rarity - 1);
		}
		unit.PossibleRollsSimple.SetVisible(true);
		SimpleTween.Start(0f, 1f, 0.3f, EaseType.EaseOutExpo, delegate(float val)
		{
			unit.PossibleRollsSimple.SetInfoBoxWidth(val);
		});
	}

	public IEnumerator IntroEnemyTeam()
	{
		List<UnitView> uv = battleController.EnemyUnits;
		List<UnitView> orderedEntryList = new List<UnitView>();
		if (uv.Count == 1)
		{
			orderedEntryList.Add(uv[0]);
		}
		else
		{
			int currIndex = 1;
			for (int i = 0; i < uv.Count; i++)
			{
				orderedEntryList.Add(uv[currIndex]);
				currIndex += 2;
				if (currIndex >= uv.Count)
				{
					currIndex = 0;
				}
			}
		}
		foreach (UnitView unit in orderedEntryList)
		{
			StartCoroutine(EnemyUnitSequence(unit));
			yield return new WaitForSeconds(0.5f);
		}
		yield return new WaitForSeconds(1.25f);
	}

	private IEnumerator EnemyUnitSequence(UnitView unit)
	{
		BringUnitOnToField(unit);
		yield return new WaitForSeconds(0.5f);
		unit.PossibleRollsSimple.CloseInfoBox();
		unit.PossibleRollsSimple.CloseDieBox(0f, true);
		PresentUnitHUD(unit);
		unit.PossibleRollsSimple.CloseDieBox();
	}

	public override void OnExitPhase()
	{
		foreach (UnitView enemyUnit in battleController.EnemyUnits)
		{
			enemyUnit.PossibleRollsSimple.CloseDieBox();
		}
	}
}
