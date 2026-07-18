using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatalogueSceneController : SceneController
{
	private const string DISABLED_TAB_SPRITE_NAME = "Secondary_button_grey";

	[SerializeField]
	private ScrollableAreaController unitTypesScrollableAreaController;

	[SerializeField]
	private ScrollableAreaController catalogueScrollableAreaController;

	[SerializeField]
	private tk2dUIToggleButtonGroup toggleButtonGroup;

	private Transform tutorialHighlightedObject;

	private tk2dUIItem scrollableAreaUIItem;

	private List<UnitTypeDataModel> unitTypesList;

	private List<UnitDataModel> allUnits = new List<UnitDataModel>();

	private void Start()
	{
		base.SectionTitle = "Catalogue";
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
		}
	}

	private void Init()
	{
		AudioTrigger.Map_Music.PlayMusic();
		allowsBackButton = true;
		allUnits = UnitDataModel.GetAll();
		InitializeScrollableArea();
	}

	public void InitializeScrollableArea()
	{
		unitTypesList = UnitTypeDataModel.GetAll().ToList();
		if ((bool)unitTypesScrollableAreaController)
		{
			unitTypesScrollableAreaController.DataSource = unitTypesList;
		}
		List<tk2dUIToggleButton> cellComponents = unitTypesScrollableAreaController.GetCellComponents<tk2dUIToggleButton>();
		if ((bool)toggleButtonGroup)
		{
			toggleButtonGroup.AddNewToggleButtons(cellComponents.FindAll((tk2dUIToggleButton x) => x != null).ToArray());
			toggleButtonGroup.SelectedIndex = 0;
		}
	}

	public void Confirm()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
	}

	private void OnToggleGroupButtonChange(tk2dUIToggleButtonGroup toggleButton)
	{
		if (unitTypesList != null && unitTypesList.Count != 0)
		{
			int selectedIndex = toggleButton.SelectedIndex;
			CatalogueTypeCell component = toggleButton.ToggleBtns[selectedIndex].gameObject.GetComponent<CatalogueTypeCell>();
			if (!component)
			{
				Log.Error("No CatalogueTypeCell component found", base.gameObject);
			}
			int currentType = int.Parse(component.UnitType.id);
			List<UnitDataModel> dataSource = allUnits.FindAll((UnitDataModel x) => x.UnitType == (UnitType)currentType);
			if (unitTypesList != null)
			{
				catalogueScrollableAreaController.DataSource = dataSource;
				catalogueScrollableAreaController.ContentToTop();
			}
		}
	}
}
