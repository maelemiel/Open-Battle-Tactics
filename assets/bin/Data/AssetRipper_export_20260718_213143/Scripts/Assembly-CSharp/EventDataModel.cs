using System;
using System.Collections.Generic;
using System.Globalization;

public class EventDataModel : BaseLocalizationDataModel
{
	public enum EventTypes
	{
		WINS_EVENT = 0,
		POINTS_EVENT = 1,
		RAIDBOSS_EVENT = 2,
		PVP_TOURNAMENT_EVENT = 3
	}

	private long cachedStartTimeStamp = -1L;

	private long cachedEndTimeStamp = -1L;

	private AssetLinkageDataModel cachedBackgroundAssetLinkage;

	private AssetLinkageDataModel cachedLogoAssetLinkage;

	private AssetLinkageDataModel cachedLeaderboardsBannerAssetLinkage;

	private AssetLinkageDataModel cachedGachaAssetBundle1;

	private AssetLinkageDataModel cachedGachaAssetBundle2;

	private UnitLevelProgressionDataModel cachedLevelProgressionDataModel;

	private UnitLevelProgressionDataModel cachedleftUnitLevelProgressionDataModel;

	private UnitLevelProgressionDataModel cachedRightUnitLevelProgressionDataModel;

	private EventAssetsDataModel cachedEventAssetsDataModel;

	private List<EventUnitsDataModel> cachedEventUnits;

	private AssetLinkageDataModel[] cachedEventAssetLinkages;

	public int cooldownTime;

	public string dateEnd;

	public string dateStart;

	public int eventAssetsId;

	public string eventType;

	public int forceBotMatching;

	public int rewardsId;

	public int soloRewardId;

	public EventTypes EventType
	{
		get
		{
			switch (eventType)
			{
			case "points_battle":
				return EventTypes.POINTS_EVENT;
			case "wins":
				return EventTypes.WINS_EVENT;
			case "raid_boss":
				return EventTypes.RAIDBOSS_EVENT;
			case "pvp_tournament":
				return EventTypes.PVP_TOURNAMENT_EVENT;
			default:
				return EventTypes.POINTS_EVENT;
			}
		}
	}

	public AssetLinkageDataModel[] GetGachaInfoAssetIds
	{
		get
		{
			if (cachedEventAssetLinkages == null)
			{
				cachedEventAssetLinkages = new AssetLinkageDataModel[4];
				cachedEventAssetLinkages[0] = AssetLinkageDataModel.GetSingle(Assets.gachaInfoAssetBundle1);
				cachedEventAssetLinkages[1] = AssetLinkageDataModel.GetSingle(Assets.gachaInfoAssetBundle2);
				cachedEventAssetLinkages[2] = AssetLinkageDataModel.GetSingle(Assets.gachaInfoAssetBundle3);
				cachedEventAssetLinkages[3] = AssetLinkageDataModel.GetSingle(Assets.gachaInfoAssetBundle4);
			}
			return cachedEventAssetLinkages;
		}
	}

	public string PopupAlreadyMemberTitle
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.keyTitleAlreadyMemberEventPopup.Localize("ALREADY_MEMBER_TITLE");
		}
	}

	public string PopupAlreadyMemberDescription
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.keyDescriptionAlreadyMemberEventPopup.Localize("ALREADY_MEMBER_DESCRIPTION");
		}
	}

	public string PopupJoinTitle
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.keyTitleJoinClubEventPopup.Localize("JOIN_TITLE");
		}
	}

	public string PopupJoinDescription
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.keyDescriptionJoinClubEventPopup.Localize("JOIN_DESCRIPTION");
		}
	}

	public string PopupEventInfoH1
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.eventInfoKeyStringEventH1.Localize("HEADER 1");
		}
	}

	public string PopupEventInfoH2
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.eventInfoKeyStringH2.Localize("HEADER 2");
		}
	}

	public string PopupEventInfoUnitSetTitle
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.eventInfoKeyStringUnitsSet.Localize("AWESOME UNITS");
		}
	}

	public string PopupEventBodyMessage1
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.eventInfoKeyStringBody1.Localize("Line 1");
		}
	}

	public string PopupEventBodyMessage2
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.eventInfoKeyStringBody2.Localize("Line 2");
		}
	}

	public string PopupEventBodyMessage3
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.eventInfoKeyStringBody3.Localize("Line 3");
		}
	}

	public string PopupEventBodyMessage4
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.eventInfoKeyStringBody4.Localize("Line 4");
		}
	}

	public string PopupEventBodyMessage5
	{
		get
		{
			if (Assets == null)
			{
				return "NO_TEXT";
			}
			return Assets.eventInfoKeyStringBody5.Localize("Line 5");
		}
	}

	public bool IsInPast
	{
		get
		{
			return NonUnitySingleton<TimeManager>.instance.IsTimestampInPast(DateEndTimeStamp);
		}
	}

	public bool IsInFuture
	{
		get
		{
			return NonUnitySingleton<TimeManager>.instance.IsTimestampInFuture(DateStartTimeStamp);
		}
	}

	public long CooldownTimeInMilliseconds
	{
		get
		{
			return cooldownTime * 1000;
		}
	}

	public bool IsActive
	{
		get
		{
			return NonUnitySingleton<TimeManager>.instance.IsTimestampInPast(DateStartTimeStamp) && NonUnitySingleton<TimeManager>.instance.IsTimestampInFuture(DateEndTimeStamp);
		}
	}

	public bool IsOnCooldown
	{
		get
		{
			return NonUnitySingleton<TimeManager>.instance.IsTimestampInPast(DateStartTimeStamp) && NonUnitySingleton<TimeManager>.instance.IsTimestampInFuture(DateEndWithCooldownTimeStamp);
		}
	}

	public long DateStartTimeStamp
	{
		get
		{
			if (cachedStartTimeStamp == -1)
			{
				cachedStartTimeStamp = TimeManager.TimeToUnixTimeStamp(DateTime.Parse(dateStart, CultureInfo.InvariantCulture).ToUniversalTime());
			}
			return cachedStartTimeStamp;
		}
	}

	public long DateEndTimeStamp
	{
		get
		{
			if (cachedEndTimeStamp == -1)
			{
				cachedEndTimeStamp = TimeManager.TimeToUnixTimeStamp(DateTime.Parse(dateEnd, CultureInfo.InvariantCulture).ToUniversalTime());
			}
			return cachedEndTimeStamp;
		}
	}

	public long DateEndWithCooldownTimeStamp
	{
		get
		{
			return DateEndTimeStamp + CooldownTimeInMilliseconds;
		}
	}

	public EventAssetsDataModel Assets
	{
		get
		{
			if (cachedEventAssetsDataModel == null)
			{
				cachedEventAssetsDataModel = EventAssetsDataModel.GetSingle(eventAssetsId);
			}
			return cachedEventAssetsDataModel;
		}
	}

	public AssetLinkageDataModel BackgroundAssetLinkage
	{
		get
		{
			if (Assets == null)
			{
				return null;
			}
			if (cachedBackgroundAssetLinkage == null)
			{
				cachedBackgroundAssetLinkage = AssetLinkageDataModel.GetSingle(Assets.backgroundAssetId);
			}
			return cachedBackgroundAssetLinkage;
		}
	}

	public AssetLinkageDataModel LogoAssetLinkage
	{
		get
		{
			if (Assets == null)
			{
				return null;
			}
			if (cachedLogoAssetLinkage == null)
			{
				cachedLogoAssetLinkage = AssetLinkageDataModel.GetSingle(Assets.logoAssetId);
			}
			return cachedLogoAssetLinkage;
		}
	}

	public AssetLinkageDataModel LeaderboardsBannerAssetLinkage
	{
		get
		{
			if (Assets == null)
			{
				return null;
			}
			if (cachedLeaderboardsBannerAssetLinkage == null)
			{
				cachedLeaderboardsBannerAssetLinkage = AssetLinkageDataModel.GetSingle(Assets.leaderboardsAssetId);
			}
			return cachedLeaderboardsBannerAssetLinkage;
		}
	}

	public UnitLevelProgressionDataModel EventUnitLevel
	{
		get
		{
			if (Assets == null)
			{
				return null;
			}
			if (cachedLevelProgressionDataModel == null)
			{
				cachedLevelProgressionDataModel = UnitLevelProgressionDataModel.GetSingle(Assets.homeScreenUnitId);
			}
			return cachedLevelProgressionDataModel;
		}
	}

	public UnitLevelProgressionDataModel EventLeftUnitLevel
	{
		get
		{
			if (Assets == null)
			{
				return null;
			}
			if (cachedleftUnitLevelProgressionDataModel == null)
			{
				cachedleftUnitLevelProgressionDataModel = UnitLevelProgressionDataModel.GetSingle(Assets.leftUnitId);
			}
			return cachedleftUnitLevelProgressionDataModel;
		}
	}

	public UnitLevelProgressionDataModel EventRightUnitLevel
	{
		get
		{
			if (Assets == null)
			{
				return null;
			}
			if (cachedRightUnitLevelProgressionDataModel == null)
			{
				cachedRightUnitLevelProgressionDataModel = UnitLevelProgressionDataModel.GetSingle(Assets.rightUnitId);
			}
			return cachedRightUnitLevelProgressionDataModel;
		}
	}

	public int EventUnitAssetBundleId
	{
		get
		{
			if (EventUnitLevel != null)
			{
				return EventUnitLevel.assetBundleId;
			}
			return 0;
		}
	}

	public int EventLeftUnitAssetBundleId
	{
		get
		{
			if (EventLeftUnitLevel != null)
			{
				return EventLeftUnitLevel.assetBundleId;
			}
			return 0;
		}
	}

	public int EventRightUnitAssetBundleId
	{
		get
		{
			if (EventRightUnitLevel != null)
			{
				return EventRightUnitLevel.assetBundleId;
			}
			return 0;
		}
	}

	public AssetLinkageDataModel GachaAssetBundle1
	{
		get
		{
			if (Assets == null)
			{
				return null;
			}
			if (cachedGachaAssetBundle1 == null)
			{
				cachedGachaAssetBundle1 = AssetLinkageDataModel.GetSingle(Assets.gachaAssetBundle1);
			}
			return cachedGachaAssetBundle1;
		}
	}

	public AssetLinkageDataModel GachaAssetBundle2
	{
		get
		{
			if (Assets == null)
			{
				return null;
			}
			if (cachedGachaAssetBundle2 == null)
			{
				cachedGachaAssetBundle2 = AssetLinkageDataModel.GetSingle(Assets.gachaAssetBundle2);
			}
			return cachedGachaAssetBundle2;
		}
	}

	public static EventDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventDataModel>(id.ToString());
	}

	public static EventDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventDataModel>(id);
	}

	public static List<EventDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EventDataModel>();
	}

	public List<LeaderboardRewardsDataModel> GetRewards()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<LeaderboardRewardsDataModel>("WHERE rewards_id = " + rewardsId);
	}

	public List<LeaderboardRewardsDataModel> GetSoloRewards()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<LeaderboardRewardsDataModel>("WHERE rewards_id = " + soloRewardId);
	}

	public static EventDataModel GetActiveEvent()
	{
		return GetAll().Find((EventDataModel x) => x.IsActive);
	}

	public static EventDataModel GetOnCooldownEvent()
	{
		return GetAll().Find((EventDataModel x) => x.IsOnCooldown);
	}

	public static EventDataModel GetNextEvent()
	{
		EventDataModel result = null;
		foreach (EventDataModel item in GetAll())
		{
			if (item.IsInFuture)
			{
				result = item;
				break;
			}
		}
		return result;
	}

	public List<EventUnitsDataModel> GetEventUnits()
	{
		if (cachedEventUnits == null)
		{
			cachedEventUnits = EventUnitsDataModel.GetAll();
			cachedEventUnits = cachedEventUnits.FindAll((EventUnitsDataModel x) => x.eventId.ToString() == id);
		}
		return cachedEventUnits;
	}

	public bool UnitBelongsToEvent(string unitID)
	{
		return GetEventUnits().Find((EventUnitsDataModel x) => x.unitId.ToString() == unitID) != null;
	}
}
