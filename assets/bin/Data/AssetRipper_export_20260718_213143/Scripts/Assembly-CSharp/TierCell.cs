using System.Collections.Generic;
using UnityEngine;

public class TierCell : ScrollableCell
{
	[SerializeField]
	private ScrollableCell[] unitCells;

	[SerializeField]
	private PrefabProxy badgeproxy;

	[SerializeField]
	private tk2dSprite rewardCrate;

	[SerializeField]
	private tk2dTextMesh tierName;

	private BlueprintsTierView blueprintsTierView;

	private ProgressionDivisionDataModel divisionDataModel;

	private List<UnitDataModel> tierDataModels;

	private int divisionIndex = -1;

	public ScrollableCell[] UnitCells
	{
		get
		{
			return unitCells;
		}
	}

	private void Awake()
	{
		blueprintsTierView = GetComponent<BlueprintsTierView>();
	}

	public override void Init(ScrollableAreaController controller, object data, int index, float cellHeight = 0f, float cellWidth = 0f, ScrollableCell parentCell = null)
	{
		base.Init(controller, data, index, cellHeight, cellWidth);
		divisionDataModel = (ProgressionDivisionDataModel)data;
		divisionIndex = int.Parse(divisionDataModel.id);
		tierDataModels = UnitDataModel.GetUnitsUnlockedAtTier(divisionIndex);
	}

	private void OnEnable()
	{
		UserProfile.player.OnResearchClaimed -= ConfigureCellData;
		UserProfile.player.OnResearchClaimed += ConfigureCellData;
	}

	public override void ConfigureCell()
	{
		base.ConfigureCell();
	}

	public void OnDisable()
	{
		if (UserProfile.player != null)
		{
			UserProfile.player.OnResearchClaimed -= ConfigureCellData;
		}
	}

	public override void ConfigureCellData()
	{
		base.ConfigureCellData();
		if ((bool)blueprintsTierView)
		{
			blueprintsTierView.ConfigureView(divisionDataModel);
		}
		if ((bool)tierName)
		{
			tierName.text = divisionDataModel.name;
		}
		if ((bool)badgeproxy && divisionDataModel.BadgeLinkage != null)
		{
			StartCoroutine(badgeproxy.ChangeAssetCoroutine(divisionDataModel.BadgeLinkage));
		}
		for (int i = 0; i < unitCells.Length; i++)
		{
			if (i > tierDataModels.Count - 1)
			{
				unitCells[i].gameObject.SetActive(false);
				continue;
			}
			unitCells[i].Init(controller, tierDataModels[i], i, 0f, 0f, this);
			unitCells[i].ConfigureCell();
		}
	}
}
