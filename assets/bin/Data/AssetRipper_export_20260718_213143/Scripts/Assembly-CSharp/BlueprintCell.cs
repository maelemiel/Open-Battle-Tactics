using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintCell : ScrollableCell
{
	private const string BUILD_BUTTON_ENABLED_SPRITE_NAME = "Normal_Button";

	private const string BUILD_BUTTON_DISABLED_SPRITE_NAME = "Inactive_Button";

	private const string BUILD_BUTTON_ENABLED_TEXT_SPRITE_NAME = "Build_Normal_Text";

	private const string BUILD_BUTTON_DISABLED_TEXT_SPRITE_NAME = "Build_Inactive_Text";

	private const string REGULAR_TANK_SPRITE_BACKGROUND = "BG_Edit_BorderHolder";

	private const string EVENT_TANK_SPRITE_BACKGROUND = "BG_Edit_BorderHolder_Gold";

	private const string EXCLUSIVE_TANK_SPRITE_BACKGROUND = "BG_Edit_BorderHolder_Exclusive";

	public static bool shouldSkipClaimAnimation = false;

	[SerializeField]
	private float REGULAR_BACKGROUND_WIDTH = 235f;

	[SerializeField]
	private float BUILDING_BACKGROUND_WIDTH = 425f;

	[SerializeField]
	private float INITIAL_UNIT_BACKGROUND_WIDTH = 192f;

	[SerializeField]
	private float CONSTRUCTING_UNIT_BACKGROUND_WIDTH = 380f;

	private Dictionary<int, string> unityRarityBanners = new Dictionary<int, string>
	{
		{ 1, "banner_bronze" },
		{ 2, "banner_silver" },
		{ 3, "banner_gold" },
		{ 4, "banner_purple" },
		{ 5, "banner_red" }
	};

	private Dictionary<int, int> bannerSizes = new Dictionary<int, int>
	{
		{ 1, 80 },
		{ 2, 105 },
		{ 3, 130 },
		{ 4, 155 },
		{ 5, 170 }
	};

	public static Color COLOR_NOT_READY_TO_BUILD = Color.gray;

	public static Color COLOR_READY_TO_BUILD = Color.white;

	public static Color COLOR_ALREADY_BUILT = new Color(0f, 0.67f, 0f);

	[SerializeField]
	private tk2dSlicedSprite unitBackground;

	[SerializeField]
	private tk2dSlicedSprite unitRarityBanner;

	[SerializeField]
	private tk2dSprite buildButtonSprite;

	[SerializeField]
	private tk2dSprite buildButtonTextMesh;

	[SerializeField]
	private tk2dClippedSprite clippedUnitSprite;

	[SerializeField]
	private tk2dSprite baseUnitSprite;

	[SerializeField]
	private tk2dUIItem buildButton;

	[SerializeField]
	private GameObject constructingState;

	[SerializeField]
	private tk2dTextMesh unitName;

	[SerializeField]
	private tk2dTextMesh unitRarity;

	[SerializeField]
	private tk2dTextMesh timesBuilt;

	[SerializeField]
	private ChartBarView rarityStarsView;

	[SerializeField]
	private tk2dSprite overlay;

	[SerializeField]
	private PriceLabelController priceLabel;

	[SerializeField]
	private tk2dSlicedSprite blueprintBackground;

	[SerializeField]
	private tk2dSlicedSprite sideBorderBackground;

	[SerializeField]
	private UnitProxy unitProxy;

	[SerializeField]
	private PrefabProxy blueprintProxy;

	[SerializeField]
	private tk2dSprite checkIcon;

	[SerializeField]
	private tk2dTextMesh researchText;

	[SerializeField]
	private BuildingEffect buildingEffect;

	[SerializeField]
	private tk2dTextMesh readyToBuildText;

	[SerializeField]
	private UnitIcon unitIcon;

	[SerializeField]
	public Color buildingUnitBaseSpriteColor;

	private BlueprintCellState currentCellState;

	public float pulseLength = 1.5f;

	private UserResearcher researcher;

	private void Update()
	{
		switch (currentCellState)
		{
		case BlueprintCellState.UNLOCKED:
			if (readyToBuildText.text != string.Empty)
			{
				readyToBuildText.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time, pulseLength * 0.5f));
			}
			break;
		case BlueprintCellState.BUILT:
			if (readyToBuildText.text != string.Empty)
			{
				readyToBuildText.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time, pulseLength));
			}
			break;
		case BlueprintCellState.BUILDING:
			if (researcher.CanClaim)
			{
				currentCellState = BlueprintCellState.BUILT;
				if (shouldSkipClaimAnimation)
				{
					shouldSkipClaimAnimation = false;
					SetCellState(currentCellState);
				}
				else if ((bool)buildingEffect)
				{
					buildingEffect.DeactivateEffect(delegate
					{
						SetCellState(currentCellState);
					});
				}
			}
			else
			{
				researchText.text = NonUnitySingleton<TimeManager>.instance.GetCountdownString(researcher.finishTime, true);
				UpdateBuildingTankEffect();
			}
			break;
		case BlueprintCellState.CLAIMED:
			break;
		}
	}

	public override void ConfigureCell()
	{
		if (dataObject == null)
		{
			return;
		}
		if (dataIndex > 0)
		{
			int num = 0;
			for (int i = 1; i <= dataIndex % controller.NUMBER_OF_COLUMNS; i++)
			{
				num = dataIndex - i;
				if (num < 0)
				{
					break;
				}
				if (controller.DataSource[num] == null)
				{
					base.transform.position += new Vector3((0f - controller.cellWidth) * 0.5f, 0f, 0f);
				}
			}
		}
		ConfigureCellData();
	}

	public override void ConfigureCellData()
	{
		if (dataObject == null)
		{
			return;
		}
		UnitDataModel unitDataModel = (UnitDataModel)dataObject;
		if ((bool)unitRarityBanner)
		{
			Vector2 dimensions = new Vector2(unitRarityBanner.dimensions.x, bannerSizes[unitDataModel.rarity]);
			unitRarityBanner.dimensions = dimensions;
			unitRarityBanner.SetSprite(unityRarityBanners[unitDataModel.rarity]);
		}
		researcher = null;
		if ((bool)unitName && dataObject != null)
		{
			unitName.color = Color.black;
			unitName.text = unitDataModel.name;
			unitRarity.text = unitDataModel.Rarity.Name;
		}
		if ((bool)rarityStarsView)
		{
			rarityStarsView.SetBarLevel(unitDataModel.rarity - 1);
		}
		if ((bool)unitIcon)
		{
			unitIcon.SetUnitIcon(unitDataModel.UnitType);
		}
		if (!UserProfile.player.HasUnlockedUnit(unitDataModel))
		{
			currentCellState = BlueprintCellState.LOCKED;
		}
		else
		{
			researcher = UserProfile.player.GetResearcher(UserResearcher.ResearchType.BuildTank, unitDataModel.id);
			if (researcher != null)
			{
				currentCellState = BlueprintCellState.BUILDING;
			}
			else if (UserProfile.player.HasBuiltUnit(unitDataModel.id))
			{
				currentCellState = BlueprintCellState.CLAIMED;
			}
			else
			{
				currentCellState = BlueprintCellState.UNLOCKED;
			}
		}
		SetupCellBackground();
		SetCellState(currentCellState);
	}

	private void SetCellState(BlueprintCellState cellState)
	{
		switch (cellState)
		{
		case BlueprintCellState.LOCKED:
			SetCellLocked();
			break;
		case BlueprintCellState.UNLOCKED:
			SetCellUnlocked();
			break;
		case BlueprintCellState.CLAIMED:
			SetUnitClaimed();
			break;
		case BlueprintCellState.BUILT:
			SetUnitBuilt();
			break;
		case BlueprintCellState.BUILDING:
			SetUnitBuilding();
			break;
		}
	}

	public void OnTouch()
	{
		if (SceneTransitionManager.transitionActive)
		{
			return;
		}
		UnitDataModel unitDataModel = (UnitDataModel)dataObject;
		switch (currentCellState)
		{
		case BlueprintCellState.UNLOCKED:
			if (UserProfile.player.tutorial.CurrentStep == TutorialStep.BuildFirstTank)
			{
				BlueprintCell blueprintCell = (BlueprintCell)SceneTransitionManager.CurrentSceneDM.payload;
				if ((bool)blueprintCell && blueprintCell == this)
				{
					PopupManager.ShowPopup(PopupDataModel.BuildUnitPopUp(unitDataModel, OnBuildTank).ShowCloseButton(false));
				}
			}
			else
			{
				PopupManager.ShowPopup(PopupDataModel.BuildUnitPopUp(unitDataModel, OnBuildTank));
			}
			break;
		case BlueprintCellState.CLAIMED:
			PopupManager.ShowPopup(PopupDataModel.BuildUnitPopUp(unitDataModel, OnBuildTank));
			break;
		case BlueprintCellState.BUILT:
			Log.Warning("Unit being claimed. Cancelling user input", base.gameObject);
			break;
		case BlueprintCellState.BUILDING:
			if (!researcher.CanClaim)
			{
				PopupManager.ShowPopup(PopupDataModel.SkipConstructionPopUp(researcher, null, ConfigureCellData));
				break;
			}
			Log.Warning("Unit being claimed. Cancelling user input", base.gameObject);
			break;
		case BlueprintCellState.LOCKED:
			if (UserProfile.player.tutorial.CurrentStep != TutorialStep.BuildFirstTank)
			{
				PopupManager.ShowPopup(PopupDataModel.InspectLockedUnitPopUp(unitDataModel, null));
			}
			break;
		}
	}

	private void OnBuildTank()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (UserProfile.player.tutorial.CurrentStep == TutorialStep.BuildFirstTank)
			{
				StartCoroutine(AnnouncerController.DialogTrigger("BuildFirstUnit"));
			}
			controller.Refresh();
			if ((bool)buildingEffect)
			{
				buildingEffect.ActivateEffect();
			}
		}
	}

	private void OnBuildTankAgain()
	{
		controller.Refresh();
		if ((bool)buildingEffect)
		{
			buildingEffect.ActivateEffect();
		}
	}

	private void ClaimTank()
	{
		UserProfile.player.TryClaimResearch(researcher, delegate
		{
			TopBarController.instance.UpdateNotifications();
			UnitDataModel unitDataModel = (UnitDataModel)dataObject;
			ProgressionDivisionDataModel single = ProgressionDivisionDataModel.GetSingle(unitDataModel.unlockTier);
			if (single != null && single.UnitsAllBuilt && !UserProfile.player.HasClaimedDivisionReward(int.Parse(single.id)))
			{
				PopupManager.ShowPopup(PopupDataModel.TierReward(single, delegate
				{
					if (parentCell != null)
					{
						parentCell.ConfigureCellData();
					}
				}));
			}
			if (UserProfile.player.tutorial.CurrentStep == TutorialStep.BuildFirstTank)
			{
				StartCoroutine(FirstTankTutorialSequence());
			}
			else
			{
				OtherLevelsHelper.UsePlacement("tank_built", "Placement 4", false);
			}
		});
		ConfigureCellData();
	}

	private IEnumerator FirstTankTutorialSequence()
	{
		yield return StartCoroutine(AnnouncerController.DialogTrigger("ClaimFirstUnit"));
		UserProfile.player.tutorial.CurrentStep = TutorialStep.Complete;
		OtherLevelsHelper.UsePlacement("tutorial_finish", "Placement 3");
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
	}

	private void SetCellLocked()
	{
		if ((bool)overlay)
		{
			overlay.gameObject.SetActive(true);
		}
		SetConstructingState(false);
		readyToBuildText.color = Color.red;
		readyToBuildText.text = "ui_blueprints_locked".Localize("Locked");
		if ((bool)priceLabel)
		{
			priceLabel.gameObject.SetActive(false);
		}
		checkIcon.gameObject.SetActive(false);
		SetTankBlueprintState(false);
		SetTankSpriteState(true);
		if ((bool)buildButton)
		{
			buildButton.gameObject.SetActive(false);
		}
		if ((bool)timesBuilt)
		{
			timesBuilt.gameObject.SetActive(false);
		}
	}

	private void SetCellUnlocked()
	{
		UnitDataModel unitDataModel = (UnitDataModel)dataObject;
		if ((bool)overlay)
		{
			overlay.gameObject.SetActive(false);
		}
		SetConstructingState(false);
		if ((bool)buildButton)
		{
			buildButton.gameObject.SetActive(false);
		}
		UserPriceDataModel buildPrice = unitDataModel.GetBuildPrice(UserProfile.player.TimesBuiltUnit(unitDataModel.id));
		if (!UserProfile.player.CanAfford(buildPrice))
		{
			readyToBuildText.text = string.Empty;
		}
		else
		{
			readyToBuildText.color = Color.green;
			readyToBuildText.text = "ui_blueprints_readytobuild".Localize("Ready to build!");
		}
		if ((bool)priceLabel)
		{
			priceLabel.gameObject.SetActive(true);
			priceLabel.ConfigurePriceLabel(buildPrice);
		}
		checkIcon.gameObject.SetActive(false);
		SetTankBlueprintState(false);
		SetTankSpriteState(true);
		if ((bool)timesBuilt)
		{
			timesBuilt.gameObject.SetActive(false);
		}
	}

	private void SetUnitClaimed()
	{
		if ((bool)overlay)
		{
			overlay.gameObject.SetActive(false);
		}
		SetConstructingState(false);
		readyToBuildText.text = string.Empty;
		if ((bool)priceLabel)
		{
			priceLabel.gameObject.SetActive(false);
		}
		checkIcon.gameObject.SetActive(true);
		SetTankBlueprintState(false);
		SetTankSpriteState(true);
		if ((bool)buildButton)
		{
			buildButton.gameObject.SetActive(false);
		}
		if ((bool)timesBuilt)
		{
			UnitDataModel unitDataModel = (UnitDataModel)dataObject;
			timesBuilt.gameObject.SetActive(true);
			timesBuilt.text = "ui_blueprints_timesbuilt".Localize("Times Built:") + " " + UserProfile.player.TimesBuiltUnit(unitDataModel.id);
		}
	}

	private void SetUnitBuilt()
	{
		if ((bool)overlay)
		{
			overlay.gameObject.SetActive(false);
		}
		SetConstructingState(false);
		if ((bool)blueprintBackground)
		{
			blueprintBackground.dimensions = new Vector2(BUILDING_BACKGROUND_WIDTH, blueprintBackground.dimensions.y);
		}
		if ((bool)priceLabel)
		{
			priceLabel.gameObject.SetActive(false);
		}
		checkIcon.gameObject.SetActive(false);
		readyToBuildText.color = Color.black;
		readyToBuildText.text = "ui_blueprints_built".Localize("Built!");
		SetTankBlueprintState(false);
		SetTankSpriteState(true);
		if ((bool)buildButton)
		{
			buildButton.gameObject.SetActive(false);
		}
		ClaimTank();
		if ((bool)timesBuilt)
		{
			timesBuilt.gameObject.SetActive(false);
		}
	}

	private void SetUnitBuilding()
	{
		readyToBuildText.text = string.Empty;
		researchText.text = NonUnitySingleton<TimeManager>.instance.GetCountdownString(researcher.finishTime);
		if ((bool)priceLabel)
		{
			priceLabel.gameObject.SetActive(false);
		}
		checkIcon.gameObject.SetActive(false);
		if ((bool)overlay)
		{
			overlay.gameObject.SetActive(false);
		}
		SetTankBlueprintState(false);
		SetTankSpriteState(false);
		SetConstructingState(true);
		if ((bool)buildButton)
		{
			buildButton.gameObject.SetActive(false);
		}
		if ((bool)timesBuilt)
		{
			timesBuilt.gameObject.SetActive(false);
		}
	}

	private float CalculateProgress(ItemCollectionDataModel itemPrice)
	{
		int num = 0;
		int num2 = 0;
		ItemCollectionDataModel.Item item = null;
		int num3 = 0;
		for (int i = 0; i < itemPrice.items.Count; i++)
		{
			item = itemPrice.items[i];
			num3 = UserProfile.player.inventory.GetItem(item.itemType, item.itemId);
			num += item.amount;
			num2 += Mathf.Min(item.amount, num3);
		}
		if (num == 0)
		{
			return 0f;
		}
		return (float)num2 / (float)num;
	}

	private void SetTankSpriteState(bool state)
	{
		UnitDataModel unitDataModel = (UnitDataModel)dataObject;
		if (state)
		{
			StartCoroutine(unitProxy.ChangeAssetCoroutine(unitDataModel.Levels[0].assetBundleId));
		}
		unitProxy.gameObject.SetActive(state);
	}

	private void SetTankBlueprintState(bool state)
	{
		UnitDataModel unitDataModel = (UnitDataModel)dataObject;
		if (state)
		{
			StartCoroutine(blueprintProxy.ChangeAssetCoroutine(unitDataModel.BlueprintLinkage));
		}
		blueprintProxy.gameObject.SetActive(state);
	}

	private void SetBuildingEffectState(bool state)
	{
		if ((bool)buildingEffect)
		{
			buildingEffect.gameObject.SetActive(state);
			if (state)
			{
				buildingEffect.ActivateIdleEffect();
			}
		}
	}

	private void SetBuildButtonSprite(bool state)
	{
		if ((bool)buildButton)
		{
			buildButton.enabled = state;
		}
		if ((bool)buildButtonSprite)
		{
			buildButtonSprite.SetSprite((!state) ? "Inactive_Button" : "Normal_Button");
		}
		if ((bool)buildButtonTextMesh)
		{
			buildButtonTextMesh.SetSprite((!state) ? "Build_Inactive_Text" : "Build_Normal_Text");
			buildButtonTextMesh.color = ((!state) ? COLOR_NOT_READY_TO_BUILD : Color.white);
		}
	}

	private void SetConstructingState(bool state)
	{
		if ((bool)constructingState)
		{
			constructingState.SetActive(state);
		}
		if ((bool)buildingEffect)
		{
			SetBuildingEffectState(state);
		}
		Vector2 zero = Vector2.zero;
		if ((bool)unitBackground)
		{
			zero = ((!state) ? new Vector2(INITIAL_UNIT_BACKGROUND_WIDTH, unitBackground.dimensions.y) : new Vector2(CONSTRUCTING_UNIT_BACKGROUND_WIDTH, unitBackground.dimensions.y));
			unitBackground.dimensions = zero;
		}
		if ((bool)blueprintBackground)
		{
			zero = ((!state) ? new Vector2(REGULAR_BACKGROUND_WIDTH, blueprintBackground.dimensions.y) : new Vector2(BUILDING_BACKGROUND_WIDTH, blueprintBackground.dimensions.y));
			blueprintBackground.dimensions = zero;
		}
		StartCoroutine(SetBuildingTankBlueprintState(state));
	}

	private IEnumerator SetBuildingTankBlueprintState(bool state)
	{
		UnitDataModel dataModel = (UnitDataModel)dataObject;
		if (state)
		{
			unitProxy.gameObject.SetActive(state);
			yield return StartCoroutine(unitProxy.ChangeAssetCoroutine("Prefab.prefab", dataModel.Levels[0].assetBundleId));
			while (!unitProxy.AssetReady)
			{
				yield return 0;
			}
			tk2dSprite tankSprite = unitProxy.Prefab.GetComponentInChildren<tk2dSprite>();
			tankSprite.renderer.enabled = false;
			if (!tankSprite)
			{
				yield break;
			}
			clippedUnitSprite.gameObject.SetActive(state);
			clippedUnitSprite.SetSprite(tankSprite.Collection, tankSprite.spriteId);
			baseUnitSprite.SetSprite(tankSprite.Collection, tankSprite.spriteId);
			for (int i = 0; i < unitProxy.staticPool.Count; i++)
			{
				if ((bool)unitProxy.staticPool[i])
				{
					GameObject staticCopy = Object.Instantiate(unitProxy.staticPool[i]) as GameObject;
					staticCopy.transform.parent = clippedUnitSprite.transform;
					staticCopy.transform.localPosition = Vector3.zero;
					staticCopy.transform.localScale = Vector3.one;
					staticCopy.GetComponent<tk2dSprite>().color = baseUnitSprite.color;
					unitProxy.staticPool[i].renderer.enabled = false;
				}
			}
		}
		else
		{
			clippedUnitSprite.gameObject.SetActive(state);
		}
	}

	private void UpdateBuildingTankEffect()
	{
		if (researcher != null)
		{
			clippedUnitSprite.ClipRect = new Rect(0f, 0f, 1f, researcher.Progress);
		}
	}

	private void SetupCellBackground()
	{
		UnitDataModel unitDataModel = (UnitDataModel)dataObject;
		if (unitDataModel != null)
		{
			if (unitDataModel.UnitType.IsExclusive())
			{
				SetBackgroundSprite("BG_Edit_BorderHolder_Exclusive");
			}
			else if (unitDataModel.UnitType.IsEvent())
			{
				SetBackgroundSprite("BG_Edit_BorderHolder_Gold");
			}
			else
			{
				SetBackgroundSprite("BG_Edit_BorderHolder");
			}
		}
	}

	private void SetBackgroundSprite(string spriteName)
	{
		if ((bool)blueprintBackground)
		{
			blueprintBackground.SetSprite(spriteName);
		}
		if ((bool)sideBorderBackground)
		{
			sideBorderBackground.SetSprite(spriteName);
		}
	}
}
