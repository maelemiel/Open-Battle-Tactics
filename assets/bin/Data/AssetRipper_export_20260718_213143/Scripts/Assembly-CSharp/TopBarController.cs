using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class TopBarController : MonoBehaviour
{
	public static TopBarController instance;

	[SerializeField]
	private TextMeshFixedLength coinsLabel;

	[SerializeField]
	private TextMeshFixedLength scrapLabel;

	[SerializeField]
	private tk2dTextMesh energyLabel;

	[SerializeField]
	private tk2dTextMesh totalEnergyLabel;

	[SerializeField]
	private Transform spriteCash;

	[SerializeField]
	private Transform spriteDiamond;

	[SerializeField]
	private Transform spriteTicket;

	[SerializeField]
	private GameObject menuButton;

	[SerializeField]
	private GameObject getPartsButton;

	[SerializeField]
	private GameObject chatButton;

	[SerializeField]
	private GameObject shopButton;

	[SerializeField]
	private GameObject settingsButton;

	[SerializeField]
	private Transform slideContainer;

	[SerializeField]
	private Transform closedMarker;

	[SerializeField]
	private tk2dCamera topBarCamera;

	[SerializeField]
	private ChatWindowController chatWindow;

	[SerializeField]
	private MenuBarController menuBarController;

	[SerializeField]
	private MenuBarNotificationsController menuBarNotificationsController;

	[SerializeField]
	private PlayerProgressController playerProgressController;

	[SerializeField]
	private UserLocalNotificationsController localUserNotificationsController;

	[SerializeField]
	private ShopWindowController shopWindowController;

	[SerializeField]
	private GameObject generalNotificationRed;

	[SerializeField]
	private GameObject generalNotificationGreen;

	[SerializeField]
	private GameObject clubChatNotificationGreen;

	[SerializeField]
	private EaseType showEaseType;

	[SerializeField]
	private EaseType hideEaseType;

	[SerializeField]
	private float showTime;

	[SerializeField]
	private float hideTime;

	[SerializeField]
	private AnnouncerController announcerController;

	[SerializeField]
	private GameObject ticketsButton;

	[SerializeField]
	private tk2dTextMesh ticketsTimer;

	[SerializeField]
	private tk2dClippedSprite ticketsSprite;

	[SerializeField]
	private tk2dTextMesh ticketsButtonLabel;

	[SerializeField]
	private Transform ticketsTransform;

	[SerializeField]
	private float ticketsRechargingLocalY;

	[SerializeField]
	private float ticketsFullLocalY = -7f;

	private bool isShowing = true;

	private string section = string.Empty;

	private SceneController currentScene;

	private bool shouldUpdateBuildCount;

	private Vector3 originalPosition;

	private bool menuBarAvailable = true;

	public float fireWorksOffset = 20f;

	private float fireWorksTimer;

	[SerializeField]
	private tk2dSpineAnimation[] fireworkAnimations;

	public bool Visible
	{
		get
		{
			return isShowing;
		}
		set
		{
			if (value)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
	}

	public bool ShowHomeButton
	{
		get
		{
			return menuButton.activeSelf;
		}
		set
		{
			menuButton.SetActive(value);
			if (value)
			{
				UpdateNotifications();
			}
			generalNotificationRed.renderer.enabled = value;
			generalNotificationGreen.renderer.enabled = value;
		}
	}

	public bool ShowGetPartsButton
	{
		get
		{
			return getPartsButton.activeSelf;
		}
		set
		{
			getPartsButton.SetActive(value);
		}
	}

	public bool ShowChatButton
	{
		get
		{
			return chatButton.activeSelf;
		}
		set
		{
			chatButton.SetActive(value);
		}
	}

	public bool ShowSettingsButton
	{
		get
		{
			return settingsButton.activeSelf;
		}
		set
		{
			settingsButton.SetActive(value);
		}
	}

	public bool ShowButtons
	{
		get
		{
			return ShowHomeButton || ShowGetPartsButton || ShowChatButton || ShowSettingsButton;
		}
		set
		{
			ShowHomeButton = value;
			ShowGetPartsButton = value;
			ShowChatButton = value;
			ShowSettingsButton = value;
		}
	}

	public bool LocalUserNotificationsEnabled
	{
		get
		{
			return localUserNotificationsController.enabled;
		}
		set
		{
			localUserNotificationsController.enabled = value;
			if (value)
			{
				localUserNotificationsController.EnableCheckingState();
			}
			else
			{
				localUserNotificationsController.DisableCheckingState();
			}
		}
	}

	public GameObject MenuButton
	{
		get
		{
			return menuButton;
		}
	}

	public GameObject ChatButton
	{
		get
		{
			return chatButton;
		}
	}

	public GameObject PersonalPartsButton
	{
		get
		{
			return getPartsButton;
		}
	}

	public GameObject ShopButton
	{
		get
		{
			return shopButton;
		}
	}

	public Transform EnergyTransform
	{
		get
		{
			return spriteTicket.transform;
		}
	}

	public Transform CoinsTransform
	{
		get
		{
			return spriteCash.transform;
		}
	}

	public Transform ScrapTransform
	{
		get
		{
			return spriteDiamond.transform;
		}
	}

	public tk2dCamera TopBarCamera
	{
		get
		{
			return topBarCamera;
		}
	}

	public string SectionTitle
	{
		get
		{
			return section;
		}
		set
		{
			section = value;
		}
	}

	public bool ShowProgressBanner
	{
		get
		{
			return playerProgressController.IsOpen;
		}
		set
		{
			playerProgressController.IsOpen = value;
		}
	}

	public AnnouncerController Announcer
	{
		get
		{
			return announcerController;
		}
	}

	public MenuBarController MenuBar
	{
		get
		{
			return menuBarController;
		}
		set
		{
			menuBarController = value;
		}
	}

	private void Awake()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(announcerController.gameObject);
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		originalPosition = slideContainer.transform.localPosition;
		Hide(0f);
		instance = this;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, Init);
	}

	private IEnumerator FireWorksCheck()
	{
		while (true)
		{
			fireWorksTimer += Time.deltaTime;
			if (fireWorksTimer > 1f && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(0)))
			{
				fireWorksTimer = 0f;
				StartCoroutine(ShowFireworksAnimation());
			}
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator ShowFireworksAnimation()
	{
		AudioTrigger.Fireworks.Play();
		for (int i = 0; i < fireworkAnimations.Length; i++)
		{
			fireworkAnimations[i].transform.position = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(fireWorksOffset);
			ActivateAndAutoDeactivateAnimation(fireworkAnimations[i]);
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void ActivateAndAutoDeactivateAnimation(tk2dSpineAnimation spineAnimation)
	{
		spineAnimation.AnimationComplete += ResetAnimation;
		spineAnimation.gameObject.SetActive(true);
	}

	private void ResetAnimation(tk2dSpineAnimation spineAnimation)
	{
		if ((bool)spineAnimation)
		{
			spineAnimation.AnimationComplete -= ResetAnimation;
			spineAnimation.Reset();
			spineAnimation.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (SceneController.fireWorksEnable)
		{
			fireWorksTimer += Time.deltaTime;
			if (fireWorksTimer > 1f && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(0)))
			{
				fireWorksTimer = 0f;
				StartCoroutine(ShowFireworksAnimation());
			}
		}
		if (UserProfile.player == null)
		{
			return;
		}
		long num = Math.Max(0L, UserProfile.player.energyRecoveryTime - TimeManager.ServerTime);
		if (num > 0)
		{
			ticketsButton.SetActive(true);
			ticketsTimer.gameObject.SetActive(true);
			if (ticketsTransform.localPosition.y != ticketsRechargingLocalY)
			{
				ticketsTransform.SetLocalYPosition(ticketsRechargingLocalY);
			}
			ticketsTimer.text = TimeFormats.GetTimeString(num % (Constants.EnergyRechargeSeconds * 1000), TimeFormat.NUMBER);
			ticketsButtonLabel.gameObject.SetActive(true);
			if ((bool)ticketsSprite)
			{
				if (UserProfile.player.energy == 0)
				{
					ticketsSprite.ClipRect = new Rect(0f, 0f, 1f, 1f - (float)num / 1000f % (float)Constants.EnergyRechargeSeconds / (float)Constants.EnergyRechargeSeconds);
				}
				else if (ticketsSprite.ClipRect.height < 1f)
				{
					ticketsSprite.ClipRect = new Rect(0f, 0f, 1f, 1f);
				}
			}
			return;
		}
		ticketsButton.SetActive(false);
		if (ticketsTransform.localPosition.y != ticketsFullLocalY)
		{
			ticketsTransform.SetLocalYPosition(ticketsFullLocalY);
		}
		if (ticketsTimer.gameObject.activeSelf)
		{
			ticketsTimer.gameObject.SetActive(false);
			if (ticketsSprite != null)
			{
				ticketsSprite.ClipRect = new Rect(0f, 0f, 1f, 1f);
			}
		}
		if (ticketsButtonLabel.gameObject.activeSelf)
		{
			ticketsButtonLabel.gameObject.SetActive(false);
		}
	}

	private void Init()
	{
		UserProfile player = UserProfile.player;
		player.OnCoinsChanged += HandleOnCoinsChanged;
		player.OnGemsChanged += HandleOnScrapChanged;
		player.OnEnergyChanged += HandleOnEnergyChanged;
		player.OnClubChatChanged += UpdateNotifications;
		player.OnPartsChanged += HandleOnPartsChanged;
		HandleOnCoinsChanged(player.coins, player.coins);
		HandleOnScrapChanged(player.gems, player.gems);
		HandleOnEnergyChanged(player.energy, player.energy);
		HandleOnPartsChanged();
		menuBarController.Init();
		SetTotalEnergy();
		if ((bool)playerProgressController)
		{
			playerProgressController.SetProgress(player);
		}
		if ((bool)localUserNotificationsController)
		{
			localUserNotificationsController.Init(this);
		}
		UpdateNotifications();
		UpdateClubChat();
		UpdatePendingClubCrates();
	}

	private void UpdateClubChat()
	{
		if (UserProfile.player == null || !UserProfile.player.IsClubMember)
		{
			return;
		}
		Singleton<SessionManager>.instance.GetChatMessage("0", true, delegate(List<ChatMessage> newMessages, string lastSequence, bool success)
		{
			if (!success)
			{
				CancelInvoke("UpdateClubChat");
			}
			else
			{
				UpdateNotifications();
			}
		});
		Invoke("UpdateClubChat", Constants.TopBarClubChatRefresh);
	}

	private void UpdatePendingClubCrates()
	{
		if (UserProfile.player != null && UserProfile.player.IsClubMember)
		{
			Singleton<SessionManager>.instance.GetPendingClubCrateCount(delegate(int crateCount)
			{
				if (this != null && UserProfile.player != null)
				{
					if (crateCount > UserProfile.player.pendingClubCrateCount)
					{
						LocalUserNotificationModel.ClubCrateReady notification = new LocalUserNotificationModel.ClubCrateReady();
						AddLocalNotification(notification);
					}
					UserProfile.player.pendingClubCrateCount = crateCount;
					UpdateNotifications();
				}
			});
		}
		Invoke("UpdatePendingClubCrates", Constants.PendingClubCratesCountRefresh);
	}

	public void Reinitialize()
	{
		Hide(0f);
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, Init);
	}

	public void RefreshProgress()
	{
		if ((bool)playerProgressController)
		{
			playerProgressController.SetProgress(UserProfile.player);
		}
	}

	private void SetTotalEnergy()
	{
		if ((bool)totalEnergyLabel)
		{
			totalEnergyLabel.text = "/" + UserProfile.player.maxEnergy;
		}
	}

	private void HandleOnEnergyChanged(int previous, int current)
	{
		if ((bool)energyLabel)
		{
			energyLabel.text = current.ToString();
		}
	}

	private void HandleOnScrapChanged(int previous, int current)
	{
		scrapLabel.Text = current.ToString();
	}

	private void HandleOnCoinsChanged(int previous, int current)
	{
		coinsLabel.Text = current.ToString();
	}

	private void HandleOnPartsChanged()
	{
		UpdateNotifications();
	}

	public void UpdateNotifications()
	{
		NotificationType notificationType = NotificationType.NONE;
		if ((bool)menuBarNotificationsController)
		{
			notificationType = menuBarNotificationsController.UpdateNotifications();
		}
		if ((bool)generalNotificationGreen)
		{
			generalNotificationGreen.SetActive(false);
		}
		if ((bool)generalNotificationRed)
		{
			generalNotificationRed.SetActive(false);
		}
		if ((notificationType & NotificationType.GREEN) > NotificationType.NONE)
		{
			if ((bool)generalNotificationGreen)
			{
				generalNotificationGreen.SetActive(true);
			}
		}
		else if ((bool)generalNotificationRed)
		{
			generalNotificationRed.SetActive((notificationType & NotificationType.RED) > NotificationType.NONE);
		}
		if (UserProfile.player.IsClubMember && !UserProfile.player.HasSeenLastClubMessage)
		{
			clubChatNotificationGreen.SetActive(true);
		}
		else
		{
			clubChatNotificationGreen.SetActive(false);
		}
	}

	public void OnSceneTransitionOut(SceneController sceneController)
	{
		currentScene = null;
		SectionTitle = string.Empty;
	}

	public void OnSceneTransitionIn(SceneController sceneController)
	{
		currentScene = sceneController;
	}

	public void Show(float time = -1f)
	{
		if (!isShowing)
		{
			isShowing = true;
			Invoke("UpdateClubChat", Constants.TopBarClubChatRefresh);
			Invoke("UpdatePendingClubCrates", Constants.PendingClubCratesCountRefresh);
			topBarCamera.camera.enabled = true;
			if ((bool)localUserNotificationsController)
			{
				localUserNotificationsController.EnableCheckingState();
			}
			if (time == -1f)
			{
				time = showTime;
			}
			HOTween.To(slideContainer, time, new TweenParms().Prop("localPosition", originalPosition).Ease(showEaseType).OnComplete(OnShowHideComplete));
		}
	}

	public void Hide(float time = -1f)
	{
		if (isShowing)
		{
			isShowing = false;
			CancelInvoke("UpdateClubChat");
			CancelInvoke("UpdatePendingClubCrates");
			topBarCamera.camera.enabled = true;
			if ((bool)localUserNotificationsController)
			{
				localUserNotificationsController.IsOpen = false;
			}
			if (time == -1f)
			{
				time = hideTime;
			}
			HOTween.To(slideContainer, time, new TweenParms().Prop("localPosition", closedMarker.localPosition).Ease(hideEaseType).OnComplete(OnShowHideComplete));
		}
	}

	private void OnShowHideComplete()
	{
		if (!isShowing)
		{
			topBarCamera.camera.enabled = false;
		}
	}

	public void OnClickedHome(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadHome));
	}

	public void LoadHome()
	{
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.HomeScene)
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
		}
	}

	public void OnClickedParts(tk2dUIItem button)
	{
		if (menuBarController.IsOpen)
		{
			menuBarController.IsOpen = false;
		}
		LoadParts();
	}

	public void LoadParts()
	{
		if (menuBarController.IsOpen)
		{
			menuBarController.IsOpen = false;
		}
		PopupManager.ShowPopup(PopupDataModel.PersonalPartsPopUp(null));
	}

	public void OnClickedChat(tk2dUIItem button)
	{
		if (menuBarController.IsOpen)
		{
			menuBarController.IsOpen = false;
		}
		string clubID = UserProfile.player.clubID;
		PopupManager.ShowPopup(PopupDataModel.ChatPopUp(clubID, null));
		clubChatNotificationGreen.SetActive(false);
	}

	public void LoadShop()
	{
		if (menuBarController.IsOpen)
		{
			menuBarController.IsOpen = false;
		}
		PopupManager.ShowPopup(PopupDataModel.ShopPopUp(null));
	}

	public void BuyEnergy()
	{
		BuyEnergyThenDoAction("ui_buy_tickets_title", "ui_buy_tickets_message", null);
	}

	public void BuyEnergyThenDoAction(string title, string message, Action callIfEnergyPurchased, Action onClose = null)
	{
		title = title.Localize("You need tickets!!!");
		message = message.Localize("You don't have enough tickets to battle. Do you want to buy some? (You will get X tickets)");
		if (UserProfile.player.energyRecoveryTime <= 0)
		{
			return;
		}
		UserPriceDataModel energyCost = new UserPriceDataModel(UserInventory.ItemType.PremiumCurrency, Constants.EnergyRestoreCost);
		PopupManager.ShowPopup(PopupDataModel.BuyItemPopUp(energyCost, title, message, Singleton<SessionManager>.instance.BuyEnergy, delegate
		{
			if (callIfEnergyPurchased != null)
			{
				bool flag = UserProfile.player.energy > 0;
				Reporting.TicketsDepletedEvent((!flag) ? "closed" : "energypurchased", energyCost);
				if (flag)
				{
					callIfEnergyPurchased();
				}
			}
			if (onClose != null)
			{
				onClose();
			}
		}));
	}

	public void BuyBulkEnergyThenDoAction(int energyAmount, int totalRequired, string title, string message, Action callIfEnergyPurchased)
	{
		title = title.Localize("You need tickets!!!");
		message = string.Format(message.Localize("GET {0} TICKETS?"), energyAmount);
		UserPriceDataModel energyCost = new UserPriceDataModel(UserInventory.ItemType.PremiumCurrency, Constants.EnergyRestoreCost * energyAmount);
		PopupManager.ShowPopup(PopupDataModel.BuyItemPopUp(energyCost, title, message, delegate
		{
			Singleton<SessionManager>.instance.BuyBulkEnergy(energyAmount);
		}, delegate
		{
			if (callIfEnergyPurchased != null)
			{
				bool flag = UserProfile.player.energy >= totalRequired;
				Reporting.TicketsDepletedEvent((!flag) ? "closed" : "bulkenergypurchased", energyCost);
				if (flag)
				{
					callIfEnergyPurchased();
				}
			}
		}));
	}

	public void OnClickedGacha(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadGacha));
	}

	public void LoadGacha()
	{
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.ShopItemsSuppliesScene)
		{
			Reporting.MenuNavigation("contracts");
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
		}
	}

	public void OnClickedMenu(tk2dUIItem button)
	{
		menuBarController.IsOpen = !menuBarController.IsOpen;
		if (!menuBarController.IsOpen)
		{
			return;
		}
		if (SceneTransitionManager.CurrentSceneDM != null)
		{
			menuBarController.SetSelectedItemMarker(SceneTransitionManager.CurrentSceneDM._scene);
		}
		if (UserProfile.player.tutorial.CurrentStep <= TutorialStep.BuildFirstTank)
		{
			for (int i = 0; i < menuBarController.ButtonCount; i++)
			{
				if (menuBarController.GetSceneButtonByIndex(i) != SceneTransitionManager.Scene.BlueprintsScene)
				{
					menuBarController.SetButtonState(i, false);
				}
			}
		}
		else
		{
			menuBarController.SetMenuButtonStates();
		}
	}

	public void OnClickedMenuBackground(tk2dUIItem button)
	{
		menuBarController.IsOpen = false;
	}

	public void OnClickedShopBackground(tk2dUIItem button)
	{
	}

	public void OnClickedBattle(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadBattle));
	}

	public void LoadBattle()
	{
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.ArenaScene)
		{
			Reporting.MenuNavigation("battle");
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaScene);
		}
	}

	public void OnClickedTeam(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadTeam));
	}

	public void LoadTeam()
	{
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.EditTeamUnitsScene)
		{
			EditTeamUnitsSceneModel editTeamUnitsSceneModel = new EditTeamUnitsSceneModel();
			editTeamUnitsSceneModel.selectedTeamIndex = UserProfile.player.currentTeamIndex;
			Reporting.MenuNavigation("edit_team");
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.EditTeamUnitsScene, editTeamUnitsSceneModel);
		}
	}

	public void OnClickedAbilities(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadAbilities));
	}

	public void LoadAbilities()
	{
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.EditTeamAbilitiesScene)
		{
			Reporting.MenuNavigation("abilities");
			EditTeamAbilitiesSceneModel editTeamAbilitiesSceneModel = new EditTeamAbilitiesSceneModel();
			editTeamAbilitiesSceneModel.selectedTeamIndex = UserProfile.player.currentTeamIndex;
			EditTeamAbilitiesSceneModel sceneDM = editTeamAbilitiesSceneModel;
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.EditTeamAbilitiesScene, sceneDM);
		}
	}

	public void OnClickedBuild(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadBuild));
	}

	public void LoadBuild()
	{
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.BlueprintsScene)
		{
			Reporting.MenuNavigation("build_tanks");
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BlueprintsScene);
		}
	}

	public void OnClickedSettings(tk2dUIItem button)
	{
		if (menuBarController.IsOpen)
		{
			menuBarController.IsOpen = false;
		}
		LoadSettings();
	}

	public void LoadSettings()
	{
		if (menuBarController.IsOpen)
		{
			menuBarController.IsOpen = false;
		}
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.SettingsScene)
		{
			Reporting.MenuNavigation("settings");
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.SettingsScene);
		}
	}

	public void OnClickedTankUpgrades(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadTankUpgrades));
	}

	public void LoadTankUpgrades()
	{
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.TankUpgradeScene)
		{
			Reporting.MenuNavigation("tankUpgrade");
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.TankUpgradeScene);
		}
	}

	public void OnClickedClubs(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadClubs));
	}

	public void LoadClubs()
	{
		if ((currentScene != null && !currentScene.OnHomeButton()) || SceneTransitionManager.CurrentSceneDM._scene == SceneTransitionManager.Scene.ClubScene)
		{
			return;
		}
		Reporting.MenuNavigation("clubs");
		if (UserProfile.player.pendingClubCrateCount > 0)
		{
			Singleton<SessionManager>.instance.GetClubCrates(delegate(List<ClubCrateDataModel> fetchedClubCrates)
			{
				if (fetchedClubCrates.Count > 0)
				{
					UserProfile.player.pendingClubCrateCount = 0;
					PopupManager.ShowPopup(PopupDataModel.ClubCratePopUp(fetchedClubCrates, delegate
					{
						SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ClubScene);
					}));
				}
				else
				{
					SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ClubScene);
				}
			});
		}
		else
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ClubScene);
		}
	}

	public void OnClickedContracts(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadContracts));
	}

	public void LoadContracts()
	{
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.ContractsScene)
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ContractsScene);
		}
	}

	public void OnClickedLeaderboards(tk2dUIItem button)
	{
		StartCoroutine(CloseMenuAndCallback(button, LoadLeaderboards));
	}

	public void LoadLeaderboards()
	{
		if ((!(currentScene != null) || currentScene.OnHomeButton()) && SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.LeaderboardsScene)
		{
			Reporting.MenuNavigation("leaderboards");
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.LeaderboardsScene);
		}
	}

	private IEnumerator CloseMenuAndCallback(tk2dUIItem button, Action callback)
	{
		if (menuBarAvailable)
		{
			menuBarAvailable = false;
			Transform selectedTransform = ((!(button != null)) ? null : button.gameObject.transform);
			StartCoroutine(menuBarController.CloseMenuButtons(0f, selectedTransform));
			yield return new WaitForSeconds(0.5f);
			menuBarController.IsOpen = false;
			menuBarAvailable = true;
			if (callback != null)
			{
				callback();
			}
		}
	}

	public void AddLocalNotification(LocalUserNotificationModel notification)
	{
		if ((bool)localUserNotificationsController)
		{
			localUserNotificationsController.AddNotification(notification);
			UpdateNotifications();
		}
	}

	public void Destroy()
	{
		StopAllCoroutines();
		instance = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && instance != null)
		{
			instance.CancelInvoke("UpdatePendingClubCrates");
			instance.UpdatePendingClubCrates();
		}
	}
}
