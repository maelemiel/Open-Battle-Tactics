using System;
using UnityEngine;

public class TankUpgradeCell : ScrollableCell
{
	public class TankUpgradeCellData
	{
		public UserUnit unit1;

		public UserUnit unit2;

		public Action tankUpdated;
	}

	[SerializeField]
	private TankUpgradeUnit unitUpgrade1;

	[SerializeField]
	private TankUpgradeUnit unitUpgrade2;

	private TankUpgradeCellData cellData;

	public override void Init(ScrollableAreaController controller, object data, int index, float cellHeight = 0f, float cellWidth = 0f, ScrollableCell parentCell = null)
	{
		base.Init(controller, data, index, cellHeight, cellWidth);
		cellData = (TankUpgradeCellData)data;
	}

	public override void ConfigureCell()
	{
		base.ConfigureCell();
	}

	public override void ConfigureCellData()
	{
		base.ConfigureCellData();
		if (cellData != null)
		{
			if (cellData.unit1 != null)
			{
				unitUpgrade1.gameObject.SetActive(true);
				unitUpgrade1.Init(cellData.unit1);
			}
			else
			{
				unitUpgrade1.gameObject.SetActive(false);
			}
			if (cellData.unit2 != null)
			{
				unitUpgrade2.gameObject.SetActive(true);
				unitUpgrade2.Init(cellData.unit2);
			}
			else
			{
				unitUpgrade2.gameObject.SetActive(false);
			}
		}
	}

	public void ShowUpgradeUnitTop()
	{
		PopupManager.ShowPopup(PopupDataModel.UpgradeUnitPopUp(cellData.unit1, delegate
		{
			unitUpgrade1.Init(cellData.unit1);
			if (cellData.tankUpdated != null)
			{
				cellData.tankUpdated();
			}
		}));
	}

	public void ShowUpgradeUnitBottom()
	{
		PopupManager.ShowPopup(PopupDataModel.UpgradeUnitPopUp(cellData.unit2, delegate
		{
			unitUpgrade2.Init(cellData.unit2);
			if (cellData.tankUpdated != null)
			{
				cellData.tankUpdated();
			}
		}));
	}

	private void OnTouch()
	{
	}
}
