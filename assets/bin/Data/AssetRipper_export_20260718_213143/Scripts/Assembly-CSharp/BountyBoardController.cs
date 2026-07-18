using System;
using System.Collections.Generic;
using UnityEngine;

public class BountyBoardController : MonoBehaviour
{
	public static class RaidBossSortTypes
	{
		public static SortType Dummy = new SortTypeInt
		{
			label = "ui_editteam_sort".Localize("Sort")
		};

		public static SortType TimeLeft = new SortTypeInt
		{
			label = "ui_raidboss_sort_time".Localize("TIME"),
			sortHandler = (object x) => (int)((BountyBoardDataEntry)x).timeToLive
		};

		public static SortType Damage = new SortTypeInt
		{
			label = "ui_editteam_sort_damage".Localize("DAMAGE"),
			sortHandler = (object x) => ((BountyBoardDataEntry)x).damage
		};

		public static List<SortType> sortTypes = new List<SortType> { TimeLeft, Damage };
	}

	[SerializeField]
	private GameObject loadingIcon;

	[SerializeField]
	private ScrollableAreaController scrollableArea;

	[SerializeField]
	private tk2dTextMesh bountiesAvailable;

	[SerializeField]
	private tk2dTextMesh noBountiesLabel;

	[SerializeField]
	private GameObject sortButton;

	[SerializeField]
	private tk2dTextMesh upSortButtonLabel;

	[SerializeField]
	private tk2dTextMesh downSortButtonLabel;

	private Action<BattleSceneModel> callback;

	private List<BountyBoardDataEntry> bountyBoardEntries;

	private int currentSort;

	public static bool IsUpdating { get; set; }

	private void Awake()
	{
		if ((bool)sortButton)
		{
			sortButton.SetActive(false);
		}
		scrollableArea.OnDataChange += UpdateAvailableBountyBoards;
	}

	public void Init(Action<BattleSceneModel> battleBossCallback = null)
	{
		callback = battleBossCallback;
		loadingIcon.SetActive(true);
		scrollableArea.gameObject.SetActive(false);
		IsUpdating = false;
		Singleton<SessionManager>.instance.GetRaidBossEvents(PopulateBountyBoard);
	}

	private void PopulateBountyBoard(List<BountyBoardDataEntry> bountyBoardEntries)
	{
		BountyBoardDataEntry bountyBoardDataEntry = new BountyBoardDataEntry();
		bountyBoardDataEntry.fightCell = true;
		bountyBoardDataEntry.battleBoss = BattleBoss;
		List<string> list = new List<string>();
		scrollableArea.gameObject.SetActive(true);
		if (bountyBoardEntries.Count > 0)
		{
			for (int i = 0; i < bountyBoardEntries.Count; i++)
			{
				bountyBoardEntries[i].battleBoss = BattleBoss;
				list.Add(bountyBoardEntries[i].raidBossId + "," + bountyBoardEntries[i].State);
			}
		}
		bountyBoardEntries.Add(bountyBoardDataEntry);
		scrollableArea.DataSource = bountyBoardEntries;
		Reporting.RaidBossAccessBoard(list);
		loadingIcon.SetActive(false);
		this.bountyBoardEntries = bountyBoardEntries;
		ApplySort(RaidBossSortTypes.Dummy);
		if ((bool)sortButton)
		{
			sortButton.SetActive(true);
		}
	}

	private void UpdateAvailableBountyBoards()
	{
		if ((bool)bountiesAvailable)
		{
			bountiesAvailable.text = (bountyBoardEntries.Count - 1).ToString();
		}
		if ((bool)noBountiesLabel)
		{
			noBountiesLabel.gameObject.SetActive(bountyBoardEntries.Count == 0);
		}
	}

	private void BattleBoss(BattleSceneModel battleScene)
	{
		if (callback != null)
		{
			callback(battleScene);
		}
	}

	private void OnSortPressed(tk2dUIItem button)
	{
		if (!IsUpdating)
		{
			currentSort = (currentSort + 1) % RaidBossSortTypes.sortTypes.Count;
			ApplySort(RaidBossSortTypes.sortTypes[currentSort]);
		}
	}

	private void ApplySort(SortType sortType)
	{
		if ((bool)upSortButtonLabel)
		{
			upSortButtonLabel.text = sortType.label;
		}
		if ((bool)downSortButtonLabel)
		{
			downSortButtonLabel.text = sortType.label;
		}
		if (sortType.HasHandler)
		{
			EditTeamSortOptions.SortByType(sortType, bountyBoardEntries);
		}
		scrollableArea.OnDataChanged();
	}
}
