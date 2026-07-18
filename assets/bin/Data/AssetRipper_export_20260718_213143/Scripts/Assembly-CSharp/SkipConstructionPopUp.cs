using System.Collections;
using UnityEngine;

public class SkipConstructionPopUp : PopupController
{
	[SerializeField]
	private UnitInfoView unitInfoViewCurrentLevel;

	[SerializeField]
	private tk2dTextMesh researchText;

	private UserResearcher researcher;

	protected override void Start()
	{
		base.Start();
		researcher = (UserResearcher)model.payload;
		if (researcher != null)
		{
			if ((bool)unitInfoViewCurrentLevel)
			{
				UnitDataModel single = UnitDataModel.GetSingle(researcher.itemID);
				unitInfoViewCurrentLevel.ConfigureUnitView(single, 1);
			}
			StartCoroutine(UpdateLoop());
		}
	}

	private void OnSkipBuildPressed()
	{
		if (!researcher.CanClaim)
		{
			PopupManager.ShowPopup(PopupDataModel.SkipWaitPopup(researcher, "ui_skip_build_tank_title".Localize("SKIP"), "ui_skip_build_tank_message".Localize("Speed up tank construction???"), OnBuildSkipped));
			return;
		}
		Log.Warning("Unit being claimed. Cancelling user input", base.gameObject);
		Close();
	}

	private void OnBuildSkipped()
	{
		OnRightButton();
		Close();
	}

	protected override void Update()
	{
		base.Update();
		if (researcher != null)
		{
			researchText.text = NonUnitySingleton<TimeManager>.instance.GetCountdownString(researcher.finishTime, true);
		}
	}

	private IEnumerator UpdateLoop()
	{
		while (!researcher.CanClaim)
		{
			yield return new WaitForSeconds(1f);
		}
		OnCloseButton();
	}
}
