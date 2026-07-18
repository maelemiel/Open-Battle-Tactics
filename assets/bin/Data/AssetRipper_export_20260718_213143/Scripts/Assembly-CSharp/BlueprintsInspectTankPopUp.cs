using UnityEngine;

public class BlueprintsInspectTankPopUp : PopupController
{
	[SerializeField]
	private UnitInfoView unitInfoViewBaseLevel;

	[SerializeField]
	private UnitInfoView unitInfoViewMaxLevel;

	[SerializeField]
	private tk2dUIItem buildAgainButton;

	protected override void Start()
	{
		base.Start();
		UnitDataModel unitDataModel = (UnitDataModel)model.payload;
		if (unitDataModel != null)
		{
			if ((bool)unitInfoViewBaseLevel)
			{
				unitInfoViewBaseLevel.ConfigureUnitView(unitDataModel, 1);
			}
			if ((bool)unitInfoViewMaxLevel)
			{
				unitInfoViewMaxLevel.ConfigureUnitView(unitDataModel);
			}
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}

	private void BuildUnitButton()
	{
		PopupManager.ShowPopup(PopupDataModel.BuildUnitPopUp((UnitDataModel)model.payload, ClosePopUpButton));
	}
}
