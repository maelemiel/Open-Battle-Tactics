using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoloLeaderboardRewardsPopUpController : PopupController
{
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

	[SerializeField]
	private EventCoinSpriteController[] eventCoinsSprites;

	private LeaderboardRewardsSceneModel leaderboardRewardsDataModel;

	protected override void Start()
	{
		base.Start();
		leaderboardRewardsDataModel = (LeaderboardRewardsSceneModel)model.payload;
		if (leaderboardRewardsDataModel == null)
		{
			Log.Warning("leaderboardRewardsDataModel null");
		}
		else
		{
			UpdateLeaderboardRewardsPopUp();
		}
	}

	public void UpdateLeaderboardRewardsPopUp()
	{
		EventDataModel single = NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventDataModel>(leaderboardRewardsDataModel.rewardResponse.leaderboardId);
		if ((bool)_title)
		{
			_title.text = string.Format("ui_soloLeaderboards_resultslastleaderboard".Localize("{0} SOLO WINNERS"), single.name.Localize());
		}
		finalRankText.text = string.Format("ui_leaderboards_resultsfinalrank".Localize("Your position: #{0}"), leaderboardRewardsDataModel.rank);
		finalRatingText.text = leaderboardRewardsDataModel.points.ToString();
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
		for (int num2 = 0; num2 < eventCoinsSprites.Length; num2++)
		{
			eventCoinsSprites[num2].Init(single);
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
		if (currentTank != null && level > 0 && level <= currentTank.Levels.Count)
		{
			yield return StartCoroutine(tankProxy.ChangeAssetCoroutine("Prefab.prefab", currentTank.Levels[level - 1].assetBundleId));
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
