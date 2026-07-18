using UnityEngine;

public class TierRewardPopUpController : PopupController
{
	[SerializeField]
	private BlueprintsTierView blueprintsTierView;

	[SerializeField]
	private GameObject closeButton;

	protected override void Start()
	{
		base.Start();
		if ((bool)closeButton)
		{
			closeButton.SetActive(false);
		}
		ProgressionDivisionDataModel progressionDivisionDataModel = (ProgressionDivisionDataModel)model.payload;
		if (progressionDivisionDataModel == null)
		{
			progressionDivisionDataModel = new ProgressionDivisionDataModel();
			progressionDivisionDataModel.id = "20";
		}
		if ((bool)blueprintsTierView)
		{
			blueprintsTierView.ConfigureView(progressionDivisionDataModel, true, UnlockPopUp);
		}
		if ((bool)_message)
		{
			_message.text = "Tier " + progressionDivisionDataModel.name + " completed!";
		}
	}

	private void UnlockPopUp()
	{
		if ((bool)closeButton)
		{
			closeButton.SetActive(true);
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
