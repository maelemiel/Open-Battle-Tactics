using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardsSceneController : SceneController
{
	private enum LeaderboardType
	{
		PvpAllRanking = 0,
		SoloRanking = 1,
		ClubRanking = 2
	}

	[SerializeField]
	private GameObject gameCenterLeaderboardsButton;

	[SerializeField]
	private tk2dUIToggleButton myLeaderboardButton;

	[SerializeField]
	private tk2dUIToggleButton toggleClubLeaderboardsButton;

	[SerializeField]
	private tk2dUIToggleButton toggleSoloLeaderboardsButton;

	[SerializeField]
	private EventLeaderboardController eventLeaderboardController;

	[SerializeField]
	private PVPLeaderboardController pvpLeaderboardController;

	[SerializeField]
	private SoloEventLeaderboardController soloEventLeaderboardController;

	private IList myPositionEntries;

	private IList topPlayerEntries;

	private IList rewardEntries;

	private IList clubRewardEntries;

	private IList clubEntries;

	private IList clubTopEntries;

	private List<RewardsPopupPackage> rewardPackages = new List<RewardsPopupPackage>();

	private LeaderboardType currentType;

	[SerializeField]
	private GameObject leftLeaderboardAnchor;

	[SerializeField]
	private GameObject rightLeaderboardAnchor;

	public override void Awake()
	{
		_showHomeButton = true;
		_showTopBar = true;
		allowsBackButton = true;
		base.Awake();
		base.SectionTitle = "Leaderboards";
		gameCenterLeaderboardsButton.SetActive(false);
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
		}
		EventLeaderboardController obj = eventLeaderboardController;
		obj.addRewardEvent = (Action<RewardsPopupPackage>)Delegate.Combine(obj.addRewardEvent, new Action<RewardsPopupPackage>(AddGiftReceiver));
		EventLeaderboardController obj2 = eventLeaderboardController;
		obj2.kickedFromClubEvent = (Action)Delegate.Combine(obj2.kickedFromClubEvent, new Action(UserKickedFromClub));
		EventLeaderboardController obj3 = eventLeaderboardController;
		obj3.finichEvent = (Action)Delegate.Combine(obj3.finichEvent, new Action(EventEnd));
		PVPLeaderboardController pVPLeaderboardController = pvpLeaderboardController;
		pVPLeaderboardController.addRewardEvent = (Action<RewardsPopupPackage>)Delegate.Combine(pVPLeaderboardController.addRewardEvent, new Action<RewardsPopupPackage>(AddGiftReceiver));
		SoloEventLeaderboardController obj4 = soloEventLeaderboardController;
		obj4.addRewardEvent = (Action<RewardsPopupPackage>)Delegate.Combine(obj4.addRewardEvent, new Action<RewardsPopupPackage>(AddGiftReceiver));
		SoloEventLeaderboardController obj5 = soloEventLeaderboardController;
		obj5.finichEvent = (Action)Delegate.Combine(obj5.finichEvent, new Action(EventEnd));
		SoloEventLeaderboardController obj6 = soloEventLeaderboardController;
		obj6.cellExpantedEvent = (Action)Delegate.Combine(obj6.cellExpantedEvent, new Action(AdjustLeaderboardSize));
	}

	private void AddGiftReceiver(RewardsPopupPackage reward)
	{
		rewardPackages.Add(reward);
	}

	private void UserKickedFromClub()
	{
		ToggleLeaderboard(LeaderboardType.PvpAllRanking);
	}

	private void EventEnd()
	{
		float x = gameCenterLeaderboardsButton.transform.localPosition.x - (toggleClubLeaderboardsButton.gameObject.transform.localPosition.x - myLeaderboardButton.gameObject.transform.localPosition.x);
		gameCenterLeaderboardsButton.transform.localPosition = new Vector3(x, gameCenterLeaderboardsButton.transform.localPosition.y, gameCenterLeaderboardsButton.transform.localPosition.z);
		toggleClubLeaderboardsButton.gameObject.SetActive(false);
		toggleSoloLeaderboardsButton.gameObject.SetActive(false);
		ToggleMyLeaderboard();
	}

	private void AdjustLeaderboardSize()
	{
	}

	public override bool OnHomeButton()
	{
		return true;
	}

	private void Start()
	{
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		StartCoroutine(ShowAllRewardPopups());
		if (UserProfile.player.GetActiveEvent() == null)
		{
			float x = gameCenterLeaderboardsButton.transform.localPosition.x - (toggleClubLeaderboardsButton.gameObject.transform.localPosition.x - myLeaderboardButton.gameObject.transform.localPosition.x);
			gameCenterLeaderboardsButton.transform.localPosition = new Vector3(x, gameCenterLeaderboardsButton.transform.localPosition.y, gameCenterLeaderboardsButton.transform.localPosition.z);
			toggleClubLeaderboardsButton.gameObject.SetActive(false);
			toggleSoloLeaderboardsButton.gameObject.SetActive(false);
		}
		if (UserProfile.player.GetActiveEvent() != null)
		{
			if (UserProfile.player.GetActiveEvent() == null)
			{
				ToggleLeaderboard(LeaderboardType.SoloRanking);
			}
			else
			{
				ToggleLeaderboard(LeaderboardType.ClubRanking);
			}
		}
		else
		{
			ToggleLeaderboard(LeaderboardType.PvpAllRanking);
		}
		pvpLeaderboardController.CheckRewards();
		eventLeaderboardController.CheckRewards();
		soloEventLeaderboardController.CheckRewards();
		SceneTransitionManager.readyToTransitionIn = true;
	}

	private IEnumerator ShowAllRewardPopups()
	{
		while (true)
		{
			if (rewardPackages.Count > 0)
			{
				bool waitingForPopup = true;
				switch (rewardPackages[0].type)
				{
				case RewardsPopupPackage.Type.PvpAllTimeLeaderboard:
					PopupManager.ShowPopup(PopupDataModel.LeaderboardRewardsResult(rewardPackages[0].rewardsModel, "ui_leaderboard_rewards_title", delegate
					{
						waitingForPopup = false;
					}));
					break;
				case RewardsPopupPackage.Type.EventSoloLeaderboard:
					PopupManager.ShowPopup(PopupDataModel.SoloLeaderboardRewardsResult(rewardPackages[0].rewardsModel, "ui_leaderboard_solo_rewards_title", delegate
					{
						PopupManager.ShowPopup(PopupDataModel.SoloRewardResult(rewardPackages[0].rewardsModel, "ui_leaderboard_solo_rewards_title", delegate
						{
							waitingForPopup = false;
						}));
					}));
					break;
				case RewardsPopupPackage.Type.EventClubLeaderboard:
					PopupManager.ShowPopup(PopupDataModel.ClubLeaderboardRewardsResult(rewardPackages[0].rewardsModel, "ui_leaderboard_club_rewards_title", delegate
					{
						PopupManager.ShowPopup(PopupDataModel.ClubRewardResult(rewardPackages[0].rewardsModel, "ui_leaderboard_club_rewards_title", delegate
						{
							waitingForPopup = false;
						}));
					}));
					break;
				}
				while (waitingForPopup)
				{
					yield return new WaitForEndOfFrame();
				}
				rewardPackages.RemoveAt(0);
			}
			else
			{
				yield return new WaitForEndOfFrame();
			}
		}
	}

	public void OnClickGameCenter()
	{
		SocialLeaderboard.DisplayLeaderboards();
	}

	private void OnEventInfo()
	{
		PopupManager.ShowPopup(PopupDataModel.EventInfoPopUp(UserProfile.player.GetActiveEvent(), null));
	}

	private void ToggleMyLeaderboard()
	{
		if (currentType != LeaderboardType.PvpAllRanking)
		{
			ToggleLeaderboard(LeaderboardType.PvpAllRanking);
		}
	}

	private void ToggleClubLeaderboards()
	{
		if (currentType != LeaderboardType.ClubRanking)
		{
			Singleton<UserProfileManager>.instance.ShowJoinClubNotification();
			ToggleLeaderboard(LeaderboardType.ClubRanking);
		}
	}

	private void ToggleSoloLeaderboards()
	{
		if (currentType != LeaderboardType.SoloRanking)
		{
			ToggleLeaderboard(LeaderboardType.SoloRanking);
		}
	}

	private void ToggleLeaderboard(LeaderboardType type)
	{
		switch (type)
		{
		case LeaderboardType.PvpAllRanking:
			pvpLeaderboardController.EnableLeaderboard();
			eventLeaderboardController.DisableLeaderboard();
			soloEventLeaderboardController.DisableLeaderboard();
			toggleClubLeaderboardsButton.IsOn = false;
			toggleClubLeaderboardsButton.enabled = true;
			toggleSoloLeaderboardsButton.IsOn = false;
			toggleSoloLeaderboardsButton.enabled = true;
			myLeaderboardButton.IsOn = true;
			myLeaderboardButton.enabled = false;
			break;
		case LeaderboardType.ClubRanking:
			pvpLeaderboardController.DisableLeaderboard();
			eventLeaderboardController.EnableLeaderboard();
			soloEventLeaderboardController.DisableLeaderboard();
			toggleClubLeaderboardsButton.IsOn = true;
			toggleClubLeaderboardsButton.enabled = false;
			toggleSoloLeaderboardsButton.IsOn = false;
			toggleSoloLeaderboardsButton.enabled = true;
			myLeaderboardButton.IsOn = false;
			myLeaderboardButton.enabled = true;
			break;
		case LeaderboardType.SoloRanking:
			pvpLeaderboardController.DisableLeaderboard();
			eventLeaderboardController.DisableLeaderboard();
			soloEventLeaderboardController.EnableLeaderboard();
			toggleClubLeaderboardsButton.IsOn = false;
			toggleClubLeaderboardsButton.enabled = true;
			toggleSoloLeaderboardsButton.IsOn = true;
			toggleSoloLeaderboardsButton.enabled = false;
			myLeaderboardButton.IsOn = false;
			myLeaderboardButton.enabled = true;
			break;
		}
		currentType = type;
	}

	private void CheckIfIsInClub()
	{
	}
}
