using System.Collections.Generic;
using UnityEngine;

public class ItemsMixerTrajectoryRecorder : MonoBehaviour
{
	public int limitTrajectoriesPerSlot = 5;

	public ItemsMixerTrajectories itemsMixerTrajectoriesSO;

	[ContextMenu("Initialize ScriptableObject")]
	private void Initialize()
	{
		if ((bool)itemsMixerTrajectoriesSO)
		{
			itemsMixerTrajectoriesSO.CreateTrajectoriesList();
		}
	}

	public void AddNewTrajectory(int playerSlot, int itemSlot, List<Vector3> trajectoryPath)
	{
		string text = playerSlot + "-" + itemSlot;
		if ((bool)itemsMixerTrajectoriesSO)
		{
			int num = 0;
			num = itemsMixerTrajectoriesSO.GetTrajectoryCount(playerSlot, itemSlot);
			Debug.Log("Count for trajectory: [" + text + "] = " + num);
			if (num < limitTrajectoriesPerSlot)
			{
				Debug.Log("Adding trajectory: " + text);
				itemsMixerTrajectoriesSO.AddTrajectory(playerSlot, itemSlot, trajectoryPath);
			}
			else
			{
				Log.Warning("Skipping key - Limit reached: " + text);
			}
		}
	}

	[ContextMenu("Show Stored Trajectories Count")]
	private void PrintTrajectoriesInfo()
	{
		Log.Debug("PrintTrajectoriesInfo");
		if (!itemsMixerTrajectoriesSO)
		{
			Log.Error("Items Mixer Trajectories Scriptable Object reference not found", base.gameObject);
			return;
		}
		int num = 0;
		for (int i = 0; i < itemsMixerTrajectoriesSO.playerSlotCount; i++)
		{
			for (int j = 0; j < itemsMixerTrajectoriesSO.itemSlotCount; j++)
			{
				num = itemsMixerTrajectoriesSO.GetTrajectoryCount(i, j);
				if (num < limitTrajectoriesPerSlot)
				{
					Log.Warning(i + "-" + j + ": " + num);
				}
			}
		}
	}
}
