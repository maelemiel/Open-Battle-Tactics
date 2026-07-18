using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeUnitPartDetailPopUp : PopupController
{
	public class PayloadData
	{
		public UnitPartialLevelDataModel partialLevel;

		public UnitPartTypesDataModel partInfo;

		public int requiredAmount;

		public int index;

		public string unitId;

		public int currentPartialBitFlag;

		public PayloadData(UnitPartTypesDataModel partInfo, int requiredAmount, UnitPartialLevelDataModel partialLevel, string unitID, int index, int currentPartialBitFlag)
		{
			unitId = unitID;
			this.partialLevel = partialLevel;
			this.partInfo = partInfo;
			this.requiredAmount = requiredAmount;
			this.index = index;
			this.currentPartialBitFlag = currentPartialBitFlag;
		}
	}

	private enum PartJourneyState
	{
		None = 0,
		UnitBuilding = 1,
		UnitClaimable = 2,
		UnitAvailable = 3,
		OtherUnitBuilding = 4
	}

	[SerializeField]
	private PrefabProxy badgeProxy;

	[SerializeField]
	private PriceLabelController partPriceLabel;

	[SerializeField]
	private tk2dTextMesh partInventoryLabel;

	[SerializeField]
	private tk2dTextMesh partNameLabel;

	[SerializeField]
	private tk2dTextMesh tierLevelRequirement;

	[SerializeField]
	private GameObject equipButtonDisabledSprite;

	[SerializeField]
	private tk2dUIItem equipButton;

	[SerializeField]
	private GameObject equipContainer;

	[SerializeField]
	private GameObject getAllContainer;

	[SerializeField]
	private GameObject goPVPContainer;

	[SerializeField]
	private tk2dSprite partBackground;

	[SerializeField]
	private GameObject battleOnlyContainer;

	[SerializeField]
	private GameObject getTokenContainer;

	[SerializeField]
	private tk2dTextMesh getTokenInstructionText;

	[SerializeField]
	private GameObject getTokenButtonToken;

	[SerializeField]
	private GameObject getTokenButtonContracts;

	[SerializeField]
	private GameObject tierRequirementText;

	[SerializeField]
	private GameObject statIncreaseText;

	[SerializeField]
	private Transform detailPartAnchor;

	[SerializeField]
	private Transform basicPartAnchor;

	[SerializeField]
	private GameObject partInfoContainer;

	[SerializeField]
	private UnitPartialUpgradeView partialUpgradeView;

	[SerializeField]
	private GameObject requiredTierNoStatAnchor;

	[SerializeField]
	private GameObject tierBadgeNoStatAnchor;

	[SerializeField]
	private tk2dUIItem fightBotTeamButton;

	private PayloadData payloadData;

	private PartJourneyState currentPartJourneyState;

	protected override void Start()
	{
		base.Start();
		payloadData = (PayloadData)model.payload;
		ConfigureWithPart();
	}

	public void ConfigureWithPart()
	{
		UserPriceDataModel itemPriceFromPartDataModel = GetItemPriceFromPartDataModel(payloadData.partInfo, payloadData.requiredAmount);
		partPriceLabel.ConfigurePriceLabel(itemPriceFromPartDataModel);
		partInventoryLabel.text = string.Format("ui_part_inventory_display".Localize("{0}/{1}"), UserProfile.player.inventory.GetParts(payloadData.partInfo.id), payloadData.requiredAmount);
		partNameLabel.text = payloadData.partInfo.Name;
		bool flag = UserProfile.player.CanAfford(itemPriceFromPartDataModel);
		if (flag)
		{
			partInventoryLabel.color = Color.green;
		}
		else
		{
			partInventoryLabel.color = Color.red;
		}
		int num = ((payloadData.partialLevel != null) ? payloadData.partialLevel.requirementTier : 0);
		bool flag2 = num <= UserProfile.player.divisionInt;
		bool flag3 = UserProfile.player.CanAfford(itemPriceFromPartDataModel);
		bool flag4 = flag3 && flag2;
		equipButton.enabled = flag4;
		if (equipButtonDisabledSprite != null)
		{
			equipButtonDisabledSprite.SetActive(!flag4);
		}
		if (payloadData.partInfo.IsToken)
		{
			StartCoroutine(AnnouncerController.DialogTrigger("TokenInfoPopup"));
		}
		if (!flag2)
		{
			getTokenContainer.SetActive(false);
			equipContainer.SetActive(false);
			getAllContainer.SetActive(false);
			goPVPContainer.SetActive(true);
			StartCoroutine(AnnouncerController.DialogTrigger("FirstPartNotMeetTierRequirements"));
		}
		else if (payloadData.partInfo.IsToken && !flag3)
		{
			getTokenContainer.SetActive(true);
			equipContainer.SetActive(false);
			getAllContainer.SetActive(false);
			goPVPContainer.SetActive(false);
			tierRequirementText.SetActive(false);
			UnitLevelProgressionDataModel tokenGeneratingUnitProgression = payloadData.partInfo.TokenGeneratingUnitProgression;
			if (tokenGeneratingUnitProgression.UnitDataModel.foundInGacha == 1 || tokenGeneratingUnitProgression.UnitDataModel.UnitType.IsEvent())
			{
				getTokenInstructionText.text = string.Empty;
				getTokenButtonContracts.SetActive(true);
				getTokenButtonToken.SetActive(false);
			}
			else
			{
				getTokenButtonContracts.SetActive(false);
				getTokenButtonToken.SetActive(true);
				getTokenInstructionText.text = string.Format("ui_part_upgrade_tank_level_requirement".Localize("UPGRADE {0} TO LV {1}"), tokenGeneratingUnitProgression.UnitDataModel.name, tokenGeneratingUnitProgression.level);
			}
		}
		else
		{
			getTokenContainer.SetActive(false);
			equipContainer.SetActive(flag3);
			getAllContainer.SetActive(!flag3);
			goPVPContainer.SetActive(false);
			if ((bool)battleOnlyContainer)
			{
				battleOnlyContainer.SetActive(false);
			}
			getTokenInstructionText.gameObject.SetActive(false);
			if (!flag3)
			{
				StartCoroutine(AnnouncerController.DialogTrigger("FirstPartDetailNotEnough"));
			}
		}
		if (!flag)
		{
			partBackground.SetSprite("LevelUp_Blue_Up");
		}
		else
		{
			partBackground.SetSprite("LevelUp_Green_Up");
		}
		if (flag2)
		{
			tierRequirementText.SetActive(false);
			tierLevelRequirement.gameObject.SetActive(false);
			badgeProxy.gameObject.SetActive(false);
		}
		else
		{
			if (tierLevelRequirement != null)
			{
				tierLevelRequirement.text = string.Empty;
				tierLevelRequirement.color = ((num > UserProfile.player.divisionInt) ? Color.red : Color.green);
			}
			if (badgeProxy != null)
			{
				StartCoroutine(SetBadge(num));
			}
		}
		if (payloadData.partialLevel != null)
		{
			statIncreaseText.SetActive(true);
			equipButton.gameObject.SetActive(true);
			partInfoContainer.transform.position = detailPartAnchor.position;
			partialUpgradeView.gameObject.SetActive(true);
			partialUpgradeView.ConfigureUnitPartialUpgradeView(UnitDataModel.GetSingle(payloadData.partialLevel.unitId), payloadData.partialLevel.level, payloadData.currentPartialBitFlag, payloadData.partialLevel);
			if (!PartialLevelHasEffect(payloadData.partialLevel))
			{
				statIncreaseText.SetActive(false);
				partInfoContainer.transform.position = basicPartAnchor.position;
				partialUpgradeView.gameObject.SetActive(false);
				tierRequirementText.transform.position = requiredTierNoStatAnchor.transform.position;
				badgeProxy.transform.position = tierBadgeNoStatAnchor.transform.position;
			}
		}
		else
		{
			statIncreaseText.SetActive(false);
			equipButton.gameObject.SetActive(false);
			partInfoContainer.transform.position = basicPartAnchor.position;
			partialUpgradeView.gameObject.SetActive(false);
		}
		currentPartJourneyState = DeterminePartJourneyState();
	}

	public static bool PartialLevelHasEffect(UnitPartialLevelDataModel partialLevelDM)
	{
		return partialLevelDM.face1Value != 0 || partialLevelDM.face2Value != 0 || partialLevelDM.face3Value != 0 || partialLevelDM.face4Value != 0 || partialLevelDM.face5Value != 0 || partialLevelDM.hp != 0 || partialLevelDM.passiveBoostValueA != 0 || partialLevelDM.passiveBoostValueB != 0 || partialLevelDM.passiveId != 0 || partialLevelDM.specialId != 0;
	}

	public IEnumerator SetBadge(int tier)
	{
		ProgressionDivisionDataModel dataModel = ProgressionDivisionDataModel.GetSingle(Mathf.Max(1, tier));
		yield return StartCoroutine(badgeProxy.ChangeAssetCoroutine(dataModel.BadgeLinkage));
		yield return StartCoroutine(badgeProxy.WaitForAssetReady());
	}

	public void OnEquipPressed()
	{
		equipButton.enabled = false;
		PayloadData payloadData = (PayloadData)model.payload;
		if (UserProfile.player.CanAfford(GetItemPriceFromPartDataModel(payloadData.partInfo, payloadData.requiredAmount)))
		{
			Singleton<SessionManager>.instance.SubmitPartialUpgrade(payloadData.unitId, payloadData.partialLevel, delegate
			{
				OnRightButton();
				Close();
				StartCoroutine(AnnouncerController.DialogTrigger("AfterEquippingFirstPart"));
			});
		}
	}

	public void OnFightBotTeamPressed()
	{
		fightBotTeamButton.enabled = false;
		UserPriceDataModel itemPriceFromPartDataModel = GetItemPriceFromPartDataModel(payloadData.partInfo, payloadData.requiredAmount);
		List<AiArmyPartsDataModel> list = (from x in AiArmyPartsDataModel.GetAll()
			where x.partId.ToString() == payloadData.partInfo.id
			select x).ToList();
		if (list.Count == 0)
		{
			Reporting.MissingPartUpgrade("BotBattle_NoBots", itemPriceFromPartDataModel.PrintItems(), payloadData.unitId);
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_popup_nobotbattle_title".Localize("Not bot enable!"), "ui_popup_nobotbattle_message".Localize("We don't have bot enable for this part.")));
			fightBotTeamButton.enabled = true;
		}
		else
		{
			Reporting.MissingPartUpgrade("BotBattle", itemPriceFromPartDataModel.PrintItems(), payloadData.unitId);
			PopupManager.ShowPopup(PopupDataModel.BotBattlePopUp(payloadData.partInfo, delegate
			{
				fightBotTeamButton.enabled = true;
			}));
		}
	}

	public void OnPVPBattlePressed()
	{
		if (SceneTransitionManager.CurrentSceneDM._scene == SceneTransitionManager.Scene.EditTeamUnitsScene)
		{
			((EditTeamUnitsSceneController)SceneTransitionManager.CurrentSceneDM.controller).TriggerSaveToProfile();
		}
		PopupManager.ShowPopup(PopupDataModel.One("ui_popup_requiredtiernotmet_title".Localize("REQUIRED TIER NOT MET"), "ui_popup_requiredtiernotmet_body".Localize("You have not achieved the required tier to equip this part, gain tier points and win promotion series in Main Event Battles."), "ui_popup_requiredtiernotmet_button".Localize("MAIN EVENT"), SendToMainEvent, false));
	}

	public void SendToMainEvent()
	{
		PopupManager.DestroyPopup(PopupManager.CurrentPopupDM);
		if (UserProfile.player.CurrentTeam.GetUnitCount() < Constants.MinUnitsPerTeam)
		{
			EditTeamUnitsSceneModel sceneModel = new EditTeamUnitsSceneModel
			{
				selectedTeamIndex = UserProfile.player.currentTeamIndex
			};
			PopupManager.ShowPopup(PopupDataModel.Full("ui_popup_notenoughunits_title".Localize("Not enough units!"), string.Format("ui_popup_notenoughunits_desc".Localize("You must have {0} units to battle!"), Constants.MinUnitsPerTeam), "ui_topbar_team_button".Localize("Edit Team"), delegate
			{
				PopupManager.DestroyAllPopups();
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.EditTeamUnitsScene, sceneModel);
			}, "ui_popup_OK".Localize("OK"), null));
		}
		else if (UserProfile.player.CurrentTeam.IsOnCooldown)
		{
			PopupManager.ShowPopup(PopupDataModel.SkipCooldownPopup(UserProfile.player.CurrentTeam, delegate
			{
				PopupManager.BackupState(true);
				EventDataModel activeEvent2 = UserProfile.player.GetActiveEvent();
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaSelectionScene, new ArenaSelectionSceneModel(activeEvent2));
			}));
		}
		else
		{
			PopupManager.BackupState(true);
			EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaSelectionScene, new ArenaSelectionSceneModel(activeEvent));
		}
	}

	public void OnSwapNDropPressed()
	{
		PayloadData payloadData = (PayloadData)model.payload;
		UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(payloadData.partialLevel.requirementPriceId);
		List<GachaPoolsDataModel> list = (from x in GachaPoolsDataModel.GetAll()
			where x.GachaType == GachaTypes.ITEMS_MIXER
			orderby x.orderPosition descending
			select x).ToList();
		GachaPoolsDataModel firstMatching = list[0];
		List<GachaPlinkoPrizesDataModel> allPrizes = GachaPlinkoPrizesDataModel.GetAllPrizes(firstMatching.eventId, payloadData.partInfo.id);
		if (allPrizes.Count == 0)
		{
			Reporting.MissingPartUpgrade("DropAndSwap_NotAvailable", priceForID.PrintItems(), payloadData.unitId);
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_no_part_dns_title".Localize("Part Not Available"), "ui_no_part_dns_desc".Localize("Part not currently available through Drop n Swap.")));
			return;
		}
		Reporting.MissingPartUpgrade("DropAndSwap", priceForID.PrintItems(), payloadData.unitId);
		PopupManager.BackupState(false);
		PopupManager.DestroyAllPopups();
		GachaRewardsSceneModel gachaRewardsSceneModel = new GachaRewardsSceneModel(GachaTypes.ITEMS_MIXER);
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ItemsMixer, new ItemMixerSceneModel(gachaRewardsSceneModel, payloadData.partInfo.id, OnSwapNDropDone));
		Singleton<SessionManager>.instance.GetNextItemsMixerSet(firstMatching.eventId, delegate(ItemCollectionDataModel result)
		{
			gachaRewardsSceneModel.SetRewards(result, firstMatching);
		});
	}

	public void OnSwapNDropDone()
	{
		SceneTransitionManager.PopScene();
		PopupManager.RestoreState(false);
	}

	public void OnGetTokenPressed()
	{
		PayloadData payloadData = (PayloadData)model.payload;
		UnitLevelProgressionDataModel tokenGeneratingLevel = payloadData.partInfo.TokenGeneratingUnitProgression;
		List<UserUnit> list = UserProfile.player.unitInventory.Values.Where((UserUnit x) => x.metadataId == tokenGeneratingLevel.UnitDataModel.id && x.level < tokenGeneratingLevel.level).ToList();
		if (list.Count > 0)
		{
			PopupManager.ShowPopup(PopupDataModel.UpgradeUnitPopUp(list[0], OnUpgradeTokenTankClosed), true);
			return;
		}
		if (tokenGeneratingLevel.UnitDataModel.foundInGacha == 1)
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
			PopupManager.DestroyAllPopups();
			return;
		}
		if (tokenGeneratingLevel.UnitDataModel.UnitType.IsEvent())
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_upgrade_part_unitunavailable_title".Localize("Tank currently unavailable!"), "ui_upgrade_part_unitunavailable_desc".Localize("Tank currently unavailable, tank will become available in the future.")));
			return;
		}
		UserResearcher userResearcher = UserProfile.player.researchers[0];
		if (userResearcher.IsIdle)
		{
			PopupManager.ShowPopup(PopupDataModel.BuildUnitPopUp(tokenGeneratingLevel.UnitDataModel, OnBuildTank));
		}
		else if (userResearcher.CanClaim)
		{
			UserProfile.player.TryClaimResearch(userResearcher, OnGetTokenPressed);
		}
		else
		{
			PopupManager.ShowPopup(PopupDataModel.SkipConstructionPopUp(userResearcher, OnBuildTank));
		}
	}

	public void OnBuildTank()
	{
		StartCoroutine(OnBuildTankCoroutine());
	}

	public IEnumerator OnBuildTankCoroutine()
	{
		payloadData = (PayloadData)model.payload;
		if (base.gameObject == null)
		{
			Log.Error("Already Gone buddeh!");
			yield break;
		}
		if (currentPartJourneyState != DeterminePartJourneyState())
		{
			currentPartJourneyState = DeterminePartJourneyState();
			yield return new WaitForEndOfFrame();
			OnGetTokenPressed();
		}
		ConfigureWithPart();
	}

	public void OnUpgradeTokenTankClosed()
	{
		payloadData = (PayloadData)model.payload;
		ConfigureWithPart();
	}

	private UserPriceDataModel GetItemPriceFromPartDataModel(UnitPartTypesDataModel partInfo, int requiredAmount)
	{
		UserPriceDataModel userPriceDataModel = new UserPriceDataModel();
		userPriceDataModel.AddItem(UserInventory.ItemType.Parts, int.Parse(partInfo.id), requiredAmount);
		return userPriceDataModel;
	}

	private PartJourneyState DeterminePartJourneyState()
	{
		UnitLevelProgressionDataModel tokenGeneratingLevel = payloadData.partInfo.TokenGeneratingUnitProgression;
		if (tokenGeneratingLevel == null)
		{
			return PartJourneyState.None;
		}
		List<UserUnit> list = UserProfile.player.unitInventory.Values.Where((UserUnit x) => x.metadataId == tokenGeneratingLevel.UnitDataModel.id && x.level < tokenGeneratingLevel.level).ToList();
		if (list.Count > 0)
		{
			return PartJourneyState.UnitAvailable;
		}
		UserResearcher userResearcher = UserProfile.player.researchers[0];
		if (userResearcher.IsIdle)
		{
			return PartJourneyState.None;
		}
		if (userResearcher.CanClaim)
		{
			return PartJourneyState.UnitClaimable;
		}
		if (userResearcher.itemID == tokenGeneratingLevel.UnitDataModel.id)
		{
			return PartJourneyState.UnitBuilding;
		}
		return PartJourneyState.OtherUnitBuilding;
	}
}
