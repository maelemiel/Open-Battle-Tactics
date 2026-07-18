using System;
using System.Collections.Generic;
using UnityEngine;

public class UserGachaPrize
{
	private GachaPoolsDataModel _cachedDataModel;

	public string id;

	public int gachaPrizeID;

	public long startTime;

	public long achievedDivisionAt;

	public int gachaType;

	public long finishTime
	{
		get
		{
			if (PrizeGachaDataModel.gachaType == 3)
			{
				return startTime + PrizeGachaDataModel.availableCountdown * 1000;
			}
			return startTime + PrizeGachaDataModel.freeCooldown * 1000;
		}
	}

	public float CountDownProgress
	{
		get
		{
			return CooldownProgress;
		}
	}

	public float CooldownProgress
	{
		get
		{
			return Mathf.Clamp01((float)(TimeManager.ServerTime - startTime) / (float)(finishTime - startTime));
		}
	}

	public int RemainingSeconds
	{
		get
		{
			return Mathf.Max(0, (int)(finishTime - TimeManager.ServerTime) / 1000);
		}
	}

	public bool CanPlayGachaPrize
	{
		get
		{
			if (PrizeGachaDataModel == null)
			{
				return false;
			}
			if (!PrizeGachaDataModel.IsActive)
			{
				return false;
			}
			if (gachaType == 3)
			{
				if (CountDownProgress >= 1f)
				{
					return false;
				}
			}
			else if (IsOnCooldown)
			{
				return false;
			}
			if (!UserProfile.player.CanAfford(PrizeGachaDataModel.GetPrice()))
			{
				return false;
			}
			return true;
		}
	}

	public bool IsUnlockedByTier
	{
		get
		{
			GachaPoolsDataModel single = GachaPoolsDataModel.GetSingle(gachaPrizeID);
			return single.gachaType == 3;
		}
	}

	public bool IsOnCooldown
	{
		get
		{
			GachaPoolsDataModel single = GachaPoolsDataModel.GetSingle(gachaPrizeID);
			if (single != null && single.freeCooldown > 0 && finishTime > TimeManager.ServerTime)
			{
				return true;
			}
			return false;
		}
	}

	public GachaPoolsDataModel PrizeGachaDataModel
	{
		get
		{
			if (_cachedDataModel == null)
			{
				_cachedDataModel = GachaPoolsDataModel.GetSingle(gachaPrizeID);
			}
			return _cachedDataModel;
		}
	}

	public bool IsActive
	{
		get
		{
			return false;
		}
	}

	public UserGachaPrize(int prizeGachaID, long lastTimeClaimed)
	{
		gachaPrizeID = prizeGachaID;
		startTime = lastTimeClaimed;
	}

	public UserGachaPrize(string id, int prizeGachaID, long lastTimeClaimed, long achievedDivisionAt, int gachaType)
	{
		this.id = id;
		gachaPrizeID = prizeGachaID;
		startTime = lastTimeClaimed;
		this.achievedDivisionAt = achievedDivisionAt;
		this.gachaType = gachaType;
		if (gachaType == 3)
		{
			startTime = this.achievedDivisionAt;
		}
	}

	public static bool PlayGachaPrize(int gachaPrizeID)
	{
		GachaPoolsDataModel single = GachaPoolsDataModel.GetSingle(gachaPrizeID);
		if (single.GetPrice() != null && !UserProfile.player.CanAfford(single.GetPrice()))
		{
			Log.Error("Player has not enough money to play this Gacha");
			return false;
		}
		if (single.freeCooldown > 0)
		{
			UserGachaPrize item = new UserGachaPrize(gachaPrizeID, TimeManager.ServerTime);
			UserProfile.player.userGachaPrizes.Add(item);
		}
		if (single.GetPrice() != null)
		{
			UserProfile.player.PayPrice(single.GetPrice());
		}
		TopBarController.instance.UpdateNotifications();
		return true;
	}

	private bool ShowInListUnlockByTierGacha()
	{
		if (PrizeGachaDataModel.unlockDivision > UserProfile.player.divisionInt)
		{
			return false;
		}
		UserGachaPrize gachaPrizeData = UserProfile.player.GetGachaPrizeData(int.Parse(PrizeGachaDataModel.ID));
		if (gachaPrizeData != null && gachaPrizeData.PrizeGachaDataModel != null && (double)gachaPrizeData.CountDownProgress < 1.0)
		{
			return true;
		}
		return false;
	}

	private bool ShowInListEventGacha()
	{
		EventDataModel activeOnCooldownEvent = UserProfile.player.GetActiveOnCooldownEvent();
		if (activeOnCooldownEvent != null && int.Parse(activeOnCooldownEvent.id) == PrizeGachaDataModel.eventId)
		{
			return true;
		}
		return false;
	}

	private bool ShowInListStepGacha(Dictionary<int, UserStepGacha> userStepGachas)
	{
		if (userStepGachas.Count == 0 && PrizeGachaDataModel.stepUpNum == 1)
		{
			return true;
		}
		if (userStepGachas.Count > 0 && !userStepGachas.ContainsKey(PrizeGachaDataModel.stepUpId) && PrizeGachaDataModel.stepUpNum == 1)
		{
			return true;
		}
		if (userStepGachas.Count > 0)
		{
			foreach (KeyValuePair<int, UserStepGacha> userStepGacha in userStepGachas)
			{
				if (userStepGacha.Value.stepUpId == PrizeGachaDataModel.stepUpId && userStepGacha.Value.stepUpNum == PrizeGachaDataModel.stepUpNum)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool ShowInList(Dictionary<int, UserStepGacha> userStepGachas)
	{
		if (!PrizeGachaDataModel.IsActive)
		{
			return false;
		}
		bool flag = true;
		switch ((GachaTypes)PrizeGachaDataModel.gachaType)
		{
		case GachaTypes.UNLOCKBYTIER:
			return ShowInListUnlockByTierGacha();
		case GachaTypes.EVENT:
			return ShowInListEventGacha();
		case GachaTypes.STEPGACHA:
			return ShowInListStepGacha(userStepGachas);
		default:
			return true;
		}
	}

	public void SkipContract()
	{
	}

	public ItemCollectionDataModel SkipFreeGacha()
	{
		return new ItemCollectionDataModel();
	}

	public int GetRemainingTime(long currentServerTime = -1)
	{
		if (currentServerTime == -1)
		{
			currentServerTime = TimeManager.ServerTime;
		}
		return (int)Math.Ceiling((float)(finishTime - currentServerTime));
	}
}
