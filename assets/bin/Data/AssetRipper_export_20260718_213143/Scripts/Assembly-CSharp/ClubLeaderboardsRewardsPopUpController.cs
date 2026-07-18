using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubLeaderboardsRewardsPopUpController : PopupController
{
	[SerializeField]
	private tk2dTextMesh yourClubName;

	[SerializeField]
	private tk2dTextMesh yourClubRank;

	[SerializeField]
	private tk2dTextMesh yourClubWinsCount;

	[SerializeField]
	private PrefabProxy yourBadge;

	[SerializeField]
	private GameObject[] winnerContainers;

	[SerializeField]
	private PrefabProxy[] winnerClubBadgeProxies;

	[SerializeField]
	private tk2dTextMesh[] winnerClubNames;

	[SerializeField]
	private tk2dTextMesh[] winnerWinsCount;

	[SerializeField]
	private EventCoinSpriteController[] eventCoinsSprites;

	private LeaderboardRewardsSceneModel leaderboardRewardsDataModel;

	protected override void Start()
	{
		base.Start();
		leaderboardRewardsDataModel = (LeaderboardRewardsSceneModel)model.payload;
		if (leaderboardRewardsDataModel != null)
		{
			UpdateLeaderboardRewardsPopUp();
		}
	}

	public void UpdateLeaderboardRewardsPopUp()
	{
		EventDataModel single = NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventDataModel>(leaderboardRewardsDataModel.rewardResponse.leaderboardId);
		if ((bool)_title)
		{
			_title.text = string.Format("ui_clubleaderboards_resultslastleaderboard".Localize("{0} WINNERS"), single.name.Localize());
		}
		List<EventLeaderboardEntryData> list = (List<EventLeaderboardEntryData>)leaderboardRewardsDataModel.rewardResponse.topRanked;
		list.Sort((EventLeaderboardEntryData item1, EventLeaderboardEntryData item2) => item1.rank - item2.rank);
		for (int num = 0; num < winnerContainers.Length; num++)
		{
			if (num < list.Count)
			{
				winnerContainers[num].SetActive(true);
				EventLeaderboardEntryData eventLeaderboardEntryData = list[num];
				StartCoroutine(SetClubBage(winnerClubBadgeProxies[num], eventLeaderboardEntryData.badgeId));
				winnerClubNames[num].text = eventLeaderboardEntryData.clubName;
				winnerWinsCount[num].text = string.Format("ui_event_rewards_club_wins".Localize("{0} POINTS"), eventLeaderboardEntryData.winCount);
			}
			else
			{
				winnerContainers[num].SetActive(false);
			}
		}
		for (int num2 = 0; num2 < eventCoinsSprites.Length; num2++)
		{
			eventCoinsSprites[num2].Init(single);
		}
		yourClubName.text = UserProfile.player.userClub.name;
		yourClubRank.text = leaderboardRewardsDataModel.rank.ToString();
		yourClubWinsCount.text = string.Format("ui_event_rewards_club_wins".Localize("{0} POINTS"), leaderboardRewardsDataModel.points);
		StartCoroutine(SetClubBage(yourBadge, UserProfile.player.userClub.TeamBadgeAssetLinkage));
	}

	public IEnumerator SetClubBage(PrefabProxy badgeProxy, int clubBadgeId)
	{
		AssetLinkageDataModel clubBadgeAssetLinkage = AssetLinkageDataModel.GetSingle(clubBadgeId.ToString());
		yield return StartCoroutine(SetClubBage(badgeProxy, clubBadgeAssetLinkage));
	}

	public IEnumerator SetClubBage(PrefabProxy badgeProxy, AssetLinkageDataModel clubBadgeAssetLinkage)
	{
		if (clubBadgeAssetLinkage != null)
		{
			yield return StartCoroutine(badgeProxy.ChangeAssetCoroutine(clubBadgeAssetLinkage));
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
