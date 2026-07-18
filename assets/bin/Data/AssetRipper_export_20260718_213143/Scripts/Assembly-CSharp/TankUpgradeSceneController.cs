using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUpgradeSceneController : SceneController, iSortableController
{
	[SerializeField]
	private ScrollableAreaController scrollController;

	[SerializeField]
	protected EditTeamSortOptions sortOptions;

	[SerializeField]
	private GameObject noSpecialTanksText;

	private IList inventory;

	private int storedInventoryLength = -1;

	public ScrollableAreaController ScrollableAreaControllerInstance()
	{
		return scrollController;
	}

	public IList UnassignedInventory()
	{
		return inventory;
	}

	private void Start()
	{
		SceneTransitionManager.readyToTransitionIn = true;
		allowsBackButton = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		InitSortOptions();
		UpdateUnitInventory();
		StartCoroutine(AnnouncerController.DialogTrigger("PartsListFirstAccess"));
	}

	private void UpdateUnitInventory()
	{
		inventory = new List<UserUnit>();
		foreach (UserUnit value in UserProfile.player.unitInventory.Values)
		{
			if (value.UpgradeCostsParts())
			{
				inventory.Add(value);
			}
		}
		noSpecialTanksText.SetActive(inventory.Count <= 0);
		UpdateScrollableArea();
	}

	protected void InitSortOptions()
	{
		EditTeamSortOptions editTeamSortOptions = sortOptions;
		editTeamSortOptions.OnSortComplete = (Action)Delegate.Combine(editTeamSortOptions.OnSortComplete, new Action(UpdateScrollableArea));
		sortOptions.AddSortOption(UnitSortTypes.Close);
		sortOptions.AddSortOption(UnitSortTypes.RaritySort);
		sortOptions.AddSortOption(UnitSortTypes.LevelSort);
		sortOptions.AddSortOption(UnitSortTypes.HealthSort);
		sortOptions.AddSortOption(UnitSortTypes.DamageSort);
		sortOptions.AddSortOption(UnitSortTypes.FirstStrikeSort);
		sortOptions.AddSortOption(UnitSortTypes.SpecialSort);
		sortOptions.AddSortOption(UnitSortTypes.PartialLevel);
	}

	private void UpdateScrollableArea()
	{
		List<TankUpgradeCell.TankUpgradeCellData> list = new List<TankUpgradeCell.TankUpgradeCellData>();
		TankUpgradeCell.TankUpgradeCellData tankUpgradeCellData = null;
		int num = 0;
		EditTeamSortOptions.SortByType(UnitSortTypes.HighValueSort, inventory);
		EditTeamSortOptions.SortByType(UnitSortTypes.LevelSort, inventory);
		EditTeamSortOptions.SortByType(UnitSortTypes.RaritySort, inventory);
		foreach (UserUnit item in inventory)
		{
			if (num++ % 2 == 0)
			{
				tankUpgradeCellData = new TankUpgradeCell.TankUpgradeCellData();
				tankUpgradeCellData.unit1 = item;
				tankUpgradeCellData.tankUpdated = TankUpdated;
			}
			else
			{
				tankUpgradeCellData.unit2 = item;
				list.Add(tankUpgradeCellData);
				tankUpgradeCellData = null;
			}
		}
		if (tankUpgradeCellData != null)
		{
			list.Add(tankUpgradeCellData);
		}
		scrollController.DataSource = list;
		if (storedInventoryLength != inventory.Count)
		{
			scrollController.ScrollableArea.TweenValue(0f, 0.5f);
		}
		storedInventoryLength = inventory.Count;
	}

	private void TankUpdated()
	{
		UpdateUnitInventory();
	}

	public void ChangeTab()
	{
	}

	public void SortUnits()
	{
	}
}
