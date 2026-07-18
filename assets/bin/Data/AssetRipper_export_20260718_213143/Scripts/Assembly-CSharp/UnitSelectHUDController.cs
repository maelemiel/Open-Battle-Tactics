using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectHUDController : MonoBehaviour
{
	private const int NUMBER_OF_ROWS = 2;

	[SerializeField]
	private Transform listContentParent;

	public GameObject unitItemPrefab;

	private UnitSelectScreenController controller;

	[SerializeField]
	private tk2dUIScrollableArea scrollableArea;

	private float xOffset;

	private float yOffset;

	private float uiItemWidth = 220f;

	private float uiItemHeight = 200f;

	[SerializeField]
	private UnitItemBottomBarController bottomBarController;

	[SerializeField]
	private UnitItemPopUpController popUpController;

	[SerializeField]
	private UnitItemPromoteViewController promoteViewController;

	private PromotionLocalData previousPromotionData;

	[SerializeField]
	private tk2dUIToggleButtonGroup toggleGroup;

	public UnitSelectScreenController Controller
	{
		get
		{
			return controller;
		}
		set
		{
			controller = value;
		}
	}

	public void Awake()
	{
		if (!listContentParent)
		{
			Log.Warning("List content transform parent not found", base.gameObject);
		}
		if (!scrollableArea)
		{
			Log.Warning("Scrollable area reference not found", base.gameObject);
		}
		if (!popUpController)
		{
			Log.Warning("PopUp controller reference not found", base.gameObject);
		}
		xOffset = uiItemWidth * 0.5f;
		yOffset = uiItemHeight * 0.5f;
		popUpController.Init(this);
	}

	public void Init(UnitSelectScreenController controller)
	{
		this.controller = controller;
		if ((bool)toggleGroup)
		{
			toggleGroup.SelectedIndex = UserProfile.player.currentTeamIndex;
		}
	}

	public Dictionary<string, UnitItemController> PopulateUnitList(List<UserUnit> unitsList)
	{
		Dictionary<string, UnitItemController> dictionary = new Dictionary<string, UnitItemController>();
		GameObject gameObject = null;
		int uiItemsCount = 0;
		for (int i = 0; i < unitsList.Count; i++)
		{
			gameObject = CreateUnitItem(unitsList[i]);
			PositionUIItem(gameObject, ref uiItemsCount);
			UnitItemController component = gameObject.GetComponent<UnitItemController>();
			if (unitsList[i] != null && !string.IsNullOrEmpty(unitsList[i].ID))
			{
				dictionary.Add(unitsList[i].ID, component);
			}
		}
		UpdateContentLength(uiItemsCount);
		BoxCollider component2 = scrollableArea.gameObject.GetComponent<BoxCollider>();
		if ((bool)component2)
		{
			scrollableArea.VisibleAreaLength = component2.size.x;
		}
		return dictionary;
	}

	private void UpdateContentLength(int unitCounter)
	{
		scrollableArea.ContentLength = uiItemWidth * Mathf.Ceil((float)unitCounter / 2f);
	}

	private GameObject CreateUnitItem(UserUnit unit)
	{
		GameObject gameObject = Object.Instantiate(unitItemPrefab) as GameObject;
		gameObject.transform.parent = listContentParent;
		UnitItemController component = gameObject.GetComponent<UnitItemController>();
		if ((bool)component)
		{
			component.Init(this, unit);
			component.UpdateUnitView();
		}
		return gameObject;
	}

	public void RemoveUnitItem(UserUnit unit, Dictionary<string, UnitItemController> itemControllers)
	{
		if (itemControllers.ContainsKey(unit.ID))
		{
			GameObject obj = itemControllers[unit.ID].gameObject;
			Object.Destroy(obj);
			itemControllers.Remove(unit.ID);
		}
		int uiItemsCount = 0;
		foreach (UnitItemController value in itemControllers.Values)
		{
			if ((bool)value)
			{
				PositionUIItem(value.gameObject, ref uiItemsCount);
			}
		}
		UpdateContentLength(uiItemsCount);
	}

	private void PositionUIItem(GameObject uiTempItem, ref int uiItemsCount)
	{
		Vector3 localPosition = Vector3.zero;
		int num = uiItemsCount % 2;
		localPosition = new Vector3(xOffset + (float)(uiItemsCount / 2) * uiItemWidth, yOffset - uiItemHeight * (float)num + listContentParent.transform.localPosition.y, 0f);
		uiItemsCount++;
		uiTempItem.transform.localPosition = localPosition;
	}

	public void SetUnitState(bool state, UnitItemController unitController, UserTeam playerTeam)
	{
		if (unitController.State != state)
		{
			unitController.ToggleState();
			if (playerTeam != null && (bool)bottomBarController)
			{
				bottomBarController.UpdateUnitIcons(playerTeam);
			}
		}
	}

	public void UnitSelectedOnHUD(UnitItemController unitItemController)
	{
		bool flag = !controller.EmptyUnitsAvailable() && !unitItemController.State;
		popUpController.ConfigurePopUp(unitItemController, unitItemController.State, !flag);
		popUpController.ShowPopUp();
	}

	public void UnitSelectedOnPopUp(UnitItemController unitItemController)
	{
		controller.UnitSelected(unitItemController);
		popUpController.HidePopUp();
	}

	public void UnitPromotedOnHUD(UnitItemController unitItemController)
	{
		UserUnit unit = unitItemController.unit;
		previousPromotionData = new PromotionLocalData(unitItemController.unit.AssetBundleID, unit.StartingHealth, unit.level, unit.partialLevel, unit.RollValues, unit.RollTypes, unit.GetUpgradePrice(), unit.UnitDataModel.GetLevel(unit.level - 1));
		controller.UnitPromoted(unitItemController, UnitPromotedSuccesfully);
	}

	public void ClearAllBottomBarUnits()
	{
		bottomBarController.ClearAllUnitIcons();
	}

	private void UnitPromotedSuccesfully(UnitItemController unitItemController)
	{
		UserUnit unit = unitItemController.unit;
		popUpController.SetPromoteButtonState(false);
		PromotionLocalData currentPromotionData = new PromotionLocalData(unitItemController.unit.AssetBundleID, unit.StartingHealth, unit.level, unit.partialLevel, unit.RollValues, unit.RollTypes, unit.GetUpgradePrice(), unit.UnitDataModel.GetLevel(unit.level - 1));
		StartCoroutine(UnitPromoteSuccessfullyAnimation(previousPromotionData, currentPromotionData, unitItemController));
	}

	private IEnumerator UnitPromoteSuccessfullyAnimation(PromotionLocalData previousPromotionData, PromotionLocalData currentPromotionData, UnitItemController unitItemController)
	{
		if ((bool)promoteViewController)
		{
			yield return StartCoroutine(promoteViewController.PlayPromoteEffect(previousPromotionData, currentPromotionData, unitItemController.unit));
		}
		popUpController.UpdatePromoteButton();
		unitItemController.UpdateUnitView();
	}
}
