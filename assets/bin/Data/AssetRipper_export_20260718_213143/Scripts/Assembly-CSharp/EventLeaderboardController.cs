using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class EventLeaderboardController : AbstractLeaderboardController
{
	[SerializeField]
	private EventLogoController eventLogoController;

	[SerializeField]
	private MyClubView clubView;

	public Action kickedFromClubEvent;

	public Action finichEvent;

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
		if ((bool)clubView)
		{
			clubView.gameObject.SetActive(false);
		}
		Singleton<SessionManager>.instance.CheckActiveEventLeaderboards(delegate(List<SessionManager.LeaderboardRewardResponse> eventRewards, List<string> activeEventLeaderboards)
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
		Singleton<SessionManager>.instance.CheckActiveEventLeaderboards(delegate(List<SessionManager.LeaderboardRewardResponse> eventRewards, List<string> activeEventLeaderboards)
		{
			CheckRewards(eventRewards, RewardsPopupPackage.Type.EventClubLeaderboard);
		});
	}

	public void HideClanView()
	{
		clubView.gameObject.SetActive(false);
	}

	private IEnumerator ProcessActiveLeaderboardResponse(List<SessionManager.LeaderboardRewardResponse> rewards, List<string> activeLeaderboards, Action cb = null)
	{
		CheckRewards(rewards, RewardsPopupPackage.Type.EventClubLeaderboard);
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
			Singleton<SessionManager>.instance.GetEventLeaderboardSelfEntries(delegate(List<EventLeaderboardEntryData> lbData, bool success)
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
						((List<EventLeaderboardEntryData>)myPositionEntries).Sort((EventLeaderboardEntryData item1, EventLeaderboardEntryData item2) => item1.rank - item2.rank);
						for (int num = 0; num < myPositionEntries.Count; num++)
						{
							EventLeaderboardEntryData eventLeaderboardEntryData = (EventLeaderboardEntryData)myPositionEntries[num];
							eventLeaderboardEntryData.clubView = clubView;
							if (eventLeaderboardEntryData.clubId == UserProfile.player.clubID)
							{
								Reporting.ViewedClubLeaderboard(eventLeaderboardEntryData.rank);
							}
						}
						if ((bool)loadingIcon)
						{
							loadingIcon.gameObject.SetActive(false);
						}
						if (sacLeaderboard.gameObject.activeInHierarchy)
						{
							sacLeaderboard.DataSource = myPositionEntries;
							sacLeaderboard.ContentToTop();
							ToggleMyPlayers();
						}
					}
				}
			});
			Singleton<SessionManager>.instance.GetEventLeaderboardTopEntries(delegate(List<EventLeaderboardEntryData> lbData, bool success)
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
						((List<EventLeaderboardEntryData>)topEntries).Sort((EventLeaderboardEntryData item1, EventLeaderboardEntryData item2) => item1.rank - item2.rank);
						for (int num = 0; num < topEntries.Count; num++)
						{
							EventLeaderboardEntryData eventLeaderboardEntryData = (EventLeaderboardEntryData)topEntries[num];
							eventLeaderboardEntryData.clubView = clubView;
						}
					}
				}
			});
		}
		else if ((bool)loadingIcon)
		{
			loadingIcon.gameObject.SetActive(false);
		}
		toggleMyPositionButton.IsOn = true;
		toggleTopPlayersButton.IsOn = false;
	}

	private void UpdateActiveEventLeaderboardDisplay()
	{
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		if (activeEvent != null)
		{
			string dateEnd = activeEvent.dateEnd;
			endDate = DateTime.Parse(dateEnd, CultureInfo.InvariantCulture).ToUniversalTime();
			endDateSet = true;
			List<LeaderboardRewardsDataModel> rewards = activeEvent.GetRewards();
			ConfigurePrizeLabels(rewards);
		}
	}

	protected override bool CheckIfMyEntry(object entry)
	{
		return ((EventLeaderboardEntryData)entry).clubId == UserProfile.player.clubID;
	}
}
