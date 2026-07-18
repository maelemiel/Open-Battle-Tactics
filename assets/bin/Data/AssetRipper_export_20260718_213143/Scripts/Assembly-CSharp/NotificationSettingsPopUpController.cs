using UnityEngine;

public class NotificationSettingsPopUpController : PopupController
{
	[SerializeField]
	private tk2dUIToggleButton _tankBuildButton;

	[SerializeField]
	private tk2dUIToggleButton _tankRepairedButton;

	[SerializeField]
	private tk2dUIToggleButton _ticketRechargedButton;

	[SerializeField]
	private tk2dUIToggleButton _prizesReadyButton;

	[SerializeField]
	private tk2dUIToggleButton _clubCratesButton;

	[SerializeField]
	private tk2dUIToggleButton _eventRewardsButton;

	[SerializeField]
	private tk2dUIToggleButton _allNotificationsButton;

	[SerializeField]
	private tk2dUIToggleButton _teamReportCardsButton;

	protected override void Start()
	{
		_tankBuildButton.IsOn = UserProfile.player.preferences.NotifTanksBuild;
		_tankRepairedButton.IsOn = UserProfile.player.preferences.NotifTanksRepaired;
		_ticketRechargedButton.IsOn = UserProfile.player.preferences.NotifTicketsRecharged;
		_prizesReadyButton.IsOn = UserProfile.player.preferences.NotifPrizesReady;
		_clubCratesButton.IsOn = UserProfile.player.preferences.NotifClubCrates;
		_eventRewardsButton.IsOn = UserProfile.player.preferences.NotifEventRewards;
		_teamReportCardsButton.IsOn = UserProfile.player.preferences.NotifTeamReportCards;
		_allNotificationsButton.IsOn = _tankBuildButton.IsOn && _tankRepairedButton.IsOn && _ticketRechargedButton.IsOn && _prizesReadyButton.IsOn && _clubCratesButton.IsOn && _eventRewardsButton.IsOn && _teamReportCardsButton.IsOn;
		base.Start();
	}

	public void TankBuiltButtonPress()
	{
		CheckAllNotification();
		UserProfile.player.preferences.NotifTanksBuild = _tankBuildButton.IsOn;
		ChangeValueOnServer(UserPreferenceData.PUSH_NOTIF_TYPE.TANKS_BUILD, _tankBuildButton.IsOn);
	}

	public void TankRepairedButtonPress()
	{
		CheckAllNotification();
		UserProfile.player.preferences.NotifTanksRepaired = _tankRepairedButton.IsOn;
		ChangeValueOnServer(UserPreferenceData.PUSH_NOTIF_TYPE.TANKS_REPAIRED, _tankRepairedButton.IsOn);
	}

	public void TicketRechargedButtonPress()
	{
		CheckAllNotification();
		UserProfile.player.preferences.NotifTicketsRecharged = _ticketRechargedButton.IsOn;
		ChangeValueOnServer(UserPreferenceData.PUSH_NOTIF_TYPE.TICKET_RECHARGED, _ticketRechargedButton.IsOn);
	}

	public void PrizesReadyButtonPress()
	{
		CheckAllNotification();
		UserProfile.player.preferences.NotifPrizesReady = _prizesReadyButton.IsOn;
		ChangeValueOnServer(UserPreferenceData.PUSH_NOTIF_TYPE.PRIZES_READY, _prizesReadyButton.IsOn);
	}

	public void ClubCratesButtonPress()
	{
		CheckAllNotification();
		UserProfile.player.preferences.NotifClubCrates = _clubCratesButton.IsOn;
		ChangeValueOnServer(UserPreferenceData.PUSH_NOTIF_TYPE.CLUB_CRATES, _clubCratesButton.IsOn);
	}

	public void EventRewardsButtonPress()
	{
		CheckAllNotification();
		UserProfile.player.preferences.NotifEventRewards = _eventRewardsButton.IsOn;
		ChangeValueOnServer(UserPreferenceData.PUSH_NOTIF_TYPE.EVENT_REWARDS, _eventRewardsButton.IsOn);
	}

	public void TeamReportCardsButtonPress()
	{
		CheckAllNotification();
		UserProfile.player.preferences.NotifTeamReportCards = _teamReportCardsButton.IsOn;
		ChangeValueOnServer(UserPreferenceData.PUSH_NOTIF_TYPE.TEAM_REPORT_CARDS, _teamReportCardsButton.IsOn);
	}

	public void AllNotificationsButtonPress()
	{
		UserProfile.player.preferences.NotifAll = _allNotificationsButton.IsOn;
		UserProfile.player.preferences.NotifTanksBuild = _allNotificationsButton.IsOn;
		_tankBuildButton.IsOn = _allNotificationsButton.IsOn;
		UserProfile.player.preferences.NotifTanksRepaired = _allNotificationsButton.IsOn;
		_tankRepairedButton.IsOn = _allNotificationsButton.IsOn;
		UserProfile.player.preferences.NotifTicketsRecharged = _allNotificationsButton.IsOn;
		_ticketRechargedButton.IsOn = _allNotificationsButton.IsOn;
		UserProfile.player.preferences.NotifPrizesReady = _allNotificationsButton.IsOn;
		_prizesReadyButton.IsOn = _allNotificationsButton.IsOn;
		UserProfile.player.preferences.NotifClubCrates = _allNotificationsButton.IsOn;
		_clubCratesButton.IsOn = _allNotificationsButton.IsOn;
		UserProfile.player.preferences.NotifEventRewards = _allNotificationsButton.IsOn;
		_eventRewardsButton.IsOn = _allNotificationsButton.IsOn;
		UserProfile.player.preferences.NotifTeamReportCards = _allNotificationsButton.IsOn;
		_teamReportCardsButton.IsOn = _allNotificationsButton.IsOn;
		ChangeValueOnServer(UserPreferenceData.PUSH_NOTIF_TYPE.ALL, _allNotificationsButton.IsOn);
	}

	private void CheckAllNotification()
	{
		if (_tankBuildButton.IsOn && _tankRepairedButton.IsOn && _ticketRechargedButton.IsOn && _prizesReadyButton.IsOn && _clubCratesButton.IsOn && _eventRewardsButton.IsOn && _teamReportCardsButton.IsOn)
		{
			_allNotificationsButton.IsOn = true;
		}
		else
		{
			_allNotificationsButton.IsOn = false;
		}
	}

	private void ChangeValueOnServer(UserPreferenceData.PUSH_NOTIF_TYPE notifType, bool enable)
	{
		Singleton<SessionManager>.instance.SetPushNotifEnableStatus(notifType, enable);
	}
}
