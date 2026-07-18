using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditTeamUnitsSceneController : EditTeamBaseSceneController
{
	[SerializeField]
	private tk2dUIToggleButtonGroup teamButtonGroup;

	[SerializeField]
	private CooldownsController cooldownsController;

	[SerializeField]
	private Transform teamCellsContainer;

	[SerializeField]
	private GameObject teamCellsPrefab;

	[SerializeField]
	private GameObject sortButton;

	[SerializeField]
	private GameObject clearButton;

	private UserUnit unitBeingViewed;

	private List<EditTeamCellGroup> teamCellGroups = new List<EditTeamCellGroup>();

	private int currentTeamIndex = -1;

	private static WaitForSeconds oneSecond = new WaitForSeconds(1f);

	private EditTeamUnitsSceneModel SceneModel
	{
		get
		{
			return sceneModel as EditTeamUnitsSceneModel;
		}
		set
		{
			sceneModel = value;
		}
	}

	protected override void Start()
	{
		base.SectionTitle = "ui_editteam_title".Localize("Edit Teams");
		if (SceneModel == null)
		{
			SceneModel = new EditTeamUnitsSceneModel
			{
				selectedTeamIndex = 1
			};
			Log.Warning("using default sceneModel");
		}
		base.Start();
		StartCoroutine(UpdateTeamsSequence());
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
		}
		if ((bool)cooldownsController)
		{
			cooldownsController.OnControllerCooldownFinished += OnCooldownFinished;
		}
	}

	public override void OnBeginTransitionOut()
	{
		base.OnBeginTransitionOut();
		SaveToProfile();
	}

	private void OnDestroy()
	{
		teamCellsPrefab = null;
	}

	private void OnCooldownFinished(UserTeam team)
	{
		StartCoroutine(UpdateTeamsSequence());
	}

	private IEnumerator UpdateTeamsSequence()
	{
		foreach (EditTeamCellGroup cellGroup in teamCellGroups)
		{
			foreach (EditTeamUnitCell cell in cellGroup.cellList)
			{
				cell.UpdateTeamState();
			}
		}
		yield return oneSecond;
	}

	protected override void Update()
	{
		DragTeamSwitch();
		base.Update();
		UpdateSortButton();
		UpdateClearButton();
	}

	public override void OnConfirm()
	{
		int num = 0;
		for (int i = 0; i < teamCellGroups[currentTeamIndex].cellList.Count; i++)
		{
			EditTeamBaseCell editTeamBaseCell = teamCellGroups[currentTeamIndex].cellList[i];
			if (editTeamBaseCell.DataObject != null)
			{
				num++;
			}
		}
		if (num < Constants.MinUnitsPerTeam)
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_popup_notenoughunits_title".Localize("Not enough units!"), string.Format("ui_popup_notenoughunits_desc".Localize("You must have {0} units to battle!"), Constants.MinUnitsPerTeam)));
		}
		else
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaScene);
		}
	}

	protected override void DragTargetLocked(DragDropItem targetItem)
	{
		EditTeamUnitCell component = targetItem.GetComponent<EditTeamUnitCell>();
		if (component != null && component.isTeamCell && component.teamCellGroup.index == currentTeamIndex)
		{
			CurrentTeamOnCooldown();
		}
	}

	protected override void InitInventory()
	{
		unassignedInventory = UserProfile.player.unitInventory.Values.Where((UserUnit x) => x.Team == null).ToList();
	}

	protected override void InitSortOptions()
	{
		sortOptions.AddSortOption(UnitSortTypes.Close);
		sortOptions.AddSortOption(UnitSortTypes.RaritySort);
		sortOptions.AddSortOption(UnitSortTypes.LevelSort);
		sortOptions.AddSortOption(UnitSortTypes.HealthSort);
		sortOptions.AddSortOption(UnitSortTypes.DamageSort);
		sortOptions.AddSortOption(UnitSortTypes.FirstStrikeSort);
		sortOptions.AddSortOption(UnitSortTypes.SpecialSort);
		sortOptions.AddSortOption(UnitSortTypes.ByNameSort);
		sortOptions.SetCurrentSort(UnitSortTypes.LevelSort);
		sortOptions.SetCurrentSort(UnitSortTypes.RaritySort);
	}

	protected override void Init()
	{
		cooldownsController.Init();
		teamButtonGroup.SelectedIndex = SceneModel.selectedTeamIndex;
		base.Init();
	}

	protected override void OnCellTapped(object data)
	{
		EditTeamUnitCell editTeamUnitCell = data as EditTeamUnitCell;
		if (editTeamUnitCell.Unit != null)
		{
			AudioTrigger.UIButtonSoft.Play();
			object.Equals(unitBeingViewed, null);
			unitBeingViewed = editTeamUnitCell.Unit;
			PopupManager.ShowPopup(PopupDataModel.UpgradeUnitPopUp(editTeamUnitCell.Unit, OnUpgradeUnitPopUpClosed));
		}
	}

	public void OnUpgradeUnitPopUpClosed()
	{
		EditTeamUnitCell editTeamUnitCell = null;
		List<EditTeamBaseCell> cellList = teamCellGroups[currentTeamIndex].cellList;
		for (int i = 0; i < cellList.Count; i++)
		{
			EditTeamUnitCell editTeamUnitCell2 = (EditTeamUnitCell)cellList[i];
			if (editTeamUnitCell2.Unit != null && editTeamUnitCell2.Unit.id == unitBeingViewed.id)
			{
				editTeamUnitCell = editTeamUnitCell2;
				break;
			}
		}
		if (!UserProfile.player.unitInventory.ContainsValue(unitBeingViewed))
		{
			if (editTeamUnitCell != null && editTeamUnitCell.isTeamCell)
			{
				editTeamUnitCell.ResetWithData(null);
			}
			else
			{
				unassignedInventory.Remove(unitBeingViewed);
			}
		}
		else if (editTeamUnitCell != null)
		{
			editTeamUnitCell.ResetWithData(editTeamUnitCell.Unit);
		}
		unitBeingViewed = null;
		scrollableAreaController.OnDataChanged();
	}

	public void RestoreSavedState(UserUnit currentUnit)
	{
		object.Equals(unitBeingViewed, null);
		unitBeingViewed = currentUnit;
	}

	private void UpdateSortButton()
	{
		if (!sortButton)
		{
			return;
		}
		if (scrollableAreaController.DataSource.Count <= 1)
		{
			if (sortButton.activeSelf)
			{
				sortButton.SetActive(false);
			}
		}
		else if (!sortButton.activeSelf)
		{
			sortButton.SetActive(true);
		}
	}

	private void UpdateClearButton()
	{
		if (!clearButton || currentTeamIndex <= -1)
		{
			return;
		}
		bool flag = true;
		List<EditTeamBaseCell> cellList = teamCellGroups[currentTeamIndex].cellList;
		for (int i = 0; i < cellList.Count; i++)
		{
			if (cellList[i].DataObject != null)
			{
				flag = false;
			}
		}
		if (flag)
		{
			if (clearButton.activeSelf)
			{
				clearButton.SetActive(false);
			}
		}
		else if (!clearButton.activeSelf)
		{
			clearButton.SetActive(true);
		}
	}

	private void DragTeamSwitch()
	{
		if (!(currentDragItem == null) && !(currentDragItem.DropTarget == null))
		{
			TeamCooldownViewController component = currentDragItem.DropTarget.GetComponent<TeamCooldownViewController>();
			if (component != null && Time.time - dropTargetChangeTime > 0.5f)
			{
				teamButtonGroup.SelectedIndex = component.CurrentTeam.index;
			}
		}
	}

	private bool CurrentTeamOnCooldown()
	{
		if (UserProfile.player.teams[currentTeamIndex].IsOnCooldown)
		{
			PopupManager.ShowPopup(PopupDataModel.SkipCooldownPopup(UserProfile.player.teams[currentTeamIndex], RefreshTeam));
			return true;
		}
		return false;
	}

	private void RefreshTeam()
	{
	}

	public void OnToggleTeamButtonsChanged(tk2dUIToggleButtonGroup buttonGroup)
	{
		ShowTeam(buttonGroup.SelectedIndex);
		UserProfile.player.currentTeamIndex = buttonGroup.SelectedIndex;
	}

	public void OnAutoAssign()
	{
		if (!CurrentTeamOnCooldown())
		{
			ClearTeam(0);
			List<UserUnit> list = ((List<UserUnit>)unassignedInventory).ToList();
			EditTeamSortOptions.SortByType(UnitSortTypes.HighValueSort, list);
			EditTeamSortOptions.SortByType(UnitSortTypes.LevelSort, list);
			EditTeamSortOptions.SortByType(UnitSortTypes.RaritySort, list);
			for (int i = 0; i < teamCellGroups[currentTeamIndex].cellList.Count && i < list.Count; i++)
			{
				UserUnit userUnit = list[i];
				unassignedInventory.Remove(userUnit);
				teamCellGroups[currentTeamIndex].cellList[i].ResetWithData(userUnit);
			}
			scrollableAreaController.OnDataChanged();
		}
	}

	public void ClearTeam(int insertIndex)
	{
		if (CurrentTeamOnCooldown())
		{
			return;
		}
		insertIndex = Mathf.Clamp(insertIndex, 0, unassignedInventory.Count);
		int num = 0;
		for (int i = 0; i < teamCellGroups[currentTeamIndex].cellList.Count; i++)
		{
			EditTeamBaseCell editTeamBaseCell = teamCellGroups[currentTeamIndex].cellList[i];
			if (editTeamBaseCell.DataObject != null)
			{
				unassignedInventory.Insert(insertIndex + num, editTeamBaseCell.DataObject);
				editTeamBaseCell.ResetWithData(null);
				num++;
			}
		}
		scrollableAreaController.OnDataChanged();
	}

	public void OnClearTeam()
	{
		ClearTeam(scrollableAreaController.InitialIndex);
	}

	private void ShowTeam(int index, float duration = 0.5f)
	{
		if (index != currentTeamIndex)
		{
			if (currentTeamIndex == -1)
			{
				duration = 0f;
			}
			currentTeamIndex = index;
			teamCellsContainer.transform.TweenLocalXPosition(tk2dCamera.ScreenWidth * 1.3f * (float)(-index), duration);
		}
	}

	protected override void InitTeamCells()
	{
		List<UserUnit> list = new List<UserUnit>();
		Reporting.SetManageSquadPreTeams(UserProfile.player.teams, UserProfile.player.currentTeamIndex);
		foreach (UserTeam team in UserProfile.player.teams)
		{
			GameObject gameObject = Object.Instantiate(teamCellsPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			EditTeamCellGroup component = gameObject.GetComponent<EditTeamCellGroup>();
			gameObject.transform.SetParent(teamCellsContainer);
			gameObject.transform.localPosition = new Vector3(tk2dCamera.ScreenWidth * 1.3f, 0f, 0f) * team.index;
			component.index = team.index;
			teamCellGroups.Add(component);
			for (int i = 0; i < component.cellList.Count; i++)
			{
				EditTeamBaseCell editTeamBaseCell = component.cellList[i];
				InitDragDropItem(editTeamBaseCell.DragItem);
				UserUnit userUnit = ((i >= team.units.Count) ? null : team.units[i]);
				if (list.Contains(userUnit))
				{
					Log.Warning("Unit exists in multiple teams!! Removing.");
					userUnit = null;
				}
				editTeamBaseCell.teamCellGroup = component;
				editTeamBaseCell.index = i;
				editTeamBaseCell.DataObject = userUnit;
				list.Add(userUnit);
			}
		}
	}

	protected override void SaveToProfile()
	{
		foreach (UserTeam team in UserProfile.player.teams)
		{
			EditTeamCellGroup editTeamCellGroup = teamCellGroups[team.index];
			team.units = new List<UserUnit>(4);
			for (int i = 0; i < editTeamCellGroup.cellList.Count; i++)
			{
				team.units.Add(editTeamCellGroup.cellList[i].DataObject as UserUnit);
			}
		}
		Singleton<SessionManager>.instance.UpdateTeam();
		Reporting.ManageSquadEvent(UserProfile.player.teams, UserProfile.player.currentTeamIndex);
	}

	public void TriggerSaveToProfile()
	{
		SaveToProfile();
	}

	protected override void PlaySound()
	{
		AudioTrigger.SwapTank.Play();
	}
}
