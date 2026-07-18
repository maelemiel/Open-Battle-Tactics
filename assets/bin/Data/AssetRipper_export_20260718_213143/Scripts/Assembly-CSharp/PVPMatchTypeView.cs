using UnityEngine;

public class PVPMatchTypeView : BaseMatchTypeView
{
	[SerializeField]
	private PrefabProxy badgePrefabProxy;

	[SerializeField]
	private tk2dTextMesh tierNameLabel;

	[SerializeField]
	private tk2dUIProgressBar tierProgressBar;

	[SerializeField]
	private GameObject descriptionGameobject;

	[SerializeField]
	private GameObject[] childViews;

	[SerializeField]
	private EventLogoController eventLogoController;

	[SerializeField]
	private EventBackgroundController eventBackgroundController;

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
		if (dataObject == null)
		{
			return;
		}
		UserProfile player = UserProfile.player;
		base.SetupMatchTypeView();
		ProgressionDivisionDataModel progressionDivisionDataModel = (ProgressionDivisionDataModel)dataObject;
		if (progressionDivisionDataModel != null)
		{
			if ((bool)badgePrefabProxy)
			{
				StartCoroutine(badgePrefabProxy.ChangeAssetCoroutine(progressionDivisionDataModel.BadgeLinkage));
			}
			if ((bool)tierNameLabel)
			{
				tierNameLabel.text = progressionDivisionDataModel.name;
			}
			if (UserProfile.player.GetActiveEvent() != null && UserProfile.player.divisionInt >= Constants.MinTierEventContent && (bool)descriptionGameobject)
			{
				descriptionGameobject.SetActive(false);
			}
			float value = (float)player.points / (float)player.CurrentDivision.totalPointToPromotionSeries;
			if ((bool)tierProgressBar)
			{
				tierProgressBar.Value = Mathf.Clamp01(value);
			}
		}
	}
}
