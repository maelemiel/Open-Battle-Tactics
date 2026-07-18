using System;
using UnityEngine;

public class UserResearcher
{
	public enum ResearchType
	{
		Idle = 0,
		BuildTank = 1,
		Experiment = 2
	}

	public long startTime;

	public long finishTime;

	public ResearchType researchType;

	public string itemID;

	public int Index
	{
		get
		{
			return UserProfile.player.researchers.IndexOf(this);
		}
	}

	public float Progress
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

	public bool IsIdle
	{
		get
		{
			return researchType == ResearchType.Idle;
		}
	}

	public bool CanClaim
	{
		get
		{
			return !IsIdle && TimeManager.ServerTime >= finishTime;
		}
	}

	public IResearchableDataModel ResearchItem
	{
		get
		{
			if (researchType == ResearchType.BuildTank)
			{
				return UnitDataModel.GetSingle(itemID);
			}
			return null;
		}
	}

	public string ResearchName
	{
		get
		{
			if (researchType == ResearchType.BuildTank)
			{
				return UnitDataModel.GetSingle(itemID).name;
			}
			return null;
		}
	}

	public void StartBuildingTank(UnitDataModel tankData)
	{
		StartResearch(ResearchType.BuildTank, tankData.id);
	}

	public void StartResearch(ResearchType researchType, string itemID)
	{
		this.researchType = researchType;
		this.itemID = itemID;
		startTime = TimeManager.ServerTime;
		finishTime = TimeManager.ServerTime + ResearchItem.ResearchDuration;
	}

	public void SkipResearch()
	{
		finishTime = TimeManager.ServerTime;
	}

	public void ClaimResearch()
	{
		switch (researchType)
		{
		case ResearchType.BuildTank:
		{
			UnitDataModel unitDataModel = (UnitDataModel)ResearchItem;
			UserProfile.player.inventory.AddItem((UserInventory.ItemType)unitDataModel.rewardTypeId, null, unitDataModel.rewardAmount);
			break;
		}
		}
		researchType = ResearchType.Idle;
		itemID = null;
	}

	public int GetRemainingTime(long currentServerTime = -1)
	{
		if (currentServerTime == -1)
		{
			currentServerTime = TimeManager.ServerTime;
		}
		return (int)Math.Ceiling((float)(finishTime - currentServerTime) / 1000f);
	}

	public UserPriceDataModel GetHurryCost(long currentServerTime = -1)
	{
		if (currentServerTime == -1)
		{
			currentServerTime = TimeManager.ServerTime;
		}
		UserPriceDataModel userPriceDataModel = new UserPriceDataModel(Constants.ResearchSkipItem, Constants.ResearchSkipCost);
		int hurryTimeUnits = GetHurryTimeUnits(currentServerTime);
		return userPriceDataModel.Multiply(hurryTimeUnits);
	}

	public int GetHurryTimeUnits(long currentServerTime = -1)
	{
		if (currentServerTime == -1)
		{
			currentServerTime = TimeManager.ServerTime;
		}
		int remainingTime = GetRemainingTime(currentServerTime);
		return (int)Math.Ceiling((float)remainingTime / (float)Constants.ResearchSkipTimeUnit);
	}
}
