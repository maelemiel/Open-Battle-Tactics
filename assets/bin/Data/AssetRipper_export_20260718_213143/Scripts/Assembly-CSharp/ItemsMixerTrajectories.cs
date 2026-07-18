using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemsMixerTrajectories : ScriptableObject
{
	[Serializable]
	public class ItemsMixerTrajectoryPlayerSlot
	{
		[SerializeField]
		private List<ItemsMixerTrajectoryItemSlot> trajectoriesPerItemSlot;

		public List<ItemsMixerTrajectoryItemSlot> TrajectoriesPerItemSlot
		{
			get
			{
				return trajectoriesPerItemSlot;
			}
			set
			{
				trajectoriesPerItemSlot = value;
			}
		}
	}

	[Serializable]
	public class ItemsMixerTrajectoryItemSlot
	{
		[SerializeField]
		private List<ItemMixerTrajectoryPoints> trajectories;

		public List<ItemMixerTrajectoryPoints> Trajectories
		{
			get
			{
				return trajectories;
			}
			set
			{
				trajectories = value;
			}
		}

		public ItemsMixerTrajectoryItemSlot()
		{
			trajectories = new List<ItemMixerTrajectoryPoints>();
		}

		public ItemsMixerTrajectoryItemSlot(List<Vector3> trajectory)
		{
			trajectories.Add(new ItemMixerTrajectoryPoints(trajectory));
		}
	}

	[Serializable]
	public class ItemMixerTrajectoryPoints
	{
		[SerializeField]
		private List<Vector3> trajectoryPoints;

		public List<Vector3> TrajectoryPoints
		{
			get
			{
				return trajectoryPoints;
			}
			set
			{
				trajectoryPoints = value;
			}
		}

		public ItemMixerTrajectoryPoints(List<Vector3> points)
		{
			trajectoryPoints = points;
		}
	}

	public int playerSlotCount = 9;

	public int itemSlotCount = 9;

	[SerializeField]
	private List<ItemsMixerTrajectoryPlayerSlot> slotStoredTrajectories;

	public void CreateTrajectoriesList()
	{
		slotStoredTrajectories = new List<ItemsMixerTrajectoryPlayerSlot>(playerSlotCount);
		for (int i = 0; i < playerSlotCount; i++)
		{
			slotStoredTrajectories.Add(new ItemsMixerTrajectoryPlayerSlot());
		}
		foreach (ItemsMixerTrajectoryPlayerSlot slotStoredTrajectory in slotStoredTrajectories)
		{
			slotStoredTrajectory.TrajectoriesPerItemSlot = new List<ItemsMixerTrajectoryItemSlot>(itemSlotCount);
			for (int j = 0; j < itemSlotCount; j++)
			{
				slotStoredTrajectory.TrajectoriesPerItemSlot.Add(new ItemsMixerTrajectoryItemSlot());
			}
		}
	}

	public List<Vector3> GetTrajectory(int playerSlot, int itemSlot)
	{
		if (playerSlot > slotStoredTrajectories.Count)
		{
			Log.Error("[ItemsMixerTrajectories] Index out of range");
		}
		if (itemSlot > slotStoredTrajectories[playerSlot].TrajectoriesPerItemSlot.Count)
		{
			Log.Error("[ItemsMixerTrajectories] Index out of range");
		}
		int count = slotStoredTrajectories[playerSlot].TrajectoriesPerItemSlot[itemSlot].Trajectories.Count;
		if (count == 0)
		{
			Log.Error("Path not available. From: " + playerSlot + " to: " + itemSlot);
			return new List<Vector3>();
		}
		int index = UnityEngine.Random.Range(0, count);
		return slotStoredTrajectories[playerSlot].TrajectoriesPerItemSlot[itemSlot].Trajectories[index].TrajectoryPoints;
	}

	public void AddTrajectory(int playerSlot, int itemSlot, List<Vector3> trajectoryPositions)
	{
		if (playerSlot > slotStoredTrajectories.Count || playerSlot < 0)
		{
			Log.Error("[ItemsMixerTrajectories] Index out of range");
		}
		if (itemSlot > slotStoredTrajectories[playerSlot].TrajectoriesPerItemSlot.Count || itemSlot < 0)
		{
			Log.Error("[ItemsMixerTrajectories] Index out of range");
		}
		slotStoredTrajectories[playerSlot].TrajectoriesPerItemSlot[itemSlot].Trajectories.Add(new ItemMixerTrajectoryPoints(trajectoryPositions));
	}

	public int GetTrajectoryCount(int playerSlot, int itemSlot)
	{
		if (playerSlot > slotStoredTrajectories.Count || playerSlot < 0)
		{
			Log.Error("[ItemsMixerTrajectories] Index out of range");
		}
		if (itemSlot > slotStoredTrajectories[playerSlot].TrajectoriesPerItemSlot.Count || itemSlot < 0)
		{
			Log.Error("[ItemsMixerTrajectories] Index out of range");
		}
		return slotStoredTrajectories[playerSlot].TrajectoriesPerItemSlot[itemSlot].Trajectories.Count;
	}
}
