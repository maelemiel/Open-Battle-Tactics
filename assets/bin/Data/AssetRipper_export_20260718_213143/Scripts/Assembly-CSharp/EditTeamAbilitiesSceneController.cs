using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditTeamAbilitiesSceneController : EditTeamBaseSceneController
{
	[SerializeField]
	private Transform teamAbilitiesCellsContainer;

	[SerializeField]
	private GameObject teamAbilitiesCellsPrefab;

	[SerializeField]
	private tk2dUIToggleButtonGroup tabController;

	private List<EditTeamCellGroup> teamCellGroups = new List<EditTeamCellGroup>();

	private int currentTeamIndex = -1;

	private EditTeamAbilitiesSceneModel SceneModel
	{
		get
		{
			return sceneModel as EditTeamAbilitiesSceneModel;
		}
		set
		{
			sceneModel = value;
		}
	}

	protected override void Start()
	{
		if (SceneModel == null)
		{
			SceneModel = new EditTeamAbilitiesSceneModel();
			Log.Warning("Using default sceneModel");
		}
		base.Start();
		tabController.SelectedIndex = UserProfile.player.currentTeamIndex;
		ShowTeam(UserProfile.player.currentTeamIndex);
		if (UserProfile.player != null)
		{
			UserProfile.player.newAbilities.Clear();
		}
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.UpdateNotifications();
			TopBarController.instance.ShowProgressBanner = false;
		}
	}

	public override void OnBeginTransitionOut()
	{
		base.OnBeginTransitionOut();
		SaveToProfile();
	}

	private void OnDestroy()
	{
	}

	protected override void InitInventory()
	{
		UserProfile player = UserProfile.player;
		unassignedInventory = (from x in AbilityDataModel.GetAll()
			where !player.CurrentAbilitySet.abilities.Contains(x.id)
			where x.isActive == 1
			select x).ToList();
	}

	public void OnToggleAbilityButtonsChanged(tk2dUIToggleButtonGroup buttonGroup)
	{
		if (!(currentDragItem != null))
		{
			ShowTeam(buttonGroup.SelectedIndex);
			UserProfile.player.currentTeamIndex = buttonGroup.SelectedIndex;
			SaveLocalAbilities();
			InitInventory();
			scrollableAreaController.DataSource = unassignedInventory;
			sortOptions.SetCurrentSort(AbilitySortTypes.UnlockTierSortAscending);
			sortOptions.SetCurrentSort(AbilitySortTypes.Locked);
		}
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
			teamAbilitiesCellsContainer.transform.TweenLocalXPosition(tk2dCamera.ScreenWidth * 1.3f * (float)(-index), duration);
		}
	}

	protected override void InitSortOptions()
	{
		sortOptions.AddSortOption(AbilitySortTypes.Close);
		sortOptions.AddSortOption(AbilitySortTypes.EnergySort);
		sortOptions.AddSortOption(AbilitySortTypes.UnlockTierSort);
		sortOptions.AddSortOption(AbilitySortTypes.UnlockTierSortAscending);
		sortOptions.AddSortOption(AbilitySortTypes.Locked);
		sortOptions.SetCurrentSort(AbilitySortTypes.UnlockTierSortAscending);
		sortOptions.SetCurrentSort(AbilitySortTypes.Locked);
	}

	protected override void InitTeamCells()
	{
		Reporting.SetInitialAbilities(UserProfile.player.CurrentAbilitySet.abilities);
		foreach (UserAbilitySet userAbilitySet in UserProfile.player.userAbilitySets)
		{
			GameObject gameObject = Object.Instantiate(teamAbilitiesCellsPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			EditTeamCellGroup component = gameObject.GetComponent<EditTeamCellGroup>();
			gameObject.transform.SetParent(teamAbilitiesCellsContainer);
			gameObject.transform.localPosition = new Vector3(tk2dCamera.ScreenWidth * 1.3f, 0f, 0f) * userAbilitySet.index;
			component.index = userAbilitySet.index;
			teamCellGroups.Add(component);
			List<AbilityDataModel> list = userAbilitySet.abilities.Select((string x) => AbilityDataModel.GetSingle(x)).ToList();
			for (int num = 0; num < component.cellList.Count; num++)
			{
				EditTeamBaseCell editTeamBaseCell = component.cellList[num];
				InitDragDropItem(editTeamBaseCell.DragItem);
				AbilityDataModel dataObject = ((num >= list.Count) ? null : list[num]);
				editTeamBaseCell.teamCellGroup = component;
				editTeamBaseCell.index = num;
				editTeamBaseCell.DataObject = dataObject;
			}
		}
	}

	protected override void Init()
	{
		tabController.SelectedIndex = SceneModel.selectedTeamIndex;
		base.Init();
	}

	protected override void OnCellTapped(object data)
	{
		AudioTrigger.UIButtonSoft.Play();
		EditTeamBaseCell editTeamBaseCell = (EditTeamBaseCell)data;
		PopupManager.ShowPopup(PopupDataModel.InspectAbilityPopUp((AbilityDataModel)editTeamBaseCell.DataObject, null));
	}

	protected void SaveLocalAbilities()
	{
		foreach (UserAbilitySet userAbilitySet in UserProfile.player.userAbilitySets)
		{
			if (teamCellGroups.Count <= 0)
			{
				continue;
			}
			EditTeamCellGroup editTeamCellGroup = teamCellGroups[userAbilitySet.index];
			userAbilitySet.abilities = new List<string>(4);
			for (int i = 0; i < editTeamCellGroup.cellList.Count; i++)
			{
				AbilityDataModel abilityDataModel = editTeamCellGroup.cellList[i].DataObject as AbilityDataModel;
				if (abilityDataModel != null)
				{
					userAbilitySet.abilities.Add(abilityDataModel.ID);
				}
				else
				{
					userAbilitySet.abilities.Add(null);
				}
			}
		}
	}

	protected override void SaveToProfile()
	{
		SaveLocalAbilities();
		Reporting.ManageAbilitiesEvent(UserProfile.player.CurrentAbilitySet.abilities);
		Singleton<SessionManager>.instance.UpdateAbilities();
	}

	public override void OnConfirm()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaScene);
	}

	protected override void PlaySound()
	{
		AudioTrigger.SwapAbilities.Play();
	}
}
