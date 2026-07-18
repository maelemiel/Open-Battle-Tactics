using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractLeaderboardController : MonoBehaviour
{
	[SerializeField]
	protected ScrollableAreaController sacLeaderboard;

	[SerializeField]
	protected ScrollableAreaController sacRewards;

	[SerializeField]
	protected tk2dUIToggleButton toggleMyPositionButton;

	[SerializeField]
	protected tk2dUIToggleButton toggleTopPlayersButton;

	[SerializeField]
	protected tk2dUIToggleButton toggleRewardsButton;

	[SerializeField]
	protected GameObject loadingIcon;

	[SerializeField]
	protected tk2dTextMesh leaderboardTimer;

	[SerializeField]
	protected PriceLabelController[] prizeLabels;

	[SerializeField]
	protected List<GameObject> leaderboardObjectContainer;

	protected DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	protected DateTime endDate = default(DateTime);

	protected bool endDateSet;

	protected IList myPositionEntries;

	protected IList rewardEntries;

	protected IList topEntries;

	protected bool receivingRewards;

	protected bool initialized;

	public GameObject leftAnchorPoint;

	public GameObject rightAnchorPoint;

	public Action<RewardsPopupPackage> addRewardEvent;

	public void Awake()
	{
		topEntries = new List<LeaderboardEntryData>();
		rewardEntries = new List<LeaderboardRewardsEntryData>();
	}

	public void OnDisable()
	{
		if (!toggleRewardsButton.IsOn)
		{
			toggleRewardsButton.IsOn = !toggleRewardsButton.IsOn;
		}
	}

	public void EnableLeaderboard()
	{
		if (!initialized)
		{
			Init();
		}
		sacLeaderboard.gameObject.SetActive(true);
		sacRewards.gameObject.SetActive(false);
		for (int i = 0; i < leaderboardObjectContainer.Count; i++)
		{
			leaderboardObjectContainer[i].SetActive(true);
		}
	}

	public void DisableLeaderboard()
	{
		for (int i = 0; i < leaderboardObjectContainer.Count; i++)
		{
			leaderboardObjectContainer[i].SetActive(false);
		}
	}

	public abstract void CheckRewards();

	public abstract void Init(bool finishTimer = false);

	protected abstract bool CheckIfMyEntry(object entry);

	protected void AddRewardToQueue(RewardsPopupPackage reward)
	{
		if (addRewardEvent != null)
		{
			addRewardEvent(reward);
		}
	}

	protected void CheckRewards(List<SessionManager.LeaderboardRewardResponse> rewards, RewardsPopupPackage.Type rewardType)
	{
		if (rewards.Count <= 0)
		{
			return;
		}
		ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
		for (int i = 0; i < rewards.Count; i++)
		{
			string text = string.Empty;
			SessionManager.LeaderboardRewardResponse leaderboardRewardResponse = rewards[i];
			List<int> rewardGiftPackageIds = leaderboardRewardResponse.rewardGiftPackageIds;
			for (int j = 0; j < rewardGiftPackageIds.Count; j++)
			{
				if (rewardType == RewardsPopupPackage.Type.PvpAllTimeLeaderboard)
				{
					text = text + rewardGiftPackageIds[j] + ":";
				}
				ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(rewardGiftPackageIds[j]);
				UserProfile.player.AddItems(giftPackage);
				itemCollectionDataModel.items.AddRange(giftPackage.items);
				text += giftPackage.PrintItems();
			}
			if (rewardType == RewardsPopupPackage.Type.PvpAllTimeLeaderboard)
			{
				LeaderboardsDataModel single = LeaderboardsDataModel.GetSingle(leaderboardRewardResponse.leaderboardId);
				Reporting.PvpLeaderBoard(single.Title, text);
			}
		}
		LeaderboardRewardsSceneModel rewardsModel = new LeaderboardRewardsSceneModel(itemCollectionDataModel, rewards[0].finalRank, rewards[0].finalPoints, rewards[0]);
		AddRewardToQueue(new RewardsPopupPackage(rewardsModel, rewardType));
	}

	private void Update()
	{
		if (UserProfile.player == null)
		{
			return;
		}
		if (endDateSet)
		{
			leaderboardTimer.gameObject.SetActive(true);
			DateTime serverDateTime = TimeManager.ServerDateTime;
			leaderboardTimer.text = TimeFormats.GetTimeString((long)Math.Ceiling((endDate - serverDateTime).TotalMilliseconds), TimeFormat.LEADERBOARD_COUNTDOWN);
			if ((long)Math.Ceiling((endDate - serverDateTime).TotalMilliseconds) <= 0)
			{
				Init(true);
				endDateSet = false;
			}
		}
		else if (leaderboardTimer.gameObject.activeSelf)
		{
			leaderboardTimer.gameObject.SetActive(false);
		}
	}

	public void ToggleMyPlayers()
	{
		TogglePlayers(true);
	}

	public void ToggleTopPlayers()
	{
		TogglePlayers(false);
	}

	public void TogglePlayers(bool myPlayers)
	{
		toggleRewardsButton.IsOn = true;
		sacRewards.gameObject.SetActive(false);
		sacLeaderboard.gameObject.SetActive(true);
		toggleTopPlayersButton.IsOn = myPlayers;
		toggleMyPositionButton.IsOn = !myPlayers;
		if (!myPlayers)
		{
			sacLeaderboard.DataSource = topEntries;
			sacLeaderboard.ContentToTop();
		}
		else
		{
			if (myPositionEntries == null)
			{
				return;
			}
			sacLeaderboard.DataSource = myPositionEntries;
			for (int i = 0; i < myPositionEntries.Count; i++)
			{
				if (CheckIfMyEntry(myPositionEntries[i]))
				{
					int num = Math.Max(i - sacLeaderboard.CellsInUse.Count / 2 + 2, 0);
					sacLeaderboard.ContentToPosition((float)(-num) * sacLeaderboard.cellHeight);
					break;
				}
			}
		}
	}

	protected void ConfigurePrizeLabels(List<LeaderboardRewardsDataModel> leaderboardRewards)
	{
		List<ItemCollectionDataModel> list = new List<ItemCollectionDataModel>(3);
		for (int i = 0; i < prizeLabels.Length; i++)
		{
			list.Add(new ItemCollectionDataModel());
		}
		rewardEntries.Clear();
		for (int j = 0; j < leaderboardRewards.Count; j++)
		{
			LeaderboardRewardsDataModel leaderboardRewardsDataModel = leaderboardRewards[j];
			for (int k = 0; k < prizeLabels.Length; k++)
			{
				if (k + 1 <= leaderboardRewardsDataModel.rankEnd && k + 1 >= leaderboardRewardsDataModel.rankStart)
				{
					ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(leaderboardRewardsDataModel.giftPackageId);
					list[k].items.AddRange(giftPackage.items);
				}
			}
			LeaderboardRewardsEntryData leaderboardRewardsEntryData = new LeaderboardRewardsEntryData();
			leaderboardRewardsEntryData.rankStart = leaderboardRewardsDataModel.rankStart;
			leaderboardRewardsEntryData.rankEnd = leaderboardRewardsDataModel.rankEnd;
			leaderboardRewardsEntryData.items = ItemGiftDataModel.GetGiftPackage(leaderboardRewardsDataModel.giftPackageId);
			rewardEntries.Add(leaderboardRewardsEntryData);
		}
		for (int l = 0; l < prizeLabels.Length; l++)
		{
			if (prizeLabels[l].gameObject.activeInHierarchy)
			{
				prizeLabels[l].ConfigurePriceLabel(list[l]);
			}
		}
	}

	public void ToggleRewards()
	{
		sacLeaderboard.gameObject.SetActive(toggleRewardsButton.IsOn);
		sacRewards.gameObject.SetActive(!toggleRewardsButton.IsOn);
		if (toggleRewardsButton.IsOn)
		{
			if (!toggleTopPlayersButton.IsOn)
			{
				ToggleTopPlayers();
			}
			else
			{
				ToggleMyPlayers();
			}
		}
		else
		{
			sacRewards.DataSource = rewardEntries;
			sacRewards.ContentToTop();
		}
	}
}
