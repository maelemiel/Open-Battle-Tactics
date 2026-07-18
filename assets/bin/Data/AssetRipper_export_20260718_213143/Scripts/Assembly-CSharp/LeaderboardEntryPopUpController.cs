using System;
using System.Collections;
using UnityEngine;

public class LeaderboardEntryPopUpController : PopupController
{
	[SerializeField]
	private tk2dTextMesh faceOtherPlayersText;

	[SerializeField]
	private PrefabProxy lowTierBadgeProxy;

	[SerializeField]
	private PrefabProxy highTierBadgeProxy;

	[SerializeField]
	private tk2dSprite leaderboardTitleImage;

	private string leaderboardId;

	protected override void Start()
	{
		base.Start();
		leaderboardId = (string)model.payload;
		if (leaderboardId != null)
		{
			UpdateLeaderboardEntryPopUp();
		}
	}

	public void UpdateLeaderboardEntryPopUp()
	{
		LeaderboardsDataModel single = NonUnitySingleton<DMAccessManager>.instance.GetSingle<LeaderboardsDataModel>(leaderboardId);
		if ((bool)_title)
		{
			_title.text = string.Format("ui_leaderboards_entrycurrentleaderboard".Localize("{0} LEADERBOARD - WEEK {1}"), single.Title, single.groupId);
		}
		int id = Mathf.Max(single.tierStart, 1);
		StartCoroutine(SetBadge(lowTierBadgeProxy, id.ToString()));
		StartCoroutine(SetBadge(highTierBadgeProxy, single.tierEnd.ToString()));
		leaderboardTitleImage.SetSprite(single.TitleImage);
		ProgressionDivisionDataModel single2 = ProgressionDivisionDataModel.GetSingle(id);
		ProgressionDivisionDataModel single3 = ProgressionDivisionDataModel.GetSingle(single.tierEnd);
		string arg = single2.name.Split(new char[1] { ' ' }, StringSplitOptions.None)[0];
		string arg2 = single3.name.Split(new char[1] { ' ' }, StringSplitOptions.None)[0];
		faceOtherPlayersText.text = string.Format("ui_leaderboards_faceotherplayers".Localize("Face other {0} and {1} Players to improve your rating!"), arg, arg2);
	}

	public IEnumerator SetBadge(PrefabProxy badgeProxy, string divisionId)
	{
		ProgressionDivisionDataModel currentDivision = NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(divisionId);
		if (currentDivision != null)
		{
			yield return StartCoroutine(badgeProxy.ChangeAssetCoroutine(currentDivision.BadgeLinkage));
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
