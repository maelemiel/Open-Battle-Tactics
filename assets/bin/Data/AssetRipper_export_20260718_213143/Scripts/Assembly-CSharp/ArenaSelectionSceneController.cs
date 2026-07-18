using System.Collections;
using System.IO;
using UnityEngine;

public class ArenaSelectionSceneController : SceneController
{
	private BattleSceneModel battleSceneModel;

	[SerializeField]
	private BaseMatchTypeView stateViewAI;

	[SerializeField]
	private BaseMatchTypeView stateViewPVP;

	[SerializeField]
	private BaseMatchTypeView stateViewRaidBoss;

	private MatchData.Type currentType = MatchData.Type.PVP;

	[SerializeField]
	private tk2dUIItem battleButton;

	[SerializeField]
	private tk2dTextMesh pvpButtonText;

	[SerializeField]
	private tk2dTextMesh pvpButtonText2;

	[SerializeField]
	private ProTipsViewController proTipsViewController;

	[SerializeField]
	private tk2dBaseSprite battleTicket;

	[SerializeField]
	private EventLogoController eventLogoController;

	[SerializeField]
	private EventBackgroundController eventBackgroundController;

	[SerializeField]
	private GameObject raidBossTab;

	[SerializeField]
	private tk2dUIToggleButtonGroup toggleButtonGroup;

	public override void Awake()
	{
		allowsBackButton = true;
		base.Awake();
		base.SectionTitle = "Match Select";
		if (sceneModel != null)
		{
			battleSceneModel = sceneModel.payload as BattleSceneModel;
		}
		if (battleSceneModel == null)
		{
			battleSceneModel = new BattleSceneModel();
			battleSceneModel.matchType = currentType;
		}
		stateViewPVP.SetEnabled(false);
		stateViewAI.SetEnabled(false);
		stateViewRaidBoss.SetEnabled(false);
		RaidBossMatchTypeView raidBossMatchTypeView = (RaidBossMatchTypeView)stateViewRaidBoss;
		if ((bool)raidBossMatchTypeView)
		{
			raidBossMatchTypeView.playRaidBossCallback = BattleRaidBossCallback;
		}
		tk2dUIToggleButton[] toggleBtns = toggleButtonGroup.ToggleBtns;
		foreach (tk2dUIToggleButton tk2dUIToggleButton2 in toggleBtns)
		{
			tk2dUIToggleButton2.uiItem.enabled = false;
		}
	}

	private IEnumerator Start()
	{
		UserProfile userProfile = UserProfile.player;
		UpdateMatchDataTypeView();
		if (userProfile.GetActiveEvent() != null && userProfile.divisionInt >= Constants.MinTierEventContent)
		{
			yield return StartCoroutine(SetPVPButtonEventState());
		}
		else if (userProfile.IsInPromoSeries)
		{
			yield return StartCoroutine(SetPVPButtonPromotionState());
		}
		EventDataModel activeEvent = userProfile.GetActiveEvent();
		if (activeEvent != null && userProfile.divisionInt >= Constants.MinTierEventContent)
		{
			raidBossTab.SetActive(activeEvent.EventType == EventDataModel.EventTypes.RAIDBOSS_EVENT);
		}
		else
		{
			raidBossTab.SetActive(false);
		}
		SceneTransitionManager.readyToTransitionIn = true;
		AudioTrigger.Map_Music.PlayMusic();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	protected override void _OnEndTransitionIn()
	{
		base._OnEndTransitionIn();
		tk2dUIToggleButton[] toggleBtns = toggleButtonGroup.ToggleBtns;
		foreach (tk2dUIToggleButton tk2dUIToggleButton2 in toggleBtns)
		{
			tk2dUIToggleButton2.uiItem.enabled = true;
		}
	}

	private void Init()
	{
		TopBarController.instance.RefreshProgress();
		TopBarController.instance.ShowProgressBanner = false;
		if (AskForRating.ShouldPromptForRating())
		{
			AskForRating.PromptForRating();
		}
		if ((bool)proTipsViewController)
		{
			proTipsViewController.Init(LinesType.PRO_TIPS);
		}
		ShowEventContentPopUp();
		SetTabActive();
	}

	private void SetTabActive()
	{
		ArenaSelectionSceneModel arenaSelectionSceneModel = (ArenaSelectionSceneModel)sceneModel;
		if (arenaSelectionSceneModel.forceTabIndex >= 0 && toggleButtonGroup != null)
		{
			toggleButtonGroup.SelectedIndex = arenaSelectionSceneModel.forceTabIndex;
			switch (arenaSelectionSceneModel.forceTabIndex)
			{
			case 0:
				currentType = MatchData.Type.PVP;
				battleSceneModel.matchType = MatchData.Type.PVP;
				break;
			case 1:
				currentType = MatchData.Type.AI;
				battleSceneModel.matchType = MatchData.Type.AI;
				break;
			case 2:
				currentType = MatchData.Type.RAIDBOSS;
				battleSceneModel.matchType = MatchData.Type.RAIDBOSS;
				break;
			default:
				currentType = MatchData.Type.PVP;
				battleSceneModel.matchType = MatchData.Type.PVP;
				break;
			}
			UpdateMatchDataTypeView();
		}
		if (arenaSelectionSceneModel.eventDataModel != null)
		{
			EventDataModel.EventTypes eventType = arenaSelectionSceneModel.eventDataModel.EventType;
			if (eventType == EventDataModel.EventTypes.RAIDBOSS_EVENT && (bool)toggleButtonGroup && UserProfile.player.divisionInt >= Constants.MinTierEventContent)
			{
				toggleButtonGroup.SelectedIndex = 2;
				currentType = MatchData.Type.RAIDBOSS;
				battleSceneModel.matchType = MatchData.Type.RAIDBOSS;
				UpdateMatchDataTypeView();
			}
		}
	}

	private void ShowEventContentPopUp()
	{
		UserProfile player = UserProfile.player;
		EventDataModel activeEvent = player.GetActiveEvent();
		KeyValueStorage keyValueStorage = KeyValueStorage.Instance(KeyValueStorage.Storage.USER_PROFILE);
		if (keyValueStorage != null && player != null && activeEvent != null && player.divisionInt >= Constants.MinTierEventContent)
		{
			int num = -1;
			if (keyValueStorage.ContainsKey(EventAlreadyClubPopUpPopUpController.EVENT_ALREADY_POP_UP_SHOWN_KEY_VALUE))
			{
				num = keyValueStorage.GetValue<int>(EventAlreadyClubPopUpPopUpController.EVENT_ALREADY_POP_UP_SHOWN_KEY_VALUE);
			}
			if (num != int.Parse(activeEvent.id))
			{
				keyValueStorage.SetValue(EventAlreadyClubPopUpPopUpController.EVENT_ALREADY_POP_UP_SHOWN_KEY_VALUE, int.Parse(activeEvent.id));
				StartCoroutine(PlayVideo(activeEvent.id));
			}
		}
	}

	private void UpdateMatchDataTypeView()
	{
		switch (currentType)
		{
		case MatchData.Type.AI:
			stateViewAI.SetEnabled(true);
			stateViewPVP.SetEnabled(false);
			stateViewRaidBoss.SetEnabled(false);
			battleButton.gameObject.SetActive(true);
			stateViewAI.DataObject = UserProfile.player.NextAIArmies;
			break;
		case MatchData.Type.PVP:
			stateViewAI.SetEnabled(false);
			stateViewPVP.SetEnabled(true);
			stateViewRaidBoss.SetEnabled(false);
			battleButton.gameObject.SetActive(true);
			stateViewPVP.DataObject = UserProfile.player.CurrentDivision;
			break;
		case MatchData.Type.RAIDBOSS:
			stateViewAI.SetEnabled(false);
			stateViewPVP.SetEnabled(false);
			stateViewRaidBoss.SetEnabled(true);
			battleButton.gameObject.SetActive(false);
			stateViewRaidBoss.DataObject = null;
			break;
		case MatchData.Type.TUTORIAL:
			break;
		}
	}

	private void OnPVPButtonPressed(tk2dUIToggleButton buttonUIItem)
	{
		if (buttonUIItem.IsOn)
		{
			currentType = MatchData.Type.PVP;
			battleSceneModel.matchType = MatchData.Type.PVP;
			UpdateMatchDataTypeView();
		}
	}

	private void OnAIButtonPressed(tk2dUIToggleButton buttonUIItem)
	{
		if (buttonUIItem.IsOn)
		{
			currentType = MatchData.Type.AI;
			battleSceneModel.matchType = MatchData.Type.AI;
			UpdateMatchDataTypeView();
		}
	}

	private void OnRaidBossButtonPressed(tk2dUIToggleButton buttonUIItem)
	{
		if (buttonUIItem.IsOn)
		{
			currentType = MatchData.Type.RAIDBOSS;
			battleSceneModel.matchType = MatchData.Type.RAIDBOSS;
			UpdateMatchDataTypeView();
		}
	}

	private void OnBattleButtonPressed()
	{
		if (UserProfile.player == null)
		{
			return;
		}
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		bool flag = UserProfile.player.divisionInt >= Constants.MinTierEventTicketBoostPopUp;
		if (currentType == MatchData.Type.AI)
		{
			BattleButtonLogic(BoostType.NoBoost);
			return;
		}
		if (currentType == MatchData.Type.PVP && activeEvent != null && activeEvent.EventType == EventDataModel.EventTypes.POINTS_EVENT && flag)
		{
			PopupManager.ShowPopup(PopupDataModel.PointEventTicketBoostPopUp(delegate(BoostType boostType)
			{
				BattleButtonLogic(boostType);
			}));
			return;
		}
		if (currentType == MatchData.Type.PVP && activeEvent != null && activeEvent.EventType == EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT && flag)
		{
			PopupManager.ShowPopup(PopupDataModel.PvpEventTicketBoostPopUp(delegate(BoostType boostType)
			{
				BattleButtonLogic(boostType);
			}));
			return;
		}
		if (activeEvent != null && activeEvent.EventType == EventDataModel.EventTypes.RAIDBOSS_EVENT && flag && battleSceneModel.matchType == MatchData.Type.RAIDBOSS)
		{
			PopupManager.ShowPopup(PopupDataModel.RaidBossTicketBoostPopUp(delegate(BoostType boostType)
			{
				BattleButtonLogic(boostType);
			}));
			return;
		}
		bool flag2 = activeEvent != null && activeEvent.EventType == EventDataModel.EventTypes.RAIDBOSS_EVENT && currentType != MatchData.Type.AI;
		if (UserProfile.player.divisionInt >= Constants.MinTierForShowBoostSelection && !flag2)
		{
			PopupManager.ShowPopup(PopupDataModel.NormalTicketBoostPopUp(delegate(BoostType boostType)
			{
				BattleButtonLogic(boostType);
			}));
		}
		else if (UserProfile.player.energy >= 1)
		{
			BattleButtonLogic(BoostType.NoBoost);
		}
		else
		{
			TopBarController.instance.BuyEnergyThenDoAction("ui_tickets_depleted_title", "ui_buy_tickets_message", delegate
			{
				BattleButtonLogic(BoostType.NoBoost);
			});
		}
	}

	private void BattleButtonLogic(BoostType boostType)
	{
		int loadingPopupId = LoadingPopupManager.ShowLoadingPopup(0f);
		if (boostType != BoostType.NoBoost)
		{
			Singleton<SessionManager>.instance.PurchaseBoost(BoostDataModel.GetBoostByType(boostType), false, battleSceneModel.matchType.ToString(), delegate
			{
				LoadingPopupManager.ClearLoadingPopup(loadingPopupId);
				loadingPopupId = -1;
				StartBattleLogic();
			});
		}
		else
		{
			LoadingPopupManager.ClearLoadingPopup(loadingPopupId);
			loadingPopupId = -1;
			StartBattleLogic();
		}
	}

	private void StartBattleLogic()
	{
		if (UserProfile.player.GetActiveEvent() != null)
		{
			UpdateClubInfo();
		}
		battleButton.enabled = false;
		toggleButtonGroup.gameObject.SetActive(false);
		TopBarController.instance.Visible = false;
		AudioTrigger.ScrapEarned.Play();
		StopAllCoroutines();
		StartCoroutine(AnimateTicketAndConfirm());
	}

	private void UpdateClubInfo()
	{
		if (!string.IsNullOrEmpty(UserProfile.player.clubID))
		{
			Singleton<SessionManager>.instance.GetClub(delegate(UserClub fetchedUserClub)
			{
				if (fetchedUserClub != null)
				{
					UserProfile.player.userClub = fetchedUserClub;
				}
			});
		}
		else
		{
			Singleton<SessionManager>.instance.GetSoloEventPoints();
		}
	}

	private void SetPVPButtonLabel(string keyName, Color textColor)
	{
		LocalizeTextMesh localizeTextMesh = null;
		if ((bool)pvpButtonText)
		{
			pvpButtonText.color = textColor;
			localizeTextMesh = pvpButtonText.GetComponent<LocalizeTextMesh>();
			if ((bool)localizeTextMesh)
			{
				localizeTextMesh.TextKey = keyName;
			}
		}
		if ((bool)pvpButtonText2)
		{
			localizeTextMesh = null;
			localizeTextMesh = pvpButtonText2.GetComponent<LocalizeTextMesh>();
			if ((bool)localizeTextMesh)
			{
				localizeTextMesh.TextKey = keyName;
			}
			pvpButtonText2.text = keyName.Localize("Promotion");
		}
	}

	private IEnumerator SetPVPButtonPromotionState()
	{
		SetPVPButtonLabel("ui_promotion_text", Color.green);
		if ((bool)eventLogoController)
		{
			eventLogoController.gameObject.SetActive(false);
		}
		StartCoroutine(PulseTextColor());
		yield break;
	}

	private IEnumerator SetPVPButtonEventState()
	{
		SetPVPButtonLabel("ui_arena_selection_special_event_tab", Color.white);
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		if ((bool)eventLogoController)
		{
			yield return StartCoroutine(eventLogoController.LoadLogoCoroutine(activeEvent));
		}
		if ((bool)eventBackgroundController)
		{
			yield return StartCoroutine(eventBackgroundController.LoadBackgroundCoroutine(activeEvent));
		}
		StartCoroutine(PulseTextColor());
	}

	private IEnumerator PulseTextColor()
	{
		if ((bool)pvpButtonText)
		{
			while (true)
			{
				pvpButtonText.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time * 2f, 1f) + 0.5f);
				yield return new WaitForEndOfFrame();
			}
		}
	}

	public void Confirm()
	{
		if (currentType == MatchData.Type.AI)
		{
			battleSceneModel.difficulty = ((AIMatchTypeViewController)stateViewAI).SelectedTeamIndex;
		}
		KeyValueStorage.Instance(KeyValueStorage.Storage.LAST_BATTLE_TYPE).SetValueAsync("battleType", (int)battleSceneModel.matchType);
		if (battleSceneModel.matchType == MatchData.Type.PVP || battleSceneModel.matchType == MatchData.Type.RAIDBOSS)
		{
			battleSceneModel.activeEvent = UserProfile.player.GetActiveEvent();
		}
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.AudiencePanShotScene, new SceneModel
		{
			payload = battleSceneModel
		});
	}

	private IEnumerator AnimateTicketAndConfirm()
	{
		if (currentType != MatchData.Type.RAIDBOSS)
		{
			if ((bool)battleTicket)
			{
				battleTicket.TweenAlpha(0f, 0.5f);
				battleTicket.TweenScale(1.3f, 0.5f);
			}
			yield return new WaitForSeconds(0.5f);
		}
		Confirm();
	}

	public void OnBackToArena()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaScene);
	}

	public void BattleRaidBossCallback(BattleSceneModel sceneModel = null)
	{
		if (sceneModel != null)
		{
			battleSceneModel = sceneModel;
		}
		else
		{
			battleSceneModel.matchType = MatchData.Type.PVP;
		}
		OnBattleButtonPressed();
	}

	private IEnumerator PlayVideo(string eventID)
	{
		SceneController.resumeCallbackEnable = false;
		string videoFileName = Path.Combine(Application.persistentDataPath, eventID + "intro.mp4");
		if (File.Exists(videoFileName))
		{
			Handheld.PlayFullScreenMovie(videoFileName, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFill);
			Reporting.EventIntroDisplay(eventID);
		}
		else
		{
			yield return StartCoroutine(DownloadVideo(Constants.GetEventIntroMovieURL, videoFileName));
			if (File.Exists(videoFileName))
			{
				Handheld.PlayFullScreenMovie(videoFileName, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFill);
				Reporting.EventIntroDisplay(eventID);
			}
		}
		SceneController.resumeCallbackEnable = true;
		yield return null;
	}

	public void PlayVideoButton()
	{
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		if (activeEvent != null)
		{
			StartCoroutine(PlayVideo(activeEvent.id));
		}
	}
}
