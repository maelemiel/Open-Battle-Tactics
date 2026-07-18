using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardRewardsPopUpController : PopupController
{
	[SerializeField]
	private PriceLabelController priceLabelController;

	[SerializeField]
	private tk2dTextMesh finalRankText;

	[SerializeField]
	private tk2dTextMesh finalRatingText;

	[SerializeField]
	private GameObject[] winnerContainers;

	[SerializeField]
	private PrefabProxy[] winnerTierBadgeProxies;

	[SerializeField]
	private PrefabProxy[] winnerTankPictureProxies;

	[SerializeField]
	private tk2dTextMesh[] winnerPlayerNames;

	[SerializeField]
	private tk2dTextMesh[] winnerPVPRatings;

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
		if ((bool)_title)
		{
			LeaderboardsDataModel single = NonUnitySingleton<DMAccessManager>.instance.GetSingle<LeaderboardsDataModel>(leaderboardRewardsDataModel.rewardResponse.leaderboardId);
			_title.text = string.Format("ui_leaderboards_resultslastleaderboard".Localize("{0} LEADERBOARD - {1} WEEK'S WINNERS"), single.Title, single.groupId);
		}
		finalRankText.text = string.Format("ui_leaderboards_resultsfinalrank".Localize("Your position: #{0}"), leaderboardRewardsDataModel.rank);
		finalRatingText.text = UserProfile.player.pvpRating.ToString();
		priceLabelController.gameObject.SetActive(true);
		priceLabelController.ConfigurePriceLabel(leaderboardRewardsDataModel.leaderboardRewards);
		List<LeaderboardEntryData> list = (List<LeaderboardEntryData>)leaderboardRewardsDataModel.rewardResponse.topRanked;
		list.Sort((LeaderboardEntryData item1, LeaderboardEntryData item2) => item1.rank - item2.rank);
		for (int num = 0; num < winnerContainers.Length; num++)
		{
			if (num < list.Count)
			{
				winnerContainers[num].SetActive(true);
				LeaderboardEntryData leaderboardEntryData = list[num];
				StartCoroutine(SetBadge(winnerTierBadgeProxies[num], leaderboardEntryData.tier));
				if (leaderboardEntryData.tankID != null)
				{
					StartCoroutine(SetTank(winnerTankPictureProxies[num], leaderboardEntryData.tankID, leaderboardEntryData.tankLevel));
				}
				else
				{
					StartCoroutine(SetTank(winnerTankPictureProxies[num], "11001", 1));
				}
				winnerPlayerNames[num].text = leaderboardEntryData.playerName;
				winnerPVPRatings[num].text = leaderboardEntryData.pvpRating.ToString();
			}
			else
			{
				winnerContainers[num].SetActive(false);
			}
		}
	}

	public IEnumerator SetBadge(PrefabProxy badgeProxy, string divisionId)
	{
		ProgressionDivisionDataModel currentDivision = NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(divisionId);
		if (currentDivision != null)
		{
			yield return StartCoroutine(badgeProxy.ChangeAssetCoroutine(currentDivision.BadgeLinkage));
		}
	}

	public IEnumerator SetTank(PrefabProxy tankProxy, string unitId, int level)
	{
		UnitDataModel currentTank = UnitDataModel.GetSingle(unitId);
		if (currentTank != null)
		{
			yield return StartCoroutine(tankProxy.ChangeAssetCoroutine("Prefab.prefab", currentTank.Levels[level - 1].assetBundleId));
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
