using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupDataModel
{
	public string title;

	public string message;

	public string inputText;

	public string leftLabel;

	public string rightLabel;

	public object payload;

	public GameObject viewObject;

	public bool showCloseButton = true;

	public Action leftAction;

	public Action rightAction;

	public Action<object> rightActionObject;

	public Action<string> confirmNameAction;

	public Action afterRemoveAction;

	public Action<object> afterRemoveActionObject;

	public Action closeButtonAction;

	public int id;

	public SceneTransitionManager.Scene popUpScene = SceneTransitionManager.Scene.PopupScene;

	public PopUpTypes popupType;

	public PopupController controller;

	public bool destroyWhenClicked = true;

	public bool isHelpPopup;

	public PopupDataModel ShowCloseButton(bool val)
	{
		showCloseButton = val;
		return this;
	}

	public static PopupDataModel Full(string title, string message, string leftLabel, Action leftAction, string rightLabel, Action rightAction, PopUpTypes type = PopUpTypes.GENERIC, Action closeAction = null, bool destroyWhenClicked = true)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.title = title;
		popupDataModel.message = message;
		popupDataModel.leftLabel = leftLabel;
		popupDataModel.leftAction = leftAction;
		popupDataModel.rightLabel = rightLabel;
		popupDataModel.rightAction = rightAction;
		popupDataModel.closeButtonAction = closeAction;
		popupDataModel.popupType = type;
		popupDataModel.destroyWhenClicked = destroyWhenClicked;
		return popupDataModel;
	}

	public static PopupDataModel FullLarge(string title, string message, string leftLabel, Action leftAction, string rightLabel, Action rightAction, PopUpTypes type = PopUpTypes.GENERIC, Action closeAction = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.title = title;
		popupDataModel.message = message;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.PopupSceneLarge;
		popupDataModel.leftLabel = leftLabel;
		popupDataModel.leftAction = leftAction;
		popupDataModel.rightLabel = rightLabel;
		popupDataModel.rightAction = rightAction;
		popupDataModel.closeButtonAction = closeAction;
		popupDataModel.popupType = type;
		return popupDataModel;
	}

	public static PopupDataModel CancelOk(string title, string message, Action okAction, Action cancelAction = null, Action closeAction = null)
	{
		return Full(title, message, "ui_popup_cancel".Localize("Cancel"), cancelAction, "ui_popup_OK".Localize("OK"), okAction, PopUpTypes.GENERIC, closeAction);
	}

	public static PopupDataModel NoYes(string title, string message, Action yesAction, Action noAction = null, Action closeAction = null)
	{
		return Full(title, message, "ui_popup_no".Localize("No"), noAction, "ui_popup_yes".Localize("Yes"), yesAction, PopUpTypes.GENERIC, closeAction);
	}

	public static PopupDataModel CancelCustom(string title, string message, string rightMessage, Action okAction, Action cancelAction = null, Action closeAction = null)
	{
		return Full(title, message, "ui_popup_cancel".Localize("Cancel"), cancelAction, rightMessage, okAction, PopUpTypes.GENERIC, closeAction);
	}

	public static PopupDataModel One(string title, string message, string buttonLabel, Action buttonAction, bool destroyWhenClicked = true)
	{
		bool flag = destroyWhenClicked;
		return Full(title, message, null, null, buttonLabel, buttonAction, PopUpTypes.GENERIC, null, flag);
	}

	public static PopupDataModel Ok(string title, string message)
	{
		return Full(title, message, null, null, "ui_popup_OK".Localize("OK"), null);
	}

	public static PopupDataModel OkLarge(string title, string message)
	{
		return FullLarge(title, message, null, null, "ui_popup_OK".Localize("OK"), null);
	}

	public static PopupDataModel Ok(string title, string message, Action okAction)
	{
		return Full(title, message, null, null, "ui_popup_OK".Localize("OK"), okAction);
	}

	public static PopupDataModel NetworkError(Action retry)
	{
		PopupManager.highDepThPopup = true;
		string text = "ui_network_error_title".Localize("Cannot Connect");
		string text2 = "ui_network_error_message".Localize("Please try again later.");
		string buttonLabel = "ui_network_error_button".Localize("OK");
		return One(text, text2, buttonLabel, QuitUtility.Restart).ShowCloseButton(false);
	}

	public static PopupDataModel FileIOError()
	{
		PopupManager.highDepThPopup = true;
		string text = "ui_io_error_title".Localize("Error");
		string text2 = "ui_io_error_message".Localize("Could not write to disk!");
		string buttonLabel = "ui_network_error_button".Localize("OK");
		return One(text, text2, buttonLabel, QuitUtility.Restart).ShowCloseButton(false);
	}

	public static PopupDataModel SessionExpiredError(Action retry)
	{
		PopupManager.highDepThPopup = true;
		string text = "ui_session_expired_title".Localize("Session Expired");
		string text2 = "ui_session_expire_message".Localize("Your session has expired. Press OK to restart.");
		string buttonLabel = "ui_session_expire_button".Localize("OK");
		return One(text, text2, buttonLabel, QuitUtility.Restart).ShowCloseButton(false);
	}

	public static PopupDataModel ProdURLError(Action retry)
	{
		PopupManager.highDepThPopup = true;
		string text = "ui_prod_url_error_title".Localize("Cannot Connect");
		string text2 = "ui_prod_url_error_message".Localize("Please try again later.");
		string buttonLabel = "ui_prod_url_error_button".Localize("OK");
		return One(text, text2, buttonLabel, retry).ShowCloseButton(false);
	}

	public static PopupDataModel VerboseNetworkError(string errorCode, string description, string message, Action callback)
	{
		return One("ui_network_error_title".Localize("Network Error"), "[Debug Code: " + errorCode + " Description: " + description + " Message: " + message, "ui_network_error_button".Localize("OK"), callback).ShowCloseButton(false);
	}

	public static PopupDataModel NetworkErrorClockSync(Action callback)
	{
		return One("ui_network_error_title".Localize("Network Error"), "ui_network_error_clocksync".Localize("Your device time is different from the server, please update your device time to the world time."), "ui_network_error_button".Localize("OK"), callback).ShowCloseButton(false);
	}

	public static PopupDataModel CannotAffordPrice(UserPriceDataModel price, Action okAction = null)
	{
		return Ok("Cannot afford!", "You need more " + price.items[0].itemType.ToString() + "!", okAction).ShowCloseButton(false);
	}

	public static PopupDataModel OkPreviewVersion(string title, string message, string rightLabel, Action okAction)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.title = title;
		popupDataModel.message = message;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.PopupScenePreview;
		popupDataModel.rightLabel = rightLabel;
		popupDataModel.rightAction = okAction;
		popupDataModel.leftLabel = null;
		popupDataModel.leftAction = null;
		popupDataModel.popupType = PopUpTypes.GENERIC;
		return popupDataModel;
	}

	public static PopupDataModel SkipWaitPopup(UserResearcher researcher, string title, string msgBody, Action onSkipWait = null, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = researcher;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.SkipWaitPopupScene;
		popupDataModel.title = title;
		popupDataModel.message = msgBody;
		popupDataModel.rightAction = onSkipWait;
		popupDataModel.leftAction = onPopUpClosed;
		popupDataModel.rightLabel = "ui_popup_yes".Localize("Yes");
		popupDataModel.leftLabel = "ui_popup_no".Localize("No");
		popupDataModel.popupType = PopUpTypes.SKIP_WAIT;
		return popupDataModel;
	}

	public static PopupDataModel SkipConstructionPopUp(UserResearcher researcher, Action onSkipWait = null, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = researcher;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.SkipConstructionPopup;
		popupDataModel.rightAction = onSkipWait;
		popupDataModel.leftAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.SKIP_CONSTRUCTION;
		return popupDataModel;
	}

	public static PopupDataModel SkipCooldownPopup(UserTeam userTeam, Action onSkipWait = null, Action onPopUpClosed = null, Action onCloseButtonAction = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = userTeam;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.SkipCooldownPopUpScene;
		popupDataModel.rightAction = onSkipWait;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.closeButtonAction = onCloseButtonAction;
		popupDataModel.rightLabel = "ui_popup_yes".Localize("Yes");
		popupDataModel.leftLabel = "ui_popup_no".Localize("No");
		popupDataModel.popupType = PopUpTypes.SKIP_COOLDOWN;
		return popupDataModel;
	}

	public static PopupDataModel PriceConfirmationPopUp(ItemCollectionDataModel price, string title, string msgBody, Action onAccept = null, Action onPopUpClosed = null, Action onPopUpDeclined = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.title = title;
		popupDataModel.message = msgBody;
		popupDataModel.payload = price;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.PriceConfirmationPopUp;
		popupDataModel.rightAction = onAccept;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.rightLabel = "ui_popup_yes".Localize("Yes");
		popupDataModel.leftLabel = "ui_popup_no".Localize("No");
		popupDataModel.popupType = PopUpTypes.PRICE_CONFIRMATION;
		popupDataModel.closeButtonAction = onPopUpDeclined;
		return popupDataModel;
	}

	public static PopupDataModel BuildUnitPopUp(UnitDataModel unitDataModel, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = unitDataModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.BlueprintsBuildPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.BUILD_UNIT;
		return popupDataModel;
	}

	public static PopupDataModel InspectUnitPopUp(UnitDataModel unitDataModel, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = unitDataModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.BlueprintsInspectTankPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.INSPECT_UNIT;
		return popupDataModel;
	}

	public static PopupDataModel UpgradeUnitPopUp(UserUnit unit, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = unit;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.UpgradeUnitPartsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.UPGRADE_UNIT_PARTS;
		Reporting.TankUpgradePopup(unit);
		return popupDataModel;
	}

	public static PopupDataModel UpgradeUnitPartDetailsPopUp(UnitPartTypesDataModel partInfo, int requiredAmount, int index, UnitPartialLevelDataModel partialLevel, string unitid, int currentPartialBitFlag, Action onPopUpClosed, Action<object> onEquip)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = new UpgradeUnitPartDetailPopUp.PayloadData(partInfo, requiredAmount, partialLevel, unitid, index, currentPartialBitFlag);
		popupDataModel.popUpScene = SceneTransitionManager.Scene.UpgradeUnitPartDetailPopUp;
		popupDataModel.rightActionObject = onEquip;
		popupDataModel.popupType = PopUpTypes.UPGRADE_UNIT_PART_DETAIL;
		return popupDataModel;
	}

	public static PopupDataModel InspectLockedUnitPopUp(UnitDataModel unitDataModel, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = unitDataModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.BlueprintsInspectLockedTankPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.INSPECT_UNIT_LOCKED;
		return popupDataModel;
	}

	public static PopupDataModel ClaimUnitPopUp(UnitDataModel unitDataModel, UnitPartTypesDataModel partType, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		if (unitDataModel != null)
		{
			popupDataModel.payload = unitDataModel;
		}
		else
		{
			popupDataModel.payload = partType;
		}
		popupDataModel.popUpScene = SceneTransitionManager.Scene.BlueprintsClaimTankPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.CLAIM_UNIT;
		return popupDataModel;
	}

	public static PopupDataModel InspectAbilityPopUp(AbilityDataModel abilityDataModel, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = abilityDataModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.AbilitiesInspectPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.INSPECT_ABIlITY;
		return popupDataModel;
	}

	public static PopupDataModel BuyItemPopUp(UserPriceDataModel price, string title, string message, Action onBuyItem = null, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = price;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.BuyItemPopUp;
		popupDataModel.rightAction = onBuyItem;
		popupDataModel.leftAction = onPopUpClosed;
		popupDataModel.title = title;
		popupDataModel.message = message;
		popupDataModel.rightLabel = "ui_popup_yes".Localize("Yes");
		popupDataModel.leftLabel = "ui_popup_no".Localize("No");
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.BUY_ITEM;
		return popupDataModel;
	}

	public static PopupDataModel TierReward(ProgressionDivisionDataModel divisionDataModel, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = divisionDataModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.TierRewardPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.TIER_REWARD;
		return popupDataModel;
	}

	public static PopupDataModel GachaResult(GachaRewardsSceneModel gachaUnits, string title, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = gachaUnits;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.GachaResultPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.title = title;
		popupDataModel.popupType = PopUpTypes.GACHA_RESULT;
		return popupDataModel;
	}

	public static PopupDataModel LeaderboardRewardsResult(LeaderboardRewardsSceneModel rewardSceneModel, string title, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = rewardSceneModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.LeaderboardRewardsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.title = title;
		popupDataModel.popupType = PopUpTypes.LEADERBOARD_REWARDS;
		return popupDataModel;
	}

	public static PopupDataModel ClubLeaderboardRewardsResult(LeaderboardRewardsSceneModel rewardSceneModel, string title, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = rewardSceneModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.ClubLeaderboardsRewardsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.title = title;
		popupDataModel.popupType = PopUpTypes.CLUB_LEADERBOARD_REWARDS;
		return popupDataModel;
	}

	public static PopupDataModel SoloLeaderboardRewardsResult(LeaderboardRewardsSceneModel rewardSceneModel, string title, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = rewardSceneModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.SoloLeaderboardRewardsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.title = title;
		popupDataModel.popupType = PopUpTypes.SOLO_LEADERBOARD_REWARDS;
		return popupDataModel;
	}

	public static PopupDataModel ClubRewardResult(LeaderboardRewardsSceneModel rewardSceneModel, string title, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = rewardSceneModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.ClubRewardsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.title = title;
		popupDataModel.popupType = PopUpTypes.CLUB_REWARDS_RESULT;
		return popupDataModel;
	}

	public static PopupDataModel SoloRewardResult(LeaderboardRewardsSceneModel rewardSceneModel, string title, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = rewardSceneModel;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.SoloRewardsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.title = title;
		popupDataModel.popupType = PopUpTypes.SOLO_REWARD_RESULT;
		return popupDataModel;
	}

	public static PopupDataModel LeaderboardEntry(string leaderboardId, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = leaderboardId;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.LeaderboardsEntryPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.LEADERBOARD_ENTRY;
		return popupDataModel;
	}

	public static PopupDataModel GachaInfoPopUp(string title, string description, object gachaPool, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.title = title;
		popupDataModel.message = description;
		popupDataModel.payload = gachaPool;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.GachaInfoPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.GACHA_INFO;
		return popupDataModel;
	}

	public static PopupDataModel ChatPopUp(string clubID, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = clubID;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.ChatPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.CHAT;
		return popupDataModel;
	}

	public static PopupDataModel ShopPopUp(Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.ShopWindowPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.SHOP;
		return popupDataModel;
	}

	public static PopupDataModel PersonalPartsPopUp(Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.PersonalPartsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.PERSONAL_PARTS;
		return popupDataModel;
	}

	public static PopupDataModel EventAlreadyClubPopUp(EventDataModel activeEvent, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = activeEvent;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.EventAlreadyClubPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.EVENT_ALREADY;
		return popupDataModel;
	}

	public static PopupDataModel EventJoinClubPopUp(EventDataModel activeEvent, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = activeEvent;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.EventJoinClubPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.EVENT_JOIN_CLUB;
		return popupDataModel;
	}

	public static PopupDataModel EventInfoPopUp(EventDataModel activeEvent, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = activeEvent;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.EventInfoPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.EVENT_INFO;
		return popupDataModel;
	}

	public static PopupDataModel ClubCratePopUp(List<ClubCrateDataModel> crateData, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.title = "ui_club_crate_received".Localize("YOU'VE RECEIVED A GIFT FROM:");
		popupDataModel.payload = crateData;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.ClubCratesPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.CLUB_CRATES;
		return popupDataModel;
	}

	public static PopupDataModel EventMultiTeamReport(UserMultiTeamReport.MultiTeamReport report, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.title = "ui_multiteam_popup_title".Localize("TEAM REPORT CARDS");
		popupDataModel.payload = report;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.MultiTeamReportPopUp;
		popupDataModel.showCloseButton = false;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.MULTI_TEAM_REPORT;
		return popupDataModel;
	}

	public static PopupDataModel EventMultiTeamInitial(Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.title = "ui_mutliteam_initial_popup_title".Localize("EARN MORE EVENT POINTS!");
		popupDataModel.popUpScene = SceneTransitionManager.Scene.MultiTeamInitialPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.MULTI_TEAM_INITIAL;
		return popupDataModel;
	}

	public static PopupDataModel EventMultiTeamFinal(UserMultiTeamReport userMultiTeamReport, Action onPopUpClosed)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.title = "ui_multiteam_final_report".Localize("FINAL REPORT");
		popupDataModel.payload = userMultiTeamReport;
		popupDataModel.popUpScene = SceneTransitionManager.Scene.MultiTeamFinalReportPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.MULTI_TEAM_FINAL;
		return popupDataModel;
	}

	public static PopupDataModel MobageConnectionError()
	{
		return Ok("ui_login_error_title".Localize("Connection error"), "ui_login_error_description".Localize("Please try again"), QuitUtility.Restart).ShowCloseButton(false);
	}

	public static PopupDataModel FacebookConnectionError(Action callback)
	{
		return Ok("ui_facebook_error_title".Localize("Facebook error"), "ui_facebook_error_description".Localize("There was a problem connecting to Facebook. Please try again."), callback).ShowCloseButton(false);
	}

	public static PopupDataModel MobageUpgradeUserConfirmation(Action callback)
	{
		return NoYes("ui_need_upgrade_title".Localize("Upgrade account"), "ui_need_upgrade_description".Localize("You need to upgrade your account to continue"), callback).ShowCloseButton(false);
	}

	public static PopupDataModel MobageUpgradeConflict(Action callback)
	{
		return Ok("ui_upgrade_fail_title".Localize("Upgrade account failed"), "ui_upgrade_fail_description".Localize("Error, the account you are upgrading to already has this game. Please re-login to that account."), callback).ShowCloseButton(false);
	}

	public static PopupDataModel MobageUpgradeUserSuccess(Action callback)
	{
		return Ok("ui_upgrade_success_title".Localize("Upgrade account success"), "ui_upgrade_success_description".Localize("Your account has been upgraded successfully!"), callback).ShowCloseButton(false);
	}

	public static PopupDataModel TellAFriendPopUp(Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.TellAFriendPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.TELL_A_FRIEND;
		return popupDataModel;
	}

	public static PopupDataModel AccountSettingsPopUp(Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.AccountSettingsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.ACCOUNT_SETTINGS;
		return popupDataModel;
	}

	public static PopupDataModel PerformanceSettingsPopUp(Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.PerformanceSettingsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.ACCOUNT_SETTINGS;
		return popupDataModel;
	}

	public static PopupDataModel PasswordPopUp(Action<string> passwordCallback = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.PasswordPopUpScene;
		popupDataModel.payload = passwordCallback;
		popupDataModel.popupType = PopUpTypes.PASSWORD;
		popupDataModel.title = "ui_popup_password_title".Localize("Password Required");
		popupDataModel.message = "ui_popup_password_description".Localize("This club is password protected");
		popupDataModel.rightLabel = "ui_popup_OK".Localize("OK");
		popupDataModel.leftLabel = "ui_popup_cancel".Localize("Cancel");
		return popupDataModel;
	}

	public static PopupDataModel NotificationSettingsPopUp(Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.NotificationSettingsPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.ACCOUNT_SETTINGS;
		return popupDataModel;
	}

	public static PopupDataModel BinaryUpdateRequired()
	{
		string text = "ui_binary_update_required_title".Localize("Update Required");
		string text2 = "ui_binary_update_required_message".Localize("Please update to the latest version");
		string buttonLabel = "ui_binary_update_required_button".Localize("Update");
		string url = GetBinaryUpdateRequiredUrlKey(AppConfig.platform).Localize("http://www.google.com");
		return One(text, text2, buttonLabel, delegate
		{
			Application.OpenURL(url);
		}, false).ShowCloseButton(false);
	}

	private static string GetBinaryUpdateRequiredUrlKey(AppConfig.PlatformType platformType)
	{
		switch (platformType)
		{
		case AppConfig.PlatformType.amazon:
			return "ui_binary_update_required_url_amazon";
		case AppConfig.PlatformType.ios:
			return "ui_binary_update_required_url_ios";
		default:
			return "ui_binary_update_required_url_android";
		}
	}

	public static PopupDataModel Maintenance()
	{
		PopupManager.highDepThPopup = true;
		string text = "ui_maintenance_mode_title".Localize("We'll be Right Back");
		string text2 = "ui_maintenance_mode_message".Localize("We're doing some maintenance right now.");
		return Ok(text, text2, QuitUtility.Restart).ShowCloseButton(false);
	}

	public static PopupDataModel OutOfSync(Action okAction)
	{
		string text = "ui_outofsync_mode_title".Localize("Out of Sync");
		string text2 = "ui_outofsync_mode_message".Localize("Please wait while your account is synchronized.");
		return Ok(text, text2, okAction).ShowCloseButton(false);
	}

	public static PopupDataModel DataModelUpdateRequired()
	{
		string text = "ui_datamodelupdaterequired_title".Localize("Data update available");
		string text2 = "ui_datamodelupdaterequired_message".Localize("Please wait while your game updates to the latest data.");
		return Ok(text, text2, QuitUtility.Restart).ShowCloseButton(false);
	}

	public static PopupDataModel TimeOutForfeit(Action cleanUpForfeit)
	{
		PopupManager.highDepThPopup = true;
		string text = "ui_timeoutfortfeit_mode_title".Localize("Match timed out!");
		string text2 = "ui_timeoutfortfeit_mode_message".Localize("You have timed out from your match and have forfeited.");
		return Ok(text, text2, cleanUpForfeit).ShowCloseButton(false);
	}

	public static PopupDataModel Revive(Action Continue, Action Quit)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.RevivePopup;
		popupDataModel.rightAction = Quit;
		popupDataModel.closeButtonAction = Quit;
		popupDataModel.leftAction = Continue;
		popupDataModel.rightLabel = "ui_popup_quit".Localize("QUIT");
		popupDataModel.leftLabel = "ui_popup_fight".Localize("FIGHT");
		popupDataModel.popupType = PopUpTypes.REVIVE;
		return popupDataModel;
	}

	public static PopupDataModel NormalTicketBoostPopUp(Action<BoostType> onBattle, Action Quit = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.rightAction = Quit;
		popupDataModel.closeButtonAction = Quit;
		popupDataModel.payload = new TicketBoostPopupModel(onBattle);
		popupDataModel.popUpScene = SceneTransitionManager.Scene.NormalTicketBoostPopup;
		popupDataModel.popupType = PopUpTypes.NORMAL_TICKET_BOOST;
		return popupDataModel;
	}

	public static PopupDataModel PointEventTicketBoostPopUp(Action<BoostType> onBattle)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = new TicketBoostPopupModel(onBattle);
		popupDataModel.popUpScene = SceneTransitionManager.Scene.PointEventTicketBoostPopup;
		popupDataModel.popupType = PopUpTypes.EVENT_TICKET_BOOST;
		return popupDataModel;
	}

	public static PopupDataModel PvpEventTicketBoostPopUp(Action<BoostType> onBattle)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = new TicketBoostPopupModel(onBattle);
		popupDataModel.popUpScene = SceneTransitionManager.Scene.PvpEventTicketBoostPopup;
		popupDataModel.popupType = PopUpTypes.EVENT_TICKET_BOOST;
		return popupDataModel;
	}

	public static PopupDataModel RaidBossTicketBoostPopUp(Action<BoostType> onBattle)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = new TicketBoostPopupModel(onBattle);
		popupDataModel.popUpScene = SceneTransitionManager.Scene.RaidBossTicketBoostPopUp;
		popupDataModel.popupType = PopUpTypes.RAID_BOSS_TICKET_BOOST;
		return popupDataModel;
	}

	public static PopupDataModel BountyBoard(Action<BattleSceneModel> battle, Action Quit)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.BountyBoardPopup;
		popupDataModel.payload = battle;
		popupDataModel.afterRemoveAction = Quit;
		popupDataModel.popupType = PopUpTypes.BOUNTY_BOARD;
		return popupDataModel;
	}

	public static PopupDataModel RaidBossDefeatedPopUp(Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.RaidBossDefeatedPopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.RAID_BOSS_DEFEATED;
		return popupDataModel;
	}

	public static PopupDataModel BotBattlePopUp(UnitPartTypesDataModel partInfo, Action onPopUpClosed = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.payload = new BotBattlePopUpController.PayloadData(partInfo);
		popupDataModel.popUpScene = SceneTransitionManager.Scene.BotBattlePopUp;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.popupType = PopUpTypes.BOT_BATTLE;
		return popupDataModel;
	}

	public static PopupDataModel ResetBotBattleCountPopUp(Action onYes = null, Action onPopUpClosed = null, Action onCloseButtonAction = null)
	{
		PopupDataModel popupDataModel = new PopupDataModel();
		popupDataModel.popUpScene = SceneTransitionManager.Scene.ResetBotBattleCapPopUpScene;
		popupDataModel.rightAction = onYes;
		popupDataModel.afterRemoveAction = onPopUpClosed;
		popupDataModel.closeButtonAction = onCloseButtonAction;
		popupDataModel.rightLabel = "ui_popup_OK".Localize("OK");
		popupDataModel.leftLabel = "ui_popup_no".Localize("No");
		popupDataModel.popupType = PopUpTypes.SKIP_COOLDOWN;
		return popupDataModel;
	}
}
