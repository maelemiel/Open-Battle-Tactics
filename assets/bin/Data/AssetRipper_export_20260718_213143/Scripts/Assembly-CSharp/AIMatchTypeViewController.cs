using System.Collections.Generic;
using UnityEngine;

public class AIMatchTypeViewController : BaseMatchTypeView
{
	private const int MAX_TEAMS = 3;

	[SerializeField]
	private PrefabProxy badgePrefabProxy;

	[SerializeField]
	private tk2dTextMesh teamNameLabel;

	[SerializeField]
	private tk2dTextMesh description;

	[SerializeField]
	private UnitInfoView[] unitInfoViews;

	[SerializeField]
	private GameObject[] childViews;

	private int currentTeamIndex;

	private IList<AiArmyDataModel> nextAIArmies;

	private List<UserUnit>[] aiTeamsUserUnits;

	private ProgressionDivisionDataModel[] divisonDataModels;

	[SerializeField]
	private PriceLabelController priceLableController;

	[SerializeField]
	private UnitPositionSet[] unitPositionSets;

	public int SelectedTeamIndex
	{
		get
		{
			return currentTeamIndex;
		}
	}

	public override void SetEnabled(bool state)
	{
		base.SetEnabled(state);
		GameObject[] array = childViews;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(state);
		}
	}

	protected override void SetupMatchTypeView()
	{
		base.SetupMatchTypeView();
		nextAIArmies = (List<AiArmyDataModel>)dataObject;
		if (nextAIArmies != null)
		{
			aiTeamsUserUnits = new List<UserUnit>[nextAIArmies.Count];
			divisonDataModels = new ProgressionDivisionDataModel[nextAIArmies.Count];
			for (int i = 0; i < aiTeamsUserUnits.Length; i++)
			{
				aiTeamsUserUnits[i] = nextAIArmies[i].GetUnitList();
				divisonDataModels[i] = nextAIArmies[i].GetRandomDivision();
			}
			RefreshView();
		}
	}

	private void RefreshView()
	{
		for (int i = 0; i < unitInfoViews.Length; i++)
		{
			unitInfoViews[i].gameObject.SetActive(false);
		}
		if (nextAIArmies.Count != 0)
		{
			AiArmyDataModel aiArmyDataModel = nextAIArmies[currentTeamIndex];
			if ((bool)teamNameLabel)
			{
				teamNameLabel.text = aiArmyDataModel.Name;
			}
			int count = aiTeamsUserUnits[currentTeamIndex].Count;
			for (int j = 0; j < aiTeamsUserUnits[currentTeamIndex].Count; j++)
			{
				SetupUnitView(count, j);
			}
			if ((bool)priceLableController)
			{
				priceLableController.ConfigurePriceLabel(GetAvailableParts(aiTeamsUserUnits[currentTeamIndex]));
			}
			ProgressionDivisionDataModel progressionDivisionDataModel = divisonDataModels[currentTeamIndex];
			if ((bool)badgePrefabProxy && progressionDivisionDataModel != null)
			{
				StartCoroutine(badgePrefabProxy.ChangeAssetCoroutine(progressionDivisionDataModel.BadgeLinkage));
			}
		}
	}

	private void SetupUnitView(int totalUnitViewsCount, int currentUnitViewIndex)
	{
		if (currentUnitViewIndex >= unitInfoViews.Length)
		{
			Log.Warning("Trying to setup an out-of-bounds Unity View", base.gameObject);
		}
		UserUnit userUnit = null;
		userUnit = aiTeamsUserUnits[currentTeamIndex][currentUnitViewIndex];
		unitInfoViews[currentUnitViewIndex].gameObject.SetActive(true);
		unitInfoViews[currentUnitViewIndex].ConfigureUnitView(userUnit.UnitDataModel, userUnit.level, userUnit.partialLevel);
		if (totalUnitViewsCount < Constants.MinUnitsPerTeam)
		{
			unitInfoViews[currentUnitViewIndex].RepositionUI();
		}
		else
		{
			unitInfoViews[currentUnitViewIndex].RestoreUIPosition();
		}
		if (unitPositionSets.Length > 0 && (bool)unitPositionSets[totalUnitViewsCount - 1] && unitPositionSets[totalUnitViewsCount - 1].transformList.Count > currentUnitViewIndex)
		{
			unitInfoViews[currentUnitViewIndex].transform.position = unitPositionSets[totalUnitViewsCount - 1].transformList[currentUnitViewIndex].position;
		}
	}

	private void NextTeam()
	{
		currentTeamIndex++;
		currentTeamIndex %= 3;
		RefreshView();
	}

	private void PreviousTeam()
	{
		currentTeamIndex--;
		currentTeamIndex = (currentTeamIndex + 3) % 3;
		RefreshView();
	}

	private ItemCollectionDataModel GetAvailableParts(List<UserUnit> units)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
		foreach (UserUnit unit in units)
		{
			for (int i = 0; i < unit.PartDrops.Length; i++)
			{
				if (!dictionary.ContainsKey(unit.PartDrops[i].ID))
				{
					dictionary.Add(unit.PartDrops[i].ID, 1);
					itemCollectionDataModel.items.Add(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, int.Parse(unit.PartDrops[i].ID), 0));
				}
			}
		}
		return itemCollectionDataModel;
	}
}
