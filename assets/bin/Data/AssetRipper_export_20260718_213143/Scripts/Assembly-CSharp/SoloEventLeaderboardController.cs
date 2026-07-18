using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class SoloEventLeaderboardController : AbstractLeaderboardController
{
	[SerializeField]
	private EventLogoController eventLogoController;

	public Action kickedFromClubEvent;

	public Action finichEvent;

	public Action cellExpantedEvent;

	public override void Init(bool finishTimer = false)
	{
		if (finishTimer)
		{
			CheckRewards();
			if (finichEvent != null)
			{
				finichEvent();
			}
			return;
		}
		Singleton<SessionManager>.instance.CheckSoloLeaderboards(delegate(List<SessionManager.LeaderboardRewardResponse> eventRewards, List<string> activeEventLeaderboards)
		{
			if (!(this == null))
			{
				StartCoroutine(ProcessActiveLeaderboardResponse(eventRewards, activeEventLeaderboards, delegate
				{
					initialized = true;
				}));
			}
		});
	}

	public override void CheckRewards()
	{
		Singleton<SessionManager>.instance.CheckSoloLeaderboards(delegate(List<SessionManager.LeaderboardRewardResponse> eventRewards, List<string> activeEventLeaderboards)
		{
			CheckRewards(eventRewards, RewardsPopupPackage.Type.EventSoloLeaderboard);
		});
	}

	private IEnumerator ProcessActiveLeaderboardResponse(List<SessionManager.LeaderboardRewardResponse> rewards, List<string> activeLeaderboards, Action cb = null)
	{
		CheckRewards(rewards, RewardsPopupPackage.Type.EventSoloLeaderboard);
		yield return new WaitForEndOfFrame();
		while (receivingRewards)
		{
			yield return new WaitForEndOfFrame();
		}
		ToggleClubLeaderboards();
		if (cb != null)
		{
			cb();
		}
	}

	private void ToggleClubLeaderboards()
	{
		StartCoroutine(eventLogoController.LoadLogoCoroutine(UserProfile.player.GetActiveEvent()));
		sacLeaderboard.DataSource = myPositionEntries;
		sacLeaderboard.ContentToTop();
		UpdateActiveEventLeaderboardDisplay();
		if (myPositionEntries == null)
		{
			Singleton<SessionManager>.instance.GetSoloLeaderboardSelfEntries(delegate(List<SoloLeaderboardEntryData> lbData, bool success)
			{
				if (!(this == null))
				{
					if (!success)
					{
						if (kickedFromClubEvent != null)
						{
							kickedFromClubEvent();
						}
					}
					else
					{
						myPositionEntries = lbData;
						((List<SoloLeaderboardEntryData>)myPositionEntries).Sort((SoloLeaderboardEntryData item1, SoloLeaderboardEntryData item2) => item1.rank - item2.rank);
						if ((bool)loadingIcon)
						{
							loadingIcon.gameObject.SetActive(false);
						}
						if (sacLeaderboard.gameObject.activeInHierarchy)
						{
							sacLeaderboard.DataSource = myPositionEntries;
							sacLeaderboard.ContentToTop();
							if (UserProfile.player.GetActiveEvent() != null)
							{
								ToggleMyPlayers();
							}
						}
					}
				}
			});
			Singleton<SessionManager>.instance.GetSoloLeaderboardTopEntries(delegate(List<SoloLeaderboardEntryData> lbData, bool success)
			{
				if (!(this == null))
				{
					if (!success)
					{
						if (kickedFromClubEvent != null)
						{
							kickedFromClubEvent();
						}
					}
					else
					{
						topEntries = lbData;
						((List<SoloLeaderboardEntryData>)topEntries).Sort((SoloLeaderboardEntryData item1, SoloLeaderboardEntryData item2) => item1.rank - item2.rank);
					}
				}
			});
		}
		else if ((bool)loadingIcon)
		{
			loadingIcon.gameObject.SetActive(false);
		}
	}

	private void UpdateActiveEventLeaderboardDisplay()
	{
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		if (activeEvent != null)
		{
			string dateEnd = activeEvent.dateEnd;
			endDate = DateTime.Parse(dateEnd, CultureInfo.InvariantCulture).ToUniversalTime();
			endDateSet = true;
			List<LeaderboardRewardsDataModel> soloRewards = activeEvent.GetSoloRewards();
			ConfigurePrizeLabels(soloRewards);
		}
	}

	protected override bool CheckIfMyEntry(object entry)
	{
		return ((SoloLeaderboardEntryData)entry).userId == UserProfile.player.id;
	}
}
