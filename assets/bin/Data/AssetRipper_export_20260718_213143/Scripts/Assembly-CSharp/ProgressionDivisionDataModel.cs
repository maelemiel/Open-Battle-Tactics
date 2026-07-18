using System;
using System.Collections.Generic;

public class ProgressionDivisionDataModel : BaseDataModel, IDivisionMetadata
{
	public int badgeLinkageId;

	public int baseLoseCoinReward;

	public int baseWinCoinReward;

	public int eventPointPvpBonus;

	public int giftPackageId;

	public int hasPromotionSeries;

	public int isHidden;

	public string keyName;

	public int losePoint;

	public int promotionSeriesId;

	public int rewardAmount;

	public int rewardTypeId;

	public int totalPointToPromotionSeries;

	public int totalPointToReward;

	public int unitDestroyCoinReward;

	public int unitSurviveCoinReward;

	public int winEventPoint;

	public int winPoint;

	public string name
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(keyName);
		}
	}

	public ItemCollectionDataModel CompletionClaimReward
	{
		get
		{
			return ItemGiftDataModel.GetGiftPackage(giftPackageId);
		}
	}

	public int ResetPoints
	{
		get
		{
			float num = (float)Constants.PromoSeriesSetback / 100f;
			return (int)Math.Ceiling((float)totalPointToPromotionSeries * num);
		}
	}

	public bool IsHidden
	{
		get
		{
			return isHidden == 1;
		}
	}

	public AssetLinkageDataModel BadgeLinkage
	{
		get
		{
			return AssetLinkageDataModel.GetSingle(badgeLinkageId);
		}
	}

	public bool UnitsAllBuilt
	{
		get
		{
			UserProfile player = UserProfile.player;
			if (UserProfile.player == null)
			{
				return false;
			}
			List<UnitDataModel> unitsUnlockedAtTier = UnitDataModel.GetUnitsUnlockedAtTier(int.Parse(id));
			for (int i = 0; i < unitsUnlockedAtTier.Count; i++)
			{
				if (!player.HasBuiltUnit(unitsUnlockedAtTier[i].id))
				{
					return false;
				}
			}
			return true;
		}
	}

	public string ID
	{
		get
		{
			return id;
		}
	}

	public int BaseWinCoinReward
	{
		get
		{
			return baseWinCoinReward;
		}
	}

	public int BaseLoseCoinReward
	{
		get
		{
			return baseLoseCoinReward;
		}
	}

	public int UnitDestroyCoinReward
	{
		get
		{
			return unitDestroyCoinReward;
		}
	}

	public int UnitSurviveCoinReward
	{
		get
		{
			return unitSurviveCoinReward;
		}
	}

	public int WinPoint
	{
		get
		{
			return winPoint;
		}
	}

	public int WinEventPoint
	{
		get
		{
			return winEventPoint;
		}
	}

	public int EventPointPVPBonus
	{
		get
		{
			return eventPointPvpBonus;
		}
	}

	public int LosePoint
	{
		get
		{
			return losePoint;
		}
	}

	public static ProgressionDivisionDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(id.ToString());
	}

	public static ProgressionDivisionDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(id);
	}

	public static List<ProgressionDivisionDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<ProgressionDivisionDataModel>();
	}
}
