using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using UnityEngine;

public class BlueprintsScreenController : SceneController
{
	private const string DISABLED_TAB_SPRITE_NAME = "Secondary_button_grey_Frame";

	[SerializeField]
	private EventBlueprintsViewController eventBlueprintsViewController;

	[SerializeField]
	private ScrollableAreaController blueprintsScrollableAreaController;

	[SerializeField]
	private ScrollableAreaController exclusiveTanksScrollableAreaController;

	[SerializeField]
	private tk2dUIToggleButton exclusiveTanksToggleButton;

	[SerializeField]
	private tk2dUIToggleButton eventTanksToggleButton;

	[SerializeField]
	private tk2dUIScrollbar listScrollbar;

	[SerializeField]
	private tk2dTextMesh pulsingTanksLabel;

	[SerializeField]
	private tk2dUIToggleButtonGroup uiToggleButtonGroup;

	private Transform tutorialHighlightedObject;

	private tk2dUIItem scrollableAreaUIItem;

	private Tweener scrollTween;

	private bool initialized;

	private ScrollableAreaController currentScrollableAreaController;

	private UnitDataModel _unitAvailable;

	private void Start()
	{
		base.SectionTitle = "Blueprints";
		SceneTransitionManager.readyToTransitionIn = true;
		RefreshTutorialState();
		scrollableAreaUIItem = blueprintsScrollableAreaController.GetComponent<tk2dUIItem>();
		ToggleScrollingEnabled(false);
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
		}
	}

	private void Init()
	{
		CatalogType defaultCatalogType = GetDefaultCatalogType();
		ToggleScrollingEnabled(true);
		InitializeScrollableArea(defaultCatalogType != CatalogType.BLUEPRINTS);
		InitializeExclusiveTanksScrollableArea();
		InitializeEventTanksViewController();
		ShowTutorialSequence();
		UserProfile.player.OnResearchClaimed += RefreshTutorialState;
		AudioTrigger.Map_Music.PlayMusic();
		initialized = true;
		allowsBackButton = true;
		EnableScrollableListByCatalogType(defaultCatalogType);
		if ((bool)uiToggleButtonGroup)
		{
			uiToggleButtonGroup.SelectedIndex = (int)defaultCatalogType;
		}
		SetupPlayerProgress();
		StartCoroutine(OpenSelectedUnit(defaultCatalogType));
	}

	public void InitializeScrollableArea(bool instantScrollToDivision)
	{
		List<ProgressionDivisionDataModel> list = (from x in ProgressionDivisionDataModel.GetAll()
			where x.isHidden == 0
			select x).ToList();
		UserProfile player = UserProfile.player;
		int num = 0;
		List<ProgressionDivisionDataModel> list2 = new List<ProgressionDivisionDataModel>();
		GetCompletedTierRewards(list);
		foreach (ProgressionDivisionDataModel item in list)
		{
			if (!item.IsHidden)
			{
				num = int.Parse(item.id);
				List<UnitDataModel> unitsUnlockedAtTier = UnitDataModel.GetUnitsUnlockedAtTier(num);
				if (unitsUnlockedAtTier.Count != 0)
				{
					list2.Add(item);
				}
			}
		}
		blueprintsScrollableAreaController.InitializeWithData(list2);
		int num2 = GetClaimableUnitTierIndex();
		if (num2 == -1)
		{
			num2 = GetBuildableUnitTierIndex();
		}
		num = ((num2 != -1) ? num2 : int.Parse(player.CurrentDivision.ID));
		ScrollToDivision(num, instantScrollToDivision);
	}

	public void InitializeExclusiveTanksScrollableArea()
	{
		List<UnitDataModel> list = UnitDataModel.GetAll().ToList();
		List<UnitDataModel> list2 = new List<UnitDataModel>();
		int num = 0;
		int num2 = 0;
		foreach (UnitDataModel item in list)
		{
			if (item.UnitType.IsExclusive())
			{
				list2.Add(item);
				if (_unitAvailable != null && item.id == _unitAvailable.id)
				{
					num = Mathf.FloorToInt(num2 / 2);
				}
				num2++;
			}
		}
		exclusiveTanksScrollableAreaController.InitializeWithData(list2);
		if (num != 0)
		{
			float position = (float)num * exclusiveTanksScrollableAreaController.cellWidth;
			exclusiveTanksScrollableAreaController.ContentToPosition(position);
		}
	}

	public void InitializeEventTanksViewController()
	{
		if ((bool)eventBlueprintsViewController)
		{
			eventBlueprintsViewController.Init(UserProfile.player.GetActiveOnCooldownEvent());
		}
	}

	private CatalogType GetDefaultCatalogType()
	{
		CatalogType result = CatalogType.BLUEPRINTS;
		if (!UserProfile.player.tutorial.IsComplete)
		{
			return result;
		}
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		result = ((activeEvent != null) ? CatalogType.EVENT : CatalogType.BLUEPRINTS);
		if (UserProfile.player.divisionInt < Constants.MinTierEventContent)
		{
			result = CatalogType.BLUEPRINTS;
		}
		if (sceneModel != null)
		{
			_unitAvailable = (UnitDataModel)sceneModel.payload;
			if (_unitAvailable != null)
			{
				result = ((_unitAvailable.UnitType.IsEvent() && activeEvent != null) ? CatalogType.EVENT : (_unitAvailable.UnitType.IsExclusive() ? CatalogType.EXCLUSIVE : CatalogType.BLUEPRINTS));
			}
		}
		return result;
	}

	private int GetClaimableUnitTierIndex()
	{
		UserResearcher claimableResearcher = UserProfile.player.ClaimableResearcher;
		if (claimableResearcher != null && claimableResearcher.researchType == UserResearcher.ResearchType.BuildTank)
		{
			UserUnit userUnit = new UserUnit(claimableResearcher.itemID + "_00", claimableResearcher.itemID, 1, 0, string.Empty, string.Empty);
			return userUnit.UnitDataModel.unlockTier;
		}
		return -1;
	}

	private int GetBuildableUnitTierIndex()
	{
		if (UserProfile.player != null)
		{
			foreach (UserResearcher researcher in UserProfile.player.researchers)
			{
				if (researcher.researchType == UserResearcher.ResearchType.BuildTank)
				{
					UnitDataModel single = UnitDataModel.GetSingle(researcher.itemID);
					if (single != null)
					{
						return single.unlockTier;
					}
				}
			}
		}
		return -1;
	}

	private void GetCompletedTierRewards(List<ProgressionDivisionDataModel> allDivisions)
	{
		foreach (ProgressionDivisionDataModel allDivision in allDivisions)
		{
			if (allDivision.UnitsAllBuilt && !UserProfile.player.HasClaimedDivisionReward(int.Parse(allDivision.id)))
			{
				PopupManager.ShowPopup(PopupDataModel.TierReward(allDivision, delegate
				{
					blueprintsScrollableAreaController.Refresh();
				}));
			}
		}
	}

	private void ScrollToDivision(int divisionIndex, bool instant)
	{
		blueprintsScrollableAreaController.ScrollableArea.OnScroll += CompleteTween;
		float num = (float)(divisionIndex - 1) * blueprintsScrollableAreaController.cellWidth;
		if (instant)
		{
			blueprintsScrollableAreaController.ContentToPosition(num);
			return;
		}
		scrollTween = SimpleTween.Start(0f, num, 0.1f * (float)divisionIndex, EaseType.EaseOutExpo, delegate(float val)
		{
			if ((bool)blueprintsScrollableAreaController)
			{
				blueprintsScrollableAreaController.ContentToPosition(val);
			}
		});
	}

	private void CompleteTween(tk2dUIScrollableArea scrollableArea)
	{
		if (scrollTween != null)
		{
			scrollTween.Kill();
		}
		if ((bool)blueprintsScrollableAreaController)
		{
			blueprintsScrollableAreaController.ScrollableArea.OnScroll -= CompleteTween;
		}
	}

	public void OnDestroy()
	{
		if (UserProfile.player != null)
		{
			UserProfile.player.OnResearchClaimed -= RefreshTutorialState;
		}
	}

	public void Confirm()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
	}

	public void OnJoinClub()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ClubScene);
	}

	private void OnToggleGroupButtonChange(tk2dUIToggleButtonGroup toggleButton)
	{
		if (initialized)
		{
			CatalogType selectedIndex = (CatalogType)toggleButton.SelectedIndex;
			EnableScrollableListByCatalogType(selectedIndex);
		}
	}

	private void EnableScrollableListByCatalogType(CatalogType type)
	{
		switch (type)
		{
		case CatalogType.EXCLUSIVE:
			exclusiveTanksScrollableAreaController.gameObject.SetActive(true);
			blueprintsScrollableAreaController.gameObject.SetActive(false);
			eventBlueprintsViewController.gameObject.SetActive(false);
			currentScrollableAreaController = exclusiveTanksScrollableAreaController;
			break;
		case CatalogType.BLUEPRINTS:
			exclusiveTanksScrollableAreaController.gameObject.SetActive(false);
			blueprintsScrollableAreaController.gameObject.SetActive(true);
			eventBlueprintsViewController.gameObject.SetActive(false);
			currentScrollableAreaController = blueprintsScrollableAreaController;
			break;
		case CatalogType.EVENT:
			exclusiveTanksScrollableAreaController.gameObject.SetActive(false);
			blueprintsScrollableAreaController.gameObject.SetActive(false);
			eventBlueprintsViewController.gameObject.SetActive(true);
			eventBlueprintsViewController.LoadEventLogo();
			currentScrollableAreaController = eventBlueprintsViewController.eventUnitsScrollableArea;
			break;
		}
		if ((bool)listScrollbar)
		{
			listScrollbar.Value = currentScrollableAreaController.ScrollableArea.Value;
			if (currentScrollableAreaController.DataSource == null || currentScrollableAreaController.DataSource.Count == 0)
			{
				listScrollbar.gameObject.SetActive(false);
			}
			else
			{
				listScrollbar.gameObject.SetActive(true);
			}
		}
	}

	private void ShowTutorialSequence()
	{
		if (UserProfile.player.tutorial.CurrentStep == TutorialStep.ChangeName)
		{
			ToggleScrollingEnabled(false);
			PopupDataModel popupDataModel = new PopupDataModel();
			popupDataModel.closeButtonAction = TutorialBuild;
			PopupManager.ShowPopup(popupDataModel, SceneTransitionManager.Scene.PickYourName);
		}
		else if (UserProfile.player.tutorial.CurrentStep == TutorialStep.BuildFirstTank)
		{
			ToggleScrollingEnabled(false);
			StartCoroutine(TutorialBuildCoroutine());
		}
	}

	private void TutorialBuild()
	{
		UserProfile.player.tutorial.CurrentStep = TutorialStep.BuildFirstTank;
		StartCoroutine(TutorialBuildCoroutine());
	}

	private IEnumerator TutorialBuildCoroutine()
	{
		yield return StartCoroutine(AnnouncerController.DialogTrigger("Build"));
		List<TierCell> createdCells = blueprintsScrollableAreaController.GetCellComponents<TierCell>();
		if (createdCells.Count <= 0)
		{
			yield break;
		}
		TierCell first = createdCells[0];
		tutorialHighlightedObject = first.UnitCells[0].transform;
		BlueprintCell cell = tutorialHighlightedObject.GetComponent<BlueprintCell>();
		if ((bool)cell)
		{
			sceneModel.payload = cell;
		}
		EffectInstance instance = GlobalEffectsManager.Create(EffectType.UIOval, tutorialHighlightedObject.position, tutorialHighlightedObject);
		instance.gameObject.SetSortingOrder(10);
		instance.transform.localScale = Vector3.one * 1.25f;
		tk2dUIItem blueprintCellUIItem = tutorialHighlightedObject.GetComponent<tk2dUIItem>();
		if (!blueprintCellUIItem)
		{
			yield break;
		}
		blueprintCellUIItem.OnDown += delegate
		{
			scrollableAreaUIItem.enabled = true;
			tk2dUIScrollableArea component = scrollableAreaUIItem.GetComponent<tk2dUIScrollableArea>();
			if ((bool)component)
			{
				component.scrollBar.enabled = true;
			}
			if ((bool)instance)
			{
				instance.Destroy();
			}
		};
	}

	private void SetupPlayerProgress()
	{
		if (UserProfile.player == null)
		{
			return;
		}
		if (UserProfile.player.tutorial.CurrentStep > TutorialStep.BuildFirstTank)
		{
			if (int.Parse(UserProfile.player.CurrentDivision.ID) < Constants.ExclusiveTanksUnlockTier)
			{
				DisableExclusiveTanksTab();
			}
			if (UserProfile.player.GetActiveOnCooldownEvent() != null)
			{
				StartCoroutine(PulseTextColor());
			}
			if (UserProfile.player.divisionInt < Constants.MinTierEventContent)
			{
				DisableEventTanksTab();
			}
		}
		else
		{
			DisableExclusiveTanksTab();
			DisableEventTanksTab();
		}
	}

	private IEnumerator OpenSelectedUnit(CatalogType defaultCatalog)
	{
		if (sceneModel.payload == null || _unitAvailable == null)
		{
			yield break;
		}
		yield return new WaitForEndOfFrame();
		while (SceneTransitionManager.transitionActive)
		{
			yield return 0;
		}
		List<BlueprintCell> unitList = new List<BlueprintCell>();
		switch (defaultCatalog)
		{
		case CatalogType.BLUEPRINTS:
			unitList = blueprintsScrollableAreaController.GetCellComponents<BlueprintCell>();
			break;
		case CatalogType.EXCLUSIVE:
			unitList = exclusiveTanksScrollableAreaController.GetCellComponents<BlueprintCell>();
			break;
		}
		if (unitList.Count == 0)
		{
			yield break;
		}
		foreach (BlueprintCell exclusiveCell in unitList)
		{
			UnitDataModel cellDataModel = (UnitDataModel)exclusiveCell.DataObject;
			if (_unitAvailable.id == cellDataModel.id)
			{
				exclusiveCell.OnTouch();
			}
		}
	}

	private void DisableExclusiveTanksTab()
	{
		if ((bool)exclusiveTanksToggleButton)
		{
			tk2dBaseSprite componentInChildren = exclusiveTanksToggleButton.offStateGO.GetComponentInChildren<tk2dBaseSprite>();
			if ((bool)componentInChildren)
			{
				componentInChildren.SetSprite("Secondary_button_grey_Frame");
			}
			if ((bool)pulsingTanksLabel)
			{
				pulsingTanksLabel.color = Color.white;
			}
			exclusiveTanksToggleButton.uiItem.enabled = false;
		}
	}

	private void DisableEventTanksTab()
	{
		if ((bool)eventTanksToggleButton)
		{
			tk2dBaseSprite componentInChildren = eventTanksToggleButton.offStateGO.GetComponentInChildren<tk2dBaseSprite>();
			if ((bool)componentInChildren)
			{
				componentInChildren.SetSprite("Secondary_button_grey_Frame");
			}
			eventTanksToggleButton.uiItem.enabled = false;
		}
	}

	public void RefreshTutorialState()
	{
	}

	private IEnumerator PulseTextColor()
	{
		if ((bool)pulsingTanksLabel)
		{
			while (true)
			{
				pulsingTanksLabel.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time * 2f, 1f) + 0.5f);
				yield return new WaitForEndOfFrame();
			}
		}
	}

	public override void OnBackButton()
	{
		if (UserProfile.player.tutorial.CurrentStep > TutorialStep.BuildFirstTank)
		{
			base.OnBackButton();
		}
	}

	public void ToggleScrollingEnabled(bool enabled)
	{
		scrollableAreaUIItem.enabled = enabled;
		tk2dUIScrollableArea component = scrollableAreaUIItem.GetComponent<tk2dUIScrollableArea>();
		if ((bool)component)
		{
			component.scrollBar.enabled = enabled;
		}
	}
}
