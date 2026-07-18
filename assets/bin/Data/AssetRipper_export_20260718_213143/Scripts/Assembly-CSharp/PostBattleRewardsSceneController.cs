using System;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class PostBattleRewardsSceneController : SceneController
{
	private const string ANNOUNCER_WIN_ANIM = "Idle 1 wink";

	private const string ANNOUNCER_LOSE_ANIM = "Idle3 blinks";

	public static string BACKGROUND_DISABLED_SPRITENAME = "LOSE_PostGame";

	public static string BACKGROUND_ENABLED_SPRITENAME = "Victory_PostGame";

	public static string BACKGROUND_ENABLED_EMPTY_SPRITENAME = "Yellow_PostGame";

	public static string BACKGROUND_DISABLED_EMPTY_SPRITENAME = "Lose_Grey_PostGame";

	[SerializeField]
	private tk2dUIItem skipButton;

	[SerializeField]
	private GameObject nextButton;

	[SerializeField]
	private GameObject battleButton;

	private AudioManager.Sfx battleRewardSfx;

	private BattleRewardsSceneModel _sceneModel;

	private PostBattleRewardsStates currentState;

	[SerializeField]
	private tk2dUIProgressBar progressBar;

	[SerializeField]
	private SceneState partsRewardState;

	[SerializeField]
	private SceneState divisionProgressState;

	[SerializeField]
	private SceneState divisionEarnedState;

	[SerializeField]
	private SceneState promotionProgressState;

	[SerializeField]
	private SceneState promotionEarnedState;

	[SerializeField]
	private SceneState promotionEarnedBlueprintsState;

	[SerializeField]
	private SceneState abilityRewardState;

	[SerializeField]
	private SceneState pvpResultsState;

	[SerializeField]
	private SceneState eventRewardState;

	[SerializeField]
	private GameObject winBackgroundGameObject;

	[SerializeField]
	private GameObject loseBackgroundGameObject;

	public override void Awake()
	{
		_showHomeButton = false;
		_showTopBar = false;
		base.Awake();
		if ((bool)nextButton)
		{
			nextButton.SetActive(false);
		}
		if ((bool)battleButton)
		{
			battleButton.SetActive(false);
		}
		partsRewardState.gameObject.SetActive(false);
		SceneController.resumeCallbackEnable = false;
	}

	private void Start()
	{
		Log.DebugTag("Start", null, "PostBattleRewards");
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<AudioManager>.instance.StopMusic();
		battleRewardSfx = AudioTrigger.BattleReward_Music.Play(PlayMenuMusic);
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		_sceneModel = SceneTransitionManager.CurrentSceneDM as BattleRewardsSceneModel;
		if (_sceneModel == null)
		{
			Log.Warning("Using Fake battle rewards!!");
			_sceneModel = CreateFakeBattleRewards(true);
		}
		UserProfile player = UserProfile.player;
		if ((bool)progressBar)
		{
			int totalPointToPromotionSeries = player.CurrentDivision.totalPointToPromotionSeries;
			int num = player.points - _sceneModel.playerStats.pointsEarned;
			progressBar.Value = (float)num / (float)totalPointToPromotionSeries;
		}
		SetBackgroundState(_sceneModel.isPlayerWinner);
		PostBattleRewardsStates postBattleRewardsStates = PostBattleRewardsStates.NONE;
		postBattleRewardsStates = ((_sceneModel.matchType != MatchData.Type.PVP || _sceneModel.enemyData.type != TeamType.Player) ? PostBattleRewardsStates.REWARDS_PARTS : PostBattleRewardsStates.REWARDS_PVP_RESULTS);
		SetState(postBattleRewardsStates, null);
	}

	private void SetState(PostBattleRewardsStates newState, object dataModel)
	{
		currentState = newState;
		skipButton.enabled = true;
		switch (currentState)
		{
		case PostBattleRewardsStates.REWARDS_PVP_RESULTS:
			InitRewardsPvpResultsSequence(_sceneModel);
			break;
		case PostBattleRewardsStates.REWARDS_PARTS:
			InitRewardsPartsSequence(_sceneModel);
			break;
		case PostBattleRewardsStates.REWARDS_DIVISION_PROGRESS:
			InitDivisionProgressSequence(dataModel);
			break;
		case PostBattleRewardsStates.REWARDS_DIVISION_EARNED:
			InitDivisionEarnedSequence(dataModel);
			break;
		case PostBattleRewardsStates.REWARDS_PROMOTION_PROGRESS:
			InitPromotionProgressSequence(dataModel);
			break;
		case PostBattleRewardsStates.REWARDS_PROMOTION_EARNED:
			InitPromotionEarnedSequence(dataModel);
			break;
		case PostBattleRewardsStates.REWARDS_PROMOTION_EARNED_BLUEPRINTS:
			InitPromotionEarnedBlueprintsSequence(dataModel);
			break;
		case PostBattleRewardsStates.REWARDS_ABILITY:
			InitRewardAbilitySequence(dataModel);
			break;
		case PostBattleRewardsStates.REWARDS_EVENT:
			InitEventRewardSequence(dataModel);
			break;
		case PostBattleRewardsStates.NONE:
		case PostBattleRewardsStates.REWARDS_END:
			EndRewardsSequence();
			break;
		}
	}

	private void SkipCurrentState()
	{
		switch (currentState)
		{
		case PostBattleRewardsStates.REWARDS_PVP_RESULTS:
			SkipState(pvpResultsState);
			break;
		case PostBattleRewardsStates.REWARDS_PARTS:
			SkipState(partsRewardState);
			break;
		case PostBattleRewardsStates.REWARDS_DIVISION_PROGRESS:
			SkipState(divisionProgressState);
			break;
		case PostBattleRewardsStates.REWARDS_DIVISION_EARNED:
			SkipState(divisionEarnedState);
			break;
		case PostBattleRewardsStates.REWARDS_PROMOTION_PROGRESS:
			SkipState(promotionProgressState);
			break;
		case PostBattleRewardsStates.REWARDS_PROMOTION_EARNED:
			SkipState(promotionEarnedState);
			break;
		case PostBattleRewardsStates.REWARDS_PROMOTION_EARNED_BLUEPRINTS:
			SkipState(promotionEarnedBlueprintsState);
			break;
		case PostBattleRewardsStates.REWARDS_ABILITY:
			SkipState(abilityRewardState);
			break;
		case PostBattleRewardsStates.REWARDS_EVENT:
			SkipState(eventRewardState);
			break;
		}
	}

	private void SkipState(SceneState state)
	{
		StopAllCoroutines();
		HOTween.Complete();
		state.SkipToEnd();
	}

	public void PlayMenuMusic()
	{
		AudioTrigger.MenuBackground_Music.PlayMusic();
		battleRewardSfx = null;
	}

	private void InitRewardsPvpResultsSequence(object dataModel)
	{
		DeactivateAllStates();
		pvpResultsState.gameObject.SetActive(true);
		pvpResultsState.InitSequence(dataModel);
		StartCoroutine(pvpResultsState.PlayStateSequence(SetState));
	}

	private void InitRewardsPartsSequence(object dataModel)
	{
		DeactivateAllStates();
		partsRewardState.gameObject.SetActive(true);
		partsRewardState.InitSequence(dataModel);
		StartCoroutine(partsRewardState.PlayStateSequence(SetState));
	}

	private void InitDivisionProgressSequence(object dataModel)
	{
		DeactivateAllStates();
		divisionProgressState.gameObject.SetActive(true);
		divisionProgressState.InitSequence(dataModel);
		StartCoroutine(divisionProgressState.PlayStateSequence(SetState));
	}

	private void InitDivisionEarnedSequence(object dataModel)
	{
		DeactivateAllStates();
		divisionEarnedState.gameObject.SetActive(true);
		divisionEarnedState.InitSequence(dataModel);
		StartCoroutine(divisionEarnedState.PlayStateSequence(SetState));
	}

	private void InitPromotionProgressSequence(object dataModel)
	{
		DeactivateAllStates();
		promotionProgressState.gameObject.SetActive(true);
		promotionProgressState.InitSequence(dataModel);
		StartCoroutine(promotionProgressState.PlayStateSequence(SetState));
	}

	private void InitPromotionEarnedSequence(object dataModel)
	{
		DeactivateAllStates();
		promotionEarnedState.gameObject.SetActive(true);
		promotionEarnedState.InitSequence(dataModel);
		StartCoroutine(promotionEarnedState.PlayStateSequence(SetState));
	}

	private void InitPromotionEarnedBlueprintsSequence(object dataModel)
	{
		DeactivateAllStates();
		promotionEarnedBlueprintsState.gameObject.SetActive(true);
		promotionEarnedBlueprintsState.InitSequence(dataModel);
		StartCoroutine(promotionEarnedBlueprintsState.PlayStateSequence(SetState));
	}

	private void InitRewardAbilitySequence(object dataModel)
	{
		DeactivateAllStates();
		abilityRewardState.gameObject.SetActive(true);
		abilityRewardState.InitSequence(dataModel);
		StartCoroutine(abilityRewardState.PlayStateSequence(SetState));
	}

	private void InitEventRewardSequence(object dataModel)
	{
		DeactivateAllStates();
		eventRewardState.gameObject.SetActive(true);
		eventRewardState.InitSequence(dataModel);
		StartCoroutine(eventRewardState.PlayStateSequence(SetState));
	}

	private void DeactivateAllStates()
	{
		partsRewardState.gameObject.SetActive(false);
		divisionProgressState.gameObject.SetActive(false);
		divisionEarnedState.gameObject.SetActive(false);
		promotionProgressState.gameObject.SetActive(false);
		promotionEarnedState.gameObject.SetActive(false);
		promotionEarnedBlueprintsState.gameObject.SetActive(false);
		abilityRewardState.gameObject.SetActive(false);
		pvpResultsState.gameObject.SetActive(false);
		eventRewardState.gameObject.SetActive(false);
	}

	private void EndRewardsSequence()
	{
		if ((bool)nextButton)
		{
			nextButton.SetActive(true);
		}
		if (UserProfile.player.preferences.KamcordOn)
		{
			Kamcord.StopRecording();
		}
		if ((bool)battleButton)
		{
			battleButton.SetActive(Kamcord.IsEnabled() && _sceneModel.matchType != MatchData.Type.TUTORIAL && _sceneModel.matchType != MatchData.Type.AUTO_BOT_BATTLE);
		}
	}

	public bool GetBattleResultForPlayer()
	{
		return _sceneModel.isPlayerWinner;
	}

	public static void MakeObjectAppear(GameObject obj)
	{
		obj.transform.localScale = Vector3.zero;
		obj.transform.TweenLocalScale(1f, 1f);
	}

	public static void MakeObjectDisappear(GameObject obj)
	{
		obj.transform.TweenLocalScale(0f, 1f);
	}

	public void NextButtonClick()
	{
		nextButton.GetComponent<tk2dUIItem>().enabled = false;
		if (currentState == PostBattleRewardsStates.NONE)
		{
			FinishPostBattleFlow();
		}
		else
		{
			UserNotification.ExecuteNotifications(FinishPostBattleFlow);
		}
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.LocalUserNotificationsEnabled = true;
		}
	}

	public void BattleButtonClick()
	{
		if (UserProfile.player.preferences.KamcordOn)
		{
			Kamcord.SetTwitterDescription("kamcord_twitter_description".Localize());
			Kamcord.SetVideoTitle("kamcord_video_title".Localize());
			Kamcord.SetYouTubeSettings("kamcord_youtube_description".Localize(), "kamcord_youtube_tags".Localize());
			Kamcord.SetFacebookDescription("kamcord_facebook_description".Localize());
			Kamcord.SetDefaultEmailSubject("kamcord_email_subject".Localize());
			Kamcord.SetDefaultEmailBody("kamcord_email_body".Localize());
			Kamcord.ShowView();
			Reporting.KamcordShareIntent();
		}
		else
		{
			PopupManager.ShowPopup(PopupDataModel.NoYes("ui_popup_kamcord_disabled_title".Localize("Kamcord disabled!"), "ui_popup_kamcord_disabled_desc".Localize("To share videos, Kamcord must be enabled before the start of a battle, enable now?"), delegate
			{
				UserProfile.player.preferences.KamcordOn = true;
				battleButton.SetActive(false);
				PopupManager.ShowPopup(PopupDataModel.Ok("ui_popup_kamcord_enabled_notice_title".Localize("Kamcord Enabled!"), "ui_popup_kamcord_enabled_notice_desc".Localize("Kamcord was successfully enabled for future battles."), null));
			}));
		}
	}

	public override void OnBeginTransitionOut()
	{
		base.OnBeginTransitionOut();
		if (battleRewardSfx != null)
		{
			battleRewardSfx.Stop(0f);
		}
	}

	private void FinishPostBattleFlow()
	{
		SceneController.resumeCallbackEnable = true;
		SceneTransitionManager.ClearHistory();
		if (UserProfile.player.tutorial.CurrentStep <= TutorialStep.BuildFirstTank)
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BlueprintsScene, null, false);
			return;
		}
		if (_sceneModel.playerStats.isWinner)
		{
			OtherLevelsHelper.UsePlacement("battle_win", "Placement 1");
		}
		else
		{
			OtherLevelsHelper.UsePlacement("battle_lose", "Placement 2");
		}
		if (PopupManager.HasBackupState())
		{
			PopupManager.RestoreState(true);
			if ((bool)TopBarController.instance)
			{
				TopBarController.instance.ShowButtons = true;
			}
		}
		else
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaScene, null, false);
		}
	}

	private void SetBackgroundState(bool state)
	{
		if ((bool)loseBackgroundGameObject)
		{
			loseBackgroundGameObject.SetActive(!state);
		}
		if ((bool)winBackgroundGameObject)
		{
			winBackgroundGameObject.SetActive(state);
		}
	}

	private BattleRewardsSceneModel CreateFakeBattleRewards(bool fakeWin)
	{
		BattleRewardsSceneModel battleRewardsSceneModel = new BattleRewardsSceneModel(MatchData.Type.PVP, fakeWin, new ServerTeamStatsState(), new ServerTeamStatsState(), new OpponentData(), new OpponentData(), 100);
		battleRewardsSceneModel.isPlayerWinner = fakeWin;
		battleRewardsSceneModel.playerStats.baseCoins = ((!fakeWin) ? 5 : 15);
		battleRewardsSceneModel.playerStats.coinsFromUnitsDestroyed = ((!fakeWin) ? 10 : 20);
		battleRewardsSceneModel.playerStats.coinsFromUnitsSurvived = ((!fakeWin) ? 10 : 10);
		battleRewardsSceneModel.playerStats.coinsFromBestRolls = ((!fakeWin) ? 10 : 30);
		battleRewardsSceneModel.playerStats.coinsFromMultiKill = ((!fakeWin) ? 10 : 30);
		battleRewardsSceneModel.playerStats.coinsFromOverKills = ((!fakeWin) ? 10 : 35);
		battleRewardsSceneModel.playerStats.coinsFromPerfectKills = ((!fakeWin) ? 10 : 12);
		battleRewardsSceneModel.playerStats.partsEarned = new List<IPartMetadata>();
		battleRewardsSceneModel.enemyStats.partsEarned = new List<IPartMetadata>();
		List<IPartMetadata> list = new List<IPartMetadata>();
		list.AddRange(UnitPartsDataModel.GetAll().ConvertAll((Converter<UnitPartsDataModel, IPartMetadata>)((UnitPartsDataModel x) => x)));
		bool flag = false;
		foreach (UnitPartsDataModel item in list)
		{
			flag = false;
			foreach (UnitPartsDataModel item2 in battleRewardsSceneModel.playerStats.partsEarned)
			{
				if (item2.Name.Equals(item.Name))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				battleRewardsSceneModel.playerStats.partsEarned.Add(item);
			}
		}
		battleRewardsSceneModel.playerData.name = "Player!";
		battleRewardsSceneModel.playerData.winStreak = 2;
		battleRewardsSceneModel.playerData.division = new ProgressionDivisionDataModel();
		battleRewardsSceneModel.playerData.pvpRating = 1350;
		battleRewardsSceneModel.playerData.thumbnailURL = "http://mobage.com/system/games/icons/000/043/371/site/open-uri20141023-32542-156xlt8.png?1414041445";
		battleRewardsSceneModel.enemyData.name = "Enemy!";
		battleRewardsSceneModel.enemyData.winStreak = -1;
		battleRewardsSceneModel.enemyData.division = new ProgressionDivisionDataModel();
		battleRewardsSceneModel.enemyData.pvpRating = 1124;
		battleRewardsSceneModel.enemyData.thumbnailURL = "http://mobage.com/system/games/icons/000/043/370/site/open-uri20141023-32542-1p95aw8.png?1414040891";
		return battleRewardsSceneModel;
	}
}
