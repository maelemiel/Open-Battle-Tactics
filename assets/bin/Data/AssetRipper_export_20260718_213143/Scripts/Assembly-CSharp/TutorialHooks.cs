using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class TutorialHooks : BattleHooks
{
	private static float HIGHLIGHT_DELAY = 1f;

	protected bool restartPlayer = true;

	protected bool restartEnemy = true;

	protected int badRollThreshold = 1;

	protected EffectInstance highlightedArea;

	protected Sequence highlightDelay;

	protected Coroutine StartCoroutine(IEnumerator e)
	{
		return battleController.StartCoroutine(e);
	}

	protected Coroutine DialogTrigger(string triggerName)
	{
		return StartCoroutine(DialogTriggerInternal(triggerName));
	}

	private IEnumerator DialogTriggerInternal(string triggerName)
	{
		AnnouncerController.Events.OnSequenceAction += DialogActionListener;
		AnnouncerController.Events.OnSequenceExit += DialogCleanupListener;
		yield return StartCoroutine(AnnouncerController.DialogTrigger(triggerName, 0.2f));
		AnnouncerController.Events.OnSequenceAction -= DialogActionListener;
		AnnouncerController.Events.OnSequenceExit -= DialogCleanupListener;
	}

	protected void DialogCleanupListener()
	{
		ClearHighlight();
	}

	protected void DialogActionListener(string action)
	{
		BattleTunables tunables = battleController.tunables;
		if (action == "showPlayerOne")
		{
			battleController.PlayerUnits[0].PossibleRollsSimple.OpenDieBox();
			HighlightArea(tunables.tutorialHighlightSpinnerOne);
		}
		else if (action == "spinPlayerOne")
		{
			battleController.PlayerUnits[0].PossibleRollsSimple.CloseDieBox();
			UnitView unitView = battleController.PlayerUnits[0];
			StartCoroutine(unitView.RollDice(unitView.state.currentRoll, true, 10, true));
		}
		if (action == "highlight playerSpinnerOne")
		{
			HighlightArea(tunables.tutorialHighlightSpinnerOne);
		}
		else if (action == "highlight playerSpinnerTwo")
		{
			HighlightArea(tunables.tutorialHighlightSpinnerTwo);
		}
		switch (action)
		{
		case "highlight abilityButtonOne":
			HighlightArea(tunables.tutorialHighlightAbilityOne, false);
			break;
		case "highlight abilityButtonTwo":
			HighlightArea(tunables.tutorialHighlightAbilityTwo, false);
			break;
		case "highlight abilityButtonThree":
			HighlightArea(tunables.tutorialHighlightAbilityThree, false);
			break;
		}
		if (action == "highlight abilityPoints")
		{
			HighlightArea(tunables.tutorialHighlightAbilityPoints, false);
		}
		if (action == "clear highlight")
		{
			ClearHighlight();
		}
	}

	protected void HighlightArea(Transform parent, bool isOval = true)
	{
		ClearHighlight();
		highlightDelay = new Sequence();
		highlightDelay.AppendInterval(HIGHLIGHT_DELAY);
		highlightDelay.AppendCallback((TweenDelegate.TweenCallback)delegate
		{
			if ((bool)parent)
			{
				highlightedArea = GlobalEffectsManager.Create((!isOval) ? EffectType.UICircle : EffectType.UIOval, parent.position, parent).SetLayer(parent.gameObject.layer);
				Vector3 position = ((highlightedArea.gameObject.layer != battleController.playerField.gameObject.layer) ? battleController.hud.uiCamera.ScreenCamera.WorldToScreenPoint(highlightedArea.transform.position) : battleController.playerField.unityCamera.WorldToScreenPoint(highlightedArea.transform.position));
				if (AnnouncerController.AnnouncerCamera != null)
				{
					highlightedArea.transform.parent = AnnouncerController.AnnouncerCamera.transform;
					highlightedArea.transform.position = AnnouncerController.AnnouncerCamera.ScreenToWorldPoint(position);
					highlightedArea.gameObject.layer = AnnouncerController.AnnouncerCamera.gameObject.layer;
				}
				highlightedArea.transform.localRotation = Quaternion.identity;
			}
		});
		highlightDelay.Play();
	}

	protected void ClearHighlight()
	{
		if (highlightDelay != null)
		{
			highlightDelay.Kill();
			highlightDelay = null;
		}
		if (highlightedArea != null)
		{
			EffectInstance cachedInstance = highlightedArea;
			highlightedArea.SpineAnimation.TweenAlpha(0f, 0.3f, EaseType.Linear, delegate
			{
				cachedInstance.SpineAnimation.Skeleton.skeleton.A = 1f;
				cachedInstance.Destroy();
			});
			highlightedArea = null;
		}
	}

	protected void ShowAbilityButtonArrow(int index)
	{
		battleController.hud.bouncingArrowAction.Hide();
		bool[] array = new bool[3];
		array[index] = true;
		battleController.hud.bouncingArrowAction.StartBouncingAfterSeconds(1.5f, array);
	}

	protected void ShowBattleButtonArrow()
	{
		battleController.hud.bouncingArrowCommit.Hide();
		battleController.hud.bouncingArrowCommit.StartBouncingAfterSeconds(2f, new bool[1] { true });
	}

	public override void Init(BattleController battleController)
	{
		base.Init(battleController);
		battleController.introPhase.showText = false;
	}

	public override bool StopMusicOnBattleComplete()
	{
		return false;
	}

	public override IEnumerator PreIntroPlayerRollIn()
	{
		yield return DialogTrigger("TutorialRestartFromLaunch");
	}

	public override bool OnClickBattle()
	{
		Reporting.TutorialAction("TapBattle");
		if (battleController.playerTeam.energy > 0)
		{
			foreach (UnitView playerUnit in battleController.PlayerUnits)
			{
				if (playerUnit.CurrentRoll <= badRollThreshold)
				{
					Transform parent = battleController.tunables.tutorialHighlightSpinnerOne;
					if (playerUnit.state.index == 1)
					{
						parent = battleController.tunables.tutorialHighlightSpinnerTwo;
					}
					else if (playerUnit.state.index == 2)
					{
						parent = battleController.tunables.tutorialHighlightSpinnerThree;
					}
					else if (playerUnit.state.index == 3)
					{
						parent = battleController.tunables.tutorialHighlightSpinnerFour;
					}
					HighlightArea(parent);
					DialogTrigger("TutorialAbilityReminder");
					return false;
				}
			}
		}
		return true;
	}

	public virtual OpponentData GetPlayerTeam()
	{
		return null;
	}

	public virtual OpponentData GetOpponentTeam()
	{
		return null;
	}

	public override IEnumerator OnRestartBattle()
	{
		UnitState[] units = battleController.enemyTeam.units;
		foreach (UnitState unit in units)
		{
			if (unit.IsDead)
			{
				unit.unitView.DriveOffScreen(0f);
			}
		}
		UnitState[] units2 = battleController.playerTeam.units;
		foreach (UnitState unit2 in units2)
		{
			if (unit2.IsDead)
			{
				unit2.unitView.DriveOffScreen(0f);
			}
		}
		if (restartPlayer || !battleController.playerTeam.IsBattleWinner)
		{
			battleController.InitPlayer();
		}
		if (restartEnemy)
		{
			battleController.InitEnemy();
		}
		battleController.BeginBattle();
		while (!AreUnitsLoaded(battleController.PlayerUnits))
		{
			yield return 0;
		}
		yield return StartCoroutine(battleController.introPhase.IntroPlayerTeam());
		while (!AreUnitsLoaded(battleController.EnemyUnits))
		{
			yield return 0;
		}
		yield return StartCoroutine(battleController.introPhase.IntroEnemyTeam());
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

	public override bool OnClickCancel()
	{
		Reporting.TutorialAction("TapCancel");
		return true;
	}

	public override bool OnTapUnit(UnitView unit)
	{
		Reporting.TutorialAction("OpenCloseUnit_" + unit.ToString());
		return true;
	}

	public override bool OnInvalidTapUnit(UnitView unit)
	{
		Reporting.TutorialAction("InvalidTapUnit_" + unit.ToString());
		return true;
	}

	public override bool OnTargetUnit(UnitView unit)
	{
		Reporting.TutorialAction("TargetUnit_" + unit.ToString());
		return true;
	}

	public override bool OnTapAbility(AbilityState ability)
	{
		Reporting.TutorialAction("UseAbility_" + ((ability == null) ? "None" : ability.metadata.ID));
		return true;
	}

	public virtual void GotoPostBattleScene()
	{
		UserProfile.player.tutorial.CurrentStep = TutorialStep.ChangeName;
		if (!TutorialConstants.SHOW_CHANGE_NAME)
		{
			UserProfile.player.tutorial.CurrentStep = TutorialStep.BuildFirstTank;
		}
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			UserProfile.player.TryClaimFirstUnit(null);
		});
		ProgressionDivisionDataModel single = ProgressionDivisionDataModel.GetSingle(1);
		UserProfile.player.notifications.Add(new UserNotification.DivisionPromotion(single));
		AbilityDataModel single2 = AbilityDataModel.GetSingle("402001");
		UserProfile.player.notifications.Add(new UserNotification.AbilityEarned(single2));
		BattleRewardsSceneModel sceneDM = CreateInitialBattleRewards();
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.PostBattleRewardsScene, sceneDM);
	}

	private void NextScreen()
	{
		UserProfile.player.tutorial.GotoTutorialScene();
	}

	private BattleRewardsSceneModel CreateInitialBattleRewards()
	{
		BattleRewardsSceneModel battleRewardsSceneModel = new BattleRewardsSceneModel(MatchData.Type.TUTORIAL, true, new ServerTeamStatsState(), new ServerTeamStatsState(), new OpponentData(), new OpponentData(), 100);
		battleRewardsSceneModel.isPlayerWinner = true;
		battleRewardsSceneModel.playerStats.baseCoins = Constants.PlayerStartingCoins;
		battleRewardsSceneModel.playerStats.coinsFromUnitsDestroyed = 0;
		battleRewardsSceneModel.playerStats.coinsFromUnitsSurvived = 0;
		battleRewardsSceneModel.playerStats.coinsFromBestRolls = 0;
		battleRewardsSceneModel.playerStats.coinsFromMultiKill = 0;
		battleRewardsSceneModel.playerStats.coinsFromOverKills = 0;
		battleRewardsSceneModel.playerStats.coinsFromPerfectKills = 0;
		battleRewardsSceneModel.playerStats.partsEarned = new List<IPartMetadata>();
		battleRewardsSceneModel.enemyStats.partsEarned = new List<IPartMetadata>();
		List<ItemGiftDataModel> all = ItemGiftDataModel.GetAll();
		ItemGiftDataModel itemGiftDataModel = all.Find((ItemGiftDataModel x) => x.giftId == Constants.PlayerStartingGift);
		List<IPartMetadata> list = new List<IPartMetadata>();
		UnitPartsDataModel unitPartsDataModel = new UnitPartsDataModel();
		unitPartsDataModel.partType = itemGiftDataModel.itemId;
		int amount = itemGiftDataModel.amount;
		for (int num = 0; num < amount; num++)
		{
			list.Add(unitPartsDataModel);
		}
		battleRewardsSceneModel.playerStats.partsEarned.AddRange(list);
		return battleRewardsSceneModel;
	}
}
