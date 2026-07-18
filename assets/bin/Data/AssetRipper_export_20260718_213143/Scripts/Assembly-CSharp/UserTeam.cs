using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserTeam
{
	public int index;

	public List<UserUnit> units;

	public long cooldownFinishTime;

	public int Count
	{
		get
		{
			return units.Count;
		}
	}

	public bool IsOnCooldown
	{
		get
		{
			return cooldownFinishTime > TimeManager.ServerTime;
		}
	}

	public long CooldownLength
	{
		get
		{
			int num = 0;
			foreach (UserUnit unit in units)
			{
				if (unit != null)
				{
					num += unit.CooldownCost;
				}
			}
			return num;
		}
	}

	public float CooldownProgress
	{
		get
		{
			return Mathf.Clamp01(1f - (float)(cooldownFinishTime - TimeManager.ServerTime) / ((float)CooldownLength * 1000f));
		}
	}

	public UserPriceDataModel GetPriceToSkipRepair(long timestamp = -1)
	{
		if (timestamp == -1)
		{
			timestamp = TimeManager.ServerTime;
		}
		UserPriceDataModel userPriceDataModel = new UserPriceDataModel(Constants.RepairSkipItem, Constants.RepairSkipCost);
		int cooldownTimeUnits = GetCooldownTimeUnits(timestamp);
		return userPriceDataModel.Multiply(cooldownTimeUnits);
	}

	public int GetCooldownTimeUnits(long timestamp = -1)
	{
		if (timestamp == -1)
		{
			timestamp = TimeManager.ServerTime;
		}
		int num = (int)Math.Ceiling((float)(cooldownFinishTime - timestamp) / 1000f);
		return (int)Math.Ceiling((float)num / (float)Constants.RepairSkipTimeUnit);
	}

	public bool Contains(string unitID)
	{
		foreach (UserUnit unit in units)
		{
			if (unit != null && unit.ID == unitID)
			{
				return true;
			}
		}
		return false;
	}

	public string[] GetUnitIDs()
	{
		return units.Select((UserUnit x) => (x == null) ? null : x.ID).ToArray();
	}

	public int GetUnitCount()
	{
		int num = 0;
		foreach (UserUnit unit in units)
		{
			if (unit != null)
			{
				num++;
			}
		}
		return num;
	}

	public UserUnit GetUnit(int slot)
	{
		if (slot < units.Count)
		{
			return units[slot];
		}
		return null;
	}

	public void SetUnit(int slot, UserUnit unit)
	{
		while (units.Count <= slot)
		{
			units.Add(null);
		}
		units[slot] = unit;
	}

	public bool TryInsertUnit(UserUnit unit)
	{
		if (GetUnitCount() < Constants.MinUnitsPerTeam)
		{
			for (int i = 0; i < Constants.MinUnitsPerTeam; i++)
			{
				if (GetUnit(i) == null)
				{
					SetUnit(i, unit);
					Singleton<SessionManager>.instance.UpdateTeam();
					return true;
				}
			}
		}
		return false;
	}

	public bool RemoveUnit(UserUnit unit)
	{
		return units.Remove(unit);
	}
}
