using System.Collections;
using UnityEngine;

public class ArenaController : SceneController
{
	private UserProfile userProfile;

	[SerializeField]
	private ArenaHUDController hudController;

	private EffectInstance effectInstance;

	private bool isInTransitionOut;

	[SerializeField]
	private GameObject gachaBannerButton;

	[SerializeField]
	private PrefabProxy gachaBannerButtonPrefabProxy;

	[SerializeField]
	private GameObject notifcationObject;

	private bool buttonEnabled = true;

	private void Start()
	{
		AudioTrigger.MenuBackground_Music.PlayMusic();
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteIfStateEquals(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	public void Init()
	{
		userProfile = UserProfile.player;
		base.SectionTitle = "Team Select";
		allowsBackButton = true;
		hudController.Init(this);
		hudController.UpdateAbilityIcons(userProfile.CurrentAbilitySet.abilities);
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowButtons = true;
			TopBarController.instance.ShowProgressBanner = false;
			TopBarController.instance.RefreshProgress();
		}
		StartCoroutine(AnnouncerController.DialogTrigger("TutorialArena"));
		if (userProfile.wins + userProfile.losses == 1)
		{
			StartCoroutine(ShowTutorialSequence());
		}
		if (userProfile.divisionId == Constants.TellFriendDivisionId.ToString() && UserProfile.player.dialogTriggers.ShouldTriggerDialog("Tell A Friend"))
		{
			UserProfile.player.dialogTriggers.SetDialogTriggered(46);
			PopupManager.ShowPopup(PopupDataModel.TellAFriendPopUp());
		}
		if ((bool)gachaBannerButton)
		{
			gachaBannerButton.SetActive(false);
			StartCoroutine(SetupGachaBannerButton());
		}
		NotificationType notificationType = TopBarController.instance.MenuBar.UpdateNotificationForScene(SceneTransitionManager.Scene.ArenaScene);
		notifcationObject.SetActive(notificationType == NotificationType.RED);
	}

	private IEnumerator ShowTutorialSequence()
	{
		yield return StartCoroutine(AnnouncerController.DialogTrigger("Teaches Menu Nav"));
		if (AnnouncerController.HasDialogTrigger("Teaches Menu Nav"))
		{
			effectInstance = GlobalEffectsManager.Create(EffectType.UIOval, TopBarController.instance.MenuButton.transform.position, TopBarController.instance.MenuButton.transform);
			effectInstance.gameObject.SetSortingOrder(10);
			effectInstance.transform.localScale = Vector3.one * 0.4f;
			yield return new WaitForSeconds(0.5f);
			Pulse pulseComponent = effectInstance.gameObject.AddComponent<Pulse>();
			pulseComponent.minScale = 0.4f;
			pulseComponent.maxScale = 0.8f;
			pulseComponent.speed = 1f;
			yield return new WaitForSeconds(3f);
			Object.Destroy(pulseComponent);
			effectInstance.Destroy();
		}
	}

	public override void OnBeginTransitionOut()
	{
		base.OnBeginTransitionOut();
		isInTransitionOut = true;
		if ((bool)effectInstance)
		{
			effectInstance.Destroy();
		}
	}

	private void EditTeamButton()
	{
		EditTeamUnitsSceneModel editTeamUnitsSceneModel = new EditTeamUnitsSceneModel();
		editTeamUnitsSceneModel.selectedTeamIndex = hudController.cooldownsController.SelectedIndex;
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.EditTeamUnitsScene, editTeamUnitsSceneModel);
	}

	private void EditAbilitiesButton()
	{
		EditTeamAbilitiesSceneModel editTeamAbilitiesSceneModel = new EditTeamAbilitiesSceneModel();
		editTeamAbilitiesSceneModel.selectedTeamIndex = UserProfile.player.currentTeamIndex;
		EditTeamAbilitiesSceneModel sceneDM = editTeamAbilitiesSceneModel;
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.EditTeamAbilitiesScene, sceneDM);
	}

	private void GachaBannerButton()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
	}

	private void TankUpgradeButton()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.TankUpgradeScene);
	}

	private void BattleButton()
	{
		if (!buttonEnabled)
		{
			return;
		}
		buttonEnabled = false;
		if (UserProfile.player.CurrentTeam.GetUnitCount() < Constants.MinUnitsPerTeam)
		{
			EditTeamUnitsSceneModel sceneModel = new EditTeamUnitsSceneModel
			{
				selectedTeamIndex = UserProfile.player.currentTeamIndex
			};
			PopupManager.ShowPopup(PopupDataModel.Full("ui_popup_notenoughunits_title".Localize("Not enough units!"), string.Format("ui_popup_notenoughunits_desc".Localize("You must have {0} units to battle!"), Constants.MinUnitsPerTeam), "ui_topbar_team_button".Localize("Edit Team"), delegate
			{
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.EditTeamUnitsScene, sceneModel);
			}, "ui_popup_OK".Localize("OK"), delegate
			{
				buttonEnabled = true;
			}, PopUpTypes.GENERIC, delegate
			{
				buttonEnabled = true;
			}));
		}
		else if (userProfile.CurrentTeam.IsOnCooldown)
		{
			PopupManager.ShowPopup(PopupDataModel.SkipCooldownPopup(userProfile.CurrentTeam, delegate
			{
				hudController.UpdateUnitsTeam(userProfile.CurrentTeam);
				LoadBattleScene();
			}, delegate
			{
				buttonEnabled = true;
			}));
		}
		else
		{
			LoadBattleScene();
		}
	}

	private void LoadBattleScene()
	{
		Singleton<SessionManager>.instance.UpdateTeam();
		UserProfile.player.UpdateUnitEventItems();
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaSelectionScene, new ArenaSelectionSceneModel(activeEvent));
	}

	private void HUBButton()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
	}

	public void SelectTeam(int teamIndex)
	{
		if (!isInTransitionOut)
		{
			UserProfile player = UserProfile.player;
			player.currentTeamIndex = teamIndex;
			hudController.UpdateUnitsTeam(userProfile.teams[teamIndex]);
			hudController.UpdateAbilityIcons(userProfile.CurrentAbilitySet.abilities);
			if (userProfile.teams[teamIndex].IsOnCooldown)
			{
				AudioTrigger.RepairTank.Play();
			}
		}
	}

	private IEnumerator SetupGachaBannerButton()
	{
		if (!gachaBannerButtonPrefabProxy)
		{
			yield break;
		}
		AssetLinkageDataModel gachaBannerAssetLinkage = Constants.GachaBannerAssetLinkage;
		if (gachaBannerAssetLinkage != null)
		{
			yield return StartCoroutine(gachaBannerButtonPrefabProxy.ChangeAssetCoroutine(gachaBannerAssetLinkage));
			while (!gachaBannerButtonPrefabProxy.AssetReady)
			{
				yield return 0;
			}
			gachaBannerButton.SetActive(true);
		}
	}

	public override void OnBackButton()
	{
		if (allowsBackButton && !SceneTransitionManager.transitionActive && PopupManager.PopupCount == 0 && !SceneTransitionManager.PopScene())
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
		}
	}
}
