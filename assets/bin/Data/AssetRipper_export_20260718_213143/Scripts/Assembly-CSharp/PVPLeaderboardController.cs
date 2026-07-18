using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class PVPLeaderboardController : AbstractLeaderboardController
{
	[SerializeField]
	private tk2dTextMesh leaderboardTitleText;

	[SerializeField]
	private tk2dTextMesh leaderboardWeekText;

	[SerializeField]
	private tk2dSprite leaderboardTitleImage;

	[SerializeField]
	private GameObject rewardsGameObject;

	[SerializeField]
	private GameObject countDownGameObject;

	[SerializeField]
	private GameObject endRewardNotifGameObject;

	public override void Init(bool finishTimer = false)
	{
		if (!Constants.PvpRankingRewardsEnable)
		{
			if ((bool)rewardsGameObject)
			{
				rewardsGameObject.SetActive(false);
			}
			if ((bool)countDownGameObject)
			{
				countDownGameObject.SetActive(false);
			}
			if ((bool)endRewardNotifGameObject)
			{
				endRewardNotifGameObject.SetActive(true);
			}
		}
		myPositionEntries = new List<LeaderboardEntryData>();
		Singleton<SessionManager>.instance.CheckActiveLeaderboards(delegate(List<SessionManager.LeaderboardRewardResponse> rewards, List<string> activeLeaderboards)
		{
			if (!(this == null))
			{
				sacLeaderboard.DataSource = null;
				StartCoroutine(ProcessActiveLeaderboardResponse(rewards, activeLeaderboards, delegate
				{
					initialized = true;
				}));
			}
		});
	}

	public override void CheckRewards()
	{
		Singleton<SessionManager>.instance.CheckActiveLeaderboards(delegate(List<SessionManager.LeaderboardRewardResponse> eventRewards, List<string> activeEventLeaderboards)
		{
			CheckRewards(eventRewards, RewardsPopupPackage.Type.PvpAllTimeLeaderboard);
		});
	}

	private IEnumerator ProcessActiveLeaderboardResponse(List<SessionManager.LeaderboardRewardResponse> rewards, List<string> activeLeaderboards, Action cb = null)
	{
		CheckRewards(rewards, RewardsPopupPackage.Type.PvpAllTimeLeaderboard);
		yield return new WaitForEndOfFrame();
		while (receivingRewards)
		{
			yield return new WaitForEndOfFrame();
		}
		ToggleLeaderBoards(activeLeaderboards);
		if (cb != null)
		{
			cb();
		}
	}

	private void ToggleLeaderBoards(List<string> activeLeaderboards)
	{
		if (activeLeaderboards != null)
		{
			UserProfile player = UserProfile.player;
			IEnumerable<string> source = player.activeLeaderboards.Except(activeLeaderboards);
			player.activeLeaderboards = activeLeaderboards;
			if (source.Count() > 0)
			{
				PopupManager.ShowPopup(PopupDataModel.LeaderboardEntry(UserProfile.player.activeLeaderboards[0]));
			}
		}
		if (UserProfile.player.activeLeaderboards.Count <= 0)
		{
			return;
		}
		UpdateActiveLeaderboardDisplay();
		Singleton<SessionManager>.instance.GetLeaderboardSelfEntries(delegate(List<LeaderboardEntryData> lbData, bool success)
		{
			if (!(this == null))
			{
				myPositionEntries = lbData;
				((List<LeaderboardEntryData>)myPositionEntries).Sort((LeaderboardEntryData item1, LeaderboardEntryData item2) => item1.rank - item2.rank);
				ToggleMyLeaderboard();
				for (int num = 0; num < myPositionEntries.Count; num++)
				{
					LeaderboardEntryData leaderboardEntryData = (LeaderboardEntryData)myPositionEntries[num];
					if (leaderboardEntryData.userId == UserProfile.player.id)
					{
						Reporting.ViewedLeaderboard(leaderboardEntryData.rank);
						break;
					}
				}
				if ((bool)loadingIcon)
				{
					loadingIcon.gameObject.SetActive(false);
				}
			}
		});
		Singleton<SessionManager>.instance.GetLeaderboardTopEntries(delegate(List<LeaderboardEntryData> lbData, bool success)
		{
			if (!(this == null))
			{
				topEntries = lbData;
				((List<LeaderboardEntryData>)topEntries).Sort((LeaderboardEntryData item1, LeaderboardEntryData item2) => item1.rank - item2.rank);
			}
		});
	}

	private void UpdateActiveLeaderboardDisplay()
	{
		LeaderboardsDataModel single = LeaderboardsDataModel.GetSingle(UserProfile.player.activeLeaderboards[0]);
		string dateEnd = single.dateEnd;
		endDate = DateTime.Parse(dateEnd, CultureInfo.InvariantCulture).ToUniversalTime();
		endDateSet = true;
		List<LeaderboardRewardsDataModel> rewards = single.GetRewards();
		ConfigurePrizeLabels(rewards);
		leaderboardTitleText.text = single.Title;
		leaderboardWeekText.text = string.Format("ui_leaderboards_weeknumleaderboards".Localize("WEEK {0} LEADERBOARDS"), single.groupId);
		leaderboardTitleImage.SetSprite(single.TitleImage);
	}

	private void ToggleMyLeaderboard()
	{
		toggleMyPositionButton.IsOn = true;
		ToggleMyPlayers();
	}

	protected override bool CheckIfMyEntry(object entry)
	{
		return ((LeaderboardEntryData)entry).userId == UserProfile.player.id;
	}
}
