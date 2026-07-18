using UnityEngine;

public class AbilitiesInspectPopUpController : PopupController
{
	[SerializeField]
	private tk2dTextMesh abilityNameLabel;

	[SerializeField]
	private tk2dTextMesh abilityNameDescription;

	[SerializeField]
	private PrefabProxy abilityIconProxy;

	[SerializeField]
	private tk2dSprite abilityIcon;

	protected override void Start()
	{
		base.Start();
		AbilityDataModel abilityDataModel = (AbilityDataModel)model.payload;
		if (abilityDataModel != null)
		{
			if ((bool)abilityNameLabel)
			{
				abilityNameLabel.text = abilityDataModel.name;
			}
			if ((bool)abilityNameDescription)
			{
				abilityNameDescription.text = abilityDataModel.description;
			}
			if ((bool)abilityIcon)
			{
				abilityIcon.SetSprite(abilityDataModel.ButtonIconArtName);
			}
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
