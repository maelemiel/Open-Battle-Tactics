using System;
using System.Collections.Generic;
using UnityEngine;

public class UserContract
{
	public int contractID;

	public long startTime;

	public long finishTime;

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

	public bool CanClaim
	{
		get
		{
			return TimeManager.ServerTime >= finishTime;
		}
	}

	public ContractsDataModel ContractDataModel
	{
		get
		{
			List<ContractsDataModel> all = ContractsDataModel.GetAll();
			return all.Find((ContractsDataModel x) => x.contractId == contractID);
		}
	}

	public bool StartContract(int contractID)
	{
		List<ContractsDataModel> all = ContractsDataModel.GetAll();
		ContractsDataModel contractsDataModel = all.Find((ContractsDataModel x) => x.contractId == contractID);
		if (contractsDataModel == null)
		{
			Debug.LogWarning("NO CONTRACT!!");
			return false;
		}
		this.contractID = contractID;
		startTime = TimeManager.ServerTime;
		finishTime = TimeManager.ServerTime + contractsDataModel.contractDuration * 1000;
		TopBarController.instance.UpdateNotifications();
		return true;
	}

	public void SkipContract()
	{
		finishTime = TimeManager.ServerTime;
	}

	public ItemCollectionDataModel ClaimContract()
	{
		List<ContractDetailsDataModel> all = ContractDetailsDataModel.GetAll();
		all = all.FindAll((ContractDetailsDataModel x) => x.contractId == contractID);
		ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
		MersenneTwister randomProvider = new MersenneTwister((uint)UserProfile.player.random_seed_contracts);
		foreach (ContractDetailsDataModel item in all)
		{
			int nextContractsRandom = GetNextContractsRandom(randomProvider, 0, 101);
			if (item.dropRate >= nextContractsRandom)
			{
				UserInventory.ItemType itemId = (UserInventory.ItemType)item.itemId;
				int num = GetNextContractsRandom(randomProvider, item.dropMin, item.dropMax);
				if (itemId == UserInventory.ItemType.Coins)
				{
					num += 5 - num % 5;
				}
				itemCollectionDataModel.AddItem(itemId, item.partType, num);
				UserProfile.player.inventory.AddItem(itemId, item.partType.ToString(), num);
			}
		}
		contractID = -1;
		startTime = TimeManager.ServerTime;
		finishTime = TimeManager.ServerTime;
		UserProfile.player.nextContracts = FetchNewContracts(randomProvider);
		UserProfile.player.random_seed_contracts = GetNextContractsRandom(randomProvider, 0, 2000001);
		TopBarController.instance.UpdateNotifications();
		return itemCollectionDataModel;
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

	public int GetNextContractsRandom(IRandomProvider randomProvider, int from, int to)
	{
		if (from == to)
		{
			return from;
		}
		return from + (int)randomProvider.Next((uint)(to - from));
	}

	public List<int> FetchNewContracts(IRandomProvider randomProvider)
	{
		List<int> list = new List<int>();
		List<ContractsDataModel> all = ContractsDataModel.GetAll();
		for (int i = 0; i < Constants.ContractsPoolCount; i++)
		{
			list.Add(GetNextContract(all, randomProvider, i));
		}
		return list;
	}

	public int GetNextContract(List<ContractsDataModel> contracts, IRandomProvider randomProvider, int poolID)
	{
		int num = 0;
		List<ContractsDataModel> list = contracts.FindAll((ContractsDataModel x) => x.contractPool == poolID);
		int totalWeight = GetTotalWeight(list);
		int nextContractsRandom = GetNextContractsRandom(randomProvider, 0, totalWeight);
		foreach (ContractsDataModel item in list)
		{
			num += item.contractWeight;
			if (num > nextContractsRandom)
			{
				return item.contractId;
			}
		}
		return -1;
	}

	public int GetTotalWeight(List<ContractsDataModel> contracts)
	{
		int num = 0;
		for (int i = 0; i < contracts.Count; i++)
		{
			num += contracts[i].contractWeight;
		}
		return num;
	}
}
