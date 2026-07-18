using System.Collections;
using UnityEngine;

public class EventMultiTeamController : MonoBehaviour
{
	public static string LAST_CLAIMED_REPORT_EVENT_ID = "multi_team_last_claimed_report_event_id";

	[SerializeField]
	private float checkFrequency = 1f;

	[SerializeField]
	private EventMultiTeamTimer eventMultiTeamTimer;

	private UserProfile userProfile;

	private EventDataModel activeEvent;

	private ServerMultiteamReport lastServerReport;

	private bool lastReportClaimed;

	private void Start()
	{
		if (!Constants.MultiTeamReportsEnabled)
		{
			base.gameObject.SetActive(false);
			return;
		}
		Singleton<InitializationManager>.instance.ExecuteIfStateEquals(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		userProfile = UserProfile.player;
		activeEvent = userProfile.GetActiveOnCooldownEvent();
		bool flag = true;
		flag &= userProfile.divisionInt >= Constants.MultiTeamReportMinimumTier;
		if (!(flag & (activeEvent != null)))
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			StartCoroutine(InitCoroutine());
		}
	}

	private IEnumerator InitCoroutine()
	{
		yield return StartCoroutine(CheckMultiTeamReports());
		if ((bool)eventMultiTeamTimer)
		{
			eventMultiTeamTimer.Init();
		}
		StartCoroutine(ScheduledReportCheck());
	}

	private IEnumerator CheckMultiTeamReports()
	{
		UserProfile userProfile = UserProfile.player;
		activeEvent = userProfile.GetActiveOnCooldownEvent();
		if (activeEvent == null)
		{
			yield break;
		}
		bool multiTeamReportFetched = false;
		if (!activeEvent.IsActive && activeEvent.IsOnCooldown)
		{
			Singleton<SessionManager>.instance.GetEventReport(activeEvent.id, delegate(ServerMultiteamReport serverMultiteamReport)
			{
				lastServerReport = serverMultiteamReport;
				multiTeamReportFetched = true;
			});
			while (!multiTeamReportFetched)
			{
				yield return 0;
			}
		}
		else if (userProfile.userMultiTeamReport == null || userProfile.userMultiTeamReport.IsClaimable)
		{
			Singleton<SessionManager>.instance.UpdateEventReports(activeEvent.id, delegate(ServerMultiteamReport serverMultiteamReport)
			{
				lastServerReport = serverMultiteamReport;
				multiTeamReportFetched = true;
			});
			while (!multiTeamReportFetched)
			{
				yield return 0;
			}
		}
		ProcessServerResponse();
	}

	private void ProcessServerResponse()
	{
		if (lastServerReport == null)
		{
			Log.Debug("Server has returned a null EventMultiTeamReport");
			return;
		}
		UserProfile player = UserProfile.player;
		player.userMultiTeamReport = lastServerReport.userMultiTeamReport;
		if (!lastServerReport.IsFinalReport)
		{
			ClaimedReportData lastClaimedReport = player.lastClaimedReport;
			if (lastClaimedReport == null || !lastClaimedReport.eventId.Equals(activeEvent.id))
			{
				PopupManager.ShowPopup(PopupDataModel.EventMultiTeamInitial(ShowMultiTeamReport));
			}
			else if (lastServerReport.claimedReportData.IsMoreRecentThan(lastClaimedReport))
			{
				ShowMultiTeamReport();
			}
			return;
		}
		int num = 0;
		KeyValueStorage keyValueStorage = KeyValueStorage.Instance(KeyValueStorage.Storage.USER_PROFILE);
		if (keyValueStorage == null || player == null || activeEvent == null)
		{
			return;
		}
		if (keyValueStorage.ContainsKey(LAST_CLAIMED_REPORT_EVENT_ID))
		{
			num = keyValueStorage.GetValue<int>(LAST_CLAIMED_REPORT_EVENT_ID);
		}
		if (num < int.Parse(activeEvent.id))
		{
			keyValueStorage.SetValue(LAST_CLAIMED_REPORT_EVENT_ID, int.Parse(activeEvent.id));
			if (player.userMultiTeamReport != null)
			{
				PopupManager.ShowPopup(PopupDataModel.EventMultiTeamFinal(player.userMultiTeamReport, null));
			}
		}
	}

	private void ShowMultiTeamReport()
	{
		UserProfile player = UserProfile.player;
		player.lastClaimedReport = lastServerReport.claimedReportData;
		UserProfile.player.userMultiTeamReport.CalculateEventPoints();
		PopupManager.ShowPopup(PopupDataModel.EventMultiTeamReport(UserProfile.player.userMultiTeamReport.LastAvailableReport, null));
		lastReportClaimed = ReportIsEnding();
		if (!lastReportClaimed && (bool)eventMultiTeamTimer)
		{
			eventMultiTeamTimer.Init();
		}
	}

	private IEnumerator ScheduledReportCheck()
	{
		UserProfile userProfile = UserProfile.player;
		while (true)
		{
			if (userProfile.userMultiTeamReport != null && activeEvent.IsActive && !lastReportClaimed && userProfile.userMultiTeamReport.IsClaimable)
			{
				yield return StartCoroutine(CheckMultiTeamReports());
			}
			yield return new WaitForSeconds(checkFrequency);
			userProfile = UserProfile.player;
		}
	}

	private bool ReportIsEnding()
	{
		if (activeEvent == null)
		{
			return true;
		}
		return NonUnitySingleton<TimeManager>.instance.IsTimestampInPast(activeEvent.DateEndTimeStamp - Constants.EventReportLengthInSeconds * 1000);
	}
}
